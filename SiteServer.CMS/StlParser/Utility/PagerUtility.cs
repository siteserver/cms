using System.Threading.Tasks;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Db;
using SiteServer.CMS.StlParser.StlElement;

namespace SiteServer.CMS.StlParser.Utility
{
    public static class PagerUtility
    {
        public static async Task<string> GetUrlInChannelPageAsync(string type, Site site, ChannelInfo nodeInfo, int index, int currentPageIndex, int pageCount, bool isLocal)
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

            var physicalPath = PathUtility.GetChannelPageFilePath(site, nodeInfo.Id, pageIndex);
            return await PageUtility.GetSiteUrlByPhysicalPathAsync(site, physicalPath, isLocal);
        }

        public static async Task<string> GetUrlInContentPageAsync(string type, Site site, int channelId, int contentId, int index, int currentPageIndex, int pageCount, bool isLocal)
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

            var physicalPath = PathUtility.GetContentPageFilePath(site, channelId, contentId, pageIndex);
            return await PageUtility.GetSiteUrlByPhysicalPathAsync(site, physicalPath, isLocal);
        }

        public static string GetClickStringInSearchPage(string type, string ajaxDivId, int index, int currentPageIndex, int pageCount)
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
                clickString = $"stlRedirect{ajaxDivId}(this.options[this.selectedIndex].value)";
            }

            return clickString;
        }

        public static async Task<string> GetJsMethodInDynamicPageAsync(string type, Site site, int channelId, int contentId, int index, int currentPageIndex, int pageCount, bool isPageRefresh, string ajaxDivId, bool isLocal)
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
                    var nodeInfo = ChannelManager.GetChannelInfo(site.Id, channelId);
                    var redirectUrl = contentId > 0 ? PathUtility.GetContentPageFilePath(site, nodeInfo.Id, contentId, pageIndex) : PathUtility.GetChannelPageFilePath(site, nodeInfo.Id, pageIndex);
                    redirectUrl = await PageUtility.GetSiteUrlByPhysicalPathAsync(site, redirectUrl, isLocal);
                    jsMethod = $"window.location.href='{redirectUrl}';";
                }
            }
            return jsMethod;
        }
    }
}
