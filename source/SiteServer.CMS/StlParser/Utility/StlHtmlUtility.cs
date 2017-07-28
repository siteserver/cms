using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using BaiRong.Core;

namespace SiteServer.CMS.StlParser.Utility
{
	public class StlHtmlUtility
	{
        public static string GetStlSubmitButtonElement(string inputTemplate)
        {
            var options = ((RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline) | RegexOptions.IgnoreCase);

            var regex = "<input\\s*[^>]*?id\\s*=\\s*(\"submit\"|\'submit\'|submit).*?>";
            var reg = new Regex(regex, options);
            var match = reg.Match(inputTemplate);
            if (match.Success)
            {
                return match.Value;
            }

            regex = "<\\w+\\s*[^>]*?id\\s*=\\s*(\"submit\"|\'submit\'|submit)[^>]*/\\s*>";
            reg = new Regex(regex, options);
            match = reg.Match(inputTemplate);
            if (match.Success)
            {
                return match.Value;
            }

            regex = "<(\\w+?)\\s*[^>]*?id\\s*=\\s*(\"submit\"|\'submit\'|submit).*?>[^>]*</\\1[^>]*>";
            reg = new Regex(regex, options);
            match = reg.Match(inputTemplate);
            if (match.Success)
            {
                return match.Value;
            }

            return string.Empty;
        }

        public static ArrayList GetStlFormElementsArrayList(string content)
        {
            var arraylist = new ArrayList();

            var options = ((RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline) | RegexOptions.IgnoreCase);

            var regex = "<input\\s*[^>]*?/>|<input\\s*[^>]*?></input>";
            var reg = new Regex(regex, options);
            var mc = reg.Matches(content);
            for (var i = 0; i < mc.Count; i++)
            {
                var element = mc[i].Value;
                arraylist.Add(element);
            }

            regex = "<textarea\\s*[^>]*?/>|<textarea\\s*[^>]*?></textarea>";
            reg = new Regex(regex, options);
            mc = reg.Matches(content);
            for (var i = 0; i < mc.Count; i++)
            {
                var element = mc[i].Value;
                arraylist.Add(element);
            }

            regex = "<select\\s*[^>]*?>.*?</select>";
            reg = new Regex(regex, options);
            mc = reg.Matches(content);
            for (var i = 0; i < mc.Count; i++)
            {
                var element = mc[i].Value;
                arraylist.Add(element);
            }

            return arraylist;
        }

        public static string GetStlInputElementByID(string inputTemplate, string id)
        {
            var options = ((RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline) | RegexOptions.IgnoreCase);

            string regex = $"<input\\s*[^>]*?type\\s*=\\s*(\"{id}\"|\'{id}\'|{id}).*?>";
            var reg = new Regex(regex, options);
            var match = reg.Match(inputTemplate);
            if (match.Success)
            {
                return match.Value;
            }

            regex = $"<\\w+\\s*[^>]*?id\\s*=\\s*(\"{id}\"|\'{id}\'|{id})[^>]*/\\s*>";
            reg = new Regex(regex, options);
            match = reg.Match(inputTemplate);
            if (match.Success)
            {
                return match.Value;
            }

            regex = $"<(\\w+?)\\s*[^>]*?id\\s*=\\s*(\"{id}\"|\'{id}\'|{id}).*?>[^>]*</\\1[^>]*>";
            reg = new Regex(regex, options);
            match = reg.Match(inputTemplate);
            if (match.Success)
            {
                return match.Value;
            }

            return string.Empty;
        }

        public static void ReWriteFormElements(string stlFormElement, out XmlNode elementNode, out NameValueCollection attributes)
        {
            var document = StlParserUtility.GetXmlDocument(stlFormElement, false);
            elementNode = document.DocumentElement;
            elementNode = elementNode.FirstChild;
            var elementIE = elementNode.Attributes.GetEnumerator();
            attributes = new NameValueCollection();
            while (elementIE.MoveNext())
            {
                var attr = (XmlAttribute)elementIE.Current;
                var attributeName = attr.Name;
                if (StringUtils.EqualsIgnoreCase(attributeName, "id") || StringUtils.EqualsIgnoreCase(attributeName, "name"))
                {
                    attributeName = attributeName.ToLower();
                }
                attributes.Add(attributeName, attr.Value);
            }
            //if (!attributes.ContainsKey("name") && !string.IsNullOrEmpty(attributes["id"]))
            //{
            //    attributes.Add("name", attributes["id"]);
            //}
            if (string.IsNullOrEmpty(attributes["name"]) && !string.IsNullOrEmpty(attributes["id"]))
            {
                attributes["name"] = attributes["id"];
            }
        }

        public static void ReWriteSubmitButton(StringBuilder builder, string clickString)
        {
            var submitElement = GetStlSubmitButtonElement(builder.ToString());
            if (!string.IsNullOrEmpty(submitElement))
            {
                var document = StlParserUtility.GetXmlDocument(submitElement, false);
                XmlNode elementNode = document.DocumentElement;
                elementNode = elementNode.FirstChild;
                var elementIE = elementNode.Attributes.GetEnumerator();
                var attributes = new StringDictionary();
                while (elementIE.MoveNext())
                {
                    var attr = (XmlAttribute)elementIE.Current;
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

        public static string ReWriteOnClick(string originalHtml, string id, string clickString)
        {
            var rewriteHtml = originalHtml;
            var submitElement = GetStlInputElementByID(rewriteHtml, id);
            if (!string.IsNullOrEmpty(submitElement))
            {
                var document = StlParserUtility.GetXmlDocument(submitElement, false);
                XmlNode elementNode = document.DocumentElement;
                elementNode = elementNode.FirstChild;
                var elementIE = elementNode.Attributes.GetEnumerator();
                var attributes = new StringDictionary();
                while (elementIE.MoveNext())
                {
                    var attr = (XmlAttribute)elementIE.Current;
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
            return rewriteHtml;
        }
	}
}
