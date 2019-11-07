using System;
using System.Text;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageSite : BasePageCms
    {
        public Repeater RptContents;

        private int _hqSiteId;

        public static string GetRedirectUrl()
        {
            return PageUtils.GetSettingsUrl(nameof(PageSite), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (IsPostBack) return;

            VerifySystemPermissions(ConfigManager.SettingsPermissions.Site);

            _hqSiteId = DataProvider.SiteDao.GetIdByIsRootAsync().GetAwaiter().GetResult();

            RptContents.DataSource = SiteManager.GetSiteIdListOrderByLevelAsync().GetAwaiter().GetResult();
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var siteId = (int)e.Item.DataItem;
            var site = SiteManager.GetSiteAsync(siteId).GetAwaiter().GetResult();
            if (site == null) return;

            var ltlSiteName = (Literal)e.Item.FindControl("ltlSiteName");
            var ltlSiteDir = (Literal)e.Item.FindControl("ltlSiteDir");
            var ltlTableName = (Literal)e.Item.FindControl("ltlTableName");
            var ltlTaxis = (Literal)e.Item.FindControl("ltlTaxis");
            var ltlActions = (Literal)e.Item.FindControl("ltlActions");

            ltlSiteName.Text = GetSiteNameHtml(site);
            ltlSiteDir.Text = site.SiteDir;
            ltlTableName.Text = site.TableName;
            ltlTaxis.Text = site.Taxis == 0 ? string.Empty : site.Taxis.ToString();

            var builder = new StringBuilder();

            builder.Append($@"<a href=""{PageSiteSave.GetRedirectUrl(siteId)}"" class=""m-r-5"">保存</a>");
            if (site.ParentId == 0 && (_hqSiteId == 0 || siteId == _hqSiteId))
            {
                builder.Append($@"<a href=""javascript:;"" onClick=""{ModalChangeSiteType.GetOpenWindowString(siteId)}"" class=""m-r-5"">{(site.Root ? "转移到子目录" : "转移到根目录")}</a>");
            }

            ltlActions.Text = builder.ToString();
        }

        private static string GetSiteNameHtml(Site site)
        {
            var level = SiteManager.GetSiteLevelAsync(site.Id).GetAwaiter().GetResult();
            string psLogo;
            if (site.Root)
            {
                psLogo = "siteHQ.gif";
            }
            else
            {
                psLogo = "site.gif";
                if (level > 0 && level < 10)
                {
                    psLogo = $"subsite{level + 1}.gif";
                }
            }
            psLogo = SiteServerAssets.GetIconUrl("tree/" + psLogo);

            var padding = string.Empty;
            for (var i = 0; i < level; i++)
            {
                padding += "　";
            }
            if (level > 0)
            {
                padding += "└ ";
            }

            return
                $"{padding}<img align='absbottom' border='0' src='{psLogo}'/>&nbsp;<a href='{site.Additional.WebUrl}' target='_blank' title='{site.SiteName}'>{StringUtils.MaxLengthText(site.SiteName, 20)}</a>";
        }
    }
}
