using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Core;

namespace SS.CMS.Web.Controllers.Home
{
    [Route("home/contents")]
    public partial class ContentsController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;
        private readonly IPluginManager _pluginManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;

        public ContentsController(IAuthManager authManager, IPluginManager pluginManager, IDatabaseManager databaseManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository)
        {
            _authManager = authManager;
            _pluginManager = pluginManager;
            _databaseManager = databaseManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<ListResult>> List([FromQuery]ListRequest request)
        {
            var auth = await _authManager.GetUserAsync();
            if (!auth.IsUserLoggin ||
                !await auth.UserPermissions.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentView))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            if (channel == null) return NotFound();

            var columnsManager = new ColumnsManager(_databaseManager, _pluginManager);
            var columns = await columnsManager.GetContentListColumnsAsync(site, channel, ColumnsManager.PageType.Contents);
            var pluginIds = _pluginManager.GetContentPluginIds(channel);
            var pluginColumns = await _pluginManager.GetContentColumnsAsync(pluginIds);

            var pageContentInfoList = new List<Content>();
            var ccIds = await _contentRepository.GetSummariesAsync(site, channel, true);
            var count = ccIds.Count;

            var pages = Convert.ToInt32(Math.Ceiling((double)count / site.PageSize));
            if (pages == 0) pages = 1;

            if (count > 0)
            {
                var offset = site.PageSize * (request.Page - 1);
                var limit = site.PageSize;
                var pageCcIds = ccIds.Skip(offset).Take(limit).ToList();

                var sequence = offset + 1;
                foreach (var channelContentId in pageCcIds)
                {
                    var contentInfo = await _contentRepository.GetAsync(site, channelContentId.ChannelId, channelContentId.Id);
                    if (contentInfo == null) continue;

                    pageContentInfoList.Add(await columnsManager.CalculateContentListAsync(sequence++, site, request.ChannelId, contentInfo, columns, pluginColumns));
                }
            }

            var permissions = new Permissions
            {
                IsAdd = await auth.UserPermissions.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.ContentAdd),
                IsDelete = await auth.UserPermissions.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.ContentDelete),
                IsEdit = await auth.UserPermissions.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.ContentEdit),
                IsTranslate = await auth.UserPermissions.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.ContentTranslate),
                IsCheck = await auth.UserPermissions.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.ContentCheckLevel1),
                IsCreate = await auth.UserPermissions.HasSitePermissionsAsync(site.Id, Constants.SitePermissions.CreateContents) || await auth.UserPermissions.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.CreatePage),
                IsChannelEdit = await auth.UserPermissions.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.ChannelEdit)
            };

            return new ListResult
            {
                Contents = pageContentInfoList,
                Count = count,
                Pages = pages,
                Permissions = permissions,
                Columns = columns
            };
        }
    }
}
