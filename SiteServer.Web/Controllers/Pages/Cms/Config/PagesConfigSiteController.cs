using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.API.Controllers.Pages.Cms.Config
{
    [OpenApiIgnore]
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
                    Config = site.Additional,
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
                var request = new AuthenticatedRequest();
                var siteId = request.SiteId;

                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSitePermissions(siteId, ConfigManager.WebSitePermissions.Configration))
                {
                    return Unauthorized();
                }

                var site = await SiteManager.GetSiteAsync(siteId);

                var siteName = request.GetPostString("siteName");
                var charset = ECharsetUtils.GetEnumType(request.GetPostString("charset"));
                var pageSize = request.GetPostInt("pageSize", site.Additional.PageSize);
                var isCreateDoubleClick = request.GetPostBool("isCreateDoubleClick");

                site.SiteName = siteName;
                site.Additional.Charset = ECharsetUtils.GetValue(charset);
                site.Additional.PageSize = pageSize;
                site.Additional.IsCreateDoubleClick = isCreateDoubleClick;

                //修改所有模板编码
                var templateInfoList = DataProvider.TemplateDao.GetTemplateInfoListBySiteId(siteId);
                foreach (var templateInfo in templateInfoList)
                {
                    if (templateInfo.Charset == charset) continue;

                    var templateContent = TemplateManager.GetTemplateContent(site, templateInfo);
                    templateInfo.Charset = charset;
                    DataProvider.TemplateDao.Update(site, templateInfo, templateContent, request.AdminName);
                }

                await DataProvider.SiteDao.UpdateAsync(site);

                await request.AddSiteLogAsync(siteId, "修改站点设置");

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

        [HttpPost, Route(RouteUpload)]
        public IHttpActionResult Upload()
        {
            try
            {
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.Config))
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
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }
    }
}
