using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin
{
    public partial class ErrorController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] int logId)
        {
            var error = await _errorLogRepository.GetErrorLogAsync(logId);
            if (error == null) return NotFound();

            return new GetResult
            {
                Message = StringUtils.Trim(error.Message),
                StackTrace = StringUtils.Trim(error.StackTrace),
                Summary = StringUtils.Trim(error.Summary),
                CreatedDate = error.CreatedDate,
            };
        }
    }
}
