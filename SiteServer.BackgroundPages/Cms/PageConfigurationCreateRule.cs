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

            VerifySitePermissions(Constants.SitePermissions.ConfigCreateRule);

            ClientScriptRegisterClientScriptBlock("NodeTreeScript", ChannelLoading.GetScript(Site, string.Empty, ELoadingType.TemplateFilePathRule, _additional));

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

            var channelInfo = ChannelManager.GetChannelAsync(SiteId, channelId).GetAwaiter().GetResult();

            var ltlHtml = (Literal)e.Item.FindControl("ltlHtml");

            ltlHtml.Text = ChannelLoading.GetChannelRowHtmlAsync(Site, channelInfo, enabled, ELoadingType.TemplateFilePathRule, _additional, AuthRequest.AdminPermissionsImpl).GetAwaiter().GetResult();
        }
	}
}
