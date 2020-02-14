using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Office;
using SiteServer.CMS.ImportExport;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Home
{
    
    [RoutePrefix("home/contentsLayerExport")]
    public class HomeContentsLayerExportController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> GetConfig()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();

                var siteId = request.GetQueryInt("siteId");
                var channelId = request.GetQueryInt("channelId");

                if (!request.IsUserLoggin ||
                    !await request.UserPermissionsImpl.HasChannelPermissionsAsync(siteId, channelId,
                        Constants.ChannelPermissions.ContentView))
                {
                    return Unauthorized();
                }

                var site = await DataProvider.SiteRepository.GetAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                var channel = await DataProvider.ChannelRepository.GetAsync(channelId);
                if (channel == null) return BadRequest("无法确定内容对应的栏目");

                var columns = await ColumnsManager.GetContentListColumnsAsync(site, channel, ColumnsManager.PageType.Contents);

                var (isChecked, checkedLevel) = await CheckManager.GetUserCheckLevelAsync(request.AdminPermissionsImpl, site, siteId);
                var checkedLevels = CheckManager.GetCheckedLevels(site, isChecked, checkedLevel, true);

                return Ok(new
                {
                    Value = columns,
                    CheckedLevels = checkedLevels,
                    CheckedLevel = checkedLevel
                });
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAsync(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(Route)]
        public async Task<IHttpActionResult> Submit()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();

                var downloadUrl = string.Empty;

                var siteId = request.GetPostInt("siteId");
                var channelId = request.GetPostInt("channelId");
                var exportType = request.GetPostString("exportType");
                var isAllCheckedLevel = request.GetPostBool("isAllCheckedLevel");
                var checkedLevelKeys = request.GetPostObject<List<int>>("checkedLevelKeys");
                var isAllDate = request.GetPostBool("isAllDate");
                var startDate = request.GetPostDateTime("startDate", DateTime.Now);
                var endDate = request.GetPostDateTime("endDate", DateTime.Now);
                var columnNames = request.GetPostObject<List<string>>("columnNames");

                if (!request.IsUserLoggin ||
                    !await request.UserPermissionsImpl.HasChannelPermissionsAsync(siteId, channelId,
                        Constants.ChannelPermissions.ChannelEdit))
                {
                    return Unauthorized();
                }

                var site = await DataProvider.SiteRepository.GetAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                var channel = await DataProvider.ChannelRepository.GetAsync(channelId);
                if (channel == null) return BadRequest("无法确定内容对应的栏目");

                var columns = await ColumnsManager.GetContentListColumnsAsync(site, channel, ColumnsManager.PageType.Contents);
                var pluginIds = PluginContentManager.GetContentPluginIds(channel);
                var pluginColumns = await PluginContentManager.GetContentColumnsAsync(pluginIds);

                var contentInfoList = new List<Content>();
                var ccIds = await DataProvider.ContentRepository.GetSummariesAsync(site, channel, true);
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

                            if (!isAllCheckedLevel)
                            {
                                var checkedLevel = contentInfo.CheckedLevel;
                                if (contentInfo.Checked)
                                {
                                    checkedLevel = site.CheckContentLevel;
                                }
                                if (!checkedLevelKeys.Contains(checkedLevel))
                                {
                                    continue;
                                }
                            }

                            if (!isAllDate)
                            {
                                if (contentInfo.AddDate < startDate || contentInfo.AddDate > endDate)
                                {
                                    continue;
                                }
                            }

                            contentInfoList.Add(await ColumnsManager.CalculateContentListAsync(sequence++, site, channelId, contentInfo, columns, pluginColumns));
                        }
                    }

                    if (contentInfoList.Count > 0)
                    {
                        if (exportType == "zip")
                        {
                            var fileName = $"{channel.ChannelName}.zip";
                            var filePath = PathUtils.GetTemporaryFilesPath(fileName);
                            var exportObject = new ExportObject(site, request.AdminId);
                            contentInfoList.Reverse();
                            if (exportObject.ExportContents(filePath, contentInfoList))
                            {
                                downloadUrl = PageUtils.GetTemporaryFilesUrl(fileName);
                            }
                        }
                        else if (exportType == "excel")
                        {
                            var fileName = $"{channel.ChannelName}.csv";
                            var filePath = PathUtils.GetTemporaryFilesPath(fileName);
                            await ExcelObject.CreateExcelFileForContentsAsync(filePath, site, channel, contentInfoList, columnNames);
                            downloadUrl = PageUtils.GetTemporaryFilesUrl(fileName);
                        }
                    }
                }

                return Ok(new
                {
                    Value = downloadUrl,
                    IsSuccess = !string.IsNullOrEmpty(downloadUrl)
                });
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAsync(ex);
                return InternalServerError(ex);
            }
        }
    }
}
