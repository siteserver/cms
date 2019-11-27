using System;
using System.Threading.Tasks;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Cms.Config
{
    
    [RoutePrefix("pages/cms/configContent")]
    public class PagesConfigContentController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> GetConfig()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                var siteId = request.SiteId;

                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSitePermissionsAsync(siteId, Constants.WebSitePermissions.Configuration))
                {
                    return Unauthorized();
                }

                var site = await DataProvider.SiteDao.GetAsync(siteId);

                return Ok(new
                {
                    Value = site,
                    Config = site.ToDictionary()
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(Route)]
        public async Task<IHttpActionResult> Submit()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                var siteId = request.SiteId;

                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSitePermissionsAsync(siteId, Constants.WebSitePermissions.Configuration))
                {
                    return Unauthorized();
                }

                var site = await DataProvider.SiteDao.GetAsync(siteId);

                var isSaveImageInTextEditor = request.GetPostBool("isSaveImageInTextEditor", true);
                var isAutoPageInTextEditor = request.GetPostBool("isAutoPageInTextEditor");
                var autoPageWordNum = request.GetPostInt("autoPageWordNum", site.AutoPageWordNum);
                var isContentTitleBreakLine = request.GetPostBool("isContentTitleBreakLine", true);
                var isContentSubTitleBreakLine = request.GetPostBool("isContentSubTitleBreakLine", true);
                var isAutoCheckKeywords = request.GetPostBool("isAutoCheckKeywords", true);
                var isCheckContentLevel = request.GetPostBool("isCheckContentLevel");
                var checkContentLevel = request.GetPostInt("checkContentLevel");
                var checkContentDefaultLevel = request.GetPostInt("checkContentDefaultLevel");

                site.IsSaveImageInTextEditor = isSaveImageInTextEditor;

                var isReCalculate = false;
                if (isAutoPageInTextEditor)
                {
                    if (site.IsAutoPageInTextEditor == false)
                    {
                        isReCalculate = true;
                    }
                    else if (site.AutoPageWordNum != autoPageWordNum)
                    {
                        isReCalculate = true;
                    }
                }

                site.IsAutoPageInTextEditor = isAutoPageInTextEditor;
                site.AutoPageWordNum = autoPageWordNum;
                site.IsContentTitleBreakLine = isContentTitleBreakLine;
                site.IsContentSubTitleBreakLine = isContentSubTitleBreakLine;
                site.IsAutoCheckKeywords = isAutoCheckKeywords;

                site.IsCheckContentLevel = isCheckContentLevel;
                if (site.IsCheckContentLevel)
                {
                    site.CheckContentLevel = checkContentLevel;
                }
                site.CheckContentDefaultLevel = checkContentDefaultLevel;

                await DataProvider.SiteDao.UpdateAsync(site);

                if (isReCalculate)
                {
                    await DataProvider.ContentDao.SetAutoPageContentToSiteAsync(site);
                }

                await request.AddSiteLogAsync(siteId, "修改内容设置");

                return Ok(new
                {
                    Value = site,
                    Config = site.ToDictionary(),
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
