using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsStyleRelatedFieldController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<IEnumerable<RelatedField>>> Add([FromBody] RelatedField request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                Types.SitePermissions.SettingsStyleRelatedField))
            {
                return Unauthorized();
            }

            await _relatedFieldRepository.InsertAsync(request);

            await _authManager.AddSiteLogAsync(request.SiteId, "新增联动字段");

            return await _relatedFieldRepository.GetRelatedFieldsAsync(request.SiteId);
        }
    }
}
