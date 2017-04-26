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
        public TextBox tbTitle;
        public Literal ltlImageUrl;
        public HtmlInputHidden imageUrl;

        private int id;
        private int albumID;

        public static string GetOpenWindowStringToAdd(int publishmentSystemId, int albumID, int id)
        {
            return PageUtils.GetOpenWindowString("新建相册",
                PageUtils.GetWeiXinUrl(nameof(ModalAlbumContentAdd), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"albumID", albumID.ToString()},
                    {"id", id.ToString()}
                }), 400, 450);
        }

        public static string GetOpenWindowStringToEdit(int publishmentSystemId, int albumID, int id)
        {
            return PageUtils.GetOpenWindowString("编辑相册",
                PageUtils.GetWeiXinUrl(nameof(ModalAlbumContentAdd), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"albumID", albumID.ToString()},
                    {"id", id.ToString()}
                }), 400, 450);
        }

        public string GetUploadUrl()
        {
            return string.Empty;
            //return BackgroundAjaxUpload.GetImageUrlUploadUrl(PublishmentSystemID);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            id = Body.GetQueryInt("id");
            albumID = Body.GetQueryInt("albumID");
            ltlImageUrl.Text =
                $@"<div style=""width:220px;""><img id=""preview_imageUrl"" class=""appmsg_thumb"" src=""{string.Empty}"" width=""220"" height=""220"" align=""middle"" /></div>";
            
            if (!IsPostBack)
            { 
                if (id > 0)
                {
                    var albumContentInfo = DataProviderWX.AlbumContentDAO.GetAlbumContentInfo(id);

                    tbTitle.Text = albumContentInfo.Title;
                    imageUrl.Value = albumContentInfo.LargeImageUrl;
                    ltlImageUrl.Text =
                        $@"<div style=""width:220px;""><img id=""preview_imageUrl"" class=""appmsg_thumb"" src=""{PageUtility
                            .ParseNavigationUrl(PublishmentSystemInfo, albumContentInfo.LargeImageUrl)}"" width=""220"" height=""220"" align=""middle"" /></div>";
                }
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            try
            {
                if (id == 0)
                {
                    var albumContentInfo = new AlbumContentInfo();
                    albumContentInfo.PublishmentSystemID = PublishmentSystemId;
                    albumContentInfo.AlbumID = albumID;
                    albumContentInfo.ParentID = 0;
                    albumContentInfo.Title = tbTitle.Text;
                    albumContentInfo.LargeImageUrl = imageUrl.Value;

                    DataProviderWX.AlbumContentDAO.Insert(albumContentInfo);

                }
                else
                {
                    var albumContentInfo = DataProviderWX.AlbumContentDAO.GetAlbumContentInfo(id);
                    albumContentInfo.Title = tbTitle.Text;
                    albumContentInfo.LargeImageUrl = imageUrl.Value;

                    DataProviderWX.AlbumContentDAO.Update(albumContentInfo);

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
