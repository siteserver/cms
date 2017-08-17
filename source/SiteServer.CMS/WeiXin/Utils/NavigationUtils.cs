using System;
using System.Text;
using BaiRong.Core;
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
        public static string GetNavigationUrl(PublishmentSystemInfo publishmentSystemInfo, ENavigationType navigationType, EKeywordType keywordType, int functionId, int channelId, int contentId, string url)
        {
            var navigationUrl = string.Empty;

            if (navigationType == ENavigationType.Url)
            {
                navigationUrl = url;
            }
            else if (navigationType == ENavigationType.Function)
            {
                navigationUrl = KeywordManager.GetFunctionUrl(publishmentSystemInfo, keywordType, functionId);
            }
            else if (navigationType == ENavigationType.Site)
            {
                if (contentId > 0)
                {
                    var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, channelId);
                    var tableName = NodeManager.GetTableName(publishmentSystemInfo, channelId);

                    var contentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, contentId);

                    navigationUrl = PageUtilityWX.GetContentUrl(publishmentSystemInfo, contentInfo);
                }
                else if (channelId > 0)
                {
                    navigationUrl = PageUtilityWX.GetChannelUrl(publishmentSystemInfo, NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, channelId));
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
                var menuHtml = TemplateManager.GetContentByFilePath(menuPath);

                var startIndex = menuHtml.IndexOf("<!--menu-->", StringComparison.Ordinal);
                var endIndex = menuHtml.IndexOf("<!--menu-->", startIndex + 1, StringComparison.Ordinal);
                var menuTemplate = menuHtml.Substring(startIndex, endIndex - startIndex);

                var startSubIndex = menuTemplate.IndexOf("<!--submenu-->", StringComparison.Ordinal);
                var endSubIndex = menuTemplate.IndexOf("<!--submenu-->", startSubIndex + 1, StringComparison.Ordinal);
                var subMenuTemplate = menuTemplate.Substring(startSubIndex, endSubIndex - startSubIndex);

                var menuBuilder = new StringBuilder();
                var menuInfoList = DataProviderWx.WebMenuDao.GetMenuInfoList(publishmentSystemInfo.PublishmentSystemId, 0);

                var index = 0;
                foreach (var menuInfo in menuInfoList)
                {
                    var subMenuBuilder = new StringBuilder();
                    var subMenuInfoList = DataProviderWx.WebMenuDao.GetMenuInfoList(publishmentSystemInfo.PublishmentSystemId, menuInfo.Id);

                    if (subMenuInfoList != null && subMenuInfoList.Count > 0)
                    {
                        menuInfo.NavigationType = ENavigationTypeUtils.GetValue(ENavigationType.Url);
                        menuInfo.Url = PageUtils.UnclickedUrl;

                        foreach (var subMenuInfo in subMenuInfoList)
                        {
                            var subMenu = subMenuTemplate.Replace("{{url}}", GetNavigationUrl(publishmentSystemInfo, ENavigationTypeUtils.GetEnumType(subMenuInfo.NavigationType), EKeywordTypeUtils.GetEnumType(subMenuInfo.KeywordType), subMenuInfo.FunctionId, subMenuInfo.ChannelId, subMenuInfo.ContentId, subMenuInfo.Url));
                            subMenu = subMenu.Replace("{{menuName}}", subMenuInfo.MenuName);
                            subMenuBuilder.Append(subMenu);
                        }
                    }
                    var menu = menuTemplate.Substring(0, startSubIndex) + subMenuBuilder + menuTemplate.Substring(endSubIndex);

                    menu = menu.Replace("{{url}}", GetNavigationUrl(publishmentSystemInfo, ENavigationTypeUtils.GetEnumType(menuInfo.NavigationType), EKeywordTypeUtils.GetEnumType(menuInfo.KeywordType), menuInfo.FunctionId, menuInfo.ChannelId, menuInfo.ContentId, menuInfo.Url));
                    menu = menu.Replace("{{index}}", index.ToString());
                    menu = menu.Replace("{{menuName}}", menuInfo.MenuName);
                    menuBuilder.Append(menu);
                    index++;
                }

                menuHtml = menuHtml.Substring(0, startIndex) + menuBuilder + menuHtml.Substring(endIndex);

                return $@"
<link rel=""stylesheet"" type=""text/css"" href=""{directoryUrl}/style.css"" />
<script type=""text/javascript"" src=""{directoryUrl}/script.js""></script>
{menuHtml}";
            }
            return string.Empty;
        }
	}
}
