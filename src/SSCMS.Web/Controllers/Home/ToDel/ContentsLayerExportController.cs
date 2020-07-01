using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CacheManager.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Core.Utils;
using SSCMS.Core.Utils.Office;
using SSCMS.Core.Utils.Serialization;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home.ToDel
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.User)]
    [Route(Constants.ApiHomePrefix + "todel/")]
    public partial class ContentsLayerExportController : ControllerBase
    {
        private const string Route = "contentsLayerExport";

        private readonly ICacheManager<CacheUtils.Process> _cacheManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IOldPluginManager _pluginManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;

        public ContentsLayerExportController(ICacheManager<CacheUtils.Process> cacheManager, IAuthManager authManager, IPathManager pathManager, IOldPluginManager pluginManager, IDatabaseManager databaseManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository)
        {
            _cacheManager = cacheManager;
            _authManager = authManager;
            _pathManager = pathManager;
            _pluginManager = pluginManager;
            _databaseManager = databaseManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] ChannelRequest request)
        {
            if (!await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, AuthTypes.ContentPermissions.View))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            if (channel == null) return NotFound();

            var columnsManager = new ColumnsManager(_databaseManager, _pluginManager, _pathManager);
            var columns = await columnsManager.GetContentListColumnsAsync(site, channel, ColumnsManager.PageType.Contents);

            var (isChecked, checkedLevel) = await CheckManager.GetUserCheckLevelAsync(_authManager, site, request.SiteId);
            var checkedLevels = CheckManager.GetCheckedLevels(site, isChecked, checkedLevel, true);

            return new GetResult
            {
                Columns = columns,
                CheckedLevels = checkedLevels,
                CheckedLevel = checkedLevel
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<SubmitResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, AuthTypes.ContentPermissions.View))
            {
                return Unauthorized();
            }

            var downloadUrl = string.Empty;

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            if (channel == null) return NotFound();

            var columnsManager = new ColumnsManager(_databaseManager, _pluginManager, _pathManager);
            var columns = await columnsManager.GetContentListColumnsAsync(site, channel, ColumnsManager.PageType.Contents);
            var pluginIds = _pluginManager.GetContentPluginIds(channel);
            var pluginColumns = _pluginManager.GetContentColumns(pluginIds);

            var contentInfoList = new List<Content>();
            var ccIds = await _contentRepository.GetSummariesAsync(site, channel, true);
            var count = ccIds.Count;

            var pages = Convert.ToInt32(Math.Ceiling((double)count / site.PageSize));
            if (pages == 0) pages = 1;

            if (count > 0)
            {
                for (var page = 1; page <= pages; page++)
                {
                    var offset = site.PageSize * (page - 1);
                    var limit = site.PageSize;
                    var pageCcIds = ccIds.Skip(offset).Take(limit).ToList();

                    var sequence = offset + 1;

                    foreach (var channelContentId in pageCcIds)
                    {
                        var contentInfo = await _contentRepository.GetAsync(site, channelContentId.ChannelId, channelContentId.Id);
                        if (contentInfo == null) continue;

                        if (!request.IsAllCheckedLevel)
                        {
                            var checkedLevel = contentInfo.CheckedLevel;
                            if (contentInfo.Checked)
                            {
                                checkedLevel = site.CheckContentLevel;
                            }
                            if (!request.CheckedLevelKeys.Contains(checkedLevel))
                            {
                                continue;
                            }
                        }

                        if (!request.IsAllDate)
                        {
                            if (contentInfo.AddDate < request.StartDate || contentInfo.AddDate > request.EndDate)
                            {
                                continue;
                            }
                        }

                        contentInfoList.Add(await columnsManager.CalculateContentListAsync(sequence++, site, request.ChannelId, contentInfo, columns, pluginColumns));
                    }
                }

                if (contentInfoList.Count > 0)
                {
                    if (request.ExportType == "zip")
                    {
                        var fileName = $"{channel.ChannelName}.zip";
                        var filePath = _pathManager.GetTemporaryFilesPath(fileName);

                        var caching = new CacheUtils(_cacheManager);
                        var exportObject = new ExportObject(_pathManager, _databaseManager, caching, _pluginManager, site);
                        contentInfoList.Reverse();
                        if (await exportObject.ExportContentsAsync(filePath, contentInfoList))
                        {
                            downloadUrl = _pathManager.GetTemporaryFilesUrl(fileName);
                        }
                    }
                    else if (request.ExportType == "excel")
                    {
                        var fileName = $"{channel.ChannelName}.csv";
                        var filePath = _pathManager.GetTemporaryFilesPath(fileName);

                        var excelObject = new ExcelObject(_databaseManager, _pluginManager, _pathManager);

                        await excelObject.CreateExcelFileForContentsAsync(filePath, site, channel, contentInfoList, request.ColumnNames);
                        downloadUrl = _pathManager.GetTemporaryFilesUrl(fileName);
                    }
                }
            }

            return new SubmitResult
            {
                Value = downloadUrl,
                IsSuccess = !string.IsNullOrEmpty(downloadUrl)
            };
        }
    }
}
