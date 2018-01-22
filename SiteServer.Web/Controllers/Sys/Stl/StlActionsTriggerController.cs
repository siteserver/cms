using System.Collections.Specialized;
using System.Web;
using System.Web.Http;
using SiteServer.Utils;
using SiteServer.CMS.Controllers.Sys.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Plugin.Model;
using SiteServer.CMS.StlParser;

namespace SiteServer.API.Controllers.Sys.Stl
{
    [RoutePrefix("api")]
    public class StlActionsTriggerController : ApiController
    {
        [HttpGet]
        [Route(ActionsTrigger.Route)]
        public void Main()
        {
            var context = new RequestContext();

            var publishmentSystemId = context.GetQueryInt("publishmentSystemId");
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);

            try
            {
                var channelId = context.GetQueryInt("channelId");
                if (channelId == 0)
                {
                    channelId = publishmentSystemId;
                }
                var contentId = context.GetQueryInt("contentId");
                var fileTemplateId = context.GetQueryInt("fileTemplateId");
                var isRedirect = TranslateUtils.ToBool(context.GetQueryString("isRedirect"));

                var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, channelId);
                var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);

                if (fileTemplateId != 0)
                {
                    FileSystemObject.Execute(publishmentSystemId, ECreateType.File, 0, 0, fileTemplateId);
                }
                else if (contentId != 0)
                {
                    FileSystemObject.Execute(publishmentSystemId, ECreateType.Content, channelId, contentId, 0);
                }
                else if (channelId != 0)
                {
                    FileSystemObject.Execute(publishmentSystemId, ECreateType.Channel, channelId, 0, 0);
                }
                else if (publishmentSystemId != 0)
                {
                    FileSystemObject.Execute(publishmentSystemId, ECreateType.Channel, publishmentSystemId, 0, 0);
                }

                if (isRedirect)
                {
                    var redirectUrl = string.Empty;
                    if (fileTemplateId != 0)
                    {
                        redirectUrl = PageUtility.GetFileUrl(publishmentSystemInfo, fileTemplateId, false);
                    }
                    else if (contentId != 0)
                    {
                        var contentInfo = DataProvider.ContentDao.GetContentInfo(tableName, contentId);
                        redirectUrl = PageUtility.GetContentUrl(publishmentSystemInfo, contentInfo, false);
                    }
                    else if (channelId != 0)
                    {
                        redirectUrl = PageUtility.GetChannelUrl(publishmentSystemInfo, nodeInfo, false);
                    }
                    else if (publishmentSystemId != 0)
                    {
                        redirectUrl = PageUtility.GetIndexPageUrl(publishmentSystemInfo, false);
                    }

                    if (!string.IsNullOrEmpty(redirectUrl))
                    {
                        var parameters = new NameValueCollection();
                        var returnUrl = context.GetQueryString("returnUrl");
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
                var redirectUrl = PageUtility.GetIndexPageUrl(publishmentSystemInfo, false);
                PageUtils.Redirect(redirectUrl);
                return;
            }

            HttpContext.Current.Response.Write(string.Empty);
            HttpContext.Current.Response.End();
        }
    }
}
