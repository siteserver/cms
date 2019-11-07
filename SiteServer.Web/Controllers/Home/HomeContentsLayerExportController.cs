using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Office;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.ImportExport;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Db;
using SiteServer.CMS.Plugin;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Home
{
    [OpenApiIgnore]
    [RoutePrefix("home/contentsLayerExport")]
    public class HomeContentsLayerExportController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> GetConfig()
        {
            try
            {
                var request = new AuthenticatedRequest();

                var siteId = request.GetQueryInt("siteId");
                var channelId = request.GetQueryInt("channelId");

                if (!request.IsUserLoggin ||
                    !request.UserPermissionsImpl.HasChannelPermissions(siteId, channelId,
                        ConfigManager.ChannelPermissions.ContentView))
                {
                    return Unauthorized();
                }

                var site = await SiteManager.GetSiteAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var columns = ContentManager.GetContentColumns(site, channelInfo, true);

                var isChecked = CheckManager.GetUserCheckLevel(request.AdminPermissionsImpl, site, siteId, out var checkedLevel);
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
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(Route)]
        public async Task<IHttpActionResult> Submit()
        {
            try
            {
                var request = new AuthenticatedRequest();

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
                    !request.UserPermissionsImpl.HasChannelPermissions(siteId, channelId,
                        ConfigManager.ChannelPermissions.ChannelEdit))
                {
                    return Unauthorized();
                }

                var site = await SiteManager.GetSiteAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var adminId = request.AdminPermissionsImpl.GetAdminId(siteId, channelId);

                var columns = ContentManager.GetContentColumns(site, channelInfo, true);
                var pluginIds = PluginContentManager.GetContentPluginIds(channelInfo);
                var pluginColumns = PluginContentManager.GetContentColumns(pluginIds);

                var contentInfoList = new List<ContentInfo>();
                var count = ContentManager.GetCount(site, channelInfo, adminId, true);
                var pages = Convert.ToInt32(Math.Ceiling((double)count / site.Additional.PageSize));
                if (pages == 0) pages = 1;

                if (count > 0)
                {
                    for (var page = 1; page <= pages; page++)
                    {
                        var offset = site.Additional.PageSize * (page - 1);
                        var limit = site.Additional.PageSize;

                        var pageContentIds = ContentManager.GetChannelContentIdList(site, channelInfo, adminId, true, offset, limit);

                        var sequence = offset + 1;

                        foreach (var channelContentId in pageContentIds)
                        {
                            var contentInfo = ContentManager.GetContentInfo(site, channelContentId.ChannelId, channelContentId.ContentId);
                            if (contentInfo == null) continue;

                            if (!isAllCheckedLevel)
                            {
                                var checkedLevel = contentInfo.CheckedLevel;
                                if (contentInfo.IsChecked)
                                {
                                    checkedLevel = site.Additional.CheckContentLevel;
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

                            contentInfoList.Add(await ContentManager.CalculateAsync(sequence++, contentInfo, columns, pluginColumns));
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
                            if (exportObject.ExportContents(filePath, contentInfoList))
                            {
                                downloadUrl = PageUtils.GetTemporaryFilesUrl(fileName);
                            }
                        }
                        else if (exportType == "excel")
                        {
                            var fileName = $"{channelInfo.ChannelName}.csv";
                            var filePath = PathUtils.GetTemporaryFilesPath(fileName);
                            ExcelObject.CreateExcelFileForContents(filePath, site, channelInfo, contentInfoList, columnNames);
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
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }
    }
}
