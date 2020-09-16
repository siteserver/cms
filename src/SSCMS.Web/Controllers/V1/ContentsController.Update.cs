using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.V1
{
    public partial class ContentsController
    {
        [OpenApiOperation("修改内容 API", "修改内容，使用PUT发起请求，请求地址为/api/v1/contents/{siteId}/{channelId}/{id}")]
        [HttpPut, Route(RouteContent)]
        public async Task<ActionResult<Content>> Update([FromRoute] int siteId, [FromRoute] int channelId, [FromRoute] int id, [FromBody]Content request)
        {
            if (!await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeContents))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(siteId);
            if (site == null) return NotFound();

            var channelInfo = await _channelRepository.GetAsync(channelId);
            if (channelInfo == null) return NotFound();

            var content = await _contentRepository.GetAsync(site, channelInfo, id);
            if (content == null) return NotFound();

            content.LoadDict(request.ToDictionary());

            content.SiteId = siteId;
            content.ChannelId = channelId;
            content.LastEditAdminId = _authManager.AdminId;
            content.SourceId = request.SourceId;

            var postCheckedLevel = content.CheckedLevel;
            var isChecked = postCheckedLevel >= site.CheckContentLevel;
            var checkedLevel = postCheckedLevel;

            content.Checked = isChecked;
            content.CheckedLevel = checkedLevel;

            await _contentRepository.UpdateAsync(site, channelInfo, content);

            //foreach (var plugin in _pluginManager.GetPlugins(siteId, channelId))
            //{
            //    try
            //    {
            //        plugin.OnContentFormSubmit(new ContentFormSubmitEventArgs(siteId, channelId, content.Id, content.ToDictionary(), content));
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

            await _authManager.AddSiteLogAsync(siteId, channelId, content.Id, "修改内容",
                $"栏目:{await _channelRepository.GetChannelNameNavigationAsync(siteId, content.ChannelId)},内容标题:{content.Title}");

            return content;
        }
    }
}
