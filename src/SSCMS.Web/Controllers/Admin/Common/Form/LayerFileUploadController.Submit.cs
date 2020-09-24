using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Common.Form
{
    public partial class LayerFileUploadController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<List<SubmitResult>>> Submit([FromBody] SubmitRequest request)
        {
            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error("无法确定内容对应的站点");

            var result = new List<SubmitResult>();
            foreach (var filePath in request.FilePaths)
            {
                if (string.IsNullOrEmpty(filePath)) continue;

                var fileName = PathUtils.GetFileName(filePath);

                var virtualUrl = await _pathManager.GetVirtualUrlByPhysicalPathAsync(site, filePath);
                var fileUrl = await _pathManager.ParseSiteUrlAsync(site, virtualUrl, true);

                if (request.IsLibrary)
                {
                    var materialFileName = PathUtils.GetMaterialFileName(fileName);
                    var virtualDirectoryPath = PathUtils.GetMaterialVirtualDirectoryPath(UploadType.Image);

                    var directoryPath = _pathManager.ParsePath(virtualDirectoryPath);
                    var materialFilePath = PathUtils.Combine(directoryPath, materialFileName);
                    DirectoryUtils.CreateDirectoryIfNotExists(materialFilePath);

                    FileUtils.CopyFile(filePath, materialFilePath, true);

                    var file = new MaterialFile
                    {
                        Title = fileName,
                        Url = PageUtils.Combine(virtualDirectoryPath, materialFileName)
                    };

                    await _materialFileRepository.InsertAsync(file);
                }


                result.Add(new SubmitResult
                {
                    FileUrl = fileUrl,
                    FileVirtualUrl = virtualUrl
                });
            }

            var options = TranslateUtils.JsonDeserialize(site.Get<string>(nameof(LayerFileUploadController)), new Options
            {
                IsChangeFileName = true,
                IsLibrary = true
            });

            options.IsChangeFileName = request.IsChangeFileName;
            options.IsLibrary = request.IsLibrary;
            site.Set(nameof(LayerFileUploadController), TranslateUtils.JsonSerialize(options));

            await _siteRepository.UpdateAsync(site);

            return result;
        }
    }
}
