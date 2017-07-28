using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Sys
{
    public class PagePublishmentSystem : BasePageCms
    {
        public DataGrid dgContents;
        private int _hqSiteId;

        public static string GetRedirectUrl()
        {
            return PageUtils.GetSysUrl(nameof(PagePublishmentSystem), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (Body.IsQueryExists("PublishmentSystemID") && (Body.IsQueryExists("Up") || Body.IsQueryExists("Down")))
            {
                var publishmentSystemId = Body.GetQueryInt("PublishmentSystemID");

                if (Body.IsQueryExists("Up") && Body.GetQueryBool("Up"))
                {
                    //上升
                    DataProvider.PublishmentSystemDao.UpdateTaxisToUp(publishmentSystemId);
                    //清楚缓存
                    PublishmentSystemManager.ClearCache(false);
                }
                else
                {
                    //下降
                    DataProvider.PublishmentSystemDao.UpdateTaxisToDown(publishmentSystemId);
                    //清楚缓存
                    PublishmentSystemManager.ClearCache(false);
                }

            }

            if (!IsPostBack)
            {
                BreadCrumbSys(AppManager.Sys.LeftMenu.Site, "系统站点管理", AppManager.Sys.Permission.SysSite);

                _hqSiteId = DataProvider.PublishmentSystemDao.GetPublishmentSystemIdByIsHeadquarters();

                dgContents.DataSource = PublishmentSystemManager.GetPublishmentSystemIdList();
                dgContents.ItemDataBound += dgContents_ItemDataBound;
                dgContents.DataBind();
            }
        }

        void dgContents_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var publishmentSystemID = (int)e.Item.DataItem;
                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemID);
                if (publishmentSystemInfo != null)
                {
                    var ltlPublishmentSystemName = e.Item.FindControl("ltlPublishmentSystemName") as Literal;
                    var ltlPublishmentSystemType = e.Item.FindControl("ltlPublishmentSystemType") as Literal;
                    var ltlPublishmentSystemDir = e.Item.FindControl("ltlPublishmentSystemDir") as Literal;
                    var ltlAddDate = e.Item.FindControl("ltlAddDate") as Literal;
                    var ltlChangeType = e.Item.FindControl("ltlChangeType") as Literal;
                    var ltlDelete = e.Item.FindControl("ltlDelete") as Literal;

                    var ltUpLink = e.Item.FindControl("ltUpLink") as Literal;
                    var ltDownLink = e.Item.FindControl("ltDownLink") as Literal;

                    ltlPublishmentSystemName.Text = GetPublishmentSystemNameHtml(publishmentSystemInfo);
                    ltlPublishmentSystemType.Text = EPublishmentSystemTypeUtils.GetHtml(publishmentSystemInfo.PublishmentSystemType);
                    ltlPublishmentSystemDir.Text = publishmentSystemInfo.PublishmentSystemDir;
                    ltlAddDate.Text = DateUtils.GetDateString(NodeManager.GetAddDate(publishmentSystemID, publishmentSystemID));
                    var upUrl = PageUtils.GetSysUrl(nameof(PagePublishmentSystem), new NameValueCollection
                    {
                        {"Up", "True" },
                        {"PublishmentSystemID", publishmentSystemID.ToString() }
                    });
                    ltUpLink.Text = $@"<a href=""{upUrl}""><img src=""../Pic/icon/up.gif"" border=""0"" alt=""上升""/></a>";

                    var downUrl = PageUtils.GetSysUrl(nameof(PagePublishmentSystem), new NameValueCollection
                    {
                        {"Down", "True" },
                        {"PublishmentSystemID", publishmentSystemID.ToString() }
                    });
                    ltDownLink.Text = $@"<a href=""{downUrl}""><img src=""../Pic/icon/down.gif"" border=""0"" alt=""下降""/></a>";

                    if (publishmentSystemInfo.ParentPublishmentSystemId == 0 && (_hqSiteId == 0 || publishmentSystemID == _hqSiteId))
                    {
                        ltlChangeType.Text = GetChangeHtml(publishmentSystemID, publishmentSystemInfo.IsHeadquarters);
                    }

                    if (publishmentSystemInfo.IsHeadquarters == false)
                    {
                        ltlDelete.Text = $@"<a href=""{PagePublishmentSystemDelete.GetRedirectUrl(publishmentSystemID)}"">删除</a>";
                    }
                }
            }
        }

        private string GetPublishmentSystemNameHtml(PublishmentSystemInfo publishmentSystemInfo)
        {
            var level = PublishmentSystemManager.GetPublishmentSystemLevel(publishmentSystemInfo.PublishmentSystemId);
            var psLogo = string.Empty;
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
                $"{padding}<img align='absbottom' border='0' src='{psLogo}'/>&nbsp;<a href='{publishmentSystemInfo.PublishmentSystemUrl}' target='_blank'>{publishmentSystemInfo.PublishmentSystemName}</a>";
        }

        private string GetChangeHtml(int publishmentSystemID, bool isHeadquarters)
        {
            var showPopWinString = ModalChangePublishmentSystemType.GetOpenWindowString(publishmentSystemID);

            if (isHeadquarters == false)
            {
                return $"<a href=\"javascript:;\" onClick=\"{showPopWinString}\">转移到根目录</a>";
            }
            else
            {
                return $"<a href=\"javascript:;\" onClick=\"{showPopWinString}\">转移到子目录</a>";
            }
        }
    }
}
