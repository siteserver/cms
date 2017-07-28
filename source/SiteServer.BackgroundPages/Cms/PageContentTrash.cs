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
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageContentTrash : BasePageCms
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
        private ETableStyle _tableStyle;
        private List<int> _relatedIdentities;
        private List<TableStyleInfo> _tableStyleInfoList;

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
            var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, this._nodeId);
            _tableStyle = NodeManager.GetTableStyle(PublishmentSystemInfo, nodeInfo);
            var tableName = NodeManager.GetTableName(PublishmentSystemInfo, nodeInfo);
            _relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(PublishmentSystemId, this._nodeId);
            _tableStyleInfoList = TableStyleManager.GetTableStyleInfoList(_tableStyle, tableName, _relatedIdentities);

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
                spContents.SelectCommand = DataProvider.ContentDao.GetSelectCommend(_tableStyle, tableName, PublishmentSystemId, _nodeId, permissions.IsSystemAdministrator, ProductPermissionsManager.Current.OwningNodeIdList, SearchType.SelectedValue, Keyword.Text, DateFrom.Text, DateTo.Text, true, ETriState.All, false, true);
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
                spContents.SelectCommand = DataProvider.ContentDao.GetSelectCommend(_tableStyle, tableName, PublishmentSystemId, _nodeId, permissions.IsSystemAdministrator, ProductPermissionsManager.Current.OwningNodeIdList, Body.GetQueryString("SearchType"), Body.GetQueryString("Keyword"), Body.GetQueryString("DateFrom"), Body.GetQueryString("DateTo"), true, ETriState.All, false, true);
            }
            spContents.OrderByString = ETaxisTypeUtils.GetOrderByString(_tableStyle, ETaxisType.OrderByIdDesc);
            rptContents.ItemDataBound += rptContents_ItemDataBound;

			if(!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdContent, "内容回收站", AppManager.Cms.Permission.WebSite.ContentTrash);

                if (Body.IsQueryExists("IsDeleteAll"))
                {
                    BaiRongDataProvider.ContentDao.DeleteContentsByTrash(PublishmentSystemId, tableName);
                    Body.AddSiteLog(PublishmentSystemId, "清空回收站");
                    SuccessMessage("成功清空回收站!");
                    AddWaitAndRedirectScript(PageUrl);
                    return;
                }
                else if (Body.IsQueryExists("IsRestore"))
                {
                    var idsDictionary = ContentUtility.GetIDsDictionary(Request.QueryString);
                    foreach (var nodeID in idsDictionary.Keys)
                    {
                        var contentIDArrayList = idsDictionary[nodeID];
                        DataProvider.ContentDao.TrashContents(PublishmentSystemId, NodeManager.GetTableName(PublishmentSystemInfo, nodeID), contentIDArrayList);
                    }
                    Body.AddSiteLog(PublishmentSystemId, "从回收站还原内容");
                    SuccessMessage("成功还原内容!");
                    AddWaitAndRedirectScript(PageUrl);
                    return;
                }
                else if (Body.IsQueryExists("IsRestoreAll"))
                {
                    DataProvider.ContentDao.RestoreContentsByTrash(PublishmentSystemId, tableName);
                    Body.AddSiteLog(PublishmentSystemId, "从回收站还原所有内容");
                    SuccessMessage("成功还原所有内容!");
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

            if (!HasChannelPermissions(this._nodeId, AppManager.Cms.Permission.Channel.ContentDelete))
            {
                Delete.Visible = false;
                DeleteAll.Visible = false;
            }
            else
            {
                Delete.Attributes.Add("onclick", PageContentDelete.GetRedirectClickStringForMultiChannels(PublishmentSystemId, true, PageUrl));
                DeleteAll.Attributes.Add("onclick", PageUtils.GetRedirectStringWithConfirm(PageUtils.AddQueryString(PageUrl, "IsDeleteAll", "True"), "确实要清空回收站吗?"));
            }
            Restore.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValue(PageUtils.AddQueryString(PageUrl, "IsRestore", "True"), "IDsCollection", "IDsCollection", "请选择需要还原的内容！"));
            RestoreAll.Attributes.Add("onclick", PageUtils.GetRedirectStringWithConfirm(PageUtils.AddQueryString(PageUrl, "IsRestoreAll", "True"), "确实要还原所有内容吗?"));
		}

        private readonly Hashtable displayNameHashtable = new Hashtable();

        private readonly Hashtable nodeNameNavigations = new Hashtable();
        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var ltlItemTitle = (Literal)e.Item.FindControl("ltlItemTitle");
                var ltlChannel = (Literal)e.Item.FindControl("ltlChannel");
                var ltlDeleteDate = (Literal)e.Item.FindControl("ltlDeleteDate");
                var ltlItemEditUrl = (Literal)e.Item.FindControl("ltlItemEditUrl");
                var ltlSelect = (Literal)e.Item.FindControl("ltlSelect");

                var contentInfo = new BackgroundContentInfo(e.Item.DataItem);
                contentInfo.NodeId = -contentInfo.NodeId;

                ltlItemTitle.Text = WebUtils.GetContentTitle(PublishmentSystemInfo, contentInfo, PageUrl);
                string nodeNameNavigation;
                if (!nodeNameNavigations.ContainsKey(contentInfo.NodeId))
                {
                    nodeNameNavigation = NodeManager.GetNodeNameNavigation(PublishmentSystemId, contentInfo.NodeId);
                    nodeNameNavigations.Add(contentInfo.NodeId, nodeNameNavigation);
                }
                else
                {
                    nodeNameNavigation = nodeNameNavigations[contentInfo.NodeId] as string;
                }
                ltlChannel.Text = nodeNameNavigation;
                ltlDeleteDate.Text = DateUtils.GetDateAndTimeString(contentInfo.LastEditDate);
                ltlItemEditUrl.Text = GetEditUrl(contentInfo);

                ltlSelect.Text =
                    $@"<input type=""checkbox"" name=""IDsCollection"" value=""{contentInfo.NodeId}_{contentInfo.Id}"" />";
            }
        }

        private string GetEditUrl(BackgroundContentInfo contentInfo)
        {
            var url = string.Empty;
            if (HasChannelPermissions(contentInfo.NodeId, AppManager.Cms.Permission.Channel.ContentEdit) || Body.AdministratorName == contentInfo.AddUserName)
            {
                var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, contentInfo.NodeId);
                url =
                    $"<a href=\"{WebUtils.GetContentAddEditUrl(PublishmentSystemId, nodeInfo, contentInfo.Id, PageUrl)}\">修改</a>";
            }
            return url;
        }

		public void AddContent_OnClick(object sender, EventArgs e)
		{
            var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, _nodeId);
            PageUtils.Redirect(WebUtils.GetContentAddAddUrl(PublishmentSystemId, nodeInfo, PageUrl));
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
                    _pageUrl = PageUtils.GetCmsUrl(nameof(PageContentTrash), new NameValueCollection
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
