using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home.Common.Form
{
    public partial class LayerImageUploadController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<Options>> Get([FromQuery] SiteRequest request)
        {
            var options = new Options
            {
                IsEditor = true,
                IsThumb = false,
                ThumbWidth = 1024,
                ThumbHeight = 1024,
                IsLinkToOriginal = true
            };

            if (request.SiteId > 0)
            {
                var siteIds = await _authManager.GetSiteIdsAsync();
                if (!ListUtils.Contains(siteIds, request.SiteId)) return Unauthorized();

                var site = await _siteRepository.GetAsync(request.SiteId);
                if (site == null) return this.Error("无法确定内容对应的站点");

                options = TranslateUtils.JsonDeserialize(site.Get<string>($"Home.{nameof(LayerImageUploadController)}"), options);
            }

            return options;
        }
    }
}