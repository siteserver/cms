using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.API.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Settings.Admin
{
    
    [RoutePrefix("pages/settings/adminAccessTokens")]
    public class PagesAdminAccessTokensController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> GetList()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdminAccessTokens))
                {
                    return Unauthorized();
                }

                var adminNames = new List<string>();

                if (await request.AdminPermissionsImpl.IsSuperAdminAsync())
                {
                    adminNames = (await DataProvider.AdministratorRepository.GetUserNameListAsync()).ToList();
                }
                else
                {
                    adminNames.Add(request.AdminName);
                }

                var scopes = new List<string>(Constants.ScopeList);

                foreach (var service in await PluginManager.GetServicesAsync())
                {
                    if (service.IsApiAuthorization)
                    {
                        scopes.Add(service.PluginId);
                    }
                }

                var list = await DataProvider.AccessTokenRepository.GetAccessTokenListAsync();

                return Ok(new
                {
                    Value = list,
                    adminNames,
                    scopes,
                    request.AdminName
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete, Route(Route)]
        public async Task<IHttpActionResult> Delete()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdminAccessTokens))
                {
                    return Unauthorized();
                }

                var id = request.GetPostInt("id");

                await DataProvider.AccessTokenRepository.DeleteAsync(id);
                var list = await DataProvider.AccessTokenRepository.GetAccessTokenListAsync();

                return Ok(new
                {
                    Value = list
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(Route)]
        public async Task<IHttpActionResult> Submit([FromBody] AccessToken itemObj)
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdminAccessTokens))
                {
                    return Unauthorized();
                }

                if (itemObj.Id > 0)
                {
                    var tokenInfo = await DataProvider.AccessTokenRepository.GetAsync(itemObj.Id);

                    if (tokenInfo.Title != itemObj.Title && await DataProvider.AccessTokenRepository.IsTitleExistsAsync(itemObj.Title))
                    {
                        return BadRequest("保存失败，已存在相同标题的API密钥！");
                    }

                    tokenInfo.Title = itemObj.Title;
                    tokenInfo.AdminName = itemObj.AdminName;
                    tokenInfo.Scopes = itemObj.Scopes;

                    await DataProvider.AccessTokenRepository.UpdateAsync(tokenInfo);

                    await request.AddAdminLogAsync("修改API密钥", $"Access Token:{tokenInfo.Title}");
                }
                else
                {
                    if (await DataProvider.AccessTokenRepository.IsTitleExistsAsync(itemObj.Title))
                    {
                        return BadRequest("保存失败，已存在相同标题的API密钥！");
                    }

                    var tokenInfo = new AccessToken
                    {
                        Title = itemObj.Title,
                        AdminName = itemObj.AdminName,
                        Scopes = itemObj.Scopes
                    };

                    await DataProvider.AccessTokenRepository.InsertAsync(tokenInfo);

                    await request.AddAdminLogAsync("新增API密钥", $"Access Token:{tokenInfo.Title}");
                }

                var list = await DataProvider.AccessTokenRepository.GetAccessTokenListAsync();

                return Ok(new
                {
                    Value = list
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
