using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Core;
using SS.CMS.Framework;

namespace SS.CMS.Web.Controllers.Admin.Cms.Contents
{
    [Route("admin/cms/contents/contentsLayerView")]
    public partial class ContentsLayerViewController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;

        public ContentsLayerViewController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Contents) ||
                !await auth.AdminPermissions.HasChannelPermissionsAsync(request.SiteId, request.ChannelId,
                    Constants.ChannelPermissions.ContentView))
            {
                return Unauthorized();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await DataProvider.ChannelRepository.GetAsync(request.ChannelId);
            var content = await DataProvider.ContentRepository.GetAsync(site, channel, request.ContentId);
            if (content == null) return NotFound();

            var channelName = await DataProvider.ChannelRepository.GetChannelNameNavigationAsync(request.SiteId, request.ChannelId);

            var columns = await ColumnsManager.GetContentListColumnsAsync(site, channel, ColumnsManager.PageType.Contents);

            var calculatedContent =
                await ColumnsManager.CalculateContentListAsync(1, site, request.ChannelId, content, columns, null);
            calculatedContent.Set(ContentAttribute.Content, content.Get(ContentAttribute.Content));

            var siteUrl = await PageUtility.GetSiteUrlAsync(site, true);
            var groupNames = await DataProvider.ContentGroupRepository.GetGroupNamesAsync(request.SiteId);
            var tagNames = await DataProvider.ContentTagRepository.GetTagNamesAsync(request.SiteId);

            var editorColumns = new List<ContentColumn>();

            var tableName = await DataProvider.ChannelRepository.GetTableNameAsync(site, channel);
            var styleList = await DataProvider.TableStyleRepository.GetContentStyleListAsync(channel, tableName);
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
