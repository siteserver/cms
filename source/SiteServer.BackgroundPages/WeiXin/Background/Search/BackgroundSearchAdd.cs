using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.WeiXin.Core;
using SiteServer.WeiXin.Model;
using System.Collections.Generic;
using System.Text;

namespace SiteServer.WeiXin.BackgroundPages
{
    public class BackgroundSearchAdd : BackgroundBasePageWX
	{
        public Literal ltlPageTitle; 

        public PlaceHolder phStep1;
        public TextBox tbKeywords;
        public TextBox tbTitle;
        public TextBox tbSummary;
        public CheckBox cbIsEnabled;
        public Literal ltlImageUrl;
         
        public PlaceHolder phStep2;
        public Literal ltlContentImageUrl;
        public CheckBox cbIsOutsiteSearch;
        public CheckBox cbIsNavigation;
        public TextBox tbNavTitleColor;
        public TextBox tbNavImageColor;

        public Literal ltlSearchNavs;

        public PlaceHolder phStep3;
        public TextBox tbImageAreaTitle;
        public TextBox tbTextAreaTitle;
        public Button btnImageChannelSelect;
        public Button btnTextChannelSelect;
          
        public HtmlInputHidden imageUrl;
        public HtmlInputHidden contentImageUrl;

        public Button btnSubmit;
        public Button btnReturn;

        private int searchID;
       
        public static string GetRedirectUrl(int publishmentSystemID, int searchID)
        {
            return PageUtils.GetWXUrl(
                $"background_searchAdd.aspx?publishmentSystemID={publishmentSystemID}&searchID={searchID}");
        }

        public string GetUploadUrl()
        {
            return BackgroundAjaxUpload.GetImageUrlUploadUrl(PublishmentSystemID);
        }
         
		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");
            searchID = TranslateUtils.ToInt(GetQueryString("searchID"));

			if (!IsPostBack)
            {
                var pageTitle = searchID > 0 ? "编辑微搜索" : "添加微搜索";
                BreadCrumb(AppManager.WeiXin.LeftMenu.ID_Function, AppManager.WeiXin.LeftMenu.Function.ID_Search, pageTitle, AppManager.WeiXin.Permission.WebSite.Search);
                ltlPageTitle.Text = pageTitle;
               
                ltlImageUrl.Text =
                    $@"<img id=""preview_imageUrl"" src=""{SearchManager.GetImageUrl(PublishmentSystemInfo,
                        string.Empty)}"" width=""370"" align=""middle"" />";
                ltlContentImageUrl.Text =
                    $@"<img id=""preview_contentImageUrl"" src=""{SearchManager.GetContentImageUrl(
                        PublishmentSystemInfo, string.Empty)}"" width=""370"" align=""middle"" />";

                ltlSearchNavs.Text = $@"
itemController.openFunctionSelect = function(itemIndex){{
    var openString = ""{Modal.FunctionSelect.GetOpenWindowStringByItemIndex(PublishmentSystemID, "selectFunction",
                    "itemIndexValue")}"".replace(""itemIndexValue"", itemIndex);
    openString = openString.substr(0, openString.indexOf(';'));
    eval(openString);
}};
itemController.openChannelSelect = function(itemIndex){{
    var openString = ""{CMS.BackgroundPages.Modal.ChannelSelect.GetOpenWindowStringByItemIndex(
                    PublishmentSystemID, "selectChannel", "itemIndexValue")}"".replace(""itemIndexValue"", itemIndex);
    openString = openString.substr(0, openString.indexOf(';'));
    eval(openString);
}};
itemController.openContentSelect = function(itemIndex){{
    var openString = ""{Modal.ContentSelect.GetOpenWindowStringByItemIndex(PublishmentSystemID, "selectContent",
                    "itemIndexValue")}"".replace(""itemIndexValue"", itemIndex);
    openString = openString.substr(0, openString.indexOf(';'));
    eval(openString);
}};
itemController.openImageCssClassSelect = function(itemIndex){{
    var openString = ""{Modal.ImageCssClassSelect.GetOpenWindowStringByItemIndex(PublishmentSystemID,
                    "selectImageCssClass", "itemIndexValue")}"".replace(""itemIndexValue"", itemIndex);
    openString = openString.substr(0, openString.indexOf(';'));
    eval(openString);
}};
";
                 
                if (searchID == 0)
                {
                    ltlSearchNavs.Text += "itemController.itemCount = 2;itemController.items = [{navigationType : 'Url', imageCssClass : 'fa fa-angle-double-down'}, {navigationType : 'Url', imageCssClass : 'fa fa-angle-double-down'}];";
                }
                else
                {
                    var searchInfo = DataProviderWX.SearchDAO.GetSearchInfo(searchID);

                    tbKeywords.Text = DataProviderWX.KeywordDAO.GetKeywords(searchInfo.KeywordID);
                    cbIsEnabled.Checked = !searchInfo.IsDisabled;
                    tbTitle.Text = searchInfo.Title;
                    if (!string.IsNullOrEmpty(searchInfo.ImageUrl))
                    {
                        ltlImageUrl.Text =
                            $@"<img id=""preview_imageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, searchInfo.ImageUrl)}"" width=""370"" align=""middle"" />";
                    }
                    tbSummary.Text = searchInfo.Summary;

                    cbIsOutsiteSearch.Checked = searchInfo.IsOutsiteSearch;
                    cbIsNavigation.Checked = searchInfo.IsNavigation;
                    tbNavTitleColor.Text = searchInfo.NavTitleColor;
                    tbNavImageColor.Text = searchInfo.NavImageColor;
                    if (!string.IsNullOrEmpty(searchInfo.ContentImageUrl))
                    {
                        ltlContentImageUrl.Text =
                            $@"<img id=""preview_contentImageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, searchInfo.ContentImageUrl)}"" width=""370"" align=""middle"" />";
                    }

                    var searchNavigationInfoList = DataProviderWX.SearchNavigationDAO.GetSearchNavigationInfoList(PublishmentSystemID, searchID);
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
                            searchPageTitle = KeywordManager.GetFunctionName(EKeywordTypeUtils.GetEnumType(searchNavigationInfo.KeywordType), searchNavigationInfo.FunctionID);
                        }
                        else if (searchNavigationInfo.NavigationType == ENavigationTypeUtils.GetValue(ENavigationType.Site))
                        { 
                            if (searchNavigationInfo.ContentID > 0)
                            {
                                var tableStyle = NodeManager.GetTableStyle(PublishmentSystemInfo, searchNavigationInfo.ChannelID);
                                var tableName = NodeManager.GetTableName(PublishmentSystemInfo, searchNavigationInfo.ChannelID);
                                var contentInfo = DataProvider.ContentDAO.GetContentInfo(tableStyle, tableName, searchNavigationInfo.ContentID);

                                var pageUrl = PageUtilityWX.GetContentUrl(PublishmentSystemInfo, contentInfo);
                                searchPageTitle = $@"内容页：{contentInfo.Title}";
                            }
                            else
                            { 
                                var nodeNames = NodeManager.GetNodeNameNavigation(PublishmentSystemID, searchNavigationInfo.ChannelID);
                                var pageUrl = PageUtility.GetChannelUrl(PublishmentSystemInfo, NodeManager.GetNodeInfo(PublishmentSystemID, searchNavigationInfo.ChannelID));
                                searchPageTitle = $@"栏目页：{nodeNames}";
                            }
                        }

                        itemBuilder.AppendFormat("{{id: '{0}', title: '{1}',pageTitle: '{2}', url: '{3}', imageCssClass: '{4}',navigationType:'{5}',keywordType:'{6}',functionID:'{7}',channelID:'{8}',contentID:'{9}'}},", searchNavigationInfo.ID, searchNavigationInfo.Title, searchPageTitle, searchNavigationInfo.Url, searchNavigationInfo.ImageCssClass, searchNavigationInfo.NavigationType, searchNavigationInfo.KeywordType, searchNavigationInfo.FunctionID, searchNavigationInfo.ChannelID, searchNavigationInfo.ContentID);
                    }

                    if (itemBuilder.Length > 0) itemBuilder.Length--;

                    ltlSearchNavs.Text += $@"
itemController.itemCount = {searchNavigationInfoList.Count};itemController.items = [{itemBuilder.ToString()}];";

                    imageUrl.Value = searchInfo.ImageUrl;
                    contentImageUrl.Value = searchInfo.ContentImageUrl;

                    tbImageAreaTitle.Text = searchInfo.ImageAreaTitle;
                    tbTextAreaTitle.Text = searchInfo.TextAreaTitle;

                    if (searchInfo.ImageAreaChannelID > 0)
                    {
                        var nodeNames = NodeManager.GetNodeNameNavigation(PublishmentSystemID, searchInfo.ImageAreaChannelID);
                        ltlSearchNavs.Text += $@"
$(document).ready(function() {{
    selectChannel(1, '{nodeNames}', {searchInfo.ImageAreaChannelID});
}});
";
                    }
                    if (searchInfo.TextAreaChannelID > 0)
                    {
                        var nodeNames = NodeManager.GetNodeNameNavigation(PublishmentSystemID, searchInfo.TextAreaChannelID);
                        ltlSearchNavs.Text += $@"
$(document).ready(function() {{
    selectChannel(2, '{nodeNames}', {searchInfo.TextAreaChannelID});
}});
";
                    }
                }

                btnImageChannelSelect.Attributes.Add("onclick", CMS.BackgroundPages.Modal.ChannelSelect.GetOpenWindowStringByItemIndex(PublishmentSystemID, "selectChannel", "1"));
                btnTextChannelSelect.Attributes.Add("onclick", CMS.BackgroundPages.Modal.ChannelSelect.GetOpenWindowStringByItemIndex(PublishmentSystemID, "selectChannel", "2"));

                btnReturn.Attributes.Add("onclick",
                    $@"location.href=""{BackgroundSearch.GetRedirectUrl(PublishmentSystemID)}"";return false");
			}
		}

		public override void Submit_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
                var selectedStep = 0;
                if (phStep1.Visible)
                {
                    selectedStep = 1;
                }
                else if (phStep2.Visible)
                {
                    selectedStep = 2;
                }
                else if (phStep3.Visible)
                {
                    selectedStep = 3;
                }
                
                phStep1.Visible = phStep2.Visible = phStep3.Visible = false;

                if (selectedStep == 1)
                {
                    var isConflict = false;
                    var conflictKeywords = string.Empty;
                    if (!string.IsNullOrEmpty(tbKeywords.Text))
                    {
                        if (searchID > 0)
                        {
                            var searchInfo = DataProviderWX.SearchDAO.GetSearchInfo(searchID);
                            isConflict = KeywordManager.IsKeywordUpdateConflict(PublishmentSystemID, searchInfo.KeywordID, PageUtils.FilterXSS(tbKeywords.Text), out conflictKeywords);
                        }
                        else
                        {
                            isConflict = KeywordManager.IsKeywordInsertConflict(PublishmentSystemID, PageUtils.FilterXSS(tbKeywords.Text), out conflictKeywords);
                        }
                    }

                    if (isConflict)
                    {
                        FailMessage($"触发关键词“{conflictKeywords}”已存在，请设置其他关键词");
                        phStep1.Visible = true;
                    }
                    else
                    {
                        phStep2.Visible = true;
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
                            var itemIDList = TranslateUtils.StringCollectionToIntList(Request.Form["itemID"]);
                            var imageCssClassList = TranslateUtils.StringCollectionToStringList(Request.Form["itemImageCssClass"]);
                            var keywordTypeList = TranslateUtils.StringCollectionToStringList(Request.Form["itemKeywordType"]);
                            var functionIDList = TranslateUtils.StringCollectionToIntList(Request.Form["itemFunctionID"]);
                            var channelIDList = TranslateUtils.StringCollectionToIntList(Request.Form["itemChannelID"]);
                            var contentIDList = TranslateUtils.StringCollectionToIntList(Request.Form["itemContentID"]);

                            var titleList = TranslateUtils.StringCollectionToStringList(Request.Form["itemTitle"]);
                            var navigationTypeList = TranslateUtils.StringCollectionToStringList(Request.Form["itemNavigationType"]);
                            var urlList = TranslateUtils.StringCollectionToStringList(Request.Form["itemUrl"]);

                            var navigationInfoList = new List<SearchNavigationInfo>();
                            for (var i = 0; i < itemCount; i++)
                            {
                                var navigationInfo = new SearchNavigationInfo { ID = itemIDList[i], PublishmentSystemID = PublishmentSystemID, SearchID = searchID, Title = titleList[i], Url = urlList[i], ImageCssClass = imageCssClassList[i], NavigationType = navigationTypeList[i], KeywordType = keywordTypeList[i], FunctionID = functionIDList[i], ChannelID = channelIDList[i], ContentID = contentIDList[i] };

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
                                DataProviderWX.SearchNavigationDAO.DeleteAllNotInIDList(PublishmentSystemID, searchID, itemIDList);

                                foreach (var navigationInfo in navigationInfoList)
                                {
                                    if (navigationInfo.ID > 0)
                                    {
                                        DataProviderWX.SearchNavigationDAO.Update(navigationInfo);
                                    }
                                    else
                                    {
                                        DataProviderWX.SearchNavigationDAO.Insert(navigationInfo);
                                    }
                                }
                            }
                        }
                    }

                    if (isItemReady)
                    {
                        phStep3.Visible = true;
                        btnSubmit.Text = "确 认";
                    }
                    else
                    {
                        phStep2.Visible = true;
                    }

                }
                else if (selectedStep == 3)
                {  
                    var searchInfo = new SearchInfo();
                    if (searchID > 0)
                    {
                        searchInfo = DataProviderWX.SearchDAO.GetSearchInfo (searchID);
                    }
                    searchInfo.PublishmentSystemID = PublishmentSystemID;
                    searchInfo.KeywordID = DataProviderWX.KeywordDAO.GetKeywordID(PublishmentSystemID, searchID > 0, tbKeywords.Text, EKeywordType.Search , searchInfo.KeywordID);
                    searchInfo.IsDisabled = !cbIsEnabled.Checked;
                    searchInfo.Title = PageUtils.FilterXSS(tbTitle.Text);
                    searchInfo.ImageUrl = imageUrl.Value; ;
                    searchInfo.Summary = tbSummary.Text;
                    searchInfo.ContentImageUrl = contentImageUrl.Value;

                    searchInfo.IsOutsiteSearch = cbIsOutsiteSearch.Checked;
                    searchInfo.IsNavigation = cbIsNavigation.Checked;
                    searchInfo.NavTitleColor = tbNavTitleColor.Text;
                    searchInfo.NavImageColor = tbNavImageColor.Text;
                    
                    searchInfo.ImageAreaTitle = tbImageAreaTitle.Text;
                    searchInfo.ImageAreaChannelID = TranslateUtils.ToInt(Request.Form["imageChannelID"]);
                    searchInfo.TextAreaTitle = tbTextAreaTitle.Text;
                    searchInfo.TextAreaChannelID = TranslateUtils.ToInt(Request.Form["textChannelID"]);

                    try
                    {
                        if (searchID > 0)
                        {
                            DataProviderWX.SearchDAO.Update(searchInfo);

                            LogUtils.AddLog(BaiRongDataProvider.AdministratorDao.UserName, "修改微搜索",
                                $"微搜索:{tbTitle.Text}");
                            SuccessMessage("修改微搜索成功！");
                        }
                        else
                        {
                            searchID = DataProviderWX.SearchDAO.Insert(searchInfo);

                            DataProviderWX.SearchNavigationDAO.UpdateSearchID(PublishmentSystemID, searchID);

                            LogUtils.AddLog(BaiRongDataProvider.AdministratorDao.UserName, "添加微搜索",
                                $"微搜索:{tbTitle.Text}");
                            SuccessMessage("添加微搜索成功！");
                        }

                        var redirectUrl = BackgroundSearch.GetRedirectUrl(PublishmentSystemID);
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
