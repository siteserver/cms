using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class AuthManager
    {
        private ClaimsIdentity GetAdministratorIdentity(Administrator administrator, bool isPersistent)
        {
            return new ClaimsIdentity(new[]
            {
                new Claim(Types.Claims.UserId, administrator.Id.ToString()),
                new Claim(Types.Claims.UserName, administrator.UserName),
                new Claim(Types.Claims.Role, Types.Roles.Administrator),
                new Claim(Types.Claims.IsPersistent, isPersistent.ToString())
            });
        }

        private static string GetTokenCacheKey(Administrator admin)
        {
            return $"admin:{admin.Id}:token";
        }

        private static string GetTokenCacheKey(User user)
        {
            return $"user:{user.Id}:token";
        }

        public string AuthenticateAdministrator(Administrator administrator, bool isPersistent)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_settingsManager.SecurityKey);
            SecurityTokenDescriptor tokenDescriptor;
            var identity = GetAdministratorIdentity(administrator, isPersistent);

            if (isPersistent)
            {
                tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = identity,
                    Expires = DateTime.Now.AddDays(Constants.AccessTokenExpireDays),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };
            }
            else
            {
                tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = identity,
                    Expires = DateTime.Now.AddDays(1),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };
            }

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var tokenString = tokenHandler.WriteToken(token);

            _cacheManager.AddOrUpdate(GetTokenCacheKey(administrator), tokenString);

            return tokenString;
        }

        public async Task<string> RefreshAdministratorTokenAsync(string accessToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_settingsManager.SecurityKey);
            var principal = tokenHandler.ValidateToken(accessToken,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                }, out var validatedToken);
            var jwtToken = validatedToken as JwtSecurityToken;
            if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token passed!");
            }

            var isPersistent = TranslateUtils.ToBool(_principal.Claims.SingleOrDefault(c => c.Type == Types.Claims.IsPersistent)?.Value);

            var administrator = await _databaseManager.AdministratorRepository.GetByUserNameAsync(principal.Identity.Name);
            return AuthenticateAdministrator(administrator, isPersistent);
        }

        private ClaimsIdentity GetUserIdentity(User user, bool isPersistent)
        {
            return new ClaimsIdentity(new[]
            {
                new Claim(Types.Claims.UserId, user.Id.ToString()),
                new Claim(Types.Claims.UserName, user.UserName),
                new Claim(Types.Claims.Role, Types.Roles.User),
                new Claim(Types.Claims.IsPersistent, isPersistent.ToString())
            });
        }

        public string AuthenticateUser(User user, bool isPersistent)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_settingsManager.SecurityKey);
            SecurityTokenDescriptor tokenDescriptor;
            var identity = GetUserIdentity(user, isPersistent);

            if (isPersistent)
            {
                tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = identity,
                    Expires = DateTime.Now.AddDays(Constants.AccessTokenExpireDays),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };
            }
            else
            {
                tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = identity,
                    Expires = DateTime.Now.AddDays(1),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };
            }

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var tokenString = tokenHandler.WriteToken(token);

            _cacheManager.AddOrUpdate(GetTokenCacheKey(user), tokenString);

            return tokenString;
        }

        public async Task<string> RefreshUserTokenAsync(string accessToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_settingsManager.SecurityKey);
            var principal = tokenHandler.ValidateToken(accessToken,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                }, out var validatedToken);
            var jwtToken = validatedToken as JwtSecurityToken;
            if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token passed!");
            }

            var isPersistent = TranslateUtils.ToBool(_principal.Claims.SingleOrDefault(c => c.Type == Types.Claims.IsPersistent)?.Value);

            var user = await _databaseManager.UserRepository.GetByUserNameAsync(principal.Identity.Name);
            return AuthenticateUser(user, isPersistent);
        }
    }
}
