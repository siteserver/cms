using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageContentTrash : BasePageCms
    {
        public DropDownList DdlChannelId;
        public DropDownList DdlPageNum;
        public DropDownList DdlSearchType;
        public TextBox TbKeyword;
        public DateTimeTextBox TbDateFrom;
        public DateTimeTextBox TbDateTo;

		public Repeater RptContents;
		public SqlPager SpContents;

        public Button BtnRestore;
        public Button BtnRestoreAll;
		public Button BtnDelete;
        public Button BtnDeleteAll;

		private int _channelId;
        private List<int> _relatedIdentities;
        private List<TableStyleInfo> _tableStyleInfoList;
        private readonly Hashtable _nodeNameNavigations = new Hashtable();

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId");
            _channelId = AuthRequest.GetQueryInt("channelId");
            if (_channelId == 0)
            {
                _channelId = SiteId;
            }
            var channelInfo = ChannelManager.GetChannelInfo(SiteId, _channelId);
            var tableName = ChannelManager.GetTableName(SiteInfo, channelInfo);
            _relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(SiteId, _channelId);
            _tableStyleInfoList = TableStyleManager.GetTableStyleInfoList(tableName, _relatedIdentities);

            SpContents.ControlToPaginate = RptContents;
            if (string.IsNullOrEmpty(AuthRequest.GetQueryString("channelId")))
            {
                SpContents.ItemsPerPage = TranslateUtils.ToInt(DdlPageNum.SelectedValue) == 0 ? SiteInfo.Additional.PageSize : TranslateUtils.ToInt(DdlPageNum.SelectedValue);
                SpContents.SelectCommand = DataProvider.ContentDao.GetSqlString(tableName, SiteId, _channelId, AuthRequest.AdminPermissions.IsSystemAdministrator, AuthRequest.AdminPermissions.OwningChannelIdList, DdlSearchType.SelectedValue, TbKeyword.Text, TbDateFrom.Text, TbDateTo.Text, true, ETriState.All, true);
            }
            else
            {
                SpContents.ItemsPerPage = AuthRequest.GetQueryInt("PageNum") == 0 ? SiteInfo.Additional.PageSize : AuthRequest.GetQueryInt("PageNum");
                SpContents.SelectCommand = DataProvider.ContentDao.GetSqlString(tableName, SiteId, _channelId, AuthRequest.AdminPermissions.IsSystemAdministrator, AuthRequest.AdminPermissions.OwningChannelIdList, AuthRequest.GetQueryString("SearchType"), AuthRequest.GetQueryString("Keyword"), AuthRequest.GetQueryString("DateFrom"), AuthRequest.GetQueryString("DateTo"), true, ETriState.All, true);
            }
            SpContents.OrderByString = ETaxisTypeUtils.GetContentOrderByString(ETaxisType.OrderByIdDesc);
            RptContents.ItemDataBound += RptContents_ItemDataBound;

			if(!IsPostBack)
            {
                VerifySitePermissions(ConfigManager.WebSitePermissions.Content);

                if (AuthRequest.IsQueryExists("IsDeleteAll"))
                {
                    DataProvider.ContentDao.DeleteContentsByTrash(SiteId, tableName);
                    AuthRequest.AddSiteLog(SiteId, "清空回收站");
                    SuccessMessage("成功清空回收站!");
                    AddWaitAndRedirectScript(PageUrl);
                    return;
                }
                if (AuthRequest.IsQueryExists("IsRestore"))
                {
                    var idsDictionary = ContentUtility.GetIDsDictionary(Request.QueryString);
                    foreach (var channelId in idsDictionary.Keys)
                    {
                        var contentIdArrayList = idsDictionary[channelId];
                        DataProvider.ContentDao.TrashContents(SiteId, ChannelManager.GetTableName(SiteInfo, channelId), contentIdArrayList);
                    }
                    AuthRequest.AddSiteLog(SiteId, "从回收站还原内容");
                    SuccessMessage("成功还原内容!");
                    AddWaitAndRedirectScript(PageUrl);
                    return;
                }
                if (AuthRequest.IsQueryExists("IsRestoreAll"))
                {
                    DataProvider.ContentDao.RestoreContentsByTrash(SiteId, tableName);
                    AuthRequest.AddSiteLog(SiteId, "从回收站还原所有内容");
                    SuccessMessage("成功还原所有内容!");
                    AddWaitAndRedirectScript(PageUrl);
                    return;
                }
                ChannelManager.AddListItems(DdlChannelId.Items, SiteInfo, true, false, AuthRequest.AdminPermissions);

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

                if (AuthRequest.IsQueryExists("channelId"))
                {
                    if (SiteId != _channelId)
                    {
                        ControlUtils.SelectSingleItem(DdlChannelId, _channelId.ToString());
                    }
                    ControlUtils.SelectSingleItem(DdlPageNum, AuthRequest.GetQueryString("PageNum"));
                    ControlUtils.SelectSingleItem(DdlSearchType, AuthRequest.GetQueryString("SearchType"));
                    TbKeyword.Text = AuthRequest.GetQueryString("Keyword");
                    TbDateFrom.Text = AuthRequest.GetQueryString("DateFrom");
                    TbDateTo.Text = AuthRequest.GetQueryString("DateTo");
                }

                SpContents.DataBind();
			}

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

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var ltlItemTitle = (Literal)e.Item.FindControl("ltlItemTitle");
            var ltlChannel = (Literal)e.Item.FindControl("ltlChannel");
            var ltlDeleteDate = (Literal)e.Item.FindControl("ltlDeleteDate");
            var ltlItemEditUrl = (Literal)e.Item.FindControl("ltlItemEditUrl");
            var ltlSelect = (Literal)e.Item.FindControl("ltlSelect");

            var contentInfo = new ContentInfo((IDataRecord)e.Item.DataItem);
            contentInfo.ChannelId = -contentInfo.ChannelId;

            ltlItemTitle.Text = WebUtils.GetContentTitle(SiteInfo, contentInfo, PageUrl);
            string nodeNameNavigation;
            if (!_nodeNameNavigations.ContainsKey(contentInfo.ChannelId))
            {
                nodeNameNavigation = ChannelManager.GetChannelNameNavigation(SiteId, contentInfo.ChannelId);
                _nodeNameNavigations.Add(contentInfo.ChannelId, nodeNameNavigation);
            }
            else
            {
                nodeNameNavigation = _nodeNameNavigations[contentInfo.ChannelId] as string;
            }
            ltlChannel.Text = nodeNameNavigation;
            ltlDeleteDate.Text = DateUtils.GetDateAndTimeString(contentInfo.LastEditDate);

            if (HasChannelPermissions(contentInfo.ChannelId, ConfigManager.ChannelPermissions.ContentEdit) || AuthRequest.AdminName == contentInfo.AddUserName)
            {
                var channelInfo = ChannelManager.GetChannelInfo(SiteId, contentInfo.ChannelId);
                ltlItemEditUrl.Text =
                    $"<a href=\"{WebUtils.GetContentAddEditUrl(SiteId, channelInfo, contentInfo.Id, PageUrl)}\">修改</a>";
            }

            ltlSelect.Text =
                $@"<input type=""checkbox"" name=""IDsCollection"" value=""{contentInfo.ChannelId}_{contentInfo.Id}"" />";
        }

		public void AddContent_OnClick(object sender, EventArgs e)
		{
            var channelInfo = ChannelManager.GetChannelInfo(SiteId, _channelId);
            PageUtils.Redirect(WebUtils.GetContentAddAddUrl(SiteId, channelInfo, PageUrl));
		}

        public void Search_OnClick(object sender, EventArgs e)
        {
            Response.Redirect(PageUrl, true);
        }

        private string _pageUrl;
        private string PageUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_pageUrl))
                {
                    _pageUrl = PageUtils.GetCmsUrl(SiteId, nameof(PageContentTrash), new NameValueCollection
                    {
                        {"channelId", DdlChannelId.SelectedValue},
                        {"PageNum", DdlPageNum.SelectedValue},
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
