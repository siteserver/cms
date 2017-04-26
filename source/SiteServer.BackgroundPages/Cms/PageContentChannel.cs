using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Permissions;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Core.User;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageContentChannel : BasePageCms
    {
        public DateTimeTextBox DateFrom;
        public DropDownList SearchType;
        public TextBox Keyword;
        public Repeater RptContents;
        public SqlPager SpContents;
        public Literal LtlColumnHeadRows;
        public Literal LtlCommandHeadRows;
        public Repeater RptChannels;
        public Literal LtlContentButtons;
        public Literal LtlChannelButtons;

        private NodeInfo _nodeInfo;
        private ETableStyle _tableStyle;
        private string _tableName;
        private StringCollection _attributesOfDisplay;
        private List<int> _relatedIdentities;
        private List<TableStyleInfo> _tableStyleInfoList;
        private readonly Hashtable _displayNameHashtable = new Hashtable();

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            var permissions = PermissionsManager.GetPermissions(Body.AdministratorName);

            PageUtils.CheckRequestParameter("PublishmentSystemID");
            var nodeId = Body.IsQueryExists("NodeID") ? Body.GetQueryInt("NodeID", PublishmentSystemId) : PublishmentSystemId;
            _nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeId);
            _tableStyle = NodeManager.GetTableStyle(PublishmentSystemInfo, _nodeInfo);
            _tableName = NodeManager.GetTableName(PublishmentSystemInfo, _nodeInfo);
            _relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(PublishmentSystemId, nodeId);
            _tableStyleInfoList = TableStyleManager.GetTableStyleInfoList(_tableStyle, _tableName, _relatedIdentities);
            _attributesOfDisplay = TranslateUtils.StringCollectionToStringCollection(NodeManager.GetContentAttributesOfDisplay(PublishmentSystemId, nodeId));

            if (_nodeInfo.Additional.IsPreviewContents)
            {
                new Action(() =>
                {
                    DataProvider.ContentDao.DeletePreviewContents(PublishmentSystemId, _tableName, _nodeInfo);
                }).BeginInvoke(null, null);
            }

            if (Body.IsQueryExists("TheNodeID") && (Body.IsQueryExists("Subtract") || Body.IsQueryExists("Add")))
            {
                var theNodeId = Body.GetQueryInt("TheNodeID");
                if (PublishmentSystemId != theNodeId)
                {
                    var isSubtract = Body.IsQueryExists("Subtract");
                    DataProvider.NodeDao.UpdateTaxis(PublishmentSystemId, theNodeId, isSubtract);

                    Body.AddSiteLog(PublishmentSystemId, theNodeId, 0, "栏目排序" + (isSubtract ? "上升" : "下降"),
                        $"栏目:{NodeManager.GetNodeName(PublishmentSystemId, theNodeId)}");
                }
            }

            SpContents.ControlToPaginate = RptContents;
            RptContents.ItemDataBound += rptContents_ItemDataBound;
            SpContents.ItemsPerPage = PublishmentSystemInfo.Additional.PageSize;

            var administratorName = AdminUtility.IsViewContentOnlySelf(Body.AdministratorName, PublishmentSystemId, nodeId)
                ? Body.AdministratorName
                : string.Empty;

            if (Body.IsQueryExists("SearchType"))
            {
                var owningNodeIdList = new List<int> {nodeId};
                SpContents.SelectCommand = DataProvider.ContentDao.GetSelectCommend(_tableStyle, _tableName, PublishmentSystemId, nodeId, permissions.IsSystemAdministrator, owningNodeIdList, Body.GetQueryString("SearchType"), Body.GetQueryString("Keyword"), Body.GetQueryString("DateFrom"), string.Empty, false, ETriState.All, false, false, false, administratorName);
            }
            else
            {
                SpContents.SelectCommand = BaiRongDataProvider.ContentDao.GetSelectCommend(_tableName, nodeId, ETriState.All, administratorName);
            }

            SpContents.SortField = BaiRongDataProvider.ContentDao.GetSortFieldName();
            SpContents.SortMode = SortMode.DESC;
            if (Body.IsQueryExists("strDirection"))
            {
                SpContents.SortField = "AddDate";
                SpContents.SortMode = Body.GetQueryString("strDirection").Equals("0") ? SortMode.ASC : SortMode.DESC;
            }

            //分页的时候，不去查询总条数，直接使用栏目的属性：ContentNum
            SpContents.IsQueryTotalCount = false;
            SpContents.TotalCount = _nodeInfo.ContentNum;

            RptChannels.DataSource = DataProvider.NodeDao.GetNodeIdListByScopeType(_nodeInfo, EScopeType.Children, string.Empty, string.Empty);
            RptChannels.ItemDataBound += rptChannels_ItemDataBound;

            if (!IsPostBack)
            {
                var nodeName = NodeManager.GetNodeNameNavigation(PublishmentSystemId, nodeId).Replace(">", "/");
                BreadCrumbWithItemTitle(AppManager.Cms.LeftMenu.IdContent, "栏目及内容", nodeName, string.Empty);
                var url = PageUtils.GetCmsUrl(nameof(PageContentChannel), null);
                LtlContentButtons.Text = WebUtils.GetContentCommands(Body.AdministratorName, PublishmentSystemInfo, _nodeInfo, PageUrl, url, false);
                LtlChannelButtons.Text = WebUtils.GetChannelCommands(Body.AdministratorName, PublishmentSystemInfo, _nodeInfo, PageUrl, url);
                SpContents.DataBind();
                RptChannels.DataBind();

                if (_tableStyleInfoList != null)
                {
                    foreach (var styleInfo in _tableStyleInfoList)
                    {
                        if (styleInfo.IsVisible)
                        {
                            var listitem = new ListItem(styleInfo.DisplayName, styleInfo.AttributeName);
                            SearchType.Items.Add(listitem);
                        }
                    }
                }
                //添加隐藏属性
                SearchType.Items.Add(new ListItem("内容ID", ContentAttribute.Id));
                SearchType.Items.Add(new ListItem("添加者", ContentAttribute.AddUserName));
                SearchType.Items.Add(new ListItem("最后修改者", ContentAttribute.LastEditUserName));
                SearchType.Items.Add(new ListItem("内容组", ContentAttribute.ContentGroupNameCollection));

                if (Body.IsQueryExists("SearchType"))
                {
                    DateFrom.Text = Body.GetQueryString("DateFrom");
                    ControlUtils.SelectListItems(SearchType, Body.GetQueryString("SearchType"));
                    Keyword.Text = Body.GetQueryString("Keyword");
                    LtlContentButtons.Text += @"
<script>
$(document).ready(function() {
	$('#contentSearch').show();
});
</script>
";
                }

                LtlColumnHeadRows.Text = ContentUtility.GetColumnHeadRowsHtml(_tableStyleInfoList, _attributesOfDisplay, _tableStyle, PublishmentSystemInfo);
                LtlCommandHeadRows.Text = ContentUtility.GetCommandHeadRowsHtml(Body.AdministratorName, _tableStyle, PublishmentSystemInfo, _nodeInfo);
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var ltlItemTitle = (Literal)e.Item.FindControl("ltlItemTitle");
                var ltlColumnItemRows = (Literal)e.Item.FindControl("ltlColumnItemRows");
                var ltlItemStatus = (Literal)e.Item.FindControl("ltlItemStatus");
                var ltlItemEditUrl = (Literal)e.Item.FindControl("ltlItemEditUrl");
                var ltlCommandItemRows = (Literal)e.Item.FindControl("ltlCommandItemRows");

                var contentInfo = new ContentInfo(e.Item.DataItem);

                ltlItemTitle.Text = WebUtils.GetContentTitle(PublishmentSystemInfo, contentInfo, PageUrl);

                var showPopWinString = ModalCheckState.GetOpenWindowString(PublishmentSystemId, contentInfo, PageUrl);
                ltlItemStatus.Text =
                    $@"<a href=""javascript:;"" title=""设置内容状态"" onclick=""{showPopWinString}"">{LevelManager.GetCheckState(
                        PublishmentSystemInfo, contentInfo.IsChecked, contentInfo.CheckedLevel)}</a>";

                if (HasChannelPermissions(contentInfo.NodeId, AppManager.Cms.Permission.Channel.ContentEdit) || Body.AdministratorName == contentInfo.AddUserName)
                {
                    ltlItemEditUrl.Text =
                        $"<a href=\"{WebUtils.GetContentAddEditUrl(PublishmentSystemId, _nodeInfo, contentInfo.Id, PageUrl)}\">编辑</a>";
                }

                ltlColumnItemRows.Text = TextUtility.GetColumnItemRowsHtml(_tableStyleInfoList, _attributesOfDisplay, _displayNameHashtable, _tableStyle, PublishmentSystemInfo, contentInfo);

                ltlCommandItemRows.Text = TextUtility.GetCommandItemRowsHtml(_tableStyle, PublishmentSystemInfo, _nodeInfo, contentInfo, PageUrl, Body.AdministratorName);
            }
        }

        void rptChannels_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var nodeId = (int)e.Item.DataItem;
            var enabled = IsOwningNodeId(nodeId);
            if (!enabled)
            {
                if (!IsHasChildOwningNodeId(nodeId)) e.Item.Visible = false;
            }
            var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeId);

            var ltlEditLink = (Literal)e.Item.FindControl("ltlEditLink");
            var ltlNodeTitle = (Literal)e.Item.FindControl("ltlNodeTitle");
            var ltlNodeIndexName = (Literal)e.Item.FindControl("ltlNodeIndexName");
            var ltlUpLink = (Literal)e.Item.FindControl("ltlUpLink");
            var ltlDownLink = (Literal)e.Item.FindControl("ltlDownLink");
            var ltlCheckBoxHtml = (Literal)e.Item.FindControl("ltlCheckBoxHtml");

            if (enabled && HasChannelPermissions(nodeId, AppManager.Cms.Permission.Channel.ChannelEdit))
            {
                ltlEditLink.Text = $"<a href=\"{PageChannelEdit.GetRedirectUrl(PublishmentSystemId, nodeId, PageUrl)}\">编辑</a>";

                ltlUpLink.Text =
                    $@"<a href=""{PageUrl}&TheNodeID={nodeInfo.NodeId}&Subtract=True""><img src=""../Pic/icon/up.gif"" border=""0"" alt=""上升"" /></a>";
                ltlDownLink.Text =
                    $@"<a href=""{PageUrl}&TheNodeID={nodeInfo.NodeId}&Add=True""><img src=""../Pic/icon/down.gif"" border=""0"" alt=""下降"" /></a>";
            }

            var url = PageUtils.GetCmsUrl(nameof(PageContentChannel), new NameValueCollection
            {
                {"PublishmentSystemID", PublishmentSystemId.ToString() },
                {"NodeID", nodeId.ToString() }
            });

            ltlNodeTitle.Text =
                $@"<a href=""{PageActions.GetRedirectUrl(PublishmentSystemId, nodeId)}"" title=""浏览页面"" target=""_blank""><img src=""{SiteServerAssets.GetIconUrl("tree/folder.gif")}"" border=""0"" align=""absMiddle"" /></a>&nbsp;<A title=""进入栏目"" href=""{url}"">{nodeInfo
                    .NodeName}</A>&nbsp;{NodeManager.GetNodeTreeLastImageHtml(PublishmentSystemInfo, nodeInfo)}&nbsp;<SPAN class=""gray"" style=""FONT-SIZE: 8pt; FONT-FAMILY: arial"">({nodeInfo
                    .ContentNum})</SPAN>";

            ltlNodeIndexName.Text = nodeInfo.NodeIndexName;

            if (enabled)
            {
                ltlCheckBoxHtml.Text = $"<input type='checkbox' name='ChannelIDCollection' value='{nodeInfo.NodeId}' />";
            }
        }

        public void Search_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(PageUrl);
        }

        private string _pageUrl;
        private string PageUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_pageUrl))
                {
                    _pageUrl = PageUtils.GetCmsUrl(nameof(PageContentChannel), new NameValueCollection
                    {
                        {"PublishmentSystemID", PublishmentSystemId.ToString()},
                        {"NodeID", _nodeInfo.NodeId.ToString()},
                        {"DateFrom", DateFrom.Text},
                        {"SearchType", SearchType.SelectedValue},
                        {"Keyword", Keyword.Text},
                        {"page", Body.GetQueryInt("page", 1).ToString()}
                    });
                }
                return _pageUrl;
            }
        }
    }
}
