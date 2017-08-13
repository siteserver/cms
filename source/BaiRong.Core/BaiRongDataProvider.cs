using BaiRong.Core.Provider;

namespace BaiRong.Core
{
    public class BaiRongDataProvider
    {
        private static AdministratorDao _administratorDao;
        public static AdministratorDao AdministratorDao => _administratorDao ?? (_administratorDao = new AdministratorDao());

        private static AreaDao _areaDao;
        public static AreaDao AreaDao => _areaDao ?? (_areaDao = new AreaDao());

        private static AuxiliaryTableDataDao _auxiliaryTableDataDao;
        public static AuxiliaryTableDataDao AuxiliaryTableDataDao => _auxiliaryTableDataDao ?? (_auxiliaryTableDataDao = new AuxiliaryTableDataDao());

        private static ConfigDao _configDao;
        public static ConfigDao ConfigDao => _configDao ?? (_configDao = new ConfigDao());

        private static ContentCheckDao _contentCheckDao;
        public static ContentCheckDao ContentCheckDao => _contentCheckDao ?? (_contentCheckDao = new ContentCheckDao());

        private static ContentDao _contentDao;
        public static ContentDao ContentDao => _contentDao ?? (_contentDao = new ContentDao());

        private static CountDao _countDao;
        internal static CountDao CountDao => _countDao ?? (_countDao = new CountDao());

        private static DatabaseDao _databaseDao;
        public static DatabaseDao DatabaseDao => _databaseDao ?? (_databaseDao = new DatabaseDao());

        private static DbCacheDao _dbCacheDao;
        internal static DbCacheDao DbCacheDao => _dbCacheDao ?? (_dbCacheDao = new DbCacheDao());

        private static DepartmentDao _departmentDao;
        public static DepartmentDao DepartmentDao => _departmentDao ?? (_departmentDao = new DepartmentDao());

        private static DiggDao _diggDao;
        public static DiggDao DiggDao => _diggDao ?? (_diggDao = new DiggDao());

        private static ErrorLogDao _errorLogDao;
        public static ErrorLogDao ErrorLogDao => _errorLogDao ?? (_errorLogDao = new ErrorLogDao());

        private static LogDao _logDao;
        public static LogDao LogDao => _logDao ?? (_logDao = new LogDao());

        private static PermissionsInRolesDao _permissionsInRolesDao;
        public static PermissionsInRolesDao PermissionsInRolesDao => _permissionsInRolesDao ?? (_permissionsInRolesDao = new PermissionsInRolesDao());

        private static RecordDao _recordDao;
        public static RecordDao RecordDao => _recordDao ?? (_recordDao = new RecordDao());

        private static RoleDao _roleDao;
        public static RoleDao RoleDao => _roleDao ?? (_roleDao = new RoleDao());

        private static TableCollectionDao _tableCollectionDao;
        public static TableCollectionDao TableCollectionDao => _tableCollectionDao ?? (_tableCollectionDao = new TableCollectionDao());

        private static TableMatchDao _tableMatchDao;
        public static TableMatchDao TableMatchDao => _tableMatchDao ?? (_tableMatchDao = new TableMatchDao());

        private static TableMetadataDao _tableMetadataDao;
        public static TableMetadataDao TableMetadataDao => _tableMetadataDao ?? (_tableMetadataDao = new TableMetadataDao());

        private static TableStructureDao _tableStructureDao;
        public static TableStructureDao TableStructureDao => _tableStructureDao ?? (_tableStructureDao = new TableStructureDao());

        private static TableStyleDao _tableStyleDao;
        public static TableStyleDao TableStyleDao => _tableStyleDao ?? (_tableStyleDao = new TableStyleDao());

        private static TagDao _tagDao;
        public static TagDao TagDao => _tagDao ?? (_tagDao = new TagDao());

        private static UserDao _userDao;
        public static UserDao UserDao => _userDao ?? (_userDao = new UserDao());

        private static UserGroupDao _userGroupDao;
        public static UserGroupDao UserGroupDao => _userGroupDao ?? (_userGroupDao = new UserGroupDao());

        private static UserLogDao _userLogDao;
        public static UserLogDao UserLogDao => _userLogDao ?? (_userLogDao = new UserLogDao());
    }
}
