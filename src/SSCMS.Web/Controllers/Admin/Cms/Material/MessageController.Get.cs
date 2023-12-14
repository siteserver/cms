using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Enums;
using SSCMS.Core.Utils;
using System.Collections.Generic;
using SSCMS.Models;
using SSCMS.Configuration;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    public partial class MessageController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<QueryResult>> Get([FromQuery] QueryRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.MaterialMessage))
            {
                return Unauthorized();
            }

            IEnumerable<MaterialGroup> groups;
            int count;
            IEnumerable<MaterialMessage> items;

            var site = await _siteRepository.GetAsync(request.SiteId);
            var config = await _configRepository.GetAsync();
            if (config.IsMaterialSiteOnly)
            {
                var group = await _materialGroupRepository.GetSiteGroupAsync(MaterialType.Message, request.SiteId);
                groups = new List<MaterialGroup>
                {
                    group
                };
                count = await _materialMessageRepository.GetCountAsync(group.Id, request.Keyword);
                items = await _materialMessageRepository.GetAllAsync(group.Id, request.Keyword, request.Page, request.PerPage);
            }
            else
            {
                groups = await _materialGroupRepository.GetAllAsync(MaterialType.Message);
                count = await _materialMessageRepository.GetCountAsync(request.GroupId, request.Keyword);
                items = await _materialMessageRepository.GetAllAsync(request.GroupId, request.Keyword, request.Page, request.PerPage);
            }

            var isWxEnabled = await _wxManager.IsEnabledAsync(site);

            return new QueryResult
            {
                IsSiteOnly = config.IsMaterialSiteOnly,
                IsWxEnabled = isWxEnabled,
                Groups = groups,
                Count = count,
                Messages = items,
            };
        }
    }
}
