using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Models;
using SS.CMS.Abstractions.Repositories;
using SS.CMS.Utils;

namespace SS.CMS.Core.Services
{
    public partial class IdentityManager
    {
        public string AdminLogin(string userName, bool isAutoLogin, IAccessTokenRepository accessTokenRepository)
        {
            if (string.IsNullOrEmpty(userName)) return null;
            var adminInfo = _administratorRepository.GetAdminInfoByUserName(userName);
            if (adminInfo == null || adminInfo.Locked) return null;

            var expiresAt = DateTimeOffset.UtcNow.AddDays(Constants.AccessTokenExpireDays);
            var token = new Token
            {
                UserId = adminInfo.Id,
                UserName = adminInfo.UserName,
                ExpiresAt = expiresAt
            };
            var accessToken = GetToken(token);

            AddAdminLog("管理员登录");

            if (isAutoLogin)
            {
                _context.Response.Cookies.Delete(Constants.CookieAdminToken);
                _context.Response.Cookies.Append(Constants.CookieAdminToken, accessToken, new CookieOptions
                {
                    Expires = expiresAt
                });
            }
            else
            {
                _context.Response.Cookies.Delete(Constants.CookieAdminToken);
                _context.Response.Cookies.Append(Constants.CookieAdminToken, accessToken);
            }

            return accessToken;
        }

        public async Task SignInAsync(AdministratorInfo administratorInfo, bool isPersistent = false)
        {
            if (administratorInfo != null && !administratorInfo.Locked)
            {
                var identity = new ClaimsIdentity(this.GetUserClaims(administratorInfo), CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await _context.SignInAsync(
                  CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties()
                  {
                      IsPersistent = isPersistent
                  }
                );
            }
        }

        public async Task SignOutAsync()
        {
            await _context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        private IEnumerable<Claim> GetUserClaims(AdministratorInfo administratorInfo)
        {
            List<Claim> claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.NameIdentifier, administratorInfo.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, administratorInfo.UserName));

            var roles = _administratorsInRolesRepository.GetRolesForUser(administratorInfo.UserName);

            if (roles != null)
            {
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var permissions = _permissionsInRolesRepository.GetGeneralPermissionList(roles);

                foreach (var permission in permissions)
                {
                    claims.Add(new Claim("Permission", permission));
                }
            }

            return claims;
        }
    }
}
