using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SSCMS.Core.StlParser.Attributes;
using SSCMS.Parse;
using SSCMS.Core.Utils;
using SSCMS.Models;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "PDF展示", Description = "通过 stl:pdf 标签将PDF文件嵌入到HTML文档中")]
    public static class StlPdf
    {
        public const string ElementName = "stl:pdf";

        [StlAttribute(Title = "指定存储PDF文件的字段")]
        private const string Type = nameof(Type);

        [StlAttribute(Title = "显示字段的顺序")]
        private const string No = nameof(No);

        [StlAttribute(Title = "PDF文件地址")]
        private const string Src = nameof(Src);

        [StlAttribute(Title = "浏览器不支持时显示")]
        private const string FallbackLink = nameof(FallbackLink);

        [StlAttribute(Title = "使用iframe显示PDF")]
        private const string ForceIframe = nameof(ForceIframe);

        [StlAttribute(Title = "显示高度")]
        private const string Height = nameof(Height);

        [StlAttribute(Title = "默认显示指定页")]
        private const string Page = nameof(Page);

        [StlAttribute(Title = "显示宽度")]
        private const string Width = nameof(Width);

        [StlAttribute(Title = "整屏显示")]
        private const string Full = nameof(Full);

        public static async Task<object> ParseAsync(IParseManager parseManager)
        {
            var type = nameof(Content.FileUrl);
            var no = 0;
            var src = string.Empty;
            var fallbackLink = string.Empty;
            var forceIframe = false;
            var height = string.Empty;
            var page = 0;
            var width = string.Empty;
            var full = false;
            
            var attributes = new NameValueCollection();

            foreach (var name in parseManager.ContextInfo.Attributes.AllKeys)
            {
                var value = parseManager.ContextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, Type))
                {
                    type = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, No))
                {
                    no = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Src))
                {
                    src = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, FallbackLink))
                {
                    fallbackLink = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, ForceIframe))
                {
                    forceIframe = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Height))
                {
                    height = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Page))
                {
                    page = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Width))
                {
                    width = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Full))
                {
                    full = TranslateUtils.ToBool(value);
                }
                else
                {
                    attributes[name] = value;
                }
            }

            return await ParseAsync(parseManager, type, no, src, fallbackLink, forceIframe, height, page, width, full, attributes);
        }

        private static async Task<string> ParseAsync(IParseManager parseManager, string type, int no, string src, string fallbackLink, bool forceIframe, string height, int page, string width, bool full, NameValueCollection attributes)
        {
            var pageInfo = parseManager.PageInfo;
            var contextInfo = parseManager.ContextInfo;
            var elementId = StringUtils.GetElementId();

            var contentInfo = await parseManager.GetContentAsync();

            var fileUrl = string.Empty;
            if (!string.IsNullOrEmpty(src))
            {
                fileUrl = src;
            }
            else
            {
                if (contextInfo.ContextType == ParseType.Undefined)
                {
                    contextInfo.ContextType = ParseType.Content;
                }
                if (contextInfo.ContextType == ParseType.Content)
                {
                    if (contextInfo.ContentId != 0)
                    {
                        if (!string.IsNullOrEmpty(contentInfo?.Get<string>(type)))
                        {
                            if (no <= 1)
                            {
                                fileUrl = contentInfo.Get<string>(type);
                            }
                            else
                            {
                                var extendName = ColumnsManager.GetExtendName(type, no - 1);
                                fileUrl = contentInfo.Get<string>(extendName);
                            }
                        }
                    }
                }
                else if (contextInfo.ContextType == ParseType.Each)
                {
                    fileUrl = contextInfo.ItemContainer.EachItem.Value as string;
                }
            }

            fileUrl = await parseManager.PathManager.ParseSiteUrlAsync(pageInfo.Site, fileUrl, false);

            await pageInfo.AddPageBodyCodeIfNotExistsAsync(ParsePage.Const.PdfObject);

            var options = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(fallbackLink))
            {
                options["fallbackLink"] = fallbackLink;
            }
            options["forceIframe"] = forceIframe;
            if (page > 0)
            {
                options["page"] = page;
            }

            if (full)
            {
                return $@"<script>PDFObject.embed(""{fileUrl}"", document.body, {JsonConvert.SerializeObject(options)});</script>";
            }

            if (!string.IsNullOrEmpty(height))
            {
                options["height"] = height;
            }
            if (!string.IsNullOrEmpty(width))
            {
                options["width"] = width;
            }

            attributes["id"] = elementId;
            return $@"<div {TranslateUtils.ToAttributesString(attributes)}></div><script>PDFObject.embed(""{fileUrl}"", ""#{elementId}"", {JsonConvert.SerializeObject(options)});</script>";
        }
    }
}
