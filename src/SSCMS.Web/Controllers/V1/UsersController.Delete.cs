﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.V1
{
    public partial class UsersController
    {
        [OpenApiOperation("删除用户 API", "删除用户，使用DELETE发起请求，请求地址为/api/v1/users/{id}")]
        [HttpPost, Route(RouteUserDelete)]
        public async Task<ActionResult<User>> Delete([FromRoute] int id)
        {
            if (!await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeUsers))
            {
                return Unauthorized();
            }
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsUsers))
            {
                return Unauthorized();
            }

            var user = await _userRepository.DeleteAsync(id);

            return user;
        }
    }
}
