using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Datory.Utils;
using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto.Request;
using SiteServer.Abstractions.Dto.Result;
using SiteServer.API.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Contents
{
    public partial class PagesContentsController
    {
        [HttpPost, Route(RouteColumns)]
        public async Task<BoolResult> Columns([FromBody] ColumnsRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Contents))
            {
                return Request.Unauthorized<BoolResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<BoolResult>();

            var channel = await DataProvider.ChannelRepository.GetAsync(request.ChannelId);
            channel.ListColumns = Utilities.ToString(request.AttributeNames);

            await DataProvider.ChannelRepository.UpdateAsync(channel);

            return new BoolResult
            {
                Value = true
            };
        }

        public class ColumnsRequest : ChannelRequest
        {
            public List<string> AttributeNames { get; set; }
        }
    }
}
