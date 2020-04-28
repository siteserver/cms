using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Contents
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class ContentsLayerViewController : ControllerBase
    {
        private const string Route = "cms/contents/contentsLayerView";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IOldPluginManager _pluginManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IContentGroupRepository _contentGroupRepository;
        private readonly IContentTagRepository _contentTagRepository;
        private readonly ITableStyleRepository _tableStyleRepository;

        public ContentsLayerViewController(IAuthManager authManager, IPathManager pathManager, IDatabaseManager databaseManager, IOldPluginManager pluginManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository, IContentGroupRepository contentGroupRepository, IContentTagRepository contentTagRepository, ITableStyleRepository tableStyleRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _pluginManager = pluginManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _contentGroupRepository = contentGroupRepository;
            _contentTagRepository = contentTagRepository;
            _tableStyleRepository = tableStyleRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    AuthTypes.SitePermissions.Contents) ||
                !await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, AuthTypes.SiteContentPermissions.View))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            var content = await _contentRepository.GetAsync(site, channel, request.ContentId);
            if (content == null) return NotFound();

            var channelName = await _channelRepository.GetChannelNameNavigationAsync(request.SiteId, request.ChannelId);

            var columnsManager = new ColumnsManager(_databaseManager, _pluginManager, _pathManager);

            var columns = await columnsManager.GetContentListColumnsAsync(site, channel, ColumnsManager.PageType.Contents);

            var calculatedContent =
                await columnsManager.CalculateContentListAsync(1, site, request.ChannelId, content, columns, null);
            calculatedContent.Body = content.Body;

            var siteUrl = await _pathManager.GetSiteUrlAsync(site, true);
            var groupNames = await _contentGroupRepository.GetGroupNamesAsync(request.SiteId);
            var tagNames = await _contentTagRepository.GetTagNamesAsync(request.SiteId);

            var editorColumns = new List<ContentColumn>();

            var tableName = _channelRepository.GetTableName(site, channel);
            var styleList = await _tableStyleRepository.GetContentStyleListAsync(channel, tableName);
            foreach (var tableStyle in styleList)
            {
                if (tableStyle.InputType == InputType.TextEditor)
                {
                    editorColumns.Add(new ContentColumn
                    {
                        AttributeName = tableStyle.AttributeName,
                        DisplayName = tableStyle.DisplayName
                    });
                }
            }

            return new GetResult
            {
                Content = calculatedContent,
                ChannelName = channelName,
                State = CheckManager.GetCheckState(site, content),
                Columns = columns,
                SiteUrl = siteUrl,
                GroupNames = groupNames,
                TagNames = tagNames,
                EditorColumns = editorColumns
            };
        }
    }
}
