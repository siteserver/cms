using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Extensions;

namespace SSCMS.Web.Controllers.Admin.Cms.Open
{
    public partial class SendController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                AuthTypes.SitePermissions.OpenSend))
            {
                return Unauthorized();
            }

            var (success, token, errorMessage) = await _openManager.GetWxAccessTokenAsync(request.SiteId);
            if (!success)
            {
                return this.Error(errorMessage);
            }

            var runOnceAt = DateTime.Now;
            if (request.IsTiming)
            {
                runOnceAt = new DateTime(runOnceAt.Year, runOnceAt.Month, runOnceAt.Day, request.Hour, request.Minute, 0);
                if (!request.IsToday)
                {
                    runOnceAt = runOnceAt.AddDays(1);
                }
            }

            if (request.MaterialType == MaterialType.Text)
            {
                await _openManager.SendAsync(token, request.MaterialType, request.Text, request.IsToAll,
                    request.TagId.ToString(), request.IsTiming, runOnceAt);
            }
            else
            {
                var mediaId = await _openManager.PushMaterialAsync(token, request.MaterialType, request.MaterialId);
                await _openManager.SendAsync(token, request.MaterialType, mediaId, request.IsToAll,
                    request.TagId.ToString(), request.IsTiming, runOnceAt);
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
