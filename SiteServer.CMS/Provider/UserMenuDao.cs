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
    public class UserMenuDao : DataProviderBase
    {
        public const string DatabaseTableName = "siteserver_UserMenu";

        public override string TableName => DatabaseTableName;

        public override List<TableColumn> TableColumns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(UserMenuInfo.Id),
                DataType = DataType.Integer,
                IsPrimaryKey = true,
                IsIdentity = true
            },
            new TableColumn
            {
                AttributeName = nameof(UserMenuInfo.SystemId),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(UserMenuInfo.GroupIdCollection),
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(UserMenuInfo.IsDisabled),
                DataType = DataType.Boolean
            },
            new TableColumn
            {
                AttributeName = nameof(UserMenuInfo.ParentId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(UserMenuInfo.Taxis),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(UserMenuInfo.Text),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(UserMenuInfo.IconClass),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(UserMenuInfo.Href),
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(UserMenuInfo.Target),
                DataType = DataType.VarChar,
                DataLength = 50
            }
        };

        public int Insert(UserMenuInfo menuInfo)
        {
            var sqlString =
                $@"
INSERT INTO {TableName} (
    {nameof(UserMenuInfo.SystemId)}, 
    {nameof(UserMenuInfo.GroupIdCollection)}, 
    {nameof(UserMenuInfo.IsDisabled)}, 
    {nameof(UserMenuInfo.ParentId)}, 
    {nameof(UserMenuInfo.Taxis)}, 
    {nameof(UserMenuInfo.Text)}, 
    {nameof(UserMenuInfo.IconClass)}, 
    {nameof(UserMenuInfo.Href)}, 
    {nameof(UserMenuInfo.Target)}
) VALUES (
    @{nameof(UserMenuInfo.SystemId)}, 
    @{nameof(UserMenuInfo.GroupIdCollection)}, 
    @{nameof(UserMenuInfo.IsDisabled)}, 
    @{nameof(UserMenuInfo.ParentId)}, 
    @{nameof(UserMenuInfo.Taxis)}, 
    @{nameof(UserMenuInfo.Text)}, 
    @{nameof(UserMenuInfo.IconClass)}, 
    @{nameof(UserMenuInfo.Href)}, 
    @{nameof(UserMenuInfo.Target)}
)";

            var parms = new IDataParameter[]
            {
                GetParameter($"@{nameof(UserMenuInfo.SystemId)}", DataType.VarChar, 50, menuInfo.SystemId),
                GetParameter($"@{nameof(UserMenuInfo.GroupIdCollection)}", DataType.VarChar, 200, menuInfo.GroupIdCollection),
                GetParameter($"@{nameof(UserMenuInfo.IsDisabled)}", DataType.Boolean, menuInfo.IsDisabled),
                GetParameter($"@{nameof(UserMenuInfo.ParentId)}", DataType.Integer, menuInfo.ParentId),
                GetParameter($"@{nameof(UserMenuInfo.Taxis)}", DataType.Integer, menuInfo.Taxis),
                GetParameter($"@{nameof(UserMenuInfo.Text)}", DataType.VarChar, 50, menuInfo.Text),
                GetParameter($"@{nameof(UserMenuInfo.IconClass)}", DataType.VarChar, 50, menuInfo.IconClass),
                GetParameter($"@{nameof(UserMenuInfo.Href)}", DataType.VarChar, 200, menuInfo.Href),
                GetParameter($"@{nameof(UserMenuInfo.Target)}", DataType.VarChar, 50, menuInfo.Target)
            };

            var menuId = ExecuteNonQueryAndReturnId(TableName, nameof(UserMenuInfo.Id), sqlString, parms);

            UserMenuManager.ClearCache();

            return menuId;
        }

        public void Update(UserMenuInfo menuInfo)
        {
            var sqlString = $@"UPDATE {TableName} SET
                {nameof(UserMenuInfo.SystemId)} = @{nameof(UserMenuInfo.SystemId)}, 
                {nameof(UserMenuInfo.GroupIdCollection)} = @{nameof(UserMenuInfo.GroupIdCollection)}, 
                {nameof(UserMenuInfo.IsDisabled)} = @{nameof(UserMenuInfo.IsDisabled)}, 
                {nameof(UserMenuInfo.ParentId)} = @{nameof(UserMenuInfo.ParentId)}, 
                {nameof(UserMenuInfo.Taxis)} = @{nameof(UserMenuInfo.Taxis)}, 
                {nameof(UserMenuInfo.Text)} = @{nameof(UserMenuInfo.Text)}, 
                {nameof(UserMenuInfo.IconClass)} = @{nameof(UserMenuInfo.IconClass)}, 
                {nameof(UserMenuInfo.Href)} = @{nameof(UserMenuInfo.Href)}, 
                {nameof(UserMenuInfo.Target)} = @{nameof(UserMenuInfo.Target)}
            WHERE {nameof(UserMenuInfo.Id)} = @{nameof(UserMenuInfo.Id)}";

            IDataParameter[] parameters =
            {
                GetParameter(nameof(UserMenuInfo.SystemId), DataType.VarChar, 50, menuInfo.SystemId),
                GetParameter(nameof(UserMenuInfo.GroupIdCollection), DataType.VarChar, 200, menuInfo.GroupIdCollection),
                GetParameter(nameof(UserMenuInfo.IsDisabled), DataType.Boolean, menuInfo.IsDisabled),
                GetParameter(nameof(UserMenuInfo.ParentId), DataType.Integer, menuInfo.ParentId),
                GetParameter(nameof(UserMenuInfo.Taxis), DataType.Integer, menuInfo.Taxis),
                GetParameter(nameof(UserMenuInfo.Text), DataType.VarChar, 50, menuInfo.Text),
                GetParameter(nameof(UserMenuInfo.IconClass), DataType.VarChar, 50, menuInfo.IconClass),
                GetParameter(nameof(UserMenuInfo.Href), DataType.VarChar, 200, menuInfo.Href),
                GetParameter(nameof(UserMenuInfo.Target), DataType.VarChar, 50, menuInfo.Target),
                GetParameter(nameof(UserMenuInfo.Id), DataType.Integer, menuInfo.Id)
            };

            ExecuteNonQuery(sqlString, parameters);

            UserMenuManager.ClearCache();
        }

        public void Delete(int menuId)
        {
            var sqlString = $"DELETE FROM {TableName} WHERE {nameof(UserMenuInfo.Id)} = @{nameof(UserMenuInfo.Id)} OR {nameof(UserMenuInfo.ParentId)} = @{nameof(UserMenuInfo.ParentId)}";

            var parms = new IDataParameter[]
            {
                GetParameter($"@{nameof(UserMenuInfo.Id)}", DataType.Integer, menuId),
                GetParameter($"@{nameof(UserMenuInfo.ParentId)}", DataType.Integer, menuId)
            };

            ExecuteNonQuery(sqlString, parms);

            UserMenuManager.ClearCache();
        }

        public List<UserMenuInfo> GetUserMenuInfoList()
        {
            List<UserMenuInfo> list;

            var sqlString = $"SELECT * FROM {TableName}";
            using (var connection = GetConnection())
            {
                list = connection.Query<UserMenuInfo>(sqlString).ToList();
            }

            var systemMenus = UserMenuManager.SystemMenus.Value;
            foreach (var kvp in systemMenus)
            {
                var parent = kvp.Key;
                var children = kvp.Value;

                if (list.All(x => x.SystemId != parent.SystemId))
                {
                    parent.Id = Insert(parent);
                    list.Add(parent);
                }
                else
                {
                    parent = list.First(x => x.SystemId == parent.SystemId);
                }

                if (children != null)
                {
                    foreach (var child in children)
                    {
                        if (list.All(x => x.SystemId != child.SystemId))
                        {
                            child.ParentId = parent.Id;
                            child.Id = Insert(child);
                            list.Add(child);
                        }
                    }
                }
            }

            return list.OrderBy(menuInfo => menuInfo.Taxis == 0 ? int.MaxValue : menuInfo.Taxis).ToList();
        }
    }
}
