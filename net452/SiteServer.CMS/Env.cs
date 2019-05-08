using System;
using System.Net.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.RestRoutes;
using SiteServer.CMS.Fx;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Utils;

namespace SiteServer.CMS
{
    public static class Env
    {
        public static string AdminUrl => PageUtils.Combine(FxUtils.ApplicationPath, WebConfigUtils.AdminDirectory).TrimEnd('/');

        public static string ApiUrl => ApiManager.InnerApiUrl.TrimEnd('/');
    }
}
