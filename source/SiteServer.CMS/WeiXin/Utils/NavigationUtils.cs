using System.Text;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.IO;
using SiteServer.CMS.WeiXin.Manager;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.CMS.WeiXin.Utils
{
	public class NavigationUtils
    {
        public static string GetNavigationUrl(PublishmentSystemInfo publishmentSystemInfo, ENavigationType navigationType, EKeywordType keywordType, int functionID, int channelID, int contentID, string url)
        {
            var navigationUrl = string.Empty;

            if (navigationType == ENavigationType.Url)
            {
                navigationUrl = url;
            }
            else if (navigationType == ENavigationType.Function)
            {
                navigationUrl = KeywordManager.GetFunctionUrl(publishmentSystemInfo, keywordType, functionID);
            }
            else if (navigationType == ENavigationType.Site)
            {
                if (contentID > 0)
                {
                    var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, channelID);
                    var tableName = NodeManager.GetTableName(publishmentSystemInfo, channelID);

                    var contentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, contentID);

                    navigationUrl = PageUtilityWX.GetContentUrl(publishmentSystemInfo, contentInfo);
                }
                else if (channelID > 0)
                {
                    var nodeNames = NodeManager.GetNodeNameNavigation(publishmentSystemInfo.PublishmentSystemId, channelID);
                    navigationUrl = PageUtilityWX.GetChannelUrl(publishmentSystemInfo, NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, channelID));
                }
            }

            return navigationUrl;
        }

        public static string ParseWebMenu(PublishmentSystemInfo publishmentSystemInfo)
        {
            if (publishmentSystemInfo.Additional.WxIsWebMenu && !string.IsNullOrEmpty(publishmentSystemInfo.Additional.WxWebMenuType))
            {
                var directoryUrl = SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, $"weixin/components/webMenu/{publishmentSystemInfo.Additional.WxWebMenuType}");
                if (PageUtils.IsProtocolUrl(publishmentSystemInfo.PublishmentSystemUrl))
                {
                    directoryUrl = PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, $"weixin/components/webMenu/{publishmentSystemInfo.Additional.WxWebMenuType}"));
                } 
                 
                var menuPath = SiteFilesAssets.GetPath($"weixin/components/webMenu/{publishmentSystemInfo.Additional.WxWebMenuType}/template.html");
                var menuHtml = StlCacheManager.FileContent.GetContentByFilePath(menuPath, ECharset.utf_8);

                var startIndex = menuHtml.IndexOf("<!--menu-->");
                var endIndex = menuHtml.IndexOf("<!--menu-->", startIndex + 1);
                var menuTemplate = menuHtml.Substring(startIndex, endIndex - startIndex);

                var startSubIndex = menuTemplate.IndexOf("<!--submenu-->");
                var endSubIndex = menuTemplate.IndexOf("<!--submenu-->", startSubIndex + 1);
                var subMenuTemplate = menuTemplate.Substring(startSubIndex, endSubIndex - startSubIndex);

                var menuBuilder = new StringBuilder();
                var menuInfoList = DataProviderWX.WebMenuDAO.GetMenuInfoList(publishmentSystemInfo.PublishmentSystemId, 0);

                var index = 0;
                foreach (var menuInfo in menuInfoList)
                {
                    var subMenuBuilder = new StringBuilder();
                    var subMenuInfoList = DataProviderWX.WebMenuDAO.GetMenuInfoList(publishmentSystemInfo.PublishmentSystemId, menuInfo.ID);

                    if (subMenuInfoList != null && subMenuInfoList.Count > 0)
                    {
                        menuInfo.NavigationType = ENavigationTypeUtils.GetValue(ENavigationType.Url);
                        menuInfo.Url = PageUtils.UnclickedUrl;

                        foreach (var subMenuInfo in subMenuInfoList)
                        {
                            var subMenu = subMenuTemplate.Replace("{{url}}", GetNavigationUrl(publishmentSystemInfo, ENavigationTypeUtils.GetEnumType(subMenuInfo.NavigationType), EKeywordTypeUtils.GetEnumType(subMenuInfo.KeywordType), subMenuInfo.FunctionID, subMenuInfo.ChannelID, subMenuInfo.ContentID, subMenuInfo.Url));
                            subMenu = subMenu.Replace("{{menuName}}", subMenuInfo.MenuName);
                            subMenuBuilder.Append(subMenu);
                        }
                    }
                    var menu = menuTemplate.Substring(0, startSubIndex) + subMenuBuilder.ToString() + menuTemplate.Substring(endSubIndex);

                    menu = menu.Replace("{{url}}", GetNavigationUrl(publishmentSystemInfo, ENavigationTypeUtils.GetEnumType(menuInfo.NavigationType), EKeywordTypeUtils.GetEnumType(menuInfo.KeywordType), menuInfo.FunctionID, menuInfo.ChannelID, menuInfo.ContentID, menuInfo.Url));
                    menu = menu.Replace("{{index}}", index.ToString());
                    menu = menu.Replace("{{menuName}}", menuInfo.MenuName);
                    menuBuilder.Append(menu);
                    index++;
                }

                menuHtml = menuHtml.Substring(0, startIndex) + menuBuilder.ToString() + menuHtml.Substring(endIndex);

                return $@"
<link rel=""stylesheet"" type=""text/css"" href=""{directoryUrl}/style.css"" />
<script type=""text/javascript"" src=""{directoryUrl}/script.js""></script>
{menuHtml}";
            }
            return string.Empty;
        }
	}
}
