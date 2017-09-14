using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Permissions;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Core.User;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin;
using SiteServer.Plugin.Features;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageContent : BasePageCms
    {
        public Repeater RptContents;
        public SqlPager SpContents;
        public Literal LtlHeadRows;
        public Literal LtlHeadCommand;
        public Literal LtlButtons;
        public DateTimeTextBox TbDateFrom;
        public DropDownList DdlSearchType;
        public TextBox TbKeyword;

        private NodeInfo _nodeInfo;
        private ETableStyle _tableStyle;
        private string _tableName;
        private StringCollection _attributesOfDisplay;
        private List<int> _relatedIdentities;
        private List<TableStyleInfo> _styleInfoList;
        private Dictionary<string, IChannel> _pluginChannels;
        private bool _isEdit;
        private bool _isComment;
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
            _pluginChannels = PluginCache.GetChannelFeatures(_nodeInfo);
            _isEdit = TextUtility.IsEdit(PublishmentSystemInfo, nodeId, Body.AdministratorName);
            _isComment = TextUtility.IsComment(PublishmentSystemInfo, nodeId, Body.AdministratorName);

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

            SpContents.ControlToPaginate = RptContents;
            RptContents.ItemDataBound += rptContents_ItemDataBound;
            SpContents.ItemsPerPage = PublishmentSystemInfo.Additional.PageSize;

            var administratorName = AdminUtility.IsViewContentOnlySelf(Body.AdministratorName, PublishmentSystemId, nodeId)
                    ? Body.AdministratorName
                    : string.Empty;

            if (Body.IsQueryExists("SearchType"))
            {
                var owningNodeIdList = new List<int>
                {
                    nodeId
                };
                SpContents.SelectCommand = DataProvider.ContentDao.GetSelectCommend(_tableStyle, _tableName, PublishmentSystemId, nodeId, permissions.IsSystemAdministrator, owningNodeIdList, Body.GetQueryString("SearchType"), Body.GetQueryString("Keyword"), Body.GetQueryString("DateFrom"), string.Empty, false, ETriState.All, false, false, false, administratorName);
            }
            else
            {
                SpContents.SelectCommand = BaiRongDataProvider.ContentDao.GetSelectCommend(_tableName, nodeId, ETriState.All, administratorName);
            }

            //spContents.SortField = BaiRongDataProvider.ContentDao.GetSortFieldName();
            //spContents.SortMode = SortMode.DESC;
            //spContents.OrderByString = ETaxisTypeUtils.GetOrderByString(tableStyle, ETaxisType.OrderByTaxisDesc);
            SpContents.OrderByString = ETaxisTypeUtils.GetOrderByString(_tableStyle, ETaxisTypeUtils.GetEnumType(_nodeInfo.Additional.DefaultTaxisType));

            //分页的时候，不去查询总条数，直接使用栏目的属性：ContentNum
            SpContents.IsQueryTotalCount = false;
            SpContents.TotalCount = _nodeInfo.ContentNum;

            if (!IsPostBack)
            {
                var nodeName = NodeManager.GetNodeNameNavigation(PublishmentSystemId, nodeId);
                BreadCrumbWithTitle(AppManager.Cms.LeftMenu.IdContent, "内容管理", nodeName, string.Empty);

                LtlButtons.Text = WebUtils.GetContentCommands(Body.AdministratorName, PublishmentSystemInfo, _nodeInfo, PageUrl, GetRedirectUrl(PublishmentSystemId, _nodeInfo.NodeId), false);
                SpContents.DataBind();

                if (_styleInfoList != null)
                {
                    foreach (var styleInfo in _styleInfoList)
                    {
                        if (styleInfo.IsVisible)
                        {
                            var listitem = new ListItem(styleInfo.DisplayName, styleInfo.AttributeName);
                            DdlSearchType.Items.Add(listitem);
                        }
                    }
                }

                //添加隐藏属性
                DdlSearchType.Items.Add(new ListItem("内容ID", ContentAttribute.Id));
                DdlSearchType.Items.Add(new ListItem("添加者", ContentAttribute.AddUserName));
                DdlSearchType.Items.Add(new ListItem("最后修改者", ContentAttribute.LastEditUserName));
                DdlSearchType.Items.Add(new ListItem("内容组", ContentAttribute.ContentGroupNameCollection));

                if (Body.IsQueryExists("SearchType"))
                {
                    TbDateFrom.Text = Body.GetQueryString("DateFrom");
                    ControlUtils.SelectListItems(DdlSearchType, Body.GetQueryString("SearchType"));
                    TbKeyword.Text = Body.GetQueryString("Keyword");
                    LtlButtons.Text += @"
<script>
$(document).ready(function() {
	$('#contentSearch').show();
});
</script>
";
                }

                LtlHeadRows.Text = TextUtility.GetColumnHeadRowsHtml(_styleInfoList, _attributesOfDisplay, _tableStyle, PublishmentSystemInfo);
                var commandCount = 0;
                if (_isEdit)
                {
                    commandCount += 1;
                }
                if (_isComment)
                {
                    commandCount += 1;
                }
                commandCount += TextUtility.GetContentLinkCount(PublishmentSystemInfo, _pluginChannels);
                LtlHeadCommand.Text = $@"<td width=""{commandCount * 70}"">操作</td>";
            }
        }

        private void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var ltlTitle = (Literal)e.Item.FindControl("ltlTitle");
            var ltlRows = (Literal)e.Item.FindControl("ltlRows");
            var ltlStatus = (Literal)e.Item.FindControl("ltlStatus");
            var ltlCommands = (Literal)e.Item.FindControl("ltlCommands");

            var contentInfo = new ContentInfo(e.Item.DataItem);

            ltlTitle.Text = WebUtils.GetContentTitle(PublishmentSystemInfo, contentInfo, PageUrl);

            ltlRows.Text = TextUtility.GetColumnItemRowsHtml(_styleInfoList, _attributesOfDisplay, _valueHashtable, _tableStyle, PublishmentSystemInfo, contentInfo);

            ltlStatus.Text =
                $@"<a href=""javascript:;"" title=""设置内容状态"" onclick=""{ModalCheckState.GetOpenWindowString(PublishmentSystemId, contentInfo, PageUrl)}"">{LevelManager.GetCheckState(
                    PublishmentSystemInfo, contentInfo.IsChecked, contentInfo.CheckedLevel)}</a>";

            ltlCommands.Text = TextUtility.GetCommandHtml(PublishmentSystemInfo, _pluginChannels, contentInfo, PageUrl, Body.AdministratorName, _isEdit, _isComment);
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
                        {"DateFrom", TbDateFrom.Text},
                        {"SearchType", DdlSearchType.SelectedValue},
                        {"Keyword", TbKeyword.Text},
                        {"page", Body.GetQueryInt("page", 1).ToString()}
                    });
                }
                return _pageUrl;
            }
        }
    }
}
