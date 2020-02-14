using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Office;
using SiteServer.CMS.Dto.Request;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.ImportExport;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Contents
{
    [RoutePrefix("pages/cms/contents/contentsLayerExport")]
    public partial class PagesContentsLayerExportController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<GetResult> Get([FromUri] ChannelRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentView))
            {
                return Request.Unauthorized<GetResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<GetResult>();

            var channel = await DataProvider.ChannelRepository.GetAsync(request.ChannelId);
            if (channel == null) return Request.BadRequest<GetResult>("无法确定内容对应的栏目");

            var columns = await ColumnsManager.GetContentListColumnsAsync(site, channel, ColumnsManager.PageType.Contents);

            var (isChecked, checkedLevel) = await CheckManager.GetUserCheckLevelAsync(auth.AdminPermissionsImpl, site, request.SiteId);
            var checkedLevels = CheckManager.GetCheckedLevels(site, isChecked, checkedLevel, true);

            return new GetResult
            {
                Value = columns,
                CheckedLevels = checkedLevels,
                CheckedLevel = checkedLevel
            };
        }

        [HttpPost, Route(Route)]
        public async Task<SubmitResult> Submit([FromBody] SubmitRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ChannelEdit))
            {
                return Request.Unauthorized<SubmitResult>();
            }

            var summaries = ContentUtility.ParseSummaries(request.ChannelContentIds);

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<SubmitResult>();

            var channel = await DataProvider.ChannelRepository.GetAsync(request.ChannelId);
            if (channel == null) return Request.BadRequest<SubmitResult>("无法确定内容对应的栏目");

            var columns = await ColumnsManager.GetContentListColumnsAsync(site, channel, ColumnsManager.PageType.Contents);
            var pluginIds = PluginContentManager.GetContentPluginIds(channel);
            var pluginColumns = await PluginContentManager.GetContentColumnsAsync(pluginIds);

            var contentInfoList = new List<Content>();
            var calculatedContentInfoList = new List<Content>();

            if (summaries.Count == 0)
            {
                var ccIds = await DataProvider.ContentRepository.GetSummariesAsync(site, channel, channel.IsAllContents);
                var count = ccIds.Count();

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
                            var contentInfo = await DataProvider.ContentRepository.GetAsync(site, channelContentId.ChannelId, channelContentId.Id);
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
                            calculatedContentInfoList.Add(await ColumnsManager.CalculateContentListAsync(sequence++, site, request.ChannelId, contentInfo, columns, pluginColumns));
                        }
                    }
                }
            }
            else
            {
                var sequence = 1;
                foreach (var channelContentId in summaries)
                {
                    var contentInfo = await DataProvider.ContentRepository.GetAsync(site, channelContentId.ChannelId, channelContentId.Id);
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
                    calculatedContentInfoList.Add(await ColumnsManager.CalculateContentListAsync(sequence++, site, request.ChannelId, contentInfo, columns, pluginColumns));
                }
            }

            var downloadUrl = string.Empty;
            if (contentInfoList.Count > 0)
            {
                if (request.ExportType == "zip")
                {
                    var fileName = $"{channel.ChannelName}.zip";
                    var filePath = PathUtils.GetTemporaryFilesPath(fileName);
                    var exportObject = new ExportObject(site, auth.AdminId);
                    contentInfoList.Reverse();
                    if (exportObject.ExportContents(filePath, contentInfoList))
                    {
                        downloadUrl = PageUtils.GetTemporaryFilesUrl(fileName);
                    }
                }
                else if (request.ExportType == "excel")
                {
                    var exportColumnNames =
                        request.IsAllColumns ? columns.Select(x => x.AttributeName).ToList() : request.ColumnNames;
                    var fileName = $"{channel.ChannelName}.csv";
                    var filePath = PathUtils.GetTemporaryFilesPath(fileName);
                    await ExcelObject.CreateExcelFileForContentsAsync(filePath, site, channel, calculatedContentInfoList, exportColumnNames);
                    downloadUrl = PageUtils.GetTemporaryFilesUrl(fileName);
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
