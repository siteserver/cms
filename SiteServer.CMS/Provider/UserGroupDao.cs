using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using SiteServer.CMS.Data;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Plugin;

namespace SiteServer.CMS.Provider
{
    public class UserGroupDao : DataProviderBase
    {
        public const string DatabaseTableName = "siteserver_UserGroup";

        public override string TableName => DatabaseTableName;

        public override List<TableColumn> TableColumns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(UserGroupInfo.Id),
                DataType = DataType.Integer,
                IsPrimaryKey = true,
                IsIdentity = true
            },
            new TableColumn
            {
                AttributeName = nameof(UserGroupInfo.GroupName),
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(UserGroupInfo.AdminName),
                DataType = DataType.VarChar,
                DataLength = 200
            }
        };

        public int Insert(UserGroupInfo groupInfo)
        {
            var sqlString =
                $@"
INSERT INTO {TableName} (
    {nameof(UserGroupInfo.GroupName)},
    {nameof(UserGroupInfo.AdminName)}
) VALUES (
    @{nameof(UserGroupInfo.GroupName)},
    @{nameof(UserGroupInfo.AdminName)}
)";

            var parms = new IDataParameter[]
            {
                GetParameter($"@{nameof(UserGroupInfo.GroupName)}", DataType.VarChar, 200, groupInfo.GroupName),
                GetParameter($"@{nameof(UserGroupInfo.AdminName)}", DataType.VarChar, 200, groupInfo.AdminName)
            };

            var groupId = ExecuteNonQueryAndReturnId(TableName, nameof(UserGroupInfo.Id), sqlString, parms);

            UserGroupManager.ClearCache();

            return groupId;
        }

        public void Update(UserGroupInfo groupInfo)
        {
            var sqlString = $@"UPDATE {TableName} SET
                {nameof(UserGroupInfo.GroupName)} = @{nameof(UserGroupInfo.GroupName)},  
                {nameof(UserGroupInfo.AdminName)} = @{nameof(UserGroupInfo.AdminName)}
            WHERE {nameof(UserGroupInfo.Id)} = @{nameof(UserGroupInfo.Id)}";

            IDataParameter[] parameters =
            {
                GetParameter(nameof(UserGroupInfo.GroupName), DataType.VarChar, 200, groupInfo.GroupName),
                GetParameter(nameof(UserGroupInfo.AdminName), DataType.VarChar, 200, groupInfo.AdminName),
                GetParameter(nameof(UserGroupInfo.Id), DataType.Integer, groupInfo.Id)
            };

            ExecuteNonQuery(sqlString, parameters);

            UserGroupManager.ClearCache();
        }

        public void Delete(int groupId)
        {
            var sqlString = $"DELETE FROM {TableName} WHERE Id = @Id";

            var parms = new IDataParameter[]
            {
                GetParameter("@Id", DataType.Integer, groupId)
            };

            ExecuteNonQuery(sqlString, parms);

            UserGroupManager.ClearCache();
        }

        public List<UserGroupInfo> GetUserGroupInfoList()
        {
            List<UserGroupInfo> list;

            var sqlString = $"SELECT * FROM {TableName} ORDER BY Id";
            using (var connection = GetConnection())
            {
                list = connection.Query<UserGroupInfo>(sqlString).ToList();
            }

            list.Insert(0, new UserGroupInfo
            {
                Id = 0,
                GroupName = "默认用户组",
                AdminName = ConfigManager.SystemConfigInfo.UserDefaultGroupAdminName
            });

            return list;
        }
    }
}
