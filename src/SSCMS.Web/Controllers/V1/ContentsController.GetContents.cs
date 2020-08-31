using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.V1
{
    public partial class ContentsController
    {
        [OpenApiOperation("获取内容列表API", "")]
        [HttpPost, Route(Route)]
        public async Task<ActionResult<QueryResult>> GetContents([FromBody] QueryRequest request)
        {
            var channelId = request.ChannelId ?? request.SiteId;

            var isUserAuth = _authManager.IsUser && await _authManager.HasContentPermissionsAsync(request.SiteId, channelId, Types.ContentPermissions.View);
            var isApiAuth = await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeContents) ||
                            _authManager.IsUser &&
                            await _authManager.HasContentPermissionsAsync(request.SiteId, channelId, Types.ContentPermissions.View) ||
                            _authManager.IsAdmin &&
                            await _authManager.HasContentPermissionsAsync(request.SiteId, channelId, Types.ContentPermissions.View);
            if (!isUserAuth && !isApiAuth) return Unauthorized();

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var tableName = site.TableName;
            var query = await GetQueryAsync(request.SiteId, request.ChannelId, request);
            var totalCount = await _contentRepository.GetCountAsync(tableName, query);
            var summaries = await _contentRepository.GetSummariesAsync(tableName, query.ForPage(request.Page, request.PerPage));

            var contents = new List<Content>();
            foreach (var summary in summaries)
            {
                var content = await _contentRepository.GetAsync(site, summary.ChannelId, summary.Id);
                contents.Add(content);
            }

            return new QueryResult
            {
                Contents = contents,
                TotalCount = totalCount
            };
        }
    }
}
