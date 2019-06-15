using System;
using System.Collections.Generic;
using System.Text;
using SS.CMS.Core.Common;
using SS.CMS.Core.Plugin;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Core.StlParser.StlElement;
using SS.CMS.Core.StlParser.Utility;
using SS.CMS.Utils;

namespace SS.CMS.Core.StlParser
{
    /// <summary>
    /// Stl元素解析器
    /// </summary>
    public partial class ParseContext
    {
        /// <summary>
        /// 将原始内容中的STL元素替换为实际内容
        /// </summary>
        public void ReplaceStlElements(StringBuilder parsedBuilder)
        {
            var stlElements = StlParserUtility.GetStlElementList(parsedBuilder.ToString());
            foreach (var stlElement in stlElements)
            {
                try
                {
                    var startIndex = parsedBuilder.ToString().IndexOf(stlElement, StringComparison.Ordinal);
                    if (startIndex == -1) continue;

                    var parsedContent = ParseStlElement(stlElement);
                    parsedBuilder.Replace(stlElement, parsedContent, startIndex, stlElement.Length);
                }
                catch
                {
                    // ignored
                }
            }
        }

        public static readonly Dictionary<string, Func<ParseContext, object>> ElementsToParseDic = new Dictionary<string, Func<ParseContext, object>>
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

        private static readonly IList<string> ElementsToEncrypt = new List<string>
        {
            {StlPageContents.ElementName.ToLower()},
            {StlPageChannels.ElementName.ToLower()},
            {StlPageSqlContents.ElementName.ToLower()},
            {StlPageItems.ElementName.ToLower()}
        };

        private string ParseStlElement(string stlElement)
        {
            string parsedContent = null;

            var stlElementInfo = StlParserUtility.ParseStlElement(stlElement);

            if (stlElementInfo != null)
            {
                var elementName = stlElementInfo.Name;

                if (ElementsToEncrypt.Contains(elementName))
                {
                    parsedContent = StlEncrypt(stlElement);
                }
                else if (ElementsToParseDic.ContainsKey(elementName))
                {
                    if (stlElementInfo.IsDynamic)
                    {
                        parsedContent = StlDynamic.ParseDynamicElement(stlElement, this);
                    }
                    else
                    {
                        try
                        {
                            if (ElementsToParseDic.TryGetValue(elementName, out var func))
                            {
                                var contextClone = Clone(stlElement, stlElementInfo.InnerHtml, stlElementInfo.Attributes);

                                var obj = func(contextClone);

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
                            parsedContent = LogUtils.AddStlErrorLog(PageInfo, elementName, stlElement, ex);
                        }
                    }
                }
                else
                {
                    var parsers = PluginManager.GetParses();
                    if (parsers.ContainsKey(elementName))
                    {
                        if (stlElementInfo.IsDynamic)
                        {
                            parsedContent = StlDynamic.ParseDynamicElement(stlElement, this);
                        }
                        else
                        {
                            try
                            {
                                if (parsers.TryGetValue(elementName, out var func))
                                {
                                    var context = new ParseContextImpl(stlElementInfo.OuterHtml, stlElementInfo.InnerHtml, stlElementInfo.Attributes, PageInfo, this);
                                    parsedContent = func(context);
                                }
                            }
                            catch (Exception ex)
                            {
                                parsedContent = LogUtils.AddStlErrorLog(PageInfo, elementName, stlElement, ex);
                            }
                        }
                    }
                }
            }

            return parsedContent ?? stlElement;
        }
    }
}
