using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Repositories;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin
{
    [OpenApiIgnore]
    [Route(Constants.ApiAdminPrefix)]
    public partial class ErrorController : ControllerBase
    {
        public const string Route = "error";

        private readonly IErrorLogRepository _errorLogRepository;

        public ErrorController(IErrorLogRepository errorLogRepository)
        {
            _errorLogRepository = errorLogRepository;
        }

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
