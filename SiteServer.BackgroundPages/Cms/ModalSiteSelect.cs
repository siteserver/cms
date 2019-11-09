using System;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.DataCache;
using System.Collections.Generic;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalSiteSelect : BasePageCms
    {
        public Repeater RptContents;

        private List<int> _siteIdList;

        public static string GetOpenLayerString(int siteId)
        {
            return $@"pageUtils.openLayer({{title: '全部站点',url: '{PageUtils.GetCmsUrl(siteId, nameof(ModalSiteSelect), null)}',full: false,width: 0,height: 0}});return false;";
        }

        public static string GetRedirectUrl(int siteId)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(ModalSiteSelect), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (IsPostBack) return;

            _siteIdList = AuthRequest.AdminPermissionsImpl.GetSiteIdList();
            RptContents.DataSource = SiteManager.GetSiteIdListOrderByLevelAsync().GetAwaiter().GetResult();
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var site = SiteManager.GetSiteAsync((int)e.Item.DataItem).GetAwaiter().GetResult();

            if (!_siteIdList.Contains(site.Id))
            {
                e.Item.Visible = false;
                return;
            }

            var ltlName = (Literal)e.Item.FindControl("ltlName");
            var ltlDir = (Literal)e.Item.FindControl("ltlDir");
            var ltlWebUrl = (Literal)e.Item.FindControl("ltlWebUrl");

            ltlName.Text = $@"<a href=""{PageUtils.GetLoadingUrl(PageUtils.GetMainUrl(site.Id))}"" target=""_top"">{SiteManager.GetSiteNameAsync(site).GetAwaiter().GetResult()}</a>";
            ltlDir.Text = site.SiteDir;

            ltlWebUrl.Text = $@"<a href=""{site.WebUrl}"" target=""_blank"">{site.WebUrl}</a>";
        }
    }
}
