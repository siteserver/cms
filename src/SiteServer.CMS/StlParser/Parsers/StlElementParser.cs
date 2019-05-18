using System;
using System.Collections.Generic;
using System.Text;
using SiteServer.CMS.Core;
using SiteServer.Utils;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.Parsers
{
    /// <summary>
    /// Stl元素解析器
    /// </summary>
    public static class StlElementParser
    {
        /// <summary>
        /// 将原始内容中的STL元素替换为实际内容
        /// </summary>
        public static void ReplaceStlElements(StringBuilder parsedBuilder, PageInfo pageInfo, ContextInfo contextInfo)
        {
            var stlElements = StlParserUtility.GetStlElementList(parsedBuilder.ToString());
            foreach (var stlElement in stlElements)
            {
                try
                {
                    var startIndex = parsedBuilder.ToString().IndexOf(stlElement, StringComparison.Ordinal);
                    if (startIndex == -1) continue;

                    var parsedContent = ParseStlElement(stlElement, pageInfo, contextInfo);
                    parsedBuilder.Replace(stlElement, parsedContent, startIndex, stlElement.Length);
                }
                catch
                {
                    // ignored
                }
            }
        }

        public static readonly Dictionary<string, Func<PageInfo, ContextInfo, object>> ElementsToParseDic = new Dictionary<string, Func<PageInfo, ContextInfo, object>>
        {
            {StlA.ElementName.ToLower(), StlA.Parse},
            {StlAction.ElementName.ToLower(), StlAction.Parse},
            {StlAudio.ElementName.ToLower(), StlAudio.Parse},
            {StlChannel.ElementName.ToLower(), StlChannel.Parse},
            {StlChannels.ElementName.ToLower(), StlChannels.Parse},
            {StlContainer.ElementName.ToLower(), StlContainer.Parse},
            {StlContent.ElementName.ToLower(), StlContent.Parse},
            {StlContents.ElementName.ToLower(), StlContents.Parse},
            {StlCount.ElementName.ToLower(), StlCount.Parse},
            {StlDynamic.ElementName.ToLower(), StlDynamic.Parse},
            {StlEach.ElementName.ToLower(), StlEach.Parse},
            {StlFile.ElementName.ToLower(), StlFile.Parse},
            {StlFlash.ElementName.ToLower(), StlFlash.Parse},
            {StlFocusViewer.ElementName.ToLower(), StlFocusViewer.Parse},
            {StlIf.ElementName.ToLower(), StlIf.Parse},
            {StlImage.ElementName.ToLower(), StlImage.Parse},
            {StlInclude.ElementName.ToLower(), StlInclude.Parse},
            {StlLocation.ElementName.ToLower(), StlLocation.Parse},
            {StlMarquee.ElementName.ToLower(), StlMarquee.Parse},
            {StlNavigation.ElementName.ToLower(), StlNavigation.Parse},
            {StlPlayer.ElementName.ToLower(), StlPlayer.Parse},
            {StlPrinter.ElementName.ToLower(), StlPrinter.Parse},
            {StlRss.ElementName.ToLower(), StlRss.Parse},
            {StlSearch.ElementName.ToLower(), StlSearch.Parse},
            {StlSearch.ElementName2.ToLower(), StlSearch.Parse},
            {StlSelect.ElementName.ToLower(), StlSelect.Parse},
            {StlSite.ElementName.ToLower(), StlSite.Parse},
            {StlSites.ElementName.ToLower(), StlSites.Parse},
            {StlSqlContent.ElementName.ToLower(), StlSqlContent.Parse},
            {StlSqlContents.ElementName.ToLower(), StlSqlContents.Parse},
            {StlTabs.ElementName.ToLower(), StlTabs.Parse},
            {StlTags.ElementName.ToLower(), StlTags.Parse},
            {StlTree.ElementName.ToLower(), StlTree.Parse},
            {StlValue.ElementName.ToLower(), StlValue.Parse},
            {StlVideo.ElementName.ToLower(), StlVideo.Parse},
            {StlZoom.ElementName.ToLower(), StlZoom.Parse}
        };

        private static readonly Dictionary<string, Func<string, string>> ElementsToTranslateDic = new Dictionary<string, Func<string, string>>
        {
            {StlPageContents.ElementName.ToLower(), StlParserManager.StlEncrypt},
            {StlPageChannels.ElementName.ToLower(), StlParserManager.StlEncrypt},
            {StlPageSqlContents.ElementName.ToLower(), StlParserManager.StlEncrypt},
            //{StlPageInputContents.ElementName.ToLower(), StlParserManager.StlEncrypt},
            {StlPageItems.ElementName.ToLower(), StlParserManager.StlEncrypt}
        };

        private static string ParseStlElement(string stlElement, PageInfo pageInfo, ContextInfo contextInfo)
        {
            string parsedContent = null;

            var stlElementInfo = StlParserUtility.ParseStlElement(stlElement);

            if (stlElementInfo != null)
            {
                var elementName = stlElementInfo.Name;

                if (ElementsToTranslateDic.ContainsKey(elementName))
                {
                    if (ElementsToTranslateDic.TryGetValue(elementName, out var func))
                    {
                        parsedContent = func(stlElement);
                    }
                }
                else if (ElementsToParseDic.ContainsKey(elementName))
                {
                    if (stlElementInfo.IsDynamic)
                    {
                        parsedContent = StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo);
                    }
                    else
                    {
                        try
                        {
                            if (ElementsToParseDic.TryGetValue(elementName, out var func))
                            {
                                var contextInfoClone = contextInfo.Clone(stlElement, stlElementInfo.InnerHtml, stlElementInfo.Attributes);

                                var obj = func(pageInfo, contextInfoClone);

                                if (obj == null)
                                {
                                    parsedContent = string.Empty;
                                }
                                else if (obj is string)
                                {
                                    parsedContent = (string)obj;
                                }
                                else
                                {
                                    parsedContent = TranslateUtils.JsonSerialize(obj);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            parsedContent = LogUtils.AddStlErrorLog(pageInfo, elementName, stlElement, ex);
                        }
                    }
                }
                else
                {
                    var parsers = PluginStlParserContentManager.GetParses();
                    if (parsers.ContainsKey(elementName))
                    {
                        if (stlElementInfo.IsDynamic)
                        {
                            parsedContent = StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo);
                        }
                        else
                        {
                            try
                            {
                                if (parsers.TryGetValue(elementName, out var func))
                                {
                                    var context = new ParseContextImpl(stlElementInfo.OuterHtml, stlElementInfo.InnerHtml, stlElementInfo.Attributes, pageInfo, contextInfo);
                                    parsedContent = func(context);
                                }
                            }
                            catch (Exception ex)
                            {
                                parsedContent = LogUtils.AddStlErrorLog(pageInfo, elementName, stlElement, ex);
                            }
                        }
                    }
                }
            }

            return parsedContent ?? stlElement;
        }
    }
}
