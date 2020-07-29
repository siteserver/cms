using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    public partial class EditorController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                AuthTypes.SitePermissions.MaterialMessage))
            {
                return Unauthorized();
            }

            var message = await _materialMessageRepository.GetAsync(request.MessageId);
            var items = message?.Items ?? new List<MaterialMessageItem>();
            if (items.Count == 0)
            {
                items.Add(new MaterialMessageItem
                {
                    Taxis = 1
                });
            }
            var commentTypes = ListUtils.GetSelects<CommentType>();

            return new GetResult
            {
                Items = items,
                CommentTypes = commentTypes
            };
        }
    }
}
