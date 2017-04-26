using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.WeiXin.Model;
using SiteServer.WeiXin.Core;

namespace SiteServer.WeiXin.BackgroundPages
{
    public class BackgroundKeywordNewsAdd : BackgroundBasePageWX
    {
        public Literal ltlPageTitle;

        public PlaceHolder phSingle;
        public Literal ltlSingleTitle;
        public Literal ltlSingleImageUrl;
        public Literal ltlSingleSummary;

        public PlaceHolder phMultiple;
        public Literal ltlMultipleTitle;
        public Literal ltlMultipleImageUrl;
        public Literal ltlMultipleEditUrl;
        public Repeater rptMultipleContents;
        public Literal ltlItemEditUrl;

        //public Literal ltlPreview;

        public Literal ltlIFrame;

        private int keywordID;
        private int resourceID;
        private bool isSingle;

        public static string GetRedirectUrl(int publishmentSystemID, int keywordID, int resourceID, bool isSingle)
        {
            return PageUtils.GetWXUrl(
                $"background_keywordNewsAdd.aspx?publishmentSystemID={publishmentSystemID}&keywordID={keywordID}&resourceID={resourceID}&isSingle={isSingle}");
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");
            keywordID = TranslateUtils.ToInt(GetQueryString("keywordID"));
            resourceID = TranslateUtils.ToInt(GetQueryString("resourceID"));
            isSingle = TranslateUtils.ToBool(GetQueryString("isSingle"));

            if (Request.QueryString["deleteResource"] != null)
            {
                var deleteResourceID = TranslateUtils.ToInt(Request.QueryString["deleteResourceID"]);

                try
                {
                    DataProviderWX.KeywordResourceDAO.Delete(deleteResourceID);
                    SuccessDeleteMessage();
                }
                catch (Exception ex)
                {
                    FailDeleteMessage(ex);
                }
            }

            if (!IsPostBack)
            {
                var pageTitle = keywordID == 0 ? "添加关键词图文回复" : "修改关键词图文回复";
                ltlPageTitle.Text = pageTitle;

                BreadCrumb(AppManager.WeiXin.LeftMenu.ID_Accounts, AppManager.WeiXin.LeftMenu.Function.ID_ImageReply, pageTitle, AppManager.WeiXin.Permission.WebSite.ImageReply);
                phSingle.Visible = isSingle;
                phMultiple.Visible = !isSingle;

                if (isSingle)
                {
                    var resourceInfo = new KeywordResourceInfo();

                    resourceInfo.ResourceID = 0;
                    resourceInfo.PublishmentSystemID = PublishmentSystemID;
                    resourceInfo.KeywordID = keywordID;
                    resourceInfo.Title = "标题";
                    resourceInfo.ImageUrl = string.Empty;
                    resourceInfo.Summary = string.Empty;
                    resourceInfo.ResourceType = EResourceType.Content;
                    resourceInfo.IsShowCoverPic = true;
                    resourceInfo.Content = string.Empty;
                    resourceInfo.NavigationUrl = string.Empty;
                    resourceInfo.ChannelID = 0;
                    resourceInfo.ContentID = 0;
                    resourceInfo.Taxis = 0;

                    if (resourceID > 0)
                    {
                        resourceInfo = DataProviderWX.KeywordResourceDAO.GetResourceInfo(resourceID);
                    }
                    ltlSingleTitle.Text = $@"<a href=""javascript:;"">{resourceInfo.Title}</a>";
                    if (string.IsNullOrEmpty(resourceInfo.ImageUrl))
                    {
                        ltlSingleImageUrl.Text = @"<i class=""appmsg_thumb default"">封面图片</i>";
                    }
                    else
                    {
                        ltlSingleImageUrl.Text =
                            $@"<img class=""js_appmsg_thumb"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, resourceInfo.ImageUrl)}"">";
                    }
                    ltlSingleSummary.Text = MPUtils.GetSummary(resourceInfo.Summary, resourceInfo.Content);
                }
                else
                {
                    var resourceInfoList = DataProviderWX.KeywordResourceDAO.GetResourceInfoList(keywordID);

                    var resourceInfo = new KeywordResourceInfo();

                    resourceInfo.ResourceID = 0;
                    resourceInfo.PublishmentSystemID = PublishmentSystemID;
                    resourceInfo.KeywordID = keywordID;
                    resourceInfo.Title = "标题";
                    resourceInfo.ImageUrl = string.Empty;
                    resourceInfo.Summary = string.Empty;
                    resourceInfo.ResourceType = EResourceType.Content;
                    resourceInfo.IsShowCoverPic = true;
                    resourceInfo.Content = string.Empty;
                    resourceInfo.NavigationUrl = string.Empty;
                    resourceInfo.ChannelID = 0;
                    resourceInfo.ContentID = 0;
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

                    ltlMultipleTitle.Text = $@"<a href=""javascript:;"">{resourceInfo.Title}</a>";

                    if (string.IsNullOrEmpty(resourceInfo.ImageUrl))
                    {
                        ltlMultipleImageUrl.Text = @"<i class=""appmsg_thumb default"">封面图片</i>";
                    }
                    else
                    {
                        ltlMultipleImageUrl.Text =
                            $@"<img class=""js_appmsg_thumb"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, resourceInfo.ImageUrl)}"">";
                    }
                    ltlMultipleEditUrl.Text =
                        $@"<a class=""icon18_common edit_gray js_edit"" href=""{BackgroundKeywordResourceAdd.GetRedirectUrl(
                            PublishmentSystemID, keywordID, resourceInfo.ResourceID, 1, false)}"" target=""resource"">&nbsp;&nbsp;</a>";

                    rptMultipleContents.DataSource = resourceInfoList;
                    rptMultipleContents.ItemDataBound += rptMultipleContents_ItemDataBound;
                    rptMultipleContents.DataBind();

                    ltlItemEditUrl.Text =
                        $@"<a class=""icon18_common edit_gray js_edit"" href=""{BackgroundKeywordResourceAdd.GetRedirectUrl(
                            PublishmentSystemID, keywordID, 0, resourceInfoList.Count + 2, false)}"" target=""resource"">&nbsp;&nbsp;</a>";
                }

                ltlIFrame.Text =
                    $@"<iframe frameborder=""0"" id=""resource"" name=""resource"" width=""100%"" height=""1300"" src=""{BackgroundKeywordResourceAdd
                        .GetRedirectUrl(PublishmentSystemID, keywordID, resourceID, 1, isSingle)}"" scrolling=""no""></iframe>";
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
                $@"<a class=""icon18_common edit_gray js_edit"" href=""{BackgroundKeywordResourceAdd.GetRedirectUrl(
                    PublishmentSystemID, keywordID, resourceInfo.ResourceID, e.Item.ItemIndex + 2, false)}"" target=""resource"">&nbsp;&nbsp;</a>";

            ltlDeleteUrl.Text =
                $@"<a class=""icon18_common del_gray js_del"" href=""{GetRedirectUrl(
                    PublishmentSystemID, keywordID, 0, false)}&deleteResource=true&deleteResourceID={resourceInfo
                    .ResourceID}"">&nbsp;&nbsp;</a>";
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {

            }
        }
    }
}
