using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages
{
    [RoutePrefix("pages/error")]
    public class PagesErrorController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<GetResult> Get([FromUri] int logId)
        {
            return new GetResult
            {
                Error = await DataProvider.ErrorLogRepository.GetErrorLogAsync(logId)
            };
        }

        public class GetResult
        {
            public ErrorLog Error { get; set; }
        }
    }
}
