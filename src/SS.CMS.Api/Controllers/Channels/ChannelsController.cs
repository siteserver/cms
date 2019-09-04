using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Api.Common;
using SS.CMS.Data;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;

namespace SS.CMS.Api.Controllers.Sites.Channels
{
    [ApiController]
    [AllowAnonymous]
    [Route("sites/{siteId:int}/channels")]
    public partial class ChannelsController : ControllerBase
    {
        public const string Route = "";
        public const string RouteId = "{channelId:int}";
        private readonly ISettingsManager _settingsManager;
        private readonly IDatabase _database;
        private readonly IPluginManager _pluginManager;
        private readonly IUserManager _userManager;
        private readonly IUserRepository _userRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly ITemplateRepository _templateRepository;

        public ChannelsController(ISettingsManager settingsManager, IDatabase database, IPluginManager pluginManager, IUserManager userManager, IUserRepository userRepository, ISiteRepository siteRepository, IChannelRepository channelRepository, ITemplateRepository templateRepository)
        {
            _settingsManager = settingsManager;
            _database = database;
            _pluginManager = pluginManager;
            _userManager = userManager;
            _userRepository = userRepository;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _templateRepository = templateRepository;
        }

        [Authorize]
        [HttpGet(Route)]
        public async Task<ActionResult<IList<Channel>>> ListBySiteId(int siteId)
        {
            var list = await _channelRepository.GetChannelListAsync(siteId, 0);
            return list.ToList();
        }

        [Authorize]
        [ClaimRequirement(AuthTypes.ClaimTypes.Role, AuthTypes.Roles.SuperAdministrator)]
        [HttpPost(Route)]
        public async Task<ActionResult<Channel>> Create([FromRoute]int siteId, Channel request)
        {
            if (!_userManager.IsSuperAdministrator())
            {
                return Unauthorized();
            }

            var parent = await _channelRepository.GetChannelAsync(request.ParentId);
            if (parent == null || parent.SiteId != siteId)
            {
                return BadRequest("");
            }
            request.SiteId = siteId;

            var channelId = await _channelRepository.InsertAsync(request);

            var channel = await _channelRepository.GetChannelAsync(channelId);

            return channel;
        }

        [Authorize]
        [ClaimRequirement(AuthTypes.ClaimTypes.Role, AuthTypes.Roles.SuperAdministrator)]
        [HttpDelete(RouteId)]
        public async Task<ActionResult<Channel>> Delete(int siteId, int channelId)
        {
            if (!_userManager.IsSuperAdministrator())
            {
                return Unauthorized();
            }

            var channel = await _channelRepository.DeleteAsync(siteId, channelId);

            return channel;
        }
    }
}