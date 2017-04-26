using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.WeiXin.IO
{
    public class PathUtilityWX
    {
        private PathUtilityWX()
        {
        }

        public static string GetWeiXinFilePath(PublishmentSystemInfo publishmentSystemInfo, int keywordID, int resourceID)
        {
            return PathUtils.Combine(PathUtility.GetPublishmentSystemPath(publishmentSystemInfo), "weixin-files",
                $"{keywordID}-{resourceID}.html");
        }

        public static string GetWeiXinTemplateFilePath()
        {
            return PathUtils.GetSiteFilesPath("services/weixin/components/templates/content.html");
        }
    }
}
