using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.WeiXin.Model;
using SiteServer.WeiXin.Core;
using System.Collections.Generic;
using BaiRong.Controls;

namespace SiteServer.WeiXin.BackgroundPages
{
    public class BackgroundKeywordResourceAdd : BackgroundBasePageWX
    {
        public Literal ltlNav;
        public Literal ltlSite;
        public Button btnContentSelect;
        public Button btnChannelSelect;
        public TextBox tbTitle;
        public TextBox tbTaxis;
        public Literal ltlPreview;
        public TextBox tbSummary;
        public BREditor breContent;
        public TextBox tbNavigationUrl;
        public Literal ltlArrow;
        public Literal ltlScript;

        private int keywordID;
        private int resourceID;
        private int floor;
        private bool isSingle;

        public static string GetRedirectUrl(int publishmentSystemID, int keywordID, int resourceID, int floor, bool isSingle)
        {
            return PageUtils.GetWXUrl(
                $"background_keywordResourceAdd.aspx?publishmentSystemID={publishmentSystemID}&keywordID={keywordID}&resourceID={resourceID}&floor={floor}&isSingle={isSingle}");
        }

        public string GetUploadUrl()
        {
            return BackgroundAjaxUpload.GetImageUrlUploadUrl(PublishmentSystemID);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            keywordID = TranslateUtils.ToInt(GetQueryString("keywordID"));
            resourceID = TranslateUtils.ToInt(GetQueryString("resourceID"));
            floor = TranslateUtils.ToInt(GetQueryString("floor"));
            isSingle = TranslateUtils.ToBool(GetQueryString("isSingle"));

            if (!IsPostBack)
            {
                ltlPreview.Text = @"
<p class=""js_cover upload_preview"" style=""display:none"">
    <input type=""hidden"" id=""imageUrl"" name=""imageUrl"" value="""" />
    <img src="""" width=""64"">
    <a class=""js_removeCover"" href=""javascript:;"" onclick=""deleteImageUrl();return false;"">删除</a>
</p>";
                ltlNav.Text = GetNavHtml(EResourceType.Site);

                ltlSite.Text = @"
<div id=""titles"" class=""well well-small"" style=""display:none""></div>
<input id=""channelID"" name=""channelID"" type=""hidden"" value="""" />
<input id=""contentID"" name=""contentID"" type=""hidden"" value="""" />";

                if (resourceID > 0)
                {
                    var resourceInfo = DataProviderWX.KeywordResourceDAO.GetResourceInfo(resourceID);

                    if (resourceInfo.ResourceType == EResourceType.Site)
                    {
                        var siteHtml = MPUtils.GetSitePreivewHtml(PublishmentSystemInfo, resourceInfo.ChannelID, resourceInfo.ContentID);
                        if (!string.IsNullOrEmpty(siteHtml))
                        {
                            ltlSite.Text = $@"
<div id=""titles"" class=""well well-small"">{siteHtml}</div>
<input id=""channelID"" name=""channelID"" type=""hidden"" value=""{resourceInfo.ChannelID}"" />
<input id=""contentID"" name=""contentID"" type=""hidden"" value=""{resourceInfo.ContentID}"" />";
                        }
                    }

                    tbTitle.Text = resourceInfo.Title;
                    tbTaxis.Text = resourceInfo.Taxis.ToString();
                    if (!string.IsNullOrEmpty(resourceInfo.ImageUrl))
                    {
                        ltlPreview.Text = $@"
<p class=""js_cover upload_preview"">
    <input type=""hidden"" id=""imageUrl"" name=""imageUrl"" value=""{resourceInfo.ImageUrl}"" />
    <img src=""{PageUtility.ParseNavigationUrl(PublishmentSystemInfo, resourceInfo.ImageUrl)}"" width=""64"">
    <a class=""js_removeCover"" href=""javascript:;"" onclick=""deleteImageUrl();return false;"">删除</a>
</p>";
                    }
                    tbSummary.Text = resourceInfo.Summary;
                    if (resourceInfo.IsShowCoverPic == false)
                    {
                        ltlScript.Text += "<script>$('.js_show_cover_pic').click();</script>";
                    }
                    breContent.Text = resourceInfo.Content;
                    tbNavigationUrl.Text = resourceInfo.NavigationUrl;

                    ltlScript.Text +=
                        $@"<script>$('.nav a.{EResourceTypeUtils.GetValue(resourceInfo.ResourceType)}').click();</script>";
                }

                btnContentSelect.Attributes.Add("onclick", "parent." + Modal.ContentSelect.GetOpenWindowString(PublishmentSystemID, false, "contentSelect"));
                btnChannelSelect.Attributes.Add("onclick", "parent." + CMS.BackgroundPages.Modal.ChannelSelect.GetOpenWindowString(PublishmentSystemID));

                var top = 0;
                if (floor > 1)
                {
                    top = 67 + (floor - 1) * 103;
                }
                ltlArrow.Text =
                    $@"<i class=""arrow arrow_out"" style=""margin-top: {top}px;""></i><i class=""arrow arrow_in"" style=""margin-top: {top}px;""></i>";
            }
        }

        private string GetNavHtml(EResourceType resourceType)
        {
            var nav = string.Empty;

            var list = new List<EResourceType>();
            list.Add(EResourceType.Site);
            list.Add(EResourceType.Content);
            list.Add(EResourceType.Url);

            foreach (var rType in list)
            {
                nav +=
                    $@"<li class=""{(rType == resourceType ? "active" : string.Empty)}""><a href=""javascript:;"" class=""{EResourceTypeUtils
                        .GetValue(rType)}"" resourceType=""{EResourceTypeUtils.GetValue(rType)}"">显示{EResourceTypeUtils
                        .GetText(rType)}</a></li>";
            }

            return nav;
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                try
                {
                    var resourceInfo = new KeywordResourceInfo();
                    resourceInfo.PublishmentSystemID = PublishmentSystemID;
                    resourceInfo.KeywordID = keywordID;

                    if (resourceID > 0)
                    {
                        resourceInfo = DataProviderWX.KeywordResourceDAO.GetResourceInfo(resourceID);
                    }

                    resourceInfo.Title = tbTitle.Text;
                    resourceInfo.Taxis = Convert.ToInt32(tbTaxis.Text);
                    resourceInfo.ResourceType = EResourceTypeUtils.GetEnumType(Request.Form["resourceType"]);
                    resourceInfo.ImageUrl = Request.Form["imageUrl"];
                    resourceInfo.Summary = tbSummary.Text;
                    resourceInfo.IsShowCoverPic = TranslateUtils.ToBool(Request.Form["isShowCoverPic"]);
                    resourceInfo.Content = breContent.Text;
                    resourceInfo.NavigationUrl = tbNavigationUrl.Text;
                    resourceInfo.ChannelID = TranslateUtils.ToInt(Request.Form["channelID"]);
                    resourceInfo.ContentID = TranslateUtils.ToInt(Request.Form["contentID"]);

                    var isError = false;

                    if (resourceInfo.ResourceType == EResourceType.Site)
                    {
                        if (resourceInfo.ChannelID == 0)
                        {
                            FailMessage("图文回复保存失败，请选择需要显示的微网站页面！");
                            ltlScript.Text +=
                                $@"<script>$('.nav a.{EResourceTypeUtils.GetValue(EResourceType.Site)}').click();</script>";
                            isError = true;
                        }
                    }
                    else if (resourceInfo.ResourceType == EResourceType.Url)
                    {
                        if (string.IsNullOrEmpty(resourceInfo.NavigationUrl))
                        {
                            FailMessage("图文回复保存失败，请填写需要链接的网址！");
                            ltlScript.Text +=
                                $@"<script>$('.nav a.{EResourceTypeUtils.GetValue(EResourceType.Url)}').click();</script>";
                            isError = true;
                        }
                    }

                    if (!isError)
                    {
                        if (resourceID > 0)
                        {
                            DataProviderWX.KeywordResourceDAO.Update(resourceInfo);

                            StringUtility.AddLog(PublishmentSystemID, "修改关键词图文回复");
                            SuccessMessage("关键词图文回复修改成功！");
                        }
                        else
                        {
                            resourceID = DataProviderWX.KeywordResourceDAO.Insert(resourceInfo);

                            StringUtility.AddLog(PublishmentSystemID, "新增关键词图文回复");
                            SuccessMessage("关键词图文回复新增成功！");
                        }

                        FileUtilityWX.CreateWeiXinContent(PublishmentSystemInfo, keywordID, resourceID);

                        var redirectUrl = BackgroundKeywordNewsAdd.GetRedirectUrl(PublishmentSystemID, keywordID, resourceID, isSingle);
                        ltlScript.Text +=
                            $@"<script>setTimeout(""parent.redirect('{redirectUrl}')"", 1500);</script>";
                    }
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "关键词图文回复配置失败！");
                }
            }
        }
    }
}
