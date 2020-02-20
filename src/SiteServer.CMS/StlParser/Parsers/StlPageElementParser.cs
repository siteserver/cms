using System.Threading.Tasks;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.Parsers
{
	/// <summary>
	/// StlPageElementParser 的摘要说明。
	/// </summary>
	public static class StlPageElementParser
	{
		//在内容页中对“翻页项容器”（stl:pageItems）元素进行解析，此元素在生成页面时单独解析，不包含在ParseStlElement方法中。
		public static async Task<string> ParseStlPageInContentPageAsync(string stlElement, PageInfo pageInfo, int channelId, int contentId, int currentPageIndex, int pageCount)
		{
            return await StlPageItems.ParseAsync(stlElement, pageInfo, channelId, contentId, currentPageIndex, pageCount, pageCount, ContextType.Content);
		}

		//在栏目页中对“翻页项容器”（stl:pageItems）元素进行解析，此元素在生成页面时单独解析，不包含在ParseStlElement方法中。
		public static async Task<string> ParseStlPageInChannelPageAsync(string stlElement, PageInfo pageInfo, int channelId, int currentPageIndex, int pageCount, int totalNum)
		{
            return await StlPageItems.ParseAsync(stlElement, pageInfo, channelId, 0, currentPageIndex, pageCount, totalNum, ContextType.Channel);
		}

        public static async Task<string> ParseStlPageInSearchPageAsync(string stlElement, PageInfo pageInfo, string ajaxDivId, int channelId, int currentPageIndex, int pageCount, int totalNum)
        {
            return await StlPageItems.ParseInSearchPageAsync(stlElement, pageInfo, ajaxDivId, channelId, currentPageIndex, pageCount, totalNum);
        }

        public static async Task<string> ParseStlPageInDynamicPageAsync(string stlElement, PageInfo pageInfo, int currentPageIndex, int pageCount, int totalNum, bool isPageRefresh, string ajaxDivId)
        {
            return await StlPageItems.ParseInDynamicPageAsync(stlElement, pageInfo, currentPageIndex, pageCount, totalNum, isPageRefresh, ajaxDivId);
        }

		public static async Task<string> ParseStlPageItemsAsync(string htmlInStlPageElement, PageInfo pageInfo, int channelId, int contentId, int currentPageIndex, int pageCount, int totalNum, bool isXmlContent, ContextType contextType)
		{
			var html = htmlInStlPageElement;

            var mc = StlParserUtility.GetStlEntityRegex("pageItem").Matches(html);
            for (var i = 0; i < mc.Count; i++)
            {
                var stlEntity = mc[i].Value;
                var pageHtml = await StlPageItem.ParseEntityAsync(stlEntity, pageInfo, channelId, contentId, currentPageIndex, pageCount, totalNum, isXmlContent, contextType);
                html = html.Replace(stlEntity, pageHtml);
            }

            mc = StlParserUtility.RegexStlElement.Matches(html);
			for (var i = 0; i < mc.Count; i++)
			{
				var stlElement = mc[i].Value;
                var pageHtml = await StlPageItem.ParseElementAsync(stlElement, pageInfo, channelId, contentId, currentPageIndex, pageCount, totalNum, contextType);
				html = html.Replace(stlElement, pageHtml);
			}
            
			return html;
		}

        public static async Task<string> ParseStlPageItemsInSearchPageAsync(string htmlInStlPageElement, PageInfo pageInfo, string ajaxDivId, int channelId, int currentPageIndex, int pageCount, int totalNum)
        {
            var html = htmlInStlPageElement;

            var mc = StlParserUtility.GetStlEntityRegex("pageItem").Matches(html);
            for (var i = 0; i < mc.Count; i++)
            {
                var stlEntity = mc[i].Value;
                var pageHtml = await StlPageItem.ParseEntityInSearchPageAsync(stlEntity, pageInfo, ajaxDivId, channelId, currentPageIndex, pageCount, totalNum);
                html = html.Replace(stlEntity, pageHtml);
            }

            mc = StlParserUtility.RegexStlElement.Matches(html);
            for (var i = 0; i < mc.Count; i++)
            {
                var stlElement = mc[i].Value;
                var pageHtml = await StlPageItem.ParseElementInSearchPageAsync(stlElement, pageInfo, ajaxDivId, currentPageIndex, pageCount, totalNum);
                html = html.Replace(stlElement, pageHtml);
            }

            return html;
        }

        public static async Task<string> ParseStlPageItemsInDynamicPageAsync(string htmlInStlPageElement, PageInfo pageInfo, int currentPageIndex, int pageCount, int totalNum, bool isPageRefresh, string ajaxDivId)
        {
            var html = htmlInStlPageElement;

            var mc = StlParserUtility.GetStlEntityRegex("pageItem").Matches(html);
            for (var i = 0; i < mc.Count; i++)
            {
                var stlEntity = mc[i].Value;
                var pageHtml = await StlPageItem.ParseEntityInDynamicPageAsync(stlEntity, pageInfo, currentPageIndex, pageCount, totalNum, isPageRefresh, ajaxDivId);
                html = html.Replace(stlEntity, pageHtml);
            }

            mc = StlParserUtility.RegexStlElement.Matches(html);
            for (var i = 0; i < mc.Count; i++)
            {
                var stlElement = mc[i].Value;
                var pageHtml = await StlPageItem.ParseElementInDynamicPageAsync(stlElement, pageInfo, currentPageIndex, pageCount, totalNum, isPageRefresh, ajaxDivId);
                html = html.Replace(stlElement, pageHtml);
            }

            return html;
        }

		
		//在内容页中对“翻页”（stl:pageItem）元素进行解析，此元素在生成页面时单独解析，不包含在ParseStlElement方法中。
		public static async Task<string> ParseStlPageItemInContentPageAsync(string stlElement, PageInfo pageInfo, int channelId, int contentId, int currentPageIndex, int pageCount, int totalNum)
		{
			return await StlPageItem.ParseElementAsync(stlElement, pageInfo, channelId, contentId, currentPageIndex, pageCount, totalNum, ContextType.Content);
		}

		//在栏目页中对“翻页”（stl:pageItem）元素进行解析，此元素在生成页面时单独解析，不包含在ParseStlElement方法中。
		public static async Task<string> ParseStlPageItemInChannelPageAsync(string stlElement, PageInfo pageInfo, int channelId, int currentPageIndex, int pageCount, int totalNum)
		{
            return await StlPageItem.ParseElementAsync(stlElement, pageInfo, channelId, 0, currentPageIndex, pageCount, totalNum, ContextType.Channel);
		}

        public static async Task<string> ParseStlPageItemInSearchPageAsync(string stlElement, PageInfo pageInfo, string ajaxDivId, int currentPageIndex, int pageCount, int totalNum)
        {
            return await StlPageItem.ParseElementInSearchPageAsync(stlElement, pageInfo, ajaxDivId, currentPageIndex, pageCount, totalNum);
        }

        public static async Task<string> ParseStlPageItemInDynamicPageAsync(string stlElement, PageInfo pageInfo, int currentPageIndex, int pageCount, int totalNum, bool isPageRefresh, string ajaxDivId)
        {
            return await StlPageItem.ParseElementInDynamicPageAsync(stlElement, pageInfo, currentPageIndex, pageCount, totalNum, isPageRefresh, ajaxDivId);
        }
	}
}
