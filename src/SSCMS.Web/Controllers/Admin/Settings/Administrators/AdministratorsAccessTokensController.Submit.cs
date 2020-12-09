using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Models;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Administrators
{
    public partial class AdministratorsAccessTokensController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<TokensResult>> Submit([FromBody] AccessToken itemObj)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsAdministratorsAccessTokens))
            {
                return Unauthorized();
            }

            if (itemObj.Id > 0)
            {
                var tokenInfo = await _accessTokenRepository.GetAsync(itemObj.Id);

                if (tokenInfo.Title != itemObj.Title && await _accessTokenRepository.IsTitleExistsAsync(itemObj.Title))
                {
                    return this.Error("保存失败，已存在相同标题的API密钥！");
                }

                tokenInfo.Title = itemObj.Title;
                tokenInfo.AdminName = itemObj.AdminName;
                tokenInfo.Scopes = itemObj.Scopes;

                await _accessTokenRepository.UpdateAsync(tokenInfo);

                await _authManager.AddAdminLogAsync("修改API密钥", $"Access Token:{tokenInfo.Title}");
            }
            else
            {
                if (await _accessTokenRepository.IsTitleExistsAsync(itemObj.Title))
                {
                    return this.Error("保存失败，已存在相同标题的API密钥！");
                }

                var tokenInfo = new AccessToken
                {
                    Title = itemObj.Title,
                    AdminName = itemObj.AdminName,
                    Scopes = itemObj.Scopes
                };

                await _accessTokenRepository.InsertAsync(tokenInfo);

                await _authManager.AddAdminLogAsync("新增API密钥", $"Access Token:{tokenInfo.Title}");
            }

            var list = await _accessTokenRepository.GetAccessTokensAsync();

            return new TokensResult
            {
                Tokens = list
            };
        }
    }
}
