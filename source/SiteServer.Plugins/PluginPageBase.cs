using System;
using System.Web.UI;
using BaiRong.Core;
using SiteServer.CMS.Core;

namespace SiteServer.Plugins
{
    public class PluginPageBase : Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            var body = new RequestBody();

            if (!body.IsAdministratorLoggin)
            {
                PageUtils.RedirectToLoginPage();
            }
        }

        public int SiteId => TranslateUtils.ToInt(Request.QueryString["publishmentSystemId"]);
    }
}
