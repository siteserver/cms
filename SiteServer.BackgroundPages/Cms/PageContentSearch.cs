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
        private ChannelInfo _nodeInfo;
        private List<int> _relatedIdentities;
        private List<TableStyleInfo> _styleInfoList;
        private StringCollection _attributesOfDisplay;
        private List<TableStyleInfo> _attributesOfDisplayStyleInfoList;
        private Dictionary<string, List<HyperLink>> _pluginLinks;
        private bool _isEdit;
        private readonly Dictionary<string, string> _nameValueCacheDict = new Dictionary<string, string>();

        public static string GetRedirectUrl(int siteId)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageContentSearch), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            var permissions = PermissionsManager.GetPermissions(Body.AdminName);

            PageUtils.CheckRequestParameter("SiteId");
            _channelId = Body.IsQueryExists("ChannelId") ? Body.GetQueryInt("ChannelId") : SiteId;

            _isWritingOnly = Body.GetQueryBool("isWritingOnly");

            var administratorName = string.Empty;
            _isSelfOnly = Body.GetQueryBool("isSelfOnly");
            if (!_isSelfOnly)
            {
                administratorName = AdminUtility.IsViewContentOnlySelf(Body.AdminName, SiteId, _channelId) ? Body.AdminName : string.Empty;
            }

            _nodeInfo = ChannelManager.GetChannelInfo(SiteId, _channelId);
            var tableName = ChannelManager.GetTableName(SiteInfo, _nodeInfo);
            _relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(SiteId, _channelId);
            _styleInfoList = TableStyleManager.GetTableStyleInfoList(tableName, _relatedIdentities);
            _attributesOfDisplay = TranslateUtils.StringCollectionToStringCollection(ChannelManager.GetContentAttributesOfDisplay(SiteId, _channelId));
            _attributesOfDisplayStyleInfoList = ContentUtility.GetColumnTableStyleInfoList(SiteInfo, _styleInfoList);
            _pluginLinks = PluginContentManager.GetContentLinks(_nodeInfo);
            _isEdit = TextUtility.IsEdit(SiteInfo, _channelId, Body.AdminName);

            var stateType = Body.IsQueryExists("state") ? ETriStateUtils.GetEnumType(Body.GetQueryString("state")) : ETriState.All;
            var searchType = Body.IsQueryExists("searchType") ? Body.GetQueryString("searchType") : ContentAttribute.Title;
            var dateFrom = Body.IsQueryExists("dateFrom") ? Body.GetQueryString("dateFrom") : DateUtils.GetDateString(DateTime.Now.AddMonths(-1));
            var dateTo = Body.IsQueryExists("dateTo") ? Body.GetQueryString("dateTo") : DateUtils.GetDateString(DateTime.Now);
            var isDuplicate = Body.IsQueryExists("isDuplicate") && Body.GetQueryBool("isDuplicate");
            var keyword = Body.IsQueryExists("keyword") ? Body.GetQueryString("keyword") : string.Empty;

            SpContents.ControlToPaginate = RptContents;
            SpContents.SelectCommand = DataProvider.ContentDao.GetSqlString(tableName, SiteId, _channelId, permissions.IsSystemAdministrator, ProductPermissionsManager.Current.OwningChannelIdList, searchType, keyword, dateFrom, dateTo, true, stateType, !isDuplicate, false, _isWritingOnly, administratorName);
            SpContents.ItemsPerPage = SiteInfo.Additional.PageSize;
            SpContents.SortField = ContentAttribute.Id;
            SpContents.SortMode = SortMode.DESC;
            SpContents.OrderByString = ETaxisTypeUtils.GetContentOrderByString(ETaxisType.OrderByIdDesc);
            RptContents.ItemDataBound += RptContents_ItemDataBound;

            if (!IsPostBack)
            {
                ChannelManager.AddListItems(DdlChannelId.Items, SiteInfo, true, true, Body.AdminName);

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

                if (SiteId != _channelId)
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

                var showPopWinString = ModalAddToGroup.GetOpenWindowStringToContentForMultiChannels(SiteId);
                BtnAddToGroup.Attributes.Add("onclick", showPopWinString);

                showPopWinString = ModalSelectColumns.GetOpenWindowStringToContent(SiteId, _channelId, true);
                BtnSelect.Attributes.Add("onclick", showPopWinString);

                if (AdminUtility.HasChannelPermissions(Body.AdminName, SiteId, SiteId, ConfigManager.Permissions.Channel.ContentCheck))
                {
                    showPopWinString = ModalContentCheck.GetOpenWindowStringForMultiChannels(SiteId, PageUrl);
                    BtnCheck.Attributes.Add("onclick", showPopWinString);
                }
                else
                {
                    PhCheck.Visible = false;
                }

                LtlColumnsHead.Text = TextUtility.GetColumnsHeadHtml(_styleInfoList, _attributesOfDisplay, SiteInfo);
            }

            if (!HasChannelPermissions(_channelId, ConfigManager.Permissions.Channel.ContentAdd)) BtnAddContent.Visible = false;
            if (!HasChannelPermissions(_channelId, ConfigManager.Permissions.Channel.ContentTranslate))
            {
                BtnTranslate.Visible = false;
            }
            else
            {
                BtnTranslate.Attributes.Add("onclick", PageContentTranslate.GetRedirectClickStringForMultiChannels(SiteId, PageUrl));
            }

            if (!HasChannelPermissions(_channelId, ConfigManager.Permissions.Channel.ContentDelete))
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
            var ltlSelect = (Literal)e.Item.FindControl("ltlSelect");

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

        public void AddContent_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(WebUtils.GetContentAddAddUrl(SiteId, _nodeInfo, PageUrl));
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
                    _pageUrl = PageUtils.GetCmsUrl(SiteId, nameof(PageContentSearch), new NameValueCollection
                    {
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
