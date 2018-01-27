using System.Collections.Specialized;
using System.Web;
using System.Web.Http;
using SiteServer.Utils;
using SiteServer.CMS.Controllers.Sys.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser;

namespace SiteServer.API.Controllers.Sys.Stl
{
    [RoutePrefix("api")]
    public class StlActionsTriggerController : ApiController
    {
        [HttpGet]
        [Route(ApiRouteActionsTrigger.Route)]
        public async void Main()
        {
            var request = new Request();

            var siteId = request.GetQueryInt("siteId");
            var siteInfo = SiteManager.GetSiteInfo(siteId);

            try
            {
                var channelId = request.GetQueryInt("channelId");
                if (channelId == 0)
                {
                    channelId = siteId;
                }
                var contentId = request.GetQueryInt("contentId");
                var fileTemplateId = request.GetQueryInt("fileTemplateId");
                var isRedirect = TranslateUtils.ToBool(request.GetQueryString("isRedirect"));

                var nodeInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                var tableName = ChannelManager.GetTableName(siteInfo, nodeInfo);

                if (fileTemplateId != 0)
                {
                    await FileSystemObjectAsync.ExecuteAsync(siteId, ECreateType.File, 0, 0, fileTemplateId);
                }
                else if (contentId != 0)
                {
                    await FileSystemObjectAsync.ExecuteAsync(siteId, ECreateType.Content, channelId, contentId, 0);
                }
                else if (channelId != 0)
                {
                    await FileSystemObjectAsync.ExecuteAsync(siteId, ECreateType.Channel, channelId, 0, 0);
                }
                else if (siteId != 0)
                {
                    await FileSystemObjectAsync.ExecuteAsync(siteId, ECreateType.Channel, siteId, 0, 0);
                }

                if (isRedirect)
                {
                    var redirectUrl = string.Empty;
                    if (fileTemplateId != 0)
                    {
                        redirectUrl = PageUtility.GetFileUrl(siteInfo, fileTemplateId, false);
                    }
                    else if (contentId != 0)
                    {
                        var contentInfo = DataProvider.ContentDao.GetContentInfo(tableName, contentId);
                        redirectUrl = PageUtility.GetContentUrl(siteInfo, contentInfo, false);
                    }
                    else if (channelId != 0)
                    {
                        redirectUrl = PageUtility.GetChannelUrl(siteInfo, nodeInfo, false);
                    }
                    else if (siteId != 0)
                    {
                        redirectUrl = PageUtility.GetIndexPageUrl(siteInfo, false);
                    }

                    if (!string.IsNullOrEmpty(redirectUrl))
                    {
                        var parameters = new NameValueCollection();
                        var returnUrl = request.GetQueryString("returnUrl");
                        if (!string.IsNullOrEmpty(returnUrl))
                        {
                            if (returnUrl.StartsWith("?"))
                            {
                                parameters = TranslateUtils.ToNameValueCollection(returnUrl.Substring(1));
                            }
                            else
                            {
                                redirectUrl = returnUrl;
                            }
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
