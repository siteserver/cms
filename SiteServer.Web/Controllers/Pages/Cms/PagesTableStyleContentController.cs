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

namespace SiteServer.API.Controllers.Pages.Cms
{
    
    [RoutePrefix("pages/cms/tableStyleContent")]
    public partial class PagesTableStyleContentController : ApiController
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

            var channel = await ChannelManager.GetChannelAsync(request.SiteId, request.ChannelId);

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
                cascade = await ChannelManager.GetCascadeAsync(site, channel, async (siteInfo, channelInfo) =>
                {
                    var dict = new Dictionary<string, object>
                    {
                        ["count"] = await DataProvider.ContentRepository.GetCountAsync(siteInfo, channelInfo, 0)
                    };
                    return dict;
                });
            }

            var tableName = await ChannelManager.GetTableNameAsync(site, channel);

            return new GetResult
            {
                Styles = styles,
                TableName = tableName,
                RelatedIdentities = DataProvider.TableStyleRepository.GetRelatedIdentities(channel),
                Channels = cascade
            };
        }

        [HttpDelete, Route(Route)]
        public async Task<GenericResult<List<Style>>> Delete([FromBody] DeleteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigTableStyles))
            {
                return Request.Unauthorized<GenericResult<List<Style>>>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<GenericResult<List<Style>>>();

            var channel = await ChannelManager.GetChannelAsync(request.SiteId, request.ChannelId);
            var tableName = await ChannelManager.GetTableNameAsync(site, channel);

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

            return new GenericResult<List<Style>>
            {
                Value = styles
            };
        }

        [HttpPost, Route(RouteImport)]
        public async Task<DefaultResult> Import([FromUri]ChannelRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigTableStyles))
            {
                return Request.Unauthorized<DefaultResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<DefaultResult>();

            var channel = await ChannelManager.GetChannelAsync(request.SiteId, request.ChannelId);

            var fileName = auth.HttpRequest["fileName"];
            var fileCount = auth.HttpRequest.Files.Count;
            if (fileCount == 0)
            {
                return Request.BadRequest<DefaultResult>("请选择有效的文件上传");
            }

            var file = auth.HttpRequest.Files[0];
            if (string.IsNullOrEmpty(fileName)) fileName = Path.GetFileName(file.FileName);

            var sExt = PathUtils.GetExtension(fileName);
            if (!StringUtils.EqualsIgnoreCase(sExt, ".zip"))
            {
                return Request.BadRequest<DefaultResult>("导入文件为 Zip 格式，请选择有效的文件上传");
            }

            var filePath = PathUtils.GetTemporaryFilesPath(fileName);
            DirectoryUtils.CreateDirectoryIfNotExists(filePath);
            file.SaveAs(filePath);

            var tableName = await ChannelManager.GetTableNameAsync(site, channel);

            var directoryPath = await ImportObject.ImportTableStyleByZipFileAsync(tableName, DataProvider.TableStyleRepository.GetRelatedIdentities(channel), filePath);

            FileUtils.DeleteFileIfExists(filePath);
            DirectoryUtils.DeleteDirectoryIfExists(directoryPath);

            await auth.AddSiteLogAsync(request.SiteId, "导入站点字段");

            return new DefaultResult
            {
                Value = true
            };
        }

        [HttpPost, Route(RouteExport)]
        public async Task<GenericResult<string>> Export([FromBody] ChannelRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigTableStyles))
            {
                return Request.Unauthorized<GenericResult<string>>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<GenericResult<string>>();

            var channel = await ChannelManager.GetChannelAsync(request.SiteId, request.ChannelId);
            var tableName = await ChannelManager.GetTableNameAsync(site, channel);

            var fileName =
                await ExportObject.ExportRootSingleTableStyleAsync(tableName,
                    DataProvider.TableStyleRepository.GetRelatedIdentities(channel));

            var filePath = PathUtils.GetTemporaryFilesPath(fileName);
            var downloadUrl = PageUtils.GetRootUrlByPhysicalPath(filePath);

            return new GenericResult<string>
            {
                Value = downloadUrl
            };
        }
    }
}
