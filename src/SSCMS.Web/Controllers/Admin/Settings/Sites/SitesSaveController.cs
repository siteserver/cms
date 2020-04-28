using System.Collections.Generic;
using System.Threading.Tasks;
using CacheManager.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Core.Extensions;
using SSCMS.Core.Utils;
using SSCMS.Core.Utils.Serialization;
using SSCMS.Dto;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Sites
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SitesSaveController : ControllerBase
    {
        private const string Route = "settings/sitesSave";
        private const string RouteSettings = "settings/sitesSave/actions/settings";
        private const string RouteFiles = "settings/sitesSave/actions/files";
        private const string RouteActionsData = "settings/sitesSave/actions/data";

        private readonly ICacheManager<CacheUtils.Process> _cacheManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IOldPluginManager _pluginManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;

        public SitesSaveController(ICacheManager<CacheUtils.Process> cacheManager, IAuthManager authManager, IPathManager pathManager, IDatabaseManager databaseManager, IOldPluginManager pluginManager, ISiteRepository siteRepository, IChannelRepository channelRepository)
        {
            _cacheManager = cacheManager;
            _authManager = authManager;
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _pluginManager = pluginManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsSites))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            var templateDir = "T_" + site.SiteName.Replace(" ", "_");

            return new GetResult
            {
                Site = site,
                TemplateDir = templateDir
            };
        }

        [HttpPost, Route(RouteSettings)]
        public async Task<ActionResult<SaveSettingsResult>> SaveSettings([FromBody] SaveRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsSites))
            {
                return Unauthorized();
            }

            var caching = new CacheUtils(_cacheManager);
            var manager = new SiteTemplateManager(_pathManager, _pluginManager, _databaseManager, caching);

            if (manager.IsSiteTemplateDirectoryExists(request.TemplateDir))
            {
                return this.Error("站点模板文件夹已存在，请更换站点模板文件夹！");
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            var sitePath = await _pathManager.GetSitePathAsync(site);
            var directoryNames = DirectoryUtils.GetDirectoryNames(sitePath);

            var directories = new List<string>();
            var siteDirList = await _siteRepository.GetSiteDirListAsync(0);
            foreach (var directoryName in directoryNames)
            {
                var isSiteDirectory = false;
                if (site.Root)
                {
                    foreach (var siteDir in siteDirList)
                    {
                        if (StringUtils.EqualsIgnoreCase(siteDir, directoryName))
                        {
                            isSiteDirectory = true;
                        }
                    }
                }
                if (!isSiteDirectory && !_pathManager.IsSystemDirectory(directoryName))
                {
                    directories.Add(directoryName);
                }
            }

            var files = DirectoryUtils.GetFileNames(sitePath);

            //var fileSystems = FileUtility.GetFileSystemInfoExtendCollection(await _pathManager.GetSitePathAsync(site));
            //foreach (FileSystemInfoExtend fileSystem in fileSystems)
            //{
            //    if (!fileSystem.IsDirectory) continue;

            //    var isSiteDirectory = false;
            //    if (site.Root)
            //    {
            //        foreach (var siteDir in siteDirList)
            //        {
            //            if (StringUtils.EqualsIgnoreCase(siteDir, fileSystem.Name))
            //            {
            //                isSiteDirectory = true;
            //            }
            //        }
            //    }
            //    if (!isSiteDirectory && !_pathManager.IsSystemDirectory(fileSystem.Name))
            //    {
            //        directories.Add(fileSystem.Name);
            //    }
            //}
            //foreach (FileSystemInfoExtend fileSystem in fileSystems)
            //{
            //    if (fileSystem.IsDirectory) continue;
            //    files.Add(fileSystem.Name);
            //}

            return new SaveSettingsResult
            {
                Directories = directories,
                Files = files
            };
        }

        [HttpPost, Route(RouteFiles)]
        public async Task<ActionResult<SaveFilesResult>> SaveFiles([FromBody]SaveRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsSites))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            var caching = new CacheUtils(_cacheManager);
            var exportObject = new ExportObject(_pathManager, _databaseManager, caching, _pluginManager, site);
            var siteTemplatePath = _pathManager.GetSiteTemplatesPath(request.TemplateDir);
            await exportObject.ExportFilesToSiteAsync(siteTemplatePath, request.IsAllFiles, request.CheckedDirectories, request.CheckedFiles, true);

            var channel = await _channelRepository.GetAsync(request.SiteId);
            channel.Children = await _channelRepository.GetChildrenAsync(request.SiteId, request.SiteId);

            return new SaveFilesResult
            {
                Channel = channel
            };
        }

        [HttpPost, Route(RouteActionsData)]
        public async Task<ActionResult<BoolResult>> SaveData([FromBody]SaveRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsSites))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

            var siteTemplatePath = _pathManager.GetSiteTemplatesPath(request.TemplateDir);
            var siteContentDirectoryPath = _pathManager.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.SiteContent);

            var caching = new CacheUtils(_cacheManager);
            var exportObject = new ExportObject(_pathManager, _databaseManager, caching, _pluginManager, site);
            await exportObject.ExportSiteContentAsync(siteContentDirectoryPath, request.IsSaveContents, request.IsSaveAllChannels, request.CheckedChannelIds);

            await SiteTemplateManager.ExportSiteToSiteTemplateAsync(_pathManager, _databaseManager, caching, _pluginManager, site, request.TemplateDir);

            var siteTemplateInfo = new SiteTemplateInfo
            {
                SiteTemplateName = request.TemplateName,
                PicFileName = string.Empty,
                WebSiteUrl = request.WebSiteUrl,
                Description = request.Description
            };
            var xmlPath = _pathManager.GetSiteTemplateMetadataPath(siteTemplatePath,
                DirectoryUtils.SiteTemplates.FileMetadata);
            Serializer.SaveAsXml(siteTemplateInfo, xmlPath);

            return new BoolResult
            {
                Value = true,
            };
        }
    }
}
