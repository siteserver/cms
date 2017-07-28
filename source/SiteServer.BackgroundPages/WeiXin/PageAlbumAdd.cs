using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using System.Collections.Specialized;
using SiteServer.BackgroundPages.Ajax;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Manager;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class PageAlbumAdd : BasePageCms
    {
        public Literal ltlPageTitle;
        public TextBox tbKeywords;
        public TextBox tbTitle;
        public TextBox tbSummary;
        public CheckBox cbIsEnabled;
        public Literal ltlImageUrl;

        public HtmlInputHidden imageUrl;

        public Button btnSubmit;
        public Button btnReturn;

        private int albumID;

        public static string GetRedirectUrl(int publishmentSystemId, int albumID)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageAlbumAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"albumID", albumID.ToString()}
            });
        }

        public string GetUploadUrl()
        {
            return string.Empty;
            //AjaxUploadService
            //return BackgroundAjaxUpload.GetImageUrlUploadUrl(PublishmentSystemID);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");
            albumID = Body.GetQueryInt("albumID");

            if (!IsPostBack)
            {
                var pageTitle = albumID > 0 ? "编辑微相册" : "添加微相册";
                BreadCrumb(AppManager.WeiXin.LeftMenu.IdFunction, AppManager.WeiXin.LeftMenu.Function.IdAlbum, pageTitle, AppManager.WeiXin.Permission.WebSite.Album);
                ltlPageTitle.Text = pageTitle;

                ltlImageUrl.Text =
                    $@"<img id=""preview_imageUrl"" src=""{AlbumManager.GetImageUrl(PublishmentSystemInfo, string.Empty)}"" width=""370"" align=""middle"" />";

                if (albumID > 0)
                {
                    var albumInfo = DataProviderWX.AlbumDAO.GetAlbumInfo(albumID);

                    tbKeywords.Text = DataProviderWX.KeywordDAO.GetKeywords(albumInfo.KeywordID);
                    cbIsEnabled.Checked = !albumInfo.IsDisabled;
                    tbTitle.Text = albumInfo.Title;
                    if (!string.IsNullOrEmpty(albumInfo.ImageUrl))
                    {
                        ltlImageUrl.Text =
                            $@"<img id=""preview_imageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, albumInfo.ImageUrl)}"" width=""370"" align=""middle"" />";
                    }
                    tbSummary.Text = albumInfo.Summary;

                    imageUrl.Value = albumInfo.ImageUrl;
                }

                btnReturn.Attributes.Add("onclick",
                    $@"location.href=""{PageAlbum.GetRedirectUrl(PublishmentSystemId)}"";return false");
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                var isConflict = false;
                var conflictKeywords = string.Empty;
                if (!string.IsNullOrEmpty(tbKeywords.Text))
                {
                    if (albumID > 0)
                    {
                        var albumInfo = DataProviderWX.AlbumDAO.GetAlbumInfo(albumID);
                        isConflict = KeywordManager.IsKeywordUpdateConflict(PublishmentSystemId, albumInfo.KeywordID,PageUtils.FilterXss(tbKeywords.Text), out conflictKeywords);
                    }
                    else
                    {
                        isConflict = KeywordManager.IsKeywordInsertConflict(PublishmentSystemId, PageUtils.FilterXss(tbKeywords.Text), out conflictKeywords);
                    }
                }

                if (isConflict)
                {
                    FailMessage($"触发关键词“{conflictKeywords}”已存在，请设置其他关键词");
                }
                else
                {
                    var albumInfo = new AlbumInfo();
                    if (albumID > 0)
                    {
                        albumInfo = DataProviderWX.AlbumDAO.GetAlbumInfo(albumID);
                    }
                    albumInfo.PublishmentSystemID = PublishmentSystemId;

                    albumInfo.KeywordID = DataProviderWX.KeywordDAO.GetKeywordID(PublishmentSystemId, albumID > 0, tbKeywords.Text, EKeywordType.Album, albumInfo.KeywordID);
                    albumInfo.IsDisabled = !cbIsEnabled.Checked;

                    albumInfo.Title = PageUtils.FilterXss(tbTitle.Text);
                    albumInfo.ImageUrl = imageUrl.Value; ;
                    albumInfo.Summary = tbSummary.Text;

                    try
                    {
                        if (albumID > 0)
                        {
                            DataProviderWX.AlbumDAO.Update(albumInfo);

                            Body.AddLog(PublishmentSystemId, "修改微相册", $"微相册:{tbTitle.Text}");
                            SuccessMessage("修改微相册成功！");
                        }
                        else
                        {
                            albumID = DataProviderWX.AlbumDAO.Insert(albumInfo);

                            Body.AddLog(PublishmentSystemId, "添加微相册", $"微相册:{tbTitle.Text}");
                            SuccessMessage("添加微相册成功！");
                        }

                        var redirectUrl = PageAlbumContent.GetRedirectUrl(PublishmentSystemId, albumID);
                        AddWaitAndRedirectScript(redirectUrl);
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "微相册设置失败！");
                    }
                }
            }
        }
    }
}
