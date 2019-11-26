using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Api.Sys.Stl;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.Enumerations;
using SiteServer.CMS.StlParser;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Sys
{
    
    public class SysStlActionsTriggerController : ApiController
    {
        [HttpGet]
        [Route(ApiRouteActionsTrigger.Route)]
        public async Task Main()
        {
            var request = await AuthenticatedRequest.GetAuthAsync();

            var siteId = request.GetQueryInt("siteId");
            var site = await DataProvider.SiteDao.GetAsync(siteId);

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
                    await FileSystemObjectAsync.ExecuteAsync(siteId, ECreateType.Special, 0, 0, 0, specialId);
                }
                else if (fileTemplateId != 0)
                {
                    await FileSystemObjectAsync.ExecuteAsync(siteId, ECreateType.File, 0, 0, fileTemplateId, 0);
                }
                else if (contentId != 0)
                {
                    await FileSystemObjectAsync.ExecuteAsync(siteId, ECreateType.Content, channelId, contentId, 0, 0);
                }
                else if (channelId != 0)
                {
                    await FileSystemObjectAsync.ExecuteAsync(siteId, ECreateType.Channel, channelId, 0, 0, 0);
                }
                else if (siteId != 0)
                {
                    await FileSystemObjectAsync.ExecuteAsync(siteId, ECreateType.Channel, siteId, 0, 0, 0);
                }

                if (isRedirect)
                {
                    var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelId);

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
                        var contentInfo = await ContentManager.GetContentInfoAsync(site, channelInfo, contentId);
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

                        PageUtils.Redirect(PageUtils.AddQueryString(redirectUrl, parameters));
                        return;
                    }
                }
            }
            catch
            {
                var redirectUrl = await PageUtility.GetIndexPageUrlAsync(site, false);
                PageUtils.Redirect(redirectUrl);
                return;
            }

            HttpContext.Current.Response.Write(string.Empty);
            HttpContext.Current.Response.End();
        }
    }
}
