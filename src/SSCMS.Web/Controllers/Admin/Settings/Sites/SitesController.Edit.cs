using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Sites
{
    public partial class SitesController
    {
        [HttpPut, Route(Route)]
        public async Task<ActionResult<SitesResult>> Edit([FromBody] EditRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsSites))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

            site.SiteName = request.SiteName;
            site.SiteType = _settingsManager.GetSiteType(request.SiteType).Id;
            site.Taxis = request.Taxis;

            var tableName = string.Empty;
            if (request.TableRule == TableRule.Choose)
            {
                tableName = request.TableChoose;
            }
            else if (request.TableRule == TableRule.HandWrite)
            {
                if (string.IsNullOrEmpty(request.TableHandWrite))
                {
                    return this.Error("站点修改失败，请输入内容表名称");
                }
                tableName = request.TableHandWrite;
                if (!await _settingsManager.Database.IsTableExistsAsync(tableName))
                {
                    await _contentRepository.CreateContentTableAsync(tableName, _contentRepository.GetTableColumns(tableName));
                }
                else
                {
                    await _settingsManager.Database.AlterTableAsync(tableName, _contentRepository.GetTableColumns(tableName));
                }
            }

            if (!StringUtils.EqualsIgnoreCase(site.TableName, tableName))
            {
                site.TableName = tableName;
            }

            if (site.Root == false)
            {
                if (!StringUtils.EqualsIgnoreCase(PathUtils.GetDirectoryName(site.SiteDir, false), request.SiteDir))
                {
                    var list = await _siteRepository.GetSiteDirsAsync(site.ParentId);
                    if (ListUtils.ContainsIgnoreCase(list, request.SiteDir))
                    {
                        return this.Error("站点修改失败，已存在相同的发布路径！");
                    }

                    var parentPsPath = _settingsManager.WebRootPath;
                    if (site.ParentId > 0)
                    {
                        var parentSite = await _siteRepository.GetAsync(site.ParentId);
                        parentPsPath = await _pathManager.GetSitePathAsync(parentSite);
                    }
                    DirectoryUtility.ChangeSiteDir(parentPsPath, site.SiteDir, request.SiteDir);
                }

                if (site.ParentId != request.ParentId)
                {
                    var list = await _siteRepository.GetSiteDirsAsync(request.ParentId);
                    if (ListUtils.ContainsIgnoreCase(list, request.SiteDir))
                    {
                        return this.Error("站点修改失败，已存在相同的发布路径！");
                    }

                    await _pathManager.ChangeParentSiteAsync(site.ParentId, request.ParentId, request.SiteId, request.SiteDir);
                    site.ParentId = request.ParentId;
                }

                site.SiteDir = request.SiteDir;
            }

            await _siteRepository.UpdateAsync(site);

            await _authManager.AddAdminLogAsync("修改站点属性", $"站点:{site.SiteName}");

            var siteIdList = await _siteRepository.GetSiteIdsAsync(0);
            var sites = new List<Site>();
            foreach (var id in siteIdList)
            {
                var info = await _siteRepository.GetAsync(id);
                if (info != null)
                {
                    sites.Add(info);
                }
            }

            return new SitesResult
            {
                Sites = sites
            };
        }
    }
}