using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions.Repositories;
using SS.CMS.Abstractions.Services;
using SS.CMS.Core.Common;

namespace SS.CMS.Api.Controllers.Admin
{
    [Route("admin")]
    [ApiController]
    public class SyncController : ControllerBase
    {
        public const string Route = "sync";

        private readonly ISettingsManager _settingsManager;
        private readonly IIdentityManager _identityManager;
        private readonly IConfigRepository _configRepository;

        public SyncController(ISettingsManager settingsManager, IIdentityManager identityManager, IConfigRepository configRepository)
        {
            _settingsManager = settingsManager;
            _identityManager = identityManager;
            _configRepository = configRepository;
        }

        [HttpGet(Route)]
        public ActionResult Get()
        {
            if (!_identityManager.IsAdminLoggin) return Unauthorized();

            if (SystemManager.IsNeedInstall(_configRepository))
            {
                return BadRequest("系统未安装，向导被禁用");
            }

            return Ok(new
            {
                Value = true,
                _settingsManager.ConfigInfo.DatabaseVersion,
                SystemManager.ProductVersion
            });
        }

        [HttpPost(Route)]
        public ActionResult Update()
        {
            if (!_identityManager.IsAdminLoggin) return Unauthorized();

            SystemManager.SyncDatabase(_configRepository);

            return Ok(new
            {
                SystemManager.ProductVersion
            });
        }
    }
}