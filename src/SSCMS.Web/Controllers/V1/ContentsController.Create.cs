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
        [OpenApiOperation("添加内容API", "")]
        [HttpPost, Route(RouteChannel)]
        public async Task<ActionResult<Content>> Create([FromBody] Content request)
        {
            bool isAuth;
            if (request.SourceId == SourceManager.User)
            {
                isAuth = _authManager.IsUser && await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, Types.ContentPermissions.Add);
            }
            else
            {
                isAuth = await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeContents) ||
                         _authManager.IsUser &&
                         await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, Types.ContentPermissions.Add) ||
                         _authManager.IsAdmin &&
                         await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, Types.ContentPermissions.Add);
            }
            if (!isAuth) return Unauthorized();

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            if (channel == null) return NotFound();

            var checkedLevel = request.CheckedLevel;

            var isChecked = checkedLevel >= site.CheckContentLevel;
            if (isChecked)
            {
                if (request.SourceId == SourceManager.User || _authManager.IsUser)
                {
                    isChecked = await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, Types.ContentPermissions.CheckLevel1);
                }
                else if (_authManager.IsAdmin)
                {
                    isChecked = await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, Types.ContentPermissions.CheckLevel1);
                }
            }

            var adminId = _authManager.AdminId;
            var userId = _authManager.UserId;

            var contentInfo = new Content
            {
                SiteId = request.SiteId,
                ChannelId = request.ChannelId,
                AdminId = adminId,
                LastEditAdminId = adminId,
                UserId = userId,
                SourceId = request.SourceId,
                Checked = isChecked,
                CheckedLevel = checkedLevel
            };
            contentInfo.LoadDict(request.ToDictionary());

            contentInfo.Id = await _contentRepository.InsertAsync(site, channel, contentInfo);

            //foreach (var plugin in _pluginManager.GetPlugins(request.SiteId, request.ChannelId))
            //{
            //    try
            //    {
            //        plugin.OnContentFormSubmit(new ContentFormSubmitEventArgs(request.SiteId, request.ChannelId, contentInfo.Id, request.ToDictionary(), contentInfo));
            //    }
            //    catch (Exception ex)
            //    {
            //        await _errorLogRepository.AddErrorLogAsync(plugin.PluginId, ex, nameof(IOldPlugin.ContentFormSubmit));
            //    }
            //}

            if (contentInfo.Checked)
            {
                await _createManager.CreateContentAsync(request.SiteId, request.ChannelId, contentInfo.Id);
                await _createManager.TriggerContentChangedEventAsync(request.SiteId, request.ChannelId);
            }

            await _authManager.AddSiteLogAsync(request.SiteId, request.ChannelId, contentInfo.Id, "添加内容",
                $"栏目:{await _channelRepository.GetChannelNameNavigationAsync(request.SiteId, contentInfo.ChannelId)},内容标题:{contentInfo.Title}");

            return contentInfo;
        }
    }
}
