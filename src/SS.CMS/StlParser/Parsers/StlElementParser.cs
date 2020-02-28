using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SS.CMS;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Parse;
using SS.CMS.StlParser.Model;
using SS.CMS.StlParser.StlElement;
using SS.CMS.StlParser.Utility;
using SS.CMS.Core;
using SS.CMS.Plugins;
using SS.CMS.Plugins.Impl;

namespace SS.CMS.StlParser.Parsers
{
    /// <summary>
    /// Stl元素解析器
    /// </summary>
    public static class StlElementParser
    {
        /// <summary>
        /// 将原始内容中的STL元素替换为实际内容
        /// </summary>
        public static async Task ReplaceStlElementsAsync(StringBuilder parsedBuilder, ParsePage pageInfo, ParseContext contextInfo)
        {
            var stlElements = StlParserUtility.GetStlElementList(parsedBuilder.ToString());
            foreach (var stlElement in stlElements)
            {
                try
                {
                    var startIndex = parsedBuilder.ToString().IndexOf(stlElement, StringComparison.Ordinal);
                    if (startIndex == -1) continue;

                    var parsedContent = await ParseStlElementAsync(stlElement, pageInfo, contextInfo);
                    parsedBuilder.Replace(stlElement, parsedContent, startIndex, stlElement.Length);
                }
                catch
                {
                    // ignored
                }
            }
        }

        public static readonly Dictionary<string, Func<ParsePage, ParseContext, Task<object>>> ElementsToParseDic = new Dictionary<string, Func<ParsePage, ParseContext, Task<object>>>
        {
            {StlA.ElementName.ToLower(), StlA.ParseAsync},
            {StlAction.ElementName.ToLower(), StlAction.ParseAsync},
            {StlAudio.ElementName.ToLower(), StlAudio.ParseAsync},
            {StlChannel.ElementName.ToLower(), StlChannel.ParseAsync},
            {StlChannels.ElementName.ToLower(), StlChannels.ParseAsync},
            {StlContainer.ElementName.ToLower(), StlContainer.ParseAsync},
            {StlContent.ElementName.ToLower(), StlContent.ParseAsync},
            {StlContents.ElementName.ToLower(), StlContents.ParseAsync},
            {StlCount.ElementName.ToLower(), StlCount.ParseAsync},
            {StlDynamic.ElementName.ToLower(), StlDynamic.ParseAsync},
            {StlEach.ElementName.ToLower(), StlEach.ParseAsync},
            {StlFile.ElementName.ToLower(), StlFile.ParseAsync},
            {StlFlash.ElementName.ToLower(), StlFlash.ParseAsync},
            {StlFocusViewer.ElementName.ToLower(), StlFocusViewer.ParseAsync},
            {StlIf.ElementName.ToLower(), StlIf.ParseAsync},
            {StlImage.ElementName.ToLower(), StlImage.ParseAsync},
            {StlInclude.ElementName.ToLower(), StlInclude.ParseAsync},
            {StlLibrary.ElementName.ToLower(), StlLibrary.ParseAsync},
            {StlLocation.ElementName.ToLower(), StlLocation.ParseAsync},
            {StlMarquee.ElementName.ToLower(), StlMarquee.ParseAsync},
            {StlNavigation.ElementName.ToLower(), StlNavigation.ParseAsync},
            {StlPlayer.ElementName.ToLower(), StlPlayer.ParseAsync},
            {StlPrinter.ElementName.ToLower(), StlPrinter.ParseAsync},
            {StlSearch.ElementName.ToLower(), StlSearch.ParseAsync},
            {StlSearch.ElementName2.ToLower(), StlSearch.ParseAsync},
            {StlSelect.ElementName.ToLower(), StlSelect.ParseAsync},
            {StlSite.ElementName.ToLower(), StlSite.ParseAsync},
            {StlSites.ElementName.ToLower(), StlSites.ParseAsync},
            {StlSqlContent.ElementName.ToLower(), StlSqlContent.ParseAsync},
            {StlSqlContents.ElementName.ToLower(), StlSqlContents.ParseAsync},
            {StlTabs.ElementName.ToLower(), StlTabs.ParseAsync},
            {StlValue.ElementName.ToLower(), StlValue.ParseAsync},
            {StlVideo.ElementName.ToLower(), StlVideo.ParseAsync},
            {StlZoom.ElementName.ToLower(), StlZoom.ParseAsync}
        };

        private static readonly Dictionary<string, Func<string, string>> ElementsToTranslateDic = new Dictionary<string, Func<string, string>>
        {
            {StlPageContents.ElementName.ToLower(), StlParserManager.StlEncrypt},
            {StlPageChannels.ElementName.ToLower(), StlParserManager.StlEncrypt},
            {StlPageSqlContents.ElementName.ToLower(), StlParserManager.StlEncrypt},
            //{StlPageInputContents.ElementName.ToLower(), StlParserManager.StlEncrypt},
            {StlPageItems.ElementName.ToLower(), StlParserManager.StlEncrypt}
        };

        private static async Task<string> ParseStlElementAsync(string stlElement, ParsePage pageInfo, ParseContext contextInfo)
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
                        parsedContent = await StlDynamic.ParseDynamicElementAsync(stlElement, pageInfo, contextInfo);
                    }
                    else
                    {
                        try
                        {
                            if (ElementsToParseDic.TryGetValue(elementName, out var func))
                            {
                                var contextInfoClone = contextInfo.Clone(stlElement, stlElementInfo.InnerHtml, stlElementInfo.Attributes);

                                var obj = await func(pageInfo, contextInfoClone);

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
                            parsedContent = await LogUtils.AddStlErrorLogAsync(pageInfo, elementName, stlElement, ex);
                        }
                    }
                }
                else
                {
                    var parsers = await PluginStlParserContentManager.GetParsesAsync();
                    if (parsers.ContainsKey(elementName))
                    {
                        if (stlElementInfo.IsDynamic)
                        {
                            parsedContent = await StlDynamic.ParseDynamicElementAsync(stlElement, pageInfo, contextInfo);
                        }
                        else
                        {
                            try
                            {
                                if (parsers.TryGetValue(elementName, out var func))
                                {
                                    var context = new ParseContextImpl();
                                    await context.LoadAsync(stlElementInfo.OuterHtml, stlElementInfo.InnerHtml,
                                        stlElementInfo.Attributes, pageInfo, contextInfo);
                                    parsedContent = func(context);
                                }
                            }
                            catch (Exception ex)
                            {
                                parsedContent = await LogUtils.AddStlErrorLogAsync(pageInfo, elementName, stlElement, ex);
                            }
                        }
                    }
                }
            }

            return parsedContent ?? stlElement;
        }
    }
}
