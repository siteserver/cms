using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Table;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;
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
        public Literal LtlColumnsHead;
        public Literal LtlCommandsHead;

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
        private List<int> _relatedIdentities;
        private List<TableStyleInfo> _styleInfoList;
        private StringCollection _attributesOfDisplay;
        private List<TableStyleInfo> _attributesOfDisplayStyleInfoList;
        private Dictionary<string, IContentRelated> _pluginChannels;
        private bool _isEdit;
        private readonly Dictionary<string, string> _nameValueCacheDict = new Dictionary<string, string>();

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
            var tableName = NodeManager.GetTableName(PublishmentSystemInfo, _nodeInfo);
            _relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(PublishmentSystemId, _channelId);
            _styleInfoList = TableStyleManager.GetTableStyleInfoList(tableName, _relatedIdentities);
            _attributesOfDisplay = TranslateUtils.StringCollectionToStringCollection(NodeManager.GetContentAttributesOfDisplay(PublishmentSystemId, _channelId));
            _attributesOfDisplayStyleInfoList = ContentUtility.GetColumnTableStyleInfoList(PublishmentSystemInfo, _styleInfoList);
            _pluginChannels = PluginManager.GetContentRelatedFeatures(_nodeInfo);
            _isEdit = TextUtility.IsEdit(PublishmentSystemInfo, _channelId, Body.AdminName);

            var stateType = Body.IsQueryExists("state") ? ETriStateUtils.GetEnumType(Body.GetQueryString("state")) : ETriState.All;
            var searchType = Body.IsQueryExists("searchType") ? Body.GetQueryString("searchType") : ContentAttribute.Title;
            var dateFrom = Body.IsQueryExists("dateFrom") ? Body.GetQueryString("dateFrom") : DateUtils.GetDateString(DateTime.Now.AddMonths(-1));
            var dateTo = Body.IsQueryExists("dateTo") ? Body.GetQueryString("dateTo") : DateUtils.GetDateString(DateTime.Now);
            var isDuplicate = Body.IsQueryExists("isDuplicate") && Body.GetQueryBool("isDuplicate");
            var keyword = Body.IsQueryExists("keyword") ? Body.GetQueryString("keyword") : string.Empty;

            SpContents.ControlToPaginate = RptContents;
            SpContents.SelectCommand = DataProvider.ContentDao.GetSelectCommend(tableName, PublishmentSystemId, _channelId, permissions.IsSystemAdministrator, ProductPermissionsManager.Current.OwningNodeIdList, searchType, keyword, dateFrom, dateTo, true, stateType, !isDuplicate, false, _isWritingOnly, administratorName);
            SpContents.ItemsPerPage = PublishmentSystemInfo.Additional.PageSize;
            SpContents.SortField = ContentAttribute.Id;
            SpContents.SortMode = SortMode.DESC;
            SpContents.OrderByString = ETaxisTypeUtils.GetContentOrderByString(ETaxisType.OrderByIdDesc);
            RptContents.ItemDataBound += RptContents_ItemDataBound;

            if (!IsPostBack)
            {
                NodeManager.AddListItems(DdlChannelId.Items, PublishmentSystemInfo, true, true, Body.AdminName);

                DdlSearchType.Items.Add(new ListItem("标题", ContentAttribute.Title));
                if (_styleInfoList != null)
                {
                    foreach (var styleInfo in _styleInfoList)
                    {
                        if (styleInfo.AttributeName != ContentAttribute.AddDate)
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
                    ControlUtils.SelectSingleItem(DdlChannelId, _channelId.ToString());
                }
                ControlUtils.SelectSingleItem(DdlState, Body.GetQueryString("State"));
                CbIsDuplicate.Checked = isDuplicate;
                ControlUtils.SelectSingleItem(DdlSearchType, searchType);
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

                LtlColumnsHead.Text = TextUtility.GetColumnsHeadHtml(_styleInfoList, _attributesOfDisplay, PublishmentSystemInfo);
                LtlCommandsHead.Text = TextUtility.GetCommandsHeadHtml(PublishmentSystemInfo, _pluginChannels, _isEdit);
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

            var contentInfo = new ContentInfo(e.Item.DataItem);

            var ltlTitle = (Literal)e.Item.FindControl("ltlTitle");
            var ltlChannel = (Literal)e.Item.FindControl("ltlChannel");
            var ltlColumns = (Literal)e.Item.FindControl("ltlColumns");
            var ltlStatus = (Literal)e.Item.FindControl("ltlStatus");
            var ltlCommands = (Literal)e.Item.FindControl("ltlCommands");
            var ltlSelect = (Literal)e.Item.FindControl("ltlSelect");

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
