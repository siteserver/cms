using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Common.Material
{
    public partial class LayerVideoSelectController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<ListResult>> List([FromQuery] ListRequest request)
        {
            IEnumerable<MaterialGroup> groups;
            int count;
            IEnumerable<MaterialVideo> items;

            var vodSettings = await _vodManager.GetVodSettingsAsync();
            var isCloudVod = _vodManager is ICloudManager && vodSettings.IsVod;

            var config = await _configRepository.GetAsync();
            if (config.IsMaterialSiteOnly)
            {
                var group = await _materialGroupRepository.GetSiteGroupAsync(MaterialType.Video, request.SiteId);
                groups = new List<MaterialGroup>
                {
                    group
                };
                count = await _materialVideoRepository.GetCountAsync(group.Id, request.Keyword);
                items = await _materialVideoRepository.GetAllAsync(group.Id, request.Keyword, request.Page, request.PerPage);
            }
            else
            {
                groups = await _materialGroupRepository.GetAllAsync(MaterialType.Video);
                count = await _materialVideoRepository.GetCountAsync(request.GroupId, request.Keyword);
                items = await _materialVideoRepository.GetAllAsync(request.GroupId, request.Keyword, request.Page, request.PerPage);
            }

            return new ListResult
            {
                IsCloudVod = isCloudVod,
                IsSiteOnly = config.IsMaterialSiteOnly,
                Groups = groups,
                Count = count,
                Items = items,
            };
        }
    }
}
