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
    public class PageContents : BasePageCms
    {
        public Repeater RptContents;
        public Pager PgContents;
        public Literal LtlColumnsHead;
        public Literal LtlButtons;
        public Literal LtlMoreButtons;
        public DateTimeTextBox TbDateFrom;
        public DropDownList DdlSearchType;
        public TextBox TbKeyword;

        private ChannelInfo _channelInfo;
        private string _tableName;
        private List<int> _relatedIdentities;
        private List<TableStyleInfo> _styleInfoList;
        private StringCollection _attributesOfDisplay;
        private List<TableStyleInfo> _attributesOfDisplayStyleInfoList;
        private Dictionary<string, List<HyperLink>> _pluginLinks;
        private bool _isEdit;
        private readonly Dictionary<string, string> _nameValueCacheDict = new Dictionary<string, string>();

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
            _relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(SiteId, channelId);
            _channelInfo = ChannelManager.GetChannelInfo(SiteId, channelId);
            _tableName = ChannelManager.GetTableName(SiteInfo, _channelInfo);
            _styleInfoList = TableStyleManager.GetTableStyleInfoList(_tableName, _relatedIdentities);
            _attributesOfDisplay = TranslateUtils.StringCollectionToStringCollection(ChannelManager.GetContentAttributesOfDisplay(SiteId, channelId));
            _attributesOfDisplayStyleInfoList = ContentUtility.GetAllTableStyleInfoList(_styleInfoList);

            _pluginLinks = PluginContentManager.GetContentLinks(_channelInfo);
            _isEdit = TextUtility.IsEdit(SiteInfo, channelId, AuthRequest.AdminPermissions);

            if (_channelInfo.Additional.IsPreviewContents)
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

            string whereSqlString;
            var administratorName = AuthRequest.AdminPermissions.IsViewContentOnlySelf(SiteId, channelId) ? AuthRequest.AdminName : string.Empty;
            var allLowerAttributeNameList = TableMetadataManager.GetAllLowerAttributeNameList(_tableName);
            if (AuthRequest.IsQueryExists("searchType"))
            {
                var owningChannelIdList = new List<int>
                {
                    channelId
                };
                whereSqlString = DataProvider.ContentDao.GetPagerWhereSqlString(allLowerAttributeNameList, SiteId, channelId, AuthRequest.AdminPermissions.IsSystemAdministrator, owningChannelIdList, AuthRequest.GetQueryString("searchType"), AuthRequest.GetQueryString("keyword"), AuthRequest.GetQueryString("dateFrom"), string.Empty, false, ETriState.All, false, false, administratorName);
            }
            else
            {
                whereSqlString = DataProvider.ContentDao.GetPagerWhereSqlString(channelId, ETriState.All, administratorName);
            }

            var orderSqlString = DataProvider.ContentDao.GetPagerOrderSqlString(_channelInfo);
            var returnColumnNames = DataProvider.ContentDao.GetPagerReturnColumnNames(allLowerAttributeNameList, _attributesOfDisplay);

            PgContents.Param = new PagerParam
            {
                ControlToPaginate = RptContents,
                TableName = _tableName,
                ReturnColumnNames = returnColumnNames,
                WhereSqlString = whereSqlString,
                OrderSqlString = orderSqlString,
                PageSize = SiteInfo.Additional.PageSize,
                TotalCount = _channelInfo.ContentNum,
                Page = AuthRequest.GetQueryInt(Pager.QueryNamePage, 1)
            };

            RptContents.ItemDataBound += RptContents_ItemDataBound;

            if (IsPostBack) return;

            LtlButtons.Text = WebUtils.GetContentCommands(AuthRequest.AdminPermissions, SiteInfo, _channelInfo, PageUrl);
            LtlMoreButtons.Text = WebUtils.GetContentMoreCommands(AuthRequest.AdminPermissions, SiteInfo, _channelInfo, PageUrl);

            PgContents.DataBind();

            if (_styleInfoList != null)
            {
                foreach (var styleInfo in _styleInfoList)
                {
                    var listitem = new ListItem(styleInfo.DisplayName, styleInfo.AttributeName);
                    DdlSearchType.Items.Add(listitem);
                }
            }

            //添加隐藏属性
            DdlSearchType.Items.Add(new ListItem("内容ID", ContentAttribute.Id));
            DdlSearchType.Items.Add(new ListItem("添加者", ContentAttribute.AddUserName));
            DdlSearchType.Items.Add(new ListItem("最后修改者", ContentAttribute.LastEditUserName));
            DdlSearchType.Items.Add(new ListItem("内容组", ContentAttribute.GroupNameCollection));

            if (AuthRequest.IsQueryExists("searchType"))
            {
                TbDateFrom.Text = AuthRequest.GetQueryString("dateFrom");
                ControlUtils.SelectSingleItem(DdlSearchType, AuthRequest.GetQueryString("searchType"));
                TbKeyword.Text = AuthRequest.GetQueryString("keyword");
                if (!string.IsNullOrEmpty(AuthRequest.GetQueryString("searchType")) || !string.IsNullOrEmpty(TbDateFrom.Text) ||
                    !string.IsNullOrEmpty(TbKeyword.Text))
                {
                    LtlButtons.Text += @"
<script>
$(document).ready(function() {
	$('#contentSearch').show();
});
</script>
";
                }
                
            }

            LtlColumnsHead.Text = TextUtility.GetColumnsHeadHtml(_styleInfoList, _attributesOfDisplay, SiteInfo);
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var contentInfo = new ContentInfo(e.Item.DataItem);

            var ltlTitle = (Literal)e.Item.FindControl("ltlTitle");
            var ltlColumns = (Literal)e.Item.FindControl("ltlColumns");
            var ltlStatus = (Literal)e.Item.FindControl("ltlStatus");
            var ltlCommands = (Literal)e.Item.FindControl("ltlCommands");
            var ltlSelect = (Literal)e.Item.FindControl("ltlSelect");

            ltlTitle.Text = WebUtils.GetContentTitle(SiteInfo, contentInfo, PageUrl);

            ltlColumns.Text = TextUtility.GetColumnsHtml(_nameValueCacheDict, SiteInfo, contentInfo, _attributesOfDisplay, _attributesOfDisplayStyleInfoList);

            ltlStatus.Text =
                $@"<a href=""javascript:;"" title=""设置内容状态"" onclick=""{ModalCheckState.GetOpenWindowString(SiteId, contentInfo, PageUrl)}"">{CheckManager.GetCheckState(SiteInfo, contentInfo.IsChecked, contentInfo.CheckedLevel)}</a>";

            ltlCommands.Text = TextUtility.GetCommandsHtml(SiteInfo, _pluginLinks, contentInfo, PageUrl, AuthRequest.AdminName, _isEdit);

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
