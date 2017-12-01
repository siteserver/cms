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
        public Literal LtlScript;

        private int _albumId;
        private int _parentId;

        public static string GetOpenWindowStringToAdd(int publishmentSystemId, int albumId, int parentId)
        {
            return PageUtils.GetOpenWindowString("上传照片",
                PageUtils.GetWeiXinUrl(nameof(ModalAlbumContentPhotoUpload), new NameValueCollection
                {
                    {"PublishmentSystemId", publishmentSystemId.ToString()},
                    {"albumID", albumId.ToString()},
                    {"parentID", parentId.ToString()}
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

            _albumId = Body.GetQueryInt("albumID");
            _parentId = Body.GetQueryInt("parentID");

            if (!IsPostBack)
            {
                var list = new List<AlbumContentInfo>();
                if (_parentId > 0)
                {
                    list = DataProviderWx.AlbumContentDao.GetAlbumContentInfoList(PublishmentSystemId, _albumId, _parentId);
                }

                var scriptBuilder = new StringBuilder();

                foreach (var albumContentInfo in list)
                {
                    scriptBuilder.AppendFormat(@"
add_form({0}, '{1}', '{2}', '{3}', '{4}');
", albumContentInfo.Id, StringUtils.ToJsString(PageUtility.ParseNavigationUrl(PublishmentSystemInfo, albumContentInfo.ImageUrl)), StringUtils.ToJsString(albumContentInfo.ImageUrl), StringUtils.ToJsString(albumContentInfo.LargeImageUrl), albumContentInfo.Title);
                }

                LtlScript.Text = $@"
$(document).ready(function(){{
	{scriptBuilder}
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
                var albumContentIdList = new List<int>();
                if (_parentId > 0)
                {
                    albumContentIdList = DataProviderWx.AlbumContentDao.GetAlbumContentIdList(PublishmentSystemId, _albumId, _parentId);
                }
                var photoCount = TranslateUtils.ToInt(Request.Form["Photo_Count"]);
                if (photoCount > 0)
                {
                    for (var index = 1; index <= photoCount; index++)
                    {
                        var id = TranslateUtils.ToInt(Request.Form["ID_" + index]);
                        var smallUrl = Request.Form["SmallUrl_" + index];
                        var largeUrl = Request.Form["LargeUrl_" + index];
                        var title = Request.Form["imgTitle_" + index];

                        if (!string.IsNullOrEmpty(smallUrl) && !string.IsNullOrEmpty(largeUrl))
                        {
                            if (id > 0)
                            {
                                var albumContentInfo = DataProviderWx.AlbumContentDao.GetAlbumContentInfo(id);
                                if (albumContentInfo != null)
                                {
                                    albumContentInfo.ImageUrl = smallUrl;
                                    albumContentInfo.LargeImageUrl = largeUrl;
                                    albumContentInfo.Title = title;

                                    DataProviderWx.AlbumContentDao.Update(albumContentInfo);
                                }
                                albumContentIdList.Remove(id);
                            }
                            else
                            {
                                var albumContentInfo = new AlbumContentInfo();
                                albumContentInfo.PublishmentSystemId = PublishmentSystemId;
                                albumContentInfo.AlbumId = _albumId;
                                albumContentInfo.ParentId = _parentId;
                                albumContentInfo.ImageUrl = smallUrl;
                                albumContentInfo.LargeImageUrl = largeUrl;
                                albumContentInfo.Title = title;

                                DataProviderWx.AlbumContentDao.Insert(albumContentInfo);
                            }
                        }
                    }
                }

                if (albumContentIdList.Count > 0)
                {
                    DataProviderWx.AlbumContentDao.Delete(PublishmentSystemId, albumContentIdList);
                }

                PageUtils.CloseModalPage(Page);
            }
        }
    }
}
