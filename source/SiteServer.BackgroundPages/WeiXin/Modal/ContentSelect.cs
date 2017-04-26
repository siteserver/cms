using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Controls;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.BackgroundPages;
using SiteServer.WeiXin.Core;
using System.Text;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.WeiXin.Model;

namespace SiteServer.WeiXin.BackgroundPages.Modal
{
    public class ContentSelect : BackgroundBasePage
    {
        public DropDownList NodeIDDropDownList;
        public DropDownList State;
        public CheckBox IsDuplicate;
        public DropDownList SearchType;
        public TextBox Keyword;
        public DateTimeTextBox DateFrom;
        public DateTimeTextBox DateTo;

        public Repeater rptContents;
        public SqlPager spContents;

        int nodeID = 0;
        NodeInfo nodeInfo;
        private ETableStyle tableStyle;
        private StringCollection attributesOfDisplay;
        ArrayList relatedIdentities;
        ArrayList tableStyleInfoArrayList;

        private bool isMultiple;
        private string jsMethod;
        private int itemIndex;

        private bool isKeywordAdd;
        private int keywordID;

        public static string GetRedirectUrlByKeywordAddList(int publishmentSystemID, bool isMultiple, int keywordID)
        {
            return PageUtils.GetWXUrl(
                $"modal_contentSelect.aspx?publishmentSystemID={publishmentSystemID}&isMultiple={isMultiple}&isKeywordAdd=true&keywordID={keywordID}");
        }

        public static string GetOpenWindowString(int publishmentSystemID, bool isMultiple, string jsMethod)
        {
            var arguments = new NameValueCollection();
            arguments.Add("publishmentSystemID", publishmentSystemID.ToString());
            arguments.Add("isMultiple", isMultiple.ToString());
            arguments.Add("jsMethod", jsMethod);
            return PageUtilityWX.GetOpenWindowString("选择微官网内容", "modal_contentSelect.aspx", arguments);
        }

        public static string GetOpenWindowStringByItemIndex(int publishmentSystemID, string jsMethod, string itemIndex)
        {
            var arguments = new NameValueCollection();
            arguments.Add("publishmentSystemID", publishmentSystemID.ToString());
            arguments.Add("jsMethod", jsMethod);
            arguments.Add("itemIndex", itemIndex);
            return PageUtilityWX.GetOpenWindowString("选择微官网内容", "modal_contentSelect.aspx", arguments);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");
            isMultiple = TranslateUtils.ToBool(Request.QueryString["isMultiple"]);
            jsMethod = Request.QueryString["jsMethod"];
            itemIndex = TranslateUtils.ToInt(Request.QueryString["itemIndex"]);

            isKeywordAdd = TranslateUtils.ToBool(Request.QueryString["isKeywordAdd"]);
            keywordID = TranslateUtils.ToInt(Request.QueryString["keywordID"]);

            if (!string.IsNullOrEmpty(Request.QueryString["NodeID"]))
            {
                nodeID = int.Parse(Request.QueryString["NodeID"]);
            }
            else
            {
                nodeID = PublishmentSystemID;
            }
            nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemID, nodeID);
            tableStyle = NodeManager.GetTableStyle(PublishmentSystemInfo, nodeInfo);
            var tableName = NodeManager.GetTableName(PublishmentSystemInfo, nodeInfo);
            attributesOfDisplay = TranslateUtils.StringCollectionToStringCollection(NodeManager.GetContentAttributesOfDisplay(PublishmentSystemID, nodeID));
            relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(PublishmentSystemID, nodeID);
            tableStyleInfoArrayList = TableStyleManager.GetTableStyleInfoArrayList(tableStyle, tableName, relatedIdentities);

            spContents.ControlToPaginate = rptContents;
            spContents.ConnectionString = BaiRongDataProvider.ConnectionString;
            if (string.IsNullOrEmpty(Request.QueryString["NodeID"]))
            {
                var stateType = ETriStateUtils.GetEnumType(State.SelectedValue);
                spContents.SelectCommand = DataProvider.ContentDAO.GetSelectCommend(tableStyle, tableName, PublishmentSystemID, nodeID, PermissionsManager.Current.IsSystemAdministrator, ProductPermissionsManager.Current.OwningNodeIDList, SearchType.SelectedValue, Keyword.Text, DateFrom.Text, DateTo.Text, true, stateType, !IsDuplicate.Checked, false);
            }
            else
            {
                var stateType = ETriStateUtils.GetEnumType(Request.QueryString["State"]);
                spContents.SelectCommand = DataProvider.ContentDAO.GetSelectCommend(tableStyle, tableName, PublishmentSystemID, nodeID, PermissionsManager.Current.IsSystemAdministrator, ProductPermissionsManager.Current.OwningNodeIDList, Request.QueryString["SearchType"], Request.QueryString["Keyword"], Request.QueryString["DateFrom"], Request.QueryString["DateTo"], true, stateType, !TranslateUtils.ToBool(Request.QueryString["IsDuplicate"]), false);
            }
            spContents.ItemsPerPage = PublishmentSystemInfo.Additional.PageSize;
            spContents.SortField = ContentAttribute.Id;
            spContents.SortMode = SortMode.DESC;
            spContents.OrderByString = ETaxisTypeUtils.GetOrderByString(tableStyle, ETaxisType.OrderByIDDesc);
            rptContents.ItemDataBound += new RepeaterItemEventHandler(rptContents_ItemDataBound);

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.CMS.LeftMenu.ID_Content, "内容搜索", string.Empty);

                NodeManager.AddListItems(NodeIDDropDownList.Items, PublishmentSystemInfo, true, true);

                if (tableStyleInfoArrayList != null)
                {
                    foreach (TableStyleInfo styleInfo in tableStyleInfoArrayList)
                    {
                        if (styleInfo.IsVisible)
                        {
                            var listitem = new ListItem(styleInfo.DisplayName, styleInfo.AttributeName);
                            SearchType.Items.Add(listitem);
                        }
                    }
                }

                ETriStateUtils.AddListItems(State, "全部", "已审核", "待审核");

                //添加隐藏属性
                SearchType.Items.Add(new ListItem("内容ID", ContentAttribute.Id));
                SearchType.Items.Add(new ListItem("添加者", ContentAttribute.AddUserName));
                SearchType.Items.Add(new ListItem("最后修改者", ContentAttribute.LastEditUserName));

                if (!string.IsNullOrEmpty(Request.QueryString["NodeID"]))
                {
                    if (PublishmentSystemID != nodeID)
                    {
                        ControlUtils.SelectListItems(NodeIDDropDownList, nodeID.ToString());
                    }
                    ControlUtils.SelectListItems(State, Request.QueryString["State"]);
                    IsDuplicate.Checked = TranslateUtils.ToBool(Request.QueryString["IsDuplicate"]);
                    ControlUtils.SelectListItems(SearchType, Request.QueryString["SearchType"]);
                    Keyword.Text = Request.QueryString["Keyword"];
                    DateFrom.Text = Request.QueryString["DateFrom"];
                    DateTo.Text = Request.QueryString["DateTo"];
                }

                spContents.DataBind();
            }
        }

        private readonly Hashtable valueHashtable = new Hashtable();

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var ltlItemTitle = e.Item.FindControl("ltlItemTitle") as Literal;
                var ltlChannel = e.Item.FindControl("ltlChannel") as Literal;
                var ltlItemImageUrl = e.Item.FindControl("ltlItemImageUrl") as Literal;
                var ltlItemSummary = e.Item.FindControl("ltlItemSummary") as Literal;
                var ltlSelect = e.Item.FindControl("ltlSelect") as Literal;

                var contentInfo = new ContentInfo(e.Item.DataItem);

                ltlItemTitle.Text = WebUtils.GetContentTitle(PublishmentSystemInfo, contentInfo, PageUrl);
                var nodeName = valueHashtable[contentInfo.NodeId] as string;
                if (nodeName == null)
                {
                    nodeName = NodeManager.GetNodeNameNavigation(PublishmentSystemID, contentInfo.NodeId);
                    valueHashtable[contentInfo.NodeId] = nodeName;
                }
                ltlChannel.Text = nodeName;

                var imageUrl = contentInfo.GetExtendedAttribute(BackgroundContentAttribute.ImageUrl);
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    ltlItemImageUrl.Text =
                        $@"<img src=""{PageUtility.ParseNavigationUrl(PublishmentSystemInfo, imageUrl)}"" style=""max-width:78px;max-height:78px;"" />";
                }

                ltlItemSummary.Text = MPUtils.GetSummary(contentInfo.GetExtendedAttribute(BackgroundContentAttribute.Summary), contentInfo.GetExtendedAttribute(BackgroundContentAttribute.Content));

                if (isMultiple)
                {
                    ltlSelect.Text =
                        $@"<input type=""checkbox"" name=""IDsCollection"" value=""{contentInfo.NodeId}_{contentInfo.Id}"" />";
                }
                else
                {
                    ltlSelect.Text =
                        $@"<input type=""radio"" name=""IDsCollection"" value=""{contentInfo.NodeId}_{contentInfo.Id}"" />";
                }
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
                    if (Request.QueryString["itemIndex"] != null)
                    {
                        _pageUrl = PageUtils.GetWXUrl(
                                            $"modal_contentSelect.aspx?publishmentSystemID={PublishmentSystemID}&isMultiple={isMultiple}&jsMethod={jsMethod}&itemIndex={itemIndex}&isKeywordAdd={isKeywordAdd}&keywordID={keywordID}&") +
                                        $"NodeID={NodeIDDropDownList.SelectedValue}&State={State.SelectedValue}&IsDuplicate={IsDuplicate.Checked}&SearchType={SearchType.SelectedValue}&Keyword={Keyword.Text}&DateFrom={DateFrom.Text}&DateTo={DateTo.Text}";
                    }
                    else
                    {
                        _pageUrl = PageUtils.GetWXUrl(
                                            $"modal_contentSelect.aspx?publishmentSystemID={PublishmentSystemID}&isMultiple={isMultiple}&jsMethod={jsMethod}&isKeywordAdd={isKeywordAdd}&keywordID={keywordID}&") +
                                        $"NodeID={NodeIDDropDownList.SelectedValue}&State={State.SelectedValue}&IsDuplicate={IsDuplicate.Checked}&SearchType={SearchType.SelectedValue}&Keyword={Keyword.Text}&DateFrom={DateFrom.Text}&DateTo={DateTo.Text}";
                    }
                }
                return _pageUrl;
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            try
            {
                var idsCollection = Request.Form["IDsCollection"];

                if (string.IsNullOrEmpty(idsCollection))
                {
                    FailMessage("操作失败，请选择需要显示的内容");
                    return;
                }

                if (isKeywordAdd)
                {
                    if (keywordID > 0)
                    {
                        var idsList = TranslateUtils.StringCollectionToStringList(idsCollection);
                        var resourceID = 0;
                        foreach (var ids in idsList)
                        {
                            var nodeID = TranslateUtils.ToInt(ids.Split('_')[0]);
                            var contentID = TranslateUtils.ToInt(ids.Split('_')[1]);
                            var tableStyle = NodeManager.GetTableStyle(PublishmentSystemInfo, nodeID);
                            var tableName = NodeManager.GetTableName(PublishmentSystemInfo, nodeID);

                            var contentInfo = DataProvider.ContentDAO.GetContentInfo(tableStyle, tableName, contentID);

                            var resourceInfo = new KeywordResourceInfo();

                            resourceInfo.ResourceID = 0;
                            resourceInfo.PublishmentSystemID = PublishmentSystemID;
                            resourceInfo.KeywordID = keywordID;
                            resourceInfo.Title = contentInfo.Title;
                            resourceInfo.ImageUrl = contentInfo.GetExtendedAttribute(BackgroundContentAttribute.ImageUrl);
                            resourceInfo.Summary = MPUtils.GetSummary(contentInfo.GetExtendedAttribute(BackgroundContentAttribute.Summary), contentInfo.GetExtendedAttribute(BackgroundContentAttribute.Content));
                            resourceInfo.ResourceType = EResourceType.Site;
                            resourceInfo.IsShowCoverPic = false;
                            resourceInfo.Content = contentInfo.GetExtendedAttribute(BackgroundContentAttribute.Content);
                            resourceInfo.NavigationUrl = string.Empty;
                            resourceInfo.ChannelID = contentInfo.NodeId;
                            resourceInfo.ContentID = contentInfo.Id;
                            resourceInfo.Taxis = 0;

                            var id = DataProviderWX.KeywordResourceDAO.Insert(resourceInfo);
                            if (resourceID == 0)
                            {
                                resourceID = id;
                            }
                        }

                        var redirectUrl = BackgroundKeywordNewsAdd.GetRedirectUrl(PublishmentSystemID, keywordID, resourceID, !isMultiple);
                        JsUtils.OpenWindow.CloseModalPageAndRedirect(Page, redirectUrl);
                    }
                }
                else
                {
                    var scripts = string.Empty;
                    if (isMultiple)
                    {
                        var titleBuilder = new StringBuilder();
                        var idsList = TranslateUtils.StringCollectionToStringList(idsCollection);
                        foreach (var ids in idsList)
                        {
                            var nodeID = TranslateUtils.ToInt(ids.Split('_')[0]);
                            var contentID = TranslateUtils.ToInt(ids.Split('_')[1]);
                            var tableStyle = NodeManager.GetTableStyle(PublishmentSystemInfo, nodeID);
                            var tableName = NodeManager.GetTableName(PublishmentSystemInfo, nodeID);

                            var contentInfo = DataProvider.ContentDAO.GetContentInfo(tableStyle, tableName, contentID);

                            titleBuilder.AppendFormat("{0}&nbsp;<a href='{1}' target='blank'>查看</a><br />", contentInfo.Title, PageUtilityWX.GetContentUrl(PublishmentSystemInfo, contentInfo));
                        }
                        scripts = $@"window.parent.{jsMethod}(""{idsCollection}"", ""{titleBuilder.ToString()}"");";
                    }
                    else
                    {
                        var nodeID = TranslateUtils.ToInt(idsCollection.Split('_')[0]);
                        var contentID = TranslateUtils.ToInt(idsCollection.Split('_')[1]);
                        var tableStyle = NodeManager.GetTableStyle(PublishmentSystemInfo, nodeID);
                        var tableName = NodeManager.GetTableName(PublishmentSystemInfo, nodeID);

                        var contentInfo = DataProvider.ContentDAO.GetContentInfo(tableStyle, tableName, contentID);

                        var imageUrl = contentInfo.GetExtendedAttribute(BackgroundContentAttribute.ImageUrl);
                        var imageSrc = PageUtility.ParseNavigationUrl(PublishmentSystemInfo, imageUrl);
                        var summary = MPUtils.GetSummary(contentInfo.GetExtendedAttribute(BackgroundContentAttribute.Summary), contentInfo.GetExtendedAttribute(BackgroundContentAttribute.Content));

                        var pageUrl = PageUtilityWX.GetContentUrl(PublishmentSystemInfo, contentInfo);
                        scripts =
                            $@"window.parent.{jsMethod}(""{contentInfo.Title}"", ""{nodeID}"", ""{contentID}"", ""{pageUrl}"", ""{imageUrl}"", ""{imageSrc}"", ""{summary}"");";

                        if (Request.QueryString["itemIndex"] != null)
                        {
                            scripts =
                                $@"window.parent.{jsMethod}({itemIndex}, ""{contentInfo.Title}"", {nodeID}, {contentID});";
                        }
                    }

                    JsUtils.OpenWindow.CloseModalPageWithoutRefresh(Page, scripts);
                }
            }
            catch (Exception ex)
            {
                FailMessage(ex, "失败：" + ex.Message);
            }
        }
    }
}
