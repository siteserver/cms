using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.V1
{
    public partial class ContentsController
    {
        [OpenApiOperation("新增内容 API", "新增内容，使用POST发起请求，请求地址为/api/v1/contents/{siteId}/{channelId}")]
        [HttpPost, Route(RouteChannel)]
        public async Task<ActionResult<Content>> Create([FromRoute] int siteId, [FromRoute] int channelId, [FromBody] Content request)
        {
            if (!await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeContents))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(siteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            var channel = await _channelRepository.GetAsync(channelId);
            if (channel == null) return this.Error(Constants.ErrorNotFound);

            var checkedLevel = request.Checked ? site.CheckContentLevel : request.CheckedLevel;
            var isChecked = checkedLevel >= site.CheckContentLevel;
            if (isChecked)
            {
                if (request.SourceId == SourceManager.User || _authManager.IsUser)
                {
                    isChecked = await _authManager.HasContentPermissionsAsync(siteId, channelId, MenuUtils.ContentPermissions.CheckLevel1);
                }
                else if (_authManager.IsAdmin)
                {
                    isChecked = await _authManager.HasContentPermissionsAsync(siteId, channelId, MenuUtils.ContentPermissions.CheckLevel1);
                }
            }

            var adminId = _authManager.AdminId;
            var userId = _authManager.UserId;

            var content = new Content();
            content.LoadDict(request.ToDictionary());

            content.SiteId = siteId;
            content.ChannelId = channelId;
            content.AdminId = adminId;
            content.LastEditAdminId = adminId;
            content.UserId = userId;
            content.SourceId = request.SourceId;
            content.Checked = isChecked;
            content.CheckedLevel = checkedLevel;
            content.Id = await _contentRepository.InsertAsync(site, channel, content);

            //foreach (var plugin in _pluginManager.GetPlugins(siteId, channelId))
            //{
            //    try
            //    {
            //        plugin.OnContentFormSubmit(new ContentFormSubmitEventArgs(siteId, channelId, content.Id, request.ToDictionary(), content));
            //    }
            //    catch (Exception ex)
            //    {
            //        await _errorLogRepository.AddErrorLogAsync(plugin.PluginId, ex, nameof(IOldPlugin.ContentFormSubmit));
            //    }
            //}

            if (content.Checked)
            {
                await _createManager.CreateContentAsync(siteId, channelId, content.Id);
                await _createManager.TriggerContentChangedEventAsync(siteId, channelId);
            }

            await _authManager.AddSiteLogAsync(siteId, channelId, content.Id, "添加内容",
                $"栏目:{await _channelRepository.GetChannelNameNavigationAsync(siteId, content.ChannelId)},内容标题:{content.Title}");

            return content;
        }
    }
}
