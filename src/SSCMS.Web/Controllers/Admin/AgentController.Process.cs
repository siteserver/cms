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
            var agentCheck = CheckAgent(request.SecurityKey);
            if (agentCheck != null) return agentCheck;

            var caching = new CacheUtils(_cacheManager);
            return caching.GetProcess(request.Guid);
        }
    }
}