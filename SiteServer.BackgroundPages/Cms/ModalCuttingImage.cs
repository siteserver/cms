using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.Utils.Images;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
    //http://www.codeproject.com/Articles/83225/A-Simple-JPEG-Encoder-in-C
	public class ModalCuttingImage : BasePageCms
    {
        public Literal LtlScript;

		private string _textBoxClientId;
        private string _imageUrl;

        protected override bool IsSinglePage => true;

	    public static string GetOpenWindowStringWithTextBox(int siteId, string textBoxClientId)
        {
            return LayerUtils.GetOpenScript("裁切图片", PageUtils.GetCmsUrl(siteId, nameof(ModalCuttingImage), new NameValueCollection
            {
                {"textBoxClientID", textBoxClientId}
            }));
        }

        public static string GetOpenWindowStringToImageUrl(int siteId, string imageUrl)
        {
            return LayerUtils.GetOpenScript("裁切图片", PageUtils.GetCmsUrl(siteId, nameof(ModalCuttingImage), new NameValueCollection
            {
                {"imageUrl", imageUrl}
            }));
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId");
            _textBoxClientId = AuthRequest.GetQueryString("TextBoxClientID");
            _imageUrl = AuthRequest.GetQueryString("imageUrl");

            if (IsPostBack) return;

            var virtualUrl = string.Empty;

            if (!string.IsNullOrEmpty(_textBoxClientId))
            {
                virtualUrl = $"window.parent.document.getElementById('{_textBoxClientId}').value";
            }
            else if (!string.IsNullOrEmpty(_imageUrl))
            {
                virtualUrl = "'" + _imageUrl + "'";
            }

            LtlScript.Text = $@"
<script type=""text/javascript"">
    var rootUrl = '{PageUtils.GetRootUrl(string.Empty)}';
    var siteUrl = '{PageUtils.ParseNavigationUrl($"~/{SiteInfo.SiteDir}")}';
    var virtualUrl = {virtualUrl};
    var imageUrl = virtualUrl;
    if(imageUrl && imageUrl.search(/\.bmp|\.jpg|\.jpeg|\.gif|\.png|\.webp$/i) != -1){{
	    if (imageUrl.charAt(0) == '~'){{
		    imageUrl = imageUrl.replace('~', rootUrl);
	    }}else if (imageUrl.charAt(0) == '@'){{
		    imageUrl = imageUrl.replace('@', siteUrl);
	    }}
	    if(imageUrl.substr(0,2)=='//'){{
		    imageUrl = imageUrl.replace('//', '/');
	    }}
    }}
</script>
";
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            try
            {
                var rotate = TranslateUtils.ToIntWithNagetive(Request.Form["rotate"]);
                rotate = rotate % 4;
                var flip = Request.Form["flip"];
                var fileUrl = Request.Form["fileUrl"];
                if (string.IsNullOrEmpty(fileUrl)) return;

                var filePath = PathUtility.MapPath(SiteInfo, fileUrl);
                if (!FileUtils.IsFileExists(filePath)) return;

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

                var destUrl = PageUtility.GetVirtualUrl(SiteInfo, PageUtility.GetSiteUrlByPhysicalPath(SiteInfo, destImagePath, true));

                if (!string.IsNullOrEmpty(_textBoxClientId))
                {
                    LtlScript.Text = $@"
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

                LtlScript.Text += LayerUtils.CloseScript;

                LtlScript.Text = $@"
<script type=""text/javascript"">
    {LtlScript.Text}
</script>
";
            }
            catch (Exception ex)
            {
                FailMessage(ex, ex.Message);
            }
		}
	}
}
