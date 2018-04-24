using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageContentSearch : BasePageCms
    {
        public DropDownList DdlChannelId;
        public DropDownList DdlState;
        public DropDownList DdlSearchType;
        public TextBox TbKeyword;
        public DateTimeTextBox TbDateFrom;
        public DateTimeTextBox TbDateTo;

        public Repeater RptContents;
        public Pager PgContents;
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
        private ChannelInfo _channelInfo;
        private List<int> _relatedIdentities;
        private List<TableStyleInfo> _styleInfoList;
        private StringCollection _attributesOfDisplay;
        private List<TableStyleInfo> _allStyleInfoList;
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

            PageUtils.CheckRequestParameter("SiteId");
            _channelId = AuthRequest.IsQueryExists("ChannelId") ? AuthRequest.GetQueryInt("ChannelId") : SiteId;

            _isWritingOnly = AuthRequest.GetQueryBool("isWritingOnly");

            var administratorName = string.Empty;
            _isSelfOnly = AuthRequest.GetQueryBool("isSelfOnly");
            if (!_isSelfOnly)
            {
                administratorName = AuthRequest.AdminPermissions.IsViewContentOnlySelf(SiteId, _channelId) ? AuthRequest.AdminName : string.Empty;
            }

            _channelInfo = ChannelManager.GetChannelInfo(SiteId, _channelId);
            var tableName = ChannelManager.GetTableName(SiteInfo, _channelInfo);
            _relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(SiteId, _channelId);
            _styleInfoList = TableStyleManager.GetTableStyleInfoList(tableName, _relatedIdentities);
            _attributesOfDisplay = TranslateUtils.StringCollectionToStringCollection(ChannelManager.GetContentAttributesOfDisplay(SiteId, _channelId));
            _allStyleInfoList = ContentUtility.GetAllTableStyleInfoList(_styleInfoList);
            _pluginLinks = PluginContentManager.GetContentLinks(_channelInfo);
            _isEdit = TextUtility.IsEdit(SiteInfo, _channelId, AuthRequest.AdminPermissions);

            var stateType = AuthRequest.IsQueryExists("state") ? ETriStateUtils.GetEnumType(AuthRequest.GetQueryString("state")) : ETriState.All;
            var searchType = AuthRequest.IsQueryExists("searchType") ? AuthRequest.GetQueryString("searchType") : ContentAttribute.Title;
            var dateFrom = AuthRequest.IsQueryExists("dateFrom") ? AuthRequest.GetQueryString("dateFrom") : string.Empty;
            var dateTo = AuthRequest.IsQueryExists("dateTo") ? AuthRequest.GetQueryString("dateTo") : string.Empty;
            var keyword = AuthRequest.IsQueryExists("keyword") ? AuthRequest.GetQueryString("keyword") : string.Empty;

            //SpContents.ControlToPaginate = RptContents;
            //SpContents.SelectCommand = DataProvider.ContentDao.GetSqlString(tableName, SiteId, _channelId, AuthRequest.AdminPermissions.IsSystemAdministrator, AuthRequest.AdminPermissions.OwningChannelIdList, searchType, keyword, dateFrom, dateTo, true, stateType, !isDuplicate, false, _isWritingOnly, administratorName);
            //SpContents.ItemsPerPage = SiteInfo.Additional.PageSize;
            //SpContents.SortField = ContentAttribute.Id;
            //SpContents.SortMode = SortMode.DESC;
            //SpContents.OrderByString = ETaxisTypeUtils.GetContentOrderByString(ETaxisType.OrderByIdDesc);
            RptContents.ItemDataBound += RptContents_ItemDataBound;

            var allLowerAttributeNameList = TableMetadataManager.GetAllLowerAttributeNameList(tableName);
            var pagerParam = new PagerParam
            {
                ControlToPaginate = RptContents,
                TableName = tableName,
                PageSize = SiteInfo.Additional.PageSize,
                Page = AuthRequest.GetQueryInt(Pager.QueryNamePage, 1),
                OrderSqlString = ETaxisTypeUtils.GetContentOrderByString(ETaxisType.OrderByIdDesc),
                ReturnColumnNames =
                    DataProvider.ContentDao.GetPagerReturnColumnNames(allLowerAttributeNameList, _attributesOfDisplay)
            };
            
            var channelIdList = ChannelManager.GetChannelIdList(_channelInfo, EScopeType.All, string.Empty, string.Empty, _channelInfo.ContentModelPluginId);

            var searchChannelIdList = new List<int>();
            if (AuthRequest.AdminPermissions.IsSystemAdministrator)
            {
                searchChannelIdList = channelIdList;
            }
            else
            {
                foreach (var theChannelId in channelIdList)
                {
                    if (AuthRequest.AdminPermissions.OwningChannelIdList.Contains(theChannelId))
                    {
                        searchChannelIdList.Add(theChannelId);
                    }
                }
            }

            pagerParam.WhereSqlString = DataProvider.ContentDao.GetPagerWhereSqlString(allLowerAttributeNameList,
                SiteId, _channelId, AuthRequest.AdminPermissions.IsSystemAdministrator, searchChannelIdList, searchType, keyword,
                dateFrom, dateTo, true, stateType, false, _isWritingOnly, administratorName);
            pagerParam.TotalCount =
                DataProvider.DatabaseDao.GetPageTotalCount(tableName, pagerParam.WhereSqlString);

            PgContents.Param = pagerParam;

            if (!IsPostBack)
            {
                ChannelManager.AddListItems(DdlChannelId.Items, SiteInfo, true, true, AuthRequest.AdminPermissions);

                foreach (var styleInfo in _allStyleInfoList)
                {
                    var listitem = new ListItem(styleInfo.DisplayName, styleInfo.AttributeName);
                    DdlSearchType.Items.Add(listitem);
                }

                ETriStateUtils.AddListItems(DdlState, "全部", "已审核", "待审核");

                if (SiteId != _channelId)
                {
                    ControlUtils.SelectSingleItem(DdlChannelId, _channelId.ToString());
                }
                ControlUtils.SelectSingleItem(DdlState, AuthRequest.GetQueryString("State"));
                ControlUtils.SelectSingleItem(DdlSearchType, searchType);
                TbKeyword.Text = keyword;
                TbDateFrom.Text = dateFrom;
                TbDateTo.Text = dateTo;

                PgContents.DataBind();

                var showPopWinString = ModalAddToGroup.GetOpenWindowStringToContentForMultiChannels(SiteId);
                BtnAddToGroup.Attributes.Add("onclick", showPopWinString);

                showPopWinString = ModalSelectColumns.GetOpenWindowString(SiteId, _channelId, true);
                BtnSelect.Attributes.Add("onclick", showPopWinString);

                if (HasChannelPermissions(SiteId, ConfigManager.ChannelPermissions.ContentCheck))
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

            if (!HasChannelPermissions(_channelId, ConfigManager.ChannelPermissions.ContentAdd)) BtnAddContent.Visible = false;
            if (!HasChannelPermissions(_channelId, ConfigManager.ChannelPermissions.ContentTranslate))
            {
                BtnTranslate.Visible = false;
            }
            else
            {
                BtnTranslate.Attributes.Add("onclick", PageContentTranslate.GetRedirectClickStringForMultiChannels(SiteId, PageUrl));
            }

            if (!HasChannelPermissions(_channelId, ConfigManager.ChannelPermissions.ContentDelete))
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

            ltlColumns.Text = TextUtility.GetColumnsHtml(_nameValueCacheDict, SiteInfo, contentInfo, _attributesOfDisplay, _allStyleInfoList);

            string nodeName;
            if (!_nameValueCacheDict.TryGetValue(contentInfo.ChannelId.ToString(), out nodeName))
            {
                nodeName = ChannelManager.GetChannelNameNavigation(SiteId, contentInfo.ChannelId);
                _nameValueCacheDict[contentInfo.ChannelId.ToString()] = nodeName;
            }

            ltlChannel.Text = nodeName;

            ltlStatus.Text =
                $@"<a href=""javascript:;"" title=""设置内容状态"" onclick=""{ModalCheckState.GetOpenWindowString(SiteId, contentInfo, PageUrl)}"">{CheckManager.GetCheckState(SiteInfo, contentInfo.IsChecked, contentInfo.CheckedLevel)}</a>";

            ltlCommands.Text = TextUtility.GetCommandsHtml(SiteInfo, _pluginLinks, contentInfo, PageUrl, AuthRequest.AdminName, _isEdit);

            ltlSelect.Text = $@"<input type=""checkbox"" name=""IDsCollection"" value=""{contentInfo.ChannelId}_{contentInfo.Id}"" />";
        }

        public void AddContent_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(WebUtils.GetContentAddAddUrl(SiteId, _channelInfo, PageUrl));
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
