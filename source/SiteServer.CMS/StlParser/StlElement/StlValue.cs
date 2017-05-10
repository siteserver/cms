using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Xml;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "获取值", Description = "通过 stl:value 标签在模板中获取值")]
    public class StlValue
	{
		private StlValue(){}
		public const string ElementName = "stl:value";

		public const string AttributeType = "type";
        public const string AttributeFormatString = "formatString";
        public const string AttributeSeparator = "separator";
        public const string AttributeStartIndex = "startIndex";
        public const string AttributeLength = "length";
        public const string AttributeWordNum = "wordNum";
        public const string AttributeEllipsis = "ellipsis";
        public const string AttributeReplace = "replace";
        public const string AttributeTo = "to";
        public const string AttributeIsClearTags = "isClearTags";
        public const string AttributeIsReturnToBr = "isReturnToBr";
        public const string AttributeIsLower = "isLower";
        public const string AttributeIsUpper = "isUpper";
        public const string AttributeIsDynamic = "isDynamic";

        public static SortedList<string, string> AttributeList => new SortedList<string, string>
        {
            {AttributeType, StringUtils.SortedListToAttributeValueString("类型", TypeList)},
            {AttributeFormatString, "显示的格式"},
            {AttributeSeparator, "显示多项时的分割字符串"},
            {AttributeStartIndex, "字符开始位置"},
            {AttributeLength, "指定字符长度"},
            {AttributeWordNum, "显示字符的数目"},
            {AttributeEllipsis, "文字超出部分显示的文字"},
            {AttributeReplace, "需要替换的文字，可以是正则表达式"},
            {AttributeTo, "替换replace的文字信息"},
            {AttributeIsClearTags, "是否清除标签信息"},
            {AttributeIsReturnToBr, "是否将回车替换为HTML换行标签"},
            {AttributeIsLower, "是否转换为小写"},
            {AttributeIsUpper, "是否转换为大写"},
            {AttributeIsDynamic, "是否动态显示"}
        };

        public const string TypeSiteName = "SiteName";
		public const string TypeSiteUrl = "SiteUrl";
        public const string TypeDate = "Date";
        public const string TypeDateOfTraditional = "DateOfTraditional";

        public static SortedList<string, string> TypeList => new SortedList<string, string>
        {
            {TypeSiteName, "站点名称"},
            {TypeSiteUrl, "站点的域名地址"},
            {TypeDate, "当前日期"},
            {TypeDateOfTraditional, "带农历的当前日期"}
        };

        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
		{
			string parsedContent;
			try
			{
				var type = string.Empty;
                var formatString = string.Empty;
                string separator = null;
                var startIndex = 0;
                var length = 0;
                var wordNum = 0;
                var ellipsis = StringUtils.Constants.Ellipsis;
                var replace = string.Empty;
                var to = string.Empty;
                var isClearTags = false;
                var isReturnToBr = false;
                var isLower = false;
                var isUpper = false;
                var isDynamic = false;
                var attributes = new StringDictionary();

                var ie = node.Attributes?.GetEnumerator();
			    if (ie != null)
			    {
                    while (ie.MoveNext())
                    {
                        var attr = (XmlAttribute)ie.Current;

                        if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeType))
                        {
                            type = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeFormatString))
                        {
                            formatString = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeSeparator))
                        {
                            separator = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeStartIndex))
                        {
                            startIndex = TranslateUtils.ToInt(attr.Value);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeLength))
                        {
                            length = TranslateUtils.ToInt(attr.Value);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeWordNum))
                        {
                            wordNum = TranslateUtils.ToInt(attr.Value);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeEllipsis))
                        {
                            ellipsis = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeReplace))
                        {
                            replace = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeTo))
                        {
                            to = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsClearTags))
                        {
                            isClearTags = TranslateUtils.ToBool(attr.Value, false);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsReturnToBr))
                        {
                            isReturnToBr = TranslateUtils.ToBool(attr.Value, false);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsLower))
                        {
                            isLower = TranslateUtils.ToBool(attr.Value, true);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsUpper))
                        {
                            isUpper = TranslateUtils.ToBool(attr.Value, true);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsDynamic))
                        {
                            isDynamic = TranslateUtils.ToBool(attr.Value);
                        }
                        else
                        {
                            attributes.Add(attr.Name, attr.Value);
                        }
                    }
                }

                parsedContent = isDynamic ? StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo) : ParseImpl(node, pageInfo, contextInfo, type, attributes, formatString, separator, startIndex, length, wordNum, ellipsis, replace, to, isClearTags, isReturnToBr, isLower, isUpper);
			}
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

			return parsedContent;
		}

        private static string ParseImpl(XmlNode node, PageInfo pageInfo, ContextInfo contextInfo, string type, StringDictionary attributes, string formatString, string separator, int startIndex, int length, int wordNum, string ellipsis, string replace, string to, bool isClearTags, bool isReturnToBr, bool isLower, bool isUpper)
        {
            if (string.IsNullOrEmpty(type)) return string.Empty;

            var parsedContent = string.Empty;

            if (contextInfo.ContextType == EContextType.Each)
            {
                parsedContent = contextInfo.ItemContainer.EachItem.DataItem as string;
                return parsedContent;
            }

            if (type.ToLower().Equals(TypeSiteName.ToLower()))
            {
                parsedContent = pageInfo.PublishmentSystemInfo.PublishmentSystemName;
            }
            else if (type.ToLower().Equals(TypeSiteUrl.ToLower()))
            {
                parsedContent = pageInfo.PublishmentSystemInfo.PublishmentSystemUrl;
            }
            else if (type.ToLower().Equals(TypeDate.ToLower()))
            {
                pageInfo.AddPageScriptsIfNotExists("datestring.js",
                    $@"<script charset=""{SiteFilesAssets.DateString.Charset}"" src=""{SiteFilesAssets.GetUrl(
                        pageInfo.ApiUrl, SiteFilesAssets.DateString.Js)}"" type=""text/javascript""></script>");

                parsedContent = @"<script language=""javascript"" type=""text/javascript"">RunGLNL(false);</script>";
            }
            else if (type.ToLower().Equals(TypeDateOfTraditional.ToLower()))
            {
                pageInfo.AddPageScriptsIfNotExists("datestring",
                    $@"<script charset=""{SiteFilesAssets.DateString.Charset}"" src=""{SiteFilesAssets.GetUrl(
                        pageInfo.ApiUrl, SiteFilesAssets.DateString.Js)}"" type=""text/javascript""></script>");

                parsedContent = @"<script language=""javascript"" type=""text/javascript"">RunGLNL(true);</script>";
            }
            else
            {
                if (pageInfo.PublishmentSystemInfo.Additional.Attributes.Get(type) == null)
                {
                    var stlTagInfo = DataProvider.StlTagDao.GetStlTagInfo(pageInfo.PublishmentSystemId, type) ??
                                     DataProvider.StlTagDao.GetStlTagInfo(0, type);
                    if (!string.IsNullOrEmpty(stlTagInfo?.TagContent))
                    {
                        var innerBuilder = new StringBuilder(stlTagInfo.TagContent);
                        StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                        parsedContent = innerBuilder.ToString();
                    }
                }
                else
                {
                    parsedContent = pageInfo.PublishmentSystemInfo.Additional.Attributes[type];
                    if (!string.IsNullOrEmpty(parsedContent))
                    {
                        var styleInfo = TableStyleManager.GetTableStyleInfo(ETableStyle.Site, DataProvider.PublishmentSystemDao.TableName, type, RelatedIdentities.GetRelatedIdentities(ETableStyle.Site, pageInfo.PublishmentSystemId, pageInfo.PublishmentSystemId));

                        if (isClearTags && EInputTypeUtils.EqualsAny(styleInfo.InputType, EInputType.Image, EInputType.File))
                        {
                            parsedContent = PageUtility.ParseNavigationUrl(pageInfo.PublishmentSystemInfo, parsedContent);
                        }
                        else
                        {
                            parsedContent = InputParserUtility.GetContentByTableStyle(parsedContent, separator, pageInfo.PublishmentSystemInfo, ETableStyle.Site, styleInfo, formatString, attributes, node.InnerXml, false);
                            parsedContent = StringUtils.ParseString(EInputTypeUtils.GetEnumType(styleInfo.InputType), parsedContent, replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isReturnToBr, isLower, isUpper, formatString);
                        }
                    }
                }
            }
            return parsedContent;
        }
	}
}
