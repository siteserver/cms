using System.Collections.Specialized;
using System.Text;
using System.Web.UI;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Repositories;
using SiteServer.Abstractions;

namespace SiteServer.BackgroundPages.Controls
{
    public class ChannelTree : Control
    {
        protected override void Render(HtmlTextWriter writer)
        {
            var builder = new StringBuilder();

            var request = AuthenticatedRequest.GetAuthAsync().GetAwaiter().GetResult();

            var siteId = TranslateUtils.ToInt(Page.Request.QueryString["siteId"]);

            if (siteId > 0)
            {
                var site = DataProvider.SiteRepository.GetAsync(siteId).GetAwaiter().GetResult();
                if (site != null)
                {
                    var contentModelPluginId = Page.Request.QueryString["contentModelPluginId"];
                    var linkUrl = Page.Request.QueryString["linkUrl"];
                    var additional = new NameValueCollection();
                    if (!string.IsNullOrEmpty(linkUrl))
                    {
                        additional["linkUrl"] = linkUrl;
                    }

                    var scripts = ChannelLoading.GetScript(site, contentModelPluginId, ELoadingType.ContentTree, additional);
                    builder.Append(scripts);

                    var channelIdList = ChannelManager.GetChannelIdListAsync(ChannelManager.GetChannelAsync(site.Id, site.Id).GetAwaiter().GetResult(), EScopeType.SelfAndChildren, string.Empty, string.Empty, string.Empty).GetAwaiter().GetResult();
                    foreach (var channelId in channelIdList)
                    {
                        var channelInfo = ChannelManager.GetChannelAsync(site.Id, channelId).GetAwaiter().GetResult();
                        var enabled = request.AdminPermissionsImpl.IsOwningChannelIdAsync(channelInfo.Id).GetAwaiter().GetResult();
                        if (!string.IsNullOrEmpty(contentModelPluginId) &&
                            !StringUtils.EqualsIgnoreCase(channelInfo.ContentModelPluginId, contentModelPluginId))
                        {
                            enabled = false;
                        }
                        if (!enabled)
                        {
                            if (!request.AdminPermissionsImpl.IsDescendantOwningChannelIdAsync(channelInfo.SiteId, channelInfo.Id).GetAwaiter().GetResult()) continue;
                            if (!IsDesendantContentModelPluginIdExists(channelInfo, contentModelPluginId)) continue;
                        }

                        builder.Append(ChannelLoading.GetChannelRowHtmlAsync(site, channelInfo, enabled, ELoadingType.ContentTree, additional, request.AdminPermissionsImpl).GetAwaiter().GetResult());
                    }
                }
            }
            writer.Write(builder);
        }

        private bool IsDesendantContentModelPluginIdExists(Channel channel, string contentModelPluginId)
        {
            if (string.IsNullOrEmpty(contentModelPluginId)) return true;

            var channelIdList = ChannelManager.GetChannelIdListAsync(channel, EScopeType.Descendant, string.Empty, string.Empty, contentModelPluginId).GetAwaiter().GetResult();
            return channelIdList.Count > 0;
        }
    }
}