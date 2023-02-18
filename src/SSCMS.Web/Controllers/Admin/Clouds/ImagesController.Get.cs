using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SSCMS.Web.Controllers.Admin.Clouds
{
    public partial class ImagesController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (!await _authManager.IsSuperAdminAsync())
            {
                return Unauthorized();
            }

            var config = await _configRepository.GetAsync();
            var cloudType = await _cloudManager.GetCloudTypeAsync();

            return new GetResult
            {
                CloudType = cloudType,
                IsCloudImages = config.IsCloudImages,
            };
        }
    }
}
