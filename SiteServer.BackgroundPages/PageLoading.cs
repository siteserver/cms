using SiteServer.Utils;

namespace SiteServer.BackgroundPages
{
    public class PageLoading : BasePage
    {
        protected override bool IsAccessable => true;
        protected override bool IsSinglePage => true;

        public string GetRedirectUrl()
        {
            return TranslateUtils.DecryptStringBySecretKey(AuthRequest.GetQueryString("redirectUrl"));
        }
    }
}
