using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SS.CMS.Core.Common;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Models;
using SS.CMS.Utils;

namespace SS.CMS.Core.StlParser.StlElement
{
    [StlElement(Title = "标签", Description = "通过 stl:tags 标签在模板中显示内容标签")]
    public class StlTags
    {
        private StlTags() { }
        public const string ElementName = "stl:tags";

        [StlAttribute(Title = "标签级别")]
        private const string TagLevel = nameof(TagLevel);

        [StlAttribute(Title = "显示标签数目")]
        private const string TotalNum = nameof(TotalNum);

        [StlAttribute(Title = "是否按引用次数排序")]
        private const string IsOrderByCount = nameof(IsOrderByCount);

        [StlAttribute(Title = "主题样式")]
        private const string Theme = nameof(Theme);

        [StlAttribute(Title = "所处上下文")]
        private const string Context = nameof(Context);


        public static async Task<object> ParseAsync(ParseContext parseContext)
        {
            var tagLevel = 1;
            var totalNum = 0;
            var isOrderByCount = false;
            var theme = "default";
            var isInnerHtml = !string.IsNullOrEmpty(parseContext.InnerHtml);

            foreach (var name in parseContext.Attributes.AllKeys)
            {
                var value = parseContext.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, TagLevel))
                {
                    tagLevel = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, TotalNum))
                {
                    totalNum = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsOrderByCount))
                {
                    isOrderByCount = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Theme))
                {
                    theme = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Context))
                {
                    parseContext.ContextType = EContextTypeUtils.GetEnumType(value);
                }
            }

            return await ParseImplAsync(parseContext, isInnerHtml, tagLevel, totalNum, isOrderByCount, theme);
        }

        private static async Task<string> ParseImplAsync(ParseContext parseContext, bool isInnerHtml, int tagLevel, int totalNum, bool isOrderByCount, string theme)
        {
            var innerHtml = string.Empty;
            if (isInnerHtml)
            {
                innerHtml = StringUtils.StripTags(parseContext.OuterHtml, ElementName);
            }

            var tagsBuilder = new StringBuilder();
            if (!isInnerHtml)
            {
                tagsBuilder.Append($@"
<link rel=""stylesheet"" href=""{SiteFilesAssets.Tags.GetStyleUrl(theme)}"" type=""text/css"" />
");
                tagsBuilder.Append(@"<ul class=""tagCloud"">");
            }

            if (parseContext.ContextType == EContextType.Undefined)
            {
                parseContext.ContextType = parseContext.ContentId != 0 ? EContextType.Content : EContextType.Channel;
            }
            var contentId = 0;
            if (parseContext.ContextType == EContextType.Content)
            {
                contentId = parseContext.ContentId;
            }

            var tagInfoList = parseContext.TagRepository.GetTagInfoList(parseContext.SiteId, contentId, isOrderByCount, totalNum);
            tagInfoList = parseContext.TagRepository.GetTagInfoList(tagInfoList, totalNum, tagLevel);
            if (parseContext.ContextType == EContextType.Content)
            {
                var contentInfo = await parseContext.GetContentInfoAsync();
                if (contentInfo != null)
                {
                    var tagInfoList2 = new List<TagInfo>();
                    var tagNameList = TranslateUtils.StringCollectionToStringList(contentInfo.Tags.Trim().Replace(" ", ","));
                    foreach (var tagName in tagNameList)
                    {
                        if (!string.IsNullOrEmpty(tagName))
                        {
                            var isAdd = false;
                            foreach (var tagInfo in tagInfoList)
                            {
                                if (tagInfo.Tag == tagName)
                                {
                                    isAdd = true;
                                    tagInfoList2.Add(tagInfo);
                                    break;
                                }
                            }
                            if (!isAdd)
                            {
                                var tagInfo = new TagInfo
                                {
                                    SiteId = parseContext.SiteId,
                                    ContentIdCollection = contentId.ToString(),
                                    Tag = tagName,
                                    UseNum = 1
                                };
                                tagInfoList2.Add(tagInfo);
                            }
                        }
                    }
                    tagInfoList = tagInfoList2;
                }
            }

            foreach (var tagInfo in tagInfoList)
            {
                if (isInnerHtml)
                {
                    var tagHtml = innerHtml;
                    tagHtml = StringUtils.ReplaceIgnoreCase(tagHtml, "{Tag.Name}", tagInfo.Tag);
                    tagHtml = StringUtils.ReplaceIgnoreCase(tagHtml, "{Tag.Count}", tagInfo.UseNum.ToString());
                    tagHtml = StringUtils.ReplaceIgnoreCase(tagHtml, "{Tag.Level}", tagInfo.Level.ToString());
                    var innerBuilder = new StringBuilder(tagHtml);
                    await parseContext.ParseInnerContentAsync(innerBuilder);
                    tagsBuilder.Append(innerBuilder);
                }
                else
                {
                    var url = parseContext.UrlManager.ParseNavigationUrl(parseContext.SiteInfo,
                        $"@/utils/tags.html?tagName={PageUtils.UrlEncode(tagInfo.Tag)}", parseContext.IsLocal);
                    tagsBuilder.Append($@"
<li class=""tag_popularity_{tagInfo.Level}""><a target=""_blank"" href=""{url}"">{tagInfo.Tag}</a></li>
");
                }
            }

            if (!isInnerHtml)
            {
                tagsBuilder.Append("</ul>");
            }
            return tagsBuilder.ToString();
        }
    }
}
