using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageConfigurationCreateRule : BasePageCms
    {
        public Repeater RptContents;

        private int _currentChannelId;
        private NameValueCollection _additional;

        public static string GetRedirectUrl(int siteId, int currentChannelId)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageConfigurationCreateRule), new NameValueCollection
            {
                {"CurrentChannelId", currentChannelId.ToString()}
            });
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId");

            _additional = new NameValueCollection();

            if (IsPostBack) return;

            VerifySitePermissions(ConfigManager.Permissions.WebSite.Create);

            ClientScriptRegisterClientScriptBlock("NodeTreeScript", ChannelLoading.GetScript(SiteInfo, ELoadingType.TemplateFilePathRule, _additional));

            if (Body.IsQueryExists("CurrentChannelId"))
            {
                _currentChannelId = Body.GetQueryInt("CurrentChannelId");
                var onLoadScript = ChannelLoading.GetScriptOnLoad(SiteId, _currentChannelId);
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
            var channelId = (int)e.Item.DataItem;
            var enabled = IsOwningChannelId(channelId);
            if (!enabled)
            {
                if (!IsHasChildOwningChannelId(channelId)) e.Item.Visible = false;
            }

            var nodeInfo = ChannelManager.GetChannelInfo(SiteId, channelId);

            var ltlHtml = (Literal)e.Item.FindControl("ltlHtml");

            ltlHtml.Text = ChannelLoading.GetChannelRowHtml(SiteInfo, nodeInfo, enabled, ELoadingType.TemplateFilePathRule, _additional, Body.AdminName);
        }
	}
}
