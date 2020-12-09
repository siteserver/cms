using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Sites
{
    public partial class SitesTablesController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsSitesTables))
            {
                return Unauthorized();
            }

            var nameDict = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

            foreach (var site in await _siteRepository.GetSitesAsync())
            {
                if (nameDict.ContainsKey(site.TableName))
                {
                    var list = nameDict[site.TableName];
                    list.Add(site.SiteName);
                }
                else
                {
                    nameDict[site.TableName] = new List<string> { site.SiteName };
                }
            }

            return new GetResult
            {
                Value = nameDict.Keys.ToList(),
                NameDict = nameDict
            };
        }
    }
}
