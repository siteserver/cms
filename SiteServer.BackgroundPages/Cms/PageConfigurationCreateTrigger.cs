using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Abstractions;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Context;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Context.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageConfigurationCreateTrigger : BasePageCms
    {
        public Repeater RptContents;

        private int _currentChannelId;

        public static string GetRedirectUrl(int siteId, int channelId)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageConfigurationCreateTrigger), new NameValueCollection
            {
                {"CurrentChannelId", channelId.ToString()}
            });
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId");

			if (!IsPostBack)
			{
                VerifySitePermissions(Constants.WebSitePermissions.Create);

                ClientScriptRegisterClientScriptBlock("NodeTreeScript", ChannelLoading.GetScript(Site, string.Empty, ELoadingType.ConfigurationCreateDetails, null));

                if (AuthRequest.IsQueryExists("CurrentChannelId"))
                {
                    _currentChannelId = AuthRequest.GetQueryInt("CurrentChannelId");
                    var onLoadScript = ChannelLoading.GetScriptOnLoad(SiteId, _currentChannelId);
                    if (!string.IsNullOrEmpty(onLoadScript))
                    {
                        ClientScriptRegisterClientScriptBlock("NodeTreeScriptOnLoad", onLoadScript);
                    }
                }

                BindGrid();
			}
		}

        public void BindGrid()
        {
            var channelIdList = ChannelManager.GetChannelIdListAsync(ChannelManager.GetChannelAsync(SiteId, SiteId).GetAwaiter().GetResult(), EScopeType.SelfAndChildren, string.Empty, string.Empty, string.Empty).GetAwaiter().GetResult();

            RptContents.DataSource = channelIdList;
            RptContents.ItemDataBound += rptContents_ItemDataBound;
            RptContents.DataBind();
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var channelId = (int)e.Item.DataItem;
            var enabled = IsOwningChannelId(channelId);
            if (!enabled)
            {
                if (!IsDescendantOwningChannelId(channelId)) e.Item.Visible = false;
            }

            var nodeInfo = ChannelManager.GetChannelAsync(SiteId, channelId).GetAwaiter().GetResult();
            var ltlHtml = e.Item.FindControl("ltlHtml") as Literal;
            if (ltlHtml != null)
            {
                ltlHtml.Text = ChannelLoading.GetChannelRowHtmlAsync(Site, nodeInfo, enabled, ELoadingType.ConfigurationCreateDetails, null, AuthRequest.AdminPermissionsImpl).GetAwaiter().GetResult();
            }
        }
	}
}
