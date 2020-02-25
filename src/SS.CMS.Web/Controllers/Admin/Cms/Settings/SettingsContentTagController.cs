using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory.Utils;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Framework;

namespace SS.CMS.Web.Controllers.Admin.Cms.Settings
{
    [Route("admin/cms/settings/settingsContentTag")]
    public partial class SettingsContentTagController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;

        public SettingsContentTagController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigGroups))
            {
                return Unauthorized();
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
        public async Task<ActionResult<BoolResult>> Delete([FromBody]DeleteRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigGroups))
            {
                return Unauthorized();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            await DataProvider.ContentTagRepository.DeleteAsync(request.SiteId, request.TagName);

            await auth.AddSiteLogAsync(request.SiteId, "删除内容标签", $"内容标签:{request.TagName}");

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Add([FromBody] SubmitRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigGroups))
            {
                return Unauthorized();
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