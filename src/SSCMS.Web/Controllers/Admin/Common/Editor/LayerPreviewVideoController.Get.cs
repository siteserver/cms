using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Common.Editor
{
    public partial class LayerPreviewVideoController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] SiteRequest request)
        {
            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error("无法确定内容对应的站点");

            var rootUrl = _pathManager.GetRootUrl();
            var siteUrl = await _pathManager.GetSiteUrlAsync(site, true);

            return new GetResult
            {
                RootUrl = rootUrl,
                SiteUrl = siteUrl
            };
        }
    }
}