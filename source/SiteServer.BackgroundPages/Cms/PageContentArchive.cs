using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Permissions;
using SiteServer.BackgroundPages.Controls;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model;
using SiteServer.BackgroundPages.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageContentArchive : BasePageCms
    {
        public DropDownList NodeIDDropDownList;
        public DropDownList PageNum;
        public DropDownList SearchType;
        public TextBox Keyword;
        public DateTimeTextBox DateFrom;
        public DateTimeTextBox DateTo;

		public Repeater rptContents;
		public SqlPager spContents;

        public Button Restore;
        public Button RestoreAll;
		public Button Delete;
        public Button DeleteAll;

		private int _nodeId;
        private List<int> _relatedIdentities;
        private List<TableStyleInfo> _tableStyleInfoList;
        private readonly Hashtable _nodeNameNavigations = new Hashtable();

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            var permissions = PermissionsManager.GetPermissions(Body.AdministratorName);

            PageUtils.CheckRequestParameter("PublishmentSystemID");
            _nodeId = Body.GetQueryInt("NodeID");
            if (_nodeId == 0)
            {
                _nodeId = PublishmentSystemId;
            }
            var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, _nodeId);
            var tableStyle = NodeManager.GetTableStyle(PublishmentSystemInfo, nodeInfo);
            var tableName = NodeManager.GetTableName(PublishmentSystemInfo, nodeInfo);
            ArchiveManager.CreateArchiveTableIfNotExists(PublishmentSystemInfo, tableName);
            var tableNameOfArchive = TableManager.GetTableNameOfArchive(tableName);

            _relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(PublishmentSystemId, _nodeId);
            _tableStyleInfoList = TableStyleManager.GetTableStyleInfoList(tableStyle, tableName, _relatedIdentities);

            spContents.ControlToPaginate = rptContents;
            if (string.IsNullOrEmpty(Body.GetQueryString("NodeID")))
            {
                if (TranslateUtils.ToInt(PageNum.SelectedValue) == 0)
                {
                    spContents.ItemsPerPage = PublishmentSystemInfo.Additional.PageSize;
                }
                else
                {
                    spContents.ItemsPerPage = TranslateUtils.ToInt(PageNum.SelectedValue);
                }
                spContents.SelectCommand = DataProvider.ContentDao.GetSelectCommend(tableStyle, tableNameOfArchive, PublishmentSystemId, _nodeId, permissions.IsSystemAdministrator, ProductPermissionsManager.Current.OwningNodeIdList, SearchType.SelectedValue, Keyword.Text, DateFrom.Text, DateTo.Text, true, ETriState.All);
            }
            else
            {
                if (Body.GetQueryInt("PageNum") == 0)
                {
                    spContents.ItemsPerPage = PublishmentSystemInfo.Additional.PageSize;
                }
                else
                {
                    spContents.ItemsPerPage = Body.GetQueryInt("PageNum");
                }
                spContents.SelectCommand = DataProvider.ContentDao.GetSelectCommend(tableStyle, tableNameOfArchive, PublishmentSystemId, _nodeId, permissions.IsSystemAdministrator, ProductPermissionsManager.Current.OwningNodeIdList, Body.GetQueryString("SearchType"), Body.GetQueryString("Keyword"), Body.GetQueryString("DateFrom"), Body.GetQueryString("DateTo"), true, ETriState.All);
            }
            spContents.SortField = ContentAttribute.LastEditDate;
            spContents.SortMode = SortMode.DESC;
            rptContents.ItemDataBound += rptContents_ItemDataBound;

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdFunction, "内容归档管理", AppManager.Cms.Permission.WebSite.Archive);

                if (Body.IsQueryExists("IsDelete"))
                {
                    var contentIdList = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("ContentIDCollection"));
                    BaiRongDataProvider.ContentDao.DeleteContentsArchive(PublishmentSystemId, tableNameOfArchive, contentIdList);
                    Body.AddSiteLog(PublishmentSystemId, "删除内容归档");
                    SuccessMessage("成功删除内容归档!");
                    AddWaitAndRedirectScript(PageUrl);
                    return;
                }
                else if (Body.IsQueryExists("IsDeleteAll"))
                {
                    var contentIdList = BaiRongDataProvider.ContentDao.GetContentIdListByPublishmentSystemId(tableNameOfArchive, PublishmentSystemId);
                    BaiRongDataProvider.ContentDao.DeleteContentsArchive(PublishmentSystemId, tableNameOfArchive, contentIdList);
                    SuccessMessage("成功清空内容归档!");
                    Body.AddSiteLog(PublishmentSystemId, "清空内容归档");
                    AddWaitAndRedirectScript(PageUrl);
                    return;
                }
                else if (Body.IsQueryExists("IsRestore"))
                {
                    var contentIdList = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("ContentIDCollection"));
                    foreach (var contentId in contentIdList)
                    {
                        var contentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableNameOfArchive, contentId);
                        DataProvider.ContentDao.Insert(tableName, PublishmentSystemInfo, contentInfo);
                    }
                    BaiRongDataProvider.ContentDao.DeleteContentsArchive(PublishmentSystemId, tableNameOfArchive, contentIdList);
                    SuccessMessage("成功取消归档内容!");
                    Body.AddSiteLog(PublishmentSystemId, "取消内容归档");
                    AddWaitAndRedirectScript(PageUrl);
                    return;
                }
                else if (Body.IsQueryExists("IsRestoreAll"))
                {
                    var contentIdList = BaiRongDataProvider.ContentDao.GetContentIdListByPublishmentSystemId(tableNameOfArchive, PublishmentSystemId);
                    foreach (var contentId in contentIdList)
                    {
                        var contentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableNameOfArchive, contentId);

                        DataProvider.ContentDao.Insert(tableName, PublishmentSystemInfo, contentInfo);
                    }
                    BaiRongDataProvider.ContentDao.DeleteContentsArchive(PublishmentSystemId, tableNameOfArchive, contentIdList);

                    SuccessMessage("成功取消归档所有内容!");
                    Body.AddSiteLog(PublishmentSystemId, "取消归档所有内容");
                    AddWaitAndRedirectScript(PageUrl);
                    return;
                }
                NodeManager.AddListItems(NodeIDDropDownList.Items, PublishmentSystemInfo, true, false, Body.AdministratorName);

                if (_tableStyleInfoList != null)
                {
                    foreach (var styleInfo in _tableStyleInfoList)
                    {
                        if (styleInfo.IsVisible)
                        {
                            var listitem = new ListItem(styleInfo.DisplayName, styleInfo.AttributeName);
                            SearchType.Items.Add(listitem);
                        }
                    }
                }
                //添加隐藏属性
                SearchType.Items.Add(new ListItem("内容ID", ContentAttribute.Id));
                SearchType.Items.Add(new ListItem("添加者", ContentAttribute.AddUserName));
                SearchType.Items.Add(new ListItem("最后修改者", ContentAttribute.LastEditUserName));

                if (Body.IsQueryExists("NodeID"))
                {
                    if (PublishmentSystemId != _nodeId)
                    {
                        ControlUtils.SelectListItems(NodeIDDropDownList, _nodeId.ToString());
                    }
                    ControlUtils.SelectListItems(PageNum, Body.GetQueryString("PageNum"));
                    ControlUtils.SelectListItems(SearchType, Body.GetQueryString("SearchType"));
                    Keyword.Text = Body.GetQueryString("Keyword");
                    DateFrom.Text = Body.GetQueryString("DateFrom");
                    DateTo.Text = Body.GetQueryString("DateTo");
                }

                spContents.DataBind();
            }

            if (!HasChannelPermissions(_nodeId, AppManager.Cms.Permission.Channel.ContentDelete))
            {
                Delete.Visible = false;
                DeleteAll.Visible = false;
            }
            else
            {
                Delete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(PageUtils.AddQueryString(PageUrl, "IsDelete", "True"), "ContentIDCollection", "ContentIDCollection", "请选择需要删除的内容！", "确实要删除所选内容吗?"));
                DeleteAll.Attributes.Add("onclick", PageUtils.GetRedirectStringWithConfirm(PageUtils.AddQueryString(PageUrl, "IsDeleteAll", "True"), "确实要清空内容归档吗?"));
            }
            Restore.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValue(PageUtils.AddQueryString(PageUrl, "IsRestore", "True"), "ContentIDCollection", "ContentIDCollection", "请选择需要取消归档的内容！"));
            RestoreAll.Attributes.Add("onclick", PageUtils.GetRedirectStringWithConfirm(PageUtils.AddQueryString(PageUrl, "IsRestoreAll", "True"), "确实要取消归档所有内容吗?"));
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var itemTitle = (Literal)e.Item.FindControl("ItemTitle");
                var itemChannelName = (Literal)e.Item.FindControl("ItemChannelName");
                var itemArchiveDate = (Literal)e.Item.FindControl("ItemArchiveDate");

                var contentInfo = new BackgroundContentInfo(e.Item.DataItem);

                itemTitle.Text = WebUtils.GetContentTitle(PublishmentSystemInfo, contentInfo, PageUrl);
                string nodeNameNavigation;
                if (!_nodeNameNavigations.ContainsKey(contentInfo.NodeId))
                {
                    nodeNameNavigation = NodeManager.GetNodeNameNavigation(PublishmentSystemId, contentInfo.NodeId);
                    _nodeNameNavigations.Add(contentInfo.NodeId, nodeNameNavigation);
                }
                else
                {
                    nodeNameNavigation = _nodeNameNavigations[contentInfo.NodeId] as string;
                }
                itemChannelName.Text = nodeNameNavigation;
                itemArchiveDate.Text = DateUtils.GetDateAndTimeString(contentInfo.LastEditDate);
            }
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
                    _pageUrl = PageUtils.GetCmsUrl(nameof(PageContentArchive), new NameValueCollection
                    {
                        {"PublishmentSystemID", PublishmentSystemId.ToString()},
                        {"NodeID", NodeIDDropDownList.SelectedValue},
                        {"PageNum", PageNum.SelectedValue},
                        {"SearchType", SearchType.SelectedValue},
                        {"Keyword", Keyword.Text},
                        {"DateFrom", DateFrom.Text},
                        {"DateTo", DateTo.Text}
                    });
                }
                return _pageUrl;
            }
        }
	}
}
