using BaiRong.Core;

namespace SiteServer.CMS.Controllers.Users
{
    public class ActionsLogout
    {
        public const string Route = "users/actions/logout";

        public static string GetUrl(string apiUrl)
        {
            return PageUtils.Combine(apiUrl, Route);
        }
    }
}