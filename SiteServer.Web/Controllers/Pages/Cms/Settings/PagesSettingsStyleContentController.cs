using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Dto;
using SiteServer.CMS.Dto.Request;
using SiteServer.CMS.Dto.Result;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.ImportExport;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Settings
{
    
    [RoutePrefix("pages/cms/settings/settingsStyleContent")]
    public partial class PagesSettingsStyleContentController : ApiController
    {
        private const string Route = "";
        private const string RouteImport = "actions/import";
        private const string RouteExport = "actions/export";

        [HttpGet, Route(Route)]
        public async Task<GetResult> Get([FromUri] ChannelRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigTableStyles))
            {
                return Request.Unauthorized<GetResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<GetResult>();

            var channel = await DataProvider.ChannelRepository.GetAsync(request.ChannelId);

            var styles = new List<Style>();
            foreach (var style in await DataProvider.TableStyleRepository.GetContentStyleListAsync(site, channel))
            {
                
                styles.Add(new Style
                {
                    Id = style.Id,
                    AttributeName = style.AttributeName,
                    DisplayName = style.DisplayName,
                    InputType = style.InputType.GetDisplayName(),
                    Rules = TranslateUtils.JsonDeserialize<IEnumerable<TableStyleRule>>(style.RuleValues),
                    Taxis = style.Taxis,
                    IsSystem = style.RelatedIdentity != request.ChannelId
                });
            }

            Cascade<int> cascade = null;
            if (request.ChannelId == request.SiteId)
            {
                cascade = await DataProvider.ChannelRepository.GetCascadeAsync(site, channel, async summary =>
                {
                    var count = await DataProvider.ContentRepository.GetCountAsync(site, summary);
                    return new
                    {
                        Count = count
                    };
                });
            }

            var tableName = await DataProvider.ChannelRepository.GetTableNameAsync(site, channel);

            return new GetResult
            {
                Styles = styles,
                TableName = tableName,
                RelatedIdentities = DataProvider.TableStyleRepository.GetRelatedIdentities(channel),
                Channels = cascade
            };
        }

        [HttpDelete, Route(Route)]
        public async Task<ObjectResult<List<Style>>> Delete([FromBody] DeleteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigTableStyles))
            {
                return Request.Unauthorized<ObjectResult<List<Style>>>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<ObjectResult<List<Style>>>();

            var channel = await DataProvider.ChannelRepository.GetAsync(request.ChannelId);
            var tableName = await DataProvider.ChannelRepository.GetTableNameAsync(site, channel);

            await DataProvider.TableStyleRepository.DeleteAsync(request.ChannelId, tableName, request.AttributeName);

            var styles = new List<Style>();
            foreach (var style in await DataProvider.TableStyleRepository.GetContentStyleListAsync(site, channel))
            {
                styles.Add(new Style
                {
                    Id = style.Id,
                    AttributeName = style.AttributeName,
                    DisplayName = style.DisplayName,
                    InputType = style.InputType.GetDisplayName(),
                    Rules = TranslateUtils.JsonDeserialize<IEnumerable<TableStyleRule>>(style.RuleValues),
                    Taxis = style.Taxis,
                    IsSystem = style.RelatedIdentity != request.ChannelId
                });
            }

            return new ObjectResult<List<Style>>
            {
                Value = styles
            };
        }

        [HttpPost, Route(RouteImport)]
        public async Task<BoolResult> Import([FromUri]ChannelRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigTableStyles))
            {
                return Request.Unauthorized<BoolResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<BoolResult>();

            var channel = await DataProvider.ChannelRepository.GetAsync(request.ChannelId);

            var fileName = auth.HttpRequest["fileName"];
            var fileCount = auth.HttpRequest.Files.Count;
            if (fileCount == 0)
            {
                return Request.BadRequest<BoolResult>("请选择有效的文件上传");
            }

            var file = auth.HttpRequest.Files[0];
            if (string.IsNullOrEmpty(fileName)) fileName = Path.GetFileName(file.FileName);

            var sExt = PathUtils.GetExtension(fileName);
            if (!StringUtils.EqualsIgnoreCase(sExt, ".zip"))
            {
                return Request.BadRequest<BoolResult>("导入文件为 Zip 格式，请选择有效的文件上传");
            }

            var filePath = PathUtils.GetTemporaryFilesPath(fileName);
            DirectoryUtils.CreateDirectoryIfNotExists(filePath);
            file.SaveAs(filePath);

            var tableName = await DataProvider.ChannelRepository.GetTableNameAsync(site, channel);

            var directoryPath = await ImportObject.ImportTableStyleByZipFileAsync(tableName, DataProvider.TableStyleRepository.GetRelatedIdentities(channel), filePath);

            FileUtils.DeleteFileIfExists(filePath);
            DirectoryUtils.DeleteDirectoryIfExists(directoryPath);

            await auth.AddSiteLogAsync(request.SiteId, "导入站点字段");

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpPost, Route(RouteExport)]
        public async Task<StringResult> Export([FromBody] ChannelRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigTableStyles))
            {
                return Request.Unauthorized<StringResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<StringResult>();

            var channel = await DataProvider.ChannelRepository.GetAsync(request.ChannelId);
            var tableName = await DataProvider.ChannelRepository.GetTableNameAsync(site, channel);

            var fileName =
                await ExportObject.ExportRootSingleTableStyleAsync(request.SiteId, tableName,
                    DataProvider.TableStyleRepository.GetRelatedIdentities(channel));

            var filePath = PathUtils.GetTemporaryFilesPath(fileName);
            var downloadUrl = PageUtils.GetRootUrlByPhysicalPath(filePath);

            return new StringResult
            {
                Value = downloadUrl
            };
        }
    }
}
