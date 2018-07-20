using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
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

            if (AuthRequest.IsQueryExists("SiteId") && (AuthRequest.IsQueryExists("Up") || AuthRequest.IsQueryExists("Down")))
            {
                var siteId = AuthRequest.GetQueryInt("SiteId");

                if (AuthRequest.IsQueryExists("Up") && AuthRequest.GetQueryBool("Up"))
                {
                    DataProvider.SiteDao.UpdateTaxisToUp(siteId);
                }
                else
                {
                    DataProvider.SiteDao.UpdateTaxisToDown(siteId);
                }

            }

            if (IsPostBack) return;

            VerifySystemPermissions(ConfigManager.SettingsPermissions.Site);

            _hqSiteId = DataProvider.SiteDao.GetIdByIsRoot();

            RptContents.DataSource = SiteManager.GetSiteIdList();
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var siteId = (int)e.Item.DataItem;
            var siteInfo = SiteManager.GetSiteInfo(siteId);
            if (siteInfo == null) return;

            var ltlSiteName = (Literal)e.Item.FindControl("ltlSiteName");
            var ltlSiteDir = (Literal)e.Item.FindControl("ltlSiteDir");
            var ltlAddDate = (Literal)e.Item.FindControl("ltlAddDate");
            var ltlSave = (Literal)e.Item.FindControl("ltlSave");
            var ltlEdit = (Literal)e.Item.FindControl("ltlEdit");
            var ltlChangeType = (Literal)e.Item.FindControl("ltlChangeType");
            var ltlDelete = (Literal)e.Item.FindControl("ltlDelete");
            var ltlUpLink = (Literal)e.Item.FindControl("ltlUpLink");
            var ltlDownLink = (Literal)e.Item.FindControl("ltlDownLink");

            ltlSiteName.Text = GetSiteNameHtml(siteInfo);
            ltlSiteDir.Text = siteInfo.SiteDir;
            ltlAddDate.Text = DateUtils.GetDateString(ChannelManager.GetAddDate(siteId, siteId));

            ltlSave.Text = $@"<a href=""{PageSiteSave.GetRedirectUrl(siteId)}"">保存</a>";

            ltlEdit.Text = $@"<a href=""{PageSiteEdit.GetRedirectUrl(siteId)}"">修改</a>";

            var upUrl = PageUtils.GetSettingsUrl(nameof(PageSite), new NameValueCollection
            {
                {"Up", "True" },
                {"SiteId", siteId.ToString() }
            });
            ltlUpLink.Text = $@"<a href=""{upUrl}""><img src=""../Pic/icon/up.gif"" border=""0"" alt=""上升""/></a>";

            var downUrl = PageUtils.GetSettingsUrl(nameof(PageSite), new NameValueCollection
            {
                {"Down", "True" },
                {"SiteId", siteId.ToString() }
            });
            ltlDownLink.Text = $@"<a href=""{downUrl}""><img src=""../Pic/icon/down.gif"" border=""0"" alt=""下降""/></a>";

            if (siteInfo.ParentId == 0 && (_hqSiteId == 0 || siteId == _hqSiteId))
            {
                ltlChangeType.Text = GetChangeHtml(siteId, siteInfo.IsRoot);
            }

            if (siteInfo.IsRoot == false)
            {
                ltlDelete.Text = $@"<a href=""{PageSiteDelete.GetRedirectUrl(siteId)}"">删除</a>";
            }
        }

        private static string GetSiteNameHtml(SiteInfo siteInfo)
        {
            var level = SiteManager.GetSiteLevel(siteInfo.Id);
            string psLogo;
            if (siteInfo.IsRoot)
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
                $"{padding}<img align='absbottom' border='0' src='{psLogo}'/>&nbsp;<a href='{siteInfo.Additional.WebUrl}' target='_blank'>{siteInfo.SiteName}</a>";
        }

        private static string GetChangeHtml(int siteId, bool isHeadquarters)
        {
            var showPopWinString = ModalChangeSiteType.GetOpenWindowString(siteId);

            return isHeadquarters == false
                ? $"<a href=\"javascript:;\" onClick=\"{showPopWinString}\">转移到根目录</a>"
                : $"<a href=\"javascript:;\" onClick=\"{showPopWinString}\">转移到子目录</a>";
        }
    }
}
