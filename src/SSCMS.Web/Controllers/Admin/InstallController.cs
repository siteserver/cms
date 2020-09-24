using System.Collections.Generic;
using Datory;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin
{
    [OpenApiIgnore]
    [Route(Constants.ApiAdminPrefix)]
    public partial class InstallController : ControllerBase
    {
        public const string Route = "install";
        private const string RouteDatabaseConnect = "install/actions/databaseConnect";
        private const string RouteRedisConnect = "install/actions/redisConnect";
        private const string RouteInstall = "install/actions/install";
        private const string RoutePrepare = "install/actions/prepare";

        private readonly ISettingsManager _settingsManager;
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IConfigRepository _configRepository;
        private readonly IAdministratorRepository _administratorRepository;

        public InstallController(ISettingsManager settingsManager, IPathManager pathManager, IDatabaseManager databaseManager, IConfigRepository configRepository, IAdministratorRepository administratorRepository)
        {
            _settingsManager = settingsManager;
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _configRepository = configRepository;
            _administratorRepository = administratorRepository;
        }

        public class GetResult
        {
            public bool Forbidden { get; set; }
            public string Version { get; set; }
            public string FrameworkDescription { get; set; }
            public string OSDescription { get; set; }
            public string ContentRootPath { get; set; }
            public string WebRootPath { get; set; }
            public bool RootWritable { get; set; }
            public bool SiteFilesWritable { get; set; }
            public List<Select<string>> DatabaseTypes { get; set; }
            public string AdminUrl { get; set; }
            public bool Containerized { get; set; }
            public DatabaseType DatabaseType { get; set; }
            public string DatabaseConnectionString { get; set; }
            public string RedisConnectionString { get; set; }
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