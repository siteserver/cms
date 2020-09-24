using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Common.Form
{
    public partial class LayerVideoUploadController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<Options>> Get([FromQuery] SiteRequest request)
        {
            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error("无法确定内容对应的站点");

            var options = TranslateUtils.JsonDeserialize(site.Get<string>(nameof(LayerVideoUploadController)), new Options
            {
                IsChangeFileName = true,
                IsLibrary = true,
            });

            return options;
        }
    }
}
