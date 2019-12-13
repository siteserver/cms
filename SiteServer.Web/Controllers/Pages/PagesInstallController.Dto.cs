using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.Context;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Dto;
using SiteServer.CMS.Packaging;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages
{
    public partial class PagesInstallController
    {
        public class GetResult
        {
            public string ProductVersion { get; set; }

            public string NetVersion { get; set; }

            public string ContentRootPath { get; set; }

            public bool RootWritable { get; set; }

            public bool SiteFilesWritable { get; set; }

            public List<Select<string>> DatabaseTypes { get; set; }

            public string AdminUrl { get; set; }

            public List<Select<string>> OraclePrivileges { get; set; }
        }

        public class ConnectRequest
        {
            public DatabaseType DatabaseType { get; set; }
            public string Server { get; set; }
            public bool IsDefaultPort { get; set; }
            public string Port { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
            public EOraclePrivilege OraclePrivilege { get; set; }
            public bool OracleIsSid { get; set; }
            public string OracleDatabase { get; set; }
        }

        public class ConnectResult
        {
            public IList<string> DatabaseNames { get; set; }
        }
    }
}