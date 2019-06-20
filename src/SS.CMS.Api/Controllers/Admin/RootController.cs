using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Core.Common.Create;
using SS.CMS.Core.Packaging;
using SS.CMS.Repositories;
using SS.CMS.Services;
using SS.CMS.Utils;

namespace SS.CMS.Api.Controllers.Admin
{
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
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
        private readonly IUserManager _userManager;
        private readonly ICreateManager _createManager;
        private readonly IConfigRepository _configRepository;
        private readonly IDbCacheRepository _dbCacheRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly ISpecialRepository _specialRepository;
        private readonly ITableStyleRepository _tableStyleRepository;
        private readonly ITemplateRepository _templateRepository;

        public RootController(ISettingsManager settingsManager, IPluginManager pluginManager, IUrlManager urlManager, IPathManager pathManager, IUserManager userManager, ICreateManager createManager, IConfigRepository configRepository, IDbCacheRepository dbCacheRepository, IUserRepository userRepository, ISiteRepository siteRepository, ISpecialRepository specialRepository, ITableStyleRepository tableStyleRepository, ITemplateRepository templateRepository)
        {
            _settingsManager = settingsManager;
            _pluginManager = pluginManager;
            _urlManager = urlManager;
            _pathManager = pathManager;
            _userManager = userManager;
            _createManager = createManager;
            _configRepository = configRepository;
            _dbCacheRepository = dbCacheRepository;
            _userRepository = userRepository;
            _siteRepository = siteRepository;
            _specialRepository = specialRepository;
            _tableStyleRepository = tableStyleRepository;
            _templateRepository = templateRepository;
        }

        [Authorize]
        [HttpGet(Route)]
        public async Task<ActionResult> Get([FromQuery] int? siteId, string pageUrl)
        {
            //if (!_identityManager.IsAdminLoggin)
            //{
            //    return Ok(new
            //    {
            //        Value = false,
            //        RedirectUrl = $"{_urlManager.AdminLoginUrl}?redirectUrl={PageUtils.UrlEncode(_urlManager.GetAdminIndexUrl(siteId, pageUrl))}"
            //    });
            //}

            var adminInfo = await _userManager.GetUserAsync();

            //if (adminInfo.Locked)
            //{
            //    return Ok(new
            //    {
            //        Value = false,
            //        RedirectUrl = $"{_urlManager.AdminErrorUrl}?message={PageUtils.UrlEncode("管理员账号已被锁定，请联系超级管理员协助解决")}"
            //    });
            //}

            var redirect = AdminRedirectCheck(checkInstall: true, checkDatabaseVersion: true);
            if (redirect != null) return Ok(redirect);

            var siteInfo = siteId.HasValue ? _siteRepository.GetSiteInfo(siteId.Value) : null;
            var isSuperAdmin = _userManager.IsSuperAdministrator();
            var siteIdList = await _userManager.GetSiteIdsAsync();

            if (siteInfo == null || !siteIdList.Contains(siteInfo.Id))
            {
                if (siteIdList.Contains(adminInfo.SiteId))
                {
                    return Ok(new
                    {
                        Value = false,
                        RedirectUrl = _urlManager.GetAdminIndexUrl(adminInfo.SiteId, pageUrl)
                    });
                }

                if (siteIdList.Count > 0)
                {
                    return Ok(new
                    {
                        Value = false,
                        RedirectUrl = _urlManager.GetAdminIndexUrl(siteIdList[0], pageUrl)
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

            //var siteIdListLatestAccessed = _userRepository.UpdateSiteId(adminInfo, siteInfo.Id);

            //var permissionList = new List<string>(_userManager.GetAppPermissions());
            //if (_userManager.HasSitePermissions(siteInfo.Id))
            //{
            //    var sitePermissions = _userManager.GetSitePermissions(siteInfo.Id);
            //    if (sitePermissions != null)
            //    {
            //        permissionList.AddRange(sitePermissions);
            //    }
            //}
            //var channelPermissions = permissions.GetChannelPermissions(siteInfo.Id);
            //if (channelPermissions.Count > 0)
            //{
            //    permissionList.AddRange(channelPermissions);
            //}

            var topMenus = GetTopMenus();
            //var siteMenus =
            //    GetLeftMenus(siteInfo, Constants.TopMenu.IdSite, isSuperAdmin, permissionList);
            //var pluginMenus = GetLeftMenus(siteInfo, string.Empty, isSuperAdmin, permissionList);

            var adminInfoToReturn = new
            {
                adminInfo.Id,
                adminInfo.UserName,
                adminInfo.AvatarUrl,
                //Level = permissions.GetAdminLevel()
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
                _settingsManager.ProductVersion,
                _settingsManager.PluginVersion,
                _settingsManager.TargetFramework,
                _settingsManager.EnvironmentVersion,
                IsSuperAdmin = isSuperAdmin,
                PackageList = packageList,
                PackageIds = packageIds,
                _settingsManager.ApiPrefix,
                _settingsManager.AdminPrefix,
                _settingsManager.HomePrefix,
                TopMenus = topMenus,
                //LeftMenus = leftMenus,
                //SiteMenus = siteMenus,
                //PluginMenus = pluginMenus,
                AdminInfo = adminInfoToReturn
            });
        }

        [HttpPost(Route)]
        public async Task<ActionResult> Create()
        {
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
                _dbCacheRepository.RemoveAndInsert(PackageUtils.CacheKeySsCmsIsDownload, true.ToString());
            }

            return Ok(new
            {
                Value = true
            });
        }
    }
}