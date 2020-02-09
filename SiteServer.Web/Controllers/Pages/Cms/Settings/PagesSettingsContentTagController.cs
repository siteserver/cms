using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Datory.Utils;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Dto.Result;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Settings
{
    [RoutePrefix("pages/cms/settings/settingsContentTag")]
    public partial class PagesSettingsContentTagController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<GetResult> Get([FromUri] GetRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigGroups))
            {
                return Request.Unauthorized<GetResult>();
            }

            var tagNames = await DataProvider.ContentTagRepository.GetTagNamesAsync(request.SiteId);
            var pageTagNames = new List<string>();
            var total = tagNames.Count;
            if (total > 0)
            {
                var offset = request.PerPage * (request.Page - 1);
                var limit = request.PerPage;
                pageTagNames = tagNames.Skip(offset).Take(limit).ToList();
            }

            return new GetResult
            {
                Total = total,
                TagNames = pageTagNames
            };
        }

        [HttpDelete, Route(Route)]
        public async Task<BoolResult> Delete([FromBody]DeleteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigGroups))
            {
                return Request.Unauthorized<BoolResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<BoolResult>();

            await DataProvider.ContentTagRepository.DeleteAsync(request.SiteId, request.TagName);

            await auth.AddSiteLogAsync(request.SiteId, "删除内容标签", $"内容标签:{request.TagName}");

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpPost, Route(Route)]
        public async Task<BoolResult> Add([FromBody] SubmitRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigGroups))
            {
                return Request.Unauthorized<BoolResult>();
            }

            foreach (var tagName in request.TagNames)
            {
                await DataProvider.ContentTagRepository.InsertAsync(request.SiteId, tagName);
            }

            await auth.AddSiteLogAsync(request.SiteId, "新增内容标签", $"内容标签:{Utilities.ToString(request.TagNames)}");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}