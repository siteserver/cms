using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Ajax;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageContentPhotoUpload : BasePageCms
    {
        public Literal LtlScript;

        private string _returnUrl = string.Empty;
        private int _nodeId;
        private int _contentId;

        public string GetContentPhotoUploadMultipleUrl()
        {
            return AjaxUploadService.GetContentPhotoUploadMultipleUrl(PublishmentSystemId);
        }

        public string GetContentPhotoUploadSingleUrl()
        {
            return AjaxUploadService.GetContentPhotoUploadSingleUrl(PublishmentSystemId);
        }

        public static string GetRedirectUrl(int publishmentSystemId, int nodeId, int contentId, string returnUrl)
        {
            return PageUtils.GetCmsUrl(nameof(PageContentPhotoUpload), new NameValueCollection
            {
                {"publishmentSystemID", publishmentSystemId.ToString()},
                {"nodeID", nodeId.ToString()},
                {"contentID", contentId.ToString()},
                {"returnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID", "ReturnUrl");
            _returnUrl = StringUtils.ValueFromUrl(Body.GetQueryString("ReturnUrl"));
            _nodeId = Body.GetQueryInt("nodeID");
            _contentId = Body.GetQueryInt("ContentID");

            if (!IsPostBack)
            {
                var photoInfoList = new List<PhotoInfo>();
                if (_contentId > 0)
                {
                    photoInfoList = DataProvider.PhotoDao.GetPhotoInfoList(PublishmentSystemId, _contentId);
                }

                var scriptBuilder = new StringBuilder();

                foreach (var photoInfo in photoInfoList)
                {
                    scriptBuilder.Append($@"
add_form({photoInfo.ID}, '{StringUtils.ToJsString(PageUtility.ParseNavigationUrl(PublishmentSystemInfo, photoInfo.SmallUrl))}', '{StringUtils.ToJsString(photoInfo.SmallUrl)}', '{StringUtils.ToJsString(photoInfo.MiddleUrl)}', '{StringUtils.ToJsString(photoInfo.LargeUrl)}', '{StringUtils.ToJsString(photoInfo.Description)}');
");
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
            return
                $@"width=""{PublishmentSystemInfo.Additional.PhotoSmallWidth}"" height=""{PublishmentSystemInfo.Additional.PhotoSmallHeight}""";
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            var contentIdList = new List<int>();
            if (_contentId > 0)
            {
                contentIdList = DataProvider.PhotoDao.GetPhotoContentIdList(PublishmentSystemId, _contentId);
            }
            var photos = TranslateUtils.ToInt(Request.Form["Photo_Count"]);
            var count = 0;
            if (photos > 0)
            {
                for (var index = 1; index <= photos; index++)
                {
                    var id = TranslateUtils.ToInt(Request.Form["ID_" + index]);
                    var smallUrl = Request.Form["SmallUrl_" + index];
                    var middleUrl = Request.Form["MiddleUrl_" + index];
                    var largeUrl = Request.Form["LargeUrl_" + index];
                    var description = Request.Form["Description_" + index];

                    if (!string.IsNullOrEmpty(smallUrl) && !string.IsNullOrEmpty(middleUrl) && !string.IsNullOrEmpty(largeUrl))
                    {
                        count++;
                        if (id > 0)
                        {
                            var photoInfo = DataProvider.PhotoDao.GetPhotoInfo(id);
                            if (photoInfo != null)
                            {
                                photoInfo.SmallUrl = smallUrl;
                                photoInfo.MiddleUrl = middleUrl;
                                photoInfo.LargeUrl = largeUrl;
                                photoInfo.Description = description;

                                DataProvider.PhotoDao.Update(photoInfo);
                            }
                            contentIdList.Remove(id);
                        }
                        else
                        {
                            var photoInfo = new PhotoInfo(0, PublishmentSystemId, _contentId, smallUrl, middleUrl, largeUrl, 0, description);

                            DataProvider.PhotoDao.Insert(photoInfo);
                        }
                    }
                }
            }

            if (contentIdList.Count > 0)
            {
                DataProvider.PhotoDao.Delete(contentIdList);
            }

            var tableName = NodeManager.GetTableName(PublishmentSystemInfo, _nodeId);
            BaiRongDataProvider.ContentDao.UpdatePhotos(tableName, _contentId, count);

            PageUtils.Redirect(_returnUrl);
        }

        public void Return_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(_returnUrl);
        }
    }
}
