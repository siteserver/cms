using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;

namespace SiteServer.CMS.Repositories
{
    public partial class DatabaseRepository : IDatabaseRepository
    {
        private readonly IAccessTokenRepository _accessTokenRepository;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly IAdministratorsInRolesRepository _administratorsInRolesRepository;
        private readonly IChannelGroupRepository _channelGroupRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IConfigRepository _configRepository;
        private readonly IContentCheckRepository _contentCheckRepository;
        private readonly IContentGroupRepository _contentGroupRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IContentTagRepository _contentTagRepository;
        private readonly IDbCacheRepository _dbCacheRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly ILibraryFileRepository _libraryFileRepository;
        private readonly ILibraryGroupRepository _libraryGroupRepository;
        private readonly ILibraryImageRepository _libraryImageRepository;
        private readonly ILibraryTextRepository _libraryTextRepository;
        private readonly ILibraryVideoRepository _libraryVideoRepository;
        private readonly ILogRepository _logRepository;
        private readonly IPermissionsInRolesRepository _permissionsInRolesRepository;
        private readonly IPluginConfigRepository _pluginConfigRepository;
        private readonly IPluginRepository _pluginRepository;
        private readonly IRelatedFieldItemRepository _relatedFieldItemRepository;
        private readonly IRelatedFieldRepository _relatedFieldRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ISiteLogRepository _siteLogRepository;
        private readonly ISitePermissionsRepository _sitePermissionsRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly ISpecialRepository _specialRepository;
        private readonly ITableStyleRepository _tableStyleRepository;
        private readonly ITemplateLogRepository _templateLogRepository;
        private readonly ITemplateRepository _templateRepository;
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly IUserLogRepository _userLogRepository;
        private readonly IUserMenuRepository _userMenuRepository;
        private readonly IUserRepository _userRepository;

        public DatabaseRepository(IAccessTokenRepository accessTokenRepository, IAdministratorRepository administratorRepository, IAdministratorsInRolesRepository administratorsInRolesRepository, IChannelGroupRepository channelGroupRepository, IChannelRepository channelRepository, IConfigRepository configRepository, IContentCheckRepository contentCheckRepository, IContentGroupRepository contentGroupRepository, IContentRepository contentRepository, IContentTagRepository contentTagRepository, IDbCacheRepository dbCacheRepository, IErrorLogRepository errorLogRepository, ILibraryFileRepository libraryFileRepository, ILibraryGroupRepository libraryGroupRepository, ILibraryImageRepository libraryImageRepository, ILibraryTextRepository libraryTextRepository, ILibraryVideoRepository libraryVideoRepository, ILogRepository logRepository, IPermissionsInRolesRepository permissionsInRolesRepository, IPluginConfigRepository pluginConfigRepository, IPluginRepository pluginRepository, IRelatedFieldItemRepository relatedFieldItemRepository, IRelatedFieldRepository relatedFieldRepository, IRoleRepository roleRepository, ISiteLogRepository siteLogRepository, ISitePermissionsRepository sitePermissionsRepository, ISiteRepository siteRepository, ISpecialRepository specialRepository, ITableStyleRepository tableStyleRepository, ITemplateLogRepository templateLogRepository, ITemplateRepository templateRepository, IUserGroupRepository userGroupRepository, IUserLogRepository userLogRepository, IUserMenuRepository userMenuRepository, IUserRepository userRepository)
        {
            _accessTokenRepository = accessTokenRepository;
            _administratorRepository = administratorRepository;
            _administratorsInRolesRepository = administratorsInRolesRepository;
            _channelGroupRepository = channelGroupRepository;
            _channelRepository = channelRepository;
            _configRepository = configRepository;
            _contentCheckRepository = contentCheckRepository;
            _contentGroupRepository = contentGroupRepository;
            _contentRepository = contentRepository;
            _contentTagRepository = contentTagRepository;
            _dbCacheRepository = dbCacheRepository;
            _errorLogRepository = errorLogRepository;
            _libraryFileRepository = libraryFileRepository;
            _libraryGroupRepository = libraryGroupRepository;
            _libraryImageRepository = libraryImageRepository;
            _libraryTextRepository = libraryTextRepository;
            _libraryVideoRepository = libraryVideoRepository;
            _logRepository = logRepository;
            _permissionsInRolesRepository = permissionsInRolesRepository;
            _pluginConfigRepository = pluginConfigRepository;
            _pluginRepository = pluginRepository;
            _relatedFieldItemRepository = relatedFieldItemRepository;
            _relatedFieldRepository = relatedFieldRepository;
            _roleRepository = roleRepository;
            _siteLogRepository = siteLogRepository;
            _sitePermissionsRepository = sitePermissionsRepository;
            _siteRepository = siteRepository;
            _specialRepository = specialRepository;
            _tableStyleRepository = tableStyleRepository;
            _templateLogRepository = templateLogRepository;
            _templateRepository = templateRepository;
            _userGroupRepository = userGroupRepository;
            _userLogRepository = userLogRepository;
            _userMenuRepository = userMenuRepository;
            _userRepository = userRepository;
        }

        public List<IRepository> GetAllRepositories()
        {
            var list = new List<IRepository>
            {
                _accessTokenRepository,
                _administratorRepository,
                _administratorsInRolesRepository,
                _channelGroupRepository,
                _channelRepository,
                _configRepository,
                _contentCheckRepository,
                _contentGroupRepository,
                _contentRepository,
                _contentTagRepository,
                _dbCacheRepository,
                _errorLogRepository,
                _libraryFileRepository,
                _libraryGroupRepository,
                _libraryImageRepository,
                _libraryTextRepository,
                _libraryVideoRepository,
                _logRepository,
                _permissionsInRolesRepository,
                _pluginConfigRepository,
                _pluginRepository,
                _relatedFieldItemRepository,
                _relatedFieldRepository,
                _roleRepository,
                _siteLogRepository,
                _sitePermissionsRepository,
                _siteRepository,
                _specialRepository,
                _tableStyleRepository,
                _templateLogRepository,
                _templateRepository,
                _userGroupRepository,
                _userLogRepository,
                _userMenuRepository,
                _userRepository
            };
            return list;
        }

        public Database GetDatabase(string connectionString = null)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = WebConfigUtils.ConnectionString;
            }

            return new Database(WebConfigUtils.DatabaseType, connectionString);
        }

        private IDbConnection GetConnection(string connectionString = null)
        {
            var database = GetDatabase(connectionString);
            return database.GetConnection();
        }

        public async Task DeleteDbLogAsync()
        {
            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                using var connection = WebConfigUtils.Database.GetConnection();
                await connection.ExecuteAsync("PURGE MASTER LOGS BEFORE DATE_SUB( NOW( ), INTERVAL 3 DAY)");
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                var databaseName = await WebConfigUtils.Database.GetDatabaseNamesAsync();

                using var connection = WebConfigUtils.Database.GetConnection();
                var versions = await connection.QueryFirstAsync<string>("SELECT SERVERPROPERTY('productversion')");

                var version = 8;
                var arr = versions.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                if (arr.Length > 0)
                {
                    version = TranslateUtils.ToInt(arr[0], 8);
                }
                if (version < 10)
                {
                    await connection.ExecuteAsync($"BACKUP LOG [{databaseName}] WITH NO_LOG");
                }
                else
                {
                    await connection.ExecuteAsync($@"ALTER DATABASE [{databaseName}] SET RECOVERY SIMPLE;DBCC shrinkfile ([{databaseName}_log], 1); ALTER DATABASE [{databaseName}] SET RECOVERY FULL; ");
                }
            }
        }

        public int GetIntResult(string connectionString, string sqlString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = WebConfigUtils.ConnectionString;
            }

            int count;

            var database = new Database(WebConfigUtils.DatabaseType, connectionString);
            using (var conn = database.GetConnection())
            {
                count = conn.ExecuteScalar<int>(sqlString);
                //conn.Open();
                //using (var rdr = ExecuteReader(conn, sqlString))
                //{
                //    if (rdr.Read())
                //    {
                //        count = GetInt(rdr, 0);
                //    }
                //    rdr.Close();
                //}
            }
            return count;
        }

        public int GetIntResult(string sqlString)
        {
            return GetIntResult(WebConfigUtils.ConnectionString, sqlString);
        }

        public string GetString(string connectionString, string sqlString)
        {
            string value;

            using (var connection = GetConnection(connectionString))
            {
                value = connection.ExecuteScalar<string>(sqlString);
            }

            return value;
        }

        private string GetString(string sqlString)
        {
            string value;

            using (var connection = GetConnection())
            {
                value = connection.ExecuteScalar<string>(sqlString);
            }

            return value;
        }

        public IEnumerable<IDictionary<string, object>> GetRows(string connectionString, string sqlString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = WebConfigUtils.ConnectionString;
            }

            if (string.IsNullOrEmpty(sqlString)) return null;

            IEnumerable<IDictionary<string, object>> rows;

            using (var connection = GetConnection(connectionString))
            {
                rows = connection.Query(sqlString).Cast<IDictionary<string, object>>();
            }

            return rows;
        }

        public int GetPageTotalCount(string sqlString)
        {
            var temp = sqlString.ToLower();
            var pos = temp.LastIndexOf("order by", StringComparison.Ordinal);
            if (pos > -1)
                sqlString = sqlString.Substring(0, pos);

            // Add new ORDER BY info if SortKeyField is specified
            //if (!string.IsNullOrEmpty(sortField) && addCustomSortInfo)
            //    SelectCommand += " ORDER BY " + SortField;

            var cmdText = WebConfigUtils.DatabaseType == DatabaseType.Oracle
                ? $"SELECT COUNT(*) FROM ({sqlString})"
                : $"SELECT COUNT(*) FROM ({sqlString}) AS T0";
            return GetIntResult(cmdText);
        }

        public string GetStlPageSqlString(string sqlString, string orderString, int totalCount, int itemsPerPage, int currentPageIndex)
        {
            var retVal = string.Empty;

            var temp = sqlString.ToLower();
            var pos = temp.LastIndexOf("order by", StringComparison.Ordinal);
            if (pos > -1)
                sqlString = sqlString.Substring(0, pos);

            var recordsInLastPage = itemsPerPage;

            // Calculate the correspondent number of pages
            var lastPage = totalCount / itemsPerPage;
            var remainder = totalCount % itemsPerPage;
            if (remainder > 0)
                lastPage++;
            var pageCount = lastPage;

            if (remainder > 0)
                recordsInLastPage = remainder;

            var recsToRetrieve = itemsPerPage;
            if (currentPageIndex == pageCount - 1)
                recsToRetrieve = recordsInLastPage;

            orderString = orderString.ToUpper();
            var orderStringReverse = orderString.Replace(" DESC", " DESC2");
            orderStringReverse = orderStringReverse.Replace(" ASC", " DESC");
            orderStringReverse = orderStringReverse.Replace(" DESC2", " ASC");

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                retVal = $@"
SELECT * FROM (
    SELECT * FROM (
        SELECT * FROM ({sqlString}) AS t0 {orderString} LIMIT {itemsPerPage * (currentPageIndex + 1)}
    ) AS t1 {orderStringReverse} LIMIT {recsToRetrieve}
) AS t2 {orderString}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                retVal = $@"
SELECT * FROM (
    SELECT TOP {recsToRetrieve} * FROM (
        SELECT TOP {itemsPerPage * (currentPageIndex + 1)} * FROM ({sqlString}) AS t0 {orderString}
    ) AS t1 {orderStringReverse}
) AS t2 {orderString}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                retVal = $@"
SELECT * FROM (
    SELECT * FROM (
        SELECT * FROM ({sqlString}) AS t0 {orderString} LIMIT {itemsPerPage * (currentPageIndex + 1)}
    ) AS t1 {orderStringReverse} LIMIT {recsToRetrieve}
) AS t2 {orderString}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                retVal = $@"
SELECT * FROM (
    SELECT * FROM (
        SELECT * FROM ({sqlString}) WHERE ROWNUM <= {itemsPerPage * (currentPageIndex + 1)} {orderString}
    ) WHERE ROWNUM <= {recsToRetrieve} {orderStringReverse}
) {orderString}";
            }

            //            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            //            {
            //                return $@"
            //SELECT * FROM (
            //    SELECT * FROM (
            //        SELECT * FROM ({sqlString}) AS t0 {orderString} LIMIT {itemsPerPage * (currentPageIndex + 1)}
            //    ) AS t1 {orderStringReverse} LIMIT {recsToRetrieve}
            //) AS t2 {orderString}";
            //            }
            //            else
            //            {
            //                return $@"
            //SELECT * FROM (
            //    SELECT TOP {recsToRetrieve} * FROM (
            //        SELECT TOP {itemsPerPage * (currentPageIndex + 1)} * FROM ({sqlString}) AS t0 {orderString}
            //    ) AS t1 {orderStringReverse}
            //) AS t2 {orderString}";
            //            }

            return retVal;
        }

        public string GetSelectSqlString(string tableName, int totalNum, string columns, string whereString, string orderByString)
        {
            return GetSelectSqlString(WebConfigUtils.ConnectionString, tableName, totalNum, columns, whereString, orderByString);
        }

        private string GetSelectSqlString(string connectionString, string tableName, int totalNum, string columns, string whereString, string orderByString)
        {
            return GetSelectSqlString(connectionString, tableName, totalNum, columns, whereString, orderByString, string.Empty);
        }

        private string GetSelectSqlString(string connectionString, string tableName, int totalNum, string columns, string whereString, string orderByString, string joinString)
        {
            if (!string.IsNullOrEmpty(whereString))
            {
                whereString = StringUtils.ReplaceStartsWith(whereString.Trim(), "AND", string.Empty);
                if (!StringUtils.StartsWithIgnoreCase(whereString, "WHERE "))
                {
                    whereString = "WHERE " + whereString;
                }
            }

            if (!string.IsNullOrEmpty(joinString))
            {
                whereString = joinString + " " + whereString;
            }

            return SqlUtils.ToTopSqlString(tableName, columns, whereString, orderByString, totalNum);
        }

        public int GetCount(string tableName)
        {
            int count;

            using (var conn = WebConfigUtils.Database.GetConnection())
            {
                count = conn.ExecuteScalar<int>($"SELECT COUNT(*) FROM {SqlUtils.GetQuotedIdentifier(tableName)}");
            }
            return count;

            //return GetIntResult();
        }

        public IEnumerable<dynamic> GetObjects(string tableName)
        {
            IEnumerable<dynamic> objects;
            var sqlString = $"select * from {tableName}";

            using (var connection = WebConfigUtils.Database.GetConnection())
            {
                objects = connection.Query(sqlString, null, null, false).ToList();
            }

            return objects;
        }

        public IEnumerable<dynamic> GetPageObjects(string tableName, string identityColumnName, int offset, int limit)
        {
            IEnumerable<dynamic> objects;
            var sqlString = GetPageSqlString(tableName, "*", string.Empty, $"ORDER BY {identityColumnName} ASC", offset, limit);

            using (var connection = WebConfigUtils.Database.GetConnection())
            {
                objects = connection.Query(sqlString, null, null, false).ToList();
            }

            return objects;
        }

        private decimal? _sqlServerVersion;

        private decimal SqlServerVersion
        {
            get
            {
                if (WebConfigUtils.DatabaseType != DatabaseType.SqlServer)
                {
                    return 0;
                }

                if (_sqlServerVersion == null)
                {
                    try
                    {
                        _sqlServerVersion =
                            TranslateUtils.ToDecimal(
                                GetString("select left(cast(serverproperty('productversion') as varchar), 4)"));
                    }
                    catch
                    {
                        _sqlServerVersion = 0;
                    }
                }

                return _sqlServerVersion.Value;
            }
        }

        private bool IsSqlServer2012 => SqlServerVersion >= 11;

        public string GetPageSqlString(string tableName, string columnNames, string whereSqlString, string orderSqlString, int offset, int limit)
        {
            var retVal = string.Empty;

            if (string.IsNullOrEmpty(orderSqlString))
            {
                orderSqlString = "ORDER BY Id DESC";
            }

            if (offset == 0 && limit == 0)
            {
                return $@"SELECT {columnNames} FROM {tableName} {whereSqlString} {orderSqlString}";
            }

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                if (limit == 0)
                {
                    limit = int.MaxValue;
                }
                retVal = $@"SELECT {columnNames} FROM {tableName} {whereSqlString} {orderSqlString} LIMIT {limit} OFFSET {offset}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer && IsSqlServer2012)
            {
                retVal = limit == 0
                    ? $"SELECT {columnNames} FROM {tableName} {whereSqlString} {orderSqlString} OFFSET {offset} ROWS"
                    : $"SELECT {columnNames} FROM {tableName} {whereSqlString} {orderSqlString} OFFSET {offset} ROWS FETCH NEXT {limit} ROWS ONLY";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer && !IsSqlServer2012)
            {
                if (offset == 0)
                {
                    retVal = $"SELECT TOP {limit} {columnNames} FROM {tableName} {whereSqlString} {orderSqlString}";
                }
                else
                {
                    var rowWhere = limit == 0
                        ? $@"WHERE [row_num] > {offset}"
                        : $@"WHERE [row_num] BETWEEN {offset + 1} AND {offset + limit}";

                    retVal = $@"SELECT * FROM (
    SELECT {columnNames}, ROW_NUMBER() OVER ({orderSqlString}) AS [row_num] FROM [{tableName}] {whereSqlString}
) as T {rowWhere}";
                }
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                retVal = limit == 0
                    ? $@"SELECT {columnNames} FROM {tableName} {whereSqlString} {orderSqlString} OFFSET {offset}"
                    : $@"SELECT {columnNames} FROM {tableName} {whereSqlString} {orderSqlString} LIMIT {limit} OFFSET {offset}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                retVal = limit == 0
                    ? $"SELECT {columnNames} FROM {tableName} {whereSqlString} {orderSqlString} OFFSET {offset} ROWS"
                    : $"SELECT {columnNames} FROM {tableName} {whereSqlString} {orderSqlString} OFFSET {offset} ROWS FETCH NEXT {limit} ROWS ONLY";
            }

            return retVal;
        }
    }
}

