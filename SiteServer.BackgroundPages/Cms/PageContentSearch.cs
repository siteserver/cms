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
        public DropDownList DdlChannelId;
        public DropDownList DdlState;
        public CheckBox CbIsDuplicate;
        public DropDownList DdlSearchType;
        public TextBox TbKeyword;
        public DateTimeTextBox TbDateFrom;
        public DateTimeTextBox TbDateTo;

        public Repeater RptContents;
        public SqlPager SpContents;
        public Literal LtlColumnHeadRows;
        public Literal LtlCommandHeadRows;

        public Button BtnAddContent;
        public Button BtnAddToGroup;
        public Button BtnDelete;
        public Button BtnTranslate;
        public Button BtnSelect;
        public PlaceHolder PhCheck;
        public Button BtnCheck;

        private bool _isWritingOnly;
        private bool _isSelfOnly;
        private int _channelId;
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

            var permissions = PermissionsManager.GetPermissions(Body.AdminName);

            PageUtils.CheckRequestParameter("PublishmentSystemId");
            _channelId = Body.IsQueryExists("ChannelId") ? Body.GetQueryInt("ChannelId") : PublishmentSystemId;

            _isWritingOnly = Body.GetQueryBool("isWritingOnly");

            var administratorName = string.Empty;
            _isSelfOnly = Body.GetQueryBool("isSelfOnly");
            if (!_isSelfOnly)
            {
                administratorName = AdminUtility.IsViewContentOnlySelf(Body.AdminName, PublishmentSystemId, _channelId) ? Body.AdminName : string.Empty;
            }

            _nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, _channelId);
            _tableStyle = NodeManager.GetTableStyle(PublishmentSystemInfo, _nodeInfo);
            var tableName = NodeManager.GetTableName(PublishmentSystemInfo, _nodeInfo);
            _attributesOfDisplay = TranslateUtils.StringCollectionToStringCollection(NodeManager.GetContentAttributesOfDisplay(PublishmentSystemId, _channelId));
            _relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(PublishmentSystemId, _channelId);
            _tableStyleInfoList = TableStyleManager.GetTableStyleInfoList(_tableStyle, tableName, _relatedIdentities);
            _pluginChannels = PluginCache.GetChannelFeatures(_nodeInfo);

            var stateType = Body.IsQueryExists("state") ? ETriStateUtils.GetEnumType(Body.GetQueryString("state")) : ETriState.All;
            var searchType = Body.IsQueryExists("searchType") ? Body.GetQueryString("searchType") : ContentAttribute.Title;
            var dateFrom = Body.IsQueryExists("dateFrom") ? Body.GetQueryString("dateFrom") : DateUtils.GetDateString(DateTime.Now.AddMonths(-1));
            var dateTo = Body.IsQueryExists("dateTo") ? Body.GetQueryString("dateTo") : DateUtils.GetDateString(DateTime.Now);
            var isDuplicate = Body.IsQueryExists("isDuplicate") && Body.GetQueryBool("isDuplicate");
            var keyword = Body.IsQueryExists("keyword") ? Body.GetQueryString("keyword") : string.Empty;

            SpContents.ControlToPaginate = RptContents;
            SpContents.SelectCommand = DataProvider.ContentDao.GetSelectCommend(_tableStyle, tableName, PublishmentSystemId, _channelId, permissions.IsSystemAdministrator, ProductPermissionsManager.Current.OwningNodeIdList, searchType, keyword, dateFrom, dateTo, true, stateType, !isDuplicate, false, _isWritingOnly, administratorName);
            SpContents.ItemsPerPage = PublishmentSystemInfo.Additional.PageSize;
            SpContents.SortField = ContentAttribute.Id;
            SpContents.SortMode = SortMode.DESC;
            SpContents.OrderByString = ETaxisTypeUtils.GetOrderByString(_tableStyle, ETaxisType.OrderByIdDesc);
            RptContents.ItemDataBound += RptContents_ItemDataBound;

            if (!IsPostBack)
            {
                var pageTitle = _isSelfOnly ? "我的内容" : "内容搜索";
                if (_isWritingOnly)
                {
                    pageTitle = "投稿内容";
                }
                BreadCrumb(AppManager.Cms.LeftMenu.IdContent, pageTitle, string.Empty);

                NodeManager.AddListItems(DdlChannelId.Items, PublishmentSystemInfo, true, true, Body.AdminName);

                DdlSearchType.Items.Add(new ListItem("标题", ContentAttribute.Title));
                if (_tableStyleInfoList != null)
                {
                    foreach (var styleInfo in _tableStyleInfoList)
                    {
                        if (styleInfo.IsVisible && styleInfo.AttributeName != ContentAttribute.AddDate)
                        {
                            var listitem = new ListItem(styleInfo.DisplayName, styleInfo.AttributeName);
                            DdlSearchType.Items.Add(listitem);
                        }
                    }
                }
                DdlSearchType.Items.Add(new ListItem("内容ID", ContentAttribute.Id));
                DdlSearchType.Items.Add(new ListItem("添加者", ContentAttribute.AddUserName));
                DdlSearchType.Items.Add(new ListItem("最后修改者", ContentAttribute.LastEditUserName));

                ETriStateUtils.AddListItems(DdlState, "全部", "已审核", "待审核");

                if (PublishmentSystemId != _channelId)
                {
                    ControlUtils.SelectListItems(DdlChannelId, _channelId.ToString());
                }
                ControlUtils.SelectListItems(DdlState, Body.GetQueryString("State"));
                CbIsDuplicate.Checked = isDuplicate;
                ControlUtils.SelectListItems(DdlSearchType, searchType);
                TbKeyword.Text = keyword;
                TbDateFrom.Text = dateFrom;
                TbDateTo.Text = dateTo;

                SpContents.DataBind();

                var showPopWinString = ModalAddToGroup.GetOpenWindowStringToContentForMultiChannels(PublishmentSystemId);
                BtnAddToGroup.Attributes.Add("onclick", showPopWinString);

                showPopWinString = ModalSelectColumns.GetOpenWindowStringToContent(PublishmentSystemId, _channelId, true);
                BtnSelect.Attributes.Add("onclick", showPopWinString);

                if (AdminUtility.HasChannelPermissions(Body.AdminName, PublishmentSystemId, PublishmentSystemId, AppManager.Permissions.Channel.ContentCheck))
                {
                    showPopWinString = ModalContentCheck.GetOpenWindowStringForMultiChannels(PublishmentSystemId, PageUrl);
                    BtnCheck.Attributes.Add("onclick", showPopWinString);
                }
                else
                {
                    PhCheck.Visible = false;
                }

                LtlColumnHeadRows.Text = TextUtility.GetColumnHeadRowsHtml(_tableStyleInfoList, _attributesOfDisplay, _tableStyle, PublishmentSystemInfo);
                LtlCommandHeadRows.Text = TextUtility.GetCommandHeadRowsHtml(Body.AdminName, PublishmentSystemInfo, _nodeInfo, _pluginChannels);
            }

            if (!HasChannelPermissions(_channelId, AppManager.Permissions.Channel.ContentAdd)) BtnAddContent.Visible = false;
            if (!HasChannelPermissions(_channelId, AppManager.Permissions.Channel.ContentTranslate))
            {
                BtnTranslate.Visible = false;
            }
            else
            {
                BtnTranslate.Attributes.Add("onclick", PageContentTranslate.GetRedirectClickStringForMultiChannels(PublishmentSystemId, PageUrl));
            }

            if (!HasChannelPermissions(_channelId, AppManager.Permissions.Channel.ContentDelete))
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

            if (HasChannelPermissions(contentInfo.NodeId, AppManager.Permissions.Channel.ContentEdit) || Body.AdminName == contentInfo.AddUserName)
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
                ltlCommandItemRows.Text = TextUtility.GetCommandItemRowsHtml(PublishmentSystemInfo, _pluginChannels, contentInfo, PageUrl, Body.AdminName);
            }

            if (ltlSelect != null)
            {
                ltlSelect.Text =
                    $@"<input type=""checkbox"" name=""IDsCollection"" value=""{contentInfo.NodeId}_{contentInfo.Id}"" />";
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
                        {"PublishmentSystemId", PublishmentSystemId.ToString()},
                        {"ChannelId", DdlChannelId.SelectedValue},
                        {"State", DdlState.SelectedValue},
                        {"IsDuplicate", CbIsDuplicate.Checked.ToString()},
                        {"SearchType", DdlSearchType.SelectedValue},
                        {"Keyword", TbKeyword.Text},
                        {"DateFrom", TbDateFrom.Text},
                        {"DateTo", TbDateTo.Text},
                        {"isWritingOnly", _isWritingOnly.ToString()},
                        {"isSelfOnly", _isSelfOnly.ToString()}
                    });
                }
                return _pageUrl;
            }
        }
    }
}
