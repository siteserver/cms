using System.Collections.Specialized;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Api.Stl;
using SS.CMS.Core;
using SS.CMS.Framework;

namespace SS.CMS.Web.Controllers.Stl
{
    public partial class ActionsTriggerController : ControllerBase
    {
        private readonly ICreateManager _createManager;

        public ActionsTriggerController(ICreateManager createManager)
        {
            _createManager = createManager;
        }

        [HttpGet]
        [Route(ApiRouteActionsTrigger.Route)]
        public async Task<ActionResult<string>> Get([FromQuery] GetRequest request)
        {
            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);

            try
            {
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
                    var channelInfo = await DataProvider.ChannelRepository.GetAsync(channelId);

                    var redirectUrl = string.Empty;
                    if (request.SpecialId != 0)
                    {
                        redirectUrl = await PageUtility.GetFileUrlAsync(site, request.SpecialId, false);
                    }
                    else if (request.FileTemplateId != 0)
                    {
                        redirectUrl = await PageUtility.GetFileUrlAsync(site, request.FileTemplateId, false);
                    }
                    else if (request.ContentId != 0)
                    {
                        var contentInfo = await DataProvider.ContentRepository.GetAsync(site, channelInfo, request.ContentId);
                        redirectUrl = await PageUtility.GetContentUrlAsync(site, contentInfo, false);
                    }
                    else if (channelId != 0)
                    {
                        redirectUrl = await PageUtility.GetChannelUrlAsync(site, channelInfo, false);
                    }
                    else if (request.SiteId != 0)
                    {
                        redirectUrl = await PageUtility.GetIndexPageUrlAsync(site, false);
                    }

                    if (!string.IsNullOrEmpty(redirectUrl))
                    {
                        var parameters = new NameValueCollection();
                        if (!string.IsNullOrEmpty(request.ReturnUrl) && request.ReturnUrl.StartsWith("?"))
                        {
                            parameters = TranslateUtils.ToNameValueCollection(request.ReturnUrl.Substring(1));
                        }

                        parameters["__r"] = StringUtils.GetRandomInt(1, 10000).ToString();

                        return Redirect(PageUtils.AddQueryString(redirectUrl, parameters));
                    }
                }
            }
            catch
            {
                var redirectUrl = await PageUtility.GetIndexPageUrlAsync(site, false);
                return Redirect(redirectUrl);
            }

            return string.Empty;
        }
    }
}
