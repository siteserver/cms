using System.Collections.Generic;
using System.Security.Claims;
using SS.CMS.Models;

namespace SS.CMS.Core.Services
{
    public partial class UserManager
    {
        private IList<Claim> GetUserClaims(UserInfo userInfo)
        {
            var claims = new List<Claim>
            {
                new Claim(AuthTypes.ClaimTypes.UserId, userInfo.Id.ToString()),
                new Claim(AuthTypes.ClaimTypes.UserName, userInfo.UserName)
            };

            if (!string.IsNullOrEmpty(userInfo.RoleName))
            {
                claims.Add(new Claim(AuthTypes.ClaimTypes.Role, userInfo.RoleName));
            }

            return claims;
        }
    }
}
