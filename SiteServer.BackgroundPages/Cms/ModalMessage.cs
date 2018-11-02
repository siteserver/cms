using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalMessage : BasePageCms
    {
        public Literal LtlHtml;

        private string _type;
        public const string TypePreviewImage = "PreviewImage";
        public const string TypePreviewVideo = "PreviewVideo";
        public const string TypePreviewVideoByUrl = "PreviewVideoByUrl";

        protected override bool IsSinglePage => true;

        public static string GetRedirectUrl(int siteId, string html)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(ModalMessage), new NameValueCollection
            {
                {"html", TranslateUtils.EncryptStringBySecretKey(html)}
            });
        }

        public static string GetOpenWindowString(int siteId, string title, string html, int width, int height)
        {
            return LayerUtils.GetOpenScript(title, PageUtils.GetCmsUrl(siteId, nameof(ModalMessage), new NameValueCollection
            {
                {"html", TranslateUtils.EncryptStringBySecretKey(html)}
            }), width, height);
        }

        public static string GetOpenWindowStringToPreviewImage(int siteId, string textBoxClientId)
        {
            return LayerUtils.GetOpenScript("预览图片", PageUtils.GetCmsUrl(siteId, nameof(ModalMessage), new NameValueCollection
            {
                {"type", TypePreviewImage},
                {"textBoxClientID", textBoxClientId}
            }), 500, 500);
        }

        public static string GetOpenWindowStringToPreviewVideo(int siteId, string textBoxClientId)
        {
            return LayerUtils.GetOpenScript("预览视频", PageUtils.GetCmsUrl(siteId, nameof(ModalMessage), new NameValueCollection
            {
                {"type", TypePreviewVideo},
                {"textBoxClientID", textBoxClientId}
            }), 500, 500);
        }

        public static string GetOpenWindowStringToPreviewVideoByUrl(int siteId, string videoUrl)
        {
            return LayerUtils.GetOpenScript("预览视频", PageUtils.GetCmsUrl(siteId, nameof(ModalMessage), new NameValueCollection
            {
                {"type", TypePreviewVideoByUrl},
                {"videoUrl", videoUrl}
            }), 500, 500);
        }

        public static string GetRedirectStringToPreviewVideoByUrl(int siteId, string videoUrl)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(ModalMessage), new NameValueCollection
            {
                {"type", TypePreviewVideoByUrl},
                {"videoUrl", videoUrl}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _type = Request.QueryString["type"];

            if (IsPostBack) return;

            if (StringUtils.EqualsIgnoreCase(_type, TypePreviewImage))
            {
                var siteId = AuthRequest.GetQueryInt("siteID");
                var siteInfo = SiteManager.GetSiteInfo(siteId);
                var textBoxClientId = AuthRequest.GetQueryString("textBoxClientID");
                LtlHtml.Text = $@"
<span id=""previewImage""></span>
<script>
var rootUrl = '{PageUtils.GetRootUrl(string.Empty)}';
var siteUrl = '{PageUtils.ParseNavigationUrl($"~/{siteInfo.SiteDir}")}';
var imageUrl = window.parent.document.getElementById('{textBoxClientId}').value;
if(imageUrl && imageUrl.search(/\.bmp|\.jpg|\.jpeg|\.gif|\.png|\.webp$/i) != -1){{
	if (imageUrl.charAt(0) == '~'){{
		imageUrl = imageUrl.replace('~', rootUrl);
	}}else if (imageUrl.charAt(0) == '@'){{
		imageUrl = imageUrl.replace('@', siteUrl);
	}}
	if(imageUrl.substr(0,2)=='//'){{
		imageUrl = imageUrl.replace('//', '/');
	}}
    $('#previewImage').html('<img src=""' + imageUrl + '"" class=""img-polaroid"" />');
}}
</script>
";
            }
            else if (StringUtils.EqualsIgnoreCase(_type, TypePreviewVideo))
            {
                var siteId = AuthRequest.GetQueryInt("siteID");
                var siteInfo = SiteManager.GetSiteInfo(siteId);
                var textBoxClientId = AuthRequest.GetQueryString("textBoxClientID");

                LtlHtml.Text = $@"
<span id=""previewVideo""></span>
<script>
var rootUrl = '{PageUtils.GetRootUrl(string.Empty)}';
var siteUrl = '{PageUtils.ParseNavigationUrl($"~/{siteInfo.SiteDir}")}';
var videoUrl = window.parent.document.getElementById('{textBoxClientId}').value;
if (videoUrl.charAt(0) == '~'){{
	videoUrl = videoUrl.replace('~', rootUrl);
}}else if (videoUrl.charAt(0) == '@'){{
	videoUrl = videoUrl.replace('@', siteUrl);
}}
if(videoUrl.substr(0,2)=='//'){{
	videoUrl = videoUrl.replace('//', '/');
}}
if (videoUrl){{
    $('#previewVideo').html('<embed src=""../assets/player.swf"" allowfullscreen=""true"" flashvars=""controlbar=over&autostart=true&file='+videoUrl+'"" width=""{450}"" height=""{350}""/>');
}}
</script>
";
            }
            else if (StringUtils.EqualsIgnoreCase(_type, TypePreviewVideoByUrl))
            {
                var siteId = AuthRequest.GetQueryInt("siteID");
                var siteInfo = SiteManager.GetSiteInfo(siteId);
                var videoUrl = AuthRequest.GetQueryString("videoUrl");

                LtlHtml.Text = $@"
<embed src=""../assets/player.swf"" allowfullscreen=""true"" flashvars=""controlbar=over&autostart=true&file={PageUtility
                    .ParseNavigationUrl(siteInfo, videoUrl, true)}"" width=""{450}"" height=""{350}""/>
";
            }
            else
            {
                LtlHtml.Text = TranslateUtils.DecryptStringBySecretKey(Request.QueryString["html"]);
            }
        }
    }
}
