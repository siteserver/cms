using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using BaiRong.Core;
using BaiRong.Core.Model;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

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
        public const string AttributeIsDynamic = "isDynamic";

	    public static SortedList<string, string> AttributeList => new SortedList<string, string>
	    {
	        {AttributeTagLevel, "标签级别"},
	        {AttributeTotalNum, "显示标签数目"},
	        {AttributeIsOrderByCount, "是否按引用次数排序"},
	        {AttributeTheme, "主题样式"},
	        {AttributeContext, "所处上下文"},
	        {AttributeIsDynamic, "是否动态显示"}
	    };

        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfoRef)
		{
			string parsedContent;
            var contextInfo = contextInfoRef.Clone();
			try
			{
                var tagLevel = 1;
                var totalNum = 0;
                var isOrderByCount = false;
                var theme = "default";
                var isDynamic = false;
                var isInnerXml = !string.IsNullOrEmpty(node.InnerXml);
                var attributes = new LowerNameValueCollection();

                var ie = node.Attributes?.GetEnumerator();
			    if (ie != null)
			    {
			        while (ie.MoveNext())
			        {
			            var attr = (XmlAttribute) ie.Current;

			            if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeTagLevel))
			            {
			                tagLevel = TranslateUtils.ToInt(attr.Value);
			            }
			            else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeTotalNum))
			            {
			                totalNum = TranslateUtils.ToInt(attr.Value);
			            }
			            else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsOrderByCount))
			            {
			                isOrderByCount = TranslateUtils.ToBool(attr.Value);
			            }
			            else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeTheme))
			            {
			                theme = attr.Value;
			            }
			            else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeContext))
			            {
			                contextInfo.ContextType = EContextTypeUtils.GetEnumType(attr.Value);
			            }
			            else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsDynamic))
			            {
			                isDynamic = TranslateUtils.ToBool(attr.Value);
			            }
			            else
			            {
			                attributes.Set(attr.Name, attr.Value);
			            }
			        }
			    }

			    parsedContent = isDynamic ? StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo) : ParseImpl(stlElement, isInnerXml, pageInfo, contextInfo, tagLevel, totalNum, isOrderByCount, theme);
			}
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

			return parsedContent;
		}

        private static string ParseImpl(string stlElement, bool isInnerXml, PageInfo pageInfo, ContextInfo contextInfo, int tagLevel, int totalNum, bool isOrderByCount, string theme)
        {
            var innerHtml = string.Empty;
            if (isInnerXml)
            {
                innerHtml = StringUtils.StripTags(stlElement, ElementName);
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

            var tagInfoList = TagUtils.GetTagInfoList(pageInfo.PublishmentSystemId, contentId, totalNum, isOrderByCount, tagLevel);
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
