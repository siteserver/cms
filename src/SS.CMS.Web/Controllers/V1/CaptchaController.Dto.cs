using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SS.CMS.Web.Controllers.V1
{
    public partial class CaptchaController
    {
        public class CheckRequest
        {
            public string Captcha { get; set; }
        }
    }
}
