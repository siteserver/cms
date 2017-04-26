using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using BaiRong.Core.Model.Attributes;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.CMS.StlParser.StlElement.WCM;
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
            var stlElementArrayList = StlParserUtility.GetStlElementList(parsedBuilder.ToString());
            foreach (var stlElement in stlElementArrayList)
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
            {StlA.ElementName, StlA.Parse},
            {StlAction.ElementName, StlAction.Parse},
            {StlAd.ElementName, StlAd.Parse},
            {StlAnalysis.ElementName, StlAnalysis.Parse},
            {StlAudio.ElementName, StlAudio.Parse},
            {StlChannel.ElementName, StlChannel.Parse},
            {StlChannels.ElementName, StlChannels.Parse},
            {StlComment.ElementName, StlComment.Parse},
            {StlCommentInput.ElementName, StlCommentInput.Parse},
            {StlComments.ElementName, StlComments.Parse},
            {StlContainer.ElementName, StlContainer.Parse},
            {StlContent.ElementName, StlContent.Parse},
            {StlContents.ElementName, StlContents.Parse},
            {StlCount.ElementName, StlCount.Parse},
            {StlDigg.ElementName, StlDigg.Parse},
            {StlDynamic.ElementName, StlDynamic.Parse},
            {StlEach.ElementName, StlEach.Parse},
            {StlFile.ElementName, StlFile.Parse},
            {StlFlash.ElementName, StlFlash.Parse},
            {StlFocusViewer.ElementName, StlFocusViewer.Parse},
            {StlIf.ElementName, StlIf.Parse},
            {StlImage.ElementName, StlImage.Parse},
            {StlInclude.ElementName, StlInclude.Parse},
            {StlInput.ElementName, StlInput.Parse},
            {StlInputContent.ElementName, StlInputContent.Parse},
            {StlInputContents.ElementName, StlInputContents.Parse},
            {StlLayout.ElementName, StlLayout.Parse},
            {StlLocation.ElementName, StlLocation.Parse},
            {StlMarquee.ElementName, StlMarquee.Parse},
            {StlMenu.ElementName, StlMenu.Parse},
            {StlNavigation.ElementName, StlNavigation.Parse},
            {StlPhoto.ElementName, StlPhoto.Parse},
            {StlPlayer.ElementName, StlPlayer.Parse},
            {StlPrinter.ElementName, StlPrinter.Parse},
            {StlResume.ElementName, StlResume.Parse},
            {StlRss.ElementName, StlRss.Parse},
            {StlSearchOutput.ElementName, StlSearchOutput.Parse},
            {StlSearchOutput.ElementName2, StlSearchOutput.Parse},
            {StlSelect.ElementName, StlSelect.Parse},
            {StlSite.ElementName, StlSite.Parse},
            {StlSites.ElementName, StlSites.Parse},
            {StlSlide.ElementName, StlSlide.Parse},
            {StlSqlContent.ElementName, StlSqlContent.Parse},
            {StlSqlContents.ElementName, StlSqlContents.Parse},
            {StlStar.ElementName, StlStar.Parse},
            {StlTabs.ElementName, StlTabs.Parse},
            {StlTags.ElementName, StlTags.Parse},
            {StlTree.ElementName, StlTree.Parse},
            {StlValue.ElementName, StlValue.Parse},
            {StlVideo.ElementName, StlVideo.Parse},
            {StlVote.ElementName, StlVote.Parse},
            {StlZoom.ElementName, StlZoom.Parse},
            {StlGovInteractApply.ElementName, StlGovInteractApply.Parse},
            {StlGovInteractQuery.ElementName, StlGovInteractQuery.Parse},
            {StlGovPublicApply.ElementName, StlGovPublicApply.Parse},
            {StlGovPublicQuery.ElementName, StlGovPublicQuery.Parse}
        };

        private static readonly Dictionary<string, Func<string, string>> ElementsToTranslateDic = new Dictionary<string, Func<string, string>>
        {
            {StlPageContents.ElementName, StlPageContents.Translate},
            {StlPageChannels.ElementName, StlPageChannels.Translate},
            {StlPageSqlContents.ElementName, StlPageSqlContents.Translate},
            {StlPageInputContents.ElementName, StlPageInputContents.Translate},
            {StlPageItems.ElementName, StlPageItems.Translate}
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
