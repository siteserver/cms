using System;
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
	public class StlValue
	{
		private StlValue(){}
		public const string ElementName = "stl:value";//获取值

		public const string Attribute_Type = "type";		//需要获取值的类型
        public const string Attribute_FormatString = "formatstring";        //显示的格式
        public const string Attribute_Separator = "separator";              //显示多项时的分割字符串
        public const string Attribute_StartIndex = "startindex";			//字符开始位置
        public const string Attribute_Length = "length";			        //指定字符长度
        public const string Attribute_WordNum = "wordnum";					//显示字符的数目
        public const string Attribute_Ellipsis = "ellipsis";                //文字超出部分显示的文字
        public const string Attribute_Replace = "replace";                  //需要替换的文字，可以是正则表达式
        public const string Attribute_To = "to";                            //替换replace的文字信息
        public const string Attribute_IsClearTags = "iscleartags";          //是否清除标签信息
        public const string Attribute_IsReturnToBR = "isreturntobr";        //是否将回车替换为HTML换行标签
        public const string Attribute_IsLower = "islower";			        //转换为小写
        public const string Attribute_IsUpper = "isupper";			        //转换为大写
        public const string Attribute_IsDynamic = "isdynamic";              //是否动态显示

		public const string Type_SiteName = "SiteName";		        //频道名称
		public const string Type_SiteUrl = "SiteUrl";			    //频道的域名地址
        public const string Type_Date = "Date";			        //当前日期
        public const string Type_DateOfTraditional = "DateOfTraditional";		//带农历的当前日期

		public static ListDictionary AttributeList
		{
			get
			{
				var attributes = new ListDictionary();
				attributes.Add(Attribute_Type, "需要获取值的类型");
                attributes.Add(Attribute_FormatString, "显示的格式");
                attributes.Add(Attribute_Separator, "显示多项时的分割字符串");
                attributes.Add(Attribute_StartIndex, "字符开始位置");
                attributes.Add(Attribute_Length, "指定字符长度");
                attributes.Add(Attribute_WordNum, "显示字符的数目");
                attributes.Add(Attribute_Ellipsis, "文字超出部分显示的文字");
                attributes.Add(Attribute_Replace, "需要替换的文字，可以是正则表达式");
                attributes.Add(Attribute_To, "替换replace的文字信息");
                attributes.Add(Attribute_IsClearTags, "是否清除标签信息");
                attributes.Add(Attribute_IsReturnToBR, "是否将回车替换为HTML换行标签");
                attributes.Add(Attribute_IsLower, "转换为小写");
                attributes.Add(Attribute_IsUpper, "转换为大写");
                attributes.Add(Attribute_IsDynamic, "是否动态显示");
				return attributes;
			}
		}


		//对“值”（stl:value）元素进行解析
        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
		{
			var parsedContent = string.Empty;
			try
			{
				var ie = node.Attributes.GetEnumerator();
                var attributes = new StringDictionary();

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
                var isReturnToBR = false;
                var isLower = false;
                var isUpper = false;
                var isDynamic = false;

				while (ie.MoveNext())
				{
					var attr = (XmlAttribute)ie.Current;
					var attributeName = attr.Name.ToLower();
					if (attributeName.Equals(Attribute_Type))
					{
						type = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_FormatString))
                    {
                        formatString = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_Separator))
                    {
                        separator = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_StartIndex))
                    {
                        startIndex = TranslateUtils.ToInt(attr.Value);
                    }
                    else if (attributeName.Equals(Attribute_Length))
                    {
                        length = TranslateUtils.ToInt(attr.Value);
                    }
                    else if (attributeName.Equals(Attribute_WordNum))
                    {
                        wordNum = TranslateUtils.ToInt(attr.Value);
                    }
                    else if (attributeName.Equals(Attribute_Ellipsis))
                    {
                        ellipsis = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_Replace))
                    {
                        replace = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_To))
                    {
                        to = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_IsClearTags))
                    {
                        isClearTags = TranslateUtils.ToBool(attr.Value, false);
                    }
                    else if (attributeName.Equals(Attribute_IsReturnToBR))
                    {
                        isReturnToBR = TranslateUtils.ToBool(attr.Value, false);
                    }
                    else if (attributeName.Equals(Attribute_IsLower))
                    {
                        isLower = TranslateUtils.ToBool(attr.Value, true);
                    }
                    else if (attributeName.Equals(Attribute_IsUpper))
                    {
                        isUpper = TranslateUtils.ToBool(attr.Value, true);
                    }
                    else if (attributeName.Equals(Attribute_IsDynamic))
                    {
                        isDynamic = TranslateUtils.ToBool(attr.Value);
                    }
                    else
                    {
                        attributes.Add(attributeName, attr.Value);
                    }
				}

                if (isDynamic)
                {
                    parsedContent = StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo);
                }
                else
                {
                    parsedContent = ParseImpl(node, pageInfo, contextInfo, type, attributes, formatString, separator, startIndex, length, wordNum, ellipsis, replace, to, isClearTags, isReturnToBR, isLower, isUpper);
                }
			}
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

			return parsedContent;
		}

        private static string ParseImpl(XmlNode node, PageInfo pageInfo, ContextInfo contextInfo, string type, StringDictionary attributes, string formatString, string separator, int startIndex, int length, int wordNum, string ellipsis, string replace, string to, bool isClearTags, bool isReturnToBR, bool isLower, bool isUpper)
        {
            var parsedContent = string.Empty;

            if (type.Length > 0)
            {
                if (type.ToLower().Equals(Type_SiteName.ToLower()))
                {
                    parsedContent = pageInfo.PublishmentSystemInfo.PublishmentSystemName;
                }
                else if (type.ToLower().Equals(Type_SiteUrl.ToLower()))
                {
                    parsedContent = pageInfo.PublishmentSystemInfo.PublishmentSystemUrl;
                }
                else if (type.ToLower().Equals(Type_Date.ToLower()))
                {
                    pageInfo.AddPageScriptsIfNotExists("datestring.js",
                        $@"<script charset=""{SiteFilesAssets.DateString.Charset}"" src=""{SiteFilesAssets.GetUrl(
                            pageInfo.ApiUrl, SiteFilesAssets.DateString.Js)}"" type=""text/javascript""></script>");

                    parsedContent = @"<script language=""javascript"" type=""text/javascript"">RunGLNL(false);</script>";
                }
                else if (type.ToLower().Equals(Type_DateOfTraditional.ToLower()))
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
                        var stlTagInfo = DataProvider.StlTagDao.GetStlTagInfo(pageInfo.PublishmentSystemId, type);
                        if (stlTagInfo == null)
                        {
                            stlTagInfo = DataProvider.StlTagDao.GetStlTagInfo(0, type);
                        }
                        if (stlTagInfo != null && !string.IsNullOrEmpty(stlTagInfo.TagContent))
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
                                parsedContent = StringUtils.ParseString(EInputTypeUtils.GetEnumType(styleInfo.InputType), parsedContent, replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isReturnToBR, isLower, isUpper, formatString);
                            }
                        }
                    }                    
                }
            }

            return parsedContent;
        }
	}
}
