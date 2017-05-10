using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.StlElement;

namespace SiteServer.CMS.StlParser.Utility
{
    public class URLUtility
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

        public static string GetUrlInContentPage(string type, PublishmentSystemInfo publishmentSystemInfo, int nodeID, int contentID, int index, int currentPageIndex, int pageCount)
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

            var physicalPath = PathUtility.GetContentPageFilePath(publishmentSystemInfo, nodeID, contentID, pageIndex);
            return PageUtility.GetPublishmentSystemUrlByPhysicalPath(publishmentSystemInfo, physicalPath);
        }

        public static string GetClickStringInSearchPage(string type, string ajaxDivID, int index, int currentPageIndex, int pageCount)
        {
            var clickString = string.Empty;

            if (type.ToLower().Equals(StlPageItem.TypeFirstPage.ToLower()))//首页
            {
                clickString = $"stlRedirect{ajaxDivID}({1})";
            }
            else if (type.ToLower().Equals(StlPageItem.TypeLastPage.ToLower()))//末页
            {
                clickString = $"stlRedirect{ajaxDivID}({pageCount})";
            }
            else if (type.ToLower().Equals(StlPageItem.TypePreviousPage.ToLower()))//上一页
            {
                clickString = $"stlRedirect{ajaxDivID}({currentPageIndex})";
            }
            else if (type.ToLower().Equals(StlPageItem.TypeNextPage.ToLower()))//下一页
            {
                clickString = $"stlRedirect{ajaxDivID}({currentPageIndex + 2})";
            }
            else if (type.ToLower().Equals(StlPageItem.TypePageNavigation.ToLower()))
            {
                clickString = $"stlRedirect{ajaxDivID}({index})";
            }
            else if (type.ToLower().Equals(StlPageItem.TypePageSelect.ToLower()))
            {
                clickString = $"stlJump{ajaxDivID}(this)";
            }

            return clickString;
        }

        public static string GetJsMethodInDynamicPage(string type, PublishmentSystemInfo publishmentSystemInfo, int nodeID, int contentID, string pageUrl, int index, int currentPageIndex, int pageCount, bool isPageRefresh, string ajaxDivID)
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
                if (!string.IsNullOrEmpty(ajaxDivID))
                {
                    if (type.ToLower().Equals(StlPageItem.TypeFirstPage.ToLower()))//首页
                    {
                        jsMethod = $"stlDynamic_{ajaxDivID}({1})";
                    }
                    else if (type.ToLower().Equals(StlPageItem.TypeLastPage.ToLower()))//末页
                    {
                        jsMethod = $"stlDynamic_{ajaxDivID}({pageCount})";
                    }
                    else if (type.ToLower().Equals(StlPageItem.TypePreviousPage.ToLower()))//上一页
                    {
                        jsMethod = $"stlDynamic_{ajaxDivID}({currentPageIndex})";
                    }
                    else if (type.ToLower().Equals(StlPageItem.TypeNextPage.ToLower()))//下一页
                    {
                        jsMethod = $"stlDynamic_{ajaxDivID}({currentPageIndex + 2})";
                    }
                    else if (type.ToLower().Equals(StlPageItem.TypePageNavigation.ToLower()))
                    {
                        jsMethod = $"stlDynamic_{ajaxDivID}({index})";
                    }
                    else if (type.ToLower().Equals(StlPageItem.TypePageSelect.ToLower()))
                    {
                        jsMethod = $"stlDynamic_{ajaxDivID}(this.options[this.selectedIndex].value)";
                    }
                }
                else
                {
                    var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, nodeID);
                    var redirectUrl = string.Empty;
                    if (contentID > 0)
                    {
                        redirectUrl = PathUtility.GetContentPageFilePath(publishmentSystemInfo, nodeInfo.NodeId, contentID, pageIndex);
                    }
                    else
                    {
                        redirectUrl = PathUtility.GetChannelPageFilePath(publishmentSystemInfo, nodeInfo.NodeId, pageIndex);
                    }
                    redirectUrl = PageUtility.GetPublishmentSystemUrlByPhysicalPath(publishmentSystemInfo, redirectUrl);
                    jsMethod = $"window.location.href='{redirectUrl}';";
                }
            }
            return jsMethod;
        }
    }
}
