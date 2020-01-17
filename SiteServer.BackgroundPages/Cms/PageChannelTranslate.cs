using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Repositories;
using SiteServer.Abstractions;

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

            if (!HasChannelPermissions(SiteId, Constants.ChannelPermissions.ContentDelete))
			{
                RblIsDeleteAfterTranslate.Visible = false;
			}

            if (IsPostBack) return;

            PhReturn.Visible = !string.IsNullOrEmpty(ReturnUrl);
            ETranslateContentTypeUtilsExtensions.AddListItems(DdlTranslateType, false);
            ControlUtils.SelectSingleItem(DdlTranslateType,
                AuthRequest.IsQueryExists("ChannelIDCollection")
                    ? ETranslateTypeUtils.GetValue(ETranslateType.All)
                    : ETranslateTypeUtils.GetValue(ETranslateType.Content));

            var siteIdList = AuthRequest.AdminPermissionsImpl.GetSiteIdListAsync().GetAwaiter().GetResult();
            foreach (var psId in siteIdList)
            {
                var psInfo = DataProvider.SiteRepository.GetAsync(psId).GetAwaiter().GetResult();
                var listitem = new ListItem(psInfo.SiteName, psId.ToString());
                if (psId == SiteId) listitem.Selected = true;
                DdlSiteId.Items.Add(listitem);
            }

            var channelIdStrList = new List<string>();
            if (AuthRequest.IsQueryExists("ChannelIDCollection"))
            {
                channelIdStrList = StringUtils.GetStringList(AuthRequest.GetQueryString("ChannelIDCollection"));
            }

            var channelIdList = ChannelManager.GetChannelIdListAsync(SiteId).GetAwaiter().GetResult();
            foreach (var theChannelId in channelIdList)
            {
                var enabled = IsOwningChannelId(theChannelId);
                if (!enabled)
                {
                    if (!IsDescendantOwningChannelId(theChannelId)) continue;
                }
                var nodeInfo = ChannelManager.GetChannelAsync(SiteId, theChannelId).GetAwaiter().GetResult();

                var value = enabled ? nodeInfo.Id.ToString() : string.Empty;

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

		public string GetTitle(Channel channel)
		{
			var str = "";
            str = string.Concat(str, channel.ChannelName);
		    var adminId = AuthRequest.AdminPermissionsImpl.GetAdminIdAsync(SiteId, channel.Id).GetAwaiter().GetResult();
            var count = DataProvider.ContentRepository.GetCountAsync(Site, channel, adminId).GetAwaiter().GetResult();
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
                    if (!ChannelManager.IsAncestorOrSelfAsync(SiteId, channelId, targetChannelId).GetAwaiter().GetResult())
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
                    var channelInfo = ChannelManager.GetChannelAsync(SiteId, channelId).GetAwaiter().GetResult();
                    var subChannelIdList = ChannelManager.GetChannelIdListAsync(channelInfo, EScopeType.Descendant, string.Empty, string.Empty, string.Empty).GetAwaiter().GetResult();

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

                var nodeInfoList = new List<Channel>();
                foreach (int channelId in channelIdListToTranslate)
                {
                    var nodeInfo = ChannelManager.GetChannelAsync(SiteId, channelId).GetAwaiter().GetResult();
                    nodeInfoList.Add(nodeInfo);
                }

                TranslateChannelAndContent(nodeInfoList, targetSiteId, targetChannelId, translateType, null, null);

                if (RblIsDeleteAfterTranslate.Visible && EBooleanUtils.Equals(RblIsDeleteAfterTranslate.SelectedValue, EBoolean.True))
                {
                    foreach (var channelId in channelIdListToTranslate)
                    {
                        try
                        {
                            DataProvider.ChannelRepository.DeleteAsync(SiteId, channelId).GetAwaiter().GetResult();
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
            AuthRequest.AddSiteLogAsync(SiteId, "批量转移", $"栏目:{builder},转移后删除:{RblIsDeleteAfterTranslate.SelectedValue}").GetAwaiter().GetResult();

            SuccessMessage("批量转移成功！");

            if (!string.IsNullOrEmpty(ReturnUrl))
            {
                PageUtils.Redirect(ReturnUrl);
            }
        }

		private void TranslateChannelAndContent(List<Channel> nodeInfoList, int targetSiteId, int parentId, ETranslateType translateType, List<string> nodeIndexNameList, List<string> filePathList)
		{
			if (nodeInfoList == null || nodeInfoList.Count == 0)
			{
				return;
			}

			if (nodeIndexNameList == null)
			{
                nodeIndexNameList = DataProvider.ChannelRepository.GetIndexNameListAsync(targetSiteId).GetAwaiter().GetResult().ToList();
			}

            if (filePathList == null)
			{
                filePathList = DataProvider.ChannelRepository.GetAllFilePathBySiteIdAsync(targetSiteId).GetAwaiter().GetResult().ToList();
			}

			foreach (var oldNodeInfo in nodeInfoList)
            {
                var nodeInfo = oldNodeInfo.Clone();

                nodeInfo.SiteId = targetSiteId;
                nodeInfo.ParentId = parentId;
                nodeInfo.ChildrenCount = 0;
                nodeInfo.AddDate = DateTime.Now;

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

                var targetChannelId = DataProvider.ChannelRepository.InsertAsync(nodeInfo).GetAwaiter().GetResult();

                if (translateType == ETranslateType.All)
                {
                    TranslateContent(oldNodeInfo.Id, targetSiteId, targetChannelId);
                }

                if (targetChannelId != 0)
                {
                    //var orderByString = ETaxisTypeUtils.GetChannelOrderByString(ETaxisType.OrderByTaxis);
                    //var childrenNodeInfoList = DataProvider.ChannelRepository.GetChannelInfoList(oldNodeInfo, 0, "", EScopeType.Children, orderByString);

                    var channelIdList = ChannelManager.GetChannelIdListAsync(oldNodeInfo, EScopeType.Children, string.Empty, string.Empty, string.Empty).GetAwaiter().GetResult();
                    var childrenNodeInfoList = new List<Channel>();
                    foreach (var channelId in channelIdList)
                    {
                        childrenNodeInfoList.Add(ChannelManager.GetChannelAsync(oldNodeInfo.SiteId, channelId).GetAwaiter().GetResult());
                    }

                    if (channelIdList.Count > 0)
                    {
                        TranslateChannelAndContent(childrenNodeInfoList, targetSiteId, targetChannelId, translateType, nodeIndexNameList, filePathList);
                    }

                    CreateManager.CreateChannelAsync(targetSiteId, targetChannelId).GetAwaiter().GetResult();
                }
			}
		}

		private void TranslateContent(int channelId, int targetSiteId, int targetChannelId)
		{
            var tableName = ChannelManager.GetTableNameAsync(Site, channelId).GetAwaiter().GetResult();

            var orderByString = ETaxisTypeUtils.GetContentOrderByString(TaxisType.OrderByTaxis);

            var contentIdList = DataProvider.ContentRepository.GetContentIdListChecked(tableName, channelId, orderByString);
		    var translateType = RblIsDeleteAfterTranslate.Visible &&
		                        EBooleanUtils.Equals(RblIsDeleteAfterTranslate.SelectedValue, EBoolean.True)
		        ? TranslateContentType.Cut
		        : TranslateContentType.Copy;

            foreach (var contentId in contentIdList)
			{
                ContentUtility.TranslateAsync(Site, channelId, contentId, targetSiteId, targetChannelId, translateType).GetAwaiter().GetResult();
			}
		}

		public void DdlSiteId_OnSelectedIndexChanged(object sender, EventArgs e)
		{
			var psId = int.Parse(DdlSiteId.SelectedValue);

            DdlChannelIdTo.Items.Clear();

			var channelIdList = ChannelManager.GetChannelIdListAsync(psId).GetAwaiter().GetResult();
            var nodeCount = channelIdList.Count;
            foreach (var theChannelId in channelIdList)
			{
                var nodeInfo = ChannelManager.GetChannelAsync(psId, theChannelId).GetAwaiter().GetResult();
                var value = IsOwningChannelId(nodeInfo.Id) ? nodeInfo.Id.ToString() : "";
                var listitem = new ListItem(GetTitle(nodeInfo), value);
                DdlChannelIdTo.Items.Add(listitem);
			}
		}

        public string ReturnUrl { get; private set; }
    }
}
