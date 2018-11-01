using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.Utils.Enumerations;

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
                VerifySitePermissions(ConfigManager.WebSitePermissions.Create);

                ClientScriptRegisterClientScriptBlock("NodeTreeScript", ChannelLoading.GetScript(SiteInfo, string.Empty, ELoadingType.ConfigurationCreateDetails, null));

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
            var channelIdList = ChannelManager.GetChannelIdList(ChannelManager.GetChannelInfo(SiteId, SiteId), EScopeType.SelfAndChildren, string.Empty, string.Empty, string.Empty);

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

            var nodeInfo = ChannelManager.GetChannelInfo(SiteId, channelId);
            var ltlHtml = e.Item.FindControl("ltlHtml") as Literal;
            if (ltlHtml != null)
            {
                ltlHtml.Text = ChannelLoading.GetChannelRowHtml(SiteInfo, nodeInfo, enabled, ELoadingType.ConfigurationCreateDetails, null, AuthRequest.AdminPermissionsImpl);
            }
        }
	}
}
