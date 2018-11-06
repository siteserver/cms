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

            VerifySitePermissions(ConfigManager.WebSitePermissions.Create);

            ClientScriptRegisterClientScriptBlock("NodeTreeScript", ChannelLoading.GetScript(SiteInfo, string.Empty, ELoadingType.TemplateFilePathRule, _additional));

            if (AuthRequest.IsQueryExists("CurrentChannelId"))
            {
                _currentChannelId = AuthRequest.GetQueryInt("CurrentChannelId");
                var onLoadScript = ChannelLoading.GetScriptOnLoad(SiteId, _currentChannelId);
                if (!string.IsNullOrEmpty(onLoadScript))
                {
                    ClientScriptRegisterClientScriptBlock("NodeTreeScriptOnLoad", onLoadScript);
                }
            }

            var channelIdList = ChannelManager.GetChannelIdList(ChannelManager.GetChannelInfo(SiteId, SiteId), EScopeType.SelfAndChildren, string.Empty, string.Empty, string.Empty);

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

            var channelInfo = ChannelManager.GetChannelInfo(SiteId, channelId);

            var ltlHtml = (Literal)e.Item.FindControl("ltlHtml");

            ltlHtml.Text = ChannelLoading.GetChannelRowHtml(SiteInfo, channelInfo, enabled, ELoadingType.TemplateFilePathRule, _additional, AuthRequest.AdminPermissionsImpl);
        }
	}
}
