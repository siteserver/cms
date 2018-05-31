using System.Collections.Generic;
using System.Text;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;
using SiteServer.CMS.StlParser.Cache;

namespace SiteServer.CMS.StlParser.StlElement
{
    [StlClass(Usage = "标签", Description = "通过 stl:tags 标签在模板中显示内容标签")]
    public class StlTags
	{
        private StlTags() { }
        public const string ElementName = "stl:tags";

        private static readonly Attr TagLevel = new Attr("tagLevel", "标签级别");
        private static readonly Attr TotalNum = new Attr("totalNum", "显示标签数目");
        private static readonly Attr IsOrderByCount = new Attr("isOrderByCount", "是否按引用次数排序");
        private static readonly Attr Theme = new Attr("theme", "主题样式");
        private static readonly Attr Context = new Attr("context", "所处上下文");

        public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
		{
		    var tagLevel = 1;
            var totalNum = 0;
            var isOrderByCount = false;
            var theme = "default";
            var isInnerHtml = !string.IsNullOrEmpty(contextInfo.InnerHtml);

		    foreach (var name in contextInfo.Attributes.AllKeys)
		    {
		        var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, TagLevel.Name))
                {
                    tagLevel = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, TotalNum.Name))
                {
                    totalNum = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsOrderByCount.Name))
                {
                    isOrderByCount = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Theme.Name))
                {
                    theme = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Context.Name))
                {
                    contextInfo.ContextType = EContextTypeUtils.GetEnumType(value);
                }
            }

            return ParseImpl(isInnerHtml, pageInfo, contextInfo, tagLevel, totalNum, isOrderByCount, theme);
		}

        private static string ParseImpl(bool isInnerHtml, PageInfo pageInfo, ContextInfo contextInfo, int tagLevel, int totalNum, bool isOrderByCount, string theme)
        {
            var innerHtml = string.Empty;
            if (isInnerHtml)
            {
                innerHtml = StringUtils.StripTags(contextInfo.OuterHtml, ElementName);
            }

            var tagsBuilder = new StringBuilder();
            if (!isInnerHtml)
            {
                tagsBuilder.Append($@"
<link rel=""stylesheet"" href=""{SiteFilesAssets.Tags.GetStyleUrl(pageInfo.ApiUrl, theme)}"" type=""text/css"" />
");
                tagsBuilder.Append(@"<ul class=""tagCloud"">");
            }

            if (contextInfo.ContextType == EContextType.Undefined)
            {
                contextInfo.ContextType = contextInfo.ContentId != 0 ? EContextType.Content : EContextType.Channel;
            }
            var contentId = 0;
            if (contextInfo.ContextType == EContextType.Content)
            {
                contentId = contextInfo.ContentId;
            }

            var tagInfoList = Tag.GetTagInfoList(pageInfo.SiteId, contentId, isOrderByCount, totalNum);
            tagInfoList = TagUtils.GetTagInfoList(tagInfoList, totalNum, tagLevel);
            if (contextInfo.ContextType == EContextType.Content && contextInfo.ContentInfo != null)
            {
                var tagInfoList2 = new List<TagInfo>();
                var tagNameList = TranslateUtils.StringCollectionToStringList(contextInfo.ContentInfo.Tags.Trim().Replace(" ", ","));
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
                            var tagInfo = new TagInfo(0, pageInfo.SiteId, contentId.ToString(), tagName, 1);
                            tagInfoList2.Add(tagInfo);
                        }
                    }
                }
                tagInfoList = tagInfoList2;
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
                    StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                    tagsBuilder.Append(innerBuilder);
                }
                else
                {
                    var url = PageUtility.ParseNavigationUrl(pageInfo.SiteInfo,
                        $"@/utils/tags.html?tagName={PageUtils.UrlEncode(tagInfo.Tag)}", pageInfo.IsLocal);
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
