using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using BaiRong.Core;

namespace SiteServer.CMS.StlParser.Utility
{
	public class StlHtmlUtility
	{
        public static string GetHtmlElementById(string html, string id)
        {
            const RegexOptions options = RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline | RegexOptions.IgnoreCase;

            var regex = $"<input\\s*[^>]*?id\\s*=\\s*(\"{id}\"|\'{id}\'|{id}).*?>";
            var reg = new Regex(regex, options);
            var match = reg.Match(html);
            if (match.Success)
            {
                return match.Value;
            }

            regex = $"<\\w+\\s*[^>]*?id\\s*=\\s*(\"{id}\"|\'{id}\'|{id})[^>]*/\\s*>";
            reg = new Regex(regex, options);
            match = reg.Match(html);
            if (match.Success)
            {
                return match.Value;
            }

            regex = $"<(\\w+?)\\s*[^>]*?id\\s*=\\s*(\"{id}\"|\'{id}\'|{id}).*?>[^>]*</\\1[^>]*>";
            reg = new Regex(regex, options);
            match = reg.Match(html);
            if (match.Success)
            {
                return match.Value;
            }

            return string.Empty;
        }

        public static string GetHtmlElementBySRole(string html, string sRole)
        {
            const RegexOptions options = RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline | RegexOptions.IgnoreCase;

            var regex = $"<input\\s*[^>]*?s-role\\s*=\\s*(\"{sRole}\"|\'{sRole}\'|{sRole}).*?>";
            var reg = new Regex(regex, options);
            var match = reg.Match(html);
            if (match.Success)
            {
                return match.Value;
            }

            regex = $"<\\w+\\s*[^>]*?s-role\\s*=\\s*(\"{sRole}\"|\'{sRole}\'|{sRole})[^>]*/\\s*>";
            reg = new Regex(regex, options);
            match = reg.Match(html);
            if (match.Success)
            {
                return match.Value;
            }

            regex = $"<(\\w+?)\\s*[^>]*?s-role\\s*=\\s*(\"{sRole}\"|\'{sRole}\'|{sRole}).*?>[^>]*</\\1[^>]*>";
            reg = new Regex(regex, options);
            match = reg.Match(html);
            if (match.Success)
            {
                return match.Value;
            }

            return string.Empty;
        }

        public static List<string> GetHtmlFormElements(string content)
        {
            var list = new List<string>();

            const RegexOptions options = RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline | RegexOptions.IgnoreCase;

            var regex = "<input\\s*[^>]*?/>|<input\\s*[^>]*?>[^>]*?</input>";
            var reg = new Regex(regex, options);
            var mc = reg.Matches(content);
            for (var i = 0; i < mc.Count; i++)
            {
                var element = mc[i].Value;
                list.Add(element);
            }

            regex = "<textarea\\s*[^>]*?/>|<textarea\\s*[^>]*?>[^>]*?</textarea>";
            reg = new Regex(regex, options);
            mc = reg.Matches(content);
            for (var i = 0; i < mc.Count; i++)
            {
                var element = mc[i].Value;
                list.Add(element);
            }

            regex = "<select\\b[\\s\\S]*?</select>";
            reg = new Regex(regex, options);
            mc = reg.Matches(content);
            for (var i = 0; i < mc.Count; i++)
            {
                var element = mc[i].Value;
                list.Add(element);
            }

            return list;
        }

        public static void ParseHtmlElement(string htmlElement, out string tagName, out string innerXml, out NameValueCollection attributes)
        {
            tagName = string.Empty;
            innerXml = string.Empty;
            attributes = new NameValueCollection();

            var document = StlParserUtility.GetXmlDocument(htmlElement, false);
            XmlNode elementNode = document.DocumentElement;
            if (elementNode == null) return;

            elementNode = elementNode.FirstChild;
            tagName = elementNode.Name;
            innerXml = elementNode.InnerXml;
            if (elementNode.Attributes == null) return;

            var elementIe = elementNode.Attributes.GetEnumerator();
            while (elementIe.MoveNext())
            {
                var attr = (XmlAttribute)elementIe.Current;
                var attributeName = attr.Name;
                attributes.Add(attributeName, attr.Value);
            }
        }

        public static void RewriteSubmitButton(StringBuilder builder, string clickString)
        {
            var submitElement = GetHtmlElementBySRole(builder.ToString(), "submit");
            if (string.IsNullOrEmpty(submitElement))
            {
                submitElement = GetHtmlElementById(builder.ToString(), "submit");
            }
            if (!string.IsNullOrEmpty(submitElement))
            {
                var document = StlParserUtility.GetXmlDocument(submitElement, false);
                XmlNode elementNode = document.DocumentElement;
                if (elementNode != null)
                {
                    elementNode = elementNode.FirstChild;
                    if (elementNode.Attributes != null)
                    {
                        var elementIe = elementNode.Attributes.GetEnumerator();
                        var attributes = new StringDictionary();
                        while (elementIe.MoveNext())
                        {
                            var attr = (XmlAttribute)elementIe.Current;
                            var attributeName = attr.Name.ToLower();
                            if (attributeName == "href")
                            {
                                attributes.Add(attr.Name, PageUtils.UnclickedUrl);
                            }
                            else if (attributeName != "onclick")
                            {
                                attributes.Add(attr.Name, attr.Value);
                            }
                        }
                        attributes.Add("onclick", clickString);
                        attributes.Remove("id");
                        attributes.Remove("name");

                        //attributes.Add("id", "submit_" + styleID);

                        if (StringUtils.EqualsIgnoreCase(elementNode.Name, "a"))
                        {
                            attributes.Remove("href");
                            attributes.Add("href", PageUtils.UnclickedUrl);
                        }

                        if (!string.IsNullOrEmpty(elementNode.InnerXml))
                        {
                            builder.Replace(submitElement,
                                $@"<{elementNode.Name} {TranslateUtils.ToAttributesString(attributes)}>{elementNode.InnerXml}</{elementNode
                                    .Name}>");
                        }
                        else
                        {
                            builder.Replace(submitElement,
                                $@"<{elementNode.Name} {TranslateUtils.ToAttributesString(attributes)}/>");
                        }
                    }
                }
            }
        }

        public static string RewriteOnClick(string originalHtml, string id, string clickString)
        {
            var rewriteHtml = originalHtml;
            var submitElement = GetHtmlElementById(rewriteHtml, id);
            if (!string.IsNullOrEmpty(submitElement))
            {
                var document = StlParserUtility.GetXmlDocument(submitElement, false);
                XmlNode elementNode = document.DocumentElement;
                if (elementNode != null)
                {
                    elementNode = elementNode.FirstChild;
                    if (elementNode.Attributes != null)
                    {
                        var elementIe = elementNode.Attributes.GetEnumerator();
                        var attributes = new StringDictionary();
                        while (elementIe.MoveNext())
                        {
                            var attr = (XmlAttribute)elementIe.Current;
                            var attributeName = attr.Name.ToLower();
                            if (attributeName == "href")
                            {
                                attributes.Add(attr.Name, PageUtils.UnclickedUrl);
                            }
                            else if (attributeName != "onclick")
                            {
                                attributes.Add(attr.Name, attr.Value);
                            }
                        }
                        attributes.Add("onclick", clickString);
                        attributes.Remove("id");
                        attributes.Remove("name");
                        if (StringUtils.EqualsIgnoreCase(elementNode.Name, "a"))
                        {
                            attributes.Remove("href");
                            attributes.Add("href", PageUtils.UnclickedUrl);
                        }

                        if (!string.IsNullOrEmpty(elementNode.InnerXml))
                        {
                            rewriteHtml = rewriteHtml.Replace(submitElement,
                                $@"<{elementNode.Name} {TranslateUtils.ToAttributesString(attributes)}>{elementNode.InnerXml}</{elementNode
                                    .Name}>");
                        }
                        else
                        {
                            rewriteHtml = rewriteHtml.Replace(submitElement,
                                $@"<{elementNode.Name} {TranslateUtils.ToAttributesString(attributes)}/>");
                        }
                    }
                }
            }
            return rewriteHtml;
        }
	}
}
