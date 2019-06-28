using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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

        public static readonly Dictionary<string, Func<ParseContext, Task<object>>> ElementsToParseDic = new Dictionary<string, Func<ParseContext, Task<object>>>
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
            {StlTags.ElementName.ToLower(), StlTags.ParseAsync},
            {StlTree.ElementName.ToLower(), StlTree.ParseAsync},
            {StlValue.ElementName.ToLower(), StlValue.ParseAsync},
            {StlVideo.ElementName.ToLower(), StlVideo.ParseAsync},
            {StlZoom.ElementName.ToLower(), StlZoom.ParseAsync}
        };

        private static readonly IList<string> ElementsToEncrypt = new List<string>
        {
            {StlPageContents.ElementName.ToLower()},
            {StlPageChannels.ElementName.ToLower()},
            {StlPageSqlContents.ElementName.ToLower()},
            {StlPageItems.ElementName.ToLower()}
        };

        private async Task<string> ParseStlElementAsync(string stlElement)
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
                        parsedContent = await StlDynamic.ParseDynamicElementAsync(stlElement, this);
                    }
                    else
                    {
                        try
                        {
                            if (ElementsToParseDic.TryGetValue(elementName, out var func))
                            {
                                var contextClone = Clone(stlElement, stlElementInfo.InnerHtml, stlElementInfo.Attributes);

                                var obj = func(contextClone);

                                parsedContent = obj == null ? string.Empty : TranslateUtils.JsonSerialize(obj);
                            }
                        }
                        catch (Exception ex)
                        {
                            parsedContent = await GetErrorMessageAsync(elementName, stlElement, ex);
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
                                    await context.LoadAsync(stlElementInfo.OuterHtml, stlElementInfo.InnerHtml,
                                        stlElementInfo.Attributes, PageInfo, this);
                                    parsedContent = func(context);
                                }
                            }
                            catch (Exception ex)
                            {
                                parsedContent = await GetErrorMessageAsync(elementName, stlElement, ex);
                            }
                        }
                    }
                }
            }

            return parsedContent ?? stlElement;
        }
    }
}
