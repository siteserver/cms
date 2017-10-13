using BaiRong.Core;

namespace SiteServer.CMS.Controllers.Sys.Stl.Comments
{
    public class ActionsLogout
    {
        public const string Route = "sys/stl/comments/actions/logout";

        public static string GetUrl(string apiUrl)
        {
            return PageUtils.Combine(apiUrl, Route);
        }
    }
}