using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Db;
using SiteServer.CMS.Plugin;

namespace SiteServer.API.Controllers.Pages.Settings
{
    [OpenApiIgnore]
    [RoutePrefix("pages/settings/adminAccessTokens")]
    public class PagesAdminAccessTokensController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> GetList()
        {
            try
            {
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.Admin))
                {
                    return Unauthorized();
                }

                var adminNames = new List<string>();

                if (request.AdminPermissionsImpl.IsConsoleAdministrator)
                {
                    adminNames = (await DataProvider.AdministratorDao.GetUserNameListAsync()).ToList();
                }
                else
                {
                    adminNames.Add(request.AdminName);
                }

                var scopes = new List<string>(AccessTokenManager.ScopeList);

                foreach (var service in PluginManager.Services)
                {
                    if (service.IsApiAuthorization)
                    {
                        scopes.Add(service.PluginId);
                    }
                }

                var list = await DataProvider.AccessTokenDao.GetAccessTokenInfoListAsync();

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
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.Admin))
                {
                    return Unauthorized();
                }

                var id = request.GetPostInt("id");

                await DataProvider.AccessTokenDao.DeleteAsync(id);
                var list = await DataProvider.AccessTokenDao.GetAccessTokenInfoListAsync();

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
        public async Task<IHttpActionResult> Submit([FromBody] AccessTokenInfo itemObj)
        {
            try
            {
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.Admin))
                {
                    return Unauthorized();
                }

                if (itemObj.Id > 0)
                {
                    var tokenInfo = await DataProvider.AccessTokenDao.GetAsync(itemObj.Id);

                    if (tokenInfo.Title != itemObj.Title && await DataProvider.AccessTokenDao.IsTitleExistsAsync(itemObj.Title))
                    {
                        return BadRequest("保存失败，已存在相同标题的API密钥！");
                    }

                    tokenInfo.Title = itemObj.Title;
                    tokenInfo.AdminName = itemObj.AdminName;
                    tokenInfo.Scopes = itemObj.Scopes;

                    await DataProvider.AccessTokenDao.UpdateAsync(tokenInfo);

                    await request.AddAdminLogAsync("修改API密钥", $"Access Token:{tokenInfo.Title}");
                }
                else
                {
                    if (await DataProvider.AccessTokenDao.IsTitleExistsAsync(itemObj.Title))
                    {
                        return BadRequest("保存失败，已存在相同标题的API密钥！");
                    }

                    var tokenInfo = new AccessTokenInfo
                    {
                        Title = itemObj.Title,
                        AdminName = itemObj.AdminName,
                        Scopes = itemObj.Scopes
                    };

                    await DataProvider.AccessTokenDao.InsertAsync(tokenInfo);

                    await request.AddAdminLogAsync("新增API密钥", $"Access Token:{tokenInfo.Title}");
                }

                var list = await DataProvider.AccessTokenDao.GetAccessTokenInfoListAsync();

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
