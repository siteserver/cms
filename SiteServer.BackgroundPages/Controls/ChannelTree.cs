using System.Collections.Specialized;
using System.Text;
using System.Web.UI;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Controls
{
    public class ChannelTree : Control
    {
        protected override void Render(HtmlTextWriter writer)
        {
            var builder = new StringBuilder();

            var request = new RequestImpl();

            var siteId = TranslateUtils.ToInt(Page.Request.QueryString["siteId"]);
            var contentModelPluginId = Page.Request.QueryString["contentModelPluginId"];
            var linkUrl = Page.Request.QueryString["linkUrl"];
            var additional = new NameValueCollection();
            if (!string.IsNullOrEmpty(linkUrl))
            {
                additional["linkUrl"] = linkUrl;
            }

            if (siteId > 0)
            {
                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo != null)
                {
                    var scripts = ChannelLoading.GetScript(siteInfo, contentModelPluginId, ELoadingType.ContentTree, additional);
                    builder.Append(scripts);

                    var channelIdList = ChannelManager.GetChannelIdList(ChannelManager.GetChannelInfo(siteInfo.Id, siteInfo.Id), EScopeType.SelfAndChildren, string.Empty, string.Empty, string.Empty);
                    foreach (var channelId in channelIdList)
                    {
                        var channelInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);
                        var enabled = request.AdminPermissionsImpl.IsOwningChannelId(channelInfo.Id);
                        if (!string.IsNullOrEmpty(contentModelPluginId) &&
                            !StringUtils.EqualsIgnoreCase(channelInfo.ContentModelPluginId, contentModelPluginId))
                        {
                            enabled = false;
                        }
                        if (!enabled)
                        {
                            if (!request.AdminPermissionsImpl.IsDescendantOwningChannelId(channelInfo.SiteId, channelInfo.Id)) continue;
                            if (!IsDesendantContentModelPluginIdExists(channelInfo, contentModelPluginId)) continue;
                        }

                        builder.Append(ChannelLoading.GetChannelRowHtml(siteInfo, channelInfo, enabled, ELoadingType.ContentTree, additional, request.AdminPermissionsImpl));
                    }
                }
            }
            writer.Write(builder);
        }

        private bool IsDesendantContentModelPluginIdExists(ChannelInfo channelInfo, string contentModelPluginId)
        {
            if (string.IsNullOrEmpty(contentModelPluginId)) return true;

            var channelIdList = ChannelManager.GetChannelIdList(channelInfo, EScopeType.Descendant, string.Empty, string.Empty, contentModelPluginId);
            return channelIdList.Count > 0;
        }
    }
}