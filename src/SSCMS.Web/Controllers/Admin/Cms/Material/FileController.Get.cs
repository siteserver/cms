using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Enums;
using SSCMS.Core.Utils;
using System.Collections.Generic;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    public partial class FileController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<QueryResult>> Get([FromQuery] QueryRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.MaterialFile))
            {
                return Unauthorized();
            }

            IEnumerable<MaterialGroup> groups;
            int count;
            IEnumerable<MaterialFile> items;

            var site = await _siteRepository.GetAsync(request.SiteId);
            var config = await _configRepository.GetAsync();
            if (config.IsMaterialSiteOnly)
            {
                var group = await _materialGroupRepository.GetSiteGroupAsync(MaterialType.File, request.SiteId);
                groups = new List<MaterialGroup>
                {
                    group
                };
                count = await _materialFileRepository.GetCountAsync(group.Id, request.Keyword);
                items = await _materialFileRepository.GetAllAsync(group.Id, request.Keyword, request.Page, request.PerPage);
            }
            else
            {
                groups = await _materialGroupRepository.GetAllAsync(MaterialType.File);
                count = await _materialFileRepository.GetCountAsync(request.GroupId, request.Keyword);
                items = await _materialFileRepository.GetAllAsync(request.GroupId, request.Keyword, request.Page, request.PerPage);
            }

            return new QueryResult
            {
                IsSiteOnly = config.IsMaterialSiteOnly,
                Groups = groups,
                Count = count,
                Items = items
            };
        }
    }
}
