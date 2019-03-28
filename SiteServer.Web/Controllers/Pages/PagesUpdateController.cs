using System;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.Packaging;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages
{
    [RoutePrefix("pages/update")]
    public class PagesUpdateController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                if (SystemManager.IsNeedInstall())
                {
                    return BadRequest("系统未安装，向导被禁用");
                }

                return Ok(new
                {
                    Value = true
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(Route)]
        public IHttpActionResult Update()
        {
            var idWithVersion = $"{PackageUtils.PackageIdSsCms}.{SystemManager.Version}";
            var packagePath = PathUtils.GetPackagesPath(idWithVersion);
            var homeDirectory = PathUtils.GetHomeDirectoryPath(string.Empty);
            if (!DirectoryUtils.IsDirectoryExists(homeDirectory) || !FileUtils.IsFileExists(PathUtils.Combine(homeDirectory, "config.js")))
            {
                DirectoryUtils.Copy(PathUtils.Combine(packagePath, DirectoryUtils.Home.DirectoryName), homeDirectory, true);
            }

            SystemManager.SyncDatabase();

            return Ok(new
            {
                SystemManager.Version
            });
        }
    }
}