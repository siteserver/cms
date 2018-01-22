using System.Web.Http;
using SiteServer.Utils;
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
          
            PageUtils.Redirect(successUrl);
        }
    }
}
