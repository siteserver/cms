using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin
{
    public partial class LoadingController
    {
        [HttpPost, Route(Route)]
        public ActionResult<StringResult> Submit([FromBody] SubmitRequest request)
        {
            // 验签后解密，且只允许跳转到本站相对地址，避免伪造密文/开放重定向（GJ20260924）
            var redirectUrl = _settingsManager.VerifiedDecrypt(request?.RedirectUrl);
            if (!IsLocalUrl(redirectUrl))
            {
                return this.Error("参数签名校验失败");
            }

            return new StringResult
            {
                Value = redirectUrl
            };
        }

        private static bool IsLocalUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;
            if (url.StartsWith("//") || url.StartsWith("/\\")) return false;
            return url.StartsWith("/") && !url.Contains("\r") && !url.Contains("\n");
        }
    }
}
