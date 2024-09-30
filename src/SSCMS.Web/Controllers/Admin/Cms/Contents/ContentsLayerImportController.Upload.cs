using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Utils;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using System.Collections.Generic;
using SSCMS.Models;
using SSCMS.Core.Utils.Office;
using SSCMS.Core.Utils.Serialization;

namespace SSCMS.Web.Controllers.Admin.Cms.Contents
{
    public partial class ContentsLayerImportController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<UploadResult>> Upload([FromQuery] UploadRequest request, [FromForm] IFormFile file)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.Contents) ||
                !await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, MenuUtils.ContentPermissions.Add))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

            if (file == null)
            {
                return this.Error(Constants.ErrorUpload);
            }

            var fileName = Path.GetFileName(file.FileName);
            var filePath = _pathManager.GetTemporaryFilesPath(fileName);
            var url = string.Empty;
            var columns = new List<string>();
            var styles = new List<TableStyle>();

            if (request.ImportType == "zip")
            {
                if (!FileUtils.IsFileType(FileType.Zip, PathUtils.GetExtension(fileName)))
                {
                    return this.Error(Constants.ErrorUpload);
                }
            }
            else if (request.ImportType == "excel")
            {
                if (!FileUtils.IsFileType(FileType.Xlsx, PathUtils.GetExtension(fileName)))
                {
                    return this.Error(Constants.ErrorUpload);
                }
            }
            else if (request.ImportType == "image")
            {
                if (!FileUtils.IsImage(PathUtils.GetExtension(fileName)))
                {
                    return this.Error(Constants.ErrorUpload);
                }

                (_, filePath, _) = await _pathManager.UploadImageAsync(site, file);
                url = await _pathManager.GetVirtualUrlByPhysicalPathAsync(site, filePath);
            }
            else if (request.ImportType == "txt")
            {
                if (!FileUtils.IsFileType(FileType.Txt, PathUtils.GetExtension(fileName)))
                {
                    return this.Error(Constants.ErrorUpload);
                }
            }

            await _pathManager.UploadAsync(file, filePath);

            if (request.ImportType == "excel")
            {
                var sheet = ExcelUtils.Read(filePath);
                (columns, _) = ExcelUtils.GetColumns(sheet);

                var channel = await _channelRepository.GetAsync(request.ChannelId);
                var tableName = _channelRepository.GetTableName(site, channel);
                var relatedIdentities = _tableStyleRepository.GetRelatedIdentities(channel);
                styles = await _tableStyleRepository.GetTableStylesAsync(tableName, relatedIdentities);

                styles.Insert(0, new TableStyle
                {
                    AttributeName = ExcelObject.BelongsChannel2,
                    DisplayName = "所属栏目2"
                });
                styles.Insert(0, new TableStyle
                {
                    AttributeName = ExcelObject.BelongsChannel1,
                    DisplayName = "所属栏目1"
                });
                styles.Insert(0, new TableStyle
                {
                    AttributeName = "",
                    DisplayName = "<不导入>"
                });

                styles.Add(new TableStyle
                {
                    AttributeName = nameof(Models.Content.AddDate),
                    DisplayName = "添加时间"
                });
                styles.Add(new TableStyle
                {
                    AttributeName = nameof(Models.Content.LinkType),
                    DisplayName = "链接类型"
                });
                styles.Add(new TableStyle
                {
                    AttributeName = nameof(Models.Content.LinkUrl),
                    DisplayName = "外部链接"
                });
            }

            return new UploadResult
            {
                Name = fileName,
                Url = url,
                Columns = columns,
                Styles = styles
            };
        }
    }
}