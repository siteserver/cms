using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Plugin;
using SiteServer.Utils.IO;

namespace SiteServer.BackgroundPages.Settings
{
    public class ModalChangeSiteType : BasePageCms
    {
        protected PlaceHolder PhChangeToSite;
        protected TextBox TbDomainName;

        protected PlaceHolder PhChangeToHeadquarters;

        public Button BtnSubmit;

        private bool _isHeadquarters;

        public static string GetOpenWindowString(int siteId)
        {
            var siteInfo = SiteManager.GetSiteInfo(siteId);
            var title = siteInfo.IsRoot ? "转为子站点" : "转为主站点";
            return LayerUtils.GetOpenScript(title,
                PageUtils.GetSettingsUrl(nameof(ModalChangeSiteType),
                    new NameValueCollection
                    {
                        {"SiteId", siteId.ToString()}
                    }));
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId");

            _isHeadquarters = SiteInfo.IsRoot;

            if (Page.IsPostBack) return;

            if (_isHeadquarters)
            {
                InfoMessage($"将站点{SiteInfo.SiteName}转为子站点");

                PhChangeToSite.Visible = true;
                PhChangeToHeadquarters.Visible = false;
            }
            else
            {
                InfoMessage($"将站点{SiteInfo.SiteName}转为主站点");

                var headquartersExists = false;
                var siteIdList = SiteManager.GetSiteIdList();
                foreach (var psId in siteIdList)
                {
                    var psInfo = SiteManager.GetSiteInfo(psId);
                    if (psInfo.IsRoot)
                    {
                        headquartersExists = true;
                        break;
                    }
                }
                if (headquartersExists)
                {
                    FailMessage($"主站点已经存在，不能再将{SiteInfo.SiteName}转为主战点");
                    BtnSubmit.Visible = false;
                    PhChangeToSite.Visible = false;
                    PhChangeToHeadquarters.Visible = false;
                }
                else
                {
                    PhChangeToSite.Visible = false;
                    PhChangeToHeadquarters.Visible = true;
                }
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (_isHeadquarters)
            {
                SiteInfo.DomainName = TbDomainName.Text;
                DirectoryUtility.ChangeToSubSite(SiteInfo);
            }
            else
            {
                SiteInfo.DomainName = String.Empty;
                DirectoryUtility.ChangeToHeadquarters(SiteInfo);
            }

            AuthRequest.AddAdminLog(_isHeadquarters ? "转为子站点" : "转为主站点",
                $"站点:{SiteInfo.SiteName}");
            LayerUtils.Close(Page);
        }
    }
}
