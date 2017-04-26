using System.Collections.Specialized;
using BaiRong.Core;

namespace SiteServer.BackgroundPages
{
    public class PageLoading : BasePage
    {
        protected override bool IsAccessable => true;
        protected override bool IsSinglePage => true;

        public string GetRedirectUrl()
        {
            var redirectUrl = PageUtils.FilterXss(StringUtils.ValueToUrl(Body.GetQueryString("RedirectUrl"), true));
            if (!string.IsNullOrEmpty(redirectUrl))
            {
                var queryStringOriginal = new NameValueCollection(Request.QueryString);
                queryStringOriginal.Remove("RedirectType");
                queryStringOriginal.Remove("RedirectUrl");

                queryStringOriginal.Add(PageUtils.GetQueryString(redirectUrl));

                var queryString = new NameValueCollection();
                foreach (string name in queryStringOriginal.Keys)
                {
                    //filter xss for load page, update by sessionliang 20160112
                    queryString[name] = PageUtils.FilterXss(queryStringOriginal[name]);
                }

                redirectUrl = PageUtils.GetUrlWithoutQueryString(redirectUrl);
                if (!PageUtils.IsProtocolUrl(redirectUrl) && !redirectUrl.StartsWith("/"))
                {
                    redirectUrl = PageUtils.GetAdminDirectoryUrl(redirectUrl);
                }

                redirectUrl = StringUtils.ValueFromUrl(redirectUrl, true);

                //filter xss for preload page, update by sessionliang 20160112
                //1. get query string with filter xss
                var fxQueryString = PageUtils.GetQueryStringFilterXss(redirectUrl);
                //2. get url without query string
                redirectUrl = PageUtils.GetUrlWithoutQueryString(redirectUrl);
                //3. combin
                redirectUrl = PageUtils.AddQueryString(redirectUrl, fxQueryString);

                return PageUtils.AddQueryString(redirectUrl, queryString).Replace('"', ' ').Replace('\n', ' ');
            }
            return string.Empty;
        }
    }
}
