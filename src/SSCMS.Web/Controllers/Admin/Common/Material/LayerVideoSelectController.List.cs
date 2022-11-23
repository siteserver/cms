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
            var vodSettings = await _vodManager.GetVodSettingsAsync();
            var isCloudVod = _vodManager is ICloudManager && vodSettings.IsVod;

            IEnumerable<MaterialGroup> groups = null;
            int count = 0;
            IEnumerable<MaterialVideo> items = null;
            if (!isCloudVod)
            {
                groups = await _materialGroupRepository.GetAllAsync(MaterialType.Video);
                count = await _materialVideoRepository.GetCountAsync(request.GroupId, request.Keyword);
                items = await _materialVideoRepository.GetAllAsync(request.GroupId, request.Keyword, request.Page, request.PerPage);
            }

            return new ListResult
            {
                Groups = groups,
                Count = count,
                Items = items,
                IsCloudVod = isCloudVod
            };
        }
    }
}
