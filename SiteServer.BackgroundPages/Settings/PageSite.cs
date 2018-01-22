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

            if (Body.IsQueryExists("PublishmentSystemID") && (Body.IsQueryExists("Up") || Body.IsQueryExists("Down")))
            {
                var publishmentSystemId = Body.GetQueryInt("PublishmentSystemID");

                if (Body.IsQueryExists("Up") && Body.GetQueryBool("Up"))
                {
                    DataProvider.PublishmentSystemDao.UpdateTaxisToUp(publishmentSystemId);
                }
                else
                {
                    DataProvider.PublishmentSystemDao.UpdateTaxisToDown(publishmentSystemId);
                }

            }

            if (IsPostBack) return;

            VerifyAdministratorPermissions(AppManager.Permissions.Settings.Site);

            _hqSiteId = DataProvider.PublishmentSystemDao.GetPublishmentSystemIdByIsHeadquarters();

            RptContents.DataSource = PublishmentSystemManager.GetPublishmentSystemIdList();
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var publishmentSystemId = (int)e.Item.DataItem;
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            if (publishmentSystemInfo == null) return;

            var ltlPublishmentSystemName = (Literal)e.Item.FindControl("ltlPublishmentSystemName");
            var ltlPublishmentSystemDir = (Literal)e.Item.FindControl("ltlPublishmentSystemDir");
            var ltlAddDate = (Literal)e.Item.FindControl("ltlAddDate");
            var ltlSave = (Literal)e.Item.FindControl("ltlSave");
            var ltlEdit = (Literal)e.Item.FindControl("ltlEdit");
            var ltlChangeType = (Literal)e.Item.FindControl("ltlChangeType");
            var ltlReplace = (Literal)e.Item.FindControl("ltlReplace");
            var ltlDelete = (Literal)e.Item.FindControl("ltlDelete");
            var ltlUpLink = (Literal)e.Item.FindControl("ltlUpLink");
            var ltlDownLink = (Literal)e.Item.FindControl("ltlDownLink");

            ltlPublishmentSystemName.Text = GetPublishmentSystemNameHtml(publishmentSystemInfo);
            ltlPublishmentSystemDir.Text = publishmentSystemInfo.PublishmentSystemDir;
            ltlAddDate.Text = DateUtils.GetDateString(NodeManager.GetAddDate(publishmentSystemId, publishmentSystemId));

            ltlSave.Text = $@"<a href=""{PageSiteSave.GetRedirectUrl(publishmentSystemId)}"">保存</a>";

            ltlEdit.Text = $@"<a href=""{PageSiteEdit.GetRedirectUrl(publishmentSystemId)}"">修改</a>";

            ltlReplace.Text = $@"<a href=""{PageSiteReplace.GetRedirectUrl(publishmentSystemId)}"">替换</a>";

            var upUrl = PageUtils.GetSettingsUrl(nameof(PageSite), new NameValueCollection
            {
                {"Up", "True" },
                {"PublishmentSystemID", publishmentSystemId.ToString() }
            });
            ltlUpLink.Text = $@"<a href=""{upUrl}""><img src=""../Pic/icon/up.gif"" border=""0"" alt=""上升""/></a>";

            var downUrl = PageUtils.GetSettingsUrl(nameof(PageSite), new NameValueCollection
            {
                {"Down", "True" },
                {"PublishmentSystemID", publishmentSystemId.ToString() }
            });
            ltlDownLink.Text = $@"<a href=""{downUrl}""><img src=""../Pic/icon/down.gif"" border=""0"" alt=""下降""/></a>";

            if (publishmentSystemInfo.ParentPublishmentSystemId == 0 && (_hqSiteId == 0 || publishmentSystemId == _hqSiteId))
            {
                ltlChangeType.Text = GetChangeHtml(publishmentSystemId, publishmentSystemInfo.IsHeadquarters);
            }

            if (publishmentSystemInfo.IsHeadquarters == false)
            {
                ltlDelete.Text = $@"<a href=""{PageSiteDelete.GetRedirectUrl(publishmentSystemId)}"">删除</a>";
            }
        }

        private static string GetPublishmentSystemNameHtml(PublishmentSystemInfo publishmentSystemInfo)
        {
            var level = PublishmentSystemManager.GetPublishmentSystemLevel(publishmentSystemInfo.PublishmentSystemId);
            string psLogo;
            if (publishmentSystemInfo.IsHeadquarters)
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
                $"{padding}<img align='absbottom' border='0' src='{psLogo}'/>&nbsp;<a href='{publishmentSystemInfo.Additional.WebUrl}' target='_blank'>{publishmentSystemInfo.PublishmentSystemName}</a>";
        }

        private static string GetChangeHtml(int publishmentSystemId, bool isHeadquarters)
        {
            var showPopWinString = ModalChangePublishmentSystemType.GetOpenWindowString(publishmentSystemId);

            return isHeadquarters == false
                ? $"<a href=\"javascript:;\" onClick=\"{showPopWinString}\">转移到根目录</a>"
                : $"<a href=\"javascript:;\" onClick=\"{showPopWinString}\">转移到子目录</a>";
        }
    }
}
