using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SS.CMS.Abstractions;
using SS.CMS.Core.Plugins;
using SS.CMS.StlParser.StlElement;
using SS.CMS.StlParser.Utility;

namespace SS.CMS.Services
{
    public partial class ParseManager
    {
        /// <summary>
        /// 将原始内容中的STL元素替换为实际内容
        /// </summary>
        public async Task ReplaceStlElementsAsync(StringBuilder parsedBuilder)
        {
            var stlElements = StlParserUtility.GetStlElementList(parsedBuilder.ToString());
            foreach (var stlElement in stlElements)
            {
                try
                {
                    var startIndex = parsedBuilder.ToString().IndexOf(stlElement, StringComparison.Ordinal);
                    if (startIndex == -1) continue;

                    var parsedContent = await ParseStlElementAsync(stlElement);
                    parsedBuilder.Replace(stlElement, parsedContent, startIndex, stlElement.Length);
                }
                catch
                {
                    // ignored
                }
            }
        }

        public Dictionary<string, Func<IParseManager, Task<object>>> ElementsToParseDic => new Dictionary<string, Func<IParseManager, Task<object>>>
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

        private Dictionary<string, Func<string, string>> ElementsToTranslateDic => new Dictionary<string, Func<string, string>>
        {
            {StlPageContents.ElementName.ToLower(), StlEncrypt},
            {StlPageChannels.ElementName.ToLower(), StlEncrypt},
            {StlPageSqlContents.ElementName.ToLower(), StlEncrypt},
            //{StlPageInputContents.ElementName.ToLower(), StlParserManager.StlEncrypt},
            {StlPageItems.ElementName.ToLower(), StlEncrypt}
        };

        private async Task<string> ParseStlElementAsync(string stlElement)
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
                        parsedContent = await StlDynamic.ParseDynamicElementAsync(stlElement, this);
                    }
                    else
                    {
                        try
                        {
                            if (ElementsToParseDic.TryGetValue(elementName, out var func))
                            {
                                var contextInfo = ContextInfo;
                                ContextInfo = ContextInfo.Clone(stlElement, stlElementInfo.InnerHtml, stlElementInfo.Attributes);

                                var obj = await func(this);

                                ContextInfo = contextInfo;

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
                            parsedContent = await AddStlErrorLogAsync(elementName, stlElement, ex);
                        }
                    }
                }
                else
                {
                    var parsers = await PluginManager.GetParsesAsync();
                    if (parsers.ContainsKey(elementName))
                    {
                        if (stlElementInfo.IsDynamic)
                        {
                            parsedContent = await StlDynamic.ParseDynamicElementAsync(stlElement, this);
                        }
                        else
                        {
                            try
                            {
                                if (parsers.TryGetValue(elementName, out var func))
                                {
                                    var context = new ParseContextImpl();
                                    context.Load(DatabaseManager.ContentRepository, stlElementInfo.OuterHtml, stlElementInfo.InnerHtml,
                                        stlElementInfo.Attributes, PageInfo, ContextInfo);
                                    parsedContent = func(context);
                                }
                            }
                            catch (Exception ex)
                            {
                                parsedContent = await AddStlErrorLogAsync(elementName, stlElement, ex);
                            }
                        }
                    }
                }
            }

            return parsedContent ?? stlElement;
        }
    }
}
