using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;

namespace SiteServer.API.Controllers.Sys
{
    [OpenApiIgnore]
    public class SysErrorController : ApiController
    {
        private const string Route = "sys/errors/{id}";

        [HttpGet, Route(Route)]
        public IHttpActionResult Main(int id)
        {
            return Ok(new
            {
                LogInfo = DataProvider.ErrorLogDao.GetErrorLogInfo(id),
                Version = SystemManager.ProductVersion
            });
        }
    }
}
