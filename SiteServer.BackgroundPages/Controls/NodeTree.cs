using System.Text;
using System.Web.UI;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.Utils;

namespace SiteServer.BackgroundPages.Controls
{
    public class NodeTree : Control
    {
        protected override void Render(HtmlTextWriter writer)
        {
            var builder = new StringBuilder();

            var request = new Request();

            var siteId = TranslateUtils.ToInt(Page.Request.QueryString["siteId"]);
            
            if (siteId > 0)
            {
                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo != null)
                {
                    var scripts = ChannelLoading.GetScript(siteInfo, ELoadingType.ContentTree, null);
                    builder.Append(scripts);

                    var channelIdList = DataProvider.ChannelDao.GetIdListByParentId(siteInfo.Id, 0);
                    foreach (var channelId in channelIdList)
                    {
                        var channelInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);
                        var enabled = AdminUtility.IsOwningChannelId(request.AdminName, channelInfo.Id);
                        if (!enabled)
                        {
                            if (!AdminUtility.IsHasChildOwningChannelId(request.AdminName, channelInfo.Id)) continue;
                        }

                        builder.Append(ChannelLoading.GetChannelRowHtml(siteInfo, channelInfo, enabled, ELoadingType.ContentTree, null, request.AdminName));
                    }
                }
            }
            writer.Write(builder);
        }
    }
}