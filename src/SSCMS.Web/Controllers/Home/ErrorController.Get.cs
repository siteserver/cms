using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SSCMS.Web.Controllers.Home
{
    public partial class ErrorController
    {
        [HttpGet, Route(Route)]
        public async Task<GetResult> Get([FromQuery] int logId)
        {
            return new GetResult
            {
                Error = await _errorLogRepository.GetErrorLogAsync(logId)
            };
        }
    }
}
