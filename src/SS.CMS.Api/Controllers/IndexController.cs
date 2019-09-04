using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Core.Packaging;
using SS.CMS.Repositories;
using SS.CMS.Services;
using SS.CMS.Utils;
using SS.CMS.Utils.Enumerations;

namespace SS.CMS.Api.Controllers.Admin
{
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route("admin")]
    [ApiController]
    public partial class IndexController : ControllerBase
    {
        private const string Route = "index";
        private const string RouteUnCheckedList = "index/unCheckedList";

        private readonly ISettingsManager _settingsManager;
        private readonly IUserManager _userManager;
        private readonly IPluginManager _pluginManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IConfigRepository _configRepository;

        public IndexController(ISettingsManager settingsManager, IUserManager userManager, IPluginManager pluginManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IConfigRepository configRepository)
        {
            _settingsManager = settingsManager;
            _userManager = userManager;
            _pluginManager = pluginManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _configRepository = configRepository;
        }

        [HttpGet(Route)]
        public async Task<ActionResult<GetModel>> Get()
        {
            var configInfo = await _configRepository.GetConfigInfoAsync();
            var accountInfo = await _userManager.GetUserAsync();

            return new GetModel
            {
                Version = _settingsManager.ProductVersion,
                LastActivityDate = accountInfo.LastActivityDate,
                UpdateDate = configInfo.UpdateDate
            };
        }

        [HttpGet(RouteUnCheckedList)]
        public async Task<ActionResult> GetUnCheckedList()
        {
            var unCheckedList = new List<object>();

            foreach (var siteInfo in await _siteRepository.GetSiteListAsync())
            {
                if (!_userManager.IsSiteAdministrator(siteInfo.Id)) continue;

                var contentRepository = _channelRepository.GetContentRepository(siteInfo);
                var count = await contentRepository.GetCountAsync(siteInfo, false, _pluginManager);
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