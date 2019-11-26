using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Google.Protobuf.WellKnownTypes;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Settings.Admin
{
    public partial class PagesAdminViewController
    {
        public class GetRequest
        {
            public int UserId { get; set; }

            public string UserName { get; set; }
        }

        public class GetResult
        {
            public Administrator Administrator { get; set; }
            public string Level { get; set; }
            public bool IsSuperAdmin { get; set; }
            public string SiteNames { get; set; }
            public bool IsOrdinaryAdmin { get; set; }
            public string RoleNames { get; set; }
        }
    }
}
