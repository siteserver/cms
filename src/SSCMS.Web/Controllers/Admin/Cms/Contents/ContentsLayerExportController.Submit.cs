using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Core.Utils.Office;
using SSCMS.Core.Utils.Serialization;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Contents
{
    public partial class ContentsLayerExportController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<SubmitResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, MenuUtils.ContentPermissions.View))
            {
                return Unauthorized();
            }

            var summaries = ContentUtility.ParseSummaries(request.ChannelContentIds);

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            if (channel == null) return this.Error("无法确定内容对应的栏目");

            var columnsManager = new ColumnsManager(_databaseManager, _pathManager);
            var columns = await columnsManager.GetContentListColumnsAsync(site, channel, ColumnsManager.PageType.Contents);

            var contentInfoList = new List<Content>();
            var calculatedContentInfoList = new List<Content>();

            if (summaries.Count == 0)
            {
                var ccIds = await _contentRepository.GetSummariesAsync(site, channel, channel.IsAllContents);
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

                            contentInfoList.Add(contentInfo);
                            calculatedContentInfoList.Add(await columnsManager.CalculateContentListAsync(sequence++, site, request.ChannelId, contentInfo, columns));
                        }
                    }
                }
            }
            else
            {
                var sequence = 1;
                foreach (var channelContentId in summaries)
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

                    contentInfoList.Add(contentInfo);
                    calculatedContentInfoList.Add(await columnsManager.CalculateContentListAsync(sequence++, site, request.ChannelId, contentInfo, columns));
                }
            }

            var downloadUrl = string.Empty;
            if (contentInfoList.Count > 0)
            {
                if (request.ExportType == "zip")
                {
                    var fileName = $"{channel.ChannelName}.zip";
                    var filePath = _pathManager.GetTemporaryFilesPath(fileName);

                    var caching = new CacheUtils(_cacheManager);
                    var exportObject = new ExportObject(_pathManager, _databaseManager, caching, site);
                    contentInfoList.Reverse();
                    if (await exportObject.ExportContentsAsync(filePath, contentInfoList))
                    {
                        downloadUrl = _pathManager.GetTemporaryFilesUrl(fileName);
                    }
                }
                else if (request.ExportType == "excel")
                {
                    var exportColumnNames =
                        request.IsAllColumns ? columns.Select(x => x.AttributeName).ToList() : request.ColumnNames;
                    var fileName = $"{channel.ChannelName}.csv";
                    var filePath = _pathManager.GetTemporaryFilesPath(fileName);

                    var excelObject = new ExcelObject(_databaseManager, _pathManager);
                    await excelObject.CreateExcelFileForContentsAsync(filePath, site, channel, calculatedContentInfoList, exportColumnNames);
                    downloadUrl = _pathManager.GetTemporaryFilesUrl(fileName);
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