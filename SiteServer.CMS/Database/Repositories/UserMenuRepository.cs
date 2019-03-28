using System.Collections.Generic;
using System.Linq;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;

namespace SiteServer.CMS.Database.Repositories
{
    public class UserMenuRepository : GenericRepository<UserMenuInfo>
    {
        private static class Attr
        {
            public const string Id = nameof(UserMenuInfo.Id);
            public const string ParentId = nameof(UserMenuInfo.ParentId);
        }

        public int Insert(UserMenuInfo menuInfo)
        {
//            var sqlString =
//                $@"
//INSERT INTO {TableName} (
//    {nameof(UserMenuInfo.SystemId)}, 
//    {nameof(UserMenuInfo.GroupIdCollection)}, 
//    {nameof(UserMenuInfo.IsDisabled)}, 
//    {nameof(UserMenuInfo.ParentId)}, 
//    {nameof(UserMenuInfo.Taxis)}, 
//    {nameof(UserMenuInfo.Text)}, 
//    {nameof(UserMenuInfo.IconClass)}, 
//    {nameof(UserMenuInfo.Href)}, 
//    {nameof(UserMenuInfo.Target)}
//) VALUES (
//    @{nameof(UserMenuInfo.SystemId)}, 
//    @{nameof(UserMenuInfo.GroupIdCollection)}, 
//    @{nameof(UserMenuInfo.IsDisabled)}, 
//    @{nameof(UserMenuInfo.ParentId)}, 
//    @{nameof(UserMenuInfo.Taxis)}, 
//    @{nameof(UserMenuInfo.Text)}, 
//    @{nameof(UserMenuInfo.IconClass)}, 
//    @{nameof(UserMenuInfo.Href)}, 
//    @{nameof(UserMenuInfo.Target)}
//)";

//            IDataParameter[] parameters =
//            {
//                GetParameter($"@{nameof(UserMenuInfo.SystemId)}", menuInfo.SystemId),
//                GetParameter($"@{nameof(UserMenuInfo.GroupIdCollection)}", menuInfo.GroupIdCollection),
//                GetParameter($"@{nameof(UserMenuInfo.IsDisabled)}", menuInfo.IsDisabled),
//                GetParameter($"@{nameof(UserMenuInfo.ParentId)}", menuInfo.ParentId),
//                GetParameter($"@{nameof(UserMenuInfo.Taxis)}", menuInfo.Taxis),
//                GetParameter($"@{nameof(UserMenuInfo.Text)}", menuInfo.Text),
//                GetParameter($"@{nameof(UserMenuInfo.IconClass)}", menuInfo.IconClass),
//                GetParameter($"@{nameof(UserMenuInfo.Href)}", menuInfo.Href),
//                GetParameter($"@{nameof(UserMenuInfo.Target)}", menuInfo.Target)
//            };

//            var menuId = DatabaseApi.ExecuteNonQueryAndReturnId(ConnectionString, TableName, nameof(UserMenuInfo.Id), sqlString, parameters);

            var menuId = InsertObject(menuInfo);

            UserMenuManager.ClearCache();

            return menuId;
        }

        public void Update(UserMenuInfo menuInfo)
        {
            //var sqlString = $@"UPDATE {TableName} SET
            //    {nameof(UserMenuInfo.SystemId)} = @{nameof(UserMenuInfo.SystemId)}, 
            //    {nameof(UserMenuInfo.GroupIdCollection)} = @{nameof(UserMenuInfo.GroupIdCollection)}, 
            //    {nameof(UserMenuInfo.IsDisabled)} = @{nameof(UserMenuInfo.IsDisabled)}, 
            //    {nameof(UserMenuInfo.ParentId)} = @{nameof(UserMenuInfo.ParentId)}, 
            //    {nameof(UserMenuInfo.Taxis)} = @{nameof(UserMenuInfo.Taxis)}, 
            //    {nameof(UserMenuInfo.Text)} = @{nameof(UserMenuInfo.Text)}, 
            //    {nameof(UserMenuInfo.IconClass)} = @{nameof(UserMenuInfo.IconClass)}, 
            //    {nameof(UserMenuInfo.Href)} = @{nameof(UserMenuInfo.Href)}, 
            //    {nameof(UserMenuInfo.Target)} = @{nameof(UserMenuInfo.Target)}
            //WHERE {nameof(UserMenuInfo.Id)} = @{nameof(UserMenuInfo.Id)}";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(nameof(UserMenuInfo.SystemId), menuInfo.SystemId),
            //    GetParameter(nameof(UserMenuInfo.GroupIdCollection), menuInfo.GroupIdCollection),
            //    GetParameter(nameof(UserMenuInfo.IsDisabled), menuInfo.IsDisabled),
            //    GetParameter(nameof(UserMenuInfo.ParentId), menuInfo.ParentId),
            //    GetParameter(nameof(UserMenuInfo.Taxis), menuInfo.Taxis),
            //    GetParameter(nameof(UserMenuInfo.Text), menuInfo.Text),
            //    GetParameter(nameof(UserMenuInfo.IconClass), menuInfo.IconClass),
            //    GetParameter(nameof(UserMenuInfo.Href), menuInfo.Href),
            //    GetParameter(nameof(UserMenuInfo.Target), menuInfo.Target),
            //    GetParameter(nameof(UserMenuInfo.Id), menuInfo.Id)
            //};

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

            UpdateObject(menuInfo);

            UserMenuManager.ClearCache();
        }

        public void Delete(int menuId)
        {
            //var sqlString = $"DELETE FROM {TableName} WHERE {nameof(UserMenuInfo.Id)} = @{nameof(UserMenuInfo.Id)} OR {nameof(UserMenuInfo.ParentId)} = @{nameof(UserMenuInfo.ParentId)}";

            //IDataParameter[] parameters =
            //{
            //    GetParameter($"@{nameof(UserMenuInfo.Id)}", menuId),
            //    GetParameter($"@{nameof(UserMenuInfo.ParentId)}", menuId)
            //};

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

            DeleteAll(Q.Where(Attr.Id, menuId).OrWhere(Attr.ParentId, menuId));

            UserMenuManager.ClearCache();
        }

        public List<UserMenuInfo> GetUserMenuInfoList()
        {
            //List<UserMenuInfo> list;

            //var sqlString = $"SELECT * FROM {TableName}";
            //using (var connection = GetConnection())
            //{
            //    list = connection.Query<UserMenuInfo>(sqlString).ToList();
            //}

            var list = GetObjectList();

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
//    public class UserMenu : DataProviderBase
//    {
//        public const string DatabaseTableName = "siteserver_UserMenu";

//        public override string TableName => DatabaseTableName;

//        public override List<TableColumn> TableColumns => new List<TableColumn>
//        {
//            new TableColumn
//            {
//                AttributeName = nameof(UserMenuInfo.Id),
//                DataType = DataType.Integer,
//                IsPrimaryKey = true,
//                IsIdentity = true
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(UserMenuInfo.SystemId),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(UserMenuInfo.GroupIdCollection),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(UserMenuInfo.IsDisabled),
//                DataType = DataType.Boolean
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(UserMenuInfo.ParentId),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(UserMenuInfo.Taxis),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(UserMenuInfo.Text),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(UserMenuInfo.IconClass),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(UserMenuInfo.Href),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(UserMenuInfo.Target),
//                DataType = DataType.VarChar
//            }
//        };

//        public int InsertObject(UserMenuInfo menuInfo)
//        {
//            var sqlString =
//                $@"
//INSERT INTO {TableName} (
//    {nameof(UserMenuInfo.SystemId)}, 
//    {nameof(UserMenuInfo.GroupIdCollection)}, 
//    {nameof(UserMenuInfo.IsDisabled)}, 
//    {nameof(UserMenuInfo.ParentId)}, 
//    {nameof(UserMenuInfo.Taxis)}, 
//    {nameof(UserMenuInfo.Text)}, 
//    {nameof(UserMenuInfo.IconClass)}, 
//    {nameof(UserMenuInfo.Href)}, 
//    {nameof(UserMenuInfo.Target)}
//) VALUES (
//    @{nameof(UserMenuInfo.SystemId)}, 
//    @{nameof(UserMenuInfo.GroupIdCollection)}, 
//    @{nameof(UserMenuInfo.IsDisabled)}, 
//    @{nameof(UserMenuInfo.ParentId)}, 
//    @{nameof(UserMenuInfo.Taxis)}, 
//    @{nameof(UserMenuInfo.Text)}, 
//    @{nameof(UserMenuInfo.IconClass)}, 
//    @{nameof(UserMenuInfo.Href)}, 
//    @{nameof(UserMenuInfo.Target)}
//)";

//            IDataParameter[] parameters =
//            {
//                GetParameter($"@{nameof(UserMenuInfo.SystemId)}", menuInfo.SystemId),
//                GetParameter($"@{nameof(UserMenuInfo.GroupIdCollection)}", menuInfo.GroupIdCollection),
//                GetParameter($"@{nameof(UserMenuInfo.IsDisabled)}", menuInfo.IsDisabled),
//                GetParameter($"@{nameof(UserMenuInfo.ParentId)}", menuInfo.ParentId),
//                GetParameter($"@{nameof(UserMenuInfo.Taxis)}", menuInfo.Taxis),
//                GetParameter($"@{nameof(UserMenuInfo.Text)}", menuInfo.Text),
//                GetParameter($"@{nameof(UserMenuInfo.IconClass)}", menuInfo.IconClass),
//                GetParameter($"@{nameof(UserMenuInfo.Href)}", menuInfo.Href),
//                GetParameter($"@{nameof(UserMenuInfo.Target)}", menuInfo.Target)
//            };

//            var menuId = DatabaseApi.ExecuteNonQueryAndReturnId(ConnectionString, TableName, nameof(UserMenuInfo.Id), sqlString, parameters);

//            UserMenuManager.ClearCache();

//            return menuId;
//        }

//        public void UpdateObject(UserMenuInfo menuInfo)
//        {
//            var sqlString = $@"UPDATE {TableName} SET
//                {nameof(UserMenuInfo.SystemId)} = @{nameof(UserMenuInfo.SystemId)}, 
//                {nameof(UserMenuInfo.GroupIdCollection)} = @{nameof(UserMenuInfo.GroupIdCollection)}, 
//                {nameof(UserMenuInfo.IsDisabled)} = @{nameof(UserMenuInfo.IsDisabled)}, 
//                {nameof(UserMenuInfo.ParentId)} = @{nameof(UserMenuInfo.ParentId)}, 
//                {nameof(UserMenuInfo.Taxis)} = @{nameof(UserMenuInfo.Taxis)}, 
//                {nameof(UserMenuInfo.Text)} = @{nameof(UserMenuInfo.Text)}, 
//                {nameof(UserMenuInfo.IconClass)} = @{nameof(UserMenuInfo.IconClass)}, 
//                {nameof(UserMenuInfo.Href)} = @{nameof(UserMenuInfo.Href)}, 
//                {nameof(UserMenuInfo.Target)} = @{nameof(UserMenuInfo.Target)}
//            WHERE {nameof(UserMenuInfo.Id)} = @{nameof(UserMenuInfo.Id)}";

//            IDataParameter[] parameters =
//            {
//                GetParameter(nameof(UserMenuInfo.SystemId), menuInfo.SystemId),
//                GetParameter(nameof(UserMenuInfo.GroupIdCollection), menuInfo.GroupIdCollection),
//                GetParameter(nameof(UserMenuInfo.IsDisabled), menuInfo.IsDisabled),
//                GetParameter(nameof(UserMenuInfo.ParentId), menuInfo.ParentId),
//                GetParameter(nameof(UserMenuInfo.Taxis), menuInfo.Taxis),
//                GetParameter(nameof(UserMenuInfo.Text), menuInfo.Text),
//                GetParameter(nameof(UserMenuInfo.IconClass), menuInfo.IconClass),
//                GetParameter(nameof(UserMenuInfo.Href), menuInfo.Href),
//                GetParameter(nameof(UserMenuInfo.Target), menuInfo.Target),
//                GetParameter(nameof(UserMenuInfo.Id), menuInfo.Id)
//            };

//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

//            UserMenuManager.ClearCache();
//        }

//        public void DeleteById(int menuId)
//        {
//            var sqlString = $"DELETE FROM {TableName} WHERE {nameof(UserMenuInfo.Id)} = @{nameof(UserMenuInfo.Id)} OR {nameof(UserMenuInfo.ParentId)} = @{nameof(UserMenuInfo.ParentId)}";

//            IDataParameter[] parameters =
//            {
//                GetParameter($"@{nameof(UserMenuInfo.Id)}", menuId),
//                GetParameter($"@{nameof(UserMenuInfo.ParentId)}", menuId)
//            };

//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

//            UserMenuManager.ClearCache();
//        }

//        public List<UserMenuInfo> GetUserMenuInfoList()
//        {
//            List<UserMenuInfo> list;

//            var sqlString = $"SELECT * FROM {TableName}";
//            using (var connection = GetConnection())
//            {
//                list = connection.Query<UserMenuInfo>(sqlString).ToList();
//            }

//            var systemMenus = UserMenuManager.SystemMenus.Value;
//            foreach (var kvp in systemMenus)
//            {
//                var parent = kvp.Key;
//                var children = kvp.Value;

//                if (list.All(x => x.SystemId != parent.SystemId))
//                {
//                    parent.Id = InsertObject(parent);
//                    list.Add(parent);
//                }
//                else
//                {
//                    parent = list.GetObjectById(x => x.SystemId == parent.SystemId);
//                }

//                if (children != null)
//                {
//                    foreach (var child in children)
//                    {
//                        if (list.All(x => x.SystemId != child.SystemId))
//                        {
//                            child.ParentId = parent.Id;
//                            child.Id = InsertObject(child);
//                            list.Add(child);
//                        }
//                    }
//                }
//            }

//            return list.OrderBy(menuInfo => menuInfo.Taxis == 0 ? int.MaxValue : menuInfo.Taxis).ToList();
//        }
//    }
//}
