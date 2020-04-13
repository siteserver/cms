namespace SSCMS.Web.Controllers.V1
{
    public partial class CaptchaController
    {
        public class CheckRequest
        {
            public string Captcha { get; set; }
            public string Value { get; set; }
        }
    }
}
