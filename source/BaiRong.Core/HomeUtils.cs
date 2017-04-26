using System.Collections.Specialized;

namespace BaiRong.Core
{
    public class HomeUtils
    {
        public static string GetUrl(string homeUrl, string relatedUrl) => PageUtils.Combine(homeUrl, relatedUrl);

        public static string GetLoginUrl(string homeUrl, string returnUrl)
        {
            return PageUtils.AddQueryString(PageUtils.Combine(homeUrl, "#/login"), new NameValueCollection
            {
                {"returnUrl", returnUrl}
            });
        }

        public static string GetRegisterUrl(string homeUrl, string returnUrl)
        {
            return PageUtils.AddQueryString(PageUtils.Combine(homeUrl, "#/reg"), new NameValueCollection
            {
                {"returnUrl", returnUrl}
            });
        }

        public static string GetLogoutUrl(string homeUrl, string returnUrl)
        {
            return PageUtils.AddQueryString(PageUtils.Combine(homeUrl, "#/logout"), new NameValueCollection
            {
                {"returnUrl", returnUrl}
            });
        }
    }
}
