using System.Collections.Generic;
using Datory;
using SS.CMS.Abstractions.Dto;
using SS.CMS.Core;

namespace SS.CMS.Web.Controllers.Admin
{
    public partial class InstallController
    {
        public class GetResult
        {
            public bool Forbidden { get; set; }
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
            public OraclePrivilege OraclePrivilege { get; set; }
            public bool OracleIsSid { get; set; }
            public string OracleDatabase { get; set; }
        }

        public class ConnectResult
        {
            public IList<string> DatabaseNames { get; set; }
        }

        private string GetConnectionString(bool isDatabaseName, DatabaseType databaseType, string server, bool isDefaultPort, int port, string userName, string password, string selectedDatabaseName, string oracleDatabase, bool oracleIsSid, OraclePrivilege oraclePrivilege)
        {
            var databaseName = string.Empty;
            if (isDatabaseName)
            {
                databaseName = databaseType == DatabaseType.Oracle ? oracleDatabase : selectedDatabaseName;
            }
            return WebUtils.GetConnectionString(databaseType, server, isDefaultPort, port, userName, password, databaseName, oracleIsSid, oraclePrivilege);
        }
    }
}