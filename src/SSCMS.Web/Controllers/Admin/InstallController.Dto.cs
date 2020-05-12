using System.Collections.Generic;
using Datory;
using SSCMS.Dto;
using SSCMS.Core.Utils;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin
{
    public partial class InstallController
    {
        public class GetResult
        {
            public bool Forbidden { get; set; }
            public string Version { get; set; }

            public string TargetFramework { get; set; }

            public string ContentRootPath { get; set; }
            public string WebRootPath { get; set; }

            public bool RootWritable { get; set; }

            public bool SiteFilesWritable { get; set; }

            public List<Select<string>> DatabaseTypes { get; set; }

            public string AdminUrl { get; set; }
        }

        public class DatabaseConnectRequest
        {
            public DatabaseType DatabaseType { get; set; }
            public string DatabaseHost { get; set; }
            public bool IsDatabaseDefaultPort { get; set; }
            public string DatabasePort { get; set; }
            public string DatabaseUserName { get; set; }
            public string DatabasePassword { get; set; }
        }

        public class DatabaseConnectResult
        {
            public IList<string> DatabaseNames { get; set; }
        }

        public class RedisConnectRequest
        {
            public bool IsRedis { get; set; }
            public string RedisHost { get; set; }
            public bool IsRedisDefaultPort { get; set; }
            public int RedisPort { get; set; }
            public bool IsSsl { get; set; }
            public string RedisPassword { get; set; }
        }

        public class PrepareRequest : RedisConnectRequest
        {
            public DatabaseType DatabaseType { get; set; }
            public string DatabaseHost { get; set; }
            public bool IsDatabaseDefaultPort { get; set; }
            public string DatabasePort { get; set; }
            public string DatabaseUserName { get; set; }
            public string DatabasePassword { get; set; }
            public string DatabaseName { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public string Mobile { get; set; }
            public string AdminPassword { get; set; }
            public bool IsProtectData { get; set; }
        }

        public class InstallRequest : PrepareRequest
        {
            public string SecurityKey { get; set; }
        }
    }
}