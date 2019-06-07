using System.Collections.Generic;
using System.Linq;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Common;
using SS.CMS.Core.Models;
using SS.CMS.Data;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public class UserGroupDao : IDatabaseDao
    {
        private readonly Repository<UserGroupInfo> _repository;
        public UserGroupDao()
        {
            _repository = new Repository<UserGroupInfo>(AppSettings.DbContext);
        }

        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string Id = nameof(UserGroupInfo.Id);
        }

        public int Insert(UserGroupInfo groupInfo)
        {
            groupInfo.Id = _repository.Insert(groupInfo);

            UserGroupManager.ClearCache();

            return groupInfo.Id;
        }

        public bool Update(UserGroupInfo groupInfo)
        {
            var updated = _repository.Update(groupInfo);

            UserGroupManager.ClearCache();

            return updated;
        }

        public bool Delete(int groupId)
        {
            var deleted = _repository.Delete(groupId);

            UserGroupManager.ClearCache();

            return deleted;
        }

        public IList<UserGroupInfo> GetUserGroupInfoList()
        {
            return _repository.GetAll(Q.OrderBy(Attr.Id)).ToList();
        }
    }
}


//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using Dapper;
//using SiteServer.CMS.Database.Caches;
//using SiteServer.CMS.Database.Core;
//using SiteServer.CMS.Database.Models;
//using SiteServer.Plugin;

//namespace SiteServer.CMS.Database.Repositories
//{
//    public class UserGroup
//    {
//        public const string DatabaseTableName = "siteserver_UserGroup";

//        public override string TableName => DatabaseTableName;

//        public override List<TableColumn> TableColumns => new List<TableColumn>
//        {
//            new TableColumn
//            {
//                AttributeName = nameof(UserGroupInfo.Id),
//                DataType = DataType.Integer,
//                IsPrimaryKey = true,
//                IsIdentity = true
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(UserGroupInfo.GroupName),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(UserGroupInfo.AdminName),
//                DataType = DataType.VarChar
//            }
//        };

//        public int InsertObject(UserGroupInfo groupInfo)
//        {
//            var sqlString =
//                $@"
//INSERT INTO {TableName} (
//    {nameof(UserGroupInfo.GroupName)},
//    {nameof(UserGroupInfo.AdminName)}
//) VALUES (
//    @{nameof(UserGroupInfo.GroupName)},
//    @{nameof(UserGroupInfo.AdminName)}
//)";

//            IDataParameter[] parameters =
//            {
//                GetParameter($"@{nameof(UserGroupInfo.GroupName)}", groupInfo.GroupName),
//                GetParameter($"@{nameof(UserGroupInfo.AdminName)}", groupInfo.AdminName)
//            };

//            var groupId = DatabaseApi.ExecuteNonQueryAndReturnId(ConnectionString, TableName, nameof(UserGroupInfo.Id), sqlString, parameters);

//            UserGroupManager.ClearCache();

//            return groupId;
//        }

//        public void UpdateObject(UserGroupInfo groupInfo)
//        {
//            var sqlString = $@"UPDATE {TableName} SET
//                {nameof(UserGroupInfo.GroupName)} = @{nameof(UserGroupInfo.GroupName)},  
//                {nameof(UserGroupInfo.AdminName)} = @{nameof(UserGroupInfo.AdminName)}
//            WHERE {nameof(UserGroupInfo.Id)} = @{nameof(UserGroupInfo.Id)}";

//            IDataParameter[] parameters =
//            {
//                GetParameter(nameof(UserGroupInfo.GroupName), groupInfo.GroupName),
//                GetParameter(nameof(UserGroupInfo.AdminName), groupInfo.AdminName),
//                GetParameter(nameof(UserGroupInfo.Id), groupInfo.Id)
//            };

//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

//            UserGroupManager.ClearCache();
//        }

//        public void DeleteById(int groupId)
//        {
//            var sqlString = $"DELETE FROM {TableName} WHERE Id = @Id";

//            IDataParameter[] parameters =
//            {
//                GetParameter("@Id", groupId)
//            };

//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

//            UserGroupManager.ClearCache();
//        }

//        public List<UserGroupInfo> GetUserGroupInfoList()
//        {
//            List<UserGroupInfo> list;

//            var sqlString = $"SELECT * FROM {TableName} ORDER BY Id";
//            using (var connection = GetConnection())
//            {
//                list = connection.Query<UserGroupInfo>(sqlString).ToList();
//            }

//            list.InsertObject(0, new UserGroupInfo
//            {
//                Id = 0,
//                GroupName = "默认用户组",
//                AdminName = ConfigManager.Instance.UserDefaultGroupAdminName
//            });

//            return list;
//        }
//    }
//}
