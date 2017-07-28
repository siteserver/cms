using BaiRong.Core.Provider;

namespace BaiRong.Core
{
    public class BaiRongDataProvider
    {
        private static AdministratorDao _administratorDao;
        private static AreaDao _areaDao;
        private static AuxiliaryTableDataDao _auxiliaryTableDataDao;
        private static ThirdLoginDao _thirdLoginDao;
        private static ConfigDao _configDao;

        private static TableCollectionDao _tableCollectionDao;
        private static TableMetadataDao _tableMetadataDao;
        private static DatabaseDao _databaseDao;
        private static TableStructureDao _tableStructureDao;
        private static CountDao _countDao;
        private static DbCacheDao _dbCacheDao;
        private static TableMatchDao _tableMatchDao;
        private static TableStyleDao _tableStyleDao;
        private static DiggDao _diggDao;
        private static LogDao _logDao;
        private static ErrorLogDao _errorLogDao;
        private static ContentModelDao _contentModelDao;
        private static ContentDao _contentDao;
        private static TagDao _tagDao;
        private static ContentCheckDao _contentCheckDao;
        private static DepartmentDao _departmentDao;
        
        private static RoleDao _roleDao;
        private static PermissionsInRolesDao _permissionsInRolesDao;
        
        private static UserDao _userDao;
        private static UserBindingDao _userBindingDao;
        private static UserGroupDao _userGroupDao;
        private static UserLogDao _userLogDao;

        public static AreaDao AreaDao => _areaDao ?? (_areaDao = new AreaDao());

        public static AdministratorDao AdministratorDao => _administratorDao ?? (_administratorDao = new AdministratorDao());

        public static AuxiliaryTableDataDao AuxiliaryTableDataDao => _auxiliaryTableDataDao ?? (_auxiliaryTableDataDao = new AuxiliaryTableDataDao());

        public static ThirdLoginDao ThirdLoginDao => _thirdLoginDao ?? (_thirdLoginDao = new ThirdLoginDao());

        public static ConfigDao ConfigDao => _configDao ?? (_configDao = new ConfigDao());

        public static TableCollectionDao TableCollectionDao => _tableCollectionDao ?? (_tableCollectionDao = new TableCollectionDao());

        public static TableMetadataDao TableMetadataDao => _tableMetadataDao ?? (_tableMetadataDao = new TableMetadataDao());

        public static DatabaseDao DatabaseDao => _databaseDao ?? (_databaseDao = new DatabaseDao());

        public static TableStructureDao TableStructureDao => _tableStructureDao ?? (_tableStructureDao = new TableStructureDao());

        internal static CountDao CountDao => _countDao ?? (_countDao = new CountDao());

        internal static DbCacheDao DbCacheDao => _dbCacheDao ?? (_dbCacheDao = new DbCacheDao());

        public static TableMatchDao TableMatchDao => _tableMatchDao ?? (_tableMatchDao = new TableMatchDao());

        public static TableStyleDao TableStyleDao => _tableStyleDao ?? (_tableStyleDao = new TableStyleDao());

        public static DiggDao DiggDao => _diggDao ?? (_diggDao = new DiggDao());

        public static LogDao LogDao => _logDao ?? (_logDao = new LogDao());

        public static ErrorLogDao ErrorLogDao => _errorLogDao ?? (_errorLogDao = new ErrorLogDao());

        public static ContentModelDao ContentModelDao => _contentModelDao ?? (_contentModelDao = new ContentModelDao());

        public static ContentDao ContentDao => _contentDao ?? (_contentDao = new ContentDao());

        public static TagDao TagDao => _tagDao ?? (_tagDao = new TagDao());

        public static ContentCheckDao ContentCheckDao => _contentCheckDao ?? (_contentCheckDao = new ContentCheckDao());

        public static DepartmentDao DepartmentDao => _departmentDao ?? (_departmentDao = new DepartmentDao());

        public static RoleDao RoleDao => _roleDao ?? (_roleDao = new RoleDao());

        public static PermissionsInRolesDao PermissionsInRolesDao => _permissionsInRolesDao ?? (_permissionsInRolesDao = new PermissionsInRolesDao());

        public static UserDao UserDao => _userDao ?? (_userDao = new UserDao());

        public static UserBindingDao UserBindingDao => _userBindingDao ?? (_userBindingDao = new UserBindingDao());

        public static UserGroupDao UserGroupDao => _userGroupDao ?? (_userGroupDao = new UserGroupDao());

        public static UserLogDao UserLogDao => _userLogDao ?? (_userLogDao = new UserLogDao());
    }
}
