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
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Permissions;
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

        private NodeInfo _nodeInfo;
        private ETableStyle _tableStyle;
        private string _tableName;
        private StringCollection _attributesOfDisplay;
        private List<int> _relatedIdentities;
        private List<TableStyleInfo> _styleInfoList;
        private ContentModelInfo _modelInfo;
        private readonly Hashtable _valueHashtable = new Hashtable();

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
            var nodeId = Body.GetQueryInt("NodeID");
            _relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(PublishmentSystemId, nodeId);
            _nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeId);
            _tableName = NodeManager.GetTableName(PublishmentSystemInfo, _nodeInfo);
            _tableStyle = NodeManager.GetTableStyle(PublishmentSystemInfo, _nodeInfo);
            _styleInfoList = TableStyleManager.GetTableStyleInfoList(_tableStyle, _tableName, _relatedIdentities);
            _modelInfo = ContentModelManager.GetContentModelInfo(PublishmentSystemInfo, _nodeInfo.ContentModelId);

            if (_nodeInfo.Additional.IsPreviewContents)
            {
                new Action(() =>
                {
                    DataProvider.ContentDao.DeletePreviewContents(PublishmentSystemId, _tableName, _nodeInfo);
                }).BeginInvoke(null, null);
            }

            if (!HasChannelPermissions(nodeId, AppManager.Permissions.Channel.ContentView, AppManager.Permissions.Channel.ContentAdd, AppManager.Permissions.Channel.ContentEdit, AppManager.Permissions.Channel.ContentDelete, AppManager.Permissions.Channel.ContentTranslate))
            {
                if (!Body.IsAdministratorLoggin)
                {
                    PageUtils.RedirectToLoginPage();
                    return;
                }
                PageUtils.RedirectToErrorPage("您无此栏目的操作权限！");
                return;
            }

            _attributesOfDisplay = TranslateUtils.StringCollectionToStringCollection(NodeManager.GetContentAttributesOfDisplay(PublishmentSystemId, nodeId));

            //this.attributesOfDisplay = TranslateUtils.StringCollectionToStringCollection(this.nodeInfo.Additional.ContentAttributesOfDisplay);

            spContents.ControlToPaginate = rptContents;
            rptContents.ItemDataBound += rptContents_ItemDataBound;
            spContents.ItemsPerPage = PublishmentSystemInfo.Additional.PageSize;

            var administratorName = AdminUtility.IsViewContentOnlySelf(Body.AdministratorName, PublishmentSystemId, nodeId)
                    ? Body.AdministratorName
                    : string.Empty;

            if (Body.IsQueryExists("SearchType"))
            {
                var owningNodeIdList = new List<int>
                {
                    nodeId
                };
                spContents.SelectCommand = DataProvider.ContentDao.GetSelectCommend(_tableStyle, _tableName, PublishmentSystemId, nodeId, permissions.IsSystemAdministrator, owningNodeIdList, Body.GetQueryString("SearchType"), Body.GetQueryString("Keyword"), Body.GetQueryString("DateFrom"), string.Empty, false, ETriState.All, false, false, false, administratorName);
            }
            else
            {
                spContents.SelectCommand = BaiRongDataProvider.ContentDao.GetSelectCommend(_tableName, nodeId, ETriState.All, administratorName);
            }

            //spContents.SortField = BaiRongDataProvider.ContentDao.GetSortFieldName();
            //spContents.SortMode = SortMode.DESC;
            //spContents.OrderByString = ETaxisTypeUtils.GetOrderByString(tableStyle, ETaxisType.OrderByTaxisDesc);
            spContents.OrderByString = ETaxisTypeUtils.GetOrderByString(_tableStyle, ETaxisTypeUtils.GetEnumType(_nodeInfo.Additional.DefaultTaxisType));

            //分页的时候，不去查询总条数，直接使用栏目的属性：ContentNum
            spContents.IsQueryTotalCount = false;
            spContents.TotalCount = _nodeInfo.ContentNum;

            if (!IsPostBack)
            {
                var nodeName = NodeManager.GetNodeNameNavigation(PublishmentSystemId, nodeId);
                BreadCrumbWithTitle(AppManager.Cms.LeftMenu.IdContent, "内容管理", nodeName, string.Empty);

                ltlContentButtons.Text = WebUtils.GetContentCommands(Body.AdministratorName, PublishmentSystemInfo, _nodeInfo, PageUrl, GetRedirectUrl(PublishmentSystemId, _nodeInfo.NodeId), false);
                spContents.DataBind();

                if (_styleInfoList != null)
                {
                    foreach (var styleInfo in _styleInfoList)
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

                ltlColumnHeadRows.Text = TextUtility.GetColumnHeadRowsHtml(_styleInfoList, _attributesOfDisplay, _tableStyle, PublishmentSystemInfo);
                ltlCommandHeadRows.Text = TextUtility.GetCommandHeadRowsHtml(Body.AdministratorName, PublishmentSystemInfo, _nodeInfo, _modelInfo);
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

                if (HasChannelPermissions(contentInfo.NodeId, AppManager.Permissions.Channel.ContentEdit) || Body.AdministratorName == contentInfo.AddUserName)
                {
                    ltlItemEditUrl.Text =
                        $"<a href=\"{WebUtils.GetContentAddEditUrl(PublishmentSystemId, _nodeInfo, contentInfo.Id, PageUrl)}\">编辑</a>";
                }

                ltlColumnItemRows.Text = TextUtility.GetColumnItemRowsHtml(_styleInfoList, _attributesOfDisplay, _valueHashtable, _tableStyle, PublishmentSystemInfo, contentInfo);

                ltlCommandItemRows.Text = TextUtility.GetCommandItemRowsHtml(PublishmentSystemInfo, _modelInfo, contentInfo, PageUrl, Body.AdministratorName);
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
