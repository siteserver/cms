using System.Collections.Specialized;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Enums;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Stl
{
    public partial class ActionsTriggerController
    {
        [HttpGet, Route(Constants.RouteStlActionsTrigger)]
        public async Task<RedirectResult> Get([FromQuery] GetRequest request)
        {
            var site = await _siteRepository.GetAsync(request.SiteId);
            var redirectUrl = await _pathManager.GetIndexPageUrlAsync(site, false);

            var channelId = request.ChannelId;
            if (channelId == 0)
            {
                channelId = request.SiteId;
            }

            if (request.SpecialId != 0)
            {
                await _createManager.ExecuteAsync(request.SiteId, CreateType.Special, 0, 0, 0, request.SpecialId);
            }
            else if (request.FileTemplateId != 0)
            {
                await _createManager.ExecuteAsync(request.SiteId, CreateType.File, 0, 0, request.FileTemplateId, 0);
            }
            else if (request.ContentId != 0)
            {
                await _createManager.ExecuteAsync(request.SiteId, CreateType.Content, channelId, request.ContentId, 0, 0);
            }
            else if (channelId != 0)
            {
                await _createManager.ExecuteAsync(request.SiteId, CreateType.Channel, channelId, 0, 0, 0);
            }
            else if (request.SiteId != 0)
            {
                await _createManager.ExecuteAsync(request.SiteId, CreateType.Channel, request.SiteId, 0, 0, 0);
            }

            if (request.IsRedirect)
            {
                var channelInfo = await _channelRepository.GetAsync(channelId);

                if (request.SpecialId != 0)
                {
                    redirectUrl = await _pathManager.GetFileUrlAsync(site, request.SpecialId, false);
                }
                else if (request.FileTemplateId != 0)
                {
                    redirectUrl = await _pathManager.GetFileUrlAsync(site, request.FileTemplateId, false);
                }
                else if (request.ContentId != 0)
                {
                    var contentInfo = await _contentRepository.GetAsync(site, channelInfo, request.ContentId);
                    redirectUrl = await _pathManager.GetContentUrlAsync(site, contentInfo, false);
                }
                else if (channelId != 0)
                {
                    redirectUrl = await _pathManager.GetChannelUrlAsync(site, channelInfo, false);
                }
                else if (request.SiteId != 0)
                {
                    redirectUrl = await _pathManager.GetIndexPageUrlAsync(site, false);
                }

                if (!string.IsNullOrEmpty(redirectUrl))
                {
                    var parameters = new NameValueCollection();
                    if (!string.IsNullOrEmpty(request.ReturnUrl) && request.ReturnUrl.StartsWith("?"))
                    {
                        parameters = TranslateUtils.ToNameValueCollection(request.ReturnUrl.Substring(1));
                    }

                    parameters["__r"] = StringUtils.GetRandomInt(1, 10000).ToString();

                    redirectUrl = PageUtils.AddQueryString(redirectUrl, parameters);
                }
            }

            return Redirect(redirectUrl);
        }
    }
}
