using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Enumerations;

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
                VerifySitePermissions(ConfigManager.Permissions.WebSite.Create);

                ClientScriptRegisterClientScriptBlock("NodeTreeScript", ChannelLoading.GetScript(SiteInfo, ELoadingType.ConfigurationCreateDetails, null));

                if (Body.IsQueryExists("CurrentChannelId"))
                {
                    _currentChannelId = Body.GetQueryInt("CurrentChannelId");
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
            RptContents.DataSource = DataProvider.ChannelDao.GetIdListByParentId(SiteId, 0);
            RptContents.ItemDataBound += rptContents_ItemDataBound;
            RptContents.DataBind();
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var channelId = (int)e.Item.DataItem;
            var enabled = IsOwningChannelId(channelId);
            if (!enabled)
            {
                if (!IsHasChildOwningChannelId(channelId)) e.Item.Visible = false;
            }

            var nodeInfo = ChannelManager.GetChannelInfo(SiteId, channelId);
            var ltlHtml = e.Item.FindControl("ltlHtml") as Literal;
            if (ltlHtml != null)
            {
                ltlHtml.Text = ChannelLoading.GetChannelRowHtml(SiteInfo, nodeInfo, enabled, ELoadingType.ConfigurationCreateDetails, null, Body.AdminName);
            }
        }
	}
}
