using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsStyleSiteController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.SettingsStyleSite))
            {
                return Unauthorized();
            }

            var tableName = _siteRepository.TableName;
            var relatedIdentities = _tableStyleRepository.GetRelatedIdentities(request.SiteId);
            var styles = await _tableStyleRepository.GetTableStylesAsync(tableName, relatedIdentities);

            var inputTypes = ListUtils.GetSelects<InputType>();

            return new GetResult
            {
                InputTypes = inputTypes,
                TableName = _siteRepository.TableName,
                RelatedIdentities = ListUtils.ToString(relatedIdentities),
                Styles = styles,
            };
        }
    }
}
