using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;
using SiteServer.CMS.WeiXin.MP;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class PageKeywordNewsAdd : BasePageCms
    {
        public Literal LtlPageTitle;

        public PlaceHolder PhSingle;
        public Literal LtlSingleTitle;
        public Literal LtlSingleImageUrl;
        public Literal LtlSingleSummary;

        public PlaceHolder PhMultiple;
        public Literal LtlMultipleTitle;
        public Literal LtlMultipleImageUrl;
        public Literal LtlMultipleEditUrl;
        public Repeater RptMultipleContents;
        public Literal LtlItemEditUrl;

        //public Literal ltlPreview;

        public Literal LtlIFrame;

        private int _keywordId;
        private int _resourceId;
        private bool _isSingle;

        public static string GetRedirectUrl(int publishmentSystemId, int keywordId, int resourceId, bool isSingle)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageKeywordNewsAdd), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"keywordId", keywordId.ToString()},
                {"resourceId", resourceId.ToString()},
                {"isSingle", isSingle.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemId");
            _keywordId = Body.GetQueryInt("keywordID");
            _resourceId = Body.GetQueryInt("resourceID");
            _isSingle = TranslateUtils.ToBool(Body.GetQueryString("isSingle"));

            if (Request.QueryString["deleteResource"] != null)
            {
                var deleteResourceId = TranslateUtils.ToInt(Request.QueryString["deleteResourceID"]);

                try
                {
                    DataProviderWx.KeywordResourceDao.Delete(deleteResourceId);
                    SuccessDeleteMessage();
                }
                catch (Exception ex)
                {
                    FailDeleteMessage(ex);
                }
            }

            if (!IsPostBack)
            {
                var pageTitle = _keywordId == 0 ? "添加关键词图文回复" : "修改关键词图文回复";
                LtlPageTitle.Text = pageTitle;

                BreadCrumb(AppManager.WeiXin.LeftMenu.IdAccounts, pageTitle, AppManager.WeiXin.Permission.WebSite.ImageReply);
                PhSingle.Visible = _isSingle;
                PhMultiple.Visible = !_isSingle;

                if (_isSingle)
                {
                    var resourceInfo = new KeywordResourceInfo();

                    resourceInfo.ResourceId = 0;
                    resourceInfo.PublishmentSystemId = PublishmentSystemId;
                    resourceInfo.KeywordId = _keywordId;
                    resourceInfo.Title = "标题";
                    resourceInfo.ImageUrl = string.Empty;
                    resourceInfo.Summary = string.Empty;
                    resourceInfo.ResourceType = EResourceType.Content;
                    resourceInfo.IsShowCoverPic = true;
                    resourceInfo.Content = string.Empty;
                    resourceInfo.NavigationUrl = string.Empty;
                    resourceInfo.ChannelId = 0;
                    resourceInfo.ContentId = 0;
                    resourceInfo.Taxis = 0;

                    if (_resourceId > 0)
                    {
                        resourceInfo = DataProviderWx.KeywordResourceDao.GetResourceInfo(_resourceId);
                    }
                    LtlSingleTitle.Text = $@"<a href=""javascript:;"">{resourceInfo.Title}</a>";
                    if (string.IsNullOrEmpty(resourceInfo.ImageUrl))
                    {
                        LtlSingleImageUrl.Text = @"<i class=""appmsg_thumb default"">封面图片</i>";
                    }
                    else
                    {
                        LtlSingleImageUrl.Text =
                            $@"<img class=""js_appmsg_thumb"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, resourceInfo.ImageUrl)}"">";
                    }
                    LtlSingleSummary.Text = MPUtils.GetSummary(resourceInfo.Summary, resourceInfo.Content);
                }
                else
                {
                    var resourceInfoList = DataProviderWx.KeywordResourceDao.GetResourceInfoList(_keywordId);

                    var resourceInfo = new KeywordResourceInfo();

                    resourceInfo.ResourceId = 0;
                    resourceInfo.PublishmentSystemId = PublishmentSystemId;
                    resourceInfo.KeywordId = _keywordId;
                    resourceInfo.Title = "标题";
                    resourceInfo.ImageUrl = string.Empty;
                    resourceInfo.Summary = string.Empty;
                    resourceInfo.ResourceType = EResourceType.Content;
                    resourceInfo.IsShowCoverPic = true;
                    resourceInfo.Content = string.Empty;
                    resourceInfo.NavigationUrl = string.Empty;
                    resourceInfo.ChannelId = 0;
                    resourceInfo.ContentId = 0;
                    resourceInfo.Taxis = 0;

                    if (resourceInfoList.Count <= 1)
                    {
                        resourceInfoList.Add(resourceInfo);
                    }

                    if (resourceInfoList.Count > 1)
                    {
                        resourceInfo = resourceInfoList[0];
                        resourceInfoList.Remove(resourceInfo);
                    }

                    LtlMultipleTitle.Text = $@"<a href=""javascript:;"">{resourceInfo.Title}</a>";

                    if (string.IsNullOrEmpty(resourceInfo.ImageUrl))
                    {
                        LtlMultipleImageUrl.Text = @"<i class=""appmsg_thumb default"">封面图片</i>";
                    }
                    else
                    {
                        LtlMultipleImageUrl.Text =
                            $@"<img class=""js_appmsg_thumb"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, resourceInfo.ImageUrl)}"">";
                    }
                    LtlMultipleEditUrl.Text =
                        $@"<a class=""icon18_common edit_gray js_edit"" href=""{PageKeywordResourceAdd.GetRedirectUrl(
                            PublishmentSystemId, _keywordId, resourceInfo.ResourceId, 1, false)}"" target=""resource"">&nbsp;&nbsp;</a>";

                    RptMultipleContents.DataSource = resourceInfoList;
                    RptMultipleContents.ItemDataBound += rptMultipleContents_ItemDataBound;
                    RptMultipleContents.DataBind();

                    LtlItemEditUrl.Text =
                        $@"<a class=""icon18_common edit_gray js_edit"" href=""{PageKeywordResourceAdd.GetRedirectUrl(
                            PublishmentSystemId, _keywordId, 0, resourceInfoList.Count + 2, false)}"" target=""resource"">&nbsp;&nbsp;</a>";
                }

                LtlIFrame.Text =
                    $@"<iframe frameborder=""0"" id=""resource"" name=""resource"" width=""100%"" height=""1300"" src=""{PageKeywordResourceAdd
                        .GetRedirectUrl(PublishmentSystemId, _keywordId, _resourceId, 1, _isSingle)}"" scrolling=""no""></iframe>";
            }
        }

        void rptMultipleContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var resourceInfo = e.Item.DataItem as KeywordResourceInfo;

            var ltlImageUrl = e.Item.FindControl("ltlImageUrl") as Literal;
            var ltlTitle = e.Item.FindControl("ltlTitle") as Literal;
            var ltlEditUrl = e.Item.FindControl("ltlEditUrl") as Literal;
            var ltlDeleteUrl = e.Item.FindControl("ltlDeleteUrl") as Literal;

            if (string.IsNullOrEmpty(resourceInfo.ImageUrl))
            {
                ltlImageUrl.Text = @"<i class=""appmsg_thumb default"">缩略图</i>";
            }
            else
            {
                ltlImageUrl.Text =
                    $@"<img class=""js_appmsg_thumb appmsg_thumb"" style=""max-width:78px;max-height:78px;display:block"" src=""{PageUtility
                        .ParseNavigationUrl(PublishmentSystemInfo, resourceInfo.ImageUrl)}"">";
            }
            ltlTitle.Text = $@"<a href=""javascript:;"">{resourceInfo.Title}</a>";

            ltlEditUrl.Text =
                $@"<a class=""icon18_common edit_gray js_edit"" href=""{PageKeywordResourceAdd.GetRedirectUrl(
                    PublishmentSystemId, _keywordId, resourceInfo.ResourceId, e.Item.ItemIndex + 2, false)}"" target=""resource"">&nbsp;&nbsp;</a>";

            ltlDeleteUrl.Text =
                $@"<a class=""icon18_common del_gray js_del"" href=""{GetRedirectUrl(
                    PublishmentSystemId, _keywordId, 0, false)}&deleteResource=true&deleteResourceID={resourceInfo
                    .ResourceId}"">&nbsp;&nbsp;</a>";
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {

            }
        }
    }
}
