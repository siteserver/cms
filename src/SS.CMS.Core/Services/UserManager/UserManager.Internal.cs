using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using SS.CMS.Models;
using SS.CMS.Utils;

namespace SS.CMS.Core.Services
{
    public partial class UserManager
    {
        private async Task<IEnumerable<Claim>> GetUserClaimsAsync(UserInfo userInfo)
        {
            var claims = new List<Claim>
            {
                new Claim(AuthTypes.ClaimTypes.UserId, userInfo.Id.ToString()),
                new Claim(AuthTypes.ClaimTypes.UserName, userInfo.UserName)
            };

            var roles = await _userRoleRepository.GetRolesAsync(userInfo.UserName);

            if (roles != null)
            {
                foreach (var role in roles)
                {
                    claims.Add(new Claim(AuthTypes.ClaimTypes.Role, role));
                }
            }

            return claims;
        }
    }
}
