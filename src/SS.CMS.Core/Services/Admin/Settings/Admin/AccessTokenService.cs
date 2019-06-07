using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Cache.Content;
using SS.CMS.Core.Common;
using SS.CMS.Core.Models;
using SS.CMS.Core.Packaging;
using SS.CMS.Core.Plugin;
using SS.CMS.Plugin;
using SS.CMS.Utils;
using SS.CMS.Utils.Enumerations;

namespace SS.CMS.Core.Services.Admin.Settings.Admin
{
    public class AccessTokenService : ServiceBase
    {
        public const string Route = "access-token";
        public async Task<ResponseResult<object>> ListAsync(IRequest request, IResponse response)
        {
            if (!request.IsAdminLoggin ||
                    !request.AdminPermissions.HasSystemPermissions(ConfigManager.SettingsPermissions.Admin))
            {
                return Unauthorized();
            }

            IEnumerable<string> adminNames;

            if (request.AdminPermissions.IsSuperAdmin())
            {
                adminNames = await DataProvider.AdministratorDao.GetUserNameListAsync();
            }
            else
            {
                adminNames = new List<string> { request.AdminName };
            }

            var scopes = new List<string>(AccessTokenManager.ScopeList);

            foreach (var service in PluginManager.Services)
            {
                if (service.IsApiAuthorization)
                {
                    scopes.Add(service.PluginId);
                }
            }

            return Ok(new
            {
                Value = DataProvider.AccessTokenDao.GetAll(),
                adminNames,
                scopes,
                request.AdminName
            });
        }

        public async Task<ResponseResult<object>> CreateAsync(IRequest request, IResponse response)
        {
            if (!request.IsAdminLoggin ||
                    !request.AdminPermissions.HasSystemPermissions(ConfigManager.SettingsPermissions.Admin))
            {
                return Unauthorized();
            }

            if (!request.TryGetPost<AccessTokenInfo>(out var itemObj))
            {
                return BadRequest("参数不正确！");
            }

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

            LogUtils.AddAdminLog(request.IpAddress, request.AdminName, "新增API密钥", $"Access Token:{tokenInfo.Title}");

            return Ok(new
            {
                Value = DataProvider.AccessTokenDao.GetAll()
            });
        }

        public async Task<ResponseResult<object>> UpdateAsync(IRequest request, IResponse response)
        {
            if (!request.IsAdminLoggin ||
                    !request.AdminPermissions.HasSystemPermissions(ConfigManager.SettingsPermissions.Admin))
            {
                return Unauthorized();
            }

            if (!request.TryGetPost<AccessTokenInfo>(out var itemObj))
            {
                return BadRequest("参数不正确！");
            }

            var tokenInfo = await DataProvider.AccessTokenDao.GetAsync(itemObj.Id);

            if (tokenInfo.Title != itemObj.Title && await DataProvider.AccessTokenDao.IsTitleExistsAsync(itemObj.Title))
            {
                return BadRequest("保存失败，已存在相同标题的API密钥！");
            }

            tokenInfo.Title = itemObj.Title;
            tokenInfo.AdminName = itemObj.AdminName;
            tokenInfo.Scopes = itemObj.Scopes;

            await DataProvider.AccessTokenDao.UpdateAsync(tokenInfo);

            LogUtils.AddAdminLog(request.IpAddress, request.AdminName, "修改API密钥", $"Access Token:{tokenInfo.Title}");

            return Ok(new
            {
                Value = DataProvider.AccessTokenDao.GetAll()
            });
        }

        public async Task<ResponseResult<object>> DeleteAsync(IRequest request, IResponse response)
        {
            if (!request.IsAdminLoggin ||
                    !request.AdminPermissions.HasSystemPermissions(ConfigManager.SettingsPermissions.Admin))
            {
                return Unauthorized();
            }

            var id = request.GetPostInt("id");

            await DataProvider.AccessTokenDao.DeleteAsync(id);

            return Ok(new
            {
                Value = DataProvider.AccessTokenDao.GetAll()
            });
        }
    }
}