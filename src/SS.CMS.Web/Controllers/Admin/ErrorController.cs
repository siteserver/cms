using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Framework;

namespace SS.CMS.Web.Controllers.Admin
{
    [Route(Constants.ApiRoute)]
    public partial class ErrorController : ControllerBase
    {
        public const string Route = "error";

        [HttpGet, Route(Route)]
        public async Task<GetResult> Get([FromQuery] int logId)
        {
            return new GetResult
            {
                Error = await DataProvider.ErrorLogRepository.GetErrorLogAsync(logId)
            };
        }
    }
}
