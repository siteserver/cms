using System.Web;
using System.Web.Http;
using BaiRong.Core;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.Core;
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

                var fso = new FileSystemObject(publishmentSystemId);
                var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, channelId);
                var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, nodeInfo);
                var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);
                if (fileTemplateId != 0)
                {
                    fso.CreateFile(fileTemplateId);
                }
                else if (contentId != 0)
                {
                    fso.CreateContent(tableStyle, tableName, channelId, contentId);
                }
                else if (channelId != 0)
                {
                    fso.CreateChannel(channelId);
                }
                else if (publishmentSystemId != 0)
                {
                    fso.CreateChannel(publishmentSystemId);
                }

                if (isRedirect)
                {
                    var redirectUrl = string.Empty;
                    if (fileTemplateId != 0)
                    {
                        redirectUrl = PageUtility.GetFileUrl(publishmentSystemInfo, fileTemplateId);
                    }
                    else if (contentId != 0)
                    {
                        var contentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, contentId);
                        redirectUrl = PageUtility.GetContentUrl(publishmentSystemInfo, contentInfo);
                    }
                    else if (channelId != 0)
                    {
                        redirectUrl = PageUtility.GetChannelUrl(publishmentSystemInfo, nodeInfo);
                    }
                    else if (publishmentSystemId != 0)
                    {
                        redirectUrl = PageUtility.GetIndexPageUrl(publishmentSystemInfo);
                    }

                    if (!string.IsNullOrEmpty(redirectUrl))
                    {
                        redirectUrl = PageUtils.AddQueryString(redirectUrl, "__r", StringUtils.GetRandomInt(1, 10000).ToString());
                        HttpContext.Current.Response.Redirect(redirectUrl, true);
                        return;
                    }
                }
            }
            catch
            {
                var redirectUrl = PageUtility.GetIndexPageUrl(publishmentSystemInfo);
                HttpContext.Current.Response.Redirect(redirectUrl, true);
                return;
            }

            HttpContext.Current.Response.Write(string.Empty);
            HttpContext.Current.Response.End();
        }
    }
}
