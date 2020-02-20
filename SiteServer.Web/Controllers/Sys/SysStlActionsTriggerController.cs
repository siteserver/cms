using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.API.Context;
using SiteServer.CMS.Api.Sys.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;

namespace SiteServer.API.Controllers.Sys
{
    public class SysStlActionsTriggerController : ApiController
    {
        private readonly ICreateManager _createManager;

        public SysStlActionsTriggerController(ICreateManager createManager)
        {
            _createManager = createManager;
        }

        [HttpGet]
        [Route(ApiRouteActionsTrigger.Route)]
        public async Task Main()
        {
            var request = await AuthenticatedRequest.GetAuthAsync();

            var siteId = request.GetQueryInt("siteId");
            var site = await DataProvider.SiteRepository.GetAsync(siteId);

            try
            {
                var channelId = request.GetQueryInt("channelId");
                if (channelId == 0)
                {
                    channelId = siteId;
                }
                var contentId = request.GetQueryInt("contentId");
                var fileTemplateId = request.GetQueryInt("fileTemplateId");
                var specialId = request.GetQueryInt("specialId");
                var isRedirect = TranslateUtils.ToBool(request.GetQueryString("isRedirect"));

                if (specialId != 0)
                {
                    await _createManager.ExecuteAsync(siteId, CreateType.Special, 0, 0, 0, specialId);
                }
                else if (fileTemplateId != 0)
                {
                    await _createManager.ExecuteAsync(siteId, CreateType.File, 0, 0, fileTemplateId, 0);
                }
                else if (contentId != 0)
                {
                    await _createManager.ExecuteAsync(siteId, CreateType.Content, channelId, contentId, 0, 0);
                }
                else if (channelId != 0)
                {
                    await _createManager.ExecuteAsync(siteId, CreateType.Channel, channelId, 0, 0, 0);
                }
                else if (siteId != 0)
                {
                    await _createManager.ExecuteAsync(siteId, CreateType.Channel, siteId, 0, 0, 0);
                }

                if (isRedirect)
                {
                    var channelInfo = await DataProvider.ChannelRepository.GetAsync(channelId);

                    var redirectUrl = string.Empty;
                    if (specialId != 0)
                    {
                        redirectUrl = await PageUtility.GetFileUrlAsync(site, specialId, false);
                    }
                    else if (fileTemplateId != 0)
                    {
                        redirectUrl = await PageUtility.GetFileUrlAsync(site, fileTemplateId, false);
                    }
                    else if (contentId != 0)
                    {
                        var contentInfo = await DataProvider.ContentRepository.GetAsync(site, channelInfo, contentId);
                        redirectUrl = await PageUtility.GetContentUrlAsync(site, contentInfo, false);
                    }
                    else if (channelId != 0)
                    {
                        redirectUrl = await PageUtility.GetChannelUrlAsync(site, channelInfo, false);
                    }
                    else if (siteId != 0)
                    {
                        redirectUrl = await PageUtility.GetIndexPageUrlAsync(site, false);
                    }

                    if (!string.IsNullOrEmpty(redirectUrl))
                    {
                        var parameters = new NameValueCollection();
                        var returnUrl = request.GetQueryString("returnUrl");
                        if (!string.IsNullOrEmpty(returnUrl) && returnUrl.StartsWith("?"))
                        {
                            parameters = TranslateUtils.ToNameValueCollection(returnUrl.Substring(1));
                        }

                        parameters["__r"] = StringUtils.GetRandomInt(1, 10000).ToString();

                        ContextUtils.Redirect(PageUtils.AddQueryString(redirectUrl, parameters));
                        return;
                    }
                }
            }
            catch
            {
                var redirectUrl = await PageUtility.GetIndexPageUrlAsync(site, false);
                ContextUtils.Redirect(redirectUrl);
                return;
            }

            HttpContext.Current.Response.Write(string.Empty);
            HttpContext.Current.Response.End();
        }
    }
}
