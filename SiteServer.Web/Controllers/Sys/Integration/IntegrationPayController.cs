using System;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.Http;
using BaiRong.Core;
using SiteServer.CMS.Controllers.Sys.Integration;

namespace SiteServer.API.Controllers.Sys.Integration
{
    [RoutePrefix("api")]
    public class IntegrationPayController : ApiController
    {
        [HttpPost, Route(Pay.Route)]
        public void Main(string successUrl)
        {
            successUrl = TranslateUtils.DecryptStringBySecretKey(successUrl);
          
            HttpContext.Current.Response.Redirect(successUrl);
        }
    }
}
