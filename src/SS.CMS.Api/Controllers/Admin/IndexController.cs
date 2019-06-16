using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions.Repositories;
using SS.CMS.Abstractions.Services;
using SS.CMS.Core.Common;
using SS.CMS.Core.Packaging;
using SS.CMS.Utils;
using SS.CMS.Utils.Enumerations;

namespace SS.CMS.Api.Controllers.Admin
{
    [Route("admin")]
    [ApiController]
    public class IndexController : ControllerBase
    {
        private const string Route = "index";
        private const string RouteUnCheckedList = "index/unCheckedList";

        private readonly ISettingsManager _settingsManager;
        private readonly IPluginManager _pluginManager;
        private readonly IIdentityManager _identityManager;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly ISiteRepository _siteRepository;

        public IndexController(ISettingsManager settingsManager, IPluginManager pluginManager, IIdentityManager identityManager, IAdministratorRepository administratorRepository, ISiteRepository siteRepository)
        {
            _settingsManager = settingsManager;
            _pluginManager = pluginManager;
            _identityManager = identityManager;
            _administratorRepository = administratorRepository;
            _siteRepository = siteRepository;
        }

        [HttpGet(Route)]
        public ActionResult Get()
        {
            if (!_identityManager.IsAdminLoggin)
            {
                return Unauthorized();
            }

            var adminInfo = _administratorRepository.GetAdminInfoByUserId(_identityManager.AdminId);

            return Ok(new
            {
                Value = new
                {
                    Version = SystemManager.ProductVersion == PackageUtils.VersionDev ? "dev" : SystemManager.ProductVersion,
                    LastActivityDate = DateUtils.GetDateString(adminInfo.LastActivityDate, EDateFormatType.Chinese),
                    UpdateDate = DateUtils.GetDateString(_settingsManager.ConfigInfo.UpdateDate, EDateFormatType.Chinese)
                }
            });
        }

        [HttpGet(RouteUnCheckedList)]
        public ActionResult GetUnCheckedList()
        {
            if (!_identityManager.IsAdminLoggin)
            {
                return Unauthorized();
            }

            var unCheckedList = new List<object>();

            foreach (var siteInfo in _siteRepository.GetSiteInfoList())
            {
                if (!_identityManager.AdminPermissions.IsSiteAdmin(siteInfo.Id)) continue;

                var count = siteInfo.ContentRepository.GetCount(_pluginManager, siteInfo, false);
                if (count > 0)
                {
                    unCheckedList.Add(new
                    {
                        //Url = PageContentSearch.GetRedirectUrlCheck(siteInfo.Id),
                        Url = PageUtils.UnClickableUrl,
                        siteInfo.SiteName,
                        Count = count
                    });
                }
            }

            return Ok(new
            {
                Value = unCheckedList
            });
        }
    }
}