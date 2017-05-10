using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Text;
using BaiRong.Core.Text.Sgml;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.StlElement;

namespace SiteServer.CMS.StlParser.Utility
{
    public class StlParserUtility
    {
        public const string OrderDefault = "Default";								//默认排序
        public const string OrderBack = "Back";								//默认排序的相反方向
        public const string OrderAddDate = "AddDate";							//添加时间
        public const string OrderAddDateBack = "AddDateBack";					//添加时间的相反方向
        public const string OrderLastEditDate = "LastEditDate";				//最后更改时间（只可用于内容列表）
        public const string OrderLastEditDateBack = "LastEditDateBack";	//最后更改时间的相反方向（只可用于内容列表）
        public const string OrderHits = "Hits";	            //点击量
        public const string OrderHitsByDay = "HitsByDay";	    //日点击量
        public const string OrderHitsByWeek = "HitsByWeek";	//周点击量
        public const string OrderHitsByMonth = "HitsByMonth";	//月点击量
        public const string OrderStars = "Stars";	            //评分数
        public const string OrderDigg = "Digg";	            //Digg数
        public const string OrderComments = "Comments";        //评论数
        public const string OrderRandom = "Random";            //随机

        private const string XmlDeclaration = "<?xml version='1.0'?>";

        private const string XmlNamespaceStart = "<stl:document xmlns=\"http://www.siteserver.cn/stl\" xmlns:stl=\"http://www.siteserver.cn/stl\" xmlns:STL=\"http://www.siteserver.cn/stl\" xmlns:sTL=\"http://www.siteserver.cn/stl\" xmlns:stL=\"http://www.siteserver.cn/stl\" xmlns:sTl=\"http://www.siteserver.cn/stl\" xmlns:Stl=\"http://www.siteserver.cn/stl\" xmlns:StL=\"http://www.siteserver.cn/stl\" xmlns:asp=\"http://www.siteserver.cn/stl\" xmlns:ext=\"http://www.siteserver.cn/stl\">";

        private const string XmlNamespaceEnd = "</stl:document>";

        //http://weblogs.asp.net/whaggard/archive/2005/02/20/377025.aspx
        //        public static Regex REGEX_STL_ELEMENT = new Regex(@"
        //<stl:(\w+?)[^>]*>
        //  (?>
        //      <stl:\1[^>]*> (?<LEVEL>)
        //    | 
        //      </stl:\1[^>]*> (?<-LEVEL>)
        //    |
        //      (?! <stl:\1[^>]*> | </stl:\1[^>]*> ).
        //  )*
        //  (?(LEVEL)(?!))
        //</stl:\1[^>]*>
        //", ((RegexOptions.Singleline | RegexOptions.IgnoreCase) | RegexOptions.IgnorePatternWhitespace) | RegexOptions.Compiled);
        public static Regex RegexStlElement = new Regex(@"
<stl:(\w+?)[^>]*>
  (?>
      <stl:\1[^>]*> (?<LEVEL>)
    | 
      </stl:\1[^>]*> (?<-LEVEL>)
    |
      (?! <stl:\1[^>]*> | </stl:\1[^>]*> ).
  )*
  (?(LEVEL)(?!))
</stl:\1[^>]*>|<stl:(\w+?)[^>]*/>
", ((RegexOptions.Singleline | RegexOptions.IgnoreCase) | RegexOptions.IgnorePatternWhitespace) | RegexOptions.Compiled);

        public static string ReplaceXmlNamespace(string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                return content.Replace(@" xmlns=""http://www.siteserver.cn/stl""", string.Empty).Replace(@" xmlns:stl=""http://www.siteserver.cn/stl""", string.Empty);
            }
            return string.Empty;
        }

        public static XmlDocument GetXmlDocument(string element, bool isXmlContent)
        {
            var xmlDocument = new XmlDocument
            {
                PreserveWhitespace = true
            };
            try
            {
                if (isXmlContent)
                {
                    xmlDocument.LoadXml(XmlDeclaration + XmlNamespaceStart + element + XmlNamespaceEnd);
                }
                else
                {
                    xmlDocument.LoadXml(XmlDeclaration + XmlNamespaceStart + HtmlToXml(element) + XmlNamespaceEnd);
                }
            }
            catch
            {
                // ignored
            }
            //catch(Exception e)
            //{
            //    TraceUtils.Warn(e.ToString());
            //    throw e;
            //}
            return xmlDocument;
        }

        //还原Html转换为Xml时无法保留的特定字符
        public static string GetBackHtml(string content, PageInfo pageInfo)
        {
            if (string.IsNullOrEmpty(content)) return string.Empty;

            content = content.Replace(ContentUtility.PagePlaceHolder, string.Empty);
            content = content.Replace("&quot;", "'");
            content = content.Replace("&gt;", ">");
            content = content.Replace("&lt;", "<");
            content = content.Replace("&amp;", "&");
            content = content.Replace(" xmlns=\"http://www.siteserver.cn/stl\"", string.Empty);
            content = content.Replace(" xmlns:stl=\"http://www.siteserver.cn/stl\"", string.Empty);
            content = content.Replace(" xmlns:asp=\"http://www.siteserver.cn/stl\"", string.Empty);
            content = content.Replace("&amp;#", "&#");
            if (pageInfo?.TemplateInfo == null) return content;

            content = content.Replace("<![CDATA[", string.Empty);
            content = content.Replace("]]>", string.Empty);
            return content;
        }

        public static void XmlToHtml(StringBuilder builder)
        {
            builder.Replace("&quot;", "'");
            builder.Replace("&gt;", ">");
            builder.Replace("&lt;", "<");
            builder.Replace("&amp;", "&");
        }

        public static string XmlToHtml(string content)
        {
            if (content != null)
            {
                content = content.Replace("&quot;", "'");
                content = content.Replace("&gt;", ">");
                content = content.Replace("&lt;", "<");
                content = content.Replace("&amp;", "&");
            }
            return content;
        }

        public static string Amp(string content)
        {
            return content?.Replace("&", "&amp;");
        }

        /// <summary>
        /// 将html代码转换为xml代码，需要在try-catch块中调用。
        /// </summary>
        public static string HtmlToXml(string strInputHtml)
        {
            strInputHtml = StringUtils.ReplaceIgnoreCase(strInputHtml, "<br>", "<br />");
            strInputHtml = StringUtils.ReplaceIgnoreCase(strInputHtml, "&#", "&amp;#");
            //strInputHtml = StringUtils.ReplaceNewline(strInputHtml, NEWLINE_REPLACEMENT);
            var reader = new SgmlReader
            {
                DocType = "HTML"
            };
            var sr = new System.IO.StringReader(strInputHtml);
            reader.InputStream = sr;
            var sw = new System.IO.StringWriter();
            var w = new XmlTextWriter(sw);
            reader.Read();
            while (!reader.EOF)
            {
                w.WriteNode(reader, true);
            }
            w.Flush();
            w.Close();
            var xml = sw.ToString();
            //xml = xml.Replace(NEWLINE_REPLACEMENT, "\r\n");
            return xml;
        }

        public static bool IsStlEntityExists(string stlEntityName, string insertedLabelCollection)
        {
            return insertedLabelCollection.IndexOf(stlEntityName.Substring(0, stlEntityName.Length - 1), StringComparison.Ordinal) != -1;
        }

        public static List<string> GetStlLabelList(string templateContent)
        {
            var stlElementList = GetStlElementList(templateContent);
            var stlEntityList = GetStlEntityList(templateContent);
            stlElementList.AddRange(stlEntityList);
            return stlElementList;
        }

        //需要修改
        public static bool IsStlElementExists(string stlElementName, List<string> list)
        {
            var exists = false;
            foreach (var label in list)
            {
                if (label.ToLower().StartsWith($"<{stlElementName.ToLower()} ") || label.ToLower().StartsWith(
                        $"<{stlElementName.ToLower()}>"))
                {
                    exists = true;
                    break;
                }
            }
            return exists;
        }

        public static bool IsStlContentElementWithTypePageContent(List<string> list)
        {
            foreach (var label in list)
            {
                if (!IsStlContentElement(label, BackgroundContentAttribute.PageContent)) continue;
                return true;
            }
            return false;
        }

        public static string GetStlEntity(string stlEntityName, string insertedLabelCollection)
        {
            var stlEntity = string.Empty;
            var labelList = TranslateUtils.StringCollectionToStringList(insertedLabelCollection);
            foreach (var labelWithDisplayModeEnNameAndNodeId in labelList)
            {
                if (labelWithDisplayModeEnNameAndNodeId.StartsWith(stlEntityName.Substring(0, stlEntityName.Length - 1)))
                {
                    stlEntity = labelWithDisplayModeEnNameAndNodeId;
                    break;
                }
            }
            return stlEntity;
        }

        public static string GetStlElement(string stlElementName, List<string> labelList)
        {
            var stlElement = string.Empty;
            foreach (var labelWithDisplayModeEnNameAndNodeId in labelList)
            {
                if (labelWithDisplayModeEnNameAndNodeId.ToLower().StartsWith($"<{stlElementName.ToLower()} ") || labelWithDisplayModeEnNameAndNodeId.ToLower().StartsWith(
                        $"<{stlElementName.ToLower()}>"))
                {
                    stlElement = labelWithDisplayModeEnNameAndNodeId;
                    break;
                }
            }
            return stlElement;
        }

        public static string GetStlContentElementWithTypePageContent(List<string> labelList)
        {
            var stlPageContentElement = string.Empty;
            foreach (var label in labelList)
            {
                if (!IsStlContentElement(label, BackgroundContentAttribute.PageContent)) continue;
                stlPageContentElement = label;
                break;
            }
            return stlPageContentElement;
        }


        public static string GetNameFromEntity(string stlEntity)
        {
            var name = stlEntity;
            if (stlEntity.IndexOf("_", StringComparison.Ordinal) != -1)
            {
                name = stlEntity.Substring(0, stlEntity.IndexOf("_", StringComparison.Ordinal));
                name = name + "}";
            }
            return name;
        }

        public static string GetValueFromEntity(string stlEntity)
        {
            if (stlEntity.IndexOf("_", StringComparison.Ordinal) == -1) return string.Empty;

            var value = string.Empty;
            try
            {
                var length = stlEntity.LastIndexOf("}", StringComparison.Ordinal) - stlEntity.LastIndexOf("_", StringComparison.Ordinal) - 1;
                value = stlEntity.Substring(stlEntity.LastIndexOf("_", StringComparison.Ordinal) + 1, length);
            }
            catch
            {
                // ignored
            }
            return value;
        }

        /// <summary>
        /// 判断此标签是否为Stl实体
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public static bool IsStlEntity(string label)
        {
            if (label == null) return false;
            label = label.ToLower();
            return (label.StartsWith("{stl.") || label.StartsWith("{content.") || label.StartsWith("{channel.")) && label.EndsWith("}");
        }


        public static bool IsStlEntityInclude(string content)
        {
            if (content == null) return false;
            content = content.ToLower();
            return StringUtils.Contains(content, "}") && (StringUtils.Contains(content, "{stl.") || StringUtils.Contains(content, "{content.") || StringUtils.Contains(content, "{channel."));
        }


        public static bool IsSpecifiedStlEntity(EStlEntityType entityType, string stlEntity)
        {
            return stlEntity != null && stlEntity.TrimStart('{').ToLower().StartsWith(EStlEntityTypeUtils.GetValue(entityType).ToLower());
        }


        /// <summary>
        /// 判断此标签是否为Stl元素
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public static bool IsStlElement(string label)
        {
            if (label == null) return false;
            return label.ToLower().StartsWith("<stl:") && label.IndexOf(">", StringComparison.Ordinal) != -1;
        }


        public static bool IsSpecifiedStlElement(string stlElement, string elementName)
        {
            if (stlElement == null) return false;
            return (StringUtils.StartsWithIgnoreCase(stlElement, $"<{elementName} ") ||
                    StringUtils.StartsWithIgnoreCase(stlElement, $"<{elementName}>")) &&
                   (StringUtils.EndsWithIgnoreCase(stlElement, $"</{elementName}>") ||
                    StringUtils.EndsWithIgnoreCase(stlElement, "/>"));
        }

        public static Regex GetStlEntityRegex(string entityName)
        {
            return new Regex($@"{{{entityName}.[^{{}}]*}}", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        /// <summary>
        /// 得到内容中的STL元素列表
        /// </summary>
        public static List<string> GetStlElementList(string templateContent)
        {
            var stlElementList = new List<string>();

            var mc = RegexStlElement.Matches(templateContent);
            for (var i = 0; i < mc.Count; i++)
            {
                var stlElement = mc[i].Value;
                stlElementList.Add(stlElement);
            }

            return stlElementList;
        }

        /// <summary>
        /// 得到内容中的STL实体列表
        /// </summary>
        /// <param name="content">需要解析的内容</param>
        /// <returns></returns>
        public static List<string> GetStlEntityList(string content)
        {
            //首先需要去除<stl:项
            //content = RegexUtils.Replace(@"<stl:(\w+)[^>]*>.*?<\/stl:\1>", content, string.Empty);
            content = RegexStlElement.Replace(content, string.Empty);

            var stlEntityList = new List<string>();

            //Regex regex = new Regex(@"{[^{}]*}", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            //Regex regex = new Regex(@"{stl\.[^{}]*}|{content\.[^{}]*}|{channel\.[^{}]*}|{comment\.[^{}]*}|{request\.[^{}]*}|{sql\.[^{}]*}|{navigation\.[^{}]*}", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var regex = new Regex(EStlEntityTypeUtils.RegexStringAll, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var mc = regex.Matches(content);
            for (var i = 0; i < mc.Count; i++)
            {
                var stlEntity = mc[i].Value;
                stlEntityList.Add(stlEntity);
            }

            return stlEntityList;
        }

        public static List<string> GetStlSqlEntityList(string content)
        {
            //首先需要去除<stl:项
            //content = RegexUtils.Replace(@"<stl:(\w+)[^>]*>.*?<\/stl:\1>", content, string.Empty);
            content = RegexStlElement.Replace(content, string.Empty);

            var stlEntityList = new List<string>();

            //Regex regex = new Regex(@"{[^{}]*}", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var regex = new Regex(EStlEntityTypeUtils.RegexStringSql, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var mc = regex.Matches(content);
            for (var i = 0; i < mc.Count; i++)
            {
                var stlEntity = mc[i].Value;
                stlEntityList.Add(stlEntity);
            }

            return stlEntityList;
        }

        public static List<string> GetStlUserEntityList(string content)
        {
            content = RegexStlElement.Replace(content, string.Empty);

            var stlEntityList = new List<string>();

            var regex = new Regex(EStlEntityTypeUtils.RegexStringUser, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var mc = regex.Matches(content);
            for (var i = 0; i < mc.Count; i++)
            {
                var stlEntity = mc[i].Value;
                stlEntityList.Add(stlEntity);
            }

            return stlEntityList;
        }

        //判断属于某种类型（type）的<stl:content>元素是否存在
        public static bool IsStlContentElement(string labelString, string type)
        {
            return RegexUtils.IsMatch($@"<stl:content[^>]+type=""{type}""[^>]*>", labelString);
        }

        //判断属于某种类型（type）的<stl:channel>元素是否存在
        public static bool IsStlChannelElement(string labelString, string type)
        {
            return RegexUtils.IsMatch($@"<stl:channel[^>]+type=""{type}""[^>]*>", labelString);
        }

        public static string GetInnerXml(string stlElement, bool isInnerElement)
        {
            return GetInnerXml(stlElement, isInnerElement, null);
        }

        public static string GetInnerXml(string stlElement, bool isInnerElement, LowerNameValueCollection attributes)
        {
            var retval = string.Empty;
            try
            {
                var xmlDocument = GetXmlDocument(stlElement, isInnerElement);
                XmlNode node = xmlDocument.DocumentElement;
                if (node != null)
                {
                    node = node.FirstChild;
                    retval = node.InnerXml;

                    if (attributes != null && node.Attributes != null)
                    {
                        foreach (XmlAttribute attribute in node.Attributes)
                        {
                            attributes.Set(attribute.Name, attribute.Value);
                        }
                    }
                }
            }
            catch
            {
                // ignored
            }
            return retval;
        }

        public static string GetAttribute(string stlElement, string attributeName)
        {
            return RegexUtils.GetAttributeContent(attributeName, stlElement);
        }

        public static LowerNameValueCollection GetAttributes(string stlElement, bool isInnerElement)
        {
            var attributes = new LowerNameValueCollection();
            try
            {
                var xmlDocument = GetXmlDocument(stlElement, isInnerElement);
                XmlNode node = xmlDocument.DocumentElement;
                if (node != null)
                {
                    node = node.FirstChild;

                    if (node.Attributes != null)
                    {
                        foreach (XmlAttribute attribute in node.Attributes)
                        {
                            attributes.Set(attribute.Name, attribute.Value);
                        }
                    }
                }
            }
            catch
            {
                // ignored
            }
            return attributes;
        }

        public static NameValueCollection GetStlAttributes(string stlElement)
        {
            var attributes = new NameValueCollection();
            try
            {
                var xmlDocument = GetXmlDocument(stlElement, false);
                XmlNode node = xmlDocument.DocumentElement;
                if (node != null)
                {
                    node = node.FirstChild;

                    if (node.Attributes != null)
                    {
                        foreach (XmlAttribute attribute in node.Attributes)
                        {
                            attributes.Set(attribute.Name, attribute.Value);
                        }
                    }
                }
            }
            catch
            {
                // ignored
            }
            return attributes;
        }

        public const string ItemIndex = "ItemIndex";

        public static int ParseItemIndex(int dbItemIndex, string attributeName, ContextInfo contextInfo)
        {
            var itemIndex = contextInfo.PageItemIndex + dbItemIndex + 1;
            if (attributeName.IndexOf('+') == -1 && attributeName.IndexOf('-') == -1) return itemIndex;

            var array = attributeName.Split('+');
            if (array.Length == 2)
            {
                var addNum = TranslateUtils.ToInt(array[1].Trim(), 1);
                return itemIndex + addNum;
            }

            array = attributeName.Split('-');
            if (array.Length != 2) return itemIndex;

            var substractNum = TranslateUtils.ToInt(array[1].Trim(), 1);
            return itemIndex - substractNum;
        }

        public static int GetItemIndex(ContextInfo contextInfo)
        {
            var dbItemIndex = 0;
            if (contextInfo.ContextType == EContextType.Channel)
            {
                dbItemIndex = contextInfo.ItemContainer.ChannelItem.ItemIndex;
            }
            else if (contextInfo.ContextType == EContextType.Comment)
            {
                dbItemIndex = contextInfo.ItemContainer.CommentItem.ItemIndex;
            }
            else if (contextInfo.ContextType == EContextType.Content)
            {
                dbItemIndex = contextInfo.ItemContainer.ContentItem.ItemIndex;
            }
            else if (contextInfo.ContextType == EContextType.InputContent)
            {
                dbItemIndex = contextInfo.ItemContainer.InputItem.ItemIndex;
            }
            else if (contextInfo.ContextType == EContextType.SqlContent)
            {
                dbItemIndex = contextInfo.ItemContainer.SqlItem.ItemIndex;
            }
            else if (contextInfo.ContextType == EContextType.Site)
            {
                dbItemIndex = contextInfo.ItemContainer.SiteItem.ItemIndex;
            }
            else if (contextInfo.ContextType == EContextType.Photo)
            {
                dbItemIndex = contextInfo.ItemContainer.PhotoItem.ItemIndex;
            }
            else if (contextInfo.ContextType == EContextType.Each)
            {
                dbItemIndex = contextInfo.ItemContainer.EachItem.ItemIndex;
            }

            return contextInfo.PageItemIndex + dbItemIndex + 1;
        }

        public static void GetYesOrNoTemplateString(XmlNode node, PageInfo pageInfo, out string successTemplateString, out string failureTemplateString)
        {
            successTemplateString = string.Empty;
            failureTemplateString = string.Empty;

            if (!string.IsNullOrEmpty(node.InnerXml))
            {
                var stlElementList = GetStlElementList(node.InnerXml);
                if (stlElementList.Count > 0)
                {
                    foreach (var theStlElement in stlElementList)
                    {
                        if (IsSpecifiedStlElement(theStlElement, StlYes.ElementName) || IsSpecifiedStlElement(theStlElement, StlYes.ElementName2))
                        {
                            successTemplateString = GetInnerXml(theStlElement, true);
                        }
                        else if (IsSpecifiedStlElement(theStlElement, StlNo.ElementName) || IsSpecifiedStlElement(theStlElement, StlNo.ElementName2))
                        {
                            failureTemplateString = GetInnerXml(theStlElement, true);
                        }
                    }
                }
                if (string.IsNullOrEmpty(successTemplateString) && string.IsNullOrEmpty(failureTemplateString))
                {
                    successTemplateString = node.InnerXml;
                }
            }
        }

        public static void GetInnerTemplateString(XmlNode node, out string successTemplateString, out string failureTemplateString, PageInfo pageInfo, ContextInfo contextInfo)
        {
            successTemplateString = string.Empty;
            failureTemplateString = string.Empty;

            if (!string.IsNullOrEmpty(node.InnerXml))
            {
                var stlElementList = GetStlElementList(node.InnerXml);
                if (stlElementList.Count > 0)
                {
                    foreach (var theStlElement in stlElementList)
                    {
                        if (IsSpecifiedStlElement(theStlElement, StlYes.ElementName) || IsSpecifiedStlElement(theStlElement, StlYes.ElementName2))
                        {
                            var innerBuilder = new StringBuilder(GetInnerXml(theStlElement, true));
                            StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                            successTemplateString = innerBuilder.ToString();

                        }
                        else if (IsSpecifiedStlElement(theStlElement, StlNo.ElementName) || IsSpecifiedStlElement(theStlElement, StlNo.ElementName2))
                        {
                            var innerBuilder = new StringBuilder(GetInnerXml(theStlElement, true));
                            StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                            failureTemplateString = innerBuilder.ToString();
                        }
                    }
                }
                if (string.IsNullOrEmpty(successTemplateString) && string.IsNullOrEmpty(failureTemplateString))
                {
                    var innerBuilder = new StringBuilder(node.InnerXml);
                    StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                    successTemplateString = innerBuilder.ToString();
                }
            }
        }

        public static void GetSearchOutputTemplateString(XmlNode node, out string loadingTemplateString, out string yesTemplateString, out string noTemplateString, PageInfo pageInfo, ContextInfo contextInfo)
        {
            loadingTemplateString = string.Empty;
            yesTemplateString = string.Empty;
            noTemplateString = string.Empty;

            if (!string.IsNullOrEmpty(node.InnerXml))
            {
                var stlElementList = GetStlElementList(node.InnerXml);
                if (stlElementList.Count > 0)
                {
                    foreach (var theStlElement in stlElementList)
                    {
                        if (IsSpecifiedStlElement(theStlElement, StlLoading.ElementName))
                        {
                            var innerBuilder = new StringBuilder(GetInnerXml(theStlElement, true));
                            StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                            loadingTemplateString = innerBuilder.ToString();
                        }
                        else if (IsSpecifiedStlElement(theStlElement, StlYes.ElementName) || IsSpecifiedStlElement(theStlElement, StlYes.ElementName2))
                        {
                            yesTemplateString = GetInnerXml(theStlElement, true);
                        }
                        else if (IsSpecifiedStlElement(theStlElement, StlNo.ElementName) || IsSpecifiedStlElement(theStlElement, StlNo.ElementName2))
                        {
                            var innerBuilder = new StringBuilder(GetInnerXml(theStlElement, true));
                            StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                            noTemplateString = innerBuilder.ToString();
                        }
                    }
                }

                if (string.IsNullOrEmpty(loadingTemplateString) && string.IsNullOrEmpty(yesTemplateString) && string.IsNullOrEmpty(noTemplateString))
                {
                    yesTemplateString = node.InnerXml;
                }
            }

            loadingTemplateString = StringUtils.Trim(loadingTemplateString);
            yesTemplateString = StringUtils.Trim(yesTemplateString);
            noTemplateString = StringUtils.Trim(noTemplateString);
        }

        public static void GetInnerTemplateStringOfInput(XmlNode node, out string inputTemplateString, out string successTemplateString, out string failureTemplateString, PageInfo pageInfo, ContextInfo contextInfo)
        {
            inputTemplateString = string.Empty;
            successTemplateString = string.Empty;
            failureTemplateString = string.Empty;

            if (!string.IsNullOrEmpty(node.InnerXml))
            {
                var stlElementList = GetStlElementList(node.InnerXml);
                if (stlElementList.Count > 0)
                {
                    foreach (var theStlElement in stlElementList)
                    {
                        if (IsSpecifiedStlElement(theStlElement, StlTemplate.ElementName))
                        {
                            var innerBuilder = new StringBuilder(GetInnerXml(theStlElement, true));
                            StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                            inputTemplateString = innerBuilder.ToString();
                        }
                        else if (IsSpecifiedStlElement(theStlElement, StlYes.ElementName) || IsSpecifiedStlElement(theStlElement, StlYes.ElementName2))
                        {
                            var innerBuilder = new StringBuilder(GetInnerXml(theStlElement, true));
                            StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                            successTemplateString = innerBuilder.ToString();
                        }
                        else if (IsSpecifiedStlElement(theStlElement, StlNo.ElementName) || IsSpecifiedStlElement(theStlElement, StlNo.ElementName2))
                        {
                            var innerBuilder = new StringBuilder(GetInnerXml(theStlElement, true));
                            StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                            failureTemplateString = innerBuilder.ToString();
                        }
                    }
                }
                if (string.IsNullOrEmpty(inputTemplateString) && string.IsNullOrEmpty(successTemplateString) && string.IsNullOrEmpty(failureTemplateString))
                {
                    var innerBuilder = new StringBuilder(node.InnerXml);
                    StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                    inputTemplateString = innerBuilder.ToString();
                }
            }
        }

        public static string GetStlErrorMessage(string elementName, Exception ex)
        {
            LogUtils.AddErrorLog(ex);
            return $"<!-- {elementName} error: {ex.Message} -->";
        }

        public static string GetAjaxDivId(int updaterId)
        {
            return "ajaxElement_" + updaterId + "_" + StringUtils.GetRandomInt(100, 1000);
        }

        public static string GetStlElement(string stlElementName, NameValueCollection attributes, string innerContent)
        {
            if (string.IsNullOrEmpty(innerContent))
            {
                return $@"<{stlElementName} {TranslateUtils.ToAttributesString(attributes)}></{stlElementName}>";
            }
            return $@"<{stlElementName} {TranslateUtils.ToAttributesString(attributes)}>
{innerContent}
</{stlElementName}>";
        }
    }
}
