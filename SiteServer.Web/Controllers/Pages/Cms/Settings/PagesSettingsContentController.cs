using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Dto.Request;
using SiteServer.CMS.Dto.Result;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Settings
{
    
    [RoutePrefix("pages/cms/settings/settingsContent")]
    public partial class PagesSettingsContentController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<ObjectResult<Site>> GetConfig([FromUri] SiteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.ConfigContents))
            {
                return Request.Unauthorized<ObjectResult<Site>>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);

            return new ObjectResult<Site>
            {
                Value = site
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ObjectResult<Site>> Submit([FromBody] SubmitRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.ConfigContents))
            {
                return Request.Unauthorized<ObjectResult<Site>>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);

            site.IsSaveImageInTextEditor = request.IsSaveImageInTextEditor;

            var isReCalculate = false;
            if (request.IsAutoPageInTextEditor)
            {
                if (site.IsAutoPageInTextEditor == false)
                {
                    isReCalculate = true;
                }
                else if (site.AutoPageWordNum != request.AutoPageWordNum)
                {
                    isReCalculate = true;
                }
            }

            site.IsAutoPageInTextEditor = request.IsAutoPageInTextEditor;
            site.AutoPageWordNum = request.AutoPageWordNum;
            site.IsContentTitleBreakLine = request.IsContentTitleBreakLine;
            site.IsContentSubTitleBreakLine = request.IsContentSubTitleBreakLine;
            site.IsAutoCheckKeywords = request.IsAutoCheckKeywords;

            site.CheckContentLevel = request.CheckContentLevel;
            site.CheckContentDefaultLevel = request.CheckContentDefaultLevel;

            await DataProvider.SiteRepository.UpdateAsync(site);

            if (isReCalculate)
            {
                await DataProvider.ContentRepository.SetAutoPageContentToSiteAsync(site);
            }

            await auth.AddSiteLogAsync(request.SiteId, "修改内容设置");

            return new ObjectResult<Site>
            {
                Value = site
            };
        }
    }
}
