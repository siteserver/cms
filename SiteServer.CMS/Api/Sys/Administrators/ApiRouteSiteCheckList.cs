using SiteServer.Utils;

namespace SiteServer.CMS.Api.Sys.Administrators
{
    public class ApiRouteSiteCheckList
    {
        public const string Route = "sys/administrators/{userName}/site_check_list";

        public static string GetUrl(string apiUrl, string userName)
        {
            apiUrl = PageUtils.Combine(apiUrl, Route);
            apiUrl = apiUrl.Replace("{userName}", userName);
            return apiUrl;
        }
    }
}