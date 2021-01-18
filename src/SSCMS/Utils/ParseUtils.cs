using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace SSCMS.Utils
{
    public static class ParseUtils
    {
        private const string RegexStringAll = @"{stl\.[^{}]*}|{stl:[^{}]*}|{content\.[^{}]*}|{channel\.[^{}]*}|{comment\.[^{}]*}|{request\.[^{}]*}|{sql\.[^{}]*}|{user\.[^{}]*}|{navigation\.[^{}]*}|{photo\.[^{}]*}";

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
        public static readonly Regex RegexStlElement = new Regex(@"
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

        public static List<string> GetStlLabels(string templateContent)
        {
            var stlElements = GetStlElements(templateContent);
            var stlEntities = GetStlEntities(templateContent);
            stlElements.AddRange(stlEntities);
            return stlElements;
        }

        /// <summary>
        /// 得到内容中的STL元素列表
        /// </summary>
        public static List<string> GetStlElements(string templateContent)
        {
            var elements = new List<string>();

            var mc = RegexStlElement.Matches(templateContent);
            for (var i = 0; i < mc.Count; i++)
            {
                var stlElement = mc[i].Value;
                elements.Add(stlElement);
            }

            return elements;
        }

        /// <summary>
        /// 得到内容中的STL实体列表
        /// </summary>
        /// <param name="content">需要解析的内容</param>
        /// <returns></returns>
        public static List<string> GetStlEntities(string content)
        {
            //首先需要去除<stl:项
            //content = RegexUtils.Replace(@"<stl:(\w+)[^>]*>.*?<\/stl:\1>", content, string.Empty);
            content = RegexStlElement.Replace(content, String.Empty);

            var entities = new List<string>();

            //Regex regex = new Regex(@"{[^{}]*}", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            //Regex regex = new Regex(@"{stl\.[^{}]*}|{content\.[^{}]*}|{channel\.[^{}]*}|{comment\.[^{}]*}|{request\.[^{}]*}|{sql\.[^{}]*}|{navigation\.[^{}]*}", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var regex = new Regex(RegexStringAll, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var mc = regex.Matches(content);
            for (var i = 0; i < mc.Count; i++)
            {
                var stlEntity = mc[i].Value;
                entities.Add(stlEntity);
            }

            return entities;
        }

        public static bool IsStlElementExists(string stlElementName, List<string> list)
        {
            return list.Any(label => StringUtils.StartsWithIgnoreCase(label, $"<{stlElementName} ") || StringUtils.StartsWithIgnoreCase(label, $"<{stlElementName}>"));
        }

        public static string GetStlElement(string stlElementName, List<string> labelList)
        {
            var stlElement = string.Empty;
            foreach (var label in labelList)
            {
                if (StringUtils.StartsWithIgnoreCase(label, $"<{stlElementName} ") ||
                    StringUtils.StartsWithIgnoreCase(label,
                        $"<{stlElementName}>"))
                {
                    stlElement = label;
                    break;
                }
            }
            return stlElement;
        }

        public static bool IsSpecifiedStlElement(string stlElement, string elementName)
        {
            if (stlElement == null) return false;
            return (StringUtils.StartsWithIgnoreCase(stlElement, $"<{elementName} ") ||
                    StringUtils.StartsWithIgnoreCase(stlElement, $"<{elementName}>")) &&
                   (StringUtils.EndsWithIgnoreCase(stlElement, $"</{elementName}>") ||
                    StringUtils.EndsWithIgnoreCase(stlElement, "/>"));
        }

        public static string GetInnerHtml(string stlElement)
        {
            var innerHtml = string.Empty;

            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(stlElement);
                var docNode = htmlDoc.DocumentNode;
                if (docNode?.FirstChild != null)
                {
                    var stlNode = docNode.FirstChild;

                    innerHtml = stlNode.InnerHtml;
                }
            }
            catch
            {
                // ignored
            }

            return innerHtml;
        }

        public static NameValueCollection GetAttributes(string stlElement)
        {
            var attributes = new NameValueCollection();

            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(stlElement);
                var docNode = htmlDoc.DocumentNode;
                var stlNode = docNode?.FirstChild;

                if (stlNode?.Attributes != null)
                {
                    foreach (var attribute in stlNode.Attributes)
                    {
                        attributes[attribute.Name] = attribute.Value;
                    }
                }
            }
            catch
            {
                // ignored
            }

            return attributes;
        }

        public static (string innerHtml, NameValueCollection attributes) GetInnerHtmlAndAttributes(string stlElement)
        {
            var innerHtml = string.Empty;
            var attributes = new NameValueCollection();

            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(stlElement);
                var docNode = htmlDoc.DocumentNode;
                if (docNode?.FirstChild != null)
                {
                    var stlNode = docNode.FirstChild;

                    innerHtml = stlNode.InnerHtml;

                    if (stlNode.Attributes != null)
                    {
                        foreach (var attribute in stlNode.Attributes)
                        {
                            attributes[attribute.Name] = attribute.Value;
                        }
                    }
                }
            }
            catch
            {
                // ignored
            }

            return (innerHtml, attributes);
        }

        public static NameValueCollection GetInnerStlElements(string innerHtml, params string[] elementNames)
        {
            var elements = new NameValueCollection();
            foreach (var elementName in elementNames)
            {
                elements[elementName] = String.Empty;
            }

            if (String.IsNullOrEmpty(innerHtml)) return elements;

            var stlElementList = GetStlElements(innerHtml);
            if (stlElementList.Count > 0)
            {
                foreach (var theStlElement in stlElementList)
                {
                    foreach (var elementName in elementNames)
                    {
                        if (IsSpecifiedStlElement(theStlElement, elementName))
                        {
                            elements[elementName] = StringUtils.Trim(GetInnerHtml(theStlElement));
                            break;
                        }
                    }
                }
            }

            return elements;
        }
    }
}
