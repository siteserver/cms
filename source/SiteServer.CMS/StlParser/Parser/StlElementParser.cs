using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.Parser
{
    /// <summary>
    /// Stl元素解析器
    /// </summary>
    public class StlElementParser
    {
        private StlElementParser()
        {
        }

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

                    //DateTime start = DateTime.Now;
                    var parsedContent = ParseStlElement(stlElement, pageInfo, contextInfo);
                    //DateTime end = DateTime.Now;
                    //int retval = DateDiff(end, start);
                    //if (retval > 5)
                    //{
                    //    resultContent += $@"
                    //<!-- {stlElement} --><!-- {DateDiff(end, start)} -->
                    //";
                    //}
                    parsedBuilder.Replace(stlElement, parsedContent, startIndex, stlElement.Length);
                }
                catch
                {
                    // ignored
                }
            }
        }

        //private static int DateDiff(DateTime DateTime1, DateTime DateTime2)
        //{
        //    int retval = 0;
        //    try
        //    {
        //        TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
        //        TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
        //        TimeSpan ts = ts1.Subtract(ts2).Duration();
        //        retval = ts.Milliseconds;                
        //    }
        //    catch
        //    {

        //    }
        //    return retval;
        //}

        private static readonly Dictionary<string, Func<string, XmlNode, PageInfo, ContextInfo, string>> ElementsToParseDic = new Dictionary<string, Func<string, XmlNode, PageInfo, ContextInfo, string>>
        {
            {StlA.ElementName.ToLower(), StlA.Parse},
            {StlAction.ElementName.ToLower(), StlAction.Parse},
            {StlAd.ElementName.ToLower(), StlAd.Parse},
            {StlAnalysis.ElementName.ToLower(), StlAnalysis.Parse},
            {StlAudio.ElementName.ToLower(), StlAudio.Parse},
            {StlChannel.ElementName.ToLower(), StlChannel.Parse},
            {StlChannels.ElementName.ToLower(), StlChannels.Parse},
            {StlComment.ElementName.ToLower(), StlComment.Parse},
            {StlCommentInput.ElementName.ToLower(), StlCommentInput.Parse},
            {StlComments.ElementName.ToLower(), StlComments.Parse},
            {StlContainer.ElementName.ToLower(), StlContainer.Parse},
            {StlContent.ElementName.ToLower(), StlContent.Parse},
            {StlContents.ElementName.ToLower(), StlContents.Parse},
            {StlCount.ElementName.ToLower(), StlCount.Parse},
            {StlDigg.ElementName.ToLower(), StlDigg.Parse},
            {StlDynamic.ElementName.ToLower(), StlDynamic.Parse},
            {StlEach.ElementName.ToLower(), StlEach.Parse},
            {StlFile.ElementName.ToLower(), StlFile.Parse},
            {StlFlash.ElementName.ToLower(), StlFlash.Parse},
            {StlFocusViewer.ElementName.ToLower(), StlFocusViewer.Parse},
            {StlIf.ElementName.ToLower(), StlIf.Parse},
            {StlImage.ElementName.ToLower(), StlImage.Parse},
            {StlInclude.ElementName.ToLower(), StlInclude.Parse},
            {StlInput.ElementName.ToLower(), StlInput.Parse},
            {StlInputContent.ElementName.ToLower(), StlInputContent.Parse},
            {StlInputContents.ElementName.ToLower(), StlInputContents.Parse},
            {StlLocation.ElementName.ToLower(), StlLocation.Parse},
            {StlMarquee.ElementName.ToLower(), StlMarquee.Parse},
            {StlMenu.ElementName.ToLower(), StlMenu.Parse},
            {StlNavigation.ElementName.ToLower(), StlNavigation.Parse},
            {StlPhoto.ElementName.ToLower(), StlPhoto.Parse},
            {StlPlayer.ElementName.ToLower(), StlPlayer.Parse},
            {StlPrinter.ElementName.ToLower(), StlPrinter.Parse},
            {StlResume.ElementName.ToLower(), StlResume.Parse},
            {StlRss.ElementName.ToLower(), StlRss.Parse},
            {StlSearch.ElementName.ToLower(), StlSearch.Parse},
            {StlSearch.ElementName2.ToLower(), StlSearch.Parse},
            {StlSelect.ElementName.ToLower(), StlSelect.Parse},
            {StlSite.ElementName.ToLower(), StlSite.Parse},
            {StlSites.ElementName.ToLower(), StlSites.Parse},
            {StlSlide.ElementName.ToLower(), StlSlide.Parse},
            {StlSqlContent.ElementName.ToLower(), StlSqlContent.Parse},
            {StlSqlContents.ElementName.ToLower(), StlSqlContents.Parse},
            {StlStar.ElementName.ToLower(), StlStar.Parse},
            {StlTabs.ElementName.ToLower(), StlTabs.Parse},
            {StlTags.ElementName.ToLower(), StlTags.Parse},
            {StlTree.ElementName.ToLower(), StlTree.Parse},
            {StlValue.ElementName.ToLower(), StlValue.Parse},
            {StlVideo.ElementName.ToLower(), StlVideo.Parse},
            {StlVote.ElementName.ToLower(), StlVote.Parse},
            {StlZoom.ElementName.ToLower(), StlZoom.Parse},
            {StlGovInteractApply.ElementName.ToLower(), StlGovInteractApply.Parse},
            {StlGovInteractQuery.ElementName.ToLower(), StlGovInteractQuery.Parse},
            {StlGovPublicApply.ElementName.ToLower(), StlGovPublicApply.Parse},
            {StlGovPublicQuery.ElementName.ToLower(), StlGovPublicQuery.Parse}
        };

        private static readonly Dictionary<string, Func<string, string>> ElementsToTranslateDic = new Dictionary<string, Func<string, string>>
        {
            {StlPageContents.ElementName.ToLower(), StlPageContents.Translate},
            {StlPageChannels.ElementName.ToLower(), StlPageChannels.Translate},
            {StlPageSqlContents.ElementName.ToLower(), StlPageSqlContents.Translate},
            {StlPageInputContents.ElementName.ToLower(), StlPageInputContents.Translate},
            {StlPageItems.ElementName.ToLower(), StlPageItems.Translate}
        };

        internal static string ParseStlElement(string stlElement, PageInfo pageInfo, ContextInfo contextInfo)
        {
            string parsedContent = null;
            var xmlDocument = StlParserUtility.GetXmlDocument(stlElement, contextInfo.IsInnerElement);
            XmlNode node = xmlDocument.DocumentElement;
            if (node != null)
            {
                node = node.FirstChild;

                if (node?.Name != null)
                {
                    var elementName = node.Name.ToLower();

                    if (ElementsToTranslateDic.ContainsKey(elementName))
                    {
                        Func<string, string> func;
                        if (ElementsToTranslateDic.TryGetValue(elementName, out func))
                        {
                            parsedContent = func(stlElement);
                        }
                    }
                    else if (ElementsToParseDic.ContainsKey(elementName))
                    {
                        Func<string, XmlNode, PageInfo, ContextInfo, string> func;
                        if (ElementsToParseDic.TryGetValue(elementName, out func))
                        {
                            parsedContent = func(stlElement, node, pageInfo, contextInfo);
                        }
                    }
                }
            }

            if (parsedContent == null)
            {
                return stlElement;
            }

            return contextInfo.IsInnerElement ? parsedContent : StlParserUtility.GetBackHtml(parsedContent, pageInfo);
        }
    }
}
