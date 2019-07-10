using System.Threading.Tasks;
using SS.CMS.Core.StlParser.StlElement;
using SS.CMS.Models;

namespace SS.CMS.Core.Services
{
    public partial class UrlManager
    {
        public async Task<string> GetPagerUrlInChannelPageAsync(string type, Site siteInfo, Channel nodeInfo, int index, int currentPageIndex, int pageCount, bool isLocal)
        {
            var pageIndex = 0;
            if (type.ToLower().Equals(StlPageItem.TypeFirstPage.ToLower()))//首页
            {
                pageIndex = 0;
            }
            else if (type.ToLower().Equals(StlPageItem.TypeLastPage.ToLower()))//末页
            {
                pageIndex = pageCount - 1;
            }
            else if (type.ToLower().Equals(StlPageItem.TypePreviousPage.ToLower()))//上一页
            {
                pageIndex = currentPageIndex - 1;
            }
            else if (type.ToLower().Equals(StlPageItem.TypeNextPage.ToLower()))//下一页
            {
                pageIndex = currentPageIndex + 1;
            }
            else if (type.ToLower().Equals(StlPageItem.TypePageNavigation.ToLower()) || type.ToLower().Equals(StlPageItem.TypePageSelect.ToLower()))
            {
                pageIndex = index - 1;
            }

            var physicalPath = await _pathManager.GetChannelPageFilePathAsync(siteInfo, nodeInfo.Id, pageIndex);
            return GetSiteUrlByPhysicalPath(siteInfo, physicalPath, isLocal);
        }

        public async Task<string> GetPagerUrlInContentPageAsync(string type, Site siteInfo, int channelId, int contentId, int index, int currentPageIndex, int pageCount, bool isLocal)
        {
            var pageIndex = 0;
            if (type.ToLower().Equals(StlPageItem.TypeFirstPage.ToLower()))//首页
            {
                pageIndex = 0;
            }
            else if (type.ToLower().Equals(StlPageItem.TypeLastPage.ToLower()))//末页
            {
                pageIndex = pageCount - 1;
            }
            else if (type.ToLower().Equals(StlPageItem.TypePreviousPage.ToLower()))//上一页
            {
                pageIndex = currentPageIndex - 1;
            }
            else if (type.ToLower().Equals(StlPageItem.TypeNextPage.ToLower()))//下一页
            {
                pageIndex = currentPageIndex + 1;
            }
            else if (type.ToLower().Equals(StlPageItem.TypePageNavigation.ToLower()) || type.ToLower().Equals(StlPageItem.TypePageSelect.ToLower()))
            {
                pageIndex = index - 1;
            }

            var physicalPath = await _pathManager.GetContentPageFilePathAsync(siteInfo, channelId, contentId, pageIndex);
            return GetSiteUrlByPhysicalPath(siteInfo, physicalPath, isLocal);
        }

        public string GetPagerClickStringInSearchPage(string type, string ajaxDivId, int index, int currentPageIndex, int pageCount)
        {
            var clickString = string.Empty;

            if (type.ToLower().Equals(StlPageItem.TypeFirstPage.ToLower()))//首页
            {
                clickString = $"stlRedirect{ajaxDivId}({1})";
            }
            else if (type.ToLower().Equals(StlPageItem.TypeLastPage.ToLower()))//末页
            {
                clickString = $"stlRedirect{ajaxDivId}({pageCount})";
            }
            else if (type.ToLower().Equals(StlPageItem.TypePreviousPage.ToLower()))//上一页
            {
                clickString = $"stlRedirect{ajaxDivId}({currentPageIndex})";
            }
            else if (type.ToLower().Equals(StlPageItem.TypeNextPage.ToLower()))//下一页
            {
                clickString = $"stlRedirect{ajaxDivId}({currentPageIndex + 2})";
            }
            else if (type.ToLower().Equals(StlPageItem.TypePageNavigation.ToLower()))
            {
                clickString = $"stlRedirect{ajaxDivId}({index})";
            }
            else if (type.ToLower().Equals(StlPageItem.TypePageSelect.ToLower()))
            {
                clickString = $"stlJump{ajaxDivId}(this)";
            }

            return clickString;
        }

        public async Task<string> GetPagerJsMethodInDynamicPageAsync(string type, Site siteInfo, int channelId, int contentId, int index, int currentPageIndex, int pageCount, bool isPageRefresh, string ajaxDivId, bool isLocal)
        {
            var jsMethod = string.Empty;
            var pageIndex = 0;
            if (type.ToLower().Equals(StlPageItem.TypeFirstPage.ToLower()))//首页
            {
                pageIndex = 0;
            }
            else if (type.ToLower().Equals(StlPageItem.TypeLastPage.ToLower()))//末页
            {
                pageIndex = pageCount - 1;
            }
            else if (type.ToLower().Equals(StlPageItem.TypePreviousPage.ToLower()))//上一页
            {
                pageIndex = currentPageIndex - 1;
            }
            else if (type.ToLower().Equals(StlPageItem.TypeNextPage.ToLower()))//下一页
            {
                pageIndex = currentPageIndex + 1;
            }
            else if (type.ToLower().Equals(StlPageItem.TypePageNavigation.ToLower()) || type.ToLower().Equals(StlPageItem.TypePageSelect.ToLower()))
            {
                pageIndex = index - 1;
            }

            if (isPageRefresh)
            {
                if (type.ToLower().Equals(StlPageItem.TypeFirstPage.ToLower()))//首页
                {
                    jsMethod = $"stlRedirect{ajaxDivId}({1})";
                }
                else if (type.ToLower().Equals(StlPageItem.TypeLastPage.ToLower()))//末页
                {
                    jsMethod = $"stlRedirect{ajaxDivId}({pageCount})";
                }
                else if (type.ToLower().Equals(StlPageItem.TypePreviousPage.ToLower()))//上一页
                {
                    jsMethod = $"stlRedirect{ajaxDivId}({currentPageIndex})";
                }
                else if (type.ToLower().Equals(StlPageItem.TypeNextPage.ToLower()))//下一页
                {
                    jsMethod = $"stlRedirect{ajaxDivId}({currentPageIndex + 2})";
                }
                else if (type.ToLower().Equals(StlPageItem.TypePageNavigation.ToLower()))
                {
                    jsMethod = $"stlRedirect{ajaxDivId}({index})";
                }
                else if (type.ToLower().Equals(StlPageItem.TypePageSelect.ToLower()))
                {
                    jsMethod = $"stlRedirect{ajaxDivId}(this.options[this.selectedIndex].value)";
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(ajaxDivId))
                {
                    if (type.ToLower().Equals(StlPageItem.TypeFirstPage.ToLower()))//首页
                    {
                        jsMethod = $"stlDynamic{ajaxDivId}({1})";
                    }
                    else if (type.ToLower().Equals(StlPageItem.TypeLastPage.ToLower()))//末页
                    {
                        jsMethod = $"stlDynamic{ajaxDivId}({pageCount})";
                    }
                    else if (type.ToLower().Equals(StlPageItem.TypePreviousPage.ToLower()))//上一页
                    {
                        jsMethod = $"stlDynamic{ajaxDivId}({currentPageIndex})";
                    }
                    else if (type.ToLower().Equals(StlPageItem.TypeNextPage.ToLower()))//下一页
                    {
                        jsMethod = $"stlDynamic{ajaxDivId}({currentPageIndex + 2})";
                    }
                    else if (type.ToLower().Equals(StlPageItem.TypePageNavigation.ToLower()))
                    {
                        jsMethod = $"stlDynamic{ajaxDivId}({index})";
                    }
                    else if (type.ToLower().Equals(StlPageItem.TypePageSelect.ToLower()))
                    {
                        jsMethod = $"stlDynamic{ajaxDivId}(this.options[this.selectedIndex].value)";
                    }
                }
                else
                {
                    var nodeInfo = await _channelRepository.GetChannelInfoAsync(channelId);
                    var redirectUrl = contentId > 0 ? await _pathManager.GetContentPageFilePathAsync(siteInfo, nodeInfo.Id, contentId, pageIndex) : await _pathManager.GetChannelPageFilePathAsync(siteInfo, nodeInfo.Id, pageIndex);
                    redirectUrl = GetSiteUrlByPhysicalPath(siteInfo, redirectUrl, isLocal);
                    jsMethod = $"window.location.href='{redirectUrl}';";
                }
            }
            return jsMethod;
        }
    }
}
