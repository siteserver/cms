using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageContentCheck : BasePageCms
    {
        public DropDownList DdlChannelId;
        public DropDownList DdlState;

        public Repeater RptContents;
        public SqlPager SpContents;
        public Literal LtlColumnsHead;

        public Button BtnCheck;
        public Button BtnDelete;

        private int _channelId;
        private List<TableStyleInfo> _styleInfoList;
        private StringCollection _attributesOfDisplay;
        private List<TableStyleInfo> _attributesOfDisplayStyleInfoList;
        private List<int> _relatedIdentities;
        private ChannelInfo _nodeInfo;
        private string _tableName;
        private Dictionary<string, List<HyperLink>> _pluginLinks;
        private bool _isEdit;
        private readonly Dictionary<string, string> _nameValueCacheDict = new Dictionary<string, string>();

        public static string GetRedirectUrl(int siteId)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageContentCheck), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId");
            _channelId = Body.IsQueryExists("ChannelId") ? Body.GetQueryInt("ChannelId") : SiteId;

            var permissions = PermissionsManager.GetPermissions(Body.AdminName);

            _relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(SiteId, _channelId);
            _nodeInfo = ChannelManager.GetChannelInfo(SiteId, _channelId);
            _tableName = ChannelManager.GetTableName(SiteInfo, _nodeInfo);
            _styleInfoList = TableStyleManager.GetTableStyleInfoList(_tableName, _relatedIdentities);
            _attributesOfDisplay = TranslateUtils.StringCollectionToStringCollection(ChannelManager.GetContentAttributesOfDisplay(SiteId, _channelId));
            _attributesOfDisplayStyleInfoList = ContentUtility.GetColumnTableStyleInfoList(SiteInfo, _styleInfoList);
            _pluginLinks = PluginContentManager.GetContentLinks(_nodeInfo);
            _isEdit = TextUtility.IsEdit(SiteInfo, _channelId, Body.AdminName);

            if (IsPostBack) return;

            var checkedLevel = 5;
            var isChecked = true;
            foreach (var owningChannelId in ProductPermissionsManager.Current.OwningChannelIdList)
            {
                int checkedLevelByChannelId;
                var isCheckedByChannelId = CheckManager.GetUserCheckLevel(Body.AdminName, SiteInfo, owningChannelId, out checkedLevelByChannelId);
                if (checkedLevel > checkedLevelByChannelId)
                {
                    checkedLevel = checkedLevelByChannelId;
                }
                if (!isCheckedByChannelId)
                {
                    isChecked = false;
                }
            }

            ChannelManager.AddListItems(DdlChannelId.Items, SiteInfo, true, true, Body.AdminName);
            CheckManager.LoadContentLevelToList(DdlState, SiteInfo, SiteId, isChecked, checkedLevel);
            var checkLevelList = new List<int>();

            if (!string.IsNullOrEmpty(Body.GetQueryString("channelId")))
            {
                ControlUtils.SelectSingleItem(DdlChannelId, Body.GetQueryString("channelId"));
            }
            if (!string.IsNullOrEmpty(Body.GetQueryString("state")))
            {
                ControlUtils.SelectSingleItem(DdlState, Body.GetQueryString("state"));
                checkLevelList.Add(Body.GetQueryInt("state"));
            }
            else
            {
                checkLevelList = CheckManager.LevelInt.GetCheckLevelList(SiteInfo, isChecked, checkedLevel);
            }

            SpContents.ControlToPaginate = RptContents;
            SpContents.ItemsPerPage = SiteInfo.Additional.PageSize;

            var nodeInfo = ChannelManager.GetChannelInfo(SiteId, _channelId);
            var tableName = ChannelManager.GetTableName(SiteInfo, nodeInfo);
            var channelIdList = DataProvider.ChannelDao.GetIdListByScopeType(nodeInfo.Id, nodeInfo.ChildrenCount, EScopeType.All, string.Empty, string.Empty, nodeInfo.ContentModelPluginId);
            var list = new List<int>();
            if (permissions.IsSystemAdministrator)
            {
                list = channelIdList;
            }
            else
            {
                var owningChannelIdList = new List<int>();
                foreach (var owningChannelId in ProductPermissionsManager.Current.OwningChannelIdList)
                {
                    if (AdminUtility.HasChannelPermissions(Body.AdminName, SiteId, owningChannelId, ConfigManager.Permissions.Channel.ContentCheck))
                    {
                        owningChannelIdList.Add(owningChannelId);
                    }
                }
                foreach (var theChannelId in channelIdList)
                {
                    if (owningChannelIdList.Contains(theChannelId))
                    {
                        list.Add(theChannelId);
                    }
                }
            }

            SpContents.SelectCommand = DataProvider.ContentDao.GetSelectedCommendByCheck(tableName, SiteId, list, checkLevelList);

            SpContents.SortField = ContentAttribute.LastEditDate;
            SpContents.SortMode = SortMode.DESC;
            RptContents.ItemDataBound += RptContents_ItemDataBound;

            SpContents.DataBind();

            var showPopWinString = ModalContentCheck.GetOpenWindowStringForMultiChannels(SiteId, PageUrl);
            BtnCheck.Attributes.Add("onclick", showPopWinString);

            LtlColumnsHead.Text = TextUtility.GetColumnsHeadHtml(_styleInfoList, _attributesOfDisplay, SiteInfo);

            if (!HasChannelPermissions(SiteId, ConfigManager.Permissions.Channel.ContentDelete))
            {
                BtnDelete.Visible = false;
            }
            else
            {
                BtnDelete.Attributes.Add("onclick", PageContentDelete.GetRedirectClickStringForMultiChannels(SiteId, false, PageUrl));
            }
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var contentInfo = new ContentInfo(e.Item.DataItem);

            var ltlTitle = (Literal)e.Item.FindControl("ltlTitle");
            var ltlChannel = (Literal)e.Item.FindControl("ltlChannel");
            var ltlColumns = (Literal)e.Item.FindControl("ltlColumns");
            var ltlStatus = (Literal)e.Item.FindControl("ltlStatus");
            var ltlCommands = (Literal)e.Item.FindControl("ltlCommands");
            var ltlSelect = (Literal) e.Item.FindControl("ltlSelect");

            ltlTitle.Text = WebUtils.GetContentTitle(SiteInfo, contentInfo, PageUrl);

            ltlColumns.Text = TextUtility.GetColumnsHtml(_nameValueCacheDict, SiteInfo, contentInfo, _attributesOfDisplay, _attributesOfDisplayStyleInfoList);

            string nodeName;
            if (!_nameValueCacheDict.TryGetValue(contentInfo.ChannelId.ToString(), out nodeName))
            {
                nodeName = ChannelManager.GetChannelNameNavigation(SiteId, contentInfo.ChannelId);
                _nameValueCacheDict[contentInfo.ChannelId.ToString()] = nodeName;
            }
            ltlChannel.Text = nodeName;

            ltlStatus.Text =
                $@"<a href=""javascript:;"" title=""设置内容状态"" onclick=""{ModalCheckState.GetOpenWindowString(SiteId, contentInfo, PageUrl)}"">{CheckManager.GetCheckState(SiteInfo, contentInfo.IsChecked, contentInfo.CheckedLevel)}</a>";

            ltlCommands.Text = TextUtility.GetCommandsHtml(SiteInfo, _pluginLinks, contentInfo, PageUrl, Body.AdminName, _isEdit);

            ltlSelect.Text = $@"<input type=""checkbox"" name=""IDsCollection"" value=""{contentInfo.ChannelId}_{contentInfo.Id}"" />";
        }

        public void Search_OnClick(object sender, EventArgs e)
        {
            Response.Redirect(PageUrl, true);
        }

        private string _pageUrl;
        private string PageUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_pageUrl))
                {
                    _pageUrl = PageUtils.GetCmsUrl(SiteId, nameof(PageContentCheck), new NameValueCollection
                    {
                        {"channelId", DdlChannelId.SelectedValue},
                        {"state", DdlState.SelectedValue}
                    });
                }
                return _pageUrl;
            }
        }
    }
}
