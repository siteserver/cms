using System.Collections.Generic;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;

namespace SiteServer.CMS.Database.Repositories
{
    public class UserGroupRepository : GenericRepository<UserGroupInfo>
    {
        private static class Attr
        {
            public const string Id = nameof(UserGroupInfo.Id);
        }

        public void Insert(UserGroupInfo groupInfo)
        {
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

            InsertObject(groupInfo);

            UserGroupManager.ClearCache();
        }

        public void Update(UserGroupInfo groupInfo)
        {
            //var sqlString = $@"UPDATE {TableName} SET
            //    {nameof(UserGroupInfo.GroupName)} = @{nameof(UserGroupInfo.GroupName)},  
            //    {nameof(UserGroupInfo.AdminName)} = @{nameof(UserGroupInfo.AdminName)}
            //WHERE {nameof(UserGroupInfo.Id)} = @{nameof(UserGroupInfo.Id)}";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(nameof(UserGroupInfo.GroupName), groupInfo.GroupName),
            //    GetParameter(nameof(UserGroupInfo.AdminName), groupInfo.AdminName),
            //    GetParameter(nameof(UserGroupInfo.Id), groupInfo.Id)
            //};

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

            UpdateObject(groupInfo);

            UserGroupManager.ClearCache();
        }

        public void Delete(int groupId)
        {
            //var sqlString = $"DELETE FROM {TableName} WHERE Id = @Id";

            //IDataParameter[] parameters =
            //{
            //    GetParameter("@Id", groupId)
            //};

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

            DeleteById(groupId);

            UserGroupManager.ClearCache();
        }

        public IList<UserGroupInfo> GetUserGroupInfoList()
        {
            //List<UserGroupInfo> list;

            //var sqlString = $"SELECT * FROM {TableName} ORDER BY Id";
            //using (var connection = GetConnection())
            //{
            //    list = connection.Query<UserGroupInfo>(sqlString).ToList();
            //}

            return GetObjectList(Q.OrderBy(Attr.Id));
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
//    public class UserGroup : DataProviderBase
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
