using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Core.Security;
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
            ReturnUrl = StringUtils.ValueFromUrl(Body.GetQueryString("ReturnUrl"));

            if (!HasChannelPermissions(SiteId, ConfigManager.Permissions.Channel.ContentDelete))
			{
                RblIsDeleteAfterTranslate.Visible = false;
			}

            if (IsPostBack) return;

            PhReturn.Visible = !string.IsNullOrEmpty(ReturnUrl);
            ETranslateTypeUtils.AddListItems(DdlTranslateType);
            ControlUtils.SelectSingleItem(DdlTranslateType,
                Body.IsQueryExists("ChannelIDCollection")
                    ? ETranslateTypeUtils.GetValue(ETranslateType.All)
                    : ETranslateTypeUtils.GetValue(ETranslateType.Content));

            var siteIdList = ProductPermissionsManager.Current.SiteIdList;
            foreach (var psId in siteIdList)
            {
                var psInfo = SiteManager.GetSiteInfo(psId);
                var listitem = new ListItem(psInfo.SiteName, psId.ToString());
                if (psId == SiteId) listitem.Selected = true;
                DdlSiteId.Items.Add(listitem);
            }

            var channelIdStrList = new List<string>();
            if (Body.IsQueryExists("ChannelIDCollection"))
            {
                channelIdStrList = TranslateUtils.StringCollectionToStringList(Body.GetQueryString("ChannelIDCollection"));
            }

            var channelIdList = DataProvider.ChannelDao.GetIdListBySiteId(SiteId);
            var nodeCount = channelIdList.Count;
            _isLastNodeArray = new bool[nodeCount];
            foreach (var theChannelId in channelIdList)
            {
                var enabled = IsOwningChannelId(theChannelId);
                if (!enabled)
                {
                    if (!IsHasChildOwningChannelId(theChannelId)) continue;
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

		public string GetTitle(ChannelInfo nodeInfo)
		{
			var str = "";
            if (nodeInfo.Id == SiteId)
			{
                nodeInfo.IsLastNode = true;
			}
            if (nodeInfo.IsLastNode == false)
			{
                _isLastNodeArray[nodeInfo.ParentsCount] = false;
			}
			else
			{
                _isLastNodeArray[nodeInfo.ParentsCount] = true;
			}
            for (var i = 0; i < nodeInfo.ParentsCount; i++)
            {
                str = string.Concat(str, _isLastNodeArray[i] ? "　" : "│");
            }
		    str = string.Concat(str, nodeInfo.IsLastNode ? "└" : "├");
		    str = string.Concat(str, nodeInfo.ChannelName);
            if (nodeInfo.ContentNum != 0)
            {
                str = $"{str} ({nodeInfo.ContentNum})";
            }
			return str;
		}

        public override void Submit_OnClick(object sender, EventArgs e)
		{
		    if (!Page.IsPostBack) return;

		    var targetChannelId = TranslateUtils.ToInt(DdlChannelIdTo.SelectedValue);

		    var targetSiteId = TranslateUtils.ToInt(DdlSiteId.SelectedValue);
		    var targetSiteInfo = SiteManager.GetSiteInfo(targetSiteId);
		    bool isChecked;
		    int checkedLevel;
		    if (targetSiteInfo.Additional.CheckContentLevel == 0 || AdminUtility.HasChannelPermissions(Body.AdminName, targetSiteId, targetChannelId, ConfigManager.Permissions.Channel.ContentAdd, ConfigManager.Permissions.Channel.ContentCheck))
		    {
		        isChecked = true;
		        checkedLevel = 0;
		    }
		    else
		    {
		        var userCheckLevel = 0;
		        var ownHighestLevel = false;

		        if (AdminUtility.HasChannelPermissions(Body.AdminName, targetSiteId, targetChannelId, ConfigManager.Permissions.Channel.ContentCheckLevel1))
		        {
		            userCheckLevel = 1;
		            if (AdminUtility.HasChannelPermissions(Body.AdminName, targetSiteId, targetChannelId, ConfigManager.Permissions.Channel.ContentCheckLevel2))
		            {
		                userCheckLevel = 2;
		                if (AdminUtility.HasChannelPermissions(Body.AdminName, targetSiteId, targetChannelId, ConfigManager.Permissions.Channel.ContentCheckLevel3))
		                {
		                    userCheckLevel = 3;
		                    if (AdminUtility.HasChannelPermissions(Body.AdminName, targetSiteId, targetChannelId, ConfigManager.Permissions.Channel.ContentCheckLevel4))
		                    {
		                        userCheckLevel = 4;
		                        if (AdminUtility.HasChannelPermissions(Body.AdminName, targetSiteId, targetChannelId, ConfigManager.Permissions.Channel.ContentCheckLevel5))
		                        {
		                            userCheckLevel = 5;
		                        }
		                    }
		                }
		            }
		        }

		        if (userCheckLevel >= targetSiteInfo.Additional.CheckContentLevel)
		        {
		            ownHighestLevel = true;
		        }
		        if (ownHighestLevel)
		        {
		            isChecked = true;
		            checkedLevel = 0;
		        }
		        else
		        {
		            isChecked = false;
		            checkedLevel = userCheckLevel;
		        }
		    }

		    try
		    {
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
		                TranslateContent(targetSiteInfo, channelId, targetChannelId, isChecked, checkedLevel);
		            }
		        }

		        if (translateType != ETranslateType.Content)//需要转移栏目
		        {
		            var channelIdListToTranslate = new List<int>(channelIdList);
		            foreach (var channelId in channelIdList)
		            {
		                var subChannelIdArrayList = DataProvider.ChannelDao.GetIdListForDescendant(channelId);
		                if (subChannelIdArrayList != null && subChannelIdArrayList.Count > 0)
		                {
		                    foreach (int channelIdToDelete in subChannelIdArrayList)
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

		            TranslateChannelAndContent(nodeInfoList, targetSiteId, targetChannelId, translateType, isChecked, checkedLevel, null, null);

		            if (RblIsDeleteAfterTranslate.Visible && EBooleanUtils.Equals(RblIsDeleteAfterTranslate.SelectedValue, EBoolean.True))
		            {
		                foreach (int channelId in channelIdListToTranslate)
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
                BtnSubmit.Enabled = false;

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
		        Body.AddSiteLog(SiteId, "批量转移", $"栏目:{builder},转移后删除:{RblIsDeleteAfterTranslate.SelectedValue}");

		        SuccessMessage("批量转移成功！");
		        PageUtils.Redirect(Body.IsQueryExists("ChannelIDCollection") ? ReturnUrl : GetRedirectUrl(SiteId));
		    }
		    catch(Exception ex)
		    {
		        FailMessage(ex, "批量转移失败！");
		        LogUtils.AddSystemErrorLog(ex);
		    }
		}

		private void TranslateChannelAndContent(List<ChannelInfo> nodeInfoList, int targetSiteId, int parentId, ETranslateType translateType, bool isChecked, int checkedLevel, List<string> nodeIndexNameList, List<string> filePathList)
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

            var targetSiteInfo = SiteManager.GetSiteInfo(targetSiteId);

			foreach (var oldNodeInfo in nodeInfoList)
			{
			    var nodeInfo = new ChannelInfo(oldNodeInfo)
			    {
			        SiteId = targetSiteId,
			        ParentId = parentId,
			        ContentNum = 0,
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

                var insertedChannelId = DataProvider.ChannelDao.Insert(nodeInfo);

                if (translateType == ETranslateType.All)
                {
                    TranslateContent(targetSiteInfo, oldNodeInfo.Id, insertedChannelId, isChecked, checkedLevel);
                }

                if (insertedChannelId != 0)
                {
                    var orderByString = ETaxisTypeUtils.GetChannelOrderByString(ETaxisType.OrderByTaxis);
                    var childrenNodeInfoList = DataProvider.ChannelDao.GetChannelInfoList(oldNodeInfo.Id, oldNodeInfo.ChildrenCount, 0, "", EScopeType.Children, orderByString);
                    if (childrenNodeInfoList != null && childrenNodeInfoList.Count > 0)
                    {
                        TranslateChannelAndContent(childrenNodeInfoList, targetSiteId, insertedChannelId, translateType, isChecked, checkedLevel, nodeIndexNameList, filePathList);
                    }

                    if (isChecked)
                    {
                        CreateManager.CreateChannel(targetSiteInfo.Id, insertedChannelId);
                    }
                }
			}
		}

		private void TranslateContent(SiteInfo targetSiteInfo, int channelId, int targetChannelId, bool isChecked, int checkedLevel)
		{
            var tableName = ChannelManager.GetTableName(SiteInfo, channelId);

            var orderByString = ETaxisTypeUtils.GetContentOrderByString(ETaxisType.OrderByTaxis);

            var targetTableName = ChannelManager.GetTableName(targetSiteInfo, targetChannelId);
            var contentIdList = DataProvider.ContentDao.GetContentIdListChecked(tableName, channelId, orderByString);

            foreach (var contentId in contentIdList)
			{
                var contentInfo = DataProvider.ContentDao.GetContentInfo(tableName, contentId);
                FileUtility.MoveFileByContentInfo(SiteInfo, targetSiteInfo, contentInfo);
                contentInfo.SiteId = TranslateUtils.ToInt(DdlSiteId.SelectedValue);
                contentInfo.SourceId = contentInfo.ChannelId;
				contentInfo.ChannelId = targetChannelId;
				contentInfo.IsChecked = isChecked;
				contentInfo.CheckedLevel = checkedLevel;

                var theContentId = DataProvider.ContentDao.Insert(targetTableName, targetSiteInfo, contentInfo);
				if (contentInfo.IsChecked)
				{
                    CreateManager.CreateContentAndTrigger(targetSiteInfo.Id, contentInfo.ChannelId, theContentId);
				}
			}

			if (RblIsDeleteAfterTranslate.Visible && EBooleanUtils.Equals(RblIsDeleteAfterTranslate.SelectedValue, EBoolean.True))
			{
                try
                {
                    DataProvider.ContentDao.TrashContents(SiteId, tableName, contentIdList, channelId);
                }
			    catch
			    {
			        // ignored
			    }
			}
		}


		public void DdlSiteId_OnSelectedIndexChanged(object sender, EventArgs e)
		{
			var psId = int.Parse(DdlSiteId.SelectedValue);

            DdlChannelIdTo.Items.Clear();

			var channelIdList = DataProvider.ChannelDao.GetIdListBySiteId(psId);
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
