using System.Text;
using System.Threading.Tasks;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Ajax;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.Enumerations;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin.Impl;

namespace SiteServer.BackgroundPages.Core
{
    public static class WebUtils
    {
        public static string GetContentTitle(Site site, Content content, string pageUrl)
        {
            string url;
            var title = ContentUtility.FormatTitle(content.Get<string>(ContentAttribute.GetFormatStringAttributeName(ContentAttribute.Title)), content.Title);

            var displayString = content.Color ? $"<span style='color:#ff0000;text-decoration:none' title='醒目'>{title}</span>" : title;

            if (content.Checked && content.ChannelId > 0)
            {
                url =
                    $"<a href='{PageUtils.GetRedirectUrlToContent(site.Id, content.ChannelId, content.Id)}' target='blank'>{displayString}</a>";
            }
            else
            {
                url =
                    $@"<a href=""javascript:;"" onclick=""{ModalContentView.GetOpenWindowString(site.Id, content.ChannelId, content.Id, pageUrl)}"">{displayString}</a>";
            }

            var image = string.Empty;
            if (content.Recommend)
            {
                image += "&nbsp;<img src='../pic/icon/recommend.gif' title='推荐' align='absmiddle' border=0 />";
            }
            if (content.Hot)
            {
                image += "&nbsp;<img src='../pic/icon/hot.gif' title='热点' align='absmiddle' border=0 />";
            }
            if (content.Top)
            {
                image += "&nbsp;<img src='../pic/icon/top.gif' title='置顶' align='absmiddle' border=0 />";
            }
            if (content.ReferenceId > 0)
            {
                if (content.Get<string>(ContentAttribute.TranslateContentType) == ETranslateContentType.ReferenceContent.ToString())
                {
                    image += "&nbsp;<img src='../pic/icon/reference.png' title='引用内容' align='absmiddle' border=0 />(引用内容)";
                }
                else if (content.Get<string>(ContentAttribute.TranslateContentType) != ETranslateContentType.ReferenceContent.ToString())
                {
                    image += "&nbsp;<img src='../pic/icon/reference.png' title='引用地址' align='absmiddle' border=0 />(引用地址)";
                }
            }
            if (!string.IsNullOrEmpty(content.Get<string>(ContentAttribute.LinkUrl)))
            {
                image += "&nbsp;<img src='../pic/icon/link.png' title='外部链接' align='absmiddle' border=0 />";
            }
            if (!string.IsNullOrEmpty(content.Get<string>(ContentAttribute.ImageUrl)))
            {
                var imageUrl = PageUtility.ParseNavigationUrl(site, content.Get<string>(ContentAttribute.ImageUrl), true);
                var openWindowString = ModalMessage.GetOpenWindowString(site.Id, "预览图片", $"<img src='{imageUrl}' />", 500, 500);
                image +=
                    $@"&nbsp;<a href=""javascript:;"" onclick=""{openWindowString}""><img src='../assets/icons/img.gif' title='预览图片' align='absmiddle' border=0 /></a>";
            }
            if (!string.IsNullOrEmpty(content.Get<string>(ContentAttribute.VideoUrl)))
            {
                var openWindowString = ModalMessage.GetOpenWindowStringToPreviewVideoByUrl(site.Id, content.Get<string>(ContentAttribute.VideoUrl));
                image +=
                    $@"&nbsp;<a href=""javascript:;"" onclick=""{openWindowString}""><img src='../pic/icon/video.png' title='预览视频' align='absmiddle' border=0 /></a>";
            }
            if (!string.IsNullOrEmpty(content.Get<string>(ContentAttribute.FileUrl)))
            {
                image += "&nbsp;<img src='../pic/icon/attachment.gif' title='附件' align='absmiddle' border=0 />";
            }
            return url + image;
        }

        public static string GetContentAddUploadWordUrl(int siteId, Channel node, bool isFirstLineTitle, bool isFirstLineRemove, bool isClearFormat, bool isFirstLineIndent, bool isClearFontSize, bool isClearFontFamily, bool isClearImages, int contentLevel, string fileName, string returnUrl)
        {
            return
                $"{PageContentAdd.GetRedirectUrlOfAdd(siteId, node.Id, returnUrl)}&isUploadWord=True&isFirstLineTitle={isFirstLineTitle}&isFirstLineRemove={isFirstLineRemove}&isClearFormat={isClearFormat}&isFirstLineIndent={isFirstLineIndent}&isClearFontSize={isClearFontSize}&isClearFontFamily={isClearFontFamily}&isClearImages={isClearImages}&contentLevel={contentLevel}&fileName={fileName}";
        }

        public static string GetContentAddAddUrl(int siteId, int channelId, string returnUrl)
        {
            return PageContentAdd.GetRedirectUrlOfAdd(siteId, channelId, returnUrl);
        }

        public static string GetContentAddEditUrl(int siteId, int channelId, int id, string returnUrl)
        {
            return PageContentAdd.GetRedirectUrlOfEdit(siteId, channelId, id, returnUrl);
        }

        public static string GetTextEditorCommands(Site site, string attributeName)
        {
            return $@"
<script type=""text/javascript"">
function getWordSpliter(){{
    var pureText = {UEditorUtils.GetPureTextScript(attributeName)}
	$.post('{AjaxCmsService.GetWordSpliterUrl(site.Id)}&r=' + Math.random(), {{content:pureText}}, function(data) {{
		if(data !=''){{
            $('.nav-pills').children('li').eq(1).find('a').click();
			$('#TbTags').val(data).focus();
		}}else{{
            {AlertUtils.Error("提取关键字", "对不起，内容不足，无法提取关键字")}
        }}
	}});	
}}
</script>
<div class=""btn-group btn-group-sm"">
    <button class=""btn"" onclick=""{ModalTextEditorImportWord.GetOpenWindowString(site.Id, attributeName)}"">导入Word</button>
    <button class=""btn"" onclick=""{ModalTextEditorInsertImage.GetOpenWindowString(site.Id, attributeName)}"">插入图片</button>
    <button class=""btn"" onclick=""{ModalTextEditorInsertVideo.GetOpenWindowString(site.Id, attributeName)}"">插入视频</button>
    <button class=""btn"" onclick=""{ModalTextEditorInsertAudio.GetOpenWindowString(site.Id, attributeName)}"">插入音频</button>
    <button class=""btn"" onclick=""getWordSpliter();return false;"">提取关键字</button>
</div>
";
        }

        public static string GetImageUrlButtonGroupHtml(Site site, string attributeName)
        {
            return $@"
<div class=""btn-group btn-group-sm"">
    <button class=""btn"" onclick=""{ModalUploadImage.GetOpenWindowString(site.Id, attributeName)}"">
        上传
    </button>
    <button class=""btn"" onclick=""{ModalSelectImage.GetOpenWindowString(site, attributeName)}"">
        选择
    </button>
    <button class=""btn"" onclick=""{ModalCuttingImage.GetOpenWindowStringWithTextBox(site.Id, attributeName)}"">
        裁切
    </button>
    <button class=""btn"" onclick=""{ModalMessage.GetOpenWindowStringToPreviewImage(site.Id, attributeName)}"">
        预览
    </button>
</div>
";
        }
    }
}
