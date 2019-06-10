using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Common;

namespace SS.CMS.Api.Controllers.Admin
{
    [Route("admin")]
    [ApiController]
    public class SyncController : ControllerBase
    {
        public const string Route = "sync";

        private readonly IIdentity _identity;

        public SyncController(IIdentity identity)
        {
            _identity = identity;
        }

        [HttpGet(Route)]
        public ActionResult Get()
        {
            if (!_identity.IsAdminLoggin) return Unauthorized();

            if (SystemManager.IsNeedInstall())
            {
                return BadRequest("系统未安装，向导被禁用");
            }

            return Ok(new
            {
                Value = true,
                ConfigManager.Instance.DatabaseVersion,
                SystemManager.ProductVersion
            });
        }

        [HttpPost(Route)]
        public ActionResult Update()
        {
            if (!_identity.IsAdminLoggin) return Unauthorized();

            SystemManager.SyncDatabase();

            return Ok(new
            {
                SystemManager.ProductVersion
            });
        }
    }
}