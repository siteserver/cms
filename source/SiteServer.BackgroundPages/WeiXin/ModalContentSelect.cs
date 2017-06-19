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
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Permissions;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.IO;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;
using SiteServer.CMS.WeiXin.MP;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class ModalContentSelect : BasePageCms
    {
        public DropDownList NodeIdDropDownList;
        public DropDownList State;
        public CheckBox IsDuplicate;
        public DropDownList SearchType;
        public TextBox Keyword;
        public DateTimeTextBox DateFrom;
        public DateTimeTextBox DateTo;

        public Repeater RptContents;
        public SqlPager SpContents;

        int _nodeId = 0;
        NodeInfo _nodeInfo;
        private ETableStyle _tableStyle;
        private StringCollection _attributesOfDisplay;
        List<int> _relatedIdentities;
        List<TableStyleInfo> _tableStyleInfoList;

        private bool _isMultiple;
        private string _jsMethod;
        private int _itemIndex;

        private bool _isKeywordAdd;
        private int _keywordId;

        public static string GetRedirectUrlByKeywordAddList(int publishmentSystemId, bool isMultiple, int keywordId)
        {
            return PageUtils.GetWeiXinUrl(nameof(ModalContentSelect), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"isMultiple", isMultiple.ToString()},
                {"keywordId", keywordId.ToString()}
            });
        }

        public static string GetOpenWindowString(int publishmentSystemId, bool isMultiple, string jsMethod)
        {
            return PageUtils.GetOpenWindowString("选择微官网内容",
                PageUtils.GetWeiXinUrl(nameof(ModalContentSelect), new NameValueCollection
                {
                    {"publishmentSystemId", publishmentSystemId.ToString()},
                    {"isMultiple", isMultiple.ToString()},
                    {"jsMethod", jsMethod}
                }));
        }

        public static string GetOpenWindowStringByItemIndex(int publishmentSystemId, string jsMethod, string itemIndex)
        {
            return PageUtils.GetOpenWindowString("选择微官网内容",
                PageUtils.GetWeiXinUrl(nameof(ModalCardSnSetting), new NameValueCollection
                {
                    {"publishmentSystemId", publishmentSystemId.ToString()},
                    {"jsMethod", jsMethod},
                    {"itemIndex", itemIndex}
                }));
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemId");
            _isMultiple = TranslateUtils.ToBool(Request.QueryString["isMultiple"]);
            _jsMethod = Request.QueryString["jsMethod"];
            _itemIndex = TranslateUtils.ToInt(Request.QueryString["itemIndex"]);

            _isKeywordAdd = TranslateUtils.ToBool(Request.QueryString["isKeywordAdd"]);
            _keywordId = TranslateUtils.ToInt(Request.QueryString["keywordID"]);

            if (!string.IsNullOrEmpty(Request.QueryString["NodeID"]))
            {
                _nodeId = int.Parse(Request.QueryString["NodeID"]);
            }
            else
            {
                _nodeId = PublishmentSystemId;
            }
            _nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, _nodeId);
            _tableStyle = NodeManager.GetTableStyle(PublishmentSystemInfo, _nodeInfo);
            var tableName = NodeManager.GetTableName(PublishmentSystemInfo, _nodeInfo);
            _attributesOfDisplay = TranslateUtils.StringCollectionToStringCollection(NodeManager.GetContentAttributesOfDisplay(PublishmentSystemId, _nodeId));
            _relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(PublishmentSystemId, _nodeId);
            _tableStyleInfoList = TableStyleManager.GetTableStyleInfoList(_tableStyle, tableName, _relatedIdentities);

            SpContents.ControlToPaginate = RptContents;
            if (string.IsNullOrEmpty(Request.QueryString["NodeID"]))
            {
                var pm = PermissionsManager.GetPermissions(Body.AdministratorName);
                var stateType = ETriStateUtils.GetEnumType(State.SelectedValue);
                SpContents.SelectCommand = DataProvider.ContentDao.GetSelectCommend(_tableStyle, tableName, PublishmentSystemId, _nodeId, pm.IsSystemAdministrator, ProductPermissionsManager.Current.OwningNodeIdList, SearchType.SelectedValue, Keyword.Text, DateFrom.Text, DateTo.Text, true, stateType, !IsDuplicate.Checked, false);
            }
            else
            {
                var pm = PermissionsManager.GetPermissions(Body.AdministratorName);
                var stateType = ETriStateUtils.GetEnumType(Request.QueryString["State"]);
                SpContents.SelectCommand = DataProvider.ContentDao.GetSelectCommend(_tableStyle, tableName, PublishmentSystemId, _nodeId, pm.IsSystemAdministrator, ProductPermissionsManager.Current.OwningNodeIdList, Request.QueryString["SearchType"], Request.QueryString["Keyword"], Request.QueryString["DateFrom"], Request.QueryString["DateTo"], true, stateType, !TranslateUtils.ToBool(Request.QueryString["IsDuplicate"]), false);
            }
            SpContents.ItemsPerPage = PublishmentSystemInfo.Additional.PageSize;
            SpContents.SortField = ContentAttribute.Id;
            SpContents.SortMode = SortMode.DESC;
            SpContents.OrderByString = ETaxisTypeUtils.GetOrderByString(_tableStyle, ETaxisType.OrderByIdDesc);
            RptContents.ItemDataBound += rptContents_ItemDataBound;

            if (!IsPostBack)
            {
                NodeManager.AddListItems(NodeIdDropDownList.Items, PublishmentSystemInfo, true, true, Body.AdministratorName);

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

                ETriStateUtils.AddListItems(State, "全部", "已审核", "待审核");

                //添加隐藏属性
                SearchType.Items.Add(new ListItem("内容ID", ContentAttribute.Id));
                SearchType.Items.Add(new ListItem("添加者", ContentAttribute.AddUserName));
                SearchType.Items.Add(new ListItem("最后修改者", ContentAttribute.LastEditUserName));

                if (!string.IsNullOrEmpty(Request.QueryString["NodeID"]))
                {
                    if (PublishmentSystemId != _nodeId)
                    {
                        ControlUtils.SelectListItems(NodeIdDropDownList, _nodeId.ToString());
                    }
                    ControlUtils.SelectListItems(State, Request.QueryString["State"]);
                    IsDuplicate.Checked = TranslateUtils.ToBool(Request.QueryString["IsDuplicate"]);
                    ControlUtils.SelectListItems(SearchType, Request.QueryString["SearchType"]);
                    Keyword.Text = Request.QueryString["Keyword"];
                    DateFrom.Text = Request.QueryString["DateFrom"];
                    DateTo.Text = Request.QueryString["DateTo"];
                }

                SpContents.DataBind();
            }
        }

        private readonly Hashtable _valueHashtable = new Hashtable();

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
                var nodeName = _valueHashtable[contentInfo.NodeId] as string;
                if (nodeName == null)
                {
                    nodeName = NodeManager.GetNodeNameNavigation(PublishmentSystemId, contentInfo.NodeId);
                    _valueHashtable[contentInfo.NodeId] = nodeName;
                }
                ltlChannel.Text = nodeName;

                var imageUrl = contentInfo.GetExtendedAttribute(BackgroundContentAttribute.ImageUrl);
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    ltlItemImageUrl.Text =
                        $@"<img src=""{PageUtility.ParseNavigationUrl(PublishmentSystemInfo, imageUrl)}"" style=""max-width:78px;max-height:78px;"" />";
                }

                ltlItemSummary.Text = MPUtils.GetSummary(contentInfo.GetExtendedAttribute(BackgroundContentAttribute.Summary), contentInfo.GetExtendedAttribute(BackgroundContentAttribute.Content));

                if (_isMultiple)
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
                        _pageUrl = PageUtils.GetWeiXinUrl(nameof(ModalContentSelect), new NameValueCollection
                        {
                            {"PublishmentSystemId", PublishmentSystemId.ToString()},
                            {"isMultiple", _isMultiple.ToString()},
                            {"jsMethod", _jsMethod},
                            {"itemIndex", _itemIndex.ToString()},
                            {"isKeywordAdd", _isKeywordAdd.ToString()},
                            {"keywordID", _keywordId.ToString()},
                            {"NodeID", NodeIdDropDownList.SelectedValue},
                            {"State", State.SelectedValue},
                            {"IsDuplicate", IsDuplicate.Checked.ToString()},
                            {"SearchType", SearchType.SelectedValue},
                            {"Keyword", Keyword.Text},
                            {"DateFrom", DateFrom.Text},
                            {"DateTo", DateTo.Text}
                        });
                    }
                    else
                    {
                        _pageUrl = PageUtils.GetWeiXinUrl(nameof(ModalContentSelect), new NameValueCollection
                        {
                            {"PublishmentSystemId", PublishmentSystemId.ToString()},
                            {"isMultiple", _isMultiple.ToString()},
                            {"jsMethod", _jsMethod},
                            {"itemIndex", _itemIndex.ToString()},
                            {"isKeywordAdd", _isKeywordAdd.ToString()},
                            {"keywordID", _keywordId.ToString()},
                            {"NodeID", NodeIdDropDownList.SelectedValue},
                            {"State", State.SelectedValue},
                            {"IsDuplicate", IsDuplicate.Checked.ToString()},
                            {"SearchType", SearchType.SelectedValue},
                            {"Keyword", Keyword.Text},
                            {"DateFrom", DateFrom.Text},
                            {"DateTo", DateTo.Text}
                        });
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

                if (_isKeywordAdd)
                {
                    if (_keywordId > 0)
                    {
                        var idsList = TranslateUtils.StringCollectionToStringList(idsCollection);
                        var resourceId = 0;
                        foreach (var ids in idsList)
                        {
                            var nodeId = TranslateUtils.ToInt(ids.Split('_')[0]);
                            var contentId = TranslateUtils.ToInt(ids.Split('_')[1]);
                            var tableStyle = NodeManager.GetTableStyle(PublishmentSystemInfo, nodeId);
                            var tableName = NodeManager.GetTableName(PublishmentSystemInfo, nodeId);

                            var contentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, contentId);

                            var resourceInfo = new KeywordResourceInfo();

                            resourceInfo.ResourceId = 0;
                            resourceInfo.PublishmentSystemId = PublishmentSystemId;
                            resourceInfo.KeywordId = _keywordId;
                            resourceInfo.Title = contentInfo.Title;
                            resourceInfo.ImageUrl = contentInfo.GetExtendedAttribute(BackgroundContentAttribute.ImageUrl);
                            resourceInfo.Summary = MPUtils.GetSummary(contentInfo.GetExtendedAttribute(BackgroundContentAttribute.Summary), contentInfo.GetExtendedAttribute(BackgroundContentAttribute.Content));
                            resourceInfo.ResourceType = EResourceType.Site;
                            resourceInfo.IsShowCoverPic = false;
                            resourceInfo.Content = contentInfo.GetExtendedAttribute(BackgroundContentAttribute.Content);
                            resourceInfo.NavigationUrl = string.Empty;
                            resourceInfo.ChannelId = contentInfo.NodeId;
                            resourceInfo.ContentId = contentInfo.Id;
                            resourceInfo.Taxis = 0;

                            var id = DataProviderWx.KeywordResourceDao.Insert(resourceInfo);
                            if (resourceId == 0)
                            {
                                resourceId = id;
                            }
                        }

                        var redirectUrl = PageKeywordNewsAdd.GetRedirectUrl(PublishmentSystemId, _keywordId, resourceId, !_isMultiple);
                        PageUtils.CloseModalPageAndRedirect(Page, redirectUrl);
                    }
                }
                else
                {
                    var scripts = string.Empty;
                    if (_isMultiple)
                    {
                        var titleBuilder = new StringBuilder();
                        var idsList = TranslateUtils.StringCollectionToStringList(idsCollection);
                        foreach (var ids in idsList)
                        {
                            var nodeId = TranslateUtils.ToInt(ids.Split('_')[0]);
                            var contentId = TranslateUtils.ToInt(ids.Split('_')[1]);
                            var tableStyle = NodeManager.GetTableStyle(PublishmentSystemInfo, nodeId);
                            var tableName = NodeManager.GetTableName(PublishmentSystemInfo, nodeId);

                            var contentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, contentId);

                            titleBuilder.AppendFormat("{0}&nbsp;<a href='{1}' target='blank'>查看</a><br />", contentInfo.Title, PageUtilityWX.GetContentUrl(PublishmentSystemInfo, contentInfo));
                        }
                        scripts = $@"window.parent.{_jsMethod}(""{idsCollection}"", ""{titleBuilder}"");";
                    }
                    else
                    {
                        var nodeId = TranslateUtils.ToInt(idsCollection.Split('_')[0]);
                        var contentId = TranslateUtils.ToInt(idsCollection.Split('_')[1]);
                        var tableStyle = NodeManager.GetTableStyle(PublishmentSystemInfo, nodeId);
                        var tableName = NodeManager.GetTableName(PublishmentSystemInfo, nodeId);

                        var contentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, contentId);

                        var imageUrl = contentInfo.GetExtendedAttribute(BackgroundContentAttribute.ImageUrl);
                        var imageSrc = PageUtility.ParseNavigationUrl(PublishmentSystemInfo, imageUrl);
                        var summary = MPUtils.GetSummary(contentInfo.GetExtendedAttribute(BackgroundContentAttribute.Summary), contentInfo.GetExtendedAttribute(BackgroundContentAttribute.Content));

                        var pageUrl = PageUtilityWX.GetContentUrl(PublishmentSystemInfo, contentInfo);
                        scripts =
                            $@"window.parent.{_jsMethod}(""{contentInfo.Title}"", ""{nodeId}"", ""{contentId}"", ""{pageUrl}"", ""{imageUrl}"", ""{imageSrc}"", ""{summary}"");";

                        if (Request.QueryString["itemIndex"] != null)
                        {
                            scripts =
                                $@"window.parent.{_jsMethod}({_itemIndex}, ""{contentInfo.Title}"", {nodeId}, {contentId});";
                        }
                    }

                    PageUtils.CloseModalPageWithoutRefresh(Page, scripts);
                }
            }
            catch (Exception ex)
            {
                FailMessage(ex, "失败：" + ex.Message);
            }
        }
    }
}
