using System.Threading.Tasks;
using SSCMS.Core.StlParser.StlElement;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class PathManager
    {
        public async Task<string> GetUrlInChannelPageAsync(string type, Site site, Channel node, int index, int currentPageIndex, int pageCount, bool isLocal)
        {
            var pageIndex = 0;
            if (StringUtils.EqualsIgnoreCase(type, StlPageItem.TypeFirstPage))//首页
            {
                pageIndex = 0;
            }
            else if (StringUtils.EqualsIgnoreCase(type, StlPageItem.TypeLastPage))//末页
            {
                pageIndex = pageCount - 1;
            }
            else if (StringUtils.EqualsIgnoreCase(type, StlPageItem.TypePreviousPage))//上一页
            {
                pageIndex = currentPageIndex - 1;
            }
            else if (StringUtils.EqualsIgnoreCase(type, StlPageItem.TypeNextPage))//下一页
            {
                pageIndex = currentPageIndex + 1;
            }
            else if (StringUtils.EqualsIgnoreCase(type, StlPageItem.TypePageNavigation) || StringUtils.EqualsIgnoreCase(type, StlPageItem.TypePageSelect))
            {
                pageIndex = index - 1;
            }

            var physicalPath = await GetChannelPageFilePathAsync(site, node.Id, pageIndex);
            return await GetSiteUrlByPhysicalPathAsync(site, physicalPath, isLocal);
        }

        public async Task<string> GetUrlInContentPageAsync(string type, Site site, int channelId, int contentId, int index, int currentPageIndex, int pageCount, bool isLocal)
        {
            var pageIndex = 0;
            if (StringUtils.EqualsIgnoreCase(type, StlPageItem.TypeFirstPage))//首页
            {
                pageIndex = 0;
            }
            else if (StringUtils.EqualsIgnoreCase(type, StlPageItem.TypeLastPage))//末页
            {
                pageIndex = pageCount - 1;
            }
            else if (StringUtils.EqualsIgnoreCase(type, StlPageItem.TypePreviousPage))//上一页
            {
                pageIndex = currentPageIndex - 1;
            }
            else if (StringUtils.EqualsIgnoreCase(type, StlPageItem.TypeNextPage))//下一页
            {
                pageIndex = currentPageIndex + 1;
            }
            else if (StringUtils.EqualsIgnoreCase(type, StlPageItem.TypePageNavigation) || StringUtils.EqualsIgnoreCase(type, StlPageItem.TypePageSelect))
            {
                pageIndex = index - 1;
            }

            var physicalPath = await GetContentPageFilePathAsync(site, channelId, contentId, pageIndex);
            return await GetSiteUrlByPhysicalPathAsync(site, physicalPath, isLocal);
        }

        public string GetClickStringInSearchPage(string type, string ajaxDivId, int index, int currentPageIndex, int pageCount)
        {
            var clickString = string.Empty;

            if (StringUtils.EqualsIgnoreCase(type, StlPageItem.TypeFirstPage))//首页
            {
                clickString = $"stlRedirect{ajaxDivId}({1})";
            }
            else if (StringUtils.EqualsIgnoreCase(type, StlPageItem.TypeLastPage))//末页
            {
                clickString = $"stlRedirect{ajaxDivId}({pageCount})";
            }
            else if (StringUtils.EqualsIgnoreCase(type, StlPageItem.TypePreviousPage))//上一页
            {
                clickString = $"stlRedirect{ajaxDivId}({currentPageIndex})";
            }
            else if (StringUtils.EqualsIgnoreCase(type, StlPageItem.TypeNextPage))//下一页
            {
                clickString = $"stlRedirect{ajaxDivId}({currentPageIndex + 2})";
            }
            else if (StringUtils.EqualsIgnoreCase(type, StlPageItem.TypePageNavigation))
            {
                clickString = $"stlRedirect{ajaxDivId}({index})";
            }
            else if (StringUtils.EqualsIgnoreCase(type, StlPageItem.TypePageSelect))
            {
                clickString = $"stlRedirect{ajaxDivId}(this.options[this.selectedIndex].value)";
            }

            return clickString;
        }

        public async Task<string> GetJsMethodInDynamicPageAsync(string type, Site site, int channelId, int contentId, int index, int currentPageIndex, int pageCount, bool isPageRefresh, string ajaxDivId, bool isLocal)
        {
            var jsMethod = string.Empty;
            var pageIndex = 0;
            if (StringUtils.EqualsIgnoreCase(type, StlPageItem.TypeFirstPage))//首页
            {
                pageIndex = 0;
            }
            else if (StringUtils.EqualsIgnoreCase(type, StlPageItem.TypeLastPage))//末页
            {
                pageIndex = pageCount - 1;
            }
            else if (StringUtils.EqualsIgnoreCase(type, StlPageItem.TypePreviousPage))//上一页
            {
                pageIndex = currentPageIndex - 1;
            }
            else if (StringUtils.EqualsIgnoreCase(type, StlPageItem.TypeNextPage))//下一页
            {
                pageIndex = currentPageIndex + 1;
            }
            else if (StringUtils.EqualsIgnoreCase(type, StlPageItem.TypePageNavigation) || StringUtils.EqualsIgnoreCase(type, StlPageItem.TypePageSelect))
            {
                pageIndex = index - 1;
            }

            if (isPageRefresh)
            {
                if (StringUtils.EqualsIgnoreCase(type, StlPageItem.TypeFirstPage))//首页
                {
                    jsMethod = $"stlRedirect{ajaxDivId}({1})";
                }
                else if (StringUtils.EqualsIgnoreCase(type, StlPageItem.TypeLastPage))//末页
                {
                    jsMethod = $"stlRedirect{ajaxDivId}({pageCount})";
                }
                else if (StringUtils.EqualsIgnoreCase(type, StlPageItem.TypePreviousPage))//上一页
                {
                    jsMethod = $"stlRedirect{ajaxDivId}({currentPageIndex})";
                }
                else if (StringUtils.EqualsIgnoreCase(type, StlPageItem.TypeNextPage))//下一页
                {
                    jsMethod = $"stlRedirect{ajaxDivId}({currentPageIndex + 2})";
                }
                else if (StringUtils.EqualsIgnoreCase(type, StlPageItem.TypePageNavigation))
                {
                    jsMethod = $"stlRedirect{ajaxDivId}({index})";
                }
                else if (StringUtils.EqualsIgnoreCase(type, StlPageItem.TypePageSelect))
                {
                    jsMethod = $"stlRedirect{ajaxDivId}(this.options[this.selectedIndex].value)";
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(ajaxDivId))
                {
                    if (StringUtils.EqualsIgnoreCase(type, StlPageItem.TypeFirstPage))//首页
                    {
                        jsMethod = $"stlDynamic{ajaxDivId}({1})";
                    }
                    else if (StringUtils.EqualsIgnoreCase(type, StlPageItem.TypeLastPage))//末页
                    {
                        jsMethod = $"stlDynamic{ajaxDivId}({pageCount})";
                    }
                    else if (StringUtils.EqualsIgnoreCase(type, StlPageItem.TypePreviousPage))//上一页
                    {
                        jsMethod = $"stlDynamic{ajaxDivId}({currentPageIndex})";
                    }
                    else if (StringUtils.EqualsIgnoreCase(type, StlPageItem.TypeNextPage))//下一页
                    {
                        jsMethod = $"stlDynamic{ajaxDivId}({currentPageIndex + 2})";
                    }
                    else if (StringUtils.EqualsIgnoreCase(type, StlPageItem.TypePageNavigation))
                    {
                        jsMethod = $"stlDynamic{ajaxDivId}({index})";
                    }
                    else if (StringUtils.EqualsIgnoreCase(type, StlPageItem.TypePageSelect))
                    {
                        jsMethod = $"stlDynamic{ajaxDivId}(this.options[this.selectedIndex].value)";
                    }
                }
                else
                {
                    var nodeInfo = await _databaseManager.ChannelRepository.GetAsync(channelId);
                    var redirectUrl = contentId > 0 ? await GetContentPageFilePathAsync(site, nodeInfo.Id, contentId, pageIndex) : await GetChannelPageFilePathAsync(site, nodeInfo.Id, pageIndex);
                    redirectUrl = await GetSiteUrlByPhysicalPathAsync(site, redirectUrl, isLocal);
                    jsMethod = $"window.location.href='{redirectUrl}';";
                }
            }
            return jsMethod;
        }
    }
}
