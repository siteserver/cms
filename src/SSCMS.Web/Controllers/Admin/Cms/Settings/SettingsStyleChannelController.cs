using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Datory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Core.Extensions;
using SSCMS.Core.Utils.Serialization;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SettingsStyleChannelController : ControllerBase
    {
        private const string Route = "cms/settings/settingsStyleChannel";
        private const string RouteImport = "cms/settings/settingsStyleChannel/actions/import";
        private const string RouteExport = "cms/settings/settingsStyleChannel/actions/export";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly ITableStyleRepository _tableStyleRepository;

        public SettingsStyleChannelController(IAuthManager authManager, IPathManager pathManager, IDatabaseManager databaseManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository, ITableStyleRepository tableStyleRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _tableStyleRepository = tableStyleRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] ChannelRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    AuthTypes.SitePermissions.SettingsStyleChannel))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.ChannelId);

            var styles = new List<Style>();
            foreach (var style in await _tableStyleRepository.GetChannelStyleListAsync(channel))
            {
                styles.Add(new Style
                {
                    Id = style.Id,
                    AttributeName = style.AttributeName,
                    DisplayName = style.DisplayName,
                    InputType = style.InputType.GetDisplayName(),
                    Rules = TranslateUtils.JsonDeserialize<IEnumerable<InputStyleRule>>(style.RuleValues),
                    Taxis = style.Taxis,
                    IsSystem = style.RelatedIdentity != request.ChannelId
                });
            }

            Cascade<int> cascade = null;
            if (request.ChannelId == request.SiteId)
            {
                cascade = await _channelRepository.GetCascadeAsync(site, channel, async summary =>
                {
                    var count = await _contentRepository.GetCountAsync(site, summary);
                    return new
                    {
                        Count = count
                    };
                });
            }

            return new GetResult
            {
                Styles = styles,
                TableName = _channelRepository.TableName,
                RelatedIdentities = _tableStyleRepository.GetRelatedIdentities(channel),
                Channels = cascade
            };
        }

        [HttpDelete, Route(Route)]
        public async Task<ActionResult<ObjectResult<List<Style>>>> Delete([FromBody] DeleteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    AuthTypes.SitePermissions.SettingsStyleChannel))
            {
                return Unauthorized();
            }

            var channel = await _channelRepository.GetAsync(request.ChannelId);

            await _tableStyleRepository.DeleteAsync(request.ChannelId, _channelRepository.TableName, request.AttributeName);

            var styles = new List<Style>();
            foreach (var style in await _tableStyleRepository.GetChannelStyleListAsync(channel))
            {
                styles.Add(new Style
                {
                    Id = style.Id,
                    AttributeName = style.AttributeName,
                    DisplayName = style.DisplayName,
                    InputType = style.InputType.GetDisplayName(),
                    Rules = TranslateUtils.JsonDeserialize<IEnumerable<InputStyleRule>>(style.RuleValues),
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
        public async Task<ActionResult<BoolResult>> Import([FromQuery] ImportRequest request, [FromForm] IFormFile file)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    AuthTypes.SitePermissions.SettingsStyleChannel))
            {
                return Unauthorized();
            }

            var channel = await _channelRepository.GetAsync(request.ChannelId);

            if (file == null)
            {
                return this.Error("请选择有效的文件上传");
            }

            var fileName = Path.GetFileName(file.FileName);

            var sExt = PathUtils.GetExtension(fileName);
            if (!StringUtils.EqualsIgnoreCase(sExt, ".zip"))
            {
                return this.Error("导入文件为 Zip 格式，请选择有效的文件上传");
            }

            var filePath = _pathManager.GetTemporaryFilesPath(fileName);
            await _pathManager.UploadAsync(file, filePath);

            var directoryPath = await ImportObject.ImportTableStyleByZipFileAsync(_pathManager, _databaseManager, _channelRepository.TableName, _tableStyleRepository.GetRelatedIdentities(channel), filePath);

            FileUtils.DeleteFileIfExists(filePath);
            DirectoryUtils.DeleteDirectoryIfExists(directoryPath);

            await _authManager.AddSiteLogAsync(request.SiteId, "导入站点字段");

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpPost, Route(RouteExport)]
        public async Task<ActionResult<StringResult>> Export([FromBody] ChannelRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    AuthTypes.SitePermissions.SettingsStyleChannel))
            {
                return Unauthorized();
            }

            var channel = await _channelRepository.GetAsync(request.ChannelId);

            var fileName =
                await ExportObject.ExportRootSingleTableStyleAsync(_pathManager, _databaseManager, request.SiteId, _channelRepository.TableName,
                    _tableStyleRepository.GetRelatedIdentities(channel));

            var filePath = _pathManager.GetTemporaryFilesPath(fileName);
            var downloadUrl = _pathManager.GetRootUrlByPhysicalPath(filePath);

            return new StringResult
            {
                Value = downloadUrl
            };
        }
    }
}
