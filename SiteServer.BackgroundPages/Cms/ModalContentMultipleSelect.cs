using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using SiteServer.Abstractions;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Context;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Repositories;
using Content = SiteServer.Abstractions.Content;
using TableStyle = SiteServer.Abstractions.TableStyle;
using WebUtils = SiteServer.BackgroundPages.Core.WebUtils;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalContentMultipleSelect : BasePageCms
    {
        public DropDownList DdlChannelId;
        public DropDownList DdlSearchType;
        public TextBox TbKeyword;
        public DateTimeTextBox TbDateFrom;
        public DateTimeTextBox TbDateTo;

        public Repeater RptContents;
        public SqlPager SpContents;

        private Channel _channel;
        private List<TableStyle> _tableStyleList;
        private string _jsMethod;
        private readonly Hashtable _valueHashtable = new Hashtable();

        public static string GetOpenWindowString(int siteId, string jsMethod)
        {
            return LayerUtils.GetOpenScript("选择内容", PageUtils.GetCmsUrl(siteId, nameof(ModalContentMultipleSelect), new NameValueCollection
            {
                {"jsMethod", jsMethod}
            }));
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _jsMethod = AuthRequest.GetQueryString("jsMethod");

            PageUtils.CheckRequestParameter("siteId");
            var channelId = AuthRequest.GetQueryInt("channelId");
            if (channelId == 0)
            {
                channelId = SiteId;
            }
            _channel = ChannelManager.GetChannelAsync(SiteId, channelId).GetAwaiter().GetResult();
            var tableName = ChannelManager.GetTableNameAsync(Site, _channel).GetAwaiter().GetResult();
            _tableStyleList = TableStyleManager.GetContentStyleListAsync(Site, _channel).GetAwaiter().GetResult();

            SpContents.ControlToPaginate = RptContents;
            SpContents.SelectCommand = string.IsNullOrEmpty(AuthRequest.GetQueryString("channelId"))
                ? DataProvider.ContentRepository.GetSqlStringAsync(tableName, SiteId,
                    _channel.Id, AuthRequest.AdminPermissionsImpl.IsSiteAdminAsync().GetAwaiter().GetResult(),
                    AuthRequest.AdminPermissionsImpl.GetChannelIdListAsync().GetAwaiter().GetResult(), DdlSearchType.SelectedValue, TbKeyword.Text,
                    TbDateFrom.Text, TbDateTo.Text, true, ETriState.True, false).GetAwaiter().GetResult()
                : DataProvider.ContentRepository.GetSqlStringAsync(tableName, SiteId,
                    _channel.Id, AuthRequest.AdminPermissionsImpl.IsSiteAdminAsync().GetAwaiter().GetResult(),
                    AuthRequest.AdminPermissionsImpl.GetChannelIdListAsync().GetAwaiter().GetResult(), AuthRequest.GetQueryString("SearchType"),
                    AuthRequest.GetQueryString("Keyword"), AuthRequest.GetQueryString("DateFrom"), AuthRequest.GetQueryString("DateTo"), true,
                    ETriState.True, true).GetAwaiter().GetResult();
            SpContents.ItemsPerPage = Site.PageSize;
            SpContents.SortField = ContentAttribute.Id;
            SpContents.SortMode = SortMode.DESC;
            SpContents.OrderByString = ETaxisTypeUtils.GetContentOrderByString(ETaxisType.OrderByIdDesc);
            RptContents.ItemDataBound += RptContents_ItemDataBound;

            if (IsPostBack) return;

            ChannelManager.AddListItemsAsync(DdlChannelId.Items, Site, false, true, AuthRequest.AdminPermissionsImpl).GetAwaiter().GetResult();

            if (_tableStyleList != null)
            {
                foreach (var style in _tableStyleList)
                {
                    var listitem = new ListItem(style.DisplayName, style.AttributeName);
                    DdlSearchType.Items.Add(listitem);
                }
            }

            //添加隐藏属性
            DdlSearchType.Items.Add(new ListItem("内容ID", ContentAttribute.Id));
            DdlSearchType.Items.Add(new ListItem("添加者", ContentAttribute.AddUserName));
            DdlSearchType.Items.Add(new ListItem("最后修改者", ContentAttribute.LastEditUserName));

            if (AuthRequest.IsQueryExists("channelId"))
            {
                if (SiteId != _channel.Id)
                {
                    ControlUtils.SelectSingleItem(DdlChannelId, _channel.Id.ToString());
                }
                ControlUtils.SelectSingleItem(DdlSearchType, AuthRequest.GetQueryString("SearchType"));
                TbKeyword.Text = AuthRequest.GetQueryString("Keyword");
                TbDateFrom.Text = AuthRequest.GetQueryString("DateFrom");
                TbDateTo.Text = AuthRequest.GetQueryString("DateTo");
            }

            SpContents.DataBind();
        }

        public Content GetContent(DataRow row)
        {
            if (row == null) return null;

            var content = new Content();

            var dict = row.Table.Columns
                .Cast<DataColumn>()
                .ToDictionary(c => c.ColumnName, c => row[c]);

            foreach (var key in dict.Keys)
            {
                content.Set(key, dict[key]);
            }

            return content;
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var ltlChannel = (Literal)e.Item.FindControl("ltlChannel");
                var ltlTitle = (Literal)e.Item.FindControl("ltlTitle");
                var ltlSelect = (Literal)e.Item.FindControl("ltlSelect");

                var rowView = (DataRowView) e.Item.DataItem;
                var contentInfo = GetContent(rowView.Row);

                var nodeName = _valueHashtable[contentInfo.ChannelId] as string;
                if (nodeName == null)
                {
                    nodeName = ChannelManager.GetChannelNameNavigationAsync(SiteId, contentInfo.ChannelId).GetAwaiter().GetResult();
                    _valueHashtable[contentInfo.ChannelId] = nodeName;
                }
                ltlChannel.Text = nodeName;

                ltlTitle.Text = WebUtils.GetContentTitle(Site, contentInfo, PageUrl);

                ltlSelect.Text =
                    $@"<input type=""checkbox"" name=""IDsCollection"" value=""{contentInfo.ChannelId}_{contentInfo.Id}"" />";
            }
        }

        public void AddContent_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(WebUtils.GetContentAddAddUrl(SiteId, _channel.Id, PageUrl));
        }

        public void Search_OnClick(object sender, EventArgs e)
        {
            Response.Redirect(PageUrl, true);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.Form["IDsCollection"]))
            {
                var builder = new StringBuilder();
                foreach (var pair in StringUtils.GetStringList(Request.Form["IDsCollection"]))
                {
                    var channelId = TranslateUtils.ToInt(pair.Split('_')[0]);
                    var contentId = TranslateUtils.ToInt(pair.Split('_')[1]);

                    var tableName = ChannelManager.GetTableNameAsync(Site, channelId).GetAwaiter().GetResult();
                    var title = DataProvider.ContentRepository.GetValueAsync(tableName, contentId, ContentAttribute.Title).GetAwaiter().GetResult();
                    builder.Append($@"parent.{_jsMethod}('{title}', '{pair}');");
                }
                LayerUtils.CloseWithoutRefresh(Page, builder.ToString());
            }
            else
            {
                LayerUtils.CloseWithoutRefresh(Page);
            }
        }

        private string _pageUrl;
        private string PageUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_pageUrl))
                {
                    _pageUrl = PageUtils.GetCmsUrl(SiteId, nameof(ModalContentMultipleSelect), new NameValueCollection
                    {
                        {"channelId", DdlChannelId.SelectedValue},
                        {"SearchType", DdlSearchType.SelectedValue},
                        {"Keyword", TbKeyword.Text},
                        {"DateFrom", TbDateFrom.Text},
                        {"DateTo", TbDateTo.Text}
                    });
                }
                return _pageUrl;
            }
        }
    }
}
