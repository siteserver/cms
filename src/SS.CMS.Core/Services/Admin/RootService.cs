using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using SS.CMS.Core.Api.Preview;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Common;
using SS.CMS.Core.Common.Create;
using SS.CMS.Core.Models;
using SS.CMS.Core.Packaging;
using SS.CMS.Core.Plugin;
using SS.CMS.Core.Plugin.Impl;
using SS.CMS.Core.StlParser;
using SS.CMS.Plugin;
using SS.CMS.Utils;

namespace SS.CMS.Core.Services.Admin
{
    public partial class RootService : ServiceBase
    {
        public const string Route = "";
        public const string RouteActionsDownload = "actions/download";

        public ResponseResult<object> GetConfig(IRequest request, IResponse response)
        {
            var siteId = request.GetQueryInt("siteId");
            var pageUrl = request.GetQueryString("pageUrl");

            if (!request.IsAdminLoggin)
            {
                return Ok(new
                {
                    Value = false,
                    RedirectUrl = $"{AdminUrl.LoginUrl}?redirectUrl={PageUtils.UrlEncode(AdminUrl.GetIndexUrl(siteId, pageUrl))}"
                });
            }

            var adminInfo = AdminManager.GetAdminInfoByUserId(request.AdminId);

            if (adminInfo.Locked)
            {
                return Ok(new
                {
                    Value = false,
                    RedirectUrl = $"{AdminUrl.ErrorUrl}?message={PageUtils.UrlEncode("管理员账号已被锁定，请联系超级管理员协助解决")}"
                });
            }

            var redirect = AdminRedirectCheck(request, checkInstall: true, checkDatabaseVersion: true);
            if (redirect != null) return Ok(redirect);

            var siteInfo = SiteManager.GetSiteInfo(siteId);
            var permissions = (PermissionsImpl)request.AdminPermissions;
            var isSuperAdmin = permissions.IsSuperAdmin();
            var siteIdListWithPermissions = permissions.GetSiteIdList();

            if (siteInfo == null || !siteIdListWithPermissions.Contains(siteInfo.Id))
            {
                if (siteIdListWithPermissions.Contains(adminInfo.SiteId))
                {
                    return Ok(new
                    {
                        Value = false,
                        RedirectUrl = AdminUrl.GetIndexUrl(adminInfo.SiteId, pageUrl)
                    });
                }

                if (siteIdListWithPermissions.Count > 0)
                {
                    return Ok(new
                    {
                        Value = false,
                        RedirectUrl = AdminUrl.GetIndexUrl(siteIdListWithPermissions[0], pageUrl)
                    });
                }

                if (isSuperAdmin)
                {
                    return Ok(new
                    {
                        Value = false,
                        //RedirectUrl = PageSiteAdd.GetRedirectUrl()
                        RedirectUrl = string.Empty
                    });
                }

                return Ok(new
                {
                    Value = false,
                    RedirectUrl = $"{AdminUrl.ErrorUrl}?message={PageUtils.UrlEncode("您没有可以管理的站点，请联系超级管理员协助解决")}"
                });
            }

            var packageIds = new List<string>
                {
                    PackageUtils.PackageIdSsCms
                };
            var packageList = new List<object>();
            var dict = PluginManager.GetPluginIdAndVersionDict();
            foreach (var id in dict.Keys)
            {
                packageIds.Add(id);
                var version = dict[id];
                packageList.Add(new
                {
                    id,
                    version
                });
            }

            var siteIdListLatestAccessed = DataProvider.AdministratorDao.UpdateSiteId(adminInfo, siteInfo.Id);

            var permissionList = new List<string>(permissions.PermissionList);
            if (permissions.HasSitePermissions(siteInfo.Id))
            {
                var websitePermissionList = permissions.GetSitePermissions(siteInfo.Id);
                if (websitePermissionList != null)
                {
                    permissionList.AddRange(websitePermissionList);
                }
            }
            var channelPermissions = permissions.GetChannelPermissions(siteInfo.Id);
            if (channelPermissions.Count > 0)
            {
                permissionList.AddRange(channelPermissions);
            }

            var topMenus = GetTopMenus(siteInfo, isSuperAdmin, siteIdListLatestAccessed, siteIdListWithPermissions);
            var siteMenus =
                GetLeftMenus(siteInfo, ConfigManager.TopMenu.IdSite, isSuperAdmin, permissionList);
            var pluginMenus = GetLeftMenus(siteInfo, string.Empty, isSuperAdmin, permissionList);

            var adminInfoToReturn = new
            {
                adminInfo.Id,
                adminInfo.UserName,
                adminInfo.AvatarUrl,
                Level = permissions.GetAdminLevel()
            };

            var defaultPageUrl = PageUtils.UrlDecode(pageUrl);
            if (string.IsNullOrEmpty(defaultPageUrl))
            {
                defaultPageUrl = PluginMenuManager.GetSystemDefaultPageUrl(siteId);
            }
            if (string.IsNullOrEmpty(defaultPageUrl))
            {
                defaultPageUrl = AdminUrl.DashboardUrl;
            }

            return Ok(new
            {
                Value = true,
                DefaultPageUrl = defaultPageUrl,
                AppSettings.IsNightlyUpdate,
                SystemManager.ProductVersion,
                SystemManager.PluginVersion,
                SystemManager.TargetFramework,
                SystemManager.EnvironmentVersion,
                IsSuperAdmin = isSuperAdmin,
                PackageList = packageList,
                PackageIds = packageIds,
                AppSettings.ApiPrefix,
                AppSettings.AdminDirectory,
                AppSettings.HomeDirectory,
                TopMenus = topMenus,
                SiteMenus = siteMenus,
                PluginMenus = pluginMenus,
                AdminInfo = adminInfoToReturn
            });
        }

        public async Task<ResponseResult<object>> Create(IRequest request, IResponse response)
        {
            if (!request.IsAdminLoggin)
            {
                return Unauthorized();
            }

            var count = CreateTaskManager.PendingTaskCount;

            var pendingTask = CreateTaskManager.GetFirstPendingTask();
            if (pendingTask != null)
            {
                try
                {
                    var start = DateTime.Now;
                    await FileSystemObjectAsync.ExecuteAsync(pendingTask.SiteId, pendingTask.CreateType,
                        pendingTask.ChannelId,
                        pendingTask.ContentId, pendingTask.FileTemplateId, pendingTask.SpecialId);
                    var timeSpan = DateUtils.GetRelatedDateTimeString(start);
                    CreateTaskManager.AddSuccessLog(pendingTask, timeSpan);
                }
                catch (Exception ex)
                {
                    CreateTaskManager.AddFailureLog(pendingTask, ex);
                }
                finally
                {
                    CreateTaskManager.RemovePendingTask(pendingTask);
                }
            }

            return Ok(new
            {
                Value = count
            });
        }

        public ResponseResult<object> Download(IRequest request, IResponse response)
        {
            if (!request.IsAdminLoggin)
            {
                return Unauthorized();
            }

            var packageId = request.GetPostString("packageId");
            var version = request.GetPostString("version");

            try
            {
                PackageUtils.DownloadPackage(packageId, version);
            }
            catch
            {
                PackageUtils.DownloadPackage(packageId, version);
            }

            if (StringUtils.EqualsIgnoreCase(packageId, PackageUtils.PackageIdSsCms))
            {
                CacheDbUtils.RemoveAndInsert(PackageUtils.CacheKeySsCmsIsDownload, true.ToString());
            }

            return Ok(new
            {
                Value = true
            });
        }
    }
}