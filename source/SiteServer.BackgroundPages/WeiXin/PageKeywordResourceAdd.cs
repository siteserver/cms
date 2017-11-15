using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Cms;
using SiteServer.BackgroundPages.Controls;
using SiteServer.CMS.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.IO;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;
using SiteServer.CMS.WeiXin.MP;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class PageKeywordResourceAdd : BasePageCms
    {
        public Literal LtlNav;
        public Literal LtlSite;
        public Button BtnContentSelect;
        public Button BtnChannelSelect;
        public TextBox TbTitle;
        public TextBox TbTaxis;
        public Literal LtlPreview;
        public TextBox TbSummary;
        public UEditor BreContent;
        public TextBox TbNavigationUrl;
        public Literal LtlArrow;
        public Literal LtlScript;

        private int _keywordId;
        private int _resourceId;
        private int _floor;
        private bool _isSingle;

        public static string GetRedirectUrl(int publishmentSystemId, int keywordId, int resourceId, int floor, bool isSingle)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageKeywordResourceAdd), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"keywordId", keywordId.ToString()},
                {"resourceId", resourceId.ToString()},
                {"floor", floor.ToString()},
                {"isSingle", isSingle.ToString()}
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

            _keywordId = Body.GetQueryInt("keywordID");
            _resourceId = Body.GetQueryInt("resourceID");
            _floor = Body.GetQueryInt("floor");
            _isSingle = TranslateUtils.ToBool(Body.GetQueryString("isSingle"));

            if (!IsPostBack)
            {
                LtlPreview.Text = @"
<p class=""js_cover upload_preview"" style=""display:none"">
    <input type=""hidden"" id=""imageUrl"" name=""imageUrl"" value="""" />
    <img src="""" width=""64"">
    <a class=""js_removeCover"" href=""javascript:;"" onclick=""deleteImageUrl();return false;"">删除</a>
</p>";
                LtlNav.Text = GetNavHtml(EResourceType.Site);

                LtlSite.Text = @"
<div id=""titles"" class=""well well-small"" style=""display:none""></div>
<input id=""channelID"" name=""channelID"" type=""hidden"" value="""" />
<input id=""contentID"" name=""contentID"" type=""hidden"" value="""" />";

                if (_resourceId > 0)
                {
                    var resourceInfo = DataProviderWx.KeywordResourceDao.GetResourceInfo(_resourceId);

                    if (resourceInfo.ResourceType == EResourceType.Site)
                    {
                        var siteHtml = MPUtils.GetSitePreivewHtml(PublishmentSystemInfo, resourceInfo.ChannelId, resourceInfo.ContentId);
                        if (!string.IsNullOrEmpty(siteHtml))
                        {
                            LtlSite.Text = $@"
<div id=""titles"" class=""well well-small"">{siteHtml}</div>
<input id=""channelID"" name=""channelID"" type=""hidden"" value=""{resourceInfo.ChannelId}"" />
<input id=""contentID"" name=""contentID"" type=""hidden"" value=""{resourceInfo.ContentId}"" />";
                        }
                    }

                    TbTitle.Text = resourceInfo.Title;
                    TbTaxis.Text = resourceInfo.Taxis.ToString();
                    if (!string.IsNullOrEmpty(resourceInfo.ImageUrl))
                    {
                        LtlPreview.Text = $@"
<p class=""js_cover upload_preview"">
    <input type=""hidden"" id=""imageUrl"" name=""imageUrl"" value=""{resourceInfo.ImageUrl}"" />
    <img src=""{PageUtility.ParseNavigationUrl(PublishmentSystemInfo, resourceInfo.ImageUrl)}"" width=""64"">
    <a class=""js_removeCover"" href=""javascript:;"" onclick=""deleteImageUrl();return false;"">删除</a>
</p>";
                    }
                    TbSummary.Text = resourceInfo.Summary;
                    if (resourceInfo.IsShowCoverPic == false)
                    {
                        LtlScript.Text += "<script>$('.js_show_cover_pic').click();</script>";
                    }
                    BreContent.Text = resourceInfo.Content;
                    TbNavigationUrl.Text = resourceInfo.NavigationUrl;

                    LtlScript.Text +=
                        $@"<script>$('.nav a.{EResourceTypeUtils.GetValue(resourceInfo.ResourceType)}').click();</script>";
                }

                BtnContentSelect.Attributes.Add("onclick", "parent." + ModalContentSelect.GetOpenWindowString(PublishmentSystemId, false, "contentSelect"));
                BtnChannelSelect.Attributes.Add("onclick", "parent." + ModalChannelSelect.GetOpenWindowString(PublishmentSystemId));

                var top = 0;
                if (_floor > 1)
                {
                    top = 67 + (_floor - 1) * 103;
                }
                LtlArrow.Text =
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
                    resourceInfo.PublishmentSystemId = PublishmentSystemId;
                    resourceInfo.KeywordId = _keywordId;

                    if (_resourceId > 0)
                    {
                        resourceInfo = DataProviderWx.KeywordResourceDao.GetResourceInfo(_resourceId);
                    }

                    resourceInfo.Title = TbTitle.Text;
                    resourceInfo.Taxis = Convert.ToInt32(TbTaxis.Text);
                    resourceInfo.ResourceType = EResourceTypeUtils.GetEnumType(Request.Form["resourceType"]);
                    resourceInfo.ImageUrl = Request.Form["imageUrl"];
                    resourceInfo.Summary = TbSummary.Text;
                    resourceInfo.IsShowCoverPic = TranslateUtils.ToBool(Request.Form["isShowCoverPic"]);
                    resourceInfo.Content = BreContent.Text;
                    resourceInfo.NavigationUrl = TbNavigationUrl.Text;
                    resourceInfo.ChannelId = TranslateUtils.ToInt(Request.Form["channelID"]);
                    resourceInfo.ContentId = TranslateUtils.ToInt(Request.Form["contentID"]);

                    var isError = false;

                    if (resourceInfo.ResourceType == EResourceType.Site)
                    {
                        if (resourceInfo.ChannelId == 0)
                        {
                            FailMessage("图文回复保存失败，请选择需要显示的微网站页面！");
                            LtlScript.Text +=
                                $@"<script>$('.nav a.{EResourceTypeUtils.GetValue(EResourceType.Site)}').click();</script>";
                            isError = true;
                        }
                    }
                    else if (resourceInfo.ResourceType == EResourceType.Url)
                    {
                        if (string.IsNullOrEmpty(resourceInfo.NavigationUrl))
                        {
                            FailMessage("图文回复保存失败，请填写需要链接的网址！");
                            LtlScript.Text +=
                                $@"<script>$('.nav a.{EResourceTypeUtils.GetValue(EResourceType.Url)}').click();</script>";
                            isError = true;
                        }
                    }

                    if (!isError)
                    {
                        if (_resourceId > 0)
                        {
                            DataProviderWx.KeywordResourceDao.Update(resourceInfo);

                            Body.AddSiteLog(PublishmentSystemId, "修改关键词图文回复");
                            SuccessMessage("关键词图文回复修改成功！");
                        }
                        else
                        {
                            _resourceId = DataProviderWx.KeywordResourceDao.Insert(resourceInfo);

                            Body.AddSiteLog(PublishmentSystemId, "新增关键词图文回复");
                            SuccessMessage("关键词图文回复新增成功！");
                        }

                        FileUtilityWX.CreateWeiXinContent(PublishmentSystemInfo, _keywordId, _resourceId);

                        var redirectUrl = PageKeywordNewsAdd.GetRedirectUrl(PublishmentSystemId, _keywordId, _resourceId, _isSingle);
                        LtlScript.Text +=
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
