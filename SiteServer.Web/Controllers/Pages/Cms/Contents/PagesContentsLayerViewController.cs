using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.API.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Contents
{
    [RoutePrefix("pages/cms/contents/contentsLayerView")]
    public partial class PagesContentsLayerViewController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<GetResult> Get([FromUri] GetRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Contents) ||
                !await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(request.SiteId, request.ChannelId,
                    Constants.ChannelPermissions.ContentView))
            {
                return Request.Unauthorized<GetResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<GetResult>();

            var channel = await DataProvider.ChannelRepository.GetAsync(request.ChannelId);
            var content = await DataProvider.ContentRepository.GetAsync(site, channel, request.ContentId);
            if (content == null) return Request.NotFound<GetResult>();

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
