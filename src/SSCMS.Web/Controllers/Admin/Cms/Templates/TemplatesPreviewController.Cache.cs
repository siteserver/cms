using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesPreviewController
    {
        [HttpPost, Route(RouteCache)]
        public async Task<ActionResult<BoolResult>> Cache([FromBody] CacheRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.TemplatesPreview))
            {
                return Unauthorized();
            }

            _cacheManager.AddOrUpdateSliding(CacheKey, request.Content, 60);

            //var cacheItem = new CacheItem<string>(CacheKey, request.Content, ExpirationMode.Sliding, TimeSpan.FromHours(1));
            //_cacheManager.AddOrUpdate(cacheItem, _ => request.Content);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
