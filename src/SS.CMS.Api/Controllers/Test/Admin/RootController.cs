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
        private const string Route = "";
        private const string RouteActionsValidate = "actions/validate";
        private const string RouteActionsDownload = "actions/download";

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
        public async Task<ActionResult> Get([FromQuery] int siteId)
        {
            var adminInfo = await _userManager.GetUserAsync();

            var siteInfo = _siteRepository.GetSiteInfo(siteId);
            var isSuperAdmin = _userManager.IsSuperAdministrator();
            var siteIdList = await _userManager.GetSiteIdsAsync();

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

            var defaultPageUrl = _urlManager.GetSystemDefaultPageUrl(siteId);

            return Ok(new
            {
                Value = true,
                DefaultPageUrl = defaultPageUrl,
                _settingsManager.IsNightlyUpdate,
                _settingsManager.ProductVersion,
                _settingsManager.PluginVersion,
                _settingsManager.TargetFramework,
                IsSuperAdmin = isSuperAdmin,
                PackageList = packageList,
                PackageIds = packageIds,
                _settingsManager.AdminUrl,
                _settingsManager.HomeUrl,
                TopMenus = topMenus,
                //LeftMenus = leftMenus,
                //SiteMenus = siteMenus,
                //PluginMenus = pluginMenus,
                AdminInfo = adminInfoToReturn
            });
        }

        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [AllowAnonymous]
        [HttpPost(RouteActionsValidate)]
        public async Task<ActionResult<ValidateResult>> Validate([FromQuery] int? siteId)
        {
            var validateResult = new ValidateResult();

            validateResult.IsInstalled = !string.IsNullOrWhiteSpace(_settingsManager.DatabaseConnectionString);
            if (!validateResult.IsInstalled)
            {
                return validateResult;
            }

            validateResult.IsAuthenticated = User.Identity.IsAuthenticated;
            if (!validateResult.IsAuthenticated)
            {
                return validateResult;
            }

            if (_configRepository.Instance == null)
            {
                _configRepository.GetConfigInfo();
            }

            validateResult.IsAdministrator = _userManager.IsAdministrator();
            if (!validateResult.IsAdministrator)
            {
                return validateResult;
            }

            var adminInfo = await _userManager.GetUserAsync();
            validateResult.IsLockedOut = adminInfo.IsLockedOut;
            if (validateResult.IsLockedOut)
            {
                return validateResult;
            }

            validateResult.DatabaseVersion = _configRepository.Instance.DatabaseVersion;
            validateResult.ProductVersion = _settingsManager.ProductVersion;
            if (validateResult.DatabaseVersion != validateResult.ProductVersion)
            {
                return validateResult;
            }

            var siteIdList = await _userManager.GetSiteIdsAsync();
            if (siteId.HasValue && siteIdList.Contains(siteId.Value))
            {
                validateResult.SiteId = siteId.Value;
            }
            else
            {
                if (_userManager.IsSuperAdministrator())
                {
                    if (siteIdList.Count == 0)
                    {
                        validateResult.RedirectToCreateSite = true;
                    }
                    else
                    {
                        validateResult.SiteId = siteIdList[0];
                    }
                }
                else
                {
                    if (siteIdList.Count == 0)
                    {
                        validateResult.RedirectToErrorSite = true;
                        validateResult.ErrorMessage = "您没有可以管理的站点，请联系超级管理员协助解决";
                    }
                    else
                    {
                        validateResult.SiteId = siteIdList[0];
                    }
                }
            }

            return validateResult;
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