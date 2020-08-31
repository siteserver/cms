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
        [OpenApiOperation("修改内容API", "")]
        [HttpPut, Route(RouteContent)]
        public async Task<ActionResult<Content>> Update([FromBody]Content request)
        {
            bool isAuth;
            if (request.SourceId == SourceManager.User)
            {
                isAuth = _authManager.IsUser && await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, Types.ContentPermissions.Edit);
            }
            else
            {
                isAuth = await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeContents) ||
                         _authManager.IsUser &&
                         await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, Types.ContentPermissions.Edit) ||
                         _authManager.IsAdmin &&
                         await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, Types.ContentPermissions.Edit);
            }
            if (!isAuth) return Unauthorized();

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channelInfo = await _channelRepository.GetAsync(request.ChannelId);
            if (channelInfo == null) return NotFound();

            var content = await _contentRepository.GetAsync(site, channelInfo, request.Id);
            if (content == null) return NotFound();

            content.LoadDict(request.ToDictionary());

            content.SiteId = request.SiteId;
            content.ChannelId = request.ChannelId;
            content.LastEditAdminId = _authManager.AdminId;
            content.SourceId = request.SourceId;

            var postCheckedLevel = content.CheckedLevel;
            var isChecked = postCheckedLevel >= site.CheckContentLevel;
            var checkedLevel = postCheckedLevel;

            content.Checked = isChecked;
            content.CheckedLevel = checkedLevel;

            await _contentRepository.UpdateAsync(site, channelInfo, content);

            //foreach (var plugin in _pluginManager.GetPlugins(request.SiteId, request.ChannelId))
            //{
            //    try
            //    {
            //        plugin.OnContentFormSubmit(new ContentFormSubmitEventArgs(request.SiteId, request.ChannelId, content.Id, content.ToDictionary(), content));
            //    }
            //    catch (Exception ex)
            //    {
            //        await _errorLogRepository.AddErrorLogAsync(plugin.PluginId, ex, nameof(IOldPlugin.ContentFormSubmit));
            //    }
            //}

            if (content.Checked)
            {
                await _createManager.CreateContentAsync(request.SiteId, request.ChannelId, content.Id);
                await _createManager.TriggerContentChangedEventAsync(request.SiteId, request.ChannelId);
            }

            await _authManager.AddSiteLogAsync(request.SiteId, request.ChannelId, content.Id, "修改内容",
                $"栏目:{await _channelRepository.GetChannelNameNavigationAsync(request.SiteId, content.ChannelId)},内容标题:{content.Title}");

            return content;
        }
    }
}
