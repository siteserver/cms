using System;
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
        public IHttpActionResult GetConfig()
        {
            try
            {
                var request = new AuthenticatedRequest();
                var siteId = request.SiteId;

                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSitePermissions(siteId, ConfigManager.SitePermissions.ConfigContents))
                {
                    return Unauthorized();
                }

                var siteInfo = SiteManager.GetSiteInfo(siteId);

                return Ok(new
                {
                    Value = siteInfo,
                    Config = siteInfo.Additional
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(Route)]
        public IHttpActionResult Submit()
        {
            try
            {
                var request = new AuthenticatedRequest();
                var siteId = request.SiteId;

                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSitePermissions(siteId, ConfigManager.SitePermissions.ConfigContents))
                {
                    return Unauthorized();
                }

                var siteInfo = SiteManager.GetSiteInfo(siteId);

                var isSaveImageInTextEditor = request.GetPostBool("isSaveImageInTextEditor", true);
                var isAutoPageInTextEditor = request.GetPostBool("isAutoPageInTextEditor");
                var autoPageWordNum = request.GetPostInt("autoPageWordNum", siteInfo.Additional.AutoPageWordNum);
                var isContentTitleBreakLine = request.GetPostBool("isContentTitleBreakLine", true);
                var isContentSubTitleBreakLine = request.GetPostBool("isContentSubTitleBreakLine", true);
                var isAutoCheckKeywords = request.GetPostBool("isAutoCheckKeywords", true);
                var isCheckContentLevel = request.GetPostBool("isCheckContentLevel");
                var checkContentLevel = request.GetPostInt("checkContentLevel");
                var checkContentDefaultLevel = request.GetPostInt("checkContentDefaultLevel");

                siteInfo.Additional.IsSaveImageInTextEditor = isSaveImageInTextEditor;

                var isReCalculate = false;
                if (isAutoPageInTextEditor)
                {
                    if (siteInfo.Additional.IsAutoPageInTextEditor == false)
                    {
                        isReCalculate = true;
                    }
                    else if (siteInfo.Additional.AutoPageWordNum != autoPageWordNum)
                    {
                        isReCalculate = true;
                    }
                }

                siteInfo.Additional.IsAutoPageInTextEditor = isAutoPageInTextEditor;
                siteInfo.Additional.AutoPageWordNum = autoPageWordNum;
                siteInfo.Additional.IsContentTitleBreakLine = isContentTitleBreakLine;
                siteInfo.Additional.IsContentSubTitleBreakLine = isContentSubTitleBreakLine;
                siteInfo.Additional.IsAutoCheckKeywords = isAutoCheckKeywords;

                siteInfo.Additional.IsCheckContentLevel = isCheckContentLevel;
                if (siteInfo.Additional.IsCheckContentLevel)
                {
                    siteInfo.Additional.CheckContentLevel = checkContentLevel;
                }
                siteInfo.Additional.CheckContentDefaultLevel = checkContentDefaultLevel;

                DataProvider.SiteDao.Update(siteInfo);

                if (isReCalculate)
                {
                    DataProvider.ContentDao.SetAutoPageContentToSite(siteInfo);
                }

                request.AddSiteLog(siteId, "修改内容设置");

                return Ok(new
                {
                    Value = siteInfo,
                    Config = siteInfo.Additional,
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
