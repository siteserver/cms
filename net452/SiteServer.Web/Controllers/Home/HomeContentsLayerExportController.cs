using System;
using System.Collections.Generic;
using System.Web.Http;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Caches.Content;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Office;
using SiteServer.CMS.Database.Models;
using SiteServer.CMS.Fx;
using SiteServer.CMS.ImportExport;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Home
{
    [RoutePrefix("home/contentsLayerExport")]
    public class HomeContentsLayerExportController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult GetConfig()
        {
            try
            {
                var rest = Request.GetAuthenticatedRequest();

                var siteId = Request.GetQueryInt("siteId");
                var channelId = Request.GetQueryInt("channelId");

                if (!rest.IsUserLoggin ||
                    !rest.UserPermissions.HasChannelPermissions(siteId, channelId,
                        ConfigManager.ChannelPermissions.ContentView))
                {
                    return Unauthorized();
                }

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var columns = ContentManager.GetContentColumns(siteInfo, channelInfo, true);

                var isChecked = CheckManager.GetUserCheckLevel(rest.AdminPermissions, siteInfo, siteId, out var checkedLevel);
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
                var rest = Request.GetAuthenticatedRequest();

                var downloadUrl = string.Empty;

                var siteId = Request.GetPostInt("siteId");
                var channelId = Request.GetPostInt("channelId");
                var exportType = Request.GetPostString("exportType");
                var isAllCheckedLevel = Request.GetPostBool("isAllCheckedLevel");
                var checkedLevelKeys = Request.GetPostObject<List<int>>("checkedLevelKeys");
                var isAllDate = Request.GetPostBool("isAllDate");
                var startDate = TranslateUtils.ToDateTime(Request.GetPostString("startDate"), DateTime.Now);
                var endDate = TranslateUtils.ToDateTime(Request.GetPostString("endDate"), DateTime.Now);
                var columnNames = Request.GetPostObject<List<string>>("columnNames");

                if (!rest.IsUserLoggin ||
                    !rest.UserPermissions.HasChannelPermissions(siteId, channelId,
                        ConfigManager.ChannelPermissions.ChannelEdit))
                {
                    return Unauthorized();
                }

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var onlyAdminId = ((PermissionsImpl)rest.AdminPermissions).GetOnlyAdminId(siteId, channelId);

                var columns = ContentManager.GetContentColumns(siteInfo, channelInfo, true);
                var pluginIds = PluginContentManager.GetContentPluginIds(channelInfo);
                var pluginColumns = PluginContentManager.GetContentColumns(pluginIds);

                var contentInfoList = new List<ContentInfo>();
                var count = ContentManager.GetCount(siteInfo, channelInfo, onlyAdminId);
                var pages = Convert.ToInt32(Math.Ceiling((double)count / siteInfo.PageSize));
                if (pages == 0) pages = 1;

                if (count > 0)
                {
                    for (var page = 1; page <= pages; page++)
                    {
                        var offset = siteInfo.PageSize * (page - 1);
                        var limit = siteInfo.PageSize;

                        var pageContentIds = ContentManager.GetContentIdList(siteInfo, channelInfo, onlyAdminId, offset, limit);

                        var sequence = offset + 1;

                        foreach (var contentId in pageContentIds)
                        {
                            var contentInfo = ContentManager.GetContentInfo(siteInfo, channelInfo, contentId);
                            if (contentInfo == null) continue;

                            if (!isAllCheckedLevel)
                            {
                                var checkedLevel = contentInfo.CheckedLevel;
                                if (contentInfo.Checked)
                                {
                                    checkedLevel = siteInfo.CheckContentLevel;
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

                            //contentInfoList.Add(ContentManager.Calculate(sequence++, contentInfo, columns, pluginColumns));
                            contentInfoList.Add(contentInfo);
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
                                downloadUrl = FxUtils.GetTemporaryFilesUrl(fileName);
                            }
                        }
                        else if (exportType == "excel")
                        {
                            var fileName = $"{channelInfo.ChannelName}.csv";
                            var filePath = PathUtils.GetTemporaryFilesPath(fileName);
                            ExcelObject.CreateExcelFileForContents(filePath, siteInfo, channelInfo, contentInfoList, columnNames);
                            downloadUrl = FxUtils.GetTemporaryFilesUrl(fileName);
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
