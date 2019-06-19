using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SS.CMS.Abstractions.Repositories;
using SS.CMS.Abstractions.Services;

namespace SS.CMS.Core.Services
{
    public class IdentityManagerMiddleware
    {
        private readonly RequestDelegate _next;

        public IdentityManagerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ISettingsManager settingsManager, IIdentityManager identityManager, IAdministratorsInRolesRepository administratorsInRolesRepository, IPermissionsInRolesRepository permissionsInRolesRepository)
        {
            var cultureQuery = context.Request.Query["culture"];
            if (!string.IsNullOrWhiteSpace(cultureQuery))
            {
                var culture = new CultureInfo(cultureQuery);

                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
            }

            // await identityManager.Sync();

            // // var principal = new ClaimsPrincipal();
            // // context.User = principal;

            // if (identityManager.AdminInfo != null && !identityManager.AdminInfo.Locked)
            // {
            //     var identity = new ClaimsIdentity(this.GetUserClaims(identityManager.AdminInfo, administratorsInRolesRepository, permissionsInRolesRepository), CookieAuthenticationDefaults.AuthenticationScheme);
            //     var principal = new ClaimsPrincipal(identity);

            //     await context.SignInAsync(
            //       CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties() { IsPersistent = isPersistent }
            //     );
            // }



            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}