using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Core.Common;
using SS.CMS.Repositories;
using SS.CMS.Services.ISettingsManager;
using SS.CMS.Services.ITableManager;
using SS.CMS.Services.IUserManager;

namespace SS.CMS.Api.Controllers.Admin
{
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route("admin")]
    [ApiController]
    public class SyncController : ControllerBase
    {
        public const string Route = "sync";

        private readonly ISettingsManager _settingsManager;
        private readonly IUserManager _userManager;
        private readonly IConfigRepository _configRepository;
        private readonly ITableManager _tableManager;

        public SyncController(ISettingsManager settingsManager, IUserManager userManager, IConfigRepository configRepository, ITableManager tableManager)
        {
            _settingsManager = settingsManager;
            _userManager = userManager;
            _configRepository = configRepository;
            _tableManager = tableManager;
        }

        [HttpGet(Route)]
        public ActionResult Get()
        {
            if (_tableManager.IsNeedInstall())
            {
                return BadRequest("系统未安装，向导被禁用");
            }

            return Ok(new
            {
                Value = true,
                _configRepository.Instance.DatabaseVersion,
                _settingsManager.ProductVersion
            });
        }

        [HttpPost(Route)]
        public ActionResult Update()
        {
            _tableManager.SyncDatabase();

            return Ok(new
            {
                _settingsManager.ProductVersion
            });
        }
    }
}