using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SSCMS.Web.Controllers.Home
{
    public partial class ErrorController
    {
        /// <summary>
        /// 前台错误页接口无需登录，因此只返回通用提示，不返回异常信息与堆栈，防止敏感信息泄露。
        /// 详细错误请管理员登录后台在「错误日志」中查看。
        /// </summary>
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] int logId)
        {
            var error = await _errorLogRepository.GetErrorLogAsync(logId);
            if (error == null) return NotFound();

            return new GetResult
            {
                Message = $"系统异常，请稍后重试或联系管理员（错误编号：{error.Id}）",
                StackTrace = string.Empty,
                Summary = string.Empty,
                CreatedDate = error.CreatedDate,
            };
        }
    }
}
