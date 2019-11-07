using System;
using System.Threading.Tasks;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;

namespace SiteServer.API.Controllers.Pages.Cms.Config
{
    [OpenApiIgnore]
    [RoutePrefix("pages/cms/configContent")]
    public class PagesConfigContentController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> GetConfig()
        {
            try
            {
                var request = new AuthenticatedRequest();
                var siteId = request.SiteId;

                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSitePermissions(siteId, ConfigManager.WebSitePermissions.Configration))
                {
                    return Unauthorized();
                }

                var site = await SiteManager.GetSiteAsync(siteId);

                return Ok(new
                {
                    Value = site,
                    Config = site.Additional
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
                var request = new AuthenticatedRequest();
                var siteId = request.SiteId;

                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSitePermissions(siteId, ConfigManager.WebSitePermissions.Configration))
                {
                    return Unauthorized();
                }

                var site = await SiteManager.GetSiteAsync(siteId);

                var isSaveImageInTextEditor = request.GetPostBool("isSaveImageInTextEditor", true);
                var isAutoPageInTextEditor = request.GetPostBool("isAutoPageInTextEditor");
                var autoPageWordNum = request.GetPostInt("autoPageWordNum", site.Additional.AutoPageWordNum);
                var isContentTitleBreakLine = request.GetPostBool("isContentTitleBreakLine", true);
                var isContentSubTitleBreakLine = request.GetPostBool("isContentSubTitleBreakLine", true);
                var isAutoCheckKeywords = request.GetPostBool("isAutoCheckKeywords", true);
                var isCheckContentLevel = request.GetPostBool("isCheckContentLevel");
                var checkContentLevel = request.GetPostInt("checkContentLevel");
                var checkContentDefaultLevel = request.GetPostInt("checkContentDefaultLevel");

                site.Additional.IsSaveImageInTextEditor = isSaveImageInTextEditor;

                var isReCalculate = false;
                if (isAutoPageInTextEditor)
                {
                    if (site.Additional.IsAutoPageInTextEditor == false)
                    {
                        isReCalculate = true;
                    }
                    else if (site.Additional.AutoPageWordNum != autoPageWordNum)
                    {
                        isReCalculate = true;
                    }
                }

                site.Additional.IsAutoPageInTextEditor = isAutoPageInTextEditor;
                site.Additional.AutoPageWordNum = autoPageWordNum;
                site.Additional.IsContentTitleBreakLine = isContentTitleBreakLine;
                site.Additional.IsContentSubTitleBreakLine = isContentSubTitleBreakLine;
                site.Additional.IsAutoCheckKeywords = isAutoCheckKeywords;

                site.Additional.IsCheckContentLevel = isCheckContentLevel;
                if (site.Additional.IsCheckContentLevel)
                {
                    site.Additional.CheckContentLevel = checkContentLevel;
                }
                site.Additional.CheckContentDefaultLevel = checkContentDefaultLevel;

                await DataProvider.SiteDao.UpdateAsync(site);

                if (isReCalculate)
                {
                    DataProvider.ContentDao.SetAutoPageContentToSite(site);
                }

                await request.AddSiteLogAsync(siteId, "修改内容设置");

                return Ok(new
                {
                    Value = site,
                    Config = site.Additional,
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
