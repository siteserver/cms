using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using System.Collections.Specialized;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Manager;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class PageAlbumAdd : BasePageCms
    {
        public Literal LtlPageTitle;
        public TextBox TbKeywords;
        public TextBox TbTitle;
        public TextBox TbSummary;
        public CheckBox CbIsEnabled;
        public Literal LtlImageUrl;

        public HtmlInputHidden ImageUrl;

        public Button BtnSubmit;
        public Button BtnReturn;

        private int _albumId;

        public static string GetRedirectUrl(int publishmentSystemId, int albumId)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageAlbumAdd), new NameValueCollection
            {
                {"PublishmentSystemId", publishmentSystemId.ToString()},
                {"albumID", albumId.ToString()}
            });
        }

        public string GetUploadUrl()
        {
            return string.Empty;
            //AjaxUploadService
            //return BackgroundAjaxUpload.GetImageUrlUploadUrl(PublishmentSystemId);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemId");
            _albumId = Body.GetQueryInt("albumID");

            if (!IsPostBack)
            {
                var pageTitle = _albumId > 0 ? "编辑微相册" : "添加微相册";
                BreadCrumb(AppManager.WeiXin.LeftMenu.Function.IdAlbum, pageTitle, AppManager.WeiXin.Permission.WebSite.Album);
                LtlPageTitle.Text = pageTitle;

                LtlImageUrl.Text =
                    $@"<img id=""preview_imageUrl"" src=""{AlbumManager.GetImageUrl(PublishmentSystemInfo, string.Empty)}"" width=""370"" align=""middle"" />";

                if (_albumId > 0)
                {
                    var albumInfo = DataProviderWx.AlbumDao.GetAlbumInfo(_albumId);

                    TbKeywords.Text = DataProviderWx.KeywordDao.GetKeywords(albumInfo.KeywordId);
                    CbIsEnabled.Checked = !albumInfo.IsDisabled;
                    TbTitle.Text = albumInfo.Title;
                    if (!string.IsNullOrEmpty(albumInfo.ImageUrl))
                    {
                        LtlImageUrl.Text =
                            $@"<img id=""preview_imageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, albumInfo.ImageUrl)}"" width=""370"" align=""middle"" />";
                    }
                    TbSummary.Text = albumInfo.Summary;

                    ImageUrl.Value = albumInfo.ImageUrl;
                }

                BtnReturn.Attributes.Add("onclick",
                    $@"location.href=""{PageAlbum.GetRedirectUrl(PublishmentSystemId)}"";return false");
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                var isConflict = false;
                var conflictKeywords = string.Empty;
                if (!string.IsNullOrEmpty(TbKeywords.Text))
                {
                    if (_albumId > 0)
                    {
                        var albumInfo = DataProviderWx.AlbumDao.GetAlbumInfo(_albumId);
                        isConflict = KeywordManager.IsKeywordUpdateConflict(PublishmentSystemId, albumInfo.KeywordId,PageUtils.FilterXss(TbKeywords.Text), out conflictKeywords);
                    }
                    else
                    {
                        isConflict = KeywordManager.IsKeywordInsertConflict(PublishmentSystemId, PageUtils.FilterXss(TbKeywords.Text), out conflictKeywords);
                    }
                }

                if (isConflict)
                {
                    FailMessage($"触发关键词“{conflictKeywords}”已存在，请设置其他关键词");
                }
                else
                {
                    var albumInfo = new AlbumInfo();
                    if (_albumId > 0)
                    {
                        albumInfo = DataProviderWx.AlbumDao.GetAlbumInfo(_albumId);
                    }
                    albumInfo.PublishmentSystemId = PublishmentSystemId;

                    albumInfo.KeywordId = DataProviderWx.KeywordDao.GetKeywordId(PublishmentSystemId, _albumId > 0, TbKeywords.Text, EKeywordType.Album, albumInfo.KeywordId);
                    albumInfo.IsDisabled = !CbIsEnabled.Checked;

                    albumInfo.Title = PageUtils.FilterXss(TbTitle.Text);
                    albumInfo.ImageUrl = ImageUrl.Value; ;
                    albumInfo.Summary = TbSummary.Text;

                    try
                    {
                        if (_albumId > 0)
                        {
                            DataProviderWx.AlbumDao.Update(albumInfo);

                            Body.AddSiteLog(PublishmentSystemId, "修改微相册", $"微相册:{TbTitle.Text}");
                            SuccessMessage("修改微相册成功！");
                        }
                        else
                        {
                            _albumId = DataProviderWx.AlbumDao.Insert(albumInfo);

                            Body.AddSiteLog(PublishmentSystemId, "添加微相册", $"微相册:{TbTitle.Text}");
                            SuccessMessage("添加微相册成功！");
                        }

                        var redirectUrl = PageAlbumContent.GetRedirectUrl(PublishmentSystemId, _albumId);
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
