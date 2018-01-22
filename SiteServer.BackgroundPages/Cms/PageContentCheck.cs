using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.Utils.Model;
using SiteServer.Utils.Model.Enumerations;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin;
using SiteServer.Plugin.Features;

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
        private NodeInfo _nodeInfo;
        private string _tableName;
        private Dictionary<string, IContentRelated> _pluginChannels;
        private bool _isEdit;
        private readonly Dictionary<string, string> _nameValueCacheDict = new Dictionary<string, string>();

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetCmsUrl(nameof(PageContentCheck), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");
            _channelId = Body.IsQueryExists("ChannelId") ? Body.GetQueryInt("ChannelId") : PublishmentSystemId;

            var permissions = PermissionsManager.GetPermissions(Body.AdminName);

            _relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(PublishmentSystemId, _channelId);
            _nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, _channelId);
            _tableName = NodeManager.GetTableName(PublishmentSystemInfo, _nodeInfo);
            _styleInfoList = TableStyleManager.GetTableStyleInfoList(_tableName, _relatedIdentities);
            _attributesOfDisplay = TranslateUtils.StringCollectionToStringCollection(NodeManager.GetContentAttributesOfDisplay(PublishmentSystemId, _channelId));
            _attributesOfDisplayStyleInfoList = ContentUtility.GetColumnTableStyleInfoList(PublishmentSystemInfo, _styleInfoList);
            _pluginChannels = PluginManager.GetContentRelatedFeatures(_nodeInfo);
            _isEdit = TextUtility.IsEdit(PublishmentSystemInfo, _channelId, Body.AdminName);

            if (IsPostBack) return;

            var checkedLevel = 5;
            var isChecked = true;
            foreach (var owningNodeId in ProductPermissionsManager.Current.OwningNodeIdList)
            {
                int checkedLevelByNodeId;
                var isCheckedByNodeId = CheckManager.GetUserCheckLevel(Body.AdminName, PublishmentSystemInfo, owningNodeId, out checkedLevelByNodeId);
                if (checkedLevel > checkedLevelByNodeId)
                {
                    checkedLevel = checkedLevelByNodeId;
                }
                if (!isCheckedByNodeId)
                {
                    isChecked = false;
                }
            }

            NodeManager.AddListItems(DdlChannelId.Items, PublishmentSystemInfo, true, true, Body.AdminName);
            CheckManager.LoadContentLevelToList(DdlState, PublishmentSystemInfo, PublishmentSystemId, isChecked, checkedLevel);
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
                checkLevelList = CheckManager.LevelInt.GetCheckLevelList(PublishmentSystemInfo, isChecked, checkedLevel);
            }

            SpContents.ControlToPaginate = RptContents;
            SpContents.ItemsPerPage = PublishmentSystemInfo.Additional.PageSize;

            var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, _channelId);
            var tableName = NodeManager.GetTableName(PublishmentSystemInfo, nodeInfo);
            var nodeIdList = DataProvider.NodeDao.GetNodeIdListByScopeType(nodeInfo.NodeId, nodeInfo.ChildrenCount, EScopeType.All, string.Empty, string.Empty, nodeInfo.ContentModelPluginId);
            var list = new List<int>();
            if (permissions.IsSystemAdministrator)
            {
                list = nodeIdList;
            }
            else
            {
                var owningNodeIdList = new List<int>();
                foreach (var owningNodeId in ProductPermissionsManager.Current.OwningNodeIdList)
                {
                    if (AdminUtility.HasChannelPermissions(Body.AdminName, PublishmentSystemId, owningNodeId, AppManager.Permissions.Channel.ContentCheck))
                    {
                        owningNodeIdList.Add(owningNodeId);
                    }
                }
                foreach (var theNodeId in nodeIdList)
                {
                    if (owningNodeIdList.Contains(theNodeId))
                    {
                        list.Add(theNodeId);
                    }
                }
            }

            SpContents.SelectCommand = DataProvider.ContentDao.GetSelectedCommendByCheck(tableName, PublishmentSystemId, list, checkLevelList);

            SpContents.SortField = ContentAttribute.LastEditDate;
            SpContents.SortMode = SortMode.DESC;
            RptContents.ItemDataBound += RptContents_ItemDataBound;

            SpContents.DataBind();

            var showPopWinString = ModalContentCheck.GetOpenWindowStringForMultiChannels(PublishmentSystemId, PageUrl);
            BtnCheck.Attributes.Add("onclick", showPopWinString);

            LtlColumnsHead.Text = TextUtility.GetColumnsHeadHtml(_styleInfoList, _attributesOfDisplay, PublishmentSystemInfo);

            if (!HasChannelPermissions(PublishmentSystemId, AppManager.Permissions.Channel.ContentDelete))
            {
                BtnDelete.Visible = false;
            }
            else
            {
                BtnDelete.Attributes.Add("onclick", PageContentDelete.GetRedirectClickStringForMultiChannels(PublishmentSystemId, false, PageUrl));
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

            ltlTitle.Text = WebUtils.GetContentTitle(PublishmentSystemInfo, contentInfo, PageUrl);

            ltlColumns.Text = TextUtility.GetColumnsHtml(_nameValueCacheDict, PublishmentSystemInfo, contentInfo, _attributesOfDisplay, _attributesOfDisplayStyleInfoList);

            string nodeName;
            if (!_nameValueCacheDict.TryGetValue(contentInfo.NodeId.ToString(), out nodeName))
            {
                nodeName = NodeManager.GetNodeNameNavigation(PublishmentSystemId, contentInfo.NodeId);
                _nameValueCacheDict[contentInfo.NodeId.ToString()] = nodeName;
            }
            ltlChannel.Text = nodeName;

            ltlStatus.Text =
                $@"<a href=""javascript:;"" title=""设置内容状态"" onclick=""{ModalCheckState.GetOpenWindowString(PublishmentSystemId, contentInfo, PageUrl)}"">{CheckManager.GetCheckState(PublishmentSystemInfo, contentInfo.IsChecked, contentInfo.CheckedLevel)}</a>";

            ltlCommands.Text = TextUtility.GetCommandsHtml(PublishmentSystemInfo, _pluginChannels, contentInfo, PageUrl, Body.AdminName, _isEdit);

            ltlSelect.Text = $@"<input type=""checkbox"" name=""IDsCollection"" value=""{contentInfo.NodeId}_{contentInfo.Id}"" />";
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
                    _pageUrl = PageUtils.GetCmsUrl(nameof(PageContentCheck), new NameValueCollection
                    {
                        {"publishmentSystemID", PublishmentSystemId.ToString()},
                        {"channelId", DdlChannelId.SelectedValue},
                        {"state", DdlState.SelectedValue}
                    });
                }
                return _pageUrl;
            }
        }
    }
}
