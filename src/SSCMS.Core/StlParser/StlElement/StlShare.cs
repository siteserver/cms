using System.Threading.Tasks;
using SSCMS.Core.StlParser.Attributes;
using SSCMS.Parse;
using SSCMS.Models;
using SSCMS.Services;
using SSCMS.Utils;
using System.Collections.Specialized;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "页面分享", Description = "通过 stl:share 标签在模板中添加页面分享功能")]
    public static class StlShare
    {
        public const string ElementName = "stl:share";

        [StlAttribute(Title = "分享网址")]
        private const string Url = "url";      //默认使用 window.location.href

        [StlAttribute(Title = "来源")]
        private const string Source = "source";//QQ空间会用到, 默认读取head标签：<meta name="site" content="" />

        [StlAttribute(Title = "标题")]
        private const string Title = "title";  //默认读取 document.title 或者 <meta name="title" content="" />

        [StlAttribute(Title = "分享")]
        private const string Origin = "origin";    // 分享 @ 相关 twitter 账号

        [StlAttribute(Title = "描述")]
        private const string Description = "description";  //默认读取head标签：<meta name="description" content="" />

        [StlAttribute(Title = "图片")]
        private const string Image = "image";  //默认取网页中第一个img标签

        [StlAttribute(Title = "分享至")]
        private const string Sites = "sites";  //: ['qzone', 'qq', 'weibo','wechat', 'douban']

        [StlAttribute(Title = "禁用")]
        private const string Disabled = "disabled";  //: ['google', 'facebook', 'twitter']

        [StlAttribute(Title = "微信扫一扫提示文字")]
        private const string WechatQrcodeTitle = "wechatQrcodeTitle";//微信扫一扫：分享，微信二维码提示文字

        [StlAttribute(Title = "微信分享至朋友圈")]
        private const string WechatQrcodeHelper = "wechatQrcodeHelper";    //微信里点“发现”，扫一下二维码便可将本文分享至朋友圈。'

        public static async Task<object> ParseAsync(IParseManager parseManager)
        {
            var url = string.Empty;
            var source = string.Empty;
            var title = string.Empty;
            var origin = string.Empty;
            var description = string.Empty;
            var image = string.Empty;
            var sites = "wechat, weibo, qq, qzone, douban";
            var disabled = string.Empty;
            var wechatQrcodeTitle = string.Empty;
            var wechatQrcodeHelper = string.Empty;
            var attributes = new NameValueCollection();

            foreach (var name in parseManager.ContextInfo.Attributes.AllKeys)
            {
                var value = parseManager.ContextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, Url))
                {
                    url = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Source))
                {
                    source = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Title))
                {
                    title = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Origin))
                {
                    origin = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Description))
                {
                    description = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Image))
                {
                    image = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Sites))
                {
                    sites = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Disabled))
                {
                    disabled = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, WechatQrcodeTitle))
                {
                    wechatQrcodeTitle = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, WechatQrcodeHelper))
                {
                    wechatQrcodeHelper = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else
                {
                    attributes[name] = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
            }

            return await ParseAsync(parseManager, url, source, title, origin, description, image, sites, disabled, wechatQrcodeTitle, wechatQrcodeHelper, attributes);
        }

        private static async Task<string> ParseAsync(IParseManager parseManager, string url, string source, string title, string origin, string description, string image, string sites, string disabled, string wechatQrcodeTitle, string wechatQrcodeHelper, NameValueCollection attributes)
        {
            var pageInfo = parseManager.PageInfo;
            var contextInfo = parseManager.ContextInfo;
            var siteRepository = parseManager.DatabaseManager.SiteRepository;
            var contentRepository = parseManager.DatabaseManager.ContentRepository;
            var pathManager = parseManager.PathManager;
            var site = await siteRepository.GetAsync(pageInfo.SiteId);

            await pageInfo.AddPageAfterHtmlCodeIfNotExistsAsync(ParsePage.Const.Share);

            Content content = null;
            if (contextInfo.ContentId > 0)
            {
                if (string.IsNullOrEmpty(title))
                {
                    if (content == null)
                    {
                        content = await contentRepository.GetAsync(site, contextInfo.ChannelId, contextInfo.ContentId);
                    }

                    if (content != null)
                    {
                        title = content.Title;
                    }
                }
                if (string.IsNullOrEmpty(image))
                {
                    if (content == null)
                    {
                        content = await contentRepository.GetAsync(site, contextInfo.ChannelId, contextInfo.ContentId);
                    }

                    if (content != null && !string.IsNullOrEmpty(content.ImageUrl))
                    {
                        image = await pathManager.ParseSiteUrlAsync(site, content.ImageUrl, false);
                        image = PageUtils.AddProtocolToUrl(image);
                    }
                }
                if (string.IsNullOrEmpty(description))
                {
                    if (content == null)
                    {
                        content = await contentRepository.GetAsync(site, contextInfo.ChannelId, contextInfo.ContentId);
                    }

                    if (content != null)
                    {
                        description = content.Summary;
                        if (string.IsNullOrEmpty(description))
                        {
                            description = StringUtils.MaxLengthText(StringUtils.StripTags(content.Body), 50);
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(url))
            {
                attributes["data-url"] = url;
            }
            if (!string.IsNullOrEmpty(source))
            {
                attributes["data-source"] = source;
            }
            if (!string.IsNullOrEmpty(title))
            {
                attributes["data-title"] = title;
            }
            if (!string.IsNullOrEmpty(origin))
            {
                attributes["data-origin"] = origin;
            }
            if (!string.IsNullOrEmpty(description))
            {
                attributes["data-description"] = description;
            }
            if (!string.IsNullOrEmpty(image))
            {
                attributes["data-image"] = image;
            }
            if (!string.IsNullOrEmpty(sites))
            {
                attributes["data-sites"] = sites;
            }
            if (!string.IsNullOrEmpty(disabled))
            {
                attributes["data-disabled"] = disabled;
            }
            if (!string.IsNullOrEmpty(wechatQrcodeTitle))
            {
                attributes["data-wechat-qrcode-title"] = wechatQrcodeTitle;
            }
            if (!string.IsNullOrEmpty(wechatQrcodeHelper))
            {
                attributes["data-data-wechat-qrcode-helper"] = wechatQrcodeHelper;
            }

            return $@"<div class=""social-share"" {TranslateUtils.ToAttributesString(attributes)}></div>";
        }
    }
}
