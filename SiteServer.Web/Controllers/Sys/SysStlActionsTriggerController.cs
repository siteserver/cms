using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Caches.Content;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Enumerations;
using SiteServer.CMS.Core.RestRoutes.Sys.Stl;
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
            var rest = new Rest(Request);

            var siteId = rest.GetQueryInt("siteId");
            var siteInfo = SiteManager.GetSiteInfo(siteId);

            try
            {
                var channelId = rest.GetQueryInt("channelId");
                if (channelId == 0)
                {
                    channelId = siteId;
                }
                var contentId = rest.GetQueryInt("contentId");
                var fileTemplateId = rest.GetQueryInt("fileTemplateId");
                var specialId = rest.GetQueryInt("specialId");
                var isRedirect = TranslateUtils.ToBool(rest.GetQueryString("isRedirect"));

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
                    var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);

                    var redirectUrl = string.Empty;
                    if (specialId != 0)
                    {
                        redirectUrl = PageUtility.GetFileUrl(siteInfo, specialId, false);
                    }
                    else if (fileTemplateId != 0)
                    {
                        redirectUrl = PageUtility.GetFileUrl(siteInfo, fileTemplateId, false);
                    }
                    else if (contentId != 0)
                    {
                        var contentInfo = ContentManager.GetContentInfo(siteInfo, channelInfo, contentId);
                        redirectUrl = PageUtility.GetContentUrl(siteInfo, contentInfo, false);
                    }
                    else if (channelId != 0)
                    {
                        redirectUrl = PageUtility.GetChannelUrl(siteInfo, channelInfo, false);
                    }
                    else if (siteId != 0)
                    {
                        redirectUrl = PageUtility.GetIndexPageUrl(siteInfo, false);
                    }

                    if (!string.IsNullOrEmpty(redirectUrl))
                    {
                        var parameters = new NameValueCollection();
                        var returnUrl = rest.GetQueryString("returnUrl");
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
                var redirectUrl = PageUtility.GetIndexPageUrl(siteInfo, false);
                PageUtils.Redirect(redirectUrl);
                return;
            }

            HttpContext.Current.Response.Write(string.Empty);
            HttpContext.Current.Response.End();
        }
    }
}
