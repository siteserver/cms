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
                AttributeName = nameof(UserMenuInfo.Title),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(UserMenuInfo.Url),
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(UserMenuInfo.IconClass),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(UserMenuInfo.IsOpenWindow),
                DataType = DataType.Boolean
            }
        };

        public int Insert(UserMenuInfo menuInfo)
        {
            int menuId;

            using (var connection = GetConnection())
            {
                menuId = (int)connection.Insert(menuInfo);
            }

            UserMenuManager.ClearCache();

            return menuId;
        }

        public void Update(UserMenuInfo menuInfo)
        {
            using (var connection = GetConnection())
            {
                connection.Update(menuInfo);
            }

            UserMenuManager.ClearCache();
        }

        public void Delete(int menuId)
        {
            using (var connection = GetConnection())
            {
                connection.Delete(new UserMenuInfo { Id = menuId });
                connection.Delete(new UserMenuInfo { ParentId = menuId });
            }

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
