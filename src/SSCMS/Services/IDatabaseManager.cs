using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Enums;
using SSCMS.Repositories;

namespace SSCMS.Services
{
    public partial interface IDatabaseManager
    {
        IAccessTokenRepository AccessTokenRepository { get; }
        IAdministratorRepository AdministratorRepository { get; }
        IAdministratorsInRolesRepository AdministratorsInRolesRepository { get; }
        IChannelRepository ChannelRepository { get; }
        IChannelGroupRepository ChannelGroupRepository { get; }
        IConfigRepository ConfigRepository { get; }
        IContentCheckRepository ContentCheckRepository { get; }
        IContentGroupRepository ContentGroupRepository { get; }
        IContentRepository ContentRepository { get; }
        IContentTagRepository ContentTagRepository { get; }
        IDbCacheRepository DbCacheRepository { get; }
        IErrorLogRepository ErrorLogRepository { get; }
        IMaterialAudioRepository MaterialAudioRepository { get; }
        IMaterialArticleRepository MaterialArticleRepository { get; }
        IMaterialFileRepository MaterialFileRepository { get; }
        IMaterialGroupRepository MaterialGroupRepository { get; }
        IMaterialImageRepository MaterialImageRepository { get; }
        IMaterialVideoRepository MaterialVideoRepository { get; }
        ILogRepository LogRepository { get; }
        IPermissionsInRolesRepository PermissionsInRolesRepository { get; }
        IPluginConfigRepository PluginConfigRepository { get; }
        IRelatedFieldItemRepository RelatedFieldItemRepository { get; }
        IRelatedFieldRepository RelatedFieldRepository { get; }
        IRoleRepository RoleRepository { get; }
        ISiteLogRepository SiteLogRepository { get; }
        ISitePermissionsRepository SitePermissionsRepository { get; }
        ISiteRepository SiteRepository { get; }
        ISpecialRepository SpecialRepository { get; }
        IStatRepository StatRepository { get; }
        ITableStyleRepository TableStyleRepository { get; }
        ITemplateLogRepository TemplateLogRepository { get; }
        ITemplateRepository TemplateRepository { get; }
        ITranslateRepository TranslateRepository { get; }
        IUserGroupRepository UserGroupRepository { get; }
        IUserMenuRepository UserMenuRepository { get; }
        IUserRepository UserRepository { get; }

        List<IRepository> GetAllRepositories();

        Database GetDatabase(string connectionString = null);

        int GetIntResult(string connectionString, string sqlString);

        int GetIntResult(string sqlString);

        string GetString(string connectionString, string sqlString);

        IEnumerable<IDictionary<string, object>> GetRows(DatabaseType databaseType, string connectionString, string sqlString);

        int GetPageTotalCount(string sqlString);

        string GetStlPageSqlString(string sqlString, string orderString, int totalCount, int itemsPerPage,
            int currentPageIndex);

        string GetSelectSqlString(string tableName, int totalNum, string columns, string whereString,
            string orderByString);

        int GetCount(string tableName);

        Task<List<IDictionary<string, object>>> GetObjectsAsync(string tableName);

        Task<List<IDictionary<string, object>>> GetPageObjectsAsync(string tableName, string identityColumnName, int offset, int limit);

        string GetPageSqlString(string tableName, string columnNames, string whereSqlString, string orderSqlString,
            int offset, int limit);

        string GetContentOrderByString(TaxisType taxisType);

        string GetContentOrderByString(TaxisType taxisType, string orderByString);

        string GetDatabaseNameFormConnectionString(string connectionString);
    }
}