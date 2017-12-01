using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Ajax;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class ModalAppointmentItemPhotoUpload : BasePageCms
    {
        public Literal LtlScript;

        private string _imageUrlCollection;
        private string _largeImageUrlCollection;
        private List<string> _imageUrlArrayList=new List<string>();
        private List<string> _largeImageUrlArrayList =new List<string>();

        public static string GetOpenWindowStringToAdd(int publishmentSystemId, string imageUrlCollection, string largeImageUrlCollection)
        {
            return PageUtils.GetOpenWindowString("上传照片",
                PageUtils.GetWeiXinUrl(nameof(ModalAppointmentItemPhotoUpload), new NameValueCollection
                {
                    {"PublishmentSystemId", publishmentSystemId.ToString()},
                    {"imageUrlCollection", imageUrlCollection},
                    {"largeImageUrlCollection", largeImageUrlCollection}
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

            _imageUrlCollection = Body.GetQueryString("imageUrlCollection");
            _largeImageUrlCollection = Body.GetQueryString("largeImageUrlCollection");

            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(_imageUrlCollection))
                { 
                    _imageUrlArrayList = TranslateUtils.StringCollectionToStringList(_imageUrlCollection);
                    _largeImageUrlArrayList = TranslateUtils.StringCollectionToStringList(_largeImageUrlCollection);
                }
                
                var index = -1;
                var scriptBuilder = new StringBuilder();
                 
                foreach (string imageUrl in _imageUrlArrayList)
                {
                    index++;
                    scriptBuilder.AppendFormat(@"
add_form({0}, '{1}', '{2}', '{3}');
", index + 1, StringUtils.ToJsString(PageUtility.ParseNavigationUrl(PublishmentSystemInfo, imageUrl)), StringUtils.ToJsString(imageUrl), StringUtils.ToJsString(_largeImageUrlArrayList[index]));
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
                var photoCount = TranslateUtils.ToInt(Request.Form["Photo_Count"]);
                if (photoCount > 0)
                {
                    for (var index = 1; index <= photoCount; index++)
                    {
                        var id = TranslateUtils.ToInt(Request.Form["ID_" + index]);
                        var smallUrl = Request.Form["SmallUrl_" + index];
                        var largeUrl = Request.Form["LargeUrl_" + index];

                        if (!string.IsNullOrEmpty(smallUrl) && !string.IsNullOrEmpty(largeUrl))
                        {
                            _imageUrlArrayList.Add(smallUrl);
                            _largeImageUrlArrayList.Add(largeUrl);
                        }
                    }
                }

                if (_imageUrlArrayList != null && _imageUrlArrayList.Count > 0)
                {
                    _imageUrlCollection = TranslateUtils.ToSqlInStringWithoutQuote(_imageUrlArrayList);
                    _largeImageUrlCollection = TranslateUtils.ToSqlInStringWithoutQuote(_largeImageUrlArrayList);
                }

                string scripts =
                    $"window.parent.addImage('{_imageUrlCollection}', '{_largeImageUrlCollection}');";
                PageUtils.CloseModalPageWithoutRefresh(Page, scripts);
            }
        }
    }
}
