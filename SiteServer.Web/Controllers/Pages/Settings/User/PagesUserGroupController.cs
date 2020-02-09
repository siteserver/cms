using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Dto.Request;
using SiteServer.CMS.Dto.Result;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Settings.User
{
    
    [RoutePrefix("pages/settings/userGroup")]
    public partial class PagesUserGroupController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<GetResult> Get()
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUserGroup))
            {
                return Request.Unauthorized<GetResult>();
            }

            return new GetResult
            {
                Groups = await DataProvider.UserGroupRepository.GetUserGroupListAsync(),
                AdminNames = await DataProvider.AdministratorRepository.GetUserNameListAsync()
            };
        }

        [HttpDelete, Route(Route)]
        public async Task<ObjectResult<IEnumerable<UserGroup>>> Delete([FromBody]IdRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUserGroup))
            {
                return Request.Unauthorized<ObjectResult<IEnumerable<UserGroup>>>();
            }

            await DataProvider.UserGroupRepository.DeleteAsync(request.Id);

            return new ObjectResult<IEnumerable<UserGroup>>
            {
                Value = await DataProvider.UserGroupRepository.GetUserGroupListAsync()
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ObjectResult<IEnumerable<UserGroup>>> Submit([FromBody] UserGroup request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUserGroup))
            {
                return Request.Unauthorized<ObjectResult<IEnumerable<UserGroup>>>();
            }

            if (request.Id == -1)
            {
                if (await DataProvider.UserGroupRepository.IsExistsAsync(request.GroupName))
                {
                    return Request.BadRequest<ObjectResult<IEnumerable<UserGroup>>>("保存失败，已存在相同名称的用户组！");
                }

                var groupInfo = new UserGroup
                {
                    GroupName = request.GroupName,
                    AdminName = request.AdminName
                };

                await DataProvider.UserGroupRepository.InsertAsync(groupInfo);

                await auth.AddAdminLogAsync("新增用户组", $"用户组:{groupInfo.GroupName}");
            }
            else if (request.Id == 0)
            {
                var config = await DataProvider.ConfigRepository.GetAsync();

                config.UserDefaultGroupAdminName = request.AdminName;

                await DataProvider.ConfigRepository.UpdateAsync(config);

                await auth.AddAdminLogAsync("修改用户组", "用户组:默认用户组");
            }
            else if (request.Id > 0)
            {
                var groupInfo = await DataProvider.UserGroupRepository.GetUserGroupAsync(request.Id);

                if (groupInfo.GroupName != request.GroupName && await DataProvider.UserGroupRepository.IsExistsAsync(request.GroupName))
                {
                    return Request.BadRequest<ObjectResult<IEnumerable<UserGroup>>>("保存失败，已存在相同名称的用户组！");
                }

                groupInfo.GroupName = request.GroupName;
                groupInfo.AdminName = request.AdminName;

                await DataProvider.UserGroupRepository.UpdateAsync(groupInfo);

                await auth.AddAdminLogAsync("修改用户组", $"用户组:{groupInfo.GroupName}");
            }

            return new ObjectResult<IEnumerable<UserGroup>>
            {
                Value = await DataProvider.UserGroupRepository.GetUserGroupListAsync()
            };
        }
    }
}
