using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Home
{
    public partial class RegisterController
    {
        [HttpPost, Route(RouteCaptcha)]
        public StringResult CaptchaNew()
        {
            // 验证码值仅保存在服务端缓存，客户端只拿到随机令牌
            return new StringResult
            {
                Value = CaptchaUtils.Issue(_cacheManager)
            };
        }
    }
}
