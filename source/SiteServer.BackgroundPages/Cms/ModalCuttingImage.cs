using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Images;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
    //http://www.codeproject.com/Articles/83225/A-Simple-JPEG-Encoder-in-C
	public class ModalCuttingImage : BasePageCms
    {
        public Literal ltlScript;

		private string _textBoxClientId;
        private string _imageUrl;

        protected override bool IsSinglePage => true;

	    public static string GetOpenWindowStringWithTextBox(int publishmentSystemId, string textBoxClientId)
        {
            return PageUtils.GetOpenWindowString("裁切图片", PageUtils.GetCmsUrl(nameof(ModalCuttingImage), new NameValueCollection
            {
                {"publishmentSystemID", publishmentSystemId.ToString()},
                {"textBoxClientID", textBoxClientId}
            }));
        }

        public static string GetOpenWindowStringToImageUrl(int publishmentSystemId, string imageUrl)
        {
            return PageUtils.GetOpenWindowString("裁切图片", PageUtils.GetCmsUrl(nameof(ModalCuttingImage), new NameValueCollection
            {
                {"publishmentSystemID", publishmentSystemId.ToString()},
                {"imageUrl", imageUrl}
            }));
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");
            _textBoxClientId = Body.GetQueryString("TextBoxClientID");
            _imageUrl = Body.GetQueryString("imageUrl");

            if (!IsPostBack)
            {
                var virtualUrl = string.Empty;

                if (!string.IsNullOrEmpty(_textBoxClientId))
                {
                    virtualUrl = $"window.parent.document.getElementById('{_textBoxClientId}').value";
                }
                else if (!string.IsNullOrEmpty(_imageUrl))
                {
                    virtualUrl = "'" + _imageUrl + "'";
                }

                ltlScript.Text = $@"
var rootUrl = '{PageUtils.GetRootUrl(string.Empty)}';
var publishmentSystemUrl = '{PageUtility.GetPublishmentSystemUrl(PublishmentSystemInfo, string.Empty, true)}';
var virtualUrl = {virtualUrl};
var imageUrl = virtualUrl;
if(imageUrl && imageUrl.search(/\.bmp|\.jpg|\.jpeg|\.gif|\.png$/i) != -1){{
	if (imageUrl.charAt(0) == '~'){{
		imageUrl = imageUrl.replace('~', rootUrl);
	}}else if (imageUrl.charAt(0) == '@'){{
		imageUrl = imageUrl.replace('@', publishmentSystemUrl);
	}}
	if(imageUrl.substr(0,2)=='//'){{
		imageUrl = imageUrl.replace('//', '/');
	}}
}}
";
            }
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            try
            {
                var rotate = TranslateUtils.ToIntWithNagetive(Request.Form["rotate"]);
                rotate = rotate % 4;
                var flip = Request.Form["flip"];
                var fileUrl = Request.Form["fileUrl"];
                if (!string.IsNullOrEmpty(fileUrl))
                {
                    var filePath = PathUtility.MapPath(PublishmentSystemInfo, fileUrl);
                    if (FileUtils.IsFileExists(filePath))
                    {
                        var destImagePath = filePath.Substring(0, filePath.LastIndexOf('.')) + "_c" + filePath.Substring(filePath.LastIndexOf('.'));

                        if (rotate == 0 && string.IsNullOrEmpty(flip))
                        {
                            var x1 = TranslateUtils.ToIntWithNagetive(Request.Form["x1"]);
                            var y1 = TranslateUtils.ToIntWithNagetive(Request.Form["y1"]);
                            var w = TranslateUtils.ToIntWithNagetive(Request.Form["w"]);
                            var h = TranslateUtils.ToIntWithNagetive(Request.Form["h"]);

                            if (w > 0 && h > 0)
                            {
                                ImageUtils.CropImage(filePath, destImagePath, x1, y1, w, h);
                            }
                        }
                        else if (rotate != 0)
                        {
                            if (rotate == 1 || rotate == -3)
                            {
                                ImageUtils.RotateFlipImage(filePath, destImagePath, System.Drawing.RotateFlipType.Rotate90FlipNone);
                            }
                            else if (rotate == 2 || rotate == -2)
                            {
                                ImageUtils.RotateFlipImage(filePath, destImagePath, System.Drawing.RotateFlipType.Rotate180FlipNone);
                            }
                            else if (rotate == 3 || rotate == -1)
                            {
                                ImageUtils.RotateFlipImage(filePath, destImagePath, System.Drawing.RotateFlipType.Rotate270FlipNone);
                            }
                        }
                        else if (!string.IsNullOrEmpty(flip))
                        {
                            if (flip == "H")
                            {
                                ImageUtils.RotateFlipImage(filePath, destImagePath, System.Drawing.RotateFlipType.RotateNoneFlipX);
                            }
                            else if (flip == "V")
                            {
                                ImageUtils.RotateFlipImage(filePath, destImagePath, System.Drawing.RotateFlipType.RotateNoneFlipY);
                            }
                        }

                        var destUrl = PageUtility.GetVirtualUrl(PublishmentSystemInfo, PageUtility.GetPublishmentSystemUrlByPhysicalPath(PublishmentSystemInfo, destImagePath));

                        if (!string.IsNullOrEmpty(_textBoxClientId))
                        {
                            ltlScript.Text = $@"
if (parent.document.getElementById('{_textBoxClientId}'))
{{
    parent.document.getElementById('{_textBoxClientId}').value = '{destUrl}';
}}
";
                        }
                        else
                        {
                            FileUtils.CopyFile(destImagePath, filePath, true);
                        }

                        ltlScript.Text += PageUtils.HidePopWin;
                    }
                }
            }
            catch (Exception ex)
            {
                FailMessage(ex, ex.Message);
            }
		}
	}
}
