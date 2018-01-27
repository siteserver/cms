using System.Text;
using System.Web.UI;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Controls
{
    public class NodeTree : Control
    {
        private SiteInfo _siteInfo;

        protected override void Render(HtmlTextWriter writer)
        {
            var builder = new StringBuilder();

            var request = new Request();

            var siteId = int.Parse(Page.Request.QueryString["SiteId"]);
            _siteInfo = SiteManager.GetSiteInfo(siteId);
            var scripts = ChannelLoading.GetScript(_siteInfo, ELoadingType.ContentTree, null);
            builder.Append(scripts);
            if (Page.Request.QueryString["SiteId"] != null)
            {
                var channelIdList = DataProvider.ChannelDao.GetIdListByParentId(_siteInfo.Id, 0);
                foreach (var channelId in channelIdList)
                {
                    var nodeInfo = ChannelManager.GetChannelInfo(_siteInfo.Id, channelId);
                    var enabled = AdminUtility.IsOwningChannelId(request.AdminName, nodeInfo.Id);
                    if (!enabled)
                    {
                        if (!AdminUtility.IsHasChildOwningChannelId(request.AdminName, nodeInfo.Id)) continue;
                    }

                    builder.Append(ChannelLoading.GetChannelRowHtml(_siteInfo, nodeInfo, enabled, ELoadingType.ContentTree, null, request.AdminName));
                }
            }
            writer.Write(builder);
        }
    }
}