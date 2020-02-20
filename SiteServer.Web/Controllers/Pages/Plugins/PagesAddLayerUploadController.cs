using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.API.Context;
using SiteServer.CMS.Core;

namespace SiteServer.API.Controllers.Pages.Plugins
{
    
    [RoutePrefix("pages/plugins/addLayerUpload")]
    public class PagesAddLayerUploadController : ApiController
    {
        private const string Route = "";
        private const string RouteUpload = "actions/upload";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> GetConfig()
        {
            var request = await AuthenticatedRequest.GetAuthAsync();
            if (!request.IsAdminLoggin ||
                !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.PluginsAdd))
            {
                return Unauthorized();
            }

            return Ok(new
            {
                Value = true
            });
        }

        [HttpPost, Route(RouteUpload)]
        public async Task<IHttpActionResult> Upload()
        {
            var request = await AuthenticatedRequest.GetAuthAsync();
            if (!request.IsAdminLoggin ||
                !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.PluginsAdd))
            {
                return Unauthorized();
            }

            var fileName = request.HttpRequest["fileName"];

            var fileCount = request.HttpRequest.Files.Count;

            string filePath = null;

            if (fileCount > 0)
            {
                var file = request.HttpRequest.Files[0];

                if (string.IsNullOrEmpty(fileName)) fileName = Path.GetFileName(file.FileName);

                var extendName = fileName.Substring(fileName.LastIndexOf(".", StringComparison.Ordinal)).ToLower();
                if (extendName == ".nupkg")
                {
                    filePath = PathUtility.GetTemporaryFilesPath(fileName);
                    DirectoryUtils.CreateDirectoryIfNotExists(filePath);
                    file.SaveAs(filePath);
                }
            }

            FileInfo fileInfo = null;
            if (!string.IsNullOrEmpty(filePath))
            {
                fileInfo = new FileInfo(filePath);
            }
            if (fileInfo != null)
            {
                return Ok(new
                {
                    fileName,
                    length = fileInfo.Length,
                    ret = 1
                });
            }

            return Ok(new
            {
                ret = 0
            });
        }

        [HttpPost, Route(Route)]
        public async Task<IHttpActionResult> Submit()
        {
            var request = await AuthenticatedRequest.GetAuthAsync();
            if (!request.IsAdminLoggin ||
                !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.PluginsAdd))
            {
                return Unauthorized();
            }

            var fileNames = request.GetPostObject<List<string>>("fileNames");

            foreach (var fileName in fileNames)
            {
                var localFilePath = PathUtility.GetTemporaryFilesPath(fileName);

                //var importObject = new ImportObject(siteId, request.AdminName);
                //importObject.ImportContentsByZipFile(channel, localFilePath, isOverride, isChecked, checkedLevel, request.AdminId, 0, SourceManager.Default);
            }

            await request.AddAdminLogAsync("安装离线插件", string.Empty);

            return Ok(new
            {
                Value = true
            });
        }
    }
}
