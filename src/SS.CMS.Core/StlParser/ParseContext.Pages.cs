using SS.CMS.Core.StlParser.Models;
using SS.CMS.Core.StlParser.StlElement;
using SS.CMS.Core.StlParser.Utility;

namespace SS.CMS.Core.StlParser
{
    /// <summary>
    /// StlPageElementParser 的摘要说明。
    /// </summary>
    public partial class ParseContext
    {
        //在内容页中对“翻页项容器”（stl:pageItems）元素进行解析，此元素在生成页面时单独解析，不包含在ParseStlElement方法中。
        public string ParseStlPageInContentPage(string stlElement, int currentPageIndex, int pageCount)
        {
            ContextType = EContextType.Content;
            return StlPageItems.Parse(this, stlElement, currentPageIndex, pageCount, pageCount);
        }

        //在栏目页中对“翻页项容器”（stl:pageItems）元素进行解析，此元素在生成页面时单独解析，不包含在ParseStlElement方法中。
        public string ParseStlPageInChannelPage(string stlElement, int currentPageIndex, int pageCount, int totalNum)
        {
            ContextType = EContextType.Channel;
            return StlPageItems.Parse(this, stlElement, currentPageIndex, pageCount, totalNum);
        }

        public string ParseStlPageInSearchPage(string stlElement, string ajaxDivId, int currentPageIndex, int pageCount, int totalNum)
        {
            return StlPageItems.ParseInSearchPage(this, stlElement, ajaxDivId, currentPageIndex, pageCount, totalNum);
        }

        public string ParseStlPageInDynamicPage(string stlElement, int currentPageIndex, int pageCount, int totalNum, bool isPageRefresh, string ajaxDivId)
        {
            return StlPageItems.ParseInDynamicPage(this, stlElement, currentPageIndex, pageCount, totalNum, isPageRefresh, ajaxDivId);
        }

        public string ParseStlPageItems(string htmlInStlPageElement, int currentPageIndex, int pageCount, int totalNum, bool isXmlContent)
        {
            var html = htmlInStlPageElement;

            var mc = StlParserUtility.GetStlEntityRegex("pageItem").Matches(html);
            for (var i = 0; i < mc.Count; i++)
            {
                var stlEntity = mc[i].Value;
                var pageHtml = StlPageItem.ParseEntity(this, stlEntity, currentPageIndex, pageCount, totalNum, isXmlContent);
                html = html.Replace(stlEntity, pageHtml);
            }

            mc = StlParserUtility.RegexStlElement.Matches(html);
            for (var i = 0; i < mc.Count; i++)
            {
                var stlElement = mc[i].Value;
                var pageHtml = StlPageItem.ParseElement(this, stlElement, currentPageIndex, pageCount, totalNum);
                html = html.Replace(stlElement, pageHtml);
            }

            return html;
        }

        public string ParseStlPageItemsInSearchPage(string htmlInStlPageElement, string ajaxDivId, int currentPageIndex, int pageCount, int totalNum)
        {
            var html = htmlInStlPageElement;

            var mc = StlParserUtility.GetStlEntityRegex("pageItem").Matches(html);
            for (var i = 0; i < mc.Count; i++)
            {
                var stlEntity = mc[i].Value;
                var pageHtml = StlPageItem.ParseEntityInSearchPage(this, stlEntity, ajaxDivId, currentPageIndex, pageCount, totalNum);
                html = html.Replace(stlEntity, pageHtml);
            }

            mc = StlParserUtility.RegexStlElement.Matches(html);
            for (var i = 0; i < mc.Count; i++)
            {
                var stlElement = mc[i].Value;
                var pageHtml = StlPageItem.ParseElementInSearchPage(this, stlElement, ajaxDivId, currentPageIndex, pageCount, totalNum);
                html = html.Replace(stlElement, pageHtml);
            }

            return html;
        }

        public string ParseStlPageItemsInDynamicPage(string htmlInStlPageElement, int currentPageIndex, int pageCount, int totalNum, bool isPageRefresh, string ajaxDivId)
        {
            var html = htmlInStlPageElement;

            var mc = StlParserUtility.GetStlEntityRegex("pageItem").Matches(html);
            for (var i = 0; i < mc.Count; i++)
            {
                var stlEntity = mc[i].Value;
                var pageHtml = StlPageItem.ParseEntityInDynamicPage(this, stlEntity, currentPageIndex, pageCount, totalNum, isPageRefresh, ajaxDivId);
                html = html.Replace(stlEntity, pageHtml);
            }

            mc = StlParserUtility.RegexStlElement.Matches(html);
            for (var i = 0; i < mc.Count; i++)
            {
                var stlElement = mc[i].Value;
                var pageHtml = StlPageItem.ParseElementInDynamicPage(this, stlElement, currentPageIndex, pageCount, totalNum, isPageRefresh, ajaxDivId);
                html = html.Replace(stlElement, pageHtml);
            }

            return html;
        }


        //在内容页中对“翻页”（stl:pageItem）元素进行解析，此元素在生成页面时单独解析，不包含在ParseStlElement方法中。
        public string ParseStlPageItemInContentPage(string stlElement, int currentPageIndex, int pageCount, int totalNum)
        {
            ContextType = EContextType.Content;
            return StlPageItem.ParseElement(this, stlElement, currentPageIndex, pageCount, totalNum);
        }

        //在栏目页中对“翻页”（stl:pageItem）元素进行解析，此元素在生成页面时单独解析，不包含在ParseStlElement方法中。
        public string ParseStlPageItemInChannelPage(string stlElement, int currentPageIndex, int pageCount, int totalNum)
        {
            ContextType = EContextType.Channel;
            return StlPageItem.ParseElement(this, stlElement, currentPageIndex, pageCount, totalNum);
        }

        public string ParseStlPageItemInSearchPage(string stlElement, string ajaxDivId, int currentPageIndex, int pageCount, int totalNum)
        {
            return StlPageItem.ParseElementInSearchPage(this, stlElement, ajaxDivId, currentPageIndex, pageCount, totalNum);
        }

        public string ParseStlPageItemInDynamicPage(string stlElement, int currentPageIndex, int pageCount, int totalNum, bool isPageRefresh, string ajaxDivId)
        {
            return StlPageItem.ParseElementInDynamicPage(this, stlElement, currentPageIndex, pageCount, totalNum, isPageRefresh, ajaxDivId);
        }
    }
}
