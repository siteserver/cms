using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageChannelTranslate : BasePageCms
    {
        public ListBox LbChannelIdFrom;
        public DropDownList DdlSiteId;
        public DropDownList DdlChannelIdTo;
		public DropDownList DdlTranslateType;
		public RadioButtonList RblIsDeleteAfterTranslate;
        public PlaceHolder PhReturn;
        public Button BtnSubmit;

		private bool[] _isLastNodeArray;

        public static string GetRedirectUrl(int siteId)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageChannelTranslate), null);
        }

        public static string GetRedirectUrl(int siteId, int channelId)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageChannelTranslate), new NameValueCollection
            {
                {"channelId", channelId.ToString()}
            });
        }

        public static string GetRedirectUrl(int siteId, int channelId, string returnUrl)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageChannelTranslate), new NameValueCollection
            {
                {"channelId", channelId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
        }

        public static string GetRedirectUrl(int siteId, string returnUrl)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageChannelTranslate), new NameValueCollection
            {
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			PageUtils.CheckRequestParameter("siteId");
            ReturnUrl = StringUtils.ValueFromUrl(AuthRequest.GetQueryString("ReturnUrl"));

            if (!HasChannelPermissions(SiteId, ConfigManager.ChannelPermissions.ContentDelete))
			{
                RblIsDeleteAfterTranslate.Visible = false;
			}

            if (IsPostBack) return;

            PhReturn.Visible = !string.IsNullOrEmpty(ReturnUrl);
            ETranslateTypeUtils.AddListItems(DdlTranslateType);
            ControlUtils.SelectSingleItem(DdlTranslateType,
                AuthRequest.IsQueryExists("ChannelIDCollection")
                    ? ETranslateTypeUtils.GetValue(ETranslateType.All)
                    : ETranslateTypeUtils.GetValue(ETranslateType.Content));

            var siteIdList = AuthRequest.AdminPermissionsImpl.GetSiteIdList();
            foreach (var psId in siteIdList)
            {
                var psInfo = SiteManager.GetSiteInfo(psId);
                var listitem = new ListItem(psInfo.SiteName, psId.ToString());
                if (psId == SiteId) listitem.Selected = true;
                DdlSiteId.Items.Add(listitem);
            }

            var channelIdStrList = new List<string>();
            if (AuthRequest.IsQueryExists("ChannelIDCollection"))
            {
                channelIdStrList = TranslateUtils.StringCollectionToStringList(AuthRequest.GetQueryString("ChannelIDCollection"));
            }

            var channelIdList = ChannelManager.GetChannelIdList(SiteId);
            var nodeCount = channelIdList.Count;
            _isLastNodeArray = new bool[nodeCount];
            foreach (var theChannelId in channelIdList)
            {
                var enabled = IsOwningChannelId(theChannelId);
                if (!enabled)
                {
                    if (!IsDescendantOwningChannelId(theChannelId)) continue;
                }
                var nodeInfo = ChannelManager.GetChannelInfo(SiteId, theChannelId);

                var value = enabled ? nodeInfo.Id.ToString() : string.Empty;
                value = nodeInfo.Additional.IsContentAddable ? value : string.Empty;

                var text = GetTitle(nodeInfo);
                var listItem = new ListItem(text, value);
                if (channelIdStrList.Contains(value))
                {
                    listItem.Selected = true;
                }
                LbChannelIdFrom.Items.Add(listItem);
                listItem = new ListItem(text, value);
                DdlChannelIdTo.Items.Add(listItem);
            }
        }

		public string GetTitle(ChannelInfo channelInfo)
		{
			var str = "";
            if (channelInfo.Id == SiteId)
			{
                channelInfo.IsLastNode = true;
			}
            if (channelInfo.IsLastNode == false)
			{
                _isLastNodeArray[channelInfo.ParentsCount] = false;
			}
			else
			{
                _isLastNodeArray[channelInfo.ParentsCount] = true;
			}
            for (var i = 0; i < channelInfo.ParentsCount; i++)
            {
                str = string.Concat(str, _isLastNodeArray[i] ? "　" : "│");
            }
		    str = string.Concat(str, channelInfo.IsLastNode ? "└" : "├");
		    str = string.Concat(str, channelInfo.ChannelName);
		    var onlyAdminId = AuthRequest.AdminPermissionsImpl.GetOnlyAdminId(SiteId, channelInfo.Id);
            var count = ContentManager.GetCount(SiteInfo, channelInfo, onlyAdminId);
            if (count != 0)
            {
                str = $"{str} ({count})";
            }
			return str;
		}

        public override void Submit_OnClick(object sender, EventArgs e)
		{
		    if (!Page.IsPostBack) return;

		    var targetChannelId = TranslateUtils.ToInt(DdlChannelIdTo.SelectedValue);

		    var targetSiteId = TranslateUtils.ToInt(DdlSiteId.SelectedValue);

            var translateType = ETranslateTypeUtils.GetEnumType(DdlTranslateType.SelectedValue);

            var channelIdStrArrayList = ControlUtils.GetSelectedListControlValueArrayList(LbChannelIdFrom);

            var channelIdList = new List<int>();//需要转移的栏目ID
            foreach (string channelIdStr in channelIdStrArrayList)
            {
                var channelId = int.Parse(channelIdStr);
                if (translateType != ETranslateType.Content)//需要转移栏目
                {
                    if (!ChannelManager.IsAncestorOrSelf(SiteId, channelId, targetChannelId))
                    {
                        channelIdList.Add(channelId);
                    }
                }

                if (translateType == ETranslateType.Content)//转移内容
                {
                    TranslateContent(channelId, targetSiteId, targetChannelId);
                }
            }

            if (translateType != ETranslateType.Content)//需要转移栏目
            {
                var channelIdListToTranslate = new List<int>(channelIdList);
                foreach (var channelId in channelIdList)
                {
                    var channelInfo = ChannelManager.GetChannelInfo(SiteId, channelId);
                    var subChannelIdList = ChannelManager.GetChannelIdList(channelInfo, EScopeType.Descendant, string.Empty, string.Empty, string.Empty);

                    if (subChannelIdList != null && subChannelIdList.Count > 0)
                    {
                        foreach (var channelIdToDelete in subChannelIdList)
                        {
                            if (channelIdListToTranslate.Contains(channelIdToDelete))
                            {
                                channelIdListToTranslate.Remove(channelIdToDelete);
                            }
                        }
                    }
                }

                var nodeInfoList = new List<ChannelInfo>();
                foreach (int channelId in channelIdListToTranslate)
                {
                    var nodeInfo = ChannelManager.GetChannelInfo(SiteId, channelId);
                    nodeInfoList.Add(nodeInfo);
                }

                TranslateChannelAndContent(nodeInfoList, targetSiteId, targetChannelId, translateType, null, null);

                if (RblIsDeleteAfterTranslate.Visible && EBooleanUtils.Equals(RblIsDeleteAfterTranslate.SelectedValue, EBoolean.True))
                {
                    foreach (var channelId in channelIdListToTranslate)
                    {
                        try
                        {
                            DataProvider.ChannelDao.Delete(SiteId, channelId);
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                }
            }

            var builder = new StringBuilder();
            foreach (ListItem listItem in LbChannelIdFrom.Items)
            {
                if (listItem.Selected)
                {
                    builder.Append(listItem.Text).Append(",");
                }
            }
            if (builder.Length > 0)
            {
                builder.Length = builder.Length - 1;
            }
            AuthRequest.AddSiteLog(SiteId, "批量转移", $"栏目:{builder},转移后删除:{RblIsDeleteAfterTranslate.SelectedValue}");

            SuccessMessage("批量转移成功！");

            if (!string.IsNullOrEmpty(ReturnUrl))
            {
                PageUtils.Redirect(ReturnUrl);
            }
        }

		private void TranslateChannelAndContent(List<ChannelInfo> nodeInfoList, int targetSiteId, int parentId, ETranslateType translateType, List<string> nodeIndexNameList, List<string> filePathList)
		{
			if (nodeInfoList == null || nodeInfoList.Count == 0)
			{
				return;
			}

			if (nodeIndexNameList == null)
			{
                nodeIndexNameList = DataProvider.ChannelDao.GetIndexNameList(targetSiteId);
			}

            if (filePathList == null)
			{
                filePathList = DataProvider.ChannelDao.GetAllFilePathBySiteId(targetSiteId);
			}

			foreach (var oldNodeInfo in nodeInfoList)
			{
			    var nodeInfo = new ChannelInfo(oldNodeInfo)
			    {
			        SiteId = targetSiteId,
			        ParentId = parentId,
			        ChildrenCount = 0,
			        AddDate = DateTime.Now
			    };

			    if (RblIsDeleteAfterTranslate.Visible && EBooleanUtils.Equals(RblIsDeleteAfterTranslate.SelectedValue, EBoolean.True))
                {
                    nodeIndexNameList.Add(nodeInfo.IndexName);
                }
               
                else if (!string.IsNullOrEmpty(nodeInfo.IndexName) && nodeIndexNameList.IndexOf(nodeInfo.IndexName) == -1)
                {
                    nodeIndexNameList.Add(nodeInfo.IndexName);
                }
                else
                {
                    nodeInfo.IndexName = string.Empty;
                }

                if (!string.IsNullOrEmpty(nodeInfo.FilePath) && filePathList.IndexOf(nodeInfo.FilePath) == -1)
                {
                    filePathList.Add(nodeInfo.FilePath);
                }
                else
                {
                    nodeInfo.FilePath = string.Empty;
                }

                var targetChannelId = DataProvider.ChannelDao.Insert(nodeInfo);

                if (translateType == ETranslateType.All)
                {
                    TranslateContent(oldNodeInfo.Id, targetSiteId, targetChannelId);
                }

                if (targetChannelId != 0)
                {
                    //var orderByString = ETaxisTypeUtils.GetChannelOrderByString(ETaxisType.OrderByTaxis);
                    //var childrenNodeInfoList = DataProvider.ChannelDao.GetChannelInfoList(oldNodeInfo, 0, "", EScopeType.Children, orderByString);

                    var channelIdList = ChannelManager.GetChannelIdList(oldNodeInfo, EScopeType.Children, string.Empty, string.Empty, string.Empty);
                    var childrenNodeInfoList = new List<ChannelInfo>();
                    foreach (var channelId in channelIdList)
                    {
                        childrenNodeInfoList.Add(ChannelManager.GetChannelInfo(oldNodeInfo.SiteId, channelId));
                    }

                    if (channelIdList.Count > 0)
                    {
                        TranslateChannelAndContent(childrenNodeInfoList, targetSiteId, targetChannelId, translateType, nodeIndexNameList, filePathList);
                    }

                    CreateManager.CreateChannel(targetSiteId, targetChannelId);
                }
			}
		}

		private void TranslateContent(int channelId, int targetSiteId, int targetChannelId)
		{
            var tableName = ChannelManager.GetTableName(SiteInfo, channelId);

            var orderByString = ETaxisTypeUtils.GetContentOrderByString(ETaxisType.OrderByTaxis);

            var contentIdList = DataProvider.ContentDao.GetContentIdListChecked(tableName, channelId, orderByString);
		    var translateType = RblIsDeleteAfterTranslate.Visible &&
		                        EBooleanUtils.Equals(RblIsDeleteAfterTranslate.SelectedValue, EBoolean.True)
		        ? ETranslateContentType.Cut
		        : ETranslateContentType.Copy;

            foreach (var contentId in contentIdList)
			{
                ContentUtility.Translate(SiteInfo, channelId, contentId, targetSiteId, targetChannelId, translateType);
			}
		}

		public void DdlSiteId_OnSelectedIndexChanged(object sender, EventArgs e)
		{
			var psId = int.Parse(DdlSiteId.SelectedValue);

            DdlChannelIdTo.Items.Clear();

			var channelIdList = ChannelManager.GetChannelIdList(psId);
            var nodeCount = channelIdList.Count;
			_isLastNodeArray = new bool[nodeCount];
            foreach (var theChannelId in channelIdList)
			{
                var nodeInfo = ChannelManager.GetChannelInfo(psId, theChannelId);
                var value = IsOwningChannelId(nodeInfo.Id) ? nodeInfo.Id.ToString() : "";
                value = (nodeInfo.Additional.IsContentAddable) ? value : "";
                var listitem = new ListItem(GetTitle(nodeInfo), value);
                DdlChannelIdTo.Items.Add(listitem);
			}
		}

        public string ReturnUrl { get; private set; }
    }
}
