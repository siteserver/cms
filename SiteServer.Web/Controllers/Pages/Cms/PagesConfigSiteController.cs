using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms
{
    
    [RoutePrefix("pages/cms/configSite")]
    public class PagesConfigSiteController : ApiController
    {
        private const string Route = "";
        private const string RouteUpload = "upload";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> GetConfig()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                var siteId = request.SiteId;

                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSitePermissionsAsync(siteId, Constants.SitePermissions.ConfigSite))
                {
                    return Unauthorized();
                }

                var site = await DataProvider.SiteRepository.GetAsync(siteId);

                return Ok(new
                {
                    Value = site,
                    Config = site.ToDictionary(),
                    request.AdminToken
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
                    !await request.AdminPermissionsImpl.HasSitePermissionsAsync(siteId, Constants.SitePermissions.ConfigSite))
                {
                    return Unauthorized();
                }

                var site = await DataProvider.SiteRepository.GetAsync(siteId);

                var siteName = request.GetPostString("siteName");
                var charset = ECharsetUtils.GetEnumType(request.GetPostString("charset"));
                var pageSize = request.GetPostInt("pageSize", site.PageSize);
                var isCreateDoubleClick = request.GetPostBool("isCreateDoubleClick");

                site.SiteName = siteName;
                site.Charset = ECharsetUtils.GetValue(charset);
                site.PageSize = pageSize;
                site.IsCreateDoubleClick = isCreateDoubleClick;

                //修改所有模板编码
                var templateInfoList = await DataProvider.TemplateRepository.GetTemplateListBySiteIdAsync(siteId);
                foreach (var templateInfo in templateInfoList)
                {
                    if (templateInfo.CharsetType == charset) continue;

                    var templateContent = TemplateManager.GetTemplateContent(site, templateInfo);
                    templateInfo.CharsetType = charset;
                    await DataProvider.TemplateRepository.UpdateAsync(site, templateInfo, templateContent, request.AdminName);
                }

                await DataProvider.SiteRepository.UpdateAsync(site);

                await request.AddSiteLogAsync(siteId, "修改站点设置");

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

        [HttpPost, Route(RouteUpload)]
        public async Task<IHttpActionResult> Upload()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                var siteId = request.SiteId;

                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSitePermissionsAsync(siteId, Constants.SitePermissions.ConfigSite))
                {
                    return Unauthorized();
                }

                var adminLogoUrl = string.Empty;

                foreach (string name in HttpContext.Current.Request.Files)
                {
                    var postFile = HttpContext.Current.Request.Files[name];

                    if (postFile == null)
                    {
                        return BadRequest("Could not read image from body");
                    }

                    var fileName = postFile.FileName;
                    var filePath = PathUtils.GetAdminDirectoryPath(fileName);

                    if (!EFileSystemTypeUtils.IsImage(PathUtils.GetExtension(fileName)))
                    {
                        return BadRequest("image file extension is not correct");
                    }

                    postFile.SaveAs(filePath);

                    adminLogoUrl = fileName;
                }

                return Ok(new
                {
                    Value = adminLogoUrl
                });
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAsync(ex);
                return InternalServerError(ex);
            }
        }
    }
}
