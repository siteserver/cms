using BaiRong.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.StlElement;

namespace SiteServer.CMS.StlParser.Utility
{
    public class StlInnerUtility
    {
        public static void GetYesNo(PageInfo pageInfo, string innerXml, out string yes, out string no)
        {
            yes = string.Empty;
            no = string.Empty;
            if (string.IsNullOrEmpty(innerXml)) return;

            var stlElementList = StlParserUtility.GetStlElementList(innerXml);
            if (stlElementList.Count > 0)
            {
                foreach (var theStlElement in stlElementList)
                {
                    if (StlParserUtility.IsSpecifiedStlElement(theStlElement, StlYes.ElementName) || StlParserUtility.IsSpecifiedStlElement(theStlElement, StlYes.ElementName2))
                    {
                        yes = StlParserUtility.GetInnerXml(theStlElement, true);
                    }
                    else if (StlParserUtility.IsSpecifiedStlElement(theStlElement, StlNo.ElementName) || StlParserUtility.IsSpecifiedStlElement(theStlElement, StlNo.ElementName2))
                    {
                        no = StlParserUtility.GetInnerXml(theStlElement, true);
                    }
                }
            }

            if (string.IsNullOrEmpty(yes) && string.IsNullOrEmpty(no))
            {
                yes = innerXml;
            }

            yes = StringUtils.Trim(yes);
            no = StringUtils.Trim(no);
        }

        public static void GetLoadingYesNo(PageInfo pageInfo, string innerXml, out string loading, out string yes, out string no)
        {
            loading = string.Empty;
            yes = string.Empty;
            no = string.Empty;
            if (string.IsNullOrEmpty(innerXml)) return;

            var stlElementList = StlParserUtility.GetStlElementList(innerXml);
            if (stlElementList.Count > 0)
            {
                foreach (var theStlElement in stlElementList)
                {
                    if (StlParserUtility.IsSpecifiedStlElement(theStlElement, StlLoading.ElementName))
                    {
                        loading = StlParserUtility.GetInnerXml(theStlElement, true);
                    }
                    else if (StlParserUtility.IsSpecifiedStlElement(theStlElement, StlYes.ElementName) || StlParserUtility.IsSpecifiedStlElement(theStlElement, StlYes.ElementName2))
                    {
                        yes = StlParserUtility.GetInnerXml(theStlElement, true);
                    }
                    else if (StlParserUtility.IsSpecifiedStlElement(theStlElement, StlNo.ElementName) || StlParserUtility.IsSpecifiedStlElement(theStlElement, StlNo.ElementName2))
                    {
                        no = StlParserUtility.GetInnerXml(theStlElement, true);
                    }
                }
            }

            if (string.IsNullOrEmpty(loading) && string.IsNullOrEmpty(yes) && string.IsNullOrEmpty(no))
            {
                yes = innerXml;
            }

            loading = StringUtils.Trim(loading);
            yes = StringUtils.Trim(yes);
            no = StringUtils.Trim(no);
        }

        public static void GetTemplateLoadingYesNo(string innerXml, out string template, out string loading, out string yes, out string no)
        {
            template = string.Empty;
            loading = string.Empty;
            yes = string.Empty;
            no = string.Empty;
            if (string.IsNullOrEmpty(innerXml)) return;

            var stlElementList = StlParserUtility.GetStlElementList(innerXml);
            if (stlElementList.Count > 0)
            {
                foreach (var theStlElement in stlElementList)
                {
                    if (StlParserUtility.IsSpecifiedStlElement(theStlElement, StlTemplate.ElementName))
                    {
                        template = StlParserUtility.GetInnerXml(theStlElement, true);
                    }
                    else if (StlParserUtility.IsSpecifiedStlElement(theStlElement, StlLoading.ElementName))
                    {
                        loading = StlParserUtility.GetInnerXml(theStlElement, true);
                    }
                    else if (StlParserUtility.IsSpecifiedStlElement(theStlElement, StlYes.ElementName) || StlParserUtility.IsSpecifiedStlElement(theStlElement, StlYes.ElementName2))
                    {
                        yes = StlParserUtility.GetInnerXml(theStlElement, true);
                    }
                    else if (StlParserUtility.IsSpecifiedStlElement(theStlElement, StlNo.ElementName) || StlParserUtility.IsSpecifiedStlElement(theStlElement, StlNo.ElementName2))
                    {
                        no = StlParserUtility.GetInnerXml(theStlElement, true);
                    }
                }
            }

            if (string.IsNullOrEmpty(template) && string.IsNullOrEmpty(loading) && string.IsNullOrEmpty(yes) && string.IsNullOrEmpty(no))
            {
                template = innerXml;
            }

            template = StringUtils.Trim(template);
            loading = StringUtils.Trim(loading);
            yes = StringUtils.Trim(yes);
            no = StringUtils.Trim(no);
        }
    }
}
