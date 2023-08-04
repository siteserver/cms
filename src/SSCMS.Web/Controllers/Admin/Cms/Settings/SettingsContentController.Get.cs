using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Core.Utils;
using System.Collections.Generic;
using SSCMS.Enums;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsContentController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.SettingsContent))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

            var taxisTypes = new List<Select<string>>
            {
                new Select<string>(TaxisType.OrderByTaxisDesc),
                new Select<string>(TaxisType.OrderByAddDate),
                new Select<string>(TaxisType.OrderByAddDateDesc),
                new Select<string>(TaxisType.OrderByTitle),
                new Select<string>(TaxisType.OrderByTitleDesc),
            };

            return new GetResult
            {
                Site = site,
                TaxisTypes = taxisTypes,
            };
        }
    }
}