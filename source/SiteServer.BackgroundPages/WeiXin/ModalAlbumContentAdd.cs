using System;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class ModalAlbumContentAdd : BasePageCms
    {
        public TextBox TbTitle;
        public Literal LtlImageUrl;
        public HtmlInputHidden ImageUrl;

        private int _id;
        private int _albumId;

        public static string GetOpenWindowStringToAdd(int publishmentSystemId, int albumId, int id)
        {
            return PageUtils.GetOpenWindowString("新建相册",
                PageUtils.GetWeiXinUrl(nameof(ModalAlbumContentAdd), new NameValueCollection
                {
                    {"PublishmentSystemId", publishmentSystemId.ToString()},
                    {"albumID", albumId.ToString()},
                    {"id", id.ToString()}
                }), 400, 450);
        }

        public static string GetOpenWindowStringToEdit(int publishmentSystemId, int albumId, int id)
        {
            return PageUtils.GetOpenWindowString("编辑相册",
                PageUtils.GetWeiXinUrl(nameof(ModalAlbumContentAdd), new NameValueCollection
                {
                    {"PublishmentSystemId", publishmentSystemId.ToString()},
                    {"albumID", albumId.ToString()},
                    {"id", id.ToString()}
                }), 400, 450);
        }

        public string GetUploadUrl()
        {
            return string.Empty;
            //return BackgroundAjaxUpload.GetImageUrlUploadUrl(PublishmentSystemId);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _id = Body.GetQueryInt("id");
            _albumId = Body.GetQueryInt("albumID");
            LtlImageUrl.Text =
                $@"<div style=""width:220px;""><img id=""preview_imageUrl"" class=""appmsg_thumb"" src=""{string.Empty}"" width=""220"" height=""220"" align=""middle"" /></div>";
            
            if (!IsPostBack)
            { 
                if (_id > 0)
                {
                    var albumContentInfo = DataProviderWx.AlbumContentDao.GetAlbumContentInfo(_id);

                    TbTitle.Text = albumContentInfo.Title;
                    ImageUrl.Value = albumContentInfo.LargeImageUrl;
                    LtlImageUrl.Text =
                        $@"<div style=""width:220px;""><img id=""preview_imageUrl"" class=""appmsg_thumb"" src=""{PageUtility
                            .ParseNavigationUrl(PublishmentSystemInfo, albumContentInfo.LargeImageUrl)}"" width=""220"" height=""220"" align=""middle"" /></div>";
                }
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            try
            {
                if (_id == 0)
                {
                    var albumContentInfo = new AlbumContentInfo();
                    albumContentInfo.PublishmentSystemId = PublishmentSystemId;
                    albumContentInfo.AlbumId = _albumId;
                    albumContentInfo.ParentId = 0;
                    albumContentInfo.Title = TbTitle.Text;
                    albumContentInfo.LargeImageUrl = ImageUrl.Value;

                    DataProviderWx.AlbumContentDao.Insert(albumContentInfo);

                }
                else
                {
                    var albumContentInfo = DataProviderWx.AlbumContentDao.GetAlbumContentInfo(_id);
                    albumContentInfo.Title = TbTitle.Text;
                    albumContentInfo.LargeImageUrl = ImageUrl.Value;

                    DataProviderWx.AlbumContentDao.Update(albumContentInfo);

                }

                PageUtils.CloseModalPage(Page);
            }
            catch (Exception ex)
            {
                FailMessage(ex, "失败：" + ex.Message);
            }
        }
    }
}
