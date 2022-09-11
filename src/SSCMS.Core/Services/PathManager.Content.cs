using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class PathManager
    {
        public async Task<Content> EncodeContentAsync(Site site, Channel channel, Content content)
        {
            content = content.Clone<Content>();

            var tableStyles = await _tableStyleRepository.GetContentStylesAsync(site, channel);
            foreach (var style in tableStyles)
            {
                if (style.InputType == InputType.Image || style.InputType == InputType.Video || style.InputType == InputType.File)
                {
                    var countName = ColumnsManager.GetCountName(style.AttributeName);
                    var count = content.Get<int>(countName);
                    for (var i = 0; i <= count; i++)
                    {
                        var extendName = ColumnsManager.GetExtendName(style.AttributeName, i);
                        var value = content.Get<string>(extendName);
                        value = GetVirtualUrl(site, value);
                        content.Set(extendName, value);
                    }
                }
                else if (style.InputType == InputType.TextEditor)
                {
                    var value = content.Get<string>(style.AttributeName);
                    value = await EncodeTextEditorAsync(site, value);
                    value = UEditorUtils.TranslateToStlElement(value);
                    value = StringUtils.TrimEnd(value, @"<p><br/></p>");
                    content.Set(style.AttributeName, value);
                }
            }

            return content;
        }

        public async Task<Content> DecodeContentAsync(Site site, Channel channel, int contentId)
        {
            var content = await _contentRepository.GetAsync(site, channel, contentId);
            return await DecodeContentAsync(site, channel, content);
        }

        public async Task<Content> DecodeContentAsync(Site site, Channel channel, Content content)
        {
            content = content.Clone<Content>();

            var tableStyles = await _tableStyleRepository.GetContentStylesAsync(site, channel);
            foreach (var style in tableStyles)
            {
                if (style.InputType == InputType.Image || style.InputType == InputType.Video || style.InputType == InputType.File)
                {
                    var countName = ColumnsManager.GetCountName(style.AttributeName);
                    var count = content.Get<int>(countName);
                    for (var i = 0; i <= count; i++)
                    {
                        var extendName = ColumnsManager.GetExtendName(style.AttributeName, i);
                        var value = content.Get<string>(extendName);
                        value = await ParseSiteUrlAsync(site, value, false);

                        content.Set(extendName, value);
                    }
                }
                else if (style.InputType == InputType.TextEditor)
                {
                    var value = content.Get<string>(style.AttributeName);
                    value = await DecodeTextEditorAsync(site, value, true);
                    value = UEditorUtils.TranslateToHtml(value);

                    content.Set(style.AttributeName, value);
                }
            }

            return content;
        }

        public async Task<string> EncodeTextEditorAsync(Site site, string content)
        {
            if (site == null) return content;

            if (site.IsSaveImageInTextEditor && !string.IsNullOrEmpty(content))
            {
                content = await SaveImageAsync(site, content);
            }

            var builder = new StringBuilder(content);

            var webUrl = await GetWebUrlAsync(site);
            if (!string.IsNullOrEmpty(webUrl) && webUrl != "/")
            {
                StringUtils.ReplaceHrefOrSrc(builder, webUrl, "@");
            }
            //if (!string.IsNullOrEmpty(url))
            //{
            //    StringUtils.ReplaceHrefOrSrc(builder, url, "@");
            //}

            var localUrl = await GetSiteUrlAsync(site, true);
            if (!string.IsNullOrEmpty(localUrl) && localUrl != "/")
            {
                StringUtils.ReplaceHrefOrSrc(builder, localUrl, "@");
            }

            var relatedSiteUrl = ParseUrl($"~/{site.SiteDir}");
            StringUtils.ReplaceHrefOrSrc(builder, relatedSiteUrl, "@");

            builder.Replace("@'@", "'@");
            builder.Replace("@\"@", "\"@");

            return builder.ToString();
        }

        public async Task<string> DecodeTextEditorAsync(Site site, string content, bool isLocal)
        {
            if (site == null) return content;

            var builder = new StringBuilder(content);

            var webUrl = await GetWebUrlAsync(site);
            var virtualAssetsUrl = $"@/{site.AssetsDir}";
            string assetsUrl;
            if (isLocal)
            {
                assetsUrl = await GetSiteUrlAsync(site,
                    site.AssetsDir, true);
            }
            else
            {
                assetsUrl = await GetAssetsUrlAsync(site);
            }
            StringUtils.ReplaceHrefOrSrc(builder, virtualAssetsUrl, assetsUrl);
            StringUtils.ReplaceHrefOrSrc(builder, "@/", webUrl + "/");
            StringUtils.ReplaceHrefOrSrc(builder, "@", webUrl + "/");
            StringUtils.ReplaceHrefOrSrc(builder, "//", "/");

            builder.Replace("&#xa0;", "&nbsp;");

            if (_settingsManager.IsSafeMode)
            {
                return StringUtils.FilterXss(builder.ToString());
            }

            return builder.ToString();
        }

        public async Task PutImagePathsAsync(Site site, Content content, NameValueCollection collection)
        {
            if (content == null) return;

            var imageUrl = content.ImageUrl;
            var videoUrl = content.VideoUrl;
            var fileUrl = content.FileUrl;
            var body = content.Body;

            if (!string.IsNullOrEmpty(imageUrl) && IsVirtualUrl(imageUrl))
            {
                collection[imageUrl] = await ParseSitePathAsync(site, imageUrl);

                var countName = ColumnsManager.GetCountName(nameof(Content.ImageUrl));
                var count = content.Get<int>(countName);
                for (var i = 1; i <= count; i++)
                {
                    var extendName = ColumnsManager.GetExtendName(nameof(Content.ImageUrl), i);
                    var extend = content.Get<string>(extendName);
                    collection[extend] = await ParseSitePathAsync(site, extend);
                }
            }
            if (!string.IsNullOrEmpty(videoUrl) && IsVirtualUrl(videoUrl))
            {
                collection[videoUrl] = await ParseSitePathAsync(site, videoUrl);

                var countName = ColumnsManager.GetCountName(nameof(Content.VideoUrl));
                var count = content.Get<int>(countName);
                for (var i = 1; i <= count; i++)
                {
                    var extendName = ColumnsManager.GetExtendName(nameof(Content.VideoUrl), i);
                    var extend = content.Get<string>(extendName);
                    collection[extend] = await ParseSitePathAsync(site, extend);
                }
            }
            if (!string.IsNullOrEmpty(fileUrl) && IsVirtualUrl(fileUrl))
            {
                collection[fileUrl] = await ParseSitePathAsync(site, fileUrl);

                var countName = ColumnsManager.GetCountName(nameof(Content.FileUrl));
                var count = content.Get<int>(countName);
                for (var i = 1; i <= count; i++)
                {
                    var extendName = ColumnsManager.GetExtendName(nameof(Content.FileUrl), i);
                    var extend = content.Get<string>(extendName);
                    collection[extend] = await ParseSitePathAsync(site, extend);
                }
            }

            var srcList = RegexUtils.GetOriginalImageSrcs(body);
            foreach (var src in srcList)
            {
                if (IsVirtualUrl(src))
                {
                    collection[src] = await ParseSitePathAsync(site, src);
                }
                else if (IsRelativeUrl(src))
                {
                    collection[src] = ParsePath(src);
                }
            }

            var hrefList = RegexUtils.GetOriginalLinkHrefs(body);
            foreach (var href in hrefList)
            {
                if (IsVirtualUrl(href))
                {
                    collection[href] = await ParseSitePathAsync(site, href);
                }
                else if (IsRelativeUrl(href))
                {
                    collection[href] = ParsePath(href);
                }
            }
        }
    }
}
