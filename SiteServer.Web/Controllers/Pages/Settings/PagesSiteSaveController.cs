using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.ImportExport;
using SiteServer.CMS.Model.Db;
using SiteServer.Utils;
using SiteServer.Utils.IO;

namespace SiteServer.API.Controllers.Pages.Settings
{
    [OpenApiIgnore]
    [RoutePrefix("pages/settings/siteSave")]
    public class PagesSiteSaveController : ApiController
    {
        private const string Route = "";
        private const string RouteSettings = "actions/settings";
        private const string RouteFiles = "actions/files";
        private const string RouteData = "actions/data";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> GetConfig()
        {
            try
            {
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.Site))
                {
                    return Unauthorized();
                }

                var siteId = request.GetQueryInt("siteId");
                var site = await SiteManager.GetSiteAsync(siteId);
                var templateDir = site.Root ? "T_" + site.SiteName : "T_" + site.SiteDir.Replace("\\", "_");

                return Ok(new
                {
                    Value = site,
                    TemplateDir = templateDir
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteSettings)]
        public async Task<IHttpActionResult> SaveSettings()
        {
            try
            {
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.Site))
                {
                    return Unauthorized();
                }

                var siteId = request.GetPostInt("siteId");
                var templateDir = request.GetPostString("templateDir");

                if (SiteTemplateManager.Instance.IsSiteTemplateDirectoryExists(templateDir))
                {
                    return BadRequest("站点模板文件夹已存在，请更换站点模板文件夹！");
                }

                var site = await SiteManager.GetSiteAsync(siteId);
                
                var directories = new List<string>();
                var files = new List<string>();

                var siteDirList = await DataProvider.SiteDao.GetLowerSiteDirListThatNotIsRootAsync();
                var fileSystems = FileManager.GetFileSystemInfoExtendCollection(PathUtility.GetSitePath(site), true);
                foreach (FileSystemInfoExtend fileSystem in fileSystems)
                {
                    if (!fileSystem.IsDirectory) continue;

                    var isSiteDirectory = false;
                    if (site.Root)
                    {
                        foreach (var siteDir in siteDirList)
                        {
                            if (StringUtils.EqualsIgnoreCase(siteDir, fileSystem.Name))
                            {
                                isSiteDirectory = true;
                            }
                        }
                    }
                    if (!isSiteDirectory && !DirectoryUtils.IsSystemDirectory(fileSystem.Name))
                    {
                        directories.Add(fileSystem.Name);
                    }
                }
                foreach (FileSystemInfoExtend fileSystem in fileSystems)
                {
                    if (fileSystem.IsDirectory) continue;
                    if (!PathUtility.IsSystemFile(fileSystem.Name))
                    {
                        files.Add(fileSystem.Name);
                    }
                }

                var channels = new List<ChannelInfo>();
                var channelIdList = ChannelManager.GetChannelIdList(siteId);
                foreach (var channelId in channelIdList)
                {
                    var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                    channels.Add(channelInfo);
                }

                return Ok(new
                {
                    Value = true,
                    Directories = directories,
                    Files = files,
                    Channels = channels
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteFiles)]
        public async Task<IHttpActionResult> SaveFiles()
        {
            try
            {
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.Site))
                {
                    return Unauthorized();
                }

                var siteId = request.GetPostInt("siteId");
                var templateDir = request.GetPostString("templateDir");
                var isAllFiles = request.GetPostBool("isAllFiles");
                var checkedDirectories = request.GetPostObject<IList<string>>("checkedDirectories");
                var checkedFiles = request.GetPostObject<IList<string>>("checkedFiles");

                var exportObject = new ExportObject(siteId, request.AdminName);
                var siteTemplatePath = PathUtility.GetSiteTemplatesPath(templateDir);
                await exportObject.ExportFilesToSiteAsync(siteTemplatePath, isAllFiles, checkedDirectories, checkedFiles, true);

                var channels = new List<ChannelInfo>();
                var channelIdList = ChannelManager.GetChannelIdList(siteId);
                foreach (var channelId in channelIdList)
                {
                    var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                    channels.Add(channelInfo);
                }

                return Ok(new
                {
                    Value = true,
                    Channels = channels
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
