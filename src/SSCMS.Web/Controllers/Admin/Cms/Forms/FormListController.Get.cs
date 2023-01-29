using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Cms.Forms
{
    public partial class FormListController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.FormList))
            {
                return Unauthorized();
            }

            var forms = await _formRepository.GetFormsAsync(request.SiteId);

            var authFormIds = new List<int>();
            if (await _authManager.IsSiteAdminAsync(request.SiteId))
            {
                authFormIds = forms.Select(x => x.Id).ToList();
            }
            else
            {
                foreach (var form in forms)
                {
                    var formPermission = MenuUtils.GetFormPermission(form.Id);
                    if (await _authManager.HasSitePermissionsAsync(request.SiteId, formPermission))
                    {
                        authFormIds.Add(form.Id);
                    }
                }
            }

            return new GetResult
            {
                Forms = forms,
                AuthFormIds = authFormIds
            };
        }
    }
}
