using System.Threading.Tasks;
using SSCMS.Core.StlParser.StlElement;
using SSCMS.Core.StlParser.Utility;
using SSCMS.Parse;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class ParseManager
    {
		//在内容页中对“翻页项容器”（stl:pageItems）元素进行解析，此元素在生成页面时单独解析，不包含在ParseStlElement方法中。
		public async Task<string> ParseStlPageInContentPageAsync(string stlElement, int channelId, int contentId, int currentPageIndex, int pageCount)
		{
            return await StlPageItems.ParseAsync(this, stlElement, channelId, contentId, currentPageIndex, pageCount, pageCount, ParseType.Content);
		}

		//在栏目页中对“翻页项容器”（stl:pageItems）元素进行解析，此元素在生成页面时单独解析，不包含在ParseStlElement方法中。
		public async Task<string> ParseStlPageInChannelPageAsync(string stlElement, int channelId, int currentPageIndex, int pageCount, int totalNum)
		{
            return await StlPageItems.ParseAsync(this, stlElement, channelId, 0, currentPageIndex, pageCount, totalNum, ParseType.Channel);
		}

        public async Task<string> ParseStlPageInSearchPageAsync(string stlElement, string ajaxDivId, int channelId, int currentPageIndex, int pageCount, int totalNum)
        {
            return await StlPageItems.ParseInSearchPageAsync(this, stlElement, ajaxDivId, channelId, currentPageIndex, pageCount, totalNum);
        }

        public async Task<string> ParseStlPageInDynamicPageAsync(string stlElement, int currentPageIndex, int pageCount, int totalNum, bool isPageRefresh, string ajaxDivId)
        {
            return await StlPageItems.ParseInDynamicPageAsync(this, stlElement, currentPageIndex, pageCount, totalNum, isPageRefresh, ajaxDivId);
        }

		public async Task<string> ParseStlPageItemsAsync(string htmlInStlPageElement, int channelId, int contentId, int currentPageIndex, int pageCount, int totalNum, bool isXmlContent, ParseType contextType)
		{
			var html = htmlInStlPageElement;

            var mc = StlParserUtility.GetStlEntityRegex("pageItem").Matches(html);
            for (var i = 0; i < mc.Count; i++)
            {
                var stlEntity = mc[i].Value;
                var pageHtml = await StlPageItem.ParseEntityAsync(this, stlEntity, channelId, contentId, currentPageIndex, pageCount, totalNum, isXmlContent, contextType);
                html = html.Replace(stlEntity, pageHtml);
            }

            mc = ParseUtils.RegexStlElement.Matches(html);
			for (var i = 0; i < mc.Count; i++)
			{
				var stlElement = mc[i].Value;
                var pageHtml = await StlPageItem.ParseElementAsync(this, stlElement, channelId, contentId, currentPageIndex, pageCount, totalNum, contextType);
				html = html.Replace(stlElement, pageHtml);
			}
            
			return html;
		}

        public async Task<string> ParseStlPageItemsInSearchPageAsync(string htmlInStlPageElement, string ajaxDivId, int channelId, int currentPageIndex, int pageCount, int totalNum)
        {
            var html = htmlInStlPageElement;

            var mc = StlParserUtility.GetStlEntityRegex("pageItem").Matches(html);
            for (var i = 0; i < mc.Count; i++)
            {
                var stlEntity = mc[i].Value;
                var pageHtml = await StlPageItem.ParseEntityInSearchPageAsync(this, stlEntity, ajaxDivId, channelId, currentPageIndex, pageCount, totalNum);
                html = html.Replace(stlEntity, pageHtml);
            }

            mc = ParseUtils.RegexStlElement.Matches(html);
            for (var i = 0; i < mc.Count; i++)
            {
                var stlElement = mc[i].Value;
                var pageHtml = await StlPageItem.ParseElementInSearchPageAsync(this, stlElement, ajaxDivId, currentPageIndex, pageCount, totalNum);
                html = html.Replace(stlElement, pageHtml);
            }

            return html;
        }

        public async Task<string> ParseStlPageItemsInDynamicPageAsync(string htmlInStlPageElement, int currentPageIndex, int pageCount, int totalNum, bool isPageRefresh, string ajaxDivId)
        {
            var html = htmlInStlPageElement;

            var mc = StlParserUtility.GetStlEntityRegex("pageItem").Matches(html);
            for (var i = 0; i < mc.Count; i++)
            {
                var stlEntity = mc[i].Value;
                var pageHtml = await StlPageItem.ParseEntityInDynamicPageAsync(this, stlEntity, currentPageIndex, pageCount, totalNum, isPageRefresh, ajaxDivId);
                html = html.Replace(stlEntity, pageHtml);
            }

            mc = ParseUtils.RegexStlElement.Matches(html);
            for (var i = 0; i < mc.Count; i++)
            {
                var stlElement = mc[i].Value;
                var pageHtml = await StlPageItem.ParseElementInDynamicPageAsync(this, stlElement, currentPageIndex, pageCount, totalNum, isPageRefresh, ajaxDivId);
                html = html.Replace(stlElement, pageHtml);
            }

            return html;
        }

		
		//在内容页中对“翻页”（stl:pageItem）元素进行解析，此元素在生成页面时单独解析，不包含在ParseStlElement方法中。
		public async Task<string> ParseStlPageItemInContentPageAsync(string stlElement, int channelId, int contentId, int currentPageIndex, int pageCount, int totalNum)
		{
			return await StlPageItem.ParseElementAsync(this, stlElement, channelId, contentId, currentPageIndex, pageCount, totalNum, ParseType.Content);
		}

		//在栏目页中对“翻页”（stl:pageItem）元素进行解析，此元素在生成页面时单独解析，不包含在ParseStlElement方法中。
		public async Task<string> ParseStlPageItemInChannelPageAsync(string stlElement, int channelId, int currentPageIndex, int pageCount, int totalNum)
		{
            return await StlPageItem.ParseElementAsync(this, stlElement, channelId, 0, currentPageIndex, pageCount, totalNum, ParseType.Channel);
		}

        public async Task<string> ParseStlPageItemInSearchPageAsync(string stlElement, string ajaxDivId, int currentPageIndex, int pageCount, int totalNum)
        {
            return await StlPageItem.ParseElementInSearchPageAsync(this, stlElement, ajaxDivId, currentPageIndex, pageCount, totalNum);
        }
        public async Task<string> ParseStlPageItemInDynamicPageAsync(string stlElement, int currentPageIndex, int pageCount, int totalNum, bool isPageRefresh, string ajaxDivId)
        {
            return await StlPageItem.ParseElementInDynamicPageAsync(this, stlElement, currentPageIndex, pageCount, totalNum, isPageRefresh, ajaxDivId);
        }
	}
}
