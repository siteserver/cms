using System.Collections.Generic;
using System.Linq;
using Dapper;
using Dapper.Contrib.Extensions;
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
            int groupId;

            using (var connection = GetConnection())
            {
                groupId = (int)connection.Insert(groupInfo);
            }

            UserGroupManager.ClearCache();

            return groupId;
        }

        public void Update(UserGroupInfo groupInfo)
        {
            using (var connection = GetConnection())
            {
                connection.Update(groupInfo);
            }

            UserGroupManager.ClearCache();
        }

        public void Delete(int groupId)
        {
            using (var connection = GetConnection())
            {
                connection.Delete(new UserGroupInfo { Id = groupId });
            }

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
