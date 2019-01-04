using System;
using System.Collections.Generic;
using System.Web.Http;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.CMS.Provider;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.API.Controllers.Pages.Settings
{
    [RoutePrefix("pages/settings/siteAdd")]
    public class PagesSiteAddController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult GetConfig()
        {
            try
            {
                var request = new RequestImpl();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.SiteAdd))
                {
                    return Unauthorized();
                }

                var siteTemplates = SiteTemplateManager.Instance.GetSiteTemplateSortedList();

                var siteList = new List<KeyValuePair<int, string>>
                {
                    new KeyValuePair<int, string>(0, "<无上级站点>")
                };

                var siteIdList = SiteManager.GetSiteIdList();
                var siteInfoList = new List<SiteInfo>();
                var parentWithChildren = new Dictionary<int, List<SiteInfo>>();
                foreach (var siteId in siteIdList)
                {
                    var siteInfo = SiteManager.GetSiteInfo(siteId);
                    if (siteInfo.IsRoot == false)
                    {
                        if (siteInfo.ParentId == 0)
                        {
                            siteInfoList.Add(siteInfo);
                        }
                        else
                        {
                            var children = new List<SiteInfo>();
                            if (parentWithChildren.ContainsKey(siteInfo.ParentId))
                            {
                                children = parentWithChildren[siteInfo.ParentId];
                            }
                            children.Add(siteInfo);
                            parentWithChildren[siteInfo.ParentId] = children;
                        }
                    }
                }
                foreach (SiteInfo siteInfo in siteInfoList)
                {
                    AddSite(siteList, siteInfo, parentWithChildren, 0);
                }

                var tableNameList = SiteManager.GetSiteTableNames();

                var isRootExists = SiteManager.GetSiteInfoByIsRoot() != null;

                return Ok(new
                {
                    Value = siteTemplates.Values,
                    IsRootExists = isRootExists,
                    SiteList = siteList,
                    TableNameList = tableNameList
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private static void AddSite(List<KeyValuePair<int, string>> siteList, SiteInfo siteInfo, Dictionary<int, List<SiteInfo>> parentWithChildren, int level)
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

            if (parentWithChildren.ContainsKey(siteInfo.Id))
            {
                var children = parentWithChildren[siteInfo.Id];
                siteList.Add(new KeyValuePair<int, string>(siteInfo.Id, padding + siteInfo.SiteName + $"({children.Count})"));
                level++;
                foreach (var subSiteInfo in children)
                {
                    AddSite(siteList, subSiteInfo, parentWithChildren, level);
                }
            }
            else
            {
                siteList.Add(new KeyValuePair<int, string>(siteInfo.Id, padding + siteInfo.SiteName));
            }
        }

        [HttpPost, Route(Route)]
        public IHttpActionResult Submit()
        {
            try
            {
                var request = new RequestImpl();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.SiteAdd))
                {
                    return Unauthorized();
                }

                var createType = request.GetPostString("createType");
                var createTemplateId = request.GetPostString("createTemplateId");
                var siteName = request.GetPostString("siteName");
                var isRoot = request.GetPostBool("isRoot");
                var parentId = request.GetPostInt("parentId");
                var siteDir = request.GetPostString("siteDir");
                var tableRule = ETableRuleUtils.GetEnumType(request.GetPostString("tableRule"));
                var tableChoose = request.GetPostString("tableChoose");
                var tableHandWrite = request.GetPostString("tableHandWrite");
                var isImportContents = request.GetPostBool("isImportContents");
                var isImportTableStyles = request.GetPostBool("isImportTableStyles");

                if (!isRoot)
                {
                    if (DirectoryUtils.IsSystemDirectory(siteDir))
                    {
                        return BadRequest("文件夹名称不能为系统文件夹名称，请更改文件夹名称！");
                    }
                    if (!DirectoryUtils.IsDirectoryNameCompliant(siteDir))
                    {
                        return BadRequest("文件夹名称不符合系统要求，请更改文件夹名称！");
                    }
                    var list = DataProvider.SiteDao.GetLowerSiteDirList(parentId);
                    if (list.IndexOf(siteDir.ToLower()) != -1)
                    {
                        return BadRequest("已存在相同的发布路径，请更改文件夹名称！");
                    }
                }

                var channelInfo = new ChannelInfo();

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
                        DataProvider.DatabaseDao.AlterSystemTable(tableName, DataProvider.ContentDao.TableColumnsDefault);
                    }
                }

                var siteInfo = new SiteInfo
                {
                    SiteName = AttackUtils.FilterXss(siteName),
                    SiteDir = siteDir,
                    TableName = tableName,
                    ParentId = parentId,
                    IsRoot = isRoot
                };

                siteInfo.Additional.IsCheckContentLevel = false;
                siteInfo.Additional.Charset = ECharsetUtils.GetValue(ECharset.utf_8);

                var siteId = DataProvider.ChannelDao.InsertSiteInfo(channelInfo, siteInfo, request.AdminName);

                if (string.IsNullOrEmpty(tableName))
                {
                    tableName = ContentDao.GetContentTableName(siteId);
                    DataProvider.ContentDao.CreateContentTable(tableName, DataProvider.ContentDao.TableColumnsDefault);
                    DataProvider.SiteDao.UpdateTableName(siteId, tableName);
                }

                if (request.AdminPermissionsImpl.IsSystemAdministrator && !request.AdminPermissionsImpl.IsConsoleAdministrator)
                {
                    var siteIdList = request.AdminPermissionsImpl.GetSiteIdList() ?? new List<int>();
                    siteIdList.Add(siteId);
                    var adminInfo = AdminManager.GetAdminInfoByUserId(request.AdminId);
                    DataProvider.AdministratorDao.UpdateSiteIdCollection(adminInfo, TranslateUtils.ObjectCollectionToString(siteIdList));
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
