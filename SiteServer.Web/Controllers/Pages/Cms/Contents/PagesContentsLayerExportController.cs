using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Office;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.ImportExport;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Repositories;
using SiteServer.CMS.StlParser.Model;

namespace SiteServer.API.Controllers.Pages.Cms.Contents
{
    
    [RoutePrefix("pages/cms/contentsLayerExport")]
    public class PagesContentsLayerExportController : ApiController
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

                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissions.HasChannelPermissionsAsync(siteId, channelId,
                        Constants.ChannelPermissions.ContentView))
                {
                    return Unauthorized();
                }

                var site = await DataProvider.SiteRepository.GetAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var columns = await DataProvider.ContentRepository.GetContentColumnsAsync(site, channelInfo, true);

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
                var channelContentIds =
                    MinContentInfo.ParseMinContentInfoList(request.GetPostString("channelContentIds"));

                var exportType = request.GetPostString("exportType");
                var isAllCheckedLevel = request.GetPostBool("isAllCheckedLevel");
                var checkedLevelKeys = request.GetPostObject<List<int>>("checkedLevelKeys");
                var isAllDate = request.GetPostBool("isAllDate");
                var startDate = request.GetPostDateTime("startDate", DateTime.Now);
                var endDate = request.GetPostDateTime("endDate", DateTime.Now);
                var columnNames = request.GetPostObject<List<string>>("columnNames");

                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissions.HasChannelPermissionsAsync(siteId, channelId,
                        Constants.ChannelPermissions.ChannelEdit))
                {
                    return Unauthorized();
                }

                var site = await DataProvider.SiteRepository.GetAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var adminId = channelInfo.IsSelfOnly
                    ? request.AdminId
                    : await request.AdminPermissionsImpl.GetAdminIdAsync(siteId, channelId);
                var isAllContents = channelInfo.IsAllContents;

                var columns = await DataProvider.ContentRepository.GetContentColumnsAsync(site, channelInfo, true);
                var pluginIds = PluginContentManager.GetContentPluginIds(channelInfo);
                var pluginColumns = await PluginContentManager.GetContentColumnsAsync(pluginIds);

                var contentInfoList = new List<Content>();
                var calculatedContentInfoList = new List<Content>();

                if (channelContentIds.Count == 0)
                {
                    var count = isAllContents
                        ? await DataProvider.ContentRepository.GetCountAllAsync(site, channelInfo, adminId)
                        : await DataProvider.ContentRepository.GetCountAsync(site, channelInfo, adminId);

                    var pages = Convert.ToInt32(Math.Ceiling((double)count / site.PageSize));
                    if (pages == 0) pages = 1;

                    if (count > 0)
                    {
                        for (var page = 1; page <= pages; page++)
                        {
                            var offset = site.PageSize * (page - 1);
                            var limit = site.PageSize;

                            var pageContentIds = await DataProvider.ContentRepository.GetChannelContentIdListAsync(site, channelInfo, adminId, isAllContents, offset, limit);

                            var sequence = offset + 1;

                            foreach (var channelContentId in pageContentIds)
                            {
                                var contentInfo = await DataProvider.ContentRepository.GetAsync(site, channelContentId.ChannelId, channelContentId.ContentId);
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

                                contentInfoList.Add(contentInfo);
                                calculatedContentInfoList.Add(await DataProvider.ContentRepository.CalculateAsync(sequence++, contentInfo, columns, pluginColumns));
                            }
                        }
                    }
                }
                else
                {
                    var sequence = 1;
                    foreach (var channelContentId in channelContentIds)
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

                        contentInfoList.Add(contentInfo);
                        calculatedContentInfoList.Add(await DataProvider.ContentRepository.CalculateAsync(sequence++, contentInfo, columns, pluginColumns));
                    }
                }

                if (contentInfoList.Count > 0)
                {
                    if (exportType == "zip")
                    {
                        var fileName = $"{channelInfo.ChannelName}.zip";
                        var filePath = PathUtils.GetTemporaryFilesPath(fileName);
                        var exportObject = new ExportObject(siteId, request.AdminName);
                        contentInfoList.Reverse();
                        if (await exportObject.ExportContentsAsync(filePath, contentInfoList))
                        {
                            downloadUrl = PageUtils.GetTemporaryFilesUrl(fileName);
                        }
                    }
                    else if (exportType == "excel")
                    {
                        var fileName = $"{channelInfo.ChannelName}.csv";
                        var filePath = PathUtils.GetTemporaryFilesPath(fileName);
                        await ExcelObject.CreateExcelFileForContentsAsync(filePath, site, channelInfo, calculatedContentInfoList, columnNames);
                        downloadUrl = PageUtils.GetTemporaryFilesUrl(fileName);
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
