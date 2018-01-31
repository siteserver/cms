using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalContentMultipleSelect : BasePageCms
    {
        public DropDownList DdlChannelId;
        public CheckBox CbIsDuplicate;
        public DropDownList DdlSearchType;
        public TextBox TbKeyword;
        public DateTimeTextBox TbDateFrom;
        public DateTimeTextBox TbDateTo;

        public Repeater RptContents;
        public SqlPager SpContents;

        private ChannelInfo _channelInfo;
        private string _tableName;
        private List<int> _relatedIdentities;
        private List<TableStyleInfo> _tableStyleInfoList;
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

            var permissions = PermissionsManager.GetPermissions(Body.AdminName);

            _jsMethod = Body.GetQueryString("jsMethod");

            PageUtils.CheckRequestParameter("siteId");
            var channelId = Body.GetQueryInt("channelId");
            if (channelId == 0)
            {
                channelId = SiteId;
            }
            _channelInfo = ChannelManager.GetChannelInfo(SiteId, channelId);
            _tableName = ChannelManager.GetTableName(SiteInfo, _channelInfo);
            _relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(SiteId, _channelInfo.Id);
            _tableStyleInfoList = TableStyleManager.GetTableStyleInfoList(_tableName, _relatedIdentities);

            SpContents.ControlToPaginate = RptContents;
            SpContents.SelectCommand = string.IsNullOrEmpty(Body.GetQueryString("channelId"))
                ? DataProvider.ContentDao.GetSqlString(_tableName, SiteId,
                    _channelInfo.Id, permissions.IsSystemAdministrator,
                    ProductPermissionsManager.Current.OwningChannelIdList, DdlSearchType.SelectedValue, TbKeyword.Text,
                    TbDateFrom.Text, TbDateTo.Text, true, ETriState.True, !CbIsDuplicate.Checked, false)
                : DataProvider.ContentDao.GetSqlString(_tableName, SiteId,
                    _channelInfo.Id, permissions.IsSystemAdministrator,
                    ProductPermissionsManager.Current.OwningChannelIdList, Body.GetQueryString("SearchType"),
                    Body.GetQueryString("Keyword"), Body.GetQueryString("DateFrom"), Body.GetQueryString("DateTo"), true,
                    ETriState.True, !Body.GetQueryBool("IsDuplicate"), true);
            SpContents.ItemsPerPage = SiteInfo.Additional.PageSize;
            SpContents.SortField = ContentAttribute.Id;
            SpContents.SortMode = SortMode.DESC;
            SpContents.OrderByString = ETaxisTypeUtils.GetContentOrderByString(ETaxisType.OrderByIdDesc);
            RptContents.ItemDataBound += RptContents_ItemDataBound;

            if (IsPostBack) return;

            ChannelManager.AddListItems(DdlChannelId.Items, SiteInfo, false, true, Body.AdminName);

            if (_tableStyleInfoList != null)
            {
                foreach (var styleInfo in _tableStyleInfoList)
                {
                    var listitem = new ListItem(styleInfo.DisplayName, styleInfo.AttributeName);
                    DdlSearchType.Items.Add(listitem);
                }
            }

            //添加隐藏属性
            DdlSearchType.Items.Add(new ListItem("内容ID", ContentAttribute.Id));
            DdlSearchType.Items.Add(new ListItem("添加者", ContentAttribute.AddUserName));
            DdlSearchType.Items.Add(new ListItem("最后修改者", ContentAttribute.LastEditUserName));

            if (Body.IsQueryExists("channelId"))
            {
                if (SiteId != _channelInfo.Id)
                {
                    ControlUtils.SelectSingleItem(DdlChannelId, _channelInfo.Id.ToString());
                }
                CbIsDuplicate.Checked = Body.GetQueryBool("IsDuplicate");
                ControlUtils.SelectSingleItem(DdlSearchType, Body.GetQueryString("SearchType"));
                TbKeyword.Text = Body.GetQueryString("Keyword");
                TbDateFrom.Text = Body.GetQueryString("DateFrom");
                TbDateTo.Text = Body.GetQueryString("DateTo");
            }

            SpContents.DataBind();
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var ltlChannel = (Literal)e.Item.FindControl("ltlChannel");
                var ltlTitle = (Literal)e.Item.FindControl("ltlTitle");
                var ltlSelect = (Literal)e.Item.FindControl("ltlSelect");

                var contentInfo = new ContentInfo(e.Item.DataItem);

                var nodeName = _valueHashtable[contentInfo.ChannelId] as string;
                if (nodeName == null)
                {
                    nodeName = ChannelManager.GetChannelNameNavigation(SiteId, contentInfo.ChannelId);
                    _valueHashtable[contentInfo.ChannelId] = nodeName;
                }
                ltlChannel.Text = nodeName;

                ltlTitle.Text = WebUtils.GetContentTitle(SiteInfo, contentInfo, PageUrl);

                ltlSelect.Text =
                    $@"<input type=""checkbox"" name=""IDsCollection"" value=""{contentInfo.ChannelId}_{contentInfo.Id}"" />";
            }
        }

        public void AddContent_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(WebUtils.GetContentAddAddUrl(SiteId, _channelInfo, PageUrl));
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
                foreach (var pair in TranslateUtils.StringCollectionToStringList(Request.Form["IDsCollection"]))
                {
                    var channelId = TranslateUtils.ToInt(pair.Split('_')[0]);
                    var contentId = TranslateUtils.ToInt(pair.Split('_')[1]);

                    var tableName = ChannelManager.GetTableName(SiteInfo, channelId);
                    var title = DataProvider.ContentDao.GetValue(tableName, contentId, ContentAttribute.Title);
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
                        {"IsDuplicate", CbIsDuplicate.Checked.ToString()},
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
