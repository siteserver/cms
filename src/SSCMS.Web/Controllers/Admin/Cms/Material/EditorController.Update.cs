using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    public partial class EditorController
    {
        [HttpPut, Route(Route)]
        public async Task<ActionResult<BoolResult>> Update([FromBody] UpdateRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.MaterialMessage))
            {
                return Unauthorized();
            }

            await _materialMessageRepository.UpdateAsync(request.MessageId, request.GroupId, request.Items);

            //article.Title = request.Title;
            //article.Body = request.Body;
            //article.ImageUrl = request.ImageUrl;
            //article.Summary = request.Summary;
            //await _materialArticleRepository.UpdateAsync(article);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
