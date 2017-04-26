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
    public class PageContent : BasePageCms
    {
        public Repeater rptContents;
        public SqlPager spContents;
        public Literal ltlColumnHeadRows;
        public Literal ltlCommandHeadRows;

        public Literal ltlContentButtons;

        public DateTimeTextBox DateFrom;
        public DropDownList SearchType;
        public TextBox Keyword;

        private NodeInfo nodeInfo;
        private ETableStyle tableStyle;
        private string tableName;
        private StringCollection attributesOfDisplay;
        private List<int> relatedIdentities;
        private List<TableStyleInfo> styleInfoList;
        private readonly Hashtable valueHashtable = new Hashtable();

        public static string GetRedirectUrl(int publishmentSystemId, int nodeId)
        {
            return PageUtils.GetCmsUrl(nameof(PageContent), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            var permissions = PermissionsManager.GetPermissions(Body.AdministratorName);

            PageUtils.CheckRequestParameter("PublishmentSystemID", "NodeID");
            var nodeID = Body.GetQueryInt("NodeID");
            relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(PublishmentSystemId, nodeID);
            nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeID);
            tableName = NodeManager.GetTableName(PublishmentSystemInfo, nodeInfo);
            tableStyle = NodeManager.GetTableStyle(PublishmentSystemInfo, nodeInfo);
            styleInfoList = TableStyleManager.GetTableStyleInfoList(tableStyle, tableName, relatedIdentities);

            if (nodeInfo.Additional.IsPreviewContents)
            {
                new Action(() =>
                {
                    DataProvider.ContentDao.DeletePreviewContents(PublishmentSystemId, tableName, nodeInfo);
                }).BeginInvoke(null, null);
            }

            if (!HasChannelPermissions(nodeID, AppManager.Cms.Permission.Channel.ContentView, AppManager.Cms.Permission.Channel.ContentAdd, AppManager.Cms.Permission.Channel.ContentEdit, AppManager.Cms.Permission.Channel.ContentDelete, AppManager.Cms.Permission.Channel.ContentTranslate))
            {
                if (!Body.IsAdministratorLoggin)
                {
                    PageUtils.RedirectToLoginPage();
                    return;
                }
                PageUtils.RedirectToErrorPage("您无此栏目的操作权限！");
                return;
            }

            attributesOfDisplay = TranslateUtils.StringCollectionToStringCollection(NodeManager.GetContentAttributesOfDisplay(PublishmentSystemId, nodeID));

            //this.attributesOfDisplay = TranslateUtils.StringCollectionToStringCollection(this.nodeInfo.Additional.ContentAttributesOfDisplay);

            spContents.ControlToPaginate = rptContents;
            rptContents.ItemDataBound += rptContents_ItemDataBound;
            spContents.ItemsPerPage = PublishmentSystemInfo.Additional.PageSize;

            var administratorName = AdminUtility.IsViewContentOnlySelf(Body.AdministratorName, PublishmentSystemId, nodeID)
                    ? Body.AdministratorName
                    : string.Empty;

            if (Body.IsQueryExists("SearchType"))
            {
                var owningNodeIdList = new List<int>
                {
                    nodeID
                };
                spContents.SelectCommand = DataProvider.ContentDao.GetSelectCommend(tableStyle, tableName, PublishmentSystemId, nodeID, permissions.IsSystemAdministrator, owningNodeIdList, Body.GetQueryString("SearchType"), Body.GetQueryString("Keyword"), Body.GetQueryString("DateFrom"), string.Empty, false, ETriState.All, false, false, false, administratorName);
            }
            else
            {
                spContents.SelectCommand = BaiRongDataProvider.ContentDao.GetSelectCommend(tableName, nodeID, ETriState.All, administratorName);
            }

            spContents.SortField = BaiRongDataProvider.ContentDao.GetSortFieldName();
            spContents.SortMode = SortMode.DESC;
            spContents.OrderByString = ETaxisTypeUtils.GetOrderByString(tableStyle, ETaxisType.OrderByTaxisDesc);

            //分页的时候，不去查询总条数，直接使用栏目的属性：ContentNum
            spContents.IsQueryTotalCount = false;
            spContents.TotalCount = nodeInfo.ContentNum;

            if (!IsPostBack)
            {
                var nodeName = NodeManager.GetNodeNameNavigation(PublishmentSystemId, nodeID);
                BreadCrumbWithItemTitle(AppManager.Cms.LeftMenu.IdContent, "内容管理", nodeName, string.Empty);

                ltlContentButtons.Text = WebUtils.GetContentCommands(Body.AdministratorName, PublishmentSystemInfo, nodeInfo, PageUrl, GetRedirectUrl(PublishmentSystemId, nodeInfo.NodeId), false);
                spContents.DataBind();

                if (styleInfoList != null)
                {
                    foreach (var styleInfo in styleInfoList)
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
                    ltlContentButtons.Text += @"
<script>
$(document).ready(function() {
	$('#contentSearch').show();
});
</script>
";
                }

                ltlColumnHeadRows.Text = ContentUtility.GetColumnHeadRowsHtml(styleInfoList, attributesOfDisplay, tableStyle, PublishmentSystemInfo);
                ltlCommandHeadRows.Text = ContentUtility.GetCommandHeadRowsHtml(Body.AdministratorName, tableStyle, PublishmentSystemInfo, nodeInfo);
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var ltlItemTitle = e.Item.FindControl("ltlItemTitle") as Literal;
                var ltlColumnItemRows = e.Item.FindControl("ltlColumnItemRows") as Literal;
                var ltlItemStatus = e.Item.FindControl("ltlItemStatus") as Literal;
                var ltlItemEditUrl = e.Item.FindControl("ltlItemEditUrl") as Literal;
                var ltlCommandItemRows = e.Item.FindControl("ltlCommandItemRows") as Literal;

                var contentInfo = new ContentInfo(e.Item.DataItem);

                ltlItemTitle.Text = WebUtils.GetContentTitle(PublishmentSystemInfo, contentInfo, PageUrl);

                var showPopWinString = ModalCheckState.GetOpenWindowString(PublishmentSystemId, contentInfo, PageUrl);

                ltlItemStatus.Text =
                    $@"<a href=""javascript:;"" title=""设置内容状态"" onclick=""{showPopWinString}"">{LevelManager.GetCheckState(
                        PublishmentSystemInfo, contentInfo.IsChecked, contentInfo.CheckedLevel)}</a>";

                if (HasChannelPermissions(contentInfo.NodeId, AppManager.Cms.Permission.Channel.ContentEdit) || Body.AdministratorName == contentInfo.AddUserName)
                {
                    ltlItemEditUrl.Text =
                        $"<a href=\"{WebUtils.GetContentAddEditUrl(PublishmentSystemId, nodeInfo, contentInfo.Id, PageUrl)}\">编辑</a>";
                }

                ltlColumnItemRows.Text = TextUtility.GetColumnItemRowsHtml(styleInfoList, attributesOfDisplay, valueHashtable, tableStyle, PublishmentSystemInfo, contentInfo);

                ltlCommandItemRows.Text = TextUtility.GetCommandItemRowsHtml(tableStyle, PublishmentSystemInfo, nodeInfo, contentInfo, PageUrl, Body.AdministratorName);
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
                    _pageUrl = PageUtils.GetCmsUrl(nameof(PageContent), new NameValueCollection
                    {
                        {"PublishmentSystemID", PublishmentSystemId.ToString()},
                        {"NodeID", nodeInfo.NodeId.ToString()},
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
