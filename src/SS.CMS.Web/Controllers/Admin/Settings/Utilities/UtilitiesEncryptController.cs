using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Web.Extensions;

namespace SS.CMS.Web.Controllers.Admin.Settings.Utilities
{
    [Route("admin/settings/utilitiesEncrypt")]
    public partial class UtilitiesEncryptController : ControllerBase
    {
        private const string Route = "";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;

        public UtilitiesEncryptController(ISettingsManager settingsManager, IAuthManager authManager)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<StringResult>> Submit([FromBody] SubmitRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUtilitiesEncrypt))
            {
                return Unauthorized();
            }

            var encoded = request.IsEncrypt
                ? _settingsManager.Encrypt(request.Value)
                : _settingsManager.Decrypt(request.Value);

            if (!request.IsEncrypt && string.IsNullOrEmpty(encoded))
            {
                return this.Error("指定的字符串为非系统加密的字符串");
            }

            return new StringResult
            {
                Value = encoded
            };
        }
    }
}
