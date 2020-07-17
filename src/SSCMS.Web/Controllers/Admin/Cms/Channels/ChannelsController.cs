using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CacheManager.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Core.Utils;
using SSCMS.Core.Utils.Serialization;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Extensions;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Channels
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class ChannelsController : ControllerBase
    {
        private const string Route = "cms/channels/channels";
        private const string RouteGet = "cms/channels/channels/{siteId:int}/{channelId:int}";
        private const string RouteAppend = "cms/channels/channels/actions/append";
        private const string RouteUpload = "cms/channels/channels/actions/upload";
        private const string RouteImport = "cms/channels/channels/actions/import";
        private const string RouteExport = "cms/channels/channels/actions/export";
        private const string RouteOrder = "cms/channels/channels/actions/order";

        private readonly ICacheManager<CacheUtils.Process> _cacheManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ICreateManager _createManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IOldPluginManager _oldPluginManager;
        private readonly IPluginManager _pluginManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IChannelGroupRepository _channelGroupRepository;
        private readonly ITemplateRepository _templateRepository;
        private readonly ITableStyleRepository _tableStyleRepository;

        public ChannelsController(ICacheManager<CacheUtils.Process> cacheManager, IAuthManager authManager, IPathManager pathManager, ICreateManager createManager, IDatabaseManager databaseManager, IOldPluginManager oldPluginManager, IPluginManager pluginManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository, IChannelGroupRepository channelGroupRepository, ITemplateRepository templateRepository, ITableStyleRepository tableStyleRepository)
        {
            _cacheManager = cacheManager;
            _authManager = authManager;
            _pathManager = pathManager;
            _createManager = createManager;
            _databaseManager = databaseManager;
            _oldPluginManager = oldPluginManager;
            _pluginManager = pluginManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _channelGroupRepository = channelGroupRepository;
            _templateRepository = templateRepository;
            _tableStyleRepository = tableStyleRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<ChannelsResult>> List([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    AuthTypes.SitePermissions.Channels))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.SiteId);
            var cascade = await _channelRepository.GetCascadeAsync(site, channel, async summary =>
            {
                var count = await _contentRepository.GetCountAsync(site, summary);
                var groupNames = await _channelRepository.GetGroupNamesAsync(summary.Id);
                return new
                {
                    Count = count,
                    summary.IndexName,
                    GroupNames = groupNames,
                    summary.Taxis,
                    summary.ParentId
                };
            });

            var indexNames = await _channelRepository.GetChannelIndexNamesAsync(request.SiteId);
            var groupNameList = await _channelGroupRepository.GetGroupNamesAsync(request.SiteId);

            var channelTemplates = await _templateRepository.GetTemplatesByTypeAsync(request.SiteId, TemplateType.ChannelTemplate);
            var contentTemplates = await _templateRepository.GetTemplatesByTypeAsync(request.SiteId, TemplateType.ContentTemplate);
            var contentPlugins = _oldPluginManager.GetContentModelPlugins();
            var relatedPlugins = _oldPluginManager.GetAllContentRelatedPlugins(false);

            return new ChannelsResult
            {
                Channel = cascade,
                IndexNames = indexNames,
                GroupNames = groupNameList,
                ChannelTemplates = channelTemplates,
                ContentTemplates = contentTemplates,
                ContentPlugins = contentPlugins,
                RelatedPlugins = relatedPlugins
            };
        }

        [HttpPost, Route(RouteAppend)]
        public async Task<ActionResult<List<int>>> Append([FromBody] AppendRequest request)
        {
            if (!await _authManager.HasChannelPermissionsAsync(request.SiteId, request.ParentId, AuthTypes.ChannelPermissions.Add))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var parent = await _channelRepository.GetAsync(request.ParentId);
            if (parent == null) return this.Error("无法确定父栏目");

            var insertedChannelIdHashtable = new Hashtable { [1] = request.ParentId }; //key为栏目的级别，1为第一级栏目

            var channelTemplateId = request.ChannelTemplateId;
            var contentTemplateId = request.ContentTemplateId;
            if (request.IsParentTemplates)
            {
                channelTemplateId = parent.ChannelTemplateId;
                contentTemplateId = parent.ContentTemplateId;
            }

            var channelNames = request.Channels.Split('\n');
            IList<string> nodeIndexNameList = null;
            var expandedChannelIds = new List<int>
            {
                request.SiteId
            };
            foreach (var item in channelNames)
            {
                if (string.IsNullOrEmpty(item)) continue;

                //count为栏目的级别
                var count = StringUtils.GetStartCount('－', item) == 0 ? StringUtils.GetStartCount('-', item) : StringUtils.GetStartCount('－', item);
                var channelName = item.Substring(count, item.Length - count);
                var indexName = string.Empty;
                count++;

                if (!string.IsNullOrEmpty(channelName) && insertedChannelIdHashtable.Contains(count))
                {
                    if (request.IsIndexName)
                    {
                        indexName = channelName.Trim();
                    }

                    if (channelName.Contains('(') && channelName.Contains(')'))
                    {
                        var length = channelName.IndexOf(')') - channelName.IndexOf('(');
                        if (length > 0)
                        {
                            indexName = channelName.Substring(channelName.IndexOf('(') + 1, length);
                            channelName = channelName.Substring(0, channelName.IndexOf('('));
                        }
                    }
                    channelName = channelName.Trim();
                    indexName = indexName.Trim(' ', '(', ')');
                    if (!string.IsNullOrEmpty(indexName))
                    {
                        if (nodeIndexNameList == null)
                        {
                            nodeIndexNameList = (await _channelRepository.GetIndexNamesAsync(request.SiteId)).ToList();
                        }
                        if (nodeIndexNameList.Contains(indexName))
                        {
                            indexName = string.Empty;
                        }
                        else
                        {
                            nodeIndexNameList.Add(indexName);
                        }
                    }

                    var parentId = (int)insertedChannelIdHashtable[count];

                    var insertedChannelId = await _channelRepository.InsertAsync(request.SiteId, parentId, channelName, indexName, parent.ContentModelPluginId, parent.ContentRelatedPluginIds, channelTemplateId, contentTemplateId);
                    insertedChannelIdHashtable[count + 1] = insertedChannelId;
                    expandedChannelIds.Add(insertedChannelId);

                    await _createManager.CreateChannelAsync(request.SiteId, insertedChannelId);
                }
            }

            return expandedChannelIds;
        }

        [HttpDelete, Route(Route)]
        public async Task<ActionResult<List<int>>> Delete([FromBody] DeleteRequest request)
        {
            if (!await _authManager.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, AuthTypes.ChannelPermissions.Delete))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            if (channel == null) return this.Error("无法确定父栏目");

            var channelIdList = await _channelRepository.GetChannelIdsAsync(request.SiteId, request.ChannelId, ScopeType.All);

            if (request.DeleteFiles)
            {
                await _createManager.DeleteChannelsAsync(site, channelIdList);
            }

            var adminId = _authManager.AdminId;

            foreach (var channelId in channelIdList)
            {
                await _contentRepository.TrashContentsAsync(site, channelId, adminId);
                await _channelRepository.DeleteAsync(site, channelId, adminId);
            }

            await _authManager.AddSiteLogAsync(request.SiteId, "删除栏目", $"栏目:{channel.ChannelName}");

            return new List<int>
            {
                request.SiteId,
                channel.ParentId
            };
        }

        
        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<StringResult>> Upload([FromQuery] int siteId, [FromForm]IFormFile file)
        {
            if (!await _authManager.HasChannelPermissionsAsync(siteId, siteId, AuthTypes.ChannelPermissions.Add))
            {
                return Unauthorized();
            }

            if (file == null)
            {
                return this.Error("请选择有效的文件上传");
            }

            var fileName = Path.GetFileName(file.FileName);

            var sExt = PathUtils.GetExtension(fileName);
            if (!StringUtils.EqualsIgnoreCase(sExt, ".zip"))
            {
                return this.Error("导入文件为Zip格式，请选择有效的文件上传");
            }

            fileName = $"{StringUtils.GetShortGuid(false)}.zip";
            var filePath = _pathManager.GetTemporaryFilesPath(fileName);

            await _pathManager.UploadAsync(file, filePath);

            return new StringResult
            {
                Value = fileName
            };
        }

        [HttpPost, Route(RouteImport)]
        public async Task<ActionResult<List<int>>> Import([FromBody] ImportRequest request)
        {
            if (!await _authManager.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, AuthTypes.ChannelPermissions.Add))
            {
                return Unauthorized();
            }

            try
            {
                var site = await _siteRepository.GetAsync(request.SiteId);
                var filePath = _pathManager.GetTemporaryFilesPath(request.FileName);
                var adminId = _authManager.AdminId;
                var caching = new CacheUtils(_cacheManager);

                var importObject = new ImportObject(_pathManager, _oldPluginManager, _databaseManager, caching, site, adminId);
                await importObject.ImportChannelsAndContentsByZipFileAsync(request.ChannelId, filePath,
                    request.IsOverride, null);

                await _authManager.AddSiteLogAsync(request.SiteId, "导入栏目");
            }
            catch
            {
                return this.Error("压缩包格式不正确，请上传正确的栏目压缩包");
            }

            return new List<int>
            {
                request.SiteId,
                request.ChannelId
            };
        }

        [HttpPost, Route(RouteExport)]
        public async Task<ActionResult<StringResult>> Export([FromBody] ChannelIdsRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, AuthTypes.SitePermissions.Channels))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var caching = new CacheUtils(_cacheManager);
            var exportObject = new ExportObject(_pathManager, _databaseManager, caching, _oldPluginManager, site);
            var fileName = await exportObject.ExportChannelsAsync(request.ChannelIds);
            var filePath = _pathManager.GetTemporaryFilesPath(fileName);
            var url = _pathManager.GetDownloadApiUrl(true, filePath);

            return new StringResult
            {
                Value = url
            };
        }

        [HttpPost, Route(RouteOrder)]
        public async Task<ActionResult<List<int>>> Order([FromBody] OrderRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    AuthTypes.SitePermissions.Channels))
            {
                return Unauthorized();
            }

            if (request.IsUp)
            {
                await _channelRepository.UpdateTaxisUpAsync(request.SiteId, request.ChannelId, request.ParentId, request.Taxis);
            }
            else
            {
                await _channelRepository.UpdateTaxisDownAsync(request.SiteId, request.ChannelId, request.ParentId, request.Taxis);
            }

            return new List<int>
            {
                request.SiteId,
                request.ChannelId
            };
        }
    }
}
