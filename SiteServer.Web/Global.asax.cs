using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
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
                    return;
                }
            }
            if (SystemManager.IsInstalled && SystemManager.SiteList.Count > 0)
            {
                String host = HttpContext.Current.Request.Url.DnsSafeHost;
                SiteInfo currentSite;
                if (SystemManager.SiteList != null && SystemManager.SiteList.ContainsKey(host))
                {
                    currentSite = SystemManager.SiteList[host];
                }
                else
                {
                    currentSite = SystemManager.SiteList[""];
                }
                String LocalPath = HttpContext.Current.Request.Url.LocalPath.Substring(1);
                if (LocalPath.IndexOf("/") > 0)
                {
                    LocalPath = LocalPath.Substring(0, LocalPath.IndexOf("/"));
                    if (!DirectoryUtils.IsSystemDirectory(LocalPath) && !DirectoryUtils.IsWebSiteDirectory(LocalPath) && LocalPath != currentSite.SiteDir && SystemManager.SiteDirs.Contains("|" + LocalPath + "|"))
                    {
                        PageUtils.Redirect("/" + currentSite.SiteDir + HttpContext.Current.Request.Url.LocalPath.Substring(LocalPath.Length + 1));
                        return;
                    }
                }
                else if (LocalPath != currentSite.SiteDir && SystemManager.SiteDirs.Contains("|" + LocalPath + "|"))
                {
                    PageUtils.Redirect("/" + currentSite.SiteDir + "/");
                    return;
                }
                else if (LocalPath != "404.thml")
                {
                    PageUtils.Redirect("/" + currentSite.SiteDir +"/"+ LocalPath);
                    return;
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