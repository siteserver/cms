using System;
using System.Collections.Generic;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Office;
using SiteServer.CMS.Database.Caches;
using SiteServer.CMS.Database.Models;
using SiteServer.CMS.ImportExport;
using SiteServer.CMS.Plugin;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Cms
{
    [RoutePrefix("pages/cms/contentsLayerExport")]
    public class PagesContentsLayerExportController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult GetConfig()
        {
            try
            {
                var rest = new Rest(Request);

                var siteId = rest.GetQueryInt("siteId");
                var channelId = rest.GetQueryInt("channelId");

                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissions.HasChannelPermissions(siteId, channelId,
                        ConfigManager.ChannelPermissions.ContentView))
                {
                    return Unauthorized();
                }

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var columns = ContentManager.GetContentColumns(siteInfo, channelInfo, true);

                var isChecked = CheckManager.GetUserCheckLevel(rest.AdminPermissionsImpl, siteInfo, siteId, out var checkedLevel);
                var checkedLevels = CheckManager.GetCheckedLevels(siteInfo, isChecked, checkedLevel, true);

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
        public IHttpActionResult Submit()
        {
            try
            {
                var rest = new Rest(Request);

                var downloadUrl = string.Empty;

                var siteId = rest.GetPostInt("siteId");
                var channelId = rest.GetPostInt("channelId");
                var exportType = rest.GetPostString("exportType");
                var isAllCheckedLevel = rest.GetPostBool("isAllCheckedLevel");
                var checkedLevelKeys = rest.GetPostObject<List<int>>("checkedLevelKeys");
                var isAllDate = rest.GetPostBool("isAllDate");
                var startDate = rest.GetPostDateTime("startDate", DateTime.Now);
                var endDate = rest.GetPostDateTime("endDate", DateTime.Now);
                var columnNames = rest.GetPostObject<List<string>>("columnNames");

                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissions.HasChannelPermissions(siteId, channelId,
                        ConfigManager.ChannelPermissions.ChannelEdit))
                {
                    return Unauthorized();
                }

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var columns = ContentManager.GetContentColumns(siteInfo, channelInfo, true);
                var pluginIds = PluginContentManager.GetContentPluginIds(channelInfo);
                var pluginColumns = PluginContentManager.GetContentColumns(pluginIds);

                var contentInfoList = new List<ContentInfo>();
                var count = ContentManager.GetCount(siteInfo, channelInfo);
                var pages = Convert.ToInt32(Math.Ceiling((double)count / siteInfo.Extend.PageSize));
                if (pages == 0) pages = 1;

                if (count > 0)
                {
                    for (var page = 1; page <= pages; page++)
                    {
                        var offset = siteInfo.Extend.PageSize * (page - 1);
                        var limit = siteInfo.Extend.PageSize;

                        var pageContentIds = ContentManager.GetContentIdList(siteInfo, channelInfo, offset, limit);

                        var sequence = offset + 1;

                        foreach (var contentId in pageContentIds)
                        {
                            var contentInfo = ContentManager.GetContentInfo(siteInfo, channelInfo, contentId);
                            if (contentInfo == null) continue;

                            if (!isAllCheckedLevel)
                            {
                                var checkedLevel = contentInfo.CheckedLevel;
                                if (contentInfo.IsChecked)
                                {
                                    checkedLevel = siteInfo.Extend.CheckContentLevel;
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

                            contentInfoList.Add(ContentManager.Calculate(sequence++, contentInfo, columns, pluginColumns));
                        }
                    }

                    if (contentInfoList.Count > 0)
                    {
                        if (exportType == "zip")
                        {
                            var fileName = $"{channelInfo.ChannelName}.zip";
                            var filePath = PathUtils.GetTemporaryFilesPath(fileName);
                            var exportObject = new ExportObject(siteId, rest.AdminName);
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
                            ExcelObject.CreateExcelFileForContents(filePath, siteInfo, channelInfo, contentInfoList, columnNames);
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
