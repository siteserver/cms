using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Wx
{
    public partial class SendController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.WxSend))
            {
                return Unauthorized();
            }

            var (success, token, errorMessage) = await _wxManager.GetAccessTokenAsync(request.SiteId);
            if (!success)
            {
                return this.Error(errorMessage);
            }

            DateTime? runOnceAt = null;
            if (request.IsTiming)
            {
                var dateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, request.Hour, request.Minute, 0);
                if (!request.IsToday)
                {
                    dateTime = dateTime.AddDays(1);
                }
                runOnceAt = dateTime;
            }

            if (request.MaterialType == MaterialType.Text)
            {
                await _wxManager.MassSendAsync(token, request.MaterialType, request.Text, request.IsToAll,
                    request.TagId.ToString(), runOnceAt);
            }
            else
            {
                var mediaId = await _wxManager.PushMaterialAsync(token, request.MaterialType, request.MaterialId);
                if (string.IsNullOrEmpty(mediaId))
                {
                    return this.Error("操作失败，素材未能上传");
                }
                await _wxManager.MassSendAsync(token, request.MaterialType, mediaId, request.IsToAll,
                    request.TagId.ToString(), runOnceAt);
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
