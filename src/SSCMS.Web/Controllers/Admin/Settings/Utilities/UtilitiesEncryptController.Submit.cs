using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Utilities
{
    public partial class UtilitiesEncryptController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<StringResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsUtilitiesEncrypt))
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
