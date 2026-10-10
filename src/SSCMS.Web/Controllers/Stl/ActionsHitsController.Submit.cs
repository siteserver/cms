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
                // 前台匿名接口，不返回异常详情
                Console.Error.WriteLine($"Hits submit failed: {ex}");
                return this.Error("请求处理失败");
            }
        }
    }
}
