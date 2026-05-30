using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin
{
    public partial class AgentController
    {
        [HttpPost, Route(RouteProcess)]
        public ActionResult<CacheUtils.Process> Process([FromBody] ProcessRequest request)
        {
            if (request == null)
            {
                return this.Error("系统参数不足");
            }
            if (!TryValidateAgentSecurityKey(request.SecurityKey, out var securityKeyErrorMessage))
            {
                return this.Error(securityKeyErrorMessage);
            }

            var caching = new CacheUtils(_cacheManager);
            return caching.GetProcess(request.Guid);
        }
    }
}
