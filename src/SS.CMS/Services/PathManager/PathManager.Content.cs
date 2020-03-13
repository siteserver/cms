using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using SS.CMS.Abstractions;
using SS.CMS.Core;

namespace SS.CMS.Services
{
    public partial class PathManager
    {
        public async Task<Content> ParsePathAsync(Site site, Channel channel, int contentId)
        {
            var content = await _contentRepository.GetAsync(site, channel, contentId);
            return await ParsePathAsync(site, channel, content);
        }


        public async Task<Content> ParsePathAsync(Site site, Channel channel, Content content)
        {
            var parsedContent = content.Clone<Content>();

            var tableName = _channelRepository.GetTableName(site, channel);
            var tableStyles = await _tableStyleRepository.GetContentStyleListAsync(channel, tableName);
            foreach (var style in tableStyles)
            {
                if (style.InputType == InputType.Image)
                {
                    var value = parsedContent.Get<string>(style.AttributeName);
                    value = await ParseNavigationUrlAsync(site, value, false);

                    parsedContent.Set(style.AttributeName, value);
                }
                else if (style.InputType == InputType.TextEditor)
                {
                    var value = parsedContent.Get<string>(style.AttributeName);
                    value = await TextEditorContentDecodeAsync(site, value, true);
                    value = UEditorUtils.TranslateToHtml(value);
                    value = StringUtils.HtmlEncode(value);

                    parsedContent.Set(style.AttributeName, value);
                }
            }

            return parsedContent;
        }

        public async Task<string> TextEditorContentEncodeAsync(Site site, string content)
        {
            if (site == null) return content;

            if (site.IsSaveImageInTextEditor && !string.IsNullOrEmpty(content))
            {
                content = await SaveImageAsync(site, content);
            }

            var builder = new StringBuilder(content);

            var url = await GetWebUrlAsync(site);
            if (!string.IsNullOrEmpty(url) && url != "/")
            {
                StringUtils.ReplaceHrefOrSrc(builder, url, "@");
            }
            //if (!string.IsNullOrEmpty(url))
            //{
            //    StringUtils.ReplaceHrefOrSrc(builder, url, "@");
            //}

            var relatedSiteUrl = ParseNavigationUrl($"~/{site.SiteDir}");
            StringUtils.ReplaceHrefOrSrc(builder, relatedSiteUrl, "@");

            builder.Replace("@'@", "'@");
            builder.Replace("@\"@", "\"@");

            return builder.ToString();
        }

        public async Task<string> TextEditorContentDecodeAsync(Site site, string content, bool isLocal)
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

            return builder.ToString();
        }

        public async Task PutImagePathsAsync(Site site, Content content, NameValueCollection collection)
        {
            if (content == null) return;

            var imageUrl = content.Get<string>(ContentAttribute.ImageUrl);
            var videoUrl = content.Get<string>(ContentAttribute.VideoUrl);
            var fileUrl = content.Get<string>(ContentAttribute.FileUrl);
            var body = content.Get<string>(ContentAttribute.Content);

            if (!string.IsNullOrEmpty(imageUrl) && IsVirtualUrl(imageUrl))
            {
                collection[imageUrl] = await MapPathAsync(site, imageUrl);
            }
            if (!string.IsNullOrEmpty(videoUrl) && IsVirtualUrl(videoUrl))
            {
                collection[videoUrl] = await MapPathAsync(site, videoUrl);
            }
            if (!string.IsNullOrEmpty(fileUrl) && IsVirtualUrl(fileUrl))
            {
                collection[fileUrl] = await MapPathAsync(site, fileUrl);
            }

            var srcList = RegexUtils.GetOriginalImageSrcs(body);
            foreach (var src in srcList)
            {
                if (IsVirtualUrl(src))
                {
                    collection[src] = await MapPathAsync(site, src);
                }
                else if (IsRelativeUrl(src))
                {
                    collection[src] = MapPath(src);
                }
            }

            var hrefList = RegexUtils.GetOriginalLinkHrefs(body);
            foreach (var href in hrefList)
            {
                if (IsVirtualUrl(href))
                {
                    collection[href] = await MapPathAsync(site, href);
                }
                else if (IsRelativeUrl(href))
                {
                    collection[href] = MapPath(href);
                }
            }
        }
    }
}
