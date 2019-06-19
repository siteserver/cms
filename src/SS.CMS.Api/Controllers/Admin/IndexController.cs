using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Core.Common;
using SS.CMS.Core.Packaging;
using SS.CMS.Repositories;
using SS.CMS.Services.IPluginManager;
using SS.CMS.Services.ISettingsManager;
using SS.CMS.Services.IUserManager;
using SS.CMS.Utils;
using SS.CMS.Utils.Enumerations;

namespace SS.CMS.Api.Controllers.Admin
{
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route("admin")]
    [ApiController]
    public class IndexController : ControllerBase
    {
        private const string Route = "index";
        private const string RouteUnCheckedList = "index/unCheckedList";

        private readonly ISettingsManager _settingsManager;
        private readonly IPluginManager _pluginManager;
        private readonly IUserManager _userManager;
        private readonly IUserRepository _userRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IConfigRepository _configRepository;

        public IndexController(ISettingsManager settingsManager, IPluginManager pluginManager, IUserManager userManager, IUserRepository userRepository, ISiteRepository siteRepository, IConfigRepository configRepository)
        {
            _settingsManager = settingsManager;
            _pluginManager = pluginManager;
            _userManager = userManager;
            _userRepository = userRepository;
            _siteRepository = siteRepository;
            _configRepository = configRepository;
        }

        [HttpGet(Route)]
        public async Task<ActionResult> Get()
        {
            var accountInfo = await _userManager.GetUserAsync();

            return Ok(new
            {
                Value = new
                {
                    Version = _settingsManager.ProductVersion == PackageUtils.VersionDev ? "dev" : _settingsManager.ProductVersion,
                    LastActivityDate = DateUtils.GetDateString(accountInfo.LastActivityDate, EDateFormatType.Chinese),
                    UpdateDate = DateUtils.GetDateString(_configRepository.Instance.UpdateDate, EDateFormatType.Chinese)
                }
            });
        }

        [HttpGet(RouteUnCheckedList)]
        public ActionResult GetUnCheckedList()
        {
            var unCheckedList = new List<object>();

            foreach (var siteInfo in _siteRepository.GetSiteInfoList())
            {
                if (!_userManager.IsSiteAdministrator(siteInfo.Id)) continue;

                var count = siteInfo.ContentRepository.GetCount(siteInfo, false);
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