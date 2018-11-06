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
    public class PageContent : BasePageCms
    {
        public Literal LtlButtonsHead;
        public Literal LtlButtonsFoot;

        public Repeater RptContents;
        public Pager PgContents;
        public Literal LtlColumnsHead;
        public DateTimeTextBox TbDateFrom;
        public DropDownList DdlSearchType;
        public TextBox TbKeyword;

        private ChannelInfo _channelInfo;
        private string _tableName;
        private List<TableStyleInfo> _styleInfoList;
        private StringCollection _attributesOfDisplay;
        private List<TableStyleInfo> _allStyleInfoList;
        private List<string> _pluginIds;
        private Dictionary<string, Dictionary<string, Func<IContentContext, string>>> _pluginColumns;
        private bool _isEdit;
        private readonly Dictionary<string, string> _nameValueCacheDict = new Dictionary<string, string>();

        protected override bool IsSinglePage => true;

        public static string GetRedirectUrl(int siteId, int channelId)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageContent), new NameValueCollection
            {
                {"channelId", channelId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId", "channelId");
            var channelId = AuthRequest.GetQueryInt("channelId");
            _channelInfo = ChannelManager.GetChannelInfo(SiteId, channelId);
            _tableName = ChannelManager.GetTableName(SiteInfo, _channelInfo);
            _styleInfoList = TableStyleManager.GetContentStyleInfoList(SiteInfo, _channelInfo);
            _attributesOfDisplay = TranslateUtils.StringCollectionToStringCollection(ChannelManager.GetContentAttributesOfDisplay(SiteId, channelId));
            _allStyleInfoList = ContentUtility.GetAllTableStyleInfoList(_styleInfoList);

            _pluginIds = PluginContentManager.GetContentPluginIds(_channelInfo);
            _pluginColumns = PluginContentManager.GetContentColumns(_pluginIds);
            _isEdit = TextUtility.IsEdit(SiteInfo, channelId, AuthRequest.AdminPermissionsImpl);

            if (_channelInfo.Additional.IsPreviewContentsExists)
            {
                new Action(() =>
                {
                    DataProvider.ContentDao.DeletePreviewContents(SiteId, _tableName, _channelInfo);
                }).BeginInvoke(null, null);
            }

            if (!HasChannelPermissions(channelId, ConfigManager.ChannelPermissions.ContentView, ConfigManager.ChannelPermissions.ContentAdd, ConfigManager.ChannelPermissions.ContentEdit, ConfigManager.ChannelPermissions.ContentDelete, ConfigManager.ChannelPermissions.ContentTranslate))
            {
                if (!AuthRequest.IsAdminLoggin)
                {
                    PageUtils.RedirectToLoginPage();
                    return;
                }
                PageUtils.RedirectToErrorPage("您无此栏目的操作权限！");
                return;
            }

            RptContents.ItemDataBound += RptContents_ItemDataBound;

            var allAttributeNameList = TableColumnManager.GetTableColumnNameList(_tableName, DataType.Text);
            var pagerParam = new PagerParam
            {
                ControlToPaginate = RptContents,
                TableName = _tableName,
                PageSize = SiteInfo.Additional.PageSize,
                Page = AuthRequest.GetQueryInt(Pager.QueryNamePage, 1),
                OrderSqlString = DataProvider.ContentDao.GetOrderString(_channelInfo, string.Empty),
                ReturnColumnNames = TranslateUtils.ObjectCollectionToString(allAttributeNameList)
            };

            var administratorName = AuthRequest.AdminPermissionsImpl.IsViewContentOnlySelf(SiteId, channelId) ? AuthRequest.AdminName : string.Empty;

            if (AuthRequest.IsQueryExists("searchType"))
            {
                pagerParam.WhereSqlString = DataProvider.ContentDao.GetPagerWhereSqlString(SiteInfo, _channelInfo, AuthRequest.GetQueryString("searchType"), AuthRequest.GetQueryString("keyword"),
                    AuthRequest.GetQueryString("dateFrom"), string.Empty, CheckManager.LevelInt.All, false, true, false, false, false, AuthRequest.AdminPermissionsImpl, allAttributeNameList);
                pagerParam.TotalCount =
                    DataProvider.DatabaseDao.GetPageTotalCount(_tableName, pagerParam.WhereSqlString);
            }
            else
            {
                pagerParam.WhereSqlString = DataProvider.ContentDao.GetPagerWhereSqlString(channelId, ETriState.All, administratorName);
                var count = ContentManager.GetCount(SiteInfo, _channelInfo);
                pagerParam.TotalCount = count;
            }

            PgContents.Param = pagerParam;

            if (IsPostBack) return;

            PgContents.DataBind();

            var btnHtmls = WebUtils.GetContentCommands(AuthRequest.AdminPermissionsImpl, SiteInfo, _channelInfo, PageUrl);
            var btnDropDownsHtml =
                WebUtils.GetContentMoreCommands(AuthRequest.AdminPermissionsImpl, SiteInfo, _channelInfo, PageUrl);
            LtlButtonsHead.Text = GetButtonsHtml(true, btnHtmls, btnDropDownsHtml);
            if (pagerParam.TotalCount > 10)
            {
                LtlButtonsFoot.Text = GetButtonsHtml(false, btnHtmls, btnDropDownsHtml);
            }

            foreach (var styleInfo in _allStyleInfoList)
            {
                if (styleInfo.InputType == InputType.TextEditor) continue;

                var listitem = new ListItem(styleInfo.DisplayName, styleInfo.AttributeName);
                DdlSearchType.Items.Add(listitem);
            }

            if (AuthRequest.IsQueryExists("searchType"))
            {
                TbDateFrom.Text = AuthRequest.GetQueryString("dateFrom");
                ControlUtils.SelectSingleItem(DdlSearchType, AuthRequest.GetQueryString("searchType"));
                TbKeyword.Text = AuthRequest.GetQueryString("keyword");
                if (!string.IsNullOrEmpty(AuthRequest.GetQueryString("searchType")) || !string.IsNullOrEmpty(TbDateFrom.Text) ||
                    !string.IsNullOrEmpty(TbKeyword.Text))
                {
                    LtlButtonsHead.Text += @"
<script>
$(document).ready(function() {
	$('#contentSearch').show();
});
</script>
";
                }

            }
            else
            {
                ControlUtils.SelectSingleItem(DdlSearchType, ContentAttribute.Title);
            }

            LtlColumnsHead.Text = TextUtility.GetColumnsHeadHtml(_styleInfoList, _pluginColumns, _attributesOfDisplay);
        }

        private static string GetButtonsHtml(bool isHead, string btnsHtml, string btnDropDownsHtml)
        {
            return $@"<div class=""btn-toolbar"" role=""toolbar"">
  <div class=""btn-group"">
      {btnsHtml}
  </div>

  <div class=""btn-group ml-1"">
    <button id=""{(isHead ? "btnHeadMore" : "btnFootMore")}"" type=""button"" class=""btn btn-light text-secondary dropdown-toggle {(isHead ? "" : "dropup")}"">更多<span class=""caret""></span></button>
    <div id=""{(isHead ? "dropdownHeadMore" : "dropdownFootMore")}"" class=""dropdown-menu"">
      {btnDropDownsHtml}
    </div>
  </div>
</div>";
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var contentInfo = new ContentInfo((IDataRecord)e.Item.DataItem);

            var ltlTitle = (Literal)e.Item.FindControl("ltlTitle");
            var ltlColumns = (Literal)e.Item.FindControl("ltlColumns");
            var ltlStatus = (Literal)e.Item.FindControl("ltlStatus");
            var ltlCommands = (Literal)e.Item.FindControl("ltlCommands");
            var ltlSelect = (Literal)e.Item.FindControl("ltlSelect");

            ltlTitle.Text = WebUtils.GetContentTitle(SiteInfo, contentInfo, PageUrl);

            ltlColumns.Text = TextUtility.GetColumnsHtml(_nameValueCacheDict, SiteInfo, contentInfo, _attributesOfDisplay, _allStyleInfoList, _pluginColumns);

            ltlStatus.Text =
                $@"<a href=""javascript:;"" title=""设置内容状态"" onclick=""{ModalCheckState.GetOpenWindowString(SiteId, contentInfo, PageUrl)}"">{CheckManager.GetCheckState(SiteInfo, contentInfo)}</a>";

            var pluginMenus = PluginMenuManager.GetContentMenus(_pluginIds, contentInfo);
            ltlCommands.Text = TextUtility.GetCommandsHtml(SiteInfo, pluginMenus, contentInfo, PageUrl, AuthRequest.AdminName, _isEdit);

            ltlSelect.Text = $@"<input type=""checkbox"" name=""contentIdCollection"" value=""{contentInfo.Id}"" />";
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
                    _pageUrl = PageUtils.GetCmsUrl(SiteId, nameof(PageContent), new NameValueCollection
                    {
                        {"channelId", _channelInfo.Id.ToString()},
                        {"dateFrom", TbDateFrom.Text},
                        {"searchType", DdlSearchType.SelectedValue},
                        {"keyword", TbKeyword.Text},
                        {"page", AuthRequest.GetQueryInt("page", 1).ToString()}
                    });
                }
                return _pageUrl;
            }
        }
    }
}
