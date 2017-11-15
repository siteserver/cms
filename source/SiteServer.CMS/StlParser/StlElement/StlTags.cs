using System.Collections.Generic;
using System.Text;
using BaiRong.Core;
using BaiRong.Core.Model;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;
using SiteServer.CMS.StlParser.Cache;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "标签", Description = "通过 stl:tags 标签在模板中显示内容标签")]
    public class StlTags
	{
        private StlTags() { }
        public const string ElementName = "stl:tags";

        public const string AttributeTagLevel = "tagLevel";
        public const string AttributeTotalNum = "totalNum";
        public const string AttributeIsOrderByCount = "isOrderByCount";
        public const string AttributeTheme = "theme";
        public const string AttributeContext = "context";

	    public static SortedList<string, string> AttributeList => new SortedList<string, string>
	    {
	        {AttributeTagLevel, "标签级别"},
	        {AttributeTotalNum, "显示标签数目"},
	        {AttributeIsOrderByCount, "是否按引用次数排序"},
	        {AttributeTheme, "主题样式"},
	        {AttributeContext, "所处上下文"}
	    };

        public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
		{
		    var tagLevel = 1;
            var totalNum = 0;
            var isOrderByCount = false;
            var theme = "default";
            var isInnerXml = !string.IsNullOrEmpty(contextInfo.InnerXml);

		    foreach (var name in contextInfo.Attributes.Keys)
		    {
		        var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, AttributeTagLevel))
                {
                    tagLevel = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeTotalNum))
                {
                    totalNum = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeIsOrderByCount))
                {
                    isOrderByCount = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeTheme))
                {
                    theme = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeContext))
                {
                    contextInfo.ContextType = EContextTypeUtils.GetEnumType(value);
                }
            }

            return ParseImpl(isInnerXml, pageInfo, contextInfo, tagLevel, totalNum, isOrderByCount, theme);
		}

        private static string ParseImpl(bool isInnerXml, PageInfo pageInfo, ContextInfo contextInfo, int tagLevel, int totalNum, bool isOrderByCount, string theme)
        {
            var innerHtml = string.Empty;
            if (isInnerXml)
            {
                innerHtml = StringUtils.StripTags(contextInfo.StlElement, ElementName);
            }

            var tagsBuilder = new StringBuilder();
            if (!isInnerXml)
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

            var tagInfoList = Tag.GetTagInfoList(pageInfo.PublishmentSystemId, contentId, isOrderByCount, totalNum);
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
                            var tagInfo = new TagInfo(0, pageInfo.PublishmentSystemId, contentId.ToString(), tagName, 1);
                            tagInfoList2.Add(tagInfo);
                        }
                    }
                }
                tagInfoList = tagInfoList2;
            }

            foreach (var tagInfo in tagInfoList)
            {
                if (isInnerXml)
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
                    var url = PageUtility.ParseNavigationUrl(pageInfo.PublishmentSystemInfo,
                        $"@/utils/tags.html?tagName={PageUtils.UrlEncode(tagInfo.Tag)}");
                    tagsBuilder.Append($@"
<li class=""tag_popularity_{tagInfo.Level}""><a target=""_blank"" href=""{url}"">{tagInfo.Tag}</a></li>
");
                }
            }

            if (!isInnerXml)
            {
                tagsBuilder.Append("</ul>");
            }
            return tagsBuilder.ToString();
        }
	}
}
