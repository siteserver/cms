using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
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
    public class ModalContentMultipleSelect : BasePageCms
    {
        public DropDownList NodeIDDropDownList;
        public CheckBox IsDuplicate;
        public DropDownList SearchType;
        public TextBox Keyword;
        public DateTimeTextBox DateFrom;
        public DateTimeTextBox DateTo;

        public Repeater rptContents;
        public SqlPager spContents;

        private NodeInfo _nodeInfo;
        private ETableStyle _tableStyle;
        private string _tableName;
        private List<int> _relatedIdentities;
        private List<TableStyleInfo> _tableStyleInfoList;
        private string _jsMethod;
        private readonly Hashtable _valueHashtable = new Hashtable();

        public static string GetOpenWindowString(int publishmentSystemId, string jsMethod)
        {
            return PageUtils.GetOpenWindowString("选择内容", PageUtils.GetCmsUrl(nameof(ModalContentMultipleSelect), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"jsMethod", jsMethod}
            }));
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            var permissions = PermissionsManager.GetPermissions(Body.AdministratorName);

            _jsMethod = Body.GetQueryString("jsMethod");

            PageUtils.CheckRequestParameter("PublishmentSystemID");
            var nodeId = Body.GetQueryInt("NodeID");
            if (nodeId == 0)
            {
                nodeId = PublishmentSystemId;
            }
            _nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeId);
            _tableStyle = NodeManager.GetTableStyle(PublishmentSystemInfo, _nodeInfo);
            _tableName = NodeManager.GetTableName(PublishmentSystemInfo, _nodeInfo);
            _relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(PublishmentSystemId, _nodeInfo.NodeId);
            _tableStyleInfoList = TableStyleManager.GetTableStyleInfoList(_tableStyle, _tableName, _relatedIdentities);

            spContents.ControlToPaginate = rptContents;
            if (string.IsNullOrEmpty(Body.GetQueryString("NodeID")))
            {
                spContents.SelectCommand = DataProvider.ContentDao.GetSelectCommend(_tableStyle, _tableName, PublishmentSystemId, _nodeInfo.NodeId, permissions.IsSystemAdministrator, ProductPermissionsManager.Current.OwningNodeIdList, SearchType.SelectedValue, Keyword.Text, DateFrom.Text, DateTo.Text, true, ETriState.True, !IsDuplicate.Checked, false);
            }
            else
            {
                spContents.SelectCommand = DataProvider.ContentDao.GetSelectCommend(_tableStyle, _tableName, PublishmentSystemId, _nodeInfo.NodeId, permissions.IsSystemAdministrator, ProductPermissionsManager.Current.OwningNodeIdList, Body.GetQueryString("SearchType"), Body.GetQueryString("Keyword"), Body.GetQueryString("DateFrom"), Body.GetQueryString("DateTo"), true, ETriState.True, !Body.GetQueryBool("IsDuplicate"), true);
            }
            spContents.ItemsPerPage = PublishmentSystemInfo.Additional.PageSize;
            spContents.SortField = ContentAttribute.Id;
            spContents.SortMode = SortMode.DESC;
            spContents.OrderByString = ETaxisTypeUtils.GetOrderByString(_tableStyle, ETaxisType.OrderByIdDesc);
            rptContents.ItemDataBound += rptContents_ItemDataBound;

            if (!IsPostBack)
            {
                NodeManager.AddListItems(NodeIDDropDownList.Items, PublishmentSystemInfo, false, true, true, EContentModelType.Content, Body.AdministratorName);

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
                    if (PublishmentSystemId != _nodeInfo.NodeId)
                    {
                        ControlUtils.SelectListItems(NodeIDDropDownList, _nodeInfo.NodeId.ToString());
                    }
                    IsDuplicate.Checked = Body.GetQueryBool("IsDuplicate");
                    ControlUtils.SelectListItems(SearchType, Body.GetQueryString("SearchType"));
                    Keyword.Text = Body.GetQueryString("Keyword");
                    DateFrom.Text = Body.GetQueryString("DateFrom");
                    DateTo.Text = Body.GetQueryString("DateTo");
                }

                spContents.DataBind();
            }
        }
        
        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var ltlChannel = (Literal)e.Item.FindControl("ltlChannel");
                var ltlTitle = (Literal)e.Item.FindControl("ltlTitle");
                var ltlSelect = (Literal)e.Item.FindControl("ltlSelect");

                var contentInfo = new ContentInfo(e.Item.DataItem);

                var nodeName = _valueHashtable[contentInfo.NodeId] as string;
                if (nodeName == null)
                {
                    nodeName = NodeManager.GetNodeNameNavigation(PublishmentSystemId, contentInfo.NodeId);
                    _valueHashtable[contentInfo.NodeId] = nodeName;
                }
                ltlChannel.Text = nodeName;

                ltlTitle.Text = WebUtils.GetContentTitle(PublishmentSystemInfo, contentInfo, PageUrl);

                ltlSelect.Text =
                    $@"<input type=""checkbox"" name=""IDsCollection"" value=""{contentInfo.NodeId}_{contentInfo.Id}"" />";
            }
        }

        public void AddContent_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(WebUtils.GetContentAddAddUrl(PublishmentSystemId, _nodeInfo, PageUrl));
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

                    var tableName = NodeManager.GetTableName(PublishmentSystemInfo, channelId);
                    var title = BaiRongDataProvider.ContentDao.GetValue(tableName, contentId, ContentAttribute.Title);
                    builder.Append($@"parent.{_jsMethod}('{title}', '{pair}');");
                }
                PageUtils.CloseModalPageWithoutRefresh(Page, builder.ToString());
            }
            else
            {
                PageUtils.CloseModalPageWithoutRefresh(Page);
            }
        }

        private string _pageUrl;
        private string PageUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_pageUrl))
                {
                    _pageUrl = PageUtils.GetCmsUrl(nameof(ModalContentMultipleSelect), new NameValueCollection
                    {
                        {"PublishmentSystemID", PublishmentSystemId.ToString()},
                        {"NodeID", NodeIDDropDownList.SelectedValue},
                        {"IsDuplicate", IsDuplicate.Checked.ToString()},
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
