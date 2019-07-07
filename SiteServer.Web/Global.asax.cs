using SiteServer.CMS.Core;
using SiteServer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace SiteServer.API
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
        }

        protected void Session_Start(object sender, EventArgs e)
        {
        }
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (!SystemManager.IsInstalled)
            {
                String url2 = HttpContext.Current.Request.Url.LocalPath.ToString();
                if (!url2.StartsWith(PageUtils.GetAdminUrl("Installer")) 
                    && !url2.StartsWith(PageUtils.GetAdminUrl("inc")) 
                    && !url2.StartsWith(PageUtils.GetAdminUrl("assets")) 
                    && !url2.StartsWith(PageUtils.GetAdminUrl("Pic")))
                {
                    PageUtils.Redirect(PageUtils.GetAdminUrl("Installer"));
                }
            }
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}