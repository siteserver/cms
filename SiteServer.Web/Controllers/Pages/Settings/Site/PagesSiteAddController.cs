using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Context;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Enumerations;
using SiteServer.CMS.Model;
using SiteServer.CMS.Provider;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Settings.Site
{
    [OpenApiIgnore]
    [RoutePrefix("pages/settings/siteAdd")]
    public class PagesSiteAddController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> GetConfig()
        {
            try
            {
                var request = await AuthenticatedRequest.GetRequestAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(ConfigManager.SettingsPermissions.SiteAdd))
                {
                    return Unauthorized();
                }

                var siteTemplates = SiteTemplateManager.Instance.GetSiteTemplateSortedList();

                var siteList = new List<KeyValuePair<int, string>>
                {
                    new KeyValuePair<int, string>(0, "<无上级站点>")
                };

                var siteIdList = await SiteManager.GetSiteIdListAsync();
                var siteInfoList = new List<CMS.Model.Site>();
                var parentWithChildren = new Dictionary<int, List<CMS.Model.Site>>();
                foreach (var siteId in siteIdList)
                {
                    var site = await SiteManager.GetSiteAsync(siteId);
                    if (site.ParentId == 0)
                    {
                        siteInfoList.Add(site);
                    }
                    else
                    {
                        var children = new List<CMS.Model.Site>();
                        if (parentWithChildren.ContainsKey(site.ParentId))
                        {
                            children = parentWithChildren[site.ParentId];
                        }
                        children.Add(site);
                        parentWithChildren[site.ParentId] = children;
                    }
                }
                foreach (CMS.Model.Site site in siteInfoList)
                {
                    AddSite(siteList, site, parentWithChildren, 0);
                }

                var tableNameList = await SiteManager.GetSiteTableNamesAsync();

                var rootExists = await SiteManager.GetSiteByIsRootAsync() != null;

                return Ok(new
                {
                    Value = siteTemplates.Values,
                    RootExists = rootExists,
                    SiteList = siteList,
                    TableNameList = tableNameList
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private static void AddSite(List<KeyValuePair<int, string>> siteList, CMS.Model.Site site, Dictionary<int, List<CMS.Model.Site>> parentWithChildren, int level)
        {
            if (level > 1) return;
            var padding = string.Empty;
            for (var i = 0; i < level; i++)
            {
                padding += "　";
            }
            if (level > 0)
            {
                padding += "└ ";
            }

            if (parentWithChildren.ContainsKey(site.Id))
            {
                var children = parentWithChildren[site.Id];
                siteList.Add(new KeyValuePair<int, string>(site.Id, padding + site.SiteName + $"({children.Count})"));
                level++;
                foreach (var subSite in children)
                {
                    AddSite(siteList, subSite, parentWithChildren, level);
                }
            }
            else
            {
                siteList.Add(new KeyValuePair<int, string>(site.Id, padding + site.SiteName));
            }
        }

        [HttpPost, Route(Route)]
        public async Task<IHttpActionResult> Submit()
        {
            try
            {
                var request = await AuthenticatedRequest.GetRequestAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(ConfigManager.SettingsPermissions.SiteAdd))
                {
                    return Unauthorized();
                }

                var createType = request.GetPostString("createType");
                var createTemplateId = request.GetPostString("createTemplateId");
                var siteName = request.GetPostString("siteName");
                var root = request.GetPostBool("root");
                var parentId = request.GetPostInt("parentId");
                var siteDir = request.GetPostString("siteDir");
                var tableRule = ETableRuleUtils.GetEnumType(request.GetPostString("tableRule"));
                var tableChoose = request.GetPostString("tableChoose");
                var tableHandWrite = request.GetPostString("tableHandWrite");
                var isImportContents = request.GetPostBool("isImportContents");
                var isImportTableStyles = request.GetPostBool("isImportTableStyles");

                if (!root)
                {
                    if (WebUtils.IsSystemDirectory(siteDir))
                    {
                        return BadRequest("文件夹名称不能为系统文件夹名称，请更改文件夹名称！");
                    }
                    if (!DirectoryUtils.IsDirectoryNameCompliant(siteDir))
                    {
                        return BadRequest("文件夹名称不符合系统要求，请更改文件夹名称！");
                    }
                    var list = await DataProvider.SiteDao.GetLowerSiteDirListAsync(parentId);
                    if (list.Contains(siteDir.ToLower()))
                    {
                        return BadRequest("已存在相同的发布路径，请更改文件夹名称！");
                    }
                }

                var channelInfo = new Channel();

                channelInfo.ChannelName = channelInfo.IndexName = "首页";
                channelInfo.ParentId = 0;
                channelInfo.ContentModelPluginId = string.Empty;

                var tableName = string.Empty;
                if (tableRule == ETableRule.Choose)
                {
                    tableName = tableChoose;
                }
                else if (tableRule == ETableRule.HandWrite)
                {
                    tableName = tableHandWrite;
                    if (!DataProvider.DatabaseDao.IsTableExists(tableName))
                    {
                        DataProvider.ContentDao.CreateContentTable(tableName, DataProvider.ContentDao.TableColumnsDefault);
                    }
                    else
                    {
                        await DataProvider.DatabaseDao.AlterSystemTableAsync(tableName, DataProvider.ContentDao.TableColumnsDefault);
                    }
                }

                var site = new CMS.Model.Site
                {
                    SiteName = siteName,
                    SiteDir = siteDir,
                    TableName = tableName,
                    ParentId = parentId,
                    Root = root
                };
                site.IsCheckContentLevel = false;
                site.Charset = ECharsetUtils.GetValue(ECharset.utf_8);

                var siteId = await DataProvider.ChannelDao.InsertSiteAsync(channelInfo, site, request.AdminName);

                if (string.IsNullOrEmpty(tableName))
                {
                    tableName = ContentDao.GetContentTableName(siteId);
                    DataProvider.ContentDao.CreateContentTable(tableName, DataProvider.ContentDao.TableColumnsDefault);
                    await DataProvider.SiteDao.UpdateTableNameAsync(siteId, tableName);
                }

                if (await request.AdminPermissionsImpl.IsSiteAdminAsync() && !await request.AdminPermissionsImpl.IsSuperAdminAsync())
                {
                    var siteIdList = await request.AdminPermissionsImpl.GetSiteIdListAsync() ?? new List<int>();
                    siteIdList.Add(siteId);
                    var adminInfo = await AdminManager.GetByUserIdAsync(request.AdminId);
                    await DataProvider.AdministratorDao.UpdateSiteIdCollectionAsync(adminInfo, TranslateUtils.ObjectCollectionToString(siteIdList));
                }

                var siteTemplateDir = string.Empty;
                var onlineTemplateName = string.Empty;
                if (StringUtils.EqualsIgnoreCase(createType, "local"))
                {
                    siteTemplateDir = createTemplateId;
                }
                else if (StringUtils.EqualsIgnoreCase(createType, "cloud"))
                {
                    onlineTemplateName = createTemplateId;
                }

                var redirectUrl = PageProgressBar.GetCreateSiteUrl(siteId,
                    isImportContents, isImportTableStyles, siteTemplateDir, onlineTemplateName, StringUtils.Guid());

                return Ok(new
                {
                    Value = redirectUrl
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
