using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Configuration;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Contents
{
    public partial class ContentsController
  {
    [HttpPost, Route(RouteOrder)]
    public async Task<ActionResult<BoolResult>> Submit([FromBody] OrderRequest request)
    {
      if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
              MenuUtils.SitePermissions.Contents) ||
          !await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, MenuUtils.ContentPermissions.Edit))
      {
        return Unauthorized();
      }

      var site = await _siteRepository.GetAsync(request.SiteId);
      if (site == null) return this.Error(Constants.ErrorNotFound);

      var channel = await _channelRepository.GetAsync(request.ChannelId);
      var content = await _contentRepository.GetAsync(site, channel, request.ContentId);

      var isTop = content.Top;
      for (var i = 1; i <= request.Rows; i++)
      {
        if (request.IsUp)
        {
          if (await _contentRepository.SetTaxisToUpAsync(site, channel, request.ContentId, isTop) == false)
          {
            break;
          }
        }
        else
        {
          if (await _contentRepository.SetTaxisToDownAsync(site, channel, request.ContentId, isTop) == false)
          {
            break;
          }
        }
      }

      await _createManager.TriggerContentChangedEventAsync(request.SiteId, channel.Id);

      await _authManager.AddSiteLogAsync(request.SiteId, request.ChannelId, 0, "对内容排序", string.Empty);

      return new BoolResult
      {
          Value = true
      };
    }
  }
}