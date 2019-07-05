using System.Collections.Generic;
using System.Xml;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Core
{
    public static class OnlineTemplateManager
    {
        private const string UrlTemplatesXml = "http://templates.siteserver.cn/templates.xml";

        public const string UrlHome = "http://templates.siteserver.cn";

        public static bool TryGetOnlineTemplates(out List<Dictionary<string, string>> list)
        {
            list = new List<Dictionary<string, string>>();

            try
            {
                var content = WebClientUtils.GetRemoteFileSource(UrlTemplatesXml, ECharset.utf_8);

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
                            var childNode = (XmlNode) ie.Current;
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
            return $"http://templates.siteserver.cn/t-{name.ToLower()}/index.html";
        }

        public static string GetDownloadUrl(string name)
        {
            return $"https://api.siteserver.cn/downloads/template/{name}";
        }
    }
}
