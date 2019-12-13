using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Context;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.Abstractions;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageConfigurationCrossSiteTransChannels : BasePageCms
    {
        public Repeater RptContents;

        private int _currentChannelId;

        public static string GetRedirectUrl(int siteId, int currentChannelId)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageConfigurationCrossSiteTransChannels), new NameValueCollection
            {
                {"CurrentChannelId", currentChannelId.ToString()}
            });
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			PageUtils.CheckRequestParameter("siteId");

            if (IsPostBack) return;

            VerifySitePermissions(Constants.SitePermissions.ConfigCrossSiteTrans);

            ClientScriptRegisterClientScriptBlock("NodeTreeScript", ChannelLoading.GetScript(Site, string.Empty, ELoadingType.ConfigurationCrossSiteTrans, null));

            if (AuthRequest.IsQueryExists("CurrentChannelId"))
            {
                _currentChannelId = AuthRequest.GetQueryInt("CurrentChannelId");
                var onLoadScript = ChannelLoading.GetScriptOnLoad(SiteId, _currentChannelId);
                if (!string.IsNullOrEmpty(onLoadScript))
                {
                    ClientScriptRegisterClientScriptBlock("NodeTreeScriptOnLoad", onLoadScript);
                }
            }

            var channelIdList = ChannelManager.GetChannelIdListAsync(ChannelManager.GetChannelAsync(SiteId, SiteId).GetAwaiter().GetResult(), EScopeType.SelfAndChildren, string.Empty, string.Empty, string.Empty).GetAwaiter().GetResult();

            RptContents.DataSource = channelIdList;
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var channelId = (int)e.Item.DataItem;
            var enabled = IsOwningChannelId(channelId);
            if (!enabled)
            {
                if (!IsDescendantOwningChannelId(channelId)) e.Item.Visible = false;
            }
            var nodeInfo = ChannelManager.GetChannelAsync(SiteId, channelId).GetAwaiter().GetResult();
            var ltlHtml = (Literal)e.Item.FindControl("ltlHtml");
            ltlHtml.Text = ChannelLoading.GetChannelRowHtmlAsync(Site, nodeInfo, enabled, ELoadingType.ConfigurationCrossSiteTrans, null, AuthRequest.AdminPermissionsImpl).GetAwaiter().GetResult();
        }
	}
}
