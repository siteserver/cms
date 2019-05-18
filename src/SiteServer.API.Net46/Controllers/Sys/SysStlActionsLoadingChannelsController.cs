using System.Text;
using System.Web;
using System.Web.Http;
using SiteServer.CMS.Api.Sys.Stl;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.API.Controllers.Sys
{
    public class SysStlActionsLoadingChannelsController : ApiController
    {
        [HttpPost, Route(ApiRouteActionsLoadingChannels.Route)]
        public void Main()
        {
            var builder = new StringBuilder();

            try
            {
                var form = HttpContext.Current.Request.Form;
                var siteId = TranslateUtils.ToInt(form["siteId"]);
                var parentId = TranslateUtils.ToInt(form["parentId"]);
                var target = form["target"];
                var isShowTreeLine = TranslateUtils.ToBool(form["isShowTreeLine"]);
                var isShowContentNum = TranslateUtils.ToBool(form["isShowContentNum"]);
                var currentFormatString = form["currentFormatString"];
                var topChannelId = TranslateUtils.ToInt(form["topChannelId"]);
                var topParentsCount = TranslateUtils.ToInt(form["topParentsCount"]);
                var currentChannelId = TranslateUtils.ToInt(form["currentChannelId"]);

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                var channelIdList = ChannelManager.GetChannelIdList(ChannelManager.GetChannelInfo(siteId, parentId == 0 ? siteId : parentId), EScopeType.Children, string.Empty, string.Empty, string.Empty);

                foreach (var channelId in channelIdList)
                {
                    var nodeInfo = ChannelManager.GetChannelInfo(siteId, channelId);

                    builder.Append(StlTree.GetChannelRowHtml(siteInfo, nodeInfo, target, isShowTreeLine, isShowContentNum, TranslateUtils.DecryptStringBySecretKey(currentFormatString), topChannelId, topParentsCount, currentChannelId, false));
                }
            }
            catch
            {
                // ignored
            }

            HttpContext.Current.Response.Write(builder);
            HttpContext.Current.Response.End();
        }
    }
}
