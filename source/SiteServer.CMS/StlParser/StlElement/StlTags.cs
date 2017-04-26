using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Xml;
using BaiRong.Core;
using BaiRong.Core.Model;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
	public class StlTags
	{
        private StlTags() { }
        public const string ElementName = "stl:tags";//标签

        public const string Attribute_TagLevel = "taglevel";					        //标签级别
        public const string Attribute_TotalNum = "totalnum";					        //显示标签数目
        public const string Attribute_IsOrderByCount = "isorderbycount";				//是否按引用次数排序
        public const string Attribute_Theme = "theme";			            //主题样式
        public const string Attribute_Context = "context";                  //所处上下文

        public const string Attribute_IsDynamic = "isdynamic";              //是否动态显示

		public static ListDictionary AttributeList
		{
			get
			{
				var attributes = new ListDictionary();

                attributes.Add(Attribute_TagLevel, "标签级别");
                attributes.Add(Attribute_TotalNum, "显示标签数目");
                attributes.Add(Attribute_IsOrderByCount, "是否按引用次数排序");
                attributes.Add(Attribute_Theme, "主题样式");
                attributes.Add(Attribute_Context, "所处上下文");
                attributes.Add(Attribute_IsDynamic, "是否动态显示");
				return attributes;
			}
		}

        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfoRef)
		{
			var parsedContent = string.Empty;
            var contextInfo = contextInfoRef.Clone();
			try
			{
                var attributes = new LowerNameValueCollection();
				var ie = node.Attributes.GetEnumerator();

                var tagLevel = 1;
                var totalNum = 0;
                var isOrderByCount = false;
                var theme = "default";

                var isDynamic = false;
                var isInnerXml = !string.IsNullOrEmpty(node.InnerXml);

				while (ie.MoveNext())
				{
					var attr = (XmlAttribute)ie.Current;
					var attributeName = attr.Name.ToLower();
                    
                    if (attributeName.Equals(Attribute_TagLevel))
                    {
                        tagLevel = TranslateUtils.ToInt(attr.Value);
                    }
                    else if (attributeName.Equals(Attribute_TotalNum))
                    {
                        totalNum = TranslateUtils.ToInt(attr.Value);
                    }
                    else if (attributeName.Equals(Attribute_IsOrderByCount))
                    {
                        isOrderByCount = TranslateUtils.ToBool(attr.Value);
                    }
                    else if (attributeName.Equals(Attribute_Theme))
                    {
                        theme = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_Context))
                    {
                        contextInfo.ContextType = EContextTypeUtils.GetEnumType(attr.Value);
                    }
                    else if (attributeName.Equals(Attribute_IsDynamic))
                    {
                        isDynamic = TranslateUtils.ToBool(attr.Value);
                    }
					else
					{
                        attributes[attributeName] = attr.Value;
					}
				}

                if (isDynamic)
                {
                    parsedContent = StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo);
                }
                else
                {
                    parsedContent = ParseImpl(stlElement, isInnerXml, pageInfo, contextInfo, tagLevel, totalNum, isOrderByCount, theme);
                }
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
                if (contextInfo.ContentID != 0)
                {
                    contextInfo.ContextType = EContextType.Content;
                }
                else
                {
                    contextInfo.ContextType = EContextType.Channel;
                }
            }
            var contentID = 0;
            if (contextInfo.ContextType == EContextType.Content)
            {
                contentID = contextInfo.ContentID;
            }

            var tagInfoList = TagUtils.GetTagInfoList(pageInfo.PublishmentSystemId, contentID, totalNum, isOrderByCount, tagLevel);
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
                            var tagInfo = new TagInfo(0, pageInfo.PublishmentSystemId, contentID.ToString(), tagName, 1);
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
