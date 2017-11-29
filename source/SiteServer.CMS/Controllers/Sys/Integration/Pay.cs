using BaiRong.Core;

namespace SiteServer.CMS.Controllers.Sys.Integration
{
    public class Pay
    {
        public const string Route = "sys/integration/pay/{successUrl}";

        public static string GetUrl(string apiUrl, string successUrl)
        {
            apiUrl = PageUtils.Combine(apiUrl, Route);
            apiUrl = apiUrl.Replace("{successUrl}", TranslateUtils.EncryptStringBySecretKey(successUrl));
            return apiUrl;
        }
    }
}