using System.Collections.Generic;
using System.Xml;
using SSCMS.Utils;

namespace SSCMS.Core.Utils
{
    public static class OnlineTemplateManager
    {
        private const string UrlTemplatesXml = "https://www.siteserver.cn/templates/templates.xml";

        public const string UrlHome = "https://www.siteserver.cn/templates/";

        public static bool TryGetOnlineTemplates(out List<Dictionary<string, string>> list)
        {
            list = new List<Dictionary<string, string>>();

            try
            {
                var content = WebClientUtils.GetHtml(UrlTemplatesXml);

                var document = XmlUtils.GetXmlDocument(content);
                var rootNode = XmlUtils.GetXmlNode(document, "//siteTemplates");
                if (rootNode.ChildNodes.Count > 0)
                {
                    foreach (XmlNode node in rootNode.ChildNodes)
                    {
                        var ie = node.ChildNodes.GetEnumerator();
                        var title = string.Empty;
                        var description = string.Empty;
                        var author = string.Empty;
                        var source = string.Empty;
                        var lastEditDate = string.Empty;

                        while (ie.MoveNext())
                        {
                            var childNode = (XmlNode)ie.Current;
                            if (childNode == null) continue;

                            var nodeName = childNode.Name;
                            if (StringUtils.EqualsIgnoreCase(nodeName, "title"))
                            {
                                title = childNode.InnerText;
                            }
                            else if (StringUtils.EqualsIgnoreCase(nodeName, "description"))
                            {
                                description = childNode.InnerText;
                            }
                            else if (StringUtils.EqualsIgnoreCase(nodeName, "author"))
                            {
                                author = childNode.InnerText;
                            }
                            else if (StringUtils.EqualsIgnoreCase(nodeName, "source"))
                            {
                                source = childNode.InnerText;
                            }
                            else if (StringUtils.EqualsIgnoreCase(nodeName, "lastEditDate"))
                            {
                                lastEditDate = childNode.InnerText;
                            }
                        }

                        if (!string.IsNullOrEmpty(title))
                        {
                            list.Add(new Dictionary<string, string>
                            {
                                ["title"] = StringUtils.Trim(title),
                                ["description"] = StringUtils.Trim(description),
                                ["author"] = StringUtils.Trim(author),
                                ["source"] = StringUtils.Trim(source),
                                ["lastEditDate"] = StringUtils.Trim(lastEditDate)
                            });
                        }
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string GetTemplateUrl(string name)
        {
            return $"https://www.siteserver.cn/templates/t-{StringUtils.ToLower(name)}/index.html";
        }

        public static string GetDownloadUrl(string name)
        {
            return $"https://api.siteserver.cn/downloads/template/{name}";
        }
    }
}
