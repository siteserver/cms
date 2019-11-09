using System.Text;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Ajax;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.Model.Db;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Plugin.Impl;

namespace SiteServer.BackgroundPages.Core
{
    public static class WebUtils
    {
        public static string GetContentTitle(Site site, ContentInfo contentInfo, string pageUrl)
        {
            string url;
            var title = ContentUtility.FormatTitle(contentInfo.GetString(ContentAttribute.GetFormatStringAttributeName(ContentAttribute.Title)), contentInfo.Title);

            var displayString = contentInfo.IsColor ? $"<span style='color:#ff0000;text-decoration:none' title='醒目'>{title}</span>" : title;

            if (contentInfo.IsChecked && contentInfo.ChannelId > 0)
            {
                url =
                    $"<a href='{PageUtils.GetRedirectUrlToContent(site.Id, contentInfo.ChannelId, contentInfo.Id)}' target='blank'>{displayString}</a>";
            }
            else
            {
                url =
                    $@"<a href=""javascript:;"" onclick=""{ModalContentView.GetOpenWindowString(site.Id, contentInfo.ChannelId, contentInfo.Id, pageUrl)}"">{displayString}</a>";
            }

            var image = string.Empty;
            if (contentInfo.IsRecommend)
            {
                image += "&nbsp;<img src='../pic/icon/recommend.gif' title='推荐' align='absmiddle' border=0 />";
            }
            if (contentInfo.IsHot)
            {
                image += "&nbsp;<img src='../pic/icon/hot.gif' title='热点' align='absmiddle' border=0 />";
            }
            if (contentInfo.IsTop)
            {
                image += "&nbsp;<img src='../pic/icon/top.gif' title='置顶' align='absmiddle' border=0 />";
            }
            if (contentInfo.ReferenceId > 0)
            {
                if (contentInfo.GetString(ContentAttribute.TranslateContentType) == ETranslateContentType.ReferenceContent.ToString())
                {
                    image += "&nbsp;<img src='../pic/icon/reference.png' title='引用内容' align='absmiddle' border=0 />(引用内容)";
                }
                else if (contentInfo.GetString(ContentAttribute.TranslateContentType) != ETranslateContentType.ReferenceContent.ToString())
                {
                    image += "&nbsp;<img src='../pic/icon/reference.png' title='引用地址' align='absmiddle' border=0 />(引用地址)";
                }
            }
            if (!string.IsNullOrEmpty(contentInfo.GetString(ContentAttribute.LinkUrl)))
            {
                image += "&nbsp;<img src='../pic/icon/link.png' title='外部链接' align='absmiddle' border=0 />";
            }
            if (!string.IsNullOrEmpty(contentInfo.GetString(BackgroundContentAttribute.ImageUrl)))
            {
                var imageUrl = PageUtility.ParseNavigationUrl(site, contentInfo.GetString(BackgroundContentAttribute.ImageUrl), true);
                var openWindowString = ModalMessage.GetOpenWindowString(site.Id, "预览图片", $"<img src='{imageUrl}' />", 500, 500);
                image +=
                    $@"&nbsp;<a href=""javascript:;"" onclick=""{openWindowString}""><img src='../assets/icons/img.gif' title='预览图片' align='absmiddle' border=0 /></a>";
            }
            if (!string.IsNullOrEmpty(contentInfo.GetString(BackgroundContentAttribute.VideoUrl)))
            {
                var openWindowString = ModalMessage.GetOpenWindowStringToPreviewVideoByUrl(site.Id, contentInfo.GetString(BackgroundContentAttribute.VideoUrl));
                image +=
                    $@"&nbsp;<a href=""javascript:;"" onclick=""{openWindowString}""><img src='../pic/icon/video.png' title='预览视频' align='absmiddle' border=0 /></a>";
            }
            if (!string.IsNullOrEmpty(contentInfo.GetString(BackgroundContentAttribute.FileUrl)))
            {
                image += "&nbsp;<img src='../pic/icon/attachment.gif' title='附件' align='absmiddle' border=0 />";
            }
            return url + image;
        }

        public static string GetContentAddUploadWordUrl(int siteId, ChannelInfo nodeInfo, bool isFirstLineTitle, bool isFirstLineRemove, bool isClearFormat, bool isFirstLineIndent, bool isClearFontSize, bool isClearFontFamily, bool isClearImages, int contentLevel, string fileName, string returnUrl)
        {
            return
                $"{PageContentAdd.GetRedirectUrlOfAdd(siteId, nodeInfo.Id, returnUrl)}&isUploadWord=True&isFirstLineTitle={isFirstLineTitle}&isFirstLineRemove={isFirstLineRemove}&isClearFormat={isClearFormat}&isFirstLineIndent={isFirstLineIndent}&isClearFontSize={isClearFontSize}&isClearFontFamily={isClearFontFamily}&isClearImages={isClearImages}&contentLevel={contentLevel}&fileName={fileName}";
        }

        public static string GetContentAddAddUrl(int siteId, int channelId, string returnUrl)
        {
            return PageContentAdd.GetRedirectUrlOfAdd(siteId, channelId, returnUrl);
        }

        public static string GetContentAddEditUrl(int siteId, int channelId, int id, string returnUrl)
        {
            return PageContentAdd.GetRedirectUrlOfEdit(siteId, channelId, id, returnUrl);
        }

        public static string GetContentCommands(PermissionsImpl permissionsImpl, Site site, ChannelInfo channelInfo, string pageUrl)
        {
            var builder = new StringBuilder();

            if (permissionsImpl.HasChannelPermissions(site.Id, channelInfo.Id, ConfigManager.ChannelPermissions.ContentAdd) && channelInfo.Additional.IsContentAddable)
            {
                builder.Append($@"
<a href=""{GetContentAddAddUrl(site.Id, channelInfo.Id, pageUrl)}"" class=""btn btn-light text-secondary"">
    <i class=""ion-plus""></i>
    添加
</a>");

                builder.Append($@"
<a href=""javascript:;"" class=""btn btn-light text-secondary"" onclick=""{ModalUploadWord.GetOpenWindowString(site.Id, channelInfo.Id, StringUtils.ValueToUrl(pageUrl))}"">
    导入Word
</a>");
            }

            var adminId = permissionsImpl.GetAdminId(site.Id, channelInfo.Id);
            var count = ContentManager.GetCount(site, channelInfo, adminId);

            if (count > 0 && permissionsImpl.HasChannelPermissions(site.Id, channelInfo.Id, ConfigManager.ChannelPermissions.ContentDelete))
            {
                builder.Append($@"
<a href=""javascript:;"" class=""btn btn-light text-secondary"" onclick=""{PageContentDelete.GetRedirectClickStringForSingleChannel(site.Id, channelInfo.Id, false, pageUrl)}"">
    <i class=""ion-trash-a""></i>
    删 除
</a>");
            }

            if (count > 0)
            {
                if (permissionsImpl.HasChannelPermissions(site.Id, channelInfo.Id, ConfigManager.ChannelPermissions.ContentEdit))
                {
                    builder.Append($@"
<a href=""javascript:;"" class=""btn btn-light text-secondary"" onclick=""{ModalContentAttributes.GetOpenWindowString(site.Id, channelInfo.Id)}"">
    <i class=""ion-flag""></i>
    属性
</a>");
                    builder.Append($@"
<a href=""javascript:;"" class=""btn btn-light text-secondary"" onclick=""{ModalAddToGroup.GetOpenWindowStringToContent(site.Id, channelInfo.Id)}"">
    内容组
</a>");
                }
                if (permissionsImpl.HasChannelPermissions(site.Id, channelInfo.Id, ConfigManager.ChannelPermissions.ContentTranslate))
                {
                    var redirectUrl = PageContentTranslate.GetRedirectUrl(site.Id, channelInfo.Id, pageUrl);
                    var clickString = PageUtils.GetRedirectStringWithCheckBoxValue(redirectUrl, "contentIdCollection", "contentIdCollection", "请选择需要转移的内容！");
                    builder.Append($@"
<a href=""javascript:;"" class=""btn btn-light text-secondary"" onclick=""{clickString}"">
    转 移
</a>");
                }
                if (permissionsImpl.HasChannelPermissions(site.Id, channelInfo.Id, ConfigManager.ChannelPermissions.ContentEdit))
                {
                    builder.Append($@"
<a href=""javascript:;"" class=""btn btn-light text-secondary"" onclick=""{ModalContentTaxis.GetOpenWindowString(site.Id, channelInfo.Id, pageUrl)}"">
    排 序
</a>");
                }
                if (permissionsImpl.HasChannelPermissions(site.Id, channelInfo.Id, ConfigManager.ChannelPermissions.ContentCheck))
                {
                    builder.Append($@"
<a href=""javascript:;"" class=""btn btn-light text-secondary"" onclick=""{ModalContentCheck.GetOpenWindowString(site.Id, channelInfo.Id, pageUrl)}"">
    审 核
</a>");
                }
                if (permissionsImpl.HasSitePermissions(site.Id, ConfigManager.WebSitePermissions.Create) || permissionsImpl.HasChannelPermissions(site.Id, channelInfo.Id, ConfigManager.ChannelPermissions.CreatePage))
                {
                    builder.Append($@"
<a href=""javascript:;"" class=""btn btn-light text-secondary"" onclick=""{ModalProgressBar.GetOpenWindowStringWithCreateContentsOneByOne(site.Id, channelInfo.Id)}"">
    <i class=""ion-wand""></i>
    生 成
</a>");
                }
            }

            if (permissionsImpl.HasChannelPermissions(site.Id, channelInfo.Id, ConfigManager.ChannelPermissions.ChannelEdit))
            {
                builder.Append($@"
<a href=""javascript:;"" class=""btn btn-light text-secondary"" onclick=""{ModalSelectColumns.GetOpenWindowString(site.Id, channelInfo.Id)}"">
    <i class=""ion-ios-list-outline""></i>
    显示项
</a>");
            }

            if (count > 0)
            {
                builder.Append(@"
<a href=""javascript:;;"" class=""btn btn-light text-secondary text-secondary"" onClick=""$('#contentSearch').toggle(); return false"">
    <i class=""ion-search""></i>
    查找
</a>");
            }

            return builder.ToString();
        }

        public static string GetContentMoreCommands(PermissionsImpl permissionsImpl, Site site, ChannelInfo channelInfo, string pageUrl)
        {
            var builder = new StringBuilder();

            if (permissionsImpl.HasChannelPermissions(site.Id, channelInfo.Id, ConfigManager.ChannelPermissions.ContentAdd) && channelInfo.Additional.IsContentAddable)
            {
                builder.Append($@"
<a class=""dropdown-item"" href=""javascript:;"" onclick=""{ModalContentImport.GetOpenWindowString(site.Id, channelInfo.Id)}"">
    导 入
</a>");
            }

            var adminId = permissionsImpl.GetAdminId(site.Id, channelInfo.Id);
            var count = ContentManager.GetCount(site, channelInfo, adminId);

            if (count > 0)
            {
                builder.Append($@"
<a class=""dropdown-item"" href=""javascript:;"" onclick=""{ModalContentExport.GetOpenWindowString(site.Id, channelInfo.Id)}"">
    导 出
</a>");
                if (permissionsImpl.HasChannelPermissions(site.Id, channelInfo.Id, ConfigManager.ChannelPermissions.ContentOrder))
                {
                    builder.Append($@"
<a class=""dropdown-item"" href=""javascript:;"" onclick=""{ModalContentTidyUp.GetOpenWindowString(site.Id, channelInfo.Id, pageUrl)}"">
    整 理
</a>");
                }
                if (CrossSiteTransUtility.IsCrossSiteTransAsync(site, channelInfo).GetAwaiter().GetResult() && !CrossSiteTransUtility.IsAutomatic(channelInfo))
                {
                    builder.Append($@"
<a class=""dropdown-item"" href=""javascript:;"" onclick=""{ModalContentCrossSiteTrans.GetOpenWindowString(site.Id, channelInfo.Id)}"">
    跨站转发
</a>");
                }
            }

            return builder.ToString();
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
