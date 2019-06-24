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
        private readonly ISiteRepository _siteRepository;
        private readonly IConfigRepository _configRepository;

        public IndexController(ISettingsManager settingsManager, IUserManager userManager, ISiteRepository siteRepository, IConfigRepository configRepository)
        {
            _settingsManager = settingsManager;
            _userManager = userManager;
            _siteRepository = siteRepository;
            _configRepository = configRepository;
        }

        [HttpGet(Route)]
        public async Task<ActionResult<GetModel>> Get()
        {
            var accountInfo = await _userManager.GetUserAsync();

            return new GetModel
            {
                Version = _settingsManager.ProductVersion,
                LastActivityDate = accountInfo.LastActivityDate,
                UpdateDate = _configRepository.Instance.UpdateDate
            };
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