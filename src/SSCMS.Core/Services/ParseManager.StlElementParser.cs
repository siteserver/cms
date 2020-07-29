using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSCMS.Core.Context;
using SSCMS.Core.StlParser.StlElement;
using SSCMS.Core.StlParser.Utility;
using SSCMS.Plugins;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Services
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
            {StringUtils.ToLower(StlA.ElementName), StlA.ParseAsync},
            {StringUtils.ToLower(StlAction.ElementName), StlAction.ParseAsync},
            {StringUtils.ToLower(StlAudio.ElementName), StlAudio.ParseAsync},
            {StringUtils.ToLower(StlChannel.ElementName), StlChannel.ParseAsync},
            {StringUtils.ToLower(StlChannels.ElementName), StlChannels.ParseAsync},
            {StringUtils.ToLower(StlContainer.ElementName), StlContainer.ParseAsync},
            {StringUtils.ToLower(StlContent.ElementName), StlContent.ParseAsync},
            {StringUtils.ToLower(StlContents.ElementName), StlContents.ParseAsync},
            {StringUtils.ToLower(StlCount.ElementName), StlCount.ParseAsync},
            {StringUtils.ToLower(StlDynamic.ElementName), StlDynamic.ParseAsync},
            {StringUtils.ToLower(StlEach.ElementName), StlEach.ParseAsync},
            {StringUtils.ToLower(StlFile.ElementName), StlFile.ParseAsync},
            {StringUtils.ToLower(StlFlash.ElementName), StlFlash.ParseAsync},
            {StringUtils.ToLower(StlFocusViewer.ElementName), StlFocusViewer.ParseAsync},
            {StringUtils.ToLower(StlIf.ElementName), StlIf.ParseAsync},
            {StringUtils.ToLower(StlImage.ElementName), StlImage.ParseAsync},
            {StringUtils.ToLower(StlInclude.ElementName), StlInclude.ParseAsync},
            {StringUtils.ToLower(StlMaterial.ElementName), StlMaterial.ParseAsync},
            {StringUtils.ToLower(StlLocation.ElementName), StlLocation.ParseAsync},
            {StringUtils.ToLower(StlMarquee.ElementName), StlMarquee.ParseAsync},
            {StringUtils.ToLower(StlNavigation.ElementName), StlNavigation.ParseAsync},
            {StringUtils.ToLower(StlPlayer.ElementName), StlPlayer.ParseAsync},
            {StringUtils.ToLower(StlPrinter.ElementName), StlPrinter.ParseAsync},
            {StringUtils.ToLower(StlSearch.ElementName), StlSearch.ParseAsync},
            {StringUtils.ToLower(StlSearch.ElementName2), StlSearch.ParseAsync},
            {StringUtils.ToLower(StlSelect.ElementName), StlSelect.ParseAsync},
            {StringUtils.ToLower(StlSite.ElementName), StlSite.ParseAsync},
            {StringUtils.ToLower(StlSites.ElementName), StlSites.ParseAsync},
            {StringUtils.ToLower(StlSqlContent.ElementName), StlSqlContent.ParseAsync},
            {StringUtils.ToLower(StlSqlContents.ElementName), StlSqlContents.ParseAsync},
            {StringUtils.ToLower(StlTabs.ElementName), StlTabs.ParseAsync},
            {StringUtils.ToLower(StlValue.ElementName), StlValue.ParseAsync},
            {StringUtils.ToLower(StlVideo.ElementName), StlVideo.ParseAsync},
            {StringUtils.ToLower(StlZoom.ElementName), StlZoom.ParseAsync}
        };

        private Dictionary<string, Func<string, string>> ElementsToTranslateDic => new Dictionary<string, Func<string, string>>
        {
            {StringUtils.ToLower(StlPageContents.ElementName), StlEncrypt},
            {StringUtils.ToLower(StlPageChannels.ElementName), StlEncrypt},
            {StringUtils.ToLower(StlPageSqlContents.ElementName), StlEncrypt},
            //{StringUtils.ToLower(StlPageInputContents.ElementName), StlParserManager.StlEncrypt},
            {StringUtils.ToLower(StlPageItems.ElementName), StlEncrypt}
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
                    var parsers = _pluginManager.GetExtensions<IPluginParse>();
                    var parser = parsers?.FirstOrDefault(x => StringUtils.EqualsIgnoreCase(x.ElementName, elementName));
                    if (parser != null)
                    {
                        if (stlElementInfo.IsDynamic)
                        {
                            parsedContent = await StlDynamic.ParseDynamicElementAsync(stlElement, this);
                        }
                        else
                        {
                            try
                            {
                                var context = new PluginParseStlContext(this, stlElementInfo.OuterHtml, stlElementInfo.InnerHtml, stlElementInfo.Attributes);
                                parsedContent = parser.Parse(context);
                            }
                            catch (Exception ex)
                            {
                                parsedContent = await AddStlErrorLogAsync(elementName, stlElement, ex);
                            }
                        }
                    }

                    var parsersAsync = _pluginManager.GetExtensions<IPluginParseAsync>();
                    var parserAsync = parsersAsync?.FirstOrDefault(x => StringUtils.EqualsIgnoreCase(x.ElementName, elementName));
                    if (parserAsync != null)
                    {
                        if (stlElementInfo.IsDynamic)
                        {
                            parsedContent = await StlDynamic.ParseDynamicElementAsync(stlElement, this);
                        }
                        else
                        {
                            try
                            {
                                var context = new PluginParseStlContext(this, stlElementInfo.OuterHtml, stlElementInfo.InnerHtml, stlElementInfo.Attributes);
                                parsedContent = await parserAsync.ParseAsync(context);
                            }
                            catch (Exception ex)
                            {
                                parsedContent = await AddStlErrorLogAsync(elementName, stlElement, ex);
                            }
                        }
                    }
                    //var parsers = OldPluginManager.GetParses();
                    //if (parsers.ContainsKey(elementName))
                    //{
                    //    if (stlElementInfo.IsDynamic)
                    //    {
                    //        parsedContent = await StlDynamic.ParseDynamicElementAsync(stlElement, this);
                    //    }
                    //    else
                    //    {
                    //        try
                    //        {
                    //            if (parsers.TryGetValue(elementName, out var func))
                    //            {
                    //                var context = new PluginStlParseContext();
                    //                context.Load(stlElementInfo.OuterHtml, stlElementInfo.InnerHtml, stlElementInfo.Attributes, PageInfo, ContextInfo);
                    //                parsedContent = func(context);
                    //            }
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            parsedContent = await AddStlErrorLogAsync(elementName, stlElement, ex);
                    //        }
                    //    }
                    //}
                }
            }

            return parsedContent ?? stlElement;
        }
    }
}
