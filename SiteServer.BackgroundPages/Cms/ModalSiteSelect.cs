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
            RptContents.DataSource = SiteManager.GetSiteIdListOrderByLevel();
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var siteInfo = SiteManager.GetSiteInfo((int)e.Item.DataItem);

            if (!_siteIdList.Contains(siteInfo.Id))
            {
                e.Item.Visible = false;
                return;
            }

            var ltlName = (Literal)e.Item.FindControl("ltlName");
            var ltlDir = (Literal)e.Item.FindControl("ltlDir");
            var ltlWebUrl = (Literal)e.Item.FindControl("ltlWebUrl");

            ltlName.Text = $@"<a href=""{PageUtils.GetLoadingUrl(PageUtils.GetMainUrl(siteInfo.Id))}"" target=""_top"">{SiteManager.GetSiteName(siteInfo)}</a>";
            ltlDir.Text = siteInfo.SiteDir;

            ltlWebUrl.Text = $@"<a href=""{siteInfo.Additional.WebUrl}"" target=""_blank"">{siteInfo.Additional.WebUrl}</a>";
        }
    }
}
