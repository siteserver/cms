﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Services;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home.Common
{
    public partial class AdminLayerViewController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            Administrator admin = null;
            if (!string.IsNullOrEmpty(request.Guid))
            {
                admin = await _administratorRepository.GetByGuidAsync(request.Guid);
            }

            if (admin == null) return this.Error(Constants.ErrorNotFound);

            admin.Remove("confirmPassword");

            var permissions = new AuthManager(_context, _antiforgery, _cacheManager, _settingsManager, _databaseManager);
            await permissions.InitAsync(admin);
            var level = await permissions.GetAdminLevelAsync();
            var siteNames = new List<string>();
            var siteIdListWithPermissions = await permissions.GetSiteIdsAsync();
            foreach (var siteId in siteIdListWithPermissions)
            {
                var site = await _siteRepository.GetAsync(siteId);
                siteNames.Add(site.SiteName);
            }
            var roleNames = await _administratorRepository.GetRolesAsync(admin.UserName);

            return new GetResult
            {
                Administrator = admin,
                Level = level,
                SiteNames = ListUtils.ToString(siteNames, "<br />"),
                RoleNames = roleNames
            };
        }
    }
}
