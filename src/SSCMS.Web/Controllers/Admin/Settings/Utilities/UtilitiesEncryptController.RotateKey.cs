using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Utilities
{
    public partial class UtilitiesEncryptController
    {
        /// <summary>
        /// 轮换 SecurityKey（GJ20260924）：仅超级管理员，且需再次输入本人密码。
        /// 轮换后重新生成全部 API AccessToken，并重启服务使 JWT 签名密钥生效（所有登录状态失效）。
        /// </summary>
        [HttpPost, Route(RouteRotateKey)]
        public async Task<ActionResult<BoolResult>> RotateKey([FromBody] RotateKeyRequest request)
        {
            if (!await _authManager.IsSuperAdminAsync() ||
                !await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsUtilitiesEncrypt))
            {
                return Unauthorized();
            }

            var admin = await _authManager.GetAdminAsync();
            var (validAdmin, _, _) = await _administratorRepository.ValidateAsync(admin.UserName, request?.Password, false);
            if (validAdmin == null)
            {
                return this.Error("密码错误，请重新输入");
            }

            var (success, errorMessage) = _settingsManager.RotateSecurityKey();
            if (!success)
            {
                return this.Error(errorMessage);
            }

            // AccessToken 以 SecurityKey 加密存储，旧库可能已泄露，统一重新生成
            var accessTokens = await _accessTokenRepository.GetAccessTokensAsync();
            foreach (var accessToken in accessTokens)
            {
                await _accessTokenRepository.RegenerateAsync(accessToken);
            }

            await _authManager.AddAdminLogAsync("轮换 SecurityKey", $"重新生成 AccessToken {accessTokens.Count} 个");

            // JWT 签名密钥在启动时读取，重启后生效
            _hostApplicationLifetime.StopApplication();

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
