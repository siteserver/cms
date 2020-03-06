using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SS.CMS.Abstractions;
using SS.CMS.Api.Stl;
using SS.CMS.Framework;

namespace SS.CMS.Web.Controllers.Stl
{
    [OpenApiIgnore]
    public class SysStlActionsLoadingChannelsController : ControllerBase
    {
        [HttpPost, Route(ApiRouteActionsLoadingChannels.Route)]
        public async Task Main()
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

                var site = await SiteManager.GetSiteAsync(siteId);
                var channelIdList = await ChannelManager.GetChannelIdListAsync(await ChannelManager.GetChannelAsync(siteId, parentId == 0 ? siteId : parentId), EScopeType.Children, string.Empty, string.Empty, string.Empty);

                foreach (var channelId in channelIdList)
                {
                    var nodeInfo = await ChannelManager.GetChannelAsync(siteId, channelId);

                    builder.Append(await StlTree.GetChannelRowHtmlAsync(site, nodeInfo, target, isShowTreeLine, isShowContentNum, WebConfigUtils.DecryptStringBySecretKey(currentFormatString), topChannelId, topParentsCount, currentChannelId, false));
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
