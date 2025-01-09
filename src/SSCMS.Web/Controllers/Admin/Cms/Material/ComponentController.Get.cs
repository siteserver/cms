using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Core.Utils;
using System.Collections.Generic;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    public partial class ComponentController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<QueryResult>> Get([FromQuery] QueryRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.MaterialComponent))
            {
                return Unauthorized();
            }

            IEnumerable<MaterialGroup> groups;
            int count;
            IEnumerable<MaterialComponent> items;

            var site = await _siteRepository.GetAsync(request.SiteId);
            var config = await _configRepository.GetAsync();
            if (config.IsMaterialSiteOnly)
            {
                var group = await _materialGroupRepository.GetSiteGroupAsync(MaterialType.Component, request.SiteId);
                groups = new List<MaterialGroup>
                {
                    group
                };
                count = await _materialComponentRepository.GetCountAsync(group.Id, request.Keyword);
                items = await _materialComponentRepository.GetAllAsync(group.Id, request.Keyword, request.Page, request.PerPage);
            }
            else
            {
                groups = await _materialGroupRepository.GetAllAsync(MaterialType.Component);
                count = await _materialComponentRepository.GetCountAsync(request.GroupId, request.Keyword);
                items = await _materialComponentRepository.GetAllAsync(request.GroupId, request.Keyword, request.Page, request.PerPage);
            }

            return new QueryResult
            {
                IsSiteOnly = config.IsMaterialSiteOnly,
                Groups = groups,
                Count = count,
                Items = items,
                SiteType = site.SiteType
            };
        }
    }
}