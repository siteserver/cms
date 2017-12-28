using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
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
    public class ModalContentMultipleSelect : BasePageCms
    {
        public DropDownList DdlNodeId;
        public CheckBox CbIsDuplicate;
        public DropDownList DdlSearchType;
        public TextBox TbKeyword;
        public DateTimeTextBox TbDateFrom;
        public DateTimeTextBox TbDateTo;

        public Repeater RptContents;
        public SqlPager SpContents;

        private NodeInfo _nodeInfo;
        private string _tableName;
        private List<int> _relatedIdentities;
        private List<TableStyleInfo> _tableStyleInfoList;
        private string _jsMethod;
        private readonly Hashtable _valueHashtable = new Hashtable();

        public static string GetOpenWindowString(int publishmentSystemId, string jsMethod)
        {
            return LayerUtils.GetOpenScript("选择内容", PageUtils.GetCmsUrl(nameof(ModalContentMultipleSelect), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"jsMethod", jsMethod}
            }));
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            var permissions = PermissionsManager.GetPermissions(Body.AdminName);

            _jsMethod = Body.GetQueryString("jsMethod");

            PageUtils.CheckRequestParameter("PublishmentSystemID");
            var nodeId = Body.GetQueryInt("NodeID");
            if (nodeId == 0)
            {
                nodeId = PublishmentSystemId;
            }
            _nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeId);
            _tableName = NodeManager.GetTableName(PublishmentSystemInfo, _nodeInfo);
            _relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(PublishmentSystemId, _nodeInfo.NodeId);
            _tableStyleInfoList = TableStyleManager.GetTableStyleInfoList(_tableName, _relatedIdentities);

            SpContents.ControlToPaginate = RptContents;
            SpContents.SelectCommand = string.IsNullOrEmpty(Body.GetQueryString("NodeID"))
                ? DataProvider.ContentDao.GetSelectCommend(_tableName, PublishmentSystemId,
                    _nodeInfo.NodeId, permissions.IsSystemAdministrator,
                    ProductPermissionsManager.Current.OwningNodeIdList, DdlSearchType.SelectedValue, TbKeyword.Text,
                    TbDateFrom.Text, TbDateTo.Text, true, ETriState.True, !CbIsDuplicate.Checked, false)
                : DataProvider.ContentDao.GetSelectCommend(_tableName, PublishmentSystemId,
                    _nodeInfo.NodeId, permissions.IsSystemAdministrator,
                    ProductPermissionsManager.Current.OwningNodeIdList, Body.GetQueryString("SearchType"),
                    Body.GetQueryString("Keyword"), Body.GetQueryString("DateFrom"), Body.GetQueryString("DateTo"), true,
                    ETriState.True, !Body.GetQueryBool("IsDuplicate"), true);
            SpContents.ItemsPerPage = PublishmentSystemInfo.Additional.PageSize;
            SpContents.SortField = ContentAttribute.Id;
            SpContents.SortMode = SortMode.DESC;
            SpContents.OrderByString = ETaxisTypeUtils.GetContentOrderByString(ETaxisType.OrderByIdDesc);
            RptContents.ItemDataBound += RptContents_ItemDataBound;

            if (IsPostBack) return;

            NodeManager.AddListItems(DdlNodeId.Items, PublishmentSystemInfo, false, true, Body.AdminName);

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
                if (PublishmentSystemId != _nodeInfo.NodeId)
                {
                    ControlUtils.SelectSingleItem(DdlNodeId, _nodeInfo.NodeId.ToString());
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
                    _pageUrl = PageUtils.GetCmsUrl(nameof(ModalContentMultipleSelect), new NameValueCollection
                    {
                        {"PublishmentSystemID", PublishmentSystemId.ToString()},
                        {"NodeID", DdlNodeId.SelectedValue},
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
