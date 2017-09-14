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
    public class PageContentSearch : BasePageCms
    {
        public DropDownList NodeIDDropDownList;
        public DropDownList State;
        public CheckBox IsDuplicate;
        public DropDownList SearchType;
        public TextBox Keyword;
        public DateTimeTextBox DateFrom;
        public DateTimeTextBox DateTo;

        public Repeater rptContents;
        public SqlPager spContents;
        public Literal ltlColumnHeadRows;
        public Literal ltlCommandHeadRows;

        public Button AddContent;
        public Button AddToGroup;
        public Button Delete;
        public Button Translate;
        public Button SelectButton;
        public PlaceHolder CheckPlaceHolder;
        public Button Check;

        private bool _isWritingOnly;
        private bool _isSelfOnly;
        private int _nodeId;
        private NodeInfo _nodeInfo;
        private ETableStyle _tableStyle;
        private StringCollection _attributesOfDisplay;
        private List<int> _relatedIdentities;
        private List<TableStyleInfo> _tableStyleInfoList;
        private Dictionary<string, IChannel> _pluginChannels;
        private readonly Hashtable _valueHashtable = new Hashtable();

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetCmsUrl(nameof(PageContentSearch), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            var permissions = PermissionsManager.GetPermissions(Body.AdministratorName);

            PageUtils.CheckRequestParameter("PublishmentSystemID");
            if (Body.IsQueryExists("NodeID"))
            {
                _nodeId = Body.GetQueryInt("NodeID");
            }
            else
            {
                _nodeId = PublishmentSystemId;
            }

            _isWritingOnly = Body.GetQueryBool("isWritingOnly");

            var administratorName = string.Empty;
            _isSelfOnly = Body.GetQueryBool("isSelfOnly");
            if (!_isSelfOnly)
            {
                administratorName = AdminUtility.IsViewContentOnlySelf(Body.AdministratorName, PublishmentSystemId, _nodeId) ? Body.AdministratorName : string.Empty;
            }

            _nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, _nodeId);
            _tableStyle = NodeManager.GetTableStyle(PublishmentSystemInfo, _nodeInfo);
            var tableName = NodeManager.GetTableName(PublishmentSystemInfo, _nodeInfo);
            _attributesOfDisplay = TranslateUtils.StringCollectionToStringCollection(NodeManager.GetContentAttributesOfDisplay(PublishmentSystemId, _nodeId));
            _relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(PublishmentSystemId, _nodeId);
            _tableStyleInfoList = TableStyleManager.GetTableStyleInfoList(_tableStyle, tableName, _relatedIdentities);
            _pluginChannels = PluginCache.GetChannelFeatures(_nodeInfo);

            spContents.ControlToPaginate = rptContents;
            if (string.IsNullOrEmpty(Body.GetQueryString("NodeID")))
            {
                var stateType = ETriStateUtils.GetEnumType(State.SelectedValue);
                spContents.SelectCommand = DataProvider.ContentDao.GetSelectCommend(_tableStyle, tableName, PublishmentSystemId, _nodeId, permissions.IsSystemAdministrator, ProductPermissionsManager.Current.OwningNodeIdList, SearchType.SelectedValue, Keyword.Text, DateUtils.GetDateString(DateTime.Now.AddMonths(-1)), DateUtils.GetDateString(DateTime.Now), true, stateType, !IsDuplicate.Checked, false, _isWritingOnly, administratorName);
            }
            else
            {
                var stateType = ETriStateUtils.GetEnumType(Body.GetQueryString("State"));
                spContents.SelectCommand = DataProvider.ContentDao.GetSelectCommend(_tableStyle, tableName, PublishmentSystemId, _nodeId, permissions.IsSystemAdministrator, ProductPermissionsManager.Current.OwningNodeIdList, Body.GetQueryString("SearchType"), Body.GetQueryString("Keyword"), Body.GetQueryString("DateFrom"), Body.GetQueryString("DateTo"), true, stateType, !Body.GetQueryBool("IsDuplicate"), false, _isWritingOnly, administratorName);
            }
            spContents.ItemsPerPage = PublishmentSystemInfo.Additional.PageSize;
            spContents.SortField = ContentAttribute.Id;
            spContents.SortMode = SortMode.DESC;
            spContents.OrderByString = ETaxisTypeUtils.GetOrderByString(_tableStyle, ETaxisType.OrderByIdDesc);
            rptContents.ItemDataBound += rptContents_ItemDataBound;

            if (!IsPostBack)
            {
                var pageTitle = _isSelfOnly ? "我的内容" : "内容搜索";
                if (_isWritingOnly)
                {
                    pageTitle = "投稿内容";
                }
                BreadCrumb(AppManager.Cms.LeftMenu.IdContent, pageTitle, string.Empty);

                NodeManager.AddListItems(NodeIDDropDownList.Items, PublishmentSystemInfo, true, true, Body.AdministratorName);

                if (_tableStyleInfoList != null)
                {
                    foreach (var styleInfo in _tableStyleInfoList)
                    {
                        if (styleInfo.IsVisible && styleInfo.AttributeName != ContentAttribute.AddDate)
                        {
                            var listitem = new ListItem(styleInfo.DisplayName, styleInfo.AttributeName);
                            SearchType.Items.Add(listitem);
                        }
                    }
                }

                ETriStateUtils.AddListItems(State, "全部", "已审核", "待审核");

                //添加隐藏属性
                SearchType.Items.Add(new ListItem("内容ID", ContentAttribute.Id));
                SearchType.Items.Add(new ListItem("添加者", ContentAttribute.AddUserName));
                SearchType.Items.Add(new ListItem("最后修改者", ContentAttribute.LastEditUserName));

                if (Body.IsQueryExists("NodeID"))
                {
                    if (PublishmentSystemId != _nodeId)
                    {
                        ControlUtils.SelectListItems(NodeIDDropDownList, _nodeId.ToString());
                    }
                    ControlUtils.SelectListItems(State, Body.GetQueryString("State"));
                    IsDuplicate.Checked = Body.GetQueryBool("IsDuplicate");
                    ControlUtils.SelectListItems(SearchType, Body.GetQueryString("SearchType"));
                    Keyword.Text = Body.GetQueryString("Keyword");
                    DateFrom.Text = Body.GetQueryString("DateFrom");
                    DateTo.Text = Body.GetQueryString("DateTo");
                }
                else
                {
                    DateFrom.Text = DateUtils.GetDateString(DateTime.Now.AddMonths(-1));
                    DateTo.Text = DateUtils.GetDateString(DateTime.Now);
                }

                spContents.DataBind();

                var showPopWinString = ModalAddToGroup.GetOpenWindowStringToContentForMultiChannels(PublishmentSystemId);
                AddToGroup.Attributes.Add("onclick", showPopWinString);

                showPopWinString = ModalSelectColumns.GetOpenWindowStringToContent(PublishmentSystemId, _nodeId, true);
                SelectButton.Attributes.Add("onclick", showPopWinString);

                if (AdminUtility.HasChannelPermissions(Body.AdministratorName, PublishmentSystemId, PublishmentSystemId, AppManager.Permissions.Channel.ContentCheck))
                {
                    showPopWinString = ModalContentCheck.GetOpenWindowStringForMultiChannels(PublishmentSystemId, PageUrl);
                    Check.Attributes.Add("onclick", showPopWinString);
                }
                else
                {
                    CheckPlaceHolder.Visible = false;
                }

                ltlColumnHeadRows.Text = TextUtility.GetColumnHeadRowsHtml(_tableStyleInfoList, _attributesOfDisplay, _tableStyle, PublishmentSystemInfo);
                ltlCommandHeadRows.Text = TextUtility.GetCommandHeadRowsHtml(Body.AdministratorName, PublishmentSystemInfo, _nodeInfo, _pluginChannels);
            }

            if (!HasChannelPermissions(_nodeId, AppManager.Permissions.Channel.ContentAdd)) AddContent.Visible = false;
            if (!HasChannelPermissions(_nodeId, AppManager.Permissions.Channel.ContentTranslate))
            {
                Translate.Visible = false;
            }
            else
            {
                Translate.Attributes.Add("onclick", PageContentTranslate.GetRedirectClickStringForMultiChannels(PublishmentSystemId, PageUrl));
            }

            if (!HasChannelPermissions(_nodeId, AppManager.Permissions.Channel.ContentDelete))
            {
                Delete.Visible = false;
            }
            else
            {
                Delete.Attributes.Add("onclick", PageContentDelete.GetRedirectClickStringForMultiChannels(PublishmentSystemId, false, PageUrl));
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var ltlItemTitle = e.Item.FindControl("ltlItemTitle") as Literal;
                var ltlChannel = e.Item.FindControl("ltlChannel") as Literal;
                var ltlColumnItemRows = e.Item.FindControl("ltlColumnItemRows") as Literal;
                var ltlItemStatus = e.Item.FindControl("ltlItemStatus") as Literal;
                var ltlItemEditUrl = e.Item.FindControl("ltlItemEditUrl") as Literal;
                var ltlCommandItemRows = e.Item.FindControl("ltlCommandItemRows") as Literal;
                var ltlSelect = e.Item.FindControl("ltlSelect") as Literal;

                var contentInfo = new ContentInfo(e.Item.DataItem);
                var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, contentInfo.NodeId);

                if (ltlItemTitle != null)
                {
                    ltlItemTitle.Text = WebUtils.GetContentTitle(PublishmentSystemInfo, contentInfo, PageUrl);
                }
                var nodeName = _valueHashtable[contentInfo.NodeId] as string;
                if (nodeName == null)
                {
                    nodeName = NodeManager.GetNodeNameNavigation(PublishmentSystemId, contentInfo.NodeId);
                    _valueHashtable[contentInfo.NodeId] = nodeName;
                }
                if (ltlChannel != null) ltlChannel.Text = nodeName;

                var showPopWinString = ModalCheckState.GetOpenWindowString(PublishmentSystemId, contentInfo, PageUrl);
                if (ltlItemStatus != null)
                {
                    ltlItemStatus.Text =
                        $@"<a href=""javascript:;"" title=""设置内容状态"" onclick=""{showPopWinString}"">{LevelManager.GetCheckState(
                            PublishmentSystemInfo, contentInfo.IsChecked, contentInfo.CheckedLevel)}</a>";
                }

                if (HasChannelPermissions(contentInfo.NodeId, AppManager.Permissions.Channel.ContentEdit) || Body.AdministratorName == contentInfo.AddUserName)
                {
                    if (ltlItemEditUrl != null)
                    {
                        ltlItemEditUrl.Text =
                            $"<a href=\"{WebUtils.GetContentAddEditUrl(PublishmentSystemId, nodeInfo, contentInfo.Id, PageUrl)}\">编辑</a>";
                    }
                }

                if (ltlColumnItemRows != null)
                {
                    ltlColumnItemRows.Text = TextUtility.GetColumnItemRowsHtml(_tableStyleInfoList, _attributesOfDisplay, _valueHashtable, _tableStyle, PublishmentSystemInfo, contentInfo);
                }

                if (ltlCommandItemRows != null)
                {
                    ltlCommandItemRows.Text = TextUtility.GetCommandItemRowsHtml(PublishmentSystemInfo, _pluginChannels, contentInfo, PageUrl, Body.AdministratorName);
                }

                if (ltlSelect != null)
                {
                    ltlSelect.Text =
                        $@"<input type=""checkbox"" name=""IDsCollection"" value=""{contentInfo.NodeId}_{contentInfo.Id}"" />";
                }
            }
        }

        public void AddContent_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(WebUtils.GetContentAddAddUrl(PublishmentSystemId, _nodeInfo, PageUrl));
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
                    _pageUrl = PageUtils.GetCmsUrl(nameof(PageContentSearch), new NameValueCollection
                    {
                        {"PublishmentSystemID", PublishmentSystemId.ToString()},
                        {"NodeID", NodeIDDropDownList.SelectedValue},
                        {"State", State.SelectedValue},
                        {"IsDuplicate", IsDuplicate.Checked.ToString()},
                        {"SearchType", SearchType.SelectedValue},
                        {"Keyword", Keyword.Text},
                        {"DateFrom", DateFrom.Text},
                        {"DateTo", DateTo.Text},
                        {"isWritingOnly", _isWritingOnly.ToString()},
                        {"isSelfOnly", _isSelfOnly.ToString()}
                    });
                }
                return _pageUrl;
            }
        }
    }
}
