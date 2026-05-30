using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Stl
{
    public partial class ActionsHitsController
    {
        [HttpPost, Route(Constants.RouteStlActionsHits)]
        public async Task<ActionResult<IntResult>> Submit([FromBody] SubmitRequest request)
        {
            if (request == null)
            {
                return this.Error(Constants.ErrorNotFound);
            }

            if (!TryConsumeRequestQuota(PageUtils.GetIpAddress(Request), out var retryAfterSeconds))
            {
                return this.Error($"请求过于频繁，请在{retryAfterSeconds}秒后重试");
            }

            try
            {
                var hits = await _contentRepository.GetHitsAsync(request.SiteId, request.ChannelId, request.ContentId);
                if (request.AutoIncrease)
                {
                    hits++;
                    await _contentRepository.UpdateHitsAsync(request.SiteId, request.ChannelId, request.ContentId, hits);
                }
                
                return new IntResult
                {
                    Value = hits
                };
            }
            catch (Exception ex)
            {
                return this.Error(ex.Message);
            }
        }
    }
}
