using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Datory;
using Datory.Utils;
using SiteServer.Abstractions;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Repositories;
using Content = SiteServer.Abstractions.Content;
using TableStyle = SiteServer.Abstractions.TableStyle;
using WebUtils = SiteServer.BackgroundPages.Core.WebUtils;

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
        private Channel _channel;
        private List<TableStyle> _styleList;
        private List<string> _attributesOfDisplay;
        private List<TableStyle> _allStyleList;
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

        public async Task Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId");
            _channelId = AuthRequest.IsQueryExists("channelId") ? AuthRequest.GetQueryInt("channelId") : SiteId;

            _isCheckOnly = AuthRequest.GetQueryBool("isCheckOnly");
            _isTrashOnly = AuthRequest.GetQueryBool("isTrashOnly");
            _isWritingOnly = AuthRequest.GetQueryBool("isWritingOnly");
            _isAdminOnly = AuthRequest.GetQueryBool("isAdminOnly");

            _channel = await DataProvider.ChannelRepository.GetAsync(_channelId);
            var tableName = DataProvider.ChannelRepository.GetTableNameAsync(Site, _channel).GetAwaiter().GetResult();
            _styleList = DataProvider.TableStyleRepository.GetContentStyleListAsync(Site, _channel).GetAwaiter().GetResult();
            _attributesOfDisplay = Utilities.GetStringList(DataProvider.ChannelRepository.GetContentAttributesOfDisplayAsync(SiteId, _channelId).GetAwaiter().GetResult());
            _allStyleList = ColumnsManager.GetContentListStyles(_styleList);
            _pluginIds = PluginContentManager.GetContentPluginIds(_channel);
            _pluginColumns = PluginContentManager.GetContentColumnsAsync(_pluginIds).GetAwaiter().GetResult();
            _isEdit = TextUtility.IsEditAsync(Site, _channelId, AuthRequest.AdminPermissionsImpl).GetAwaiter().GetResult();

            var state = AuthRequest.IsQueryExists("state") ? AuthRequest.GetQueryInt("state") : CheckManager.LevelInt.All;
            var searchType = AuthRequest.IsQueryExists("searchType") ? AuthRequest.GetQueryString("searchType") : ContentAttribute.Title;
            var dateFrom = AuthRequest.IsQueryExists("dateFrom") ? AuthRequest.GetQueryString("dateFrom") : string.Empty;
            var dateTo = AuthRequest.IsQueryExists("dateTo") ? AuthRequest.GetQueryString("dateTo") : string.Empty;
            var keyword = AuthRequest.IsQueryExists("keyword") ? AuthRequest.GetQueryString("keyword") : string.Empty;

            var checkedLevel = 5;
            var isChecked = true;
            foreach (var owningChannelId in AuthRequest.AdminPermissionsImpl.GetChannelIdListAsync().GetAwaiter().GetResult())
            {
                var (isCheckedByChannelId, checkedLevelByChannelId) = CheckManager.GetUserCheckLevelAsync(AuthRequest.AdminPermissionsImpl, Site, owningChannelId).GetAwaiter().GetResult();
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

            var allAttributeNameList = await TableColumnManager.GetTableColumnNameListAsync(tableName, DataType.Text);

            var whereString = DataProvider.ContentRepository.GetPagerWhereSqlStringAsync(Site, _channel,
                searchType, keyword,
                dateFrom, dateTo, state, _isCheckOnly, false, _isTrashOnly, _isWritingOnly,
                AuthRequest.AdminPermissionsImpl.IsSiteAdminAsync().GetAwaiter().GetResult(), AuthRequest.AdminPermissionsImpl.GetChannelIdListAsync().GetAwaiter().GetResult(),
                allAttributeNameList).GetAwaiter().GetResult();

            PgContents.Param = new PagerParam
            {
                ControlToPaginate = RptContents,
                TableName = tableName,
                PageSize = Site.PageSize,
                Page = AuthRequest.GetQueryInt(Pager.QueryNamePage, 1),
                OrderSqlString = ETaxisTypeUtils.GetContentOrderByString(TaxisType.OrderByIdDesc),
                ReturnColumnNames = Utilities.ToString(allAttributeNameList),
                WhereSqlString = whereString,
                TotalCount = DataProvider.DatabaseRepository.GetPageTotalCount(tableName, whereString),
                
            };

            if (IsPostBack) return;

            if (_isTrashOnly)
            {
                if (AuthRequest.IsQueryExists("IsDeleteAll"))
                {
                    //DataProvider.ContentRepository.DeleteContentsByTrash(SiteId, _channelId, tableName);

                    var list = DataProvider.ContentRepository.GetContentIdListByTrashAsync(SiteId, tableName).GetAwaiter().GetResult();
                    foreach (var (contentChannelId, contentId) in list)
                    {
                        var channel = DataProvider.ChannelRepository.GetAsync(contentChannelId).GetAwaiter()
                            .GetResult();
                        DataProvider.ContentRepository.DeleteAsync(Site, channel, contentId).GetAwaiter().GetResult();
                    }

                    await AuthRequest.AddSiteLogAsync(SiteId, "清空回收站");
                    SuccessMessage("成功清空回收站!");
                }
                else if (AuthRequest.IsQueryExists("IsRestore"))
                {
                    var idsDictionary = ContentUtility.GetIDsDictionary(Request.QueryString);
                    foreach (var channelId in idsDictionary.Keys)
                    {
                        var contentIdList = idsDictionary[channelId];
                        var channel = DataProvider.ChannelRepository.GetAsync(channelId).GetAwaiter().GetResult();
                        DataProvider.ContentRepository.RecycleContentsAsync(Site, channel, contentIdList).GetAwaiter().GetResult();
                    }
                    await AuthRequest.AddSiteLogAsync(SiteId, "从回收站还原内容");
                    SuccessMessage("成功还原内容!");
                }
                else if (AuthRequest.IsQueryExists("IsRestoreAll"))
                {
                    DataProvider.ContentRepository.UpdateRestoreContentsByTrashAsync(SiteId, tableName).GetAwaiter().GetResult();
                    await AuthRequest.AddSiteLogAsync( SiteId, "从回收站还原所有内容");
                    SuccessMessage("成功还原所有内容!");
                }
            }

            DataProvider.ChannelRepository.AddListItemsAsync(DdlChannelId.Items, Site, true, true, AuthRequest.AdminPermissionsImpl).GetAwaiter().GetResult();

            if (_isCheckOnly)
            {
                CheckManager.LoadContentLevelToCheck(DdlState, Site, isChecked, checkedLevel);
            }
            else
            {
                CheckManager.LoadContentLevelToList(DdlState, Site, _isCheckOnly, isChecked, checkedLevel);
            }
            
            ControlUtils.SelectSingleItem(DdlState, state.ToString());

            foreach (var style in _allStyleList)
            {
                if (style.InputType == InputType.TextEditor) continue;

                var listitem = new ListItem(style.DisplayName, style.AttributeName);
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

            LtlColumnsHead.Text += TextUtility.GetColumnsHeadHtml(_styleList, _pluginColumns, _attributesOfDisplay);
            

            BtnSelect.Attributes.Add("onClick", ModalSelectColumns.GetOpenWindowString(SiteId, _channelId));

            if (_isTrashOnly)
            {
                LtlColumnsHead.Text += @"<th class=""text-center text-nowrap"" width=""150"">删除时间</th>";
                BtnAddToGroup.Visible = BtnTranslate.Visible = BtnCheck.Visible = false;
                PhTrash.Visible = true;
                if (!HasChannelPermissions(_channelId, Constants.ChannelPermissions.ContentDelete))
                {
                    BtnDelete.Visible = false;
                    BtnDeleteAll.Visible = false;
                }
                else
                {
                    BtnDelete.Attributes.Add("onClick", PageContentDelete.GetRedirectClickStringForMultiChannels(SiteId, true, PageUrl));
                    BtnDeleteAll.Attributes.Add("onClick", PageUtils.GetRedirectStringWithConfirm(PageUtils.AddQueryString(PageUrl, "IsDeleteAll", "True"), "确实要清空回收站吗?"));
                }
                BtnRestore.Attributes.Add("onClick", PageUtils.GetRedirectStringWithCheckBoxValue(PageUtils.AddQueryString(PageUrl, "IsRestore", "True"), "IDsCollection", "IDsCollection", "请选择需要还原的内容！"));
                BtnRestoreAll.Attributes.Add("onClick", PageUtils.GetRedirectStringWithConfirm(PageUtils.AddQueryString(PageUrl, "IsRestoreAll", "True"), "确实要还原所有内容吗?"));
            }
            else
            {
                LtlColumnsHead.Text += @"<th class=""text-center text-nowrap"" width=""100"">操作</th>";

                BtnAddToGroup.Attributes.Add("onClick", ModalAddToGroup.GetOpenWindowStringToContentForMultiChannels(SiteId));

                if (HasChannelPermissions(SiteId, Constants.ChannelPermissions.ContentCheckLevel1))
                {
                    BtnCheck.Attributes.Add("onClick", ModalContentCheck.GetOpenWindowStringForMultiChannels(SiteId, PageUrl));
                    if (_isCheckOnly)
                    {
                        BtnCheck.CssClass = "btn m-r-5 btn-success";
                    }
                }
                else
                {
                    PhCheck.Visible = false;
                }

                if (!HasChannelPermissions(_channelId, Constants.ChannelPermissions.ContentTranslate))
                {
                    BtnTranslate.Visible = false;
                }
                else
                {
                    BtnTranslate.Attributes.Add("onClick", PageContentTranslate.GetRedirectClickStringForMultiChannels(SiteId, PageUrl));
                }

                if (!HasChannelPermissions(_channelId, Constants.ChannelPermissions.ContentDelete))
                {
                    BtnDelete.Visible = false;
                }
                else
                {
                    BtnDelete.Attributes.Add("onClick", PageContentDelete.GetRedirectClickStringForMultiChannels(SiteId, false, PageUrl));
                }
            }
        }

        public Content GetContent(IDataRecord record)
        {
            if (record == null) return null;

            var content = new Content();

            for (var i = 0; i < record.FieldCount; i++)
            {
                var name = record.GetName(i);
                var value = record.GetValue(i);

                if (value is string && WebConfigUtils.DatabaseType == DatabaseType.Oracle && (string)value == SqlUtils.OracleEmptyValue)
                {
                    value = string.Empty;
                }
                content.Set(name, value);
            }

            return content;
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var contentInfo = GetContent((IDataRecord)e.Item.DataItem);

            var ltlTitle = (Literal)e.Item.FindControl("ltlTitle");
            var ltlChannel = (Literal)e.Item.FindControl("ltlChannel");
            var ltlColumns = (Literal)e.Item.FindControl("ltlColumns");
            var ltlStatus = (Literal)e.Item.FindControl("ltlStatus");
            var ltlSelect = (Literal)e.Item.FindControl("ltlSelect");

            ltlTitle.Text = WebUtils.GetContentTitle(Site, contentInfo, PageUrl);

            var specialHtml = string.Empty;
            
            if (_isTrashOnly)
            {
                specialHtml = DateUtils.GetDateAndTimeString(contentInfo.LastEditDate);
            }
            else
            {
                var pluginMenus = PluginMenuManager.GetContentMenusAsync(_pluginIds, contentInfo).GetAwaiter().GetResult();
                specialHtml = TextUtility.GetCommandsHtml(Site, pluginMenus, contentInfo, PageUrl,
                        AuthRequest.AdminName, _isEdit);
            }

            ltlColumns.Text = $@"
{TextUtility.GetColumnsHtmlAsync(_nameValueCacheDict, Site, contentInfo, _attributesOfDisplay, _allStyleList, _pluginColumns).GetAwaiter().GetResult()}
<td class=""text-center text-nowrap"">
{specialHtml}
</td>";

            string nodeName;
            if (!_nameValueCacheDict.TryGetValue(contentInfo.ChannelId.ToString(), out nodeName))
            {
                nodeName = DataProvider.ChannelRepository.GetChannelNameNavigationAsync(SiteId, contentInfo.ChannelId).GetAwaiter().GetResult();
                _nameValueCacheDict[contentInfo.ChannelId.ToString()] = nodeName;
            }

            ltlChannel.Text = nodeName;
            var checkState = CheckManager.GetCheckState(Site, contentInfo);

            ltlStatus.Text = _isTrashOnly
                ? checkState
                : $@"<a href=""javascript:;"" title=""设置内容状态"" onClick=""{
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
