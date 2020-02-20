using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto.Result;
using SiteServer.API.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Contents
{
    [RoutePrefix("pages/cms/contents/contentsLayerArrange")]
    public partial class PagesContentsLayerArrangeController : ApiController
    {
        private const string Route = "";

        [HttpPost, Route(Route)]
        public async Task<BoolResult> Submit([FromBody] SubmitRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Contents) ||
                !await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentArrange))
            {
                return Request.Unauthorized<BoolResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<BoolResult>();

            var channel = await DataProvider.ChannelRepository.GetAsync(request.ChannelId);
            if (channel == null) return Request.BadRequest<BoolResult>("无法确定内容对应的栏目");

            await DataProvider.ContentRepository.UpdateArrangeTaxisAsync(site, channel, request.AttributeName, request.IsDesc);

            await auth.AddSiteLogAsync(request.SiteId, "批量整理", string.Empty);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
