using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory.Utils;
using Microsoft.AspNetCore.Mvc;
using SSCMS;
using SSCMS.Dto.Result;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    [Route("admin/cms/settings/settingsContentTag")]
    public partial class SettingsContentTagController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IContentTagRepository _contentTagRepository;

        public SettingsContentTagController(IAuthManager authManager, ISiteRepository siteRepository, IContentTagRepository contentTagRepository)
        {
            _authManager = authManager;
            _siteRepository = siteRepository;
            _contentTagRepository = contentTagRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigGroups))
            {
                return Unauthorized();
            }

            var tagNames = await _contentTagRepository.GetTagNamesAsync(request.SiteId);
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
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigGroups))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            await _contentTagRepository.DeleteAsync(request.SiteId, request.TagName);

            await _authManager.AddSiteLogAsync(request.SiteId, "删除内容标签", $"内容标签:{request.TagName}");

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Add([FromBody] SubmitRequest request)
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigGroups))
            {
                return Unauthorized();
            }

            foreach (var tagName in request.TagNames)
            {
                await _contentTagRepository.InsertAsync(request.SiteId, tagName);
            }

            await _authManager.AddSiteLogAsync(request.SiteId, "新增内容标签", $"内容标签:{Utilities.ToString(request.TagNames)}");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}