using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Ajax;
using SiteServer.CMS.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class ModalAlbumContentPhotoUpload : BasePageCms
    {
        public Literal ltlScript;

        private int albumID;
        private int parentID;

        public static string GetOpenWindowStringToAdd(int publishmentSystemId, int albumID, int parentID)
        {
            return PageUtils.GetOpenWindowString("上传照片",
                PageUtils.GetWeiXinUrl(nameof(ModalAlbumContentPhotoUpload), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"albumID", albumID.ToString()},
                    {"parentID", parentID.ToString()}
                }));
        }

        public string GetContentPhotoUploadMultipleUrl()
        {
            return AjaxUploadService.GetContentPhotoUploadMultipleUrl(PublishmentSystemId);
        }

        public string GetContentPhotoUploadSingleUrl()
        {
            return AjaxUploadService.GetContentPhotoUploadSingleUrl(PublishmentSystemId);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            albumID = Body.GetQueryInt("albumID");
            parentID = Body.GetQueryInt("parentID");

            if (!IsPostBack)
            {
                var list = new List<AlbumContentInfo>();
                if (parentID > 0)
                {
                    list = DataProviderWX.AlbumContentDAO.GetAlbumContentInfoList(PublishmentSystemId, albumID, parentID);
                }

                var scriptBuilder = new StringBuilder();

                foreach (var albumContentInfo in list)
                {
                    scriptBuilder.AppendFormat(@"
add_form({0}, '{1}', '{2}', '{3}', '{4}');
", albumContentInfo.ID, StringUtils.ToJsString(PageUtility.ParseNavigationUrl(PublishmentSystemInfo, albumContentInfo.ImageUrl)), StringUtils.ToJsString(albumContentInfo.ImageUrl), StringUtils.ToJsString(albumContentInfo.LargeImageUrl), albumContentInfo.Title);
                }

                ltlScript.Text = $@"
$(document).ready(function(){{
	{scriptBuilder.ToString()}
}});
";
            }
        }

        public string GetPreviewImageSize()
        {
            return $@"width=""{200}""";
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                var albumContentIDList = new List<int>();
                if (parentID > 0)
                {
                    albumContentIDList = DataProviderWX.AlbumContentDAO.GetAlbumContentIDList(PublishmentSystemId, albumID, parentID);
                }
                var photo_Count = TranslateUtils.ToInt(Request.Form["Photo_Count"]);
                if (photo_Count > 0)
                {
                    for (var index = 1; index <= photo_Count; index++)
                    {
                        var id = TranslateUtils.ToInt(Request.Form["ID_" + index]);
                        var smallUrl = Request.Form["SmallUrl_" + index];
                        var largeUrl = Request.Form["LargeUrl_" + index];
                        var title = Request.Form["imgTitle_" + index];

                        if (!string.IsNullOrEmpty(smallUrl) && !string.IsNullOrEmpty(largeUrl))
                        {
                            if (id > 0)
                            {
                                var albumContentInfo = DataProviderWX.AlbumContentDAO.GetAlbumContentInfo(id);
                                if (albumContentInfo != null)
                                {
                                    albumContentInfo.ImageUrl = smallUrl;
                                    albumContentInfo.LargeImageUrl = largeUrl;
                                    albumContentInfo.Title = title;

                                    DataProviderWX.AlbumContentDAO.Update(albumContentInfo);
                                }
                                albumContentIDList.Remove(id);
                            }
                            else
                            {
                                var albumContentInfo = new AlbumContentInfo();
                                albumContentInfo.PublishmentSystemID = PublishmentSystemId;
                                albumContentInfo.AlbumID = albumID;
                                albumContentInfo.ParentID = parentID;
                                albumContentInfo.ImageUrl = smallUrl;
                                albumContentInfo.LargeImageUrl = largeUrl;
                                albumContentInfo.Title = title;

                                DataProviderWX.AlbumContentDAO.Insert(albumContentInfo);
                            }
                        }
                    }
                }

                if (albumContentIDList.Count > 0)
                {
                    DataProviderWX.AlbumContentDAO.Delete(PublishmentSystemId, albumContentIDList);
                }

                PageUtils.CloseModalPage(Page);
            }
        }
    }
}
