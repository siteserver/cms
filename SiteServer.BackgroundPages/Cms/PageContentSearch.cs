using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.Plugin;
using SiteServer.Plugin;
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

        public Button BtnAddToGroup;
        public Button BtnDelete;
        public Button BtnTranslate;
        public Button BtnSelect;
        public PlaceHolder PhCheck;
        public Button BtnCheck;
        public PlaceHolder PhTrash;
        public Button BtnRestore;
        public Button BtnRestoreAll;
        public Button BtnDeleteAll;

        private bool _isCheckOnly;
        private bool _isTrashOnly;
        private bool _isWritingOnly;
        private bool _isAdminOnly;
        private int _channelId;
        private ChannelInfo _channelInfo;
        private List<TableStyleInfo> _styleInfoList;
        private StringCollection _attributesOfDisplay;
        private List<TableStyleInfo> _allStyleInfoList;
        private List<string> _pluginIds;
        private Dictionary<string, Dictionary<string, Func<IContentContext, string>>> _pluginColumns;
        private bool _isEdit;
        private readonly Dictionary<string, string> _nameValueCacheDict = new Dictionary<string, string>();

        public static string GetRedirectUrlCheck(int siteId)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageContentSearch), new NameValueCollection
            {
                {"isCheckOnly", true.ToString() }
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId");
            _channelId = AuthRequest.IsQueryExists("channelId") ? AuthRequest.GetQueryInt("channelId") : SiteId;

            _isCheckOnly = AuthRequest.GetQueryBool("isCheckOnly");
            _isTrashOnly = AuthRequest.GetQueryBool("isTrashOnly");
            _isWritingOnly = AuthRequest.GetQueryBool("isWritingOnly");
            _isAdminOnly = AuthRequest.GetQueryBool("isAdminOnly");

            _channelInfo = ChannelManager.GetChannelInfo(SiteId, _channelId);
            var tableName = ChannelManager.GetTableName(SiteInfo, _channelInfo);
            _styleInfoList = TableStyleManager.GetContentStyleInfoList(SiteInfo, _channelInfo);
            _attributesOfDisplay = TranslateUtils.StringCollectionToStringCollection(ChannelManager.GetContentAttributesOfDisplay(SiteId, _channelId));
            _allStyleInfoList = ContentUtility.GetAllTableStyleInfoList(_styleInfoList);
            _pluginIds = PluginContentManager.GetContentPluginIds(_channelInfo);
            _pluginColumns = PluginContentManager.GetContentColumns(_pluginIds);
            _isEdit = TextUtility.IsEdit(SiteInfo, _channelId, AuthRequest.AdminPermissionsImpl);

            var state = AuthRequest.IsQueryExists("state") ? AuthRequest.GetQueryInt("state") : CheckManager.LevelInt.All;
            var searchType = AuthRequest.IsQueryExists("searchType") ? AuthRequest.GetQueryString("searchType") : ContentAttribute.Title;
            var dateFrom = AuthRequest.IsQueryExists("dateFrom") ? AuthRequest.GetQueryString("dateFrom") : string.Empty;
            var dateTo = AuthRequest.IsQueryExists("dateTo") ? AuthRequest.GetQueryString("dateTo") : string.Empty;
            var keyword = AuthRequest.IsQueryExists("keyword") ? AuthRequest.GetQueryString("keyword") : string.Empty;

            var checkedLevel = 5;
            var isChecked = true;
            foreach (var owningChannelId in AuthRequest.AdminPermissionsImpl.ChannelIdList)
            {
                int checkedLevelByChannelId;
                var isCheckedByChannelId = CheckManager.GetUserCheckLevel(AuthRequest.AdminPermissionsImpl, SiteInfo, owningChannelId, out checkedLevelByChannelId);
                if (checkedLevel > checkedLevelByChannelId)
                {
                    checkedLevel = checkedLevelByChannelId;
                }
                if (!isCheckedByChannelId)
                {
                    isChecked = false;
                }
            }

            RptContents.ItemDataBound += RptContents_ItemDataBound;

            var allAttributeNameList = TableColumnManager.GetTableColumnNameList(tableName, DataType.Text);
            var onlyAdminId = _isAdminOnly
                ? AuthRequest.AdminId
                : AuthRequest.AdminPermissionsImpl.GetOnlyAdminId(SiteInfo.Id, _channelInfo.Id);
            var whereString = DataProvider.ContentDao.GetPagerWhereSqlString(SiteInfo, _channelInfo,
                searchType, keyword,
                dateFrom, dateTo, state, _isCheckOnly, false, _isTrashOnly, _isWritingOnly, onlyAdminId,
                AuthRequest.AdminPermissionsImpl,
                allAttributeNameList);

            PgContents.Param = new PagerParam
            {
                ControlToPaginate = RptContents,
                TableName = tableName,
                PageSize = SiteInfo.Additional.PageSize,
                Page = AuthRequest.GetQueryInt(Pager.QueryNamePage, 1),
                OrderSqlString = ETaxisTypeUtils.GetContentOrderByString(ETaxisType.OrderByIdDesc),
                ReturnColumnNames = TranslateUtils.ObjectCollectionToString(allAttributeNameList),
                WhereSqlString = whereString,
                TotalCount = DataProvider.DatabaseDao.GetPageTotalCount(tableName, whereString)
            };

            if (IsPostBack) return;

            if (_isTrashOnly)
            {
                if (AuthRequest.IsQueryExists("IsDeleteAll"))
                {
                    DataProvider.ContentDao.DeleteContentsByTrash(SiteId, _channelId, tableName);
                    AuthRequest.AddSiteLog(SiteId, "清空回收站");
                    SuccessMessage("成功清空回收站!");
                }
                else if (AuthRequest.IsQueryExists("IsRestore"))
                {
                    var idsDictionary = ContentUtility.GetIDsDictionary(Request.QueryString);
                    foreach (var channelId in idsDictionary.Keys)
                    {
                        var contentIdList = idsDictionary[channelId];
                        DataProvider.ContentDao.UpdateTrashContents(SiteId, channelId, ChannelManager.GetTableName(SiteInfo, channelId), contentIdList);
                    }
                    AuthRequest.AddSiteLog(SiteId, "从回收站还原内容");
                    SuccessMessage("成功还原内容!");
                }
                else if (AuthRequest.IsQueryExists("IsRestoreAll"))
                {
                    DataProvider.ContentDao.UpdateRestoreContentsByTrash(SiteId, _channelId, tableName);
                    AuthRequest.AddSiteLog(SiteId, "从回收站还原所有内容");
                    SuccessMessage("成功还原所有内容!");
                }
            }

            ChannelManager.AddListItems(DdlChannelId.Items, SiteInfo, true, true, AuthRequest.AdminPermissionsImpl);

            if (_isCheckOnly)
            {
                CheckManager.LoadContentLevelToCheck(DdlState, SiteInfo, isChecked, checkedLevel);
            }
            else
            {
                CheckManager.LoadContentLevelToList(DdlState, SiteInfo, _isCheckOnly, isChecked, checkedLevel);
            }
            
            ControlUtils.SelectSingleItem(DdlState, state.ToString());

            foreach (var styleInfo in _allStyleInfoList)
            {
                if (styleInfo.InputType == InputType.TextEditor) continue;

                var listitem = new ListItem(styleInfo.DisplayName, styleInfo.AttributeName);
                DdlSearchType.Items.Add(listitem);
            }

            //ETriStateUtils.AddListItems(DdlState, "全部", "已审核", "待审核");

            if (SiteId != _channelId)
            {
                ControlUtils.SelectSingleItem(DdlChannelId, _channelId.ToString());
            }
            //ControlUtils.SelectSingleItem(DdlState, AuthRequest.GetQueryString("State"));
            ControlUtils.SelectSingleItem(DdlSearchType, searchType);
            TbKeyword.Text = keyword;
            TbDateFrom.Text = dateFrom;
            TbDateTo.Text = dateTo;

            PgContents.DataBind();

            LtlColumnsHead.Text += TextUtility.GetColumnsHeadHtml(_styleInfoList, _pluginColumns, _attributesOfDisplay);
            

            BtnSelect.Attributes.Add("onclick", ModalSelectColumns.GetOpenWindowString(SiteId, _channelId));

            if (_isTrashOnly)
            {
                LtlColumnsHead.Text += @"<th class=""text-center text-nowrap"" width=""150"">删除时间</th>";
                BtnAddToGroup.Visible = BtnTranslate.Visible = BtnCheck.Visible = false;
                PhTrash.Visible = true;
                if (!HasChannelPermissions(_channelId, ConfigManager.ChannelPermissions.ContentDelete))
                {
                    BtnDelete.Visible = false;
                    BtnDeleteAll.Visible = false;
                }
                else
                {
                    BtnDelete.Attributes.Add("onclick", PageContentDelete.GetRedirectClickStringForMultiChannels(SiteId, true, PageUrl));
                    BtnDeleteAll.Attributes.Add("onclick", PageUtils.GetRedirectStringWithConfirm(PageUtils.AddQueryString(PageUrl, "IsDeleteAll", "True"), "确实要清空回收站吗?"));
                }
                BtnRestore.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValue(PageUtils.AddQueryString(PageUrl, "IsRestore", "True"), "IDsCollection", "IDsCollection", "请选择需要还原的内容！"));
                BtnRestoreAll.Attributes.Add("onclick", PageUtils.GetRedirectStringWithConfirm(PageUtils.AddQueryString(PageUrl, "IsRestoreAll", "True"), "确实要还原所有内容吗?"));
            }
            else
            {
                LtlColumnsHead.Text += @"<th class=""text-center text-nowrap"" width=""100"">操作</th>";

                BtnAddToGroup.Attributes.Add("onclick", ModalAddToGroup.GetOpenWindowStringToContentForMultiChannels(SiteId));

                if (HasChannelPermissions(SiteId, ConfigManager.ChannelPermissions.ContentCheck))
                {
                    BtnCheck.Attributes.Add("onclick", ModalContentCheck.GetOpenWindowStringForMultiChannels(SiteId, PageUrl));
                    if (_isCheckOnly)
                    {
                        BtnCheck.CssClass = "btn m-r-5 btn-success";
                    }
                }
                else
                {
                    PhCheck.Visible = false;
                }

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
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var contentInfo = new ContentInfo((IDataRecord)e.Item.DataItem);

            var ltlTitle = (Literal)e.Item.FindControl("ltlTitle");
            var ltlChannel = (Literal)e.Item.FindControl("ltlChannel");
            var ltlColumns = (Literal)e.Item.FindControl("ltlColumns");
            var ltlStatus = (Literal)e.Item.FindControl("ltlStatus");
            var ltlSelect = (Literal)e.Item.FindControl("ltlSelect");

            ltlTitle.Text = WebUtils.GetContentTitle(SiteInfo, contentInfo, PageUrl);

            var specialHtml = string.Empty;
            
            if (_isTrashOnly)
            {
                specialHtml = DateUtils.GetDateAndTimeString(contentInfo.LastEditDate);
            }
            else
            {
                var pluginMenus = PluginMenuManager.GetContentMenus(_pluginIds, contentInfo);
                specialHtml = TextUtility.GetCommandsHtml(SiteInfo, pluginMenus, contentInfo, PageUrl,
                        AuthRequest.AdminName, _isEdit);
            }

            ltlColumns.Text = $@"
{TextUtility.GetColumnsHtml(_nameValueCacheDict, SiteInfo, contentInfo, _attributesOfDisplay, _allStyleInfoList, _pluginColumns)}
<td class=""text-center text-nowrap"">
{specialHtml}
</td>";

            string nodeName;
            if (!_nameValueCacheDict.TryGetValue(contentInfo.ChannelId.ToString(), out nodeName))
            {
                nodeName = ChannelManager.GetChannelNameNavigation(SiteId, contentInfo.ChannelId);
                _nameValueCacheDict[contentInfo.ChannelId.ToString()] = nodeName;
            }

            ltlChannel.Text = nodeName;
            var checkState = CheckManager.GetCheckState(SiteInfo, contentInfo);

            ltlStatus.Text = _isTrashOnly
                ? checkState
                : $@"<a href=""javascript:;"" title=""设置内容状态"" onclick=""{
                        ModalCheckState.GetOpenWindowString(SiteId, contentInfo, PageUrl)
                    }"">{checkState}</a>";

            ltlSelect.Text = $@"<input type=""checkbox"" name=""IDsCollection"" value=""{contentInfo.ChannelId}_{contentInfo.Id}"" />";
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
                        {"channelId", DdlChannelId.SelectedValue},
                        {"state", DdlState.SelectedValue},
                        {"searchType", DdlSearchType.SelectedValue},
                        {"keyword", TbKeyword.Text},
                        {"dateFrom", TbDateFrom.Text},
                        {"dateTo", TbDateTo.Text},
                        {"isCheckOnly", _isCheckOnly.ToString()},
                        {"isTrashOnly", _isTrashOnly.ToString()},
                        {"isWritingOnly", _isWritingOnly.ToString()},
                        {"isAdminOnly", _isAdminOnly.ToString()}
                    });
                }
                return _pageUrl;
            }
        }
    }
}
