using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions.Repositories;
using SS.CMS.Abstractions.Services;
using SS.CMS.Core.Common;
using SS.CMS.Core.Common.Create;
using SS.CMS.Core.Packaging;
using SS.CMS.Core.Security;
using SS.CMS.Utils;

namespace SS.CMS.Api.Controllers.Admin
{
    [Route("admin")]
    [ApiController]
    public partial class RootController : ControllerBase
    {
        public const string Route = "";
        public const string RouteActionsDownload = "actions/download";

        private readonly ISettingsManager _settingsManager;
        private readonly IPluginManager _pluginManager;
        private readonly IUrlManager _urlManager;
        private readonly IPathManager _pathManager;
        private readonly IIdentityManager _identityManager;
        private readonly ICreateManager _createManager;
        private readonly IMenuManager _menuManager;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly ISpecialRepository _specialRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITableStyleRepository _tableStyleRepository;
        private readonly ITemplateRepository _templateRepository;

        public RootController(ISettingsManager settingsManager, IPluginManager pluginManager, IUrlManager urlManager, IPathManager pathManager, IIdentityManager identityManager, ICreateManager createManager, IMenuManager menuManager, IAdministratorRepository administratorRepository, ISiteRepository siteRepository, ISpecialRepository specialRepository, IUserRepository userRepository, ITableStyleRepository tableStyleRepository, ITemplateRepository templateRepository)
        {
            _settingsManager = settingsManager;
            _pluginManager = pluginManager;
            _urlManager = urlManager;
            _pathManager = pathManager;
            _identityManager = identityManager;
            _createManager = createManager;
            _menuManager = menuManager;
            _administratorRepository = administratorRepository;
            _siteRepository = siteRepository;
            _specialRepository = specialRepository;
            _userRepository = userRepository;
            _tableStyleRepository = tableStyleRepository;
            _templateRepository = templateRepository;
        }

        [HttpGet(Route)]
        public async Task<ActionResult> Get([FromQuery] int? siteId, string pageUrl)
        {
            await _identityManager.Sync();

            if (!_identityManager.IsAdminLoggin)
            {
                return Ok(new
                {
                    Value = false,
                    RedirectUrl = $"{_urlManager.AdminLoginUrl}?redirectUrl={PageUtils.UrlEncode(_urlManager.GetAdminIndexUrl(siteId, pageUrl))}"
                });
            }

            var adminInfo = _administratorRepository.GetAdminInfoByUserId(_identityManager.AdminId);

            if (adminInfo.Locked)
            {
                return Ok(new
                {
                    Value = false,
                    RedirectUrl = $"{_urlManager.AdminErrorUrl}?message={PageUtils.UrlEncode("管理员账号已被锁定，请联系超级管理员协助解决")}"
                });
            }

            var redirect = AdminRedirectCheck(checkInstall: true, checkDatabaseVersion: true);
            if (redirect != null) return Ok(redirect);

            var siteInfo = siteId.HasValue ? _siteRepository.GetSiteInfo(siteId.Value) : null;
            var permissions = (Permissions)_identityManager.AdminPermissions;
            var isSuperAdmin = permissions.IsSuperAdmin();
            var siteIdListWithPermissions = permissions.GetSiteIdList();

            if (siteInfo == null || !siteIdListWithPermissions.Contains(siteInfo.Id))
            {
                if (siteIdListWithPermissions.Contains(adminInfo.SiteId))
                {
                    return Ok(new
                    {
                        Value = false,
                        RedirectUrl = _urlManager.GetAdminIndexUrl(adminInfo.SiteId, pageUrl)
                    });
                }

                if (siteIdListWithPermissions.Count > 0)
                {
                    return Ok(new
                    {
                        Value = false,
                        RedirectUrl = _urlManager.GetAdminIndexUrl(siteIdListWithPermissions[0], pageUrl)
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
                    RedirectUrl = $"{_urlManager.AdminErrorUrl}?message={PageUtils.UrlEncode("您没有可以管理的站点，请联系超级管理员协助解决")}"
                });
            }

            var packageIds = new List<string>
                {
                    PackageUtils.PackageIdSsCms
                };
            var packageList = new List<object>();
            var dict = _pluginManager.GetPluginIdAndVersionDict();
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

            var siteIdListLatestAccessed = _administratorRepository.UpdateSiteId(adminInfo, siteInfo.Id);

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
                GetLeftMenus(siteInfo, Constants.TopMenu.IdSite, isSuperAdmin, permissionList);
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
                defaultPageUrl = _urlManager.GetSystemDefaultPageUrl(siteId ?? 0);
            }
            if (string.IsNullOrEmpty(defaultPageUrl))
            {
                defaultPageUrl = _urlManager.AdminDashboardUrl;
            }

            return Ok(new
            {
                Value = true,
                DefaultPageUrl = defaultPageUrl,
                _settingsManager.IsNightlyUpdate,
                SystemManager.ProductVersion,
                SystemManager.PluginVersion,
                SystemManager.TargetFramework,
                SystemManager.EnvironmentVersion,
                IsSuperAdmin = isSuperAdmin,
                PackageList = packageList,
                PackageIds = packageIds,
                _settingsManager.ApiPrefix,
                _settingsManager.AdminDirectory,
                _settingsManager.HomeDirectory,
                TopMenus = topMenus,
                SiteMenus = siteMenus,
                PluginMenus = pluginMenus,
                AdminInfo = adminInfoToReturn
            });
        }

        [HttpPost(Route)]
        public async Task<ActionResult> Create()
        {
            if (!_identityManager.IsAdminLoggin)
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
                    await _createManager.ExecuteAsync(pendingTask.SiteId, pendingTask.CreateType,
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

        [HttpPost(RouteActionsDownload)]
        public ActionResult Download([FromBody] string packageId, string version)
        {
            if (!_identityManager.IsAdminLoggin)
            {
                return Unauthorized();
            }

            try
            {
                _pluginManager.DownloadPackage(packageId, version);
            }
            catch
            {
                _pluginManager.DownloadPackage(packageId, version);
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