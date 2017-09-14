using System.Collections.Specialized;
using System.Web;
using System.Web.Http;
using BaiRong.Core;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser;

namespace SiteServer.API.Controllers.Stl
{
    [RoutePrefix("api")]
    public class StlActionsTriggerController : ApiController
    {
        [HttpGet]
        [Route(ActionsTrigger.Route)]
        public void Main()
        {
            var body = new RequestBody();

            var publishmentSystemId = body.GetQueryInt("publishmentSystemId");
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);

            try
            {
                var channelId = body.GetQueryInt("channelId");
                if (channelId == 0)
                {
                    channelId = publishmentSystemId;
                }
                var contentId = body.GetQueryInt("contentId");
                var fileTemplateId = body.GetQueryInt("fileTemplateId");
                var isRedirect = TranslateUtils.ToBool(body.GetQueryString("isRedirect"));

                var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, channelId);
                var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, nodeInfo);
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
                        redirectUrl = PageUtility.GetFileUrl(publishmentSystemInfo, fileTemplateId, true);
                    }
                    else if (contentId != 0)
                    {
                        var contentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, contentId);
                        redirectUrl = PageUtility.GetContentUrl(publishmentSystemInfo, contentInfo, true);
                    }
                    else if (channelId != 0)
                    {
                        redirectUrl = PageUtility.GetChannelUrl(publishmentSystemInfo, nodeInfo, true);
                    }
                    else if (publishmentSystemId != 0)
                    {
                        redirectUrl = PageUtility.GetIndexPageUrl(publishmentSystemInfo, true);
                    }

                    if (!string.IsNullOrEmpty(redirectUrl))
                    {
                        var parameters = new NameValueCollection();
                        var returnUrl = body.GetQueryString("returnUrl");
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

                        HttpContext.Current.Response.Redirect(PageUtils.AddQueryString(redirectUrl, parameters), true);
                        return;
                    }
                }
            }
            catch
            {
                var redirectUrl = PageUtility.GetIndexPageUrl(publishmentSystemInfo, true);
                HttpContext.Current.Response.Redirect(redirectUrl, true);
                return;
            }

            HttpContext.Current.Response.Write(string.Empty);
            HttpContext.Current.Response.End();
        }
    }
}
