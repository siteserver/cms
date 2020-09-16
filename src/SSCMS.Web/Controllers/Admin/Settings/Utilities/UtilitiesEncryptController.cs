using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Utilities
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class UtilitiesEncryptController : ControllerBase
    {
        private const string Route = "settings/utilitiesEncrypt";

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
            if (!await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsUtilitiesEncrypt))
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
