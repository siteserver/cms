using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin
{
    public partial class IndexController
    {
        [Authorize(Roles = Types.Roles.Administrator)]
        [HttpPost, Route(RouteActionsSetLanguage)]
        public ActionResult<BoolResult> SetLanguage([FromBody]SetLanguageRequest request)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(request.Culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
