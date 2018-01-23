using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageConfigurationCrossSiteTransChannels : BasePageCms
    {
        public Repeater RptContents;

        private int _currentNodeId;

        public static string GetRedirectUrl(int siteId, int currentNodeId)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageConfigurationCrossSiteTransChannels), new NameValueCollection
            {
                {"CurrentNodeID", currentNodeId.ToString()}
            });
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			PageUtils.CheckRequestParameter("siteId");

            if (IsPostBack) return;

            VerifySitePermissions(AppManager.Permissions.WebSite.Configration);

            ClientScriptRegisterClientScriptBlock("NodeTreeScript", ChannelLoading.GetScript(SiteInfo, ELoadingType.ConfigurationCrossSiteTrans, null));

            if (Body.IsQueryExists("CurrentNodeID"))
            {
                _currentNodeId = Body.GetQueryInt("CurrentNodeID");
                var onLoadScript = ChannelLoading.GetScriptOnLoad(SiteId, _currentNodeId);
                if (!string.IsNullOrEmpty(onLoadScript))
                {
                    ClientScriptRegisterClientScriptBlock("NodeTreeScriptOnLoad", onLoadScript);
                }
            }

            RptContents.DataSource = DataProvider.ChannelDao.GetIdListByParentId(SiteId, 0);
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var nodeId = (int)e.Item.DataItem;
            var enabled = IsOwningNodeId(nodeId);
            if (!enabled)
            {
                if (!IsHasChildOwningNodeId(nodeId)) e.Item.Visible = false;
            }
            var nodeInfo = ChannelManager.GetChannelInfo(SiteId, nodeId);
            var ltlHtml = (Literal)e.Item.FindControl("ltlHtml");
            ltlHtml.Text = ChannelLoading.GetChannelRowHtml(SiteInfo, nodeInfo, enabled, ELoadingType.ConfigurationCrossSiteTrans, null, Body.AdminName);
        }
	}
}
