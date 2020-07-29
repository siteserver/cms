using System.Collections.Generic;
using Datory;
using SSCMS.Repositories;

namespace SSCMS.Services
{
    public partial interface IDatabaseManager
    {
        IAccessTokenRepository AccessTokenRepository { get; }
        IAdministratorRepository AdministratorRepository { get; }
        IAdministratorsInRolesRepository AdministratorsInRolesRepository { get; }
        IChannelGroupRepository ChannelGroupRepository { get; }
        IChannelRepository ChannelRepository { get; }
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
        IUserGroupRepository UserGroupRepository { get; }
        IUserMenuRepository UserMenuRepository { get; }
        IUserRepository UserRepository { get; }

        List<IRepository> GetAllRepositories();

        Database GetDatabase(string connectionString = null);

        int GetIntResult(string connectionString, string sqlString);

        int GetIntResult(string sqlString);

        string GetString(string connectionString, string sqlString);

        IEnumerable<IDictionary<string, object>> GetRows(string connectionString, string sqlString);

        int GetPageTotalCount(string sqlString);

        string GetStlPageSqlString(string sqlString, string orderString, int totalCount, int itemsPerPage,
            int currentPageIndex);

        string GetSelectSqlString(string tableName, int totalNum, string columns, string whereString,
            string orderByString);

        int GetCount(string tableName);

        IEnumerable<dynamic> GetObjects(string tableName);

        IEnumerable<dynamic> GetPageObjects(string tableName, string identityColumnName, int offset, int limit);

        string GetPageSqlString(string tableName, string columnNames, string whereSqlString, string orderSqlString,
            int offset, int limit);
    }
}