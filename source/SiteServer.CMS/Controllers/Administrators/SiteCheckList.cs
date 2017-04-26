using BaiRong.Core;

namespace SiteServer.CMS.Controllers.Administrators
{
    public class SiteCheckList
    {
        public const string Route = "administrators/{userName}/site_check_list";

        public static string GetUrl(string apiUrl, string userName)
        {
            apiUrl = PageUtils.Combine(apiUrl, Route);
            apiUrl = apiUrl.Replace("{userName}", userName);
            return apiUrl;
        }
    }
}