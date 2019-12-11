using System;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Cms
{
    [OpenApiIgnore]
    [RoutePrefix("pages/cms/special")]
    public class PagesSpecialController : ApiController
    {
        private const string Route = "";
        private const string RouteDownload = "actions/download";

        [HttpGet, Route(Route)]
        public IHttpActionResult List()
        {
            try
            {
                var request = new AuthenticatedRequest();

                var siteId = request.GetQueryInt("siteId");

                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSitePermissions(siteId,
                        ConfigManager.SitePermissions.Specials))
                {
                    return Unauthorized();
                }

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                var specialInfoList = DataProvider.SpecialDao.GetSpecialInfoList(siteId);

                return Ok(new
                {
                    Value = specialInfoList,
                    SiteUrl = PageUtility.GetSiteUrl(siteInfo, true)
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete, Route(Route)]
        public IHttpActionResult Delete()
        {
            try
            {
                var request = new AuthenticatedRequest();
                var siteId = request.GetPostInt("siteId");
                var specialId = request.GetPostInt("specialId");

                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSitePermissions(siteId,
                        ConfigManager.SitePermissions.Specials))
                {
                    return Unauthorized();
                }

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                var specialInfo = SpecialManager.DeleteSpecialInfo(siteInfo, specialId);

                request.AddSiteLog(siteId,
                    "删除专题",
                    $"专题名称:{specialInfo.Title}");

                var specialInfoList = DataProvider.SpecialDao.GetSpecialInfoList(siteId);

                return Ok(new
                {
                    Value = specialInfoList
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteDownload)]
        public IHttpActionResult Download()
        {
            try
            {
                var request = new AuthenticatedRequest();

                var siteId = request.GetPostInt("siteId");
                var specialId = request.GetPostInt("specialId");

                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSitePermissions(siteId,
                        ConfigManager.SitePermissions.Specials))
                {
                    return Unauthorized();
                }

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                var specialInfo = SpecialManager.GetSpecialInfo(siteId, specialId);

                var directoryPath = SpecialManager.GetSpecialDirectoryPath(siteInfo, specialInfo.Url);
                var srcDirectoryPath = SpecialManager.GetSpecialSrcDirectoryPath(directoryPath);
                var zipFilePath = SpecialManager.GetSpecialZipFilePath(specialInfo.Title, directoryPath);

                FileUtils.DeleteFileIfExists(zipFilePath);
                ZipUtils.CreateZip(zipFilePath, srcDirectoryPath);
                var url = SpecialManager.GetSpecialZipFileUrl(siteInfo, specialInfo);

                return Ok(new
                {
                    Value = url
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
