using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.API.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Repositories;
using SiteServer.CMS.Serialization;

namespace SiteServer.API.Controllers.Pages.Settings.Sites
{
    
    [RoutePrefix("pages/settings/sitesSave")]
    public class PagesSitesSaveController : ApiController
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
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSites))
                {
                    return Unauthorized();
                }

                var siteId = request.GetQueryInt("siteId");
                var site = await DataProvider.SiteRepository.GetAsync(siteId);
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
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSites))
                {
                    return Unauthorized();
                }

                var siteId = request.GetPostInt("siteId");
                var templateDir = request.GetPostString("templateDir");

                if (SiteTemplateManager.Instance.IsSiteTemplateDirectoryExists(templateDir))
                {
                    return BadRequest("站点模板文件夹已存在，请更换站点模板文件夹！");
                }

                var site = await DataProvider.SiteRepository.GetAsync(siteId);
                
                var directories = new List<string>();
                var files = new List<string>();

                var siteDirList = await DataProvider.SiteRepository.GetSiteDirListAsync(0);
                var fileSystems = FileUtility.GetFileSystemInfoExtendCollection(await PathUtility.GetSitePathAsync(site));
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
                    if (!isSiteDirectory && !WebUtils.IsSystemDirectory(fileSystem.Name))
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

                return Ok(new
                {
                    Value = true,
                    Directories = directories,
                    Files = files
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
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSites))
                {
                    return Unauthorized();
                }

                var siteId = request.GetPostInt("siteId");
                var templateDir = request.GetPostString("templateDir");
                var isAllFiles = request.GetPostBool("isAllFiles");
                var checkedDirectories = request.GetPostObject<IList<string>>("checkedDirectories");
                var checkedFiles = request.GetPostObject<IList<string>>("checkedFiles");

                var site = await DataProvider.SiteRepository.GetAsync(siteId);
                var exportObject = new ExportObject(site, request.AdminId);
                var siteTemplatePath = PathUtility.GetSiteTemplatesPath(templateDir);
                await exportObject.ExportFilesToSiteAsync(siteTemplatePath, isAllFiles, checkedDirectories, checkedFiles, true);

                var channelInfo = await DataProvider.ChannelRepository.GetAsync(siteId);
                channelInfo.Children = await DataProvider.ChannelRepository.GetChildrenAsync(siteId, siteId);

                return Ok(new
                {
                    Value = true,
                    ChannelInfo = channelInfo
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteData)]
        public async Task<IHttpActionResult> SaveData()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSites))
                {
                    return Unauthorized();
                }

                var siteId = request.GetPostInt("siteId");
                var templateName = request.GetPostString("templateName");
                var templateDir = request.GetPostString("templateDir");
                var webSiteUrl = request.GetPostString("webSiteUrl");
                var description = request.GetPostString("description");
                var isSaveContents = request.GetPostBool("isSaveContents");
                var isSaveAllChannels = request.GetPostBool("isSaveAllChannels");
                var checkedChannelIds = request.GetPostObject<IList<int>>("checkedChannelIds");

                var site = await DataProvider.SiteRepository.GetAsync(siteId);

                var siteTemplatePath = PathUtility.GetSiteTemplatesPath(templateDir);
                var siteContentDirectoryPath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.SiteContent);

                var exportObject = new ExportObject(site, request.AdminId);
                await exportObject.ExportSiteContentAsync(siteContentDirectoryPath, isSaveContents, isSaveAllChannels, checkedChannelIds);

                await SiteTemplateManager.ExportSiteToSiteTemplateAsync(site, templateDir, request.AdminId);

                var siteTemplateInfo = new SiteTemplateInfo
                {
                    SiteTemplateName = templateName,
                    PicFileName = string.Empty,
                    WebSiteUrl = webSiteUrl,
                    Description = description
                };
                var xmlPath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath,
                    DirectoryUtils.SiteTemplates.FileMetadata);
                Serializer.SaveAsXml(siteTemplateInfo, xmlPath);

                return Ok(new
                {
                    Value = true,
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
