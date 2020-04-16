using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Core.Utils;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home
{
    [OpenApiIgnore]
    [Authorize(Roles = Constants.RoleTypeUser)]
    [Route(Constants.ApiHomePrefix)]
    public partial class ContentsController : ControllerBase
    {
        private const string Route = "contents";

        private readonly IAuthManager _authManager;
        private readonly IOldPluginManager _pluginManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;

        public ContentsController(IAuthManager authManager, IOldPluginManager pluginManager, IDatabaseManager databaseManager, IPathManager pathManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository)
        {
            _authManager = authManager;
            _pluginManager = pluginManager;
            _databaseManager = databaseManager;
            _pathManager = pathManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<ListResult>> List([FromQuery]ListRequest request)
        {
            if (!await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, Constants.ContentPermissions.View))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            if (channel == null) return NotFound();

            var columnsManager = new ColumnsManager(_databaseManager, _pluginManager, _pathManager);
            var columns = await columnsManager.GetContentListColumnsAsync(site, channel, ColumnsManager.PageType.Contents);
            var pluginIds = _pluginManager.GetContentPluginIds(channel);
            var pluginColumns = _pluginManager.GetContentColumns(pluginIds);

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

            var userPermissions = new Permissions
            {
                IsAdd = await _authManager.HasContentPermissionsAsync(site.Id, channel.Id, Constants.ContentPermissions.Add),
                IsDelete = await _authManager.HasContentPermissionsAsync(site.Id, channel.Id, Constants.ContentPermissions.Delete),
                IsEdit = await _authManager.HasContentPermissionsAsync(site.Id, channel.Id, Constants.ContentPermissions.Edit),
                IsTranslate = await _authManager.HasContentPermissionsAsync(site.Id, channel.Id, Constants.ContentPermissions.Translate),
                IsCheck = await _authManager.HasContentPermissionsAsync(site.Id, channel.Id, Constants.ContentPermissions.CheckLevel1),
                IsCreate = await _authManager.HasSitePermissionsAsync(site.Id, Constants.SitePermissions.CreateContents) || await _authManager.HasContentPermissionsAsync(site.Id, channel.Id, Constants.ContentPermissions.Create),
                IsChannelEdit = await _authManager.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.Edit)
            };

            return new ListResult
            {
                Contents = pageContentInfoList,
                Count = count,
                Pages = pages,
                Permissions = userPermissions,
                Columns = columns
            };
        }
    }
}
