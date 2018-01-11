using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Table;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model;

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

		private int _nodeId;
        private List<int> _relatedIdentities;
        private List<TableStyleInfo> _tableStyleInfoList;
        private readonly Hashtable _nodeNameNavigations = new Hashtable();

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            var permissions = PermissionsManager.GetPermissions(Body.AdminName);

            PageUtils.CheckRequestParameter("PublishmentSystemID");
            _nodeId = Body.GetQueryInt("NodeID");
            if (_nodeId == 0)
            {
                _nodeId = PublishmentSystemId;
            }
            var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, _nodeId);
            var tableName = NodeManager.GetTableName(PublishmentSystemInfo, nodeInfo);
            _relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(PublishmentSystemId, _nodeId);
            _tableStyleInfoList = TableStyleManager.GetTableStyleInfoList(tableName, _relatedIdentities);

            SpContents.ControlToPaginate = RptContents;
            if (string.IsNullOrEmpty(Body.GetQueryString("NodeID")))
            {
                SpContents.ItemsPerPage = TranslateUtils.ToInt(DdlPageNum.SelectedValue) == 0 ? PublishmentSystemInfo.Additional.PageSize : TranslateUtils.ToInt(DdlPageNum.SelectedValue);
                SpContents.SelectCommand = DataProvider.ContentDao.GetSelectCommend(tableName, PublishmentSystemId, _nodeId, permissions.IsSystemAdministrator, ProductPermissionsManager.Current.OwningNodeIdList, DdlSearchType.SelectedValue, TbKeyword.Text, TbDateFrom.Text, TbDateTo.Text, true, ETriState.All, false, true);
            }
            else
            {
                SpContents.ItemsPerPage = Body.GetQueryInt("PageNum") == 0 ? PublishmentSystemInfo.Additional.PageSize : Body.GetQueryInt("PageNum");
                SpContents.SelectCommand = DataProvider.ContentDao.GetSelectCommend(tableName, PublishmentSystemId, _nodeId, permissions.IsSystemAdministrator, ProductPermissionsManager.Current.OwningNodeIdList, Body.GetQueryString("SearchType"), Body.GetQueryString("Keyword"), Body.GetQueryString("DateFrom"), Body.GetQueryString("DateTo"), true, ETriState.All, false, true);
            }
            SpContents.OrderByString = ETaxisTypeUtils.GetContentOrderByString(ETaxisType.OrderByIdDesc);
            RptContents.ItemDataBound += RptContents_ItemDataBound;

			if(!IsPostBack)
            {
                VerifySitePermissions(AppManager.Permissions.WebSite.ContentTrash);

                if (Body.IsQueryExists("IsDeleteAll"))
                {
                    DataProvider.ContentDao.DeleteContentsByTrash(PublishmentSystemId, tableName);
                    Body.AddSiteLog(PublishmentSystemId, "清空回收站");
                    SuccessMessage("成功清空回收站!");
                    AddWaitAndRedirectScript(PageUrl);
                    return;
                }
                if (Body.IsQueryExists("IsRestore"))
                {
                    var idsDictionary = ContentUtility.GetIDsDictionary(Request.QueryString);
                    foreach (var nodeId in idsDictionary.Keys)
                    {
                        var contentIdArrayList = idsDictionary[nodeId];
                        DataProvider.ContentDao.TrashContents(PublishmentSystemId, NodeManager.GetTableName(PublishmentSystemInfo, nodeId), contentIdArrayList);
                    }
                    Body.AddSiteLog(PublishmentSystemId, "从回收站还原内容");
                    SuccessMessage("成功还原内容!");
                    AddWaitAndRedirectScript(PageUrl);
                    return;
                }
                if (Body.IsQueryExists("IsRestoreAll"))
                {
                    DataProvider.ContentDao.RestoreContentsByTrash(PublishmentSystemId, tableName);
                    Body.AddSiteLog(PublishmentSystemId, "从回收站还原所有内容");
                    SuccessMessage("成功还原所有内容!");
                    AddWaitAndRedirectScript(PageUrl);
                    return;
                }
                NodeManager.AddListItems(DdlChannelId.Items, PublishmentSystemInfo, true, false, Body.AdminName);

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

                if (Body.IsQueryExists("NodeID"))
                {
                    if (PublishmentSystemId != _nodeId)
                    {
                        ControlUtils.SelectSingleItem(DdlChannelId, _nodeId.ToString());
                    }
                    ControlUtils.SelectSingleItem(DdlPageNum, Body.GetQueryString("PageNum"));
                    ControlUtils.SelectSingleItem(DdlSearchType, Body.GetQueryString("SearchType"));
                    TbKeyword.Text = Body.GetQueryString("Keyword");
                    TbDateFrom.Text = Body.GetQueryString("DateFrom");
                    TbDateTo.Text = Body.GetQueryString("DateTo");
                }

                SpContents.DataBind();
			}

            if (!HasChannelPermissions(_nodeId, AppManager.Permissions.Channel.ContentDelete))
            {
                BtnDelete.Visible = false;
                BtnDeleteAll.Visible = false;
            }
            else
            {
                BtnDelete.Attributes.Add("onclick", PageContentDelete.GetRedirectClickStringForMultiChannels(PublishmentSystemId, true, PageUrl));
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

            var contentInfo = new ContentInfo(e.Item.DataItem);
            contentInfo.NodeId = -contentInfo.NodeId;

            ltlItemTitle.Text = WebUtils.GetContentTitle(PublishmentSystemInfo, contentInfo, PageUrl);
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
            ltlChannel.Text = nodeNameNavigation;
            ltlDeleteDate.Text = DateUtils.GetDateAndTimeString(contentInfo.LastEditDate);

            if (HasChannelPermissions(contentInfo.NodeId, AppManager.Permissions.Channel.ContentEdit) || Body.AdminName == contentInfo.AddUserName)
            {
                var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, contentInfo.NodeId);
                ltlItemEditUrl.Text =
                    $"<a href=\"{WebUtils.GetContentAddEditUrl(PublishmentSystemId, nodeInfo, contentInfo.Id, PageUrl)}\">修改</a>";
            }

            ltlSelect.Text =
                $@"<input type=""checkbox"" name=""IDsCollection"" value=""{contentInfo.NodeId}_{contentInfo.Id}"" />";
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
                        {"NodeID", DdlChannelId.SelectedValue},
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
