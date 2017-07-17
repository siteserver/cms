using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.IO;
using SiteServer.CMS.WeiXin.Manager;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class PageSearchAdd : BasePageCms
    {
        public Literal LtlPageTitle; 

        public PlaceHolder PhStep1;
        public TextBox TbKeywords;
        public TextBox TbTitle;
        public TextBox TbSummary;
        public CheckBox CbIsEnabled;
        public Literal LtlImageUrl;
         
        public PlaceHolder PhStep2;
        public Literal LtlContentImageUrl;
        public CheckBox CbIsOutsiteSearch;
        public CheckBox CbIsNavigation;
        public TextBox TbNavTitleColor;
        public TextBox TbNavImageColor;

        public Literal LtlSearchNavs;

        public PlaceHolder PhStep3;
        public TextBox TbImageAreaTitle;
        public TextBox TbTextAreaTitle;
        public Button BtnImageChannelSelect;
        public Button BtnTextChannelSelect;
          
        public HtmlInputHidden ImageUrl;
        public HtmlInputHidden ContentImageUrl;

        public Button BtnSubmit;
        public Button BtnReturn;

        private int _searchId;
       
        public static string GetRedirectUrl(int publishmentSystemId, int searchId)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageSearchAdd), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"searchId", searchId.ToString()}
            });
        }

        public string GetUploadUrl()
        {
            return AjaxUpload.GetImageUrlUploadUrl(PublishmentSystemId);
        }
         
		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemId");
            _searchId = Body.GetQueryInt("searchID");

			if (!IsPostBack)
            {
                var pageTitle = _searchId > 0 ? "编辑微搜索" : "添加微搜索";
                BreadCrumb(AppManager.WeiXin.LeftMenu.Function.IdSearch, pageTitle, AppManager.WeiXin.Permission.WebSite.Search);
                LtlPageTitle.Text = pageTitle;
               
                LtlImageUrl.Text =
                    $@"<img id=""preview_imageUrl"" src=""{SearchManager.GetImageUrl(PublishmentSystemInfo,
                        string.Empty)}"" width=""370"" align=""middle"" />";
                LtlContentImageUrl.Text =
                    $@"<img id=""preview_contentImageUrl"" src=""{SearchManager.GetContentImageUrl(
                        PublishmentSystemInfo, string.Empty)}"" width=""370"" align=""middle"" />";

                LtlSearchNavs.Text = $@"
itemController.openFunctionSelect = function(itemIndex){{
    var openString = ""{ModalFunctionSelect.GetOpenWindowStringByItemIndex(PublishmentSystemId, "selectFunction",
                    "itemIndexValue")}"".replace(""itemIndexValue"", itemIndex);
    openString = openString.substr(0, openString.indexOf(';'));
    eval(openString);
}};
itemController.openChannelSelect = function(itemIndex){{
    var openString = ""{ModalChannelSelect.GetOpenWindowStringByItemIndex(
                    PublishmentSystemId, "selectChannel", "itemIndexValue")}"".replace(""itemIndexValue"", itemIndex);
    openString = openString.substr(0, openString.indexOf(';'));
    eval(openString);
}};
itemController.openContentSelect = function(itemIndex){{
    var openString = ""{ModalContentSelect.GetOpenWindowStringByItemIndex(PublishmentSystemId, "selectContent",
                    "itemIndexValue")}"".replace(""itemIndexValue"", itemIndex);
    openString = openString.substr(0, openString.indexOf(';'));
    eval(openString);
}};
itemController.openImageCssClassSelect = function(itemIndex){{
    var openString = ""{ModalImageCssClassSelect.GetOpenWindowStringByItemIndex(PublishmentSystemId,
                    "selectImageCssClass", "itemIndexValue")}"".replace(""itemIndexValue"", itemIndex);
    openString = openString.substr(0, openString.indexOf(';'));
    eval(openString);
}};
";
                 
                if (_searchId == 0)
                {
                    LtlSearchNavs.Text += "itemController.itemCount = 2;itemController.items = [{navigationType : 'Url', imageCssClass : 'fa fa-angle-double-down'}, {navigationType : 'Url', imageCssClass : 'fa fa-angle-double-down'}];";
                }
                else
                {
                    var searchInfo = DataProviderWx.SearchDao.GetSearchInfo(_searchId);

                    TbKeywords.Text = DataProviderWx.KeywordDao.GetKeywords(searchInfo.KeywordId);
                    CbIsEnabled.Checked = !searchInfo.IsDisabled;
                    TbTitle.Text = searchInfo.Title;
                    if (!string.IsNullOrEmpty(searchInfo.ImageUrl))
                    {
                        LtlImageUrl.Text =
                            $@"<img id=""preview_imageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, searchInfo.ImageUrl)}"" width=""370"" align=""middle"" />";
                    }
                    TbSummary.Text = searchInfo.Summary;

                    CbIsOutsiteSearch.Checked = searchInfo.IsOutsiteSearch;
                    CbIsNavigation.Checked = searchInfo.IsNavigation;
                    TbNavTitleColor.Text = searchInfo.NavTitleColor;
                    TbNavImageColor.Text = searchInfo.NavImageColor;
                    if (!string.IsNullOrEmpty(searchInfo.ContentImageUrl))
                    {
                        LtlContentImageUrl.Text =
                            $@"<img id=""preview_contentImageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, searchInfo.ContentImageUrl)}"" width=""370"" align=""middle"" />";
                    }

                    var searchNavigationInfoList = DataProviderWx.SearchNavigationDao.GetSearchNavigationInfoList(PublishmentSystemId, _searchId);
                    var itemBuilder = new StringBuilder();
                    foreach (var searchNavigationInfo in searchNavigationInfoList)
                    {
                        var searchPageTitle = string.Empty;

                        if (searchNavigationInfo.NavigationType == ENavigationTypeUtils.GetValue(ENavigationType.Url))
                        {
                            searchPageTitle = string.Empty;
                        }
                        else if (searchNavigationInfo.NavigationType == ENavigationTypeUtils.GetValue(ENavigationType.Function))
                        {
                            searchPageTitle = KeywordManager.GetFunctionName(EKeywordTypeUtils.GetEnumType(searchNavigationInfo.KeywordType), searchNavigationInfo.FunctionId);
                        }
                        else if (searchNavigationInfo.NavigationType == ENavigationTypeUtils.GetValue(ENavigationType.Site))
                        { 
                            if (searchNavigationInfo.ContentId > 0)
                            {
                                var tableStyle = NodeManager.GetTableStyle(PublishmentSystemInfo, searchNavigationInfo.ChannelId);
                                var tableName = NodeManager.GetTableName(PublishmentSystemInfo, searchNavigationInfo.ChannelId);
                                var contentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, searchNavigationInfo.ContentId);

                                var pageUrl = PageUtilityWX.GetContentUrl(PublishmentSystemInfo, contentInfo);
                                searchPageTitle = $@"内容页：{contentInfo.Title}";
                            }
                            else
                            { 
                                var nodeNames = NodeManager.GetNodeNameNavigation(PublishmentSystemId, searchNavigationInfo.ChannelId);
                                searchPageTitle = $@"栏目页：{nodeNames}";
                            }
                        }

                        itemBuilder.AppendFormat("{{id: '{0}', title: '{1}',pageTitle: '{2}', url: '{3}', imageCssClass: '{4}',navigationType:'{5}',keywordType:'{6}',functionID:'{7}',channelID:'{8}',contentID:'{9}'}},", searchNavigationInfo.Id, searchNavigationInfo.Title, searchPageTitle, searchNavigationInfo.Url, searchNavigationInfo.ImageCssClass, searchNavigationInfo.NavigationType, searchNavigationInfo.KeywordType, searchNavigationInfo.FunctionId, searchNavigationInfo.ChannelId, searchNavigationInfo.ContentId);
                    }

                    if (itemBuilder.Length > 0) itemBuilder.Length--;

                    LtlSearchNavs.Text += $@"
itemController.itemCount = {searchNavigationInfoList.Count};itemController.items = [{itemBuilder}];";

                    ImageUrl.Value = searchInfo.ImageUrl;
                    ContentImageUrl.Value = searchInfo.ContentImageUrl;

                    TbImageAreaTitle.Text = searchInfo.ImageAreaTitle;
                    TbTextAreaTitle.Text = searchInfo.TextAreaTitle;

                    if (searchInfo.ImageAreaChannelId > 0)
                    {
                        var nodeNames = NodeManager.GetNodeNameNavigation(PublishmentSystemId, searchInfo.ImageAreaChannelId);
                        LtlSearchNavs.Text += $@"
$(document).ready(function() {{
    selectChannel(1, '{nodeNames}', {searchInfo.ImageAreaChannelId});
}});
";
                    }
                    if (searchInfo.TextAreaChannelId > 0)
                    {
                        var nodeNames = NodeManager.GetNodeNameNavigation(PublishmentSystemId, searchInfo.TextAreaChannelId);
                        LtlSearchNavs.Text += $@"
$(document).ready(function() {{
    selectChannel(2, '{nodeNames}', {searchInfo.TextAreaChannelId});
}});
";
                    }
                }

                BtnImageChannelSelect.Attributes.Add("onclick", ModalChannelSelect.GetOpenWindowStringByItemIndex(PublishmentSystemId, "selectChannel", "1"));
                BtnTextChannelSelect.Attributes.Add("onclick", ModalChannelSelect.GetOpenWindowStringByItemIndex(PublishmentSystemId, "selectChannel", "2"));

                BtnReturn.Attributes.Add("onclick",
                    $@"location.href=""{PageSearch.GetRedirectUrl(PublishmentSystemId)}"";return false");
			}
		}

		public override void Submit_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
                var selectedStep = 0;
                if (PhStep1.Visible)
                {
                    selectedStep = 1;
                }
                else if (PhStep2.Visible)
                {
                    selectedStep = 2;
                }
                else if (PhStep3.Visible)
                {
                    selectedStep = 3;
                }
                
                PhStep1.Visible = PhStep2.Visible = PhStep3.Visible = false;

                if (selectedStep == 1)
                {
                    var isConflict = false;
                    var conflictKeywords = string.Empty;
                    if (!string.IsNullOrEmpty(TbKeywords.Text))
                    {
                        if (_searchId > 0)
                        {
                            var searchInfo = DataProviderWx.SearchDao.GetSearchInfo(_searchId);
                            isConflict = KeywordManager.IsKeywordUpdateConflict(PublishmentSystemId, searchInfo.KeywordId, PageUtils.FilterXss(TbKeywords.Text), out conflictKeywords);
                        }
                        else
                        {
                            isConflict = KeywordManager.IsKeywordInsertConflict(PublishmentSystemId, PageUtils.FilterXss(TbKeywords.Text), out conflictKeywords);
                        }
                    }

                    if (isConflict)
                    {
                        FailMessage($"触发关键词“{conflictKeywords}”已存在，请设置其他关键词");
                        PhStep1.Visible = true;
                    }
                    else
                    {
                        PhStep2.Visible = true;
                    }
                }
                else if (selectedStep == 2)
                {
                    var isItemReady = true;

                    if (isItemReady)
                    {
                        var itemCount = TranslateUtils.ToInt(Request.Form["itemCount"]);

                        if (itemCount > 0)
                        {
                            var itemIdList = TranslateUtils.StringCollectionToIntList(Request.Form["itemID"]);
                            var imageCssClassList = TranslateUtils.StringCollectionToStringList(Request.Form["itemImageCssClass"]);
                            var keywordTypeList = TranslateUtils.StringCollectionToStringList(Request.Form["itemKeywordType"]);
                            var functionIdList = TranslateUtils.StringCollectionToIntList(Request.Form["itemFunctionID"]);
                            var channelIdList = TranslateUtils.StringCollectionToIntList(Request.Form["itemChannelID"]);
                            var contentIdList = TranslateUtils.StringCollectionToIntList(Request.Form["itemContentID"]);

                            var titleList = TranslateUtils.StringCollectionToStringList(Request.Form["itemTitle"]);
                            var navigationTypeList = TranslateUtils.StringCollectionToStringList(Request.Form["itemNavigationType"]);
                            var urlList = TranslateUtils.StringCollectionToStringList(Request.Form["itemUrl"]);

                            var navigationInfoList = new List<SearchNavigationInfo>();
                            for (var i = 0; i < itemCount; i++)
                            {
                                var navigationInfo = new SearchNavigationInfo { Id = itemIdList[i], PublishmentSystemId = PublishmentSystemId, SearchId = _searchId, Title = titleList[i], Url = urlList[i], ImageCssClass = imageCssClassList[i], NavigationType = navigationTypeList[i], KeywordType = keywordTypeList[i], FunctionId = functionIdList[i], ChannelId = channelIdList[i], ContentId = contentIdList[i] };

                                if (string.IsNullOrEmpty(navigationInfo.Title))
                                {
                                    FailMessage("保存失败，导航链接名称为必填项");
                                    isItemReady = false;
                                }
                                if (string.IsNullOrEmpty(navigationInfo.ImageCssClass))
                                {
                                    FailMessage("保存失败，导航链接图标为必填项");
                                    isItemReady = false;
                                }
                                if (navigationInfo.NavigationType == ENavigationTypeUtils.GetValue(ENavigationType.Url) && string.IsNullOrEmpty(navigationInfo.Url))
                                {
                                    FailMessage("保存失败，导航链接地址为必填项");
                                    isItemReady = false;
                                }

                                navigationInfoList.Add(navigationInfo);
                            }

                            if (isItemReady)
                            {
                                DataProviderWx.SearchNavigationDao.DeleteAllNotInIdList(PublishmentSystemId, _searchId, itemIdList);

                                foreach (var navigationInfo in navigationInfoList)
                                {
                                    if (navigationInfo.Id > 0)
                                    {
                                        DataProviderWx.SearchNavigationDao.Update(navigationInfo);
                                    }
                                    else
                                    {
                                        DataProviderWx.SearchNavigationDao.Insert(navigationInfo);
                                    }
                                }
                            }
                        }
                    }

                    if (isItemReady)
                    {
                        PhStep3.Visible = true;
                        BtnSubmit.Text = "确 认";
                    }
                    else
                    {
                        PhStep2.Visible = true;
                    }

                }
                else if (selectedStep == 3)
                {  
                    var searchInfo = new SearchInfo();
                    if (_searchId > 0)
                    {
                        searchInfo = DataProviderWx.SearchDao.GetSearchInfo (_searchId);
                    }
                    searchInfo.PublishmentSystemId = PublishmentSystemId;
                    searchInfo.KeywordId = DataProviderWx.KeywordDao.GetKeywordId(PublishmentSystemId, _searchId > 0, TbKeywords.Text, EKeywordType.Search , searchInfo.KeywordId);
                    searchInfo.IsDisabled = !CbIsEnabled.Checked;
                    searchInfo.Title = PageUtils.FilterXss(TbTitle.Text);
                    searchInfo.ImageUrl = ImageUrl.Value; ;
                    searchInfo.Summary = TbSummary.Text;
                    searchInfo.ContentImageUrl = ContentImageUrl.Value;

                    searchInfo.IsOutsiteSearch = CbIsOutsiteSearch.Checked;
                    searchInfo.IsNavigation = CbIsNavigation.Checked;
                    searchInfo.NavTitleColor = TbNavTitleColor.Text;
                    searchInfo.NavImageColor = TbNavImageColor.Text;
                    
                    searchInfo.ImageAreaTitle = TbImageAreaTitle.Text;
                    searchInfo.ImageAreaChannelId = TranslateUtils.ToInt(Request.Form["imageChannelID"]);
                    searchInfo.TextAreaTitle = TbTextAreaTitle.Text;
                    searchInfo.TextAreaChannelId = TranslateUtils.ToInt(Request.Form["textChannelID"]);

                    try
                    {
                        if (_searchId > 0)
                        {
                            DataProviderWx.SearchDao.Update(searchInfo);

                            LogUtils.AddAdminLog(Body.AdministratorName, "修改微搜索", $"微搜索:{TbTitle.Text}");
                            SuccessMessage("修改微搜索成功！");
                        }
                        else
                        {
                            _searchId = DataProviderWx.SearchDao.Insert(searchInfo);

                            DataProviderWx.SearchNavigationDao.UpdateSearchId(PublishmentSystemId, _searchId);

                            LogUtils.AddAdminLog(Body.AdministratorName, "添加微搜索", $"微搜索:{TbTitle.Text}");
                            SuccessMessage("添加微搜索成功！");
                        }

                        var redirectUrl = PageSearch.GetRedirectUrl(PublishmentSystemId);
                        AddWaitAndRedirectScript(redirectUrl);
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "微搜索设置失败！");
                    }
                }
			}
		}
	}
}
