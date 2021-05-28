using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.V1
{
    public partial class ContentsController
    {
        [OpenApiOperation("获取内容列表 API", "获取内容列表使用 POST 发起请求，请求地址为 /api/v1/contents，系统将根据 POST Body 传递过来的筛选参数获取到内容列表并返回")]
        [HttpPost, Route(Route)]
        public async Task<ActionResult<QueryResult>> GetContents([FromBody] QueryRequest request)
        {
            if (!await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeContents))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var tableName = site.TableName;
            var query = await GetQueryAsync(request.SiteId, request.ChannelId, request);
            var totalCount = await _contentRepository.GetCountAsync(tableName, query);

            var page = request.Page > 0 ? request.Page : 1;
            var perPage = request.PerPage > 0 ? request.PerPage : site.PageSize;
            query.ForPage(page, perPage);

            var summaries = await _contentRepository.GetSummariesAsync(tableName, query);
            var columnsManager = new ColumnsManager(_databaseManager, _pathManager);
            var channel = request.ChannelId.HasValue && request.ChannelId.Value > 0
                ? await _channelRepository.GetAsync(request.ChannelId.Value)
                : await _channelRepository.GetAsync(request.SiteId);
            var columns = await columnsManager.GetContentListColumnsAsync(site, channel, ColumnsManager.PageType.Contents);
            var sequence = page * perPage + 1;

            var pageContents = new List<Content>();
            foreach (var summary in summaries)
            {
                var content = await _contentRepository.GetAsync(site, summary.ChannelId, summary.Id);

                var pageContent =
                    await columnsManager.CalculateContentListAsync(sequence++, site, content.ChannelId, content, columns);
                var navigationUrl = await _parseManager.PathManager.GetContentUrlAsync(site, content, false);
                pageContent.Set("NavigationUrl", navigationUrl);

                pageContents.Add(pageContent);
            }

            return new QueryResult
            {
                Contents = pageContents,
                TotalCount = totalCount
            };
        }
    }
}
