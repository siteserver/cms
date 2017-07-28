using System;
using System.Collections;
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
        public Literal ltlScript;

        private string imageUrlCollection;
        private string largeImageUrlCollection;
        private List<string> imageUrlArrayList=new List<string>();
        private List<string> largeImageUrlArrayList =new List<string>();

        public static string GetOpenWindowStringToAdd(int publishmentSystemId, string imageUrlCollection, string largeImageUrlCollection)
        {
            return PageUtils.GetOpenWindowString("上传照片",
                PageUtils.GetWeiXinUrl(nameof(ModalAppointmentItemPhotoUpload), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
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

            imageUrlCollection = Body.GetQueryString("imageUrlCollection");
            largeImageUrlCollection = Body.GetQueryString("largeImageUrlCollection");

            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(imageUrlCollection))
                { 
                    imageUrlArrayList = TranslateUtils.StringCollectionToStringList(imageUrlCollection);
                    largeImageUrlArrayList = TranslateUtils.StringCollectionToStringList(largeImageUrlCollection);
                }
                
                var index = -1;
                var scriptBuilder = new StringBuilder();
                 
                foreach (string imageUrl in imageUrlArrayList)
                {
                    index++;
                    scriptBuilder.AppendFormat(@"
add_form({0}, '{1}', '{2}', '{3}');
", index + 1, StringUtils.ToJsString(PageUtility.ParseNavigationUrl(PublishmentSystemInfo, imageUrl)), StringUtils.ToJsString(imageUrl), StringUtils.ToJsString(largeImageUrlArrayList[index].ToString()));
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
                var photo_Count = TranslateUtils.ToInt(Request.Form["Photo_Count"]);
                if (photo_Count > 0)
                {
                    for (var index = 1; index <= photo_Count; index++)
                    {
                        var id = TranslateUtils.ToInt(Request.Form["ID_" + index]);
                        var smallUrl = Request.Form["SmallUrl_" + index];
                        var largeUrl = Request.Form["LargeUrl_" + index];

                        if (!string.IsNullOrEmpty(smallUrl) && !string.IsNullOrEmpty(largeUrl))
                        {
                            imageUrlArrayList.Add(smallUrl);
                            largeImageUrlArrayList.Add(largeUrl);
                        }
                    }
                }

                if (imageUrlArrayList != null && imageUrlArrayList.Count > 0)
                {
                    imageUrlCollection = TranslateUtils.ToSqlInStringWithoutQuote(imageUrlArrayList);
                    largeImageUrlCollection = TranslateUtils.ToSqlInStringWithoutQuote(largeImageUrlArrayList);
                }

                string scripts =
                    $"window.parent.addImage('{imageUrlCollection}', '{largeImageUrlCollection}');";
                PageUtils.CloseModalPageWithoutRefresh(Page, scripts);
            }
        }
    }
}
