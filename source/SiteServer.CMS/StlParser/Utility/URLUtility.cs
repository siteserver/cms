using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.StlElement;

namespace SiteServer.CMS.StlParser.Utility
{
    public class UrlUtility
    {
        public static string GetUrlInChannelPage(string type, PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo, int index, int currentPageIndex, int pageCount)
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

            var physicalPath = PathUtility.GetChannelPageFilePath(publishmentSystemInfo, nodeInfo.NodeId, pageIndex);
            return PageUtility.GetPublishmentSystemUrlByPhysicalPath(publishmentSystemInfo, physicalPath);
        }

        public static string GetUrlInContentPage(string type, PublishmentSystemInfo publishmentSystemInfo, int nodeId, int contentId, int index, int currentPageIndex, int pageCount)
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

            var physicalPath = PathUtility.GetContentPageFilePath(publishmentSystemInfo, nodeId, contentId, pageIndex);
            return PageUtility.GetPublishmentSystemUrlByPhysicalPath(publishmentSystemInfo, physicalPath);
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
                clickString = $"stlJump{ajaxDivId}(this)";
            }

            return clickString;
        }

        public static string GetJsMethodInDynamicPage(string type, PublishmentSystemInfo publishmentSystemInfo, int nodeId, int contentId, string pageUrl, int index, int currentPageIndex, int pageCount, bool isPageRefresh, string ajaxDivId)
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
                    jsMethod = $"stlRedirectPage('{pageUrl}', {1})";
                }
                else if (type.ToLower().Equals(StlPageItem.TypeLastPage.ToLower()))//末页
                {
                    jsMethod = $"stlRedirectPage('{pageUrl}', {pageCount})";
                }
                else if (type.ToLower().Equals(StlPageItem.TypePreviousPage.ToLower()))//上一页
                {
                    jsMethod = $"stlRedirectPage('{pageUrl}', {currentPageIndex})";
                }
                else if (type.ToLower().Equals(StlPageItem.TypeNextPage.ToLower()))//下一页
                {
                    jsMethod = $"stlRedirectPage('{pageUrl}', {currentPageIndex + 2})";
                }
                else if (type.ToLower().Equals(StlPageItem.TypePageNavigation.ToLower()))
                {
                    jsMethod = $"stlRedirectPage('{pageUrl}', {index})";
                }
                else if (type.ToLower().Equals(StlPageItem.TypePageSelect.ToLower()))
                {
                    jsMethod = $"stlRedirectPage('{pageUrl}', this.options[this.selectedIndex].value)";
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(ajaxDivId))
                {
                    if (type.ToLower().Equals(StlPageItem.TypeFirstPage.ToLower()))//首页
                    {
                        jsMethod = $"stlDynamic_{ajaxDivId}({1})";
                    }
                    else if (type.ToLower().Equals(StlPageItem.TypeLastPage.ToLower()))//末页
                    {
                        jsMethod = $"stlDynamic_{ajaxDivId}({pageCount})";
                    }
                    else if (type.ToLower().Equals(StlPageItem.TypePreviousPage.ToLower()))//上一页
                    {
                        jsMethod = $"stlDynamic_{ajaxDivId}({currentPageIndex})";
                    }
                    else if (type.ToLower().Equals(StlPageItem.TypeNextPage.ToLower()))//下一页
                    {
                        jsMethod = $"stlDynamic_{ajaxDivId}({currentPageIndex + 2})";
                    }
                    else if (type.ToLower().Equals(StlPageItem.TypePageNavigation.ToLower()))
                    {
                        jsMethod = $"stlDynamic_{ajaxDivId}({index})";
                    }
                    else if (type.ToLower().Equals(StlPageItem.TypePageSelect.ToLower()))
                    {
                        jsMethod = $"stlDynamic_{ajaxDivId}(this.options[this.selectedIndex].value)";
                    }
                }
                else
                {
                    var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, nodeId);
                    var redirectUrl = contentId > 0 ? PathUtility.GetContentPageFilePath(publishmentSystemInfo, nodeInfo.NodeId, contentId, pageIndex) : PathUtility.GetChannelPageFilePath(publishmentSystemInfo, nodeInfo.NodeId, pageIndex);
                    redirectUrl = PageUtility.GetPublishmentSystemUrlByPhysicalPath(publishmentSystemInfo, redirectUrl);
                    jsMethod = $"window.location.href='{redirectUrl}';";
                }
            }
            return jsMethod;
        }
    }
}
