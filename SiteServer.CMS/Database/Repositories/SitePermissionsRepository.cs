using System.Collections.Generic;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Utils;

namespace SiteServer.CMS.Database.Repositories
{
    public class SitePermissionsRepository : GenericRepository<SitePermissionsInfo>
    {
        private static class Attr
        {
            public const string RoleName = nameof(SitePermissionsInfo.RoleName);
            public const string SiteId = nameof(SitePermissionsInfo.SiteId);
        }

        private void Insert(SitePermissionsInfo permissionsInfo)
        {
            if (IsExists(permissionsInfo.RoleName, permissionsInfo.SiteId))
            {
                Delete(permissionsInfo.RoleName, permissionsInfo.SiteId);
            }

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamRoleRoleName, permissionsInfo.RoleName),
            //    GetParameter(ParamSiteId, permissionsInfo.SiteId),
            //    GetParameter(ParamChannelIdCollection,permissionsInfo.ChannelIdCollection),
            //    GetParameter(ParamChannelPermissions,permissionsInfo.ChannelPermissions),
            //    GetParameter(ParamWebsitePermissions,permissionsInfo.WebsitePermissions)
            //};
            //string SqlInsert = "INSERT INTO siteserver_SitePermissions (RoleName, SiteId, ChannelIdCollection, ChannelPermissions, WebsitePermissions) VALUES (@RoleName, @SiteId, @ChannelIdCollection, @ChannelPermissions, @WebsitePermissions)";
            //DatabaseApi.ExecuteNonQuery(WebConfigUtils.ConnectionString, SqlInsert, parameters);

            InsertObject(permissionsInfo);
        }


        private void Delete(string roleName)
        {
            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamRoleRoleName, roleName)
            //};
            //string SqlDelete = "DELETE FROM siteserver_SitePermissions WHERE RoleName = @RoleName";
            //DatabaseApi.ExecuteNonQuery(WebConfigUtils.ConnectionString, SqlDelete, parameters);

            DeleteAll(Q.Where(Attr.RoleName, roleName));
        }

        private void Delete(string roleName, int siteId)
        {
            //const string sqlString = "DELETE FROM siteserver_SitePermissions WHERE RoleName = @RoleName AND SiteId = @SiteId";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamRoleRoleName, roleName),
            //    GetParameter(ParamSiteId, siteId)
            //};

            //DatabaseApi.ExecuteNonQuery(WebConfigUtils.ConnectionString, sqlString, parameters);

            DeleteAll(Q.Where(Attr.RoleName, roleName).Where(Attr.SiteId, siteId));
        }

        private bool IsExists(string roleName, int siteId)
        {
            //var isExists = false;

            //const string sqlString = "SELECT RoleName FROM siteserver_SitePermissions WHERE RoleName = @RoleName AND SiteId = @SiteId";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamRoleRoleName, roleName),
            //    GetParameter(ParamSiteId, siteId)
            //};

            //using (var rdr = DatabaseApi.ExecuteReader(WebConfigUtils.ConnectionString, sqlString, parameters))
            //{
            //    if (rdr.Read())
            //    {
            //        isExists = true;
            //    }
            //    rdr.Close();
            //}

            //return isExists;

            return Exists(Q.Where(Attr.RoleName, roleName).Where(Attr.SiteId, siteId));
        }

        public IList<SitePermissionsInfo> GetSystemPermissionsInfoList(string roleName)
        {
            //var list = new List<SitePermissionsInfo>();

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamRoleRoleName, roleName)
            //};
            //string SqlSelectAllByRoleName = "SELECT RoleName, SiteId, ChannelIdCollection, ChannelPermissions, WebsitePermissions FROM siteserver_SitePermissions WHERE RoleName = @RoleName ORDER BY SiteId DESC";
            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectAllByRoleName, parameters))
            //{
            //    while (rdr.Read())
            //    {
            //        var i = 0;
            //        var permissionsInfo = new SitePermissionsInfo(DatabaseApi.GetString(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i));
            //        list.Add(permissionsInfo);
            //    }
            //    rdr.Close();
            //}

            //return list;

            return GetObjectList(Q.Where(Attr.RoleName, roleName).OrderByDesc(Attr.SiteId));
        }

        public Dictionary<int, List<string>> GetWebsitePermissionSortedList(IEnumerable<string> roles)
        {
            var sortedList = new Dictionary<int, List<string>>();
            if (roles == null) return sortedList;

            foreach (var roleName in roles)
            {
                var systemPermissionsList = GetSystemPermissionsInfoList(roleName);
                foreach (var systemPermissionsInfo in systemPermissionsList)
                {
                    var list = new List<string>();
                    var websitePermissionList = TranslateUtils.StringCollectionToStringList(systemPermissionsInfo.WebsitePermissions);
                    foreach (var websitePermission in websitePermissionList)
                    {
                        if (!list.Contains(websitePermission)) list.Add(websitePermission);
                    }
                    sortedList[systemPermissionsInfo.SiteId] = list;
                }
            }

            return sortedList;
        }

        public Dictionary<string, List<string>> GetChannelPermissionSortedList(IList<string> roles)
        {
            var dict = new Dictionary<string, List<string>>();
            if (roles == null) return dict;

            foreach (var roleName in roles)
            {
                var systemPermissionsInfoList = GetSystemPermissionsInfoList(roleName);
                foreach (var systemPermissionsInfo in systemPermissionsInfoList)
                {
                    var channelIdList = TranslateUtils.StringCollectionToIntList(systemPermissionsInfo.ChannelIdCollection);
                    foreach (var channelId in channelIdList)
                    {
                        var key = PermissionsImpl.GetChannelPermissionDictKey(systemPermissionsInfo.SiteId, channelId);

                        if (!dict.TryGetValue(key, out var list))
                        {
                            list = new List<string>();
                            dict[key] = list;
                        }

                        var channelPermissionList = TranslateUtils.StringCollectionToStringList(systemPermissionsInfo.ChannelPermissions);
                        foreach (var channelPermission in channelPermissionList)
                        {
                            if (!list.Contains(channelPermission)) list.Add(channelPermission);
                        }
                    }
                }
            }

            return dict;
        }

        public List<string> GetChannelPermissionListIgnoreChannelId(IList<string> roles)
        {
            var list = new List<string>();
            if (roles == null) return list;

            foreach (var roleName in roles)
            {
                var systemPermissionsInfoList = GetSystemPermissionsInfoList(roleName);
                foreach (var systemPermissionsInfo in systemPermissionsInfoList)
                {
                    var channelPermissionList = TranslateUtils.StringCollectionToStringList(systemPermissionsInfo.ChannelPermissions);
                    foreach (var channelPermission in channelPermissionList)
                    {
                        if (!list.Contains(channelPermission))
                        {
                            list.Add(channelPermission);
                        }
                    }
                }
            }

            return list;
        }

        //public new void UpdateObject(SitePermissionsInfo permissionsInfo)
        //{
        //    //IDataParameter[] parameters =
        //    //{
        //    //    GetParameter(ParamRoleRoleName, permissionsInfo.RoleName),
        //    //    GetParameter(ParamSiteId, permissionsInfo.SiteId),
        //    //    GetParameter(ParamChannelIdCollection,permissionsInfo.ChannelIdCollection),
        //    //    GetParameter(ParamChannelPermissions,permissionsInfo.ChannelPermissions),
        //    //    GetParameter(ParamWebsitePermissions,permissionsInfo.WebsitePermissions)
        //    //};
        //    //string SqlUpdate = "UPDATE siteserver_SitePermissions SET ChannelIdCollection = @ChannelIdCollection, ChannelPermissions = @ChannelPermissions, WebsitePermissions = @WebsitePermissions WHERE RoleName = @RoleName AND SiteId = @SiteId";
        //    //DatabaseApi.ExecuteNonQuery(ConnectionString, SqlUpdate, parameters);

        //    base.UpdateObject(permissionsInfo);
        //}

        public void InsertRoleAndPermissions(string roleName, string creatorUserName, string description, List<string> generalPermissionList, List<SitePermissionsInfo> systemPermissionsInfoList)
        {
            if (generalPermissionList != null && generalPermissionList.Count > 0)
            {
                var permissionsInRolesInfo = new PermissionsInRolesInfo
                {
                    RoleName = roleName,
                    GeneralPermissionList = generalPermissionList
                };

                DataProvider.PermissionsInRoles.Insert(permissionsInRolesInfo);
            }

            foreach (var systemPermissionsInfo in systemPermissionsInfoList)
            {
                systemPermissionsInfo.RoleName = roleName;
                Insert(systemPermissionsInfo);
            }

            DataProvider.Role.InsertRole(new RoleInfo
            {
                RoleName = roleName,
                CreatorUserName = creatorUserName,
                Description = description
            });
        }

        public void UpdateSitePermissions(string roleName, List<SitePermissionsInfo> sitePermissionsInfoList)
        {
            Delete(roleName);
            foreach (var sitePermissionsInfo in sitePermissionsInfoList)
            {
                sitePermissionsInfo.RoleName = roleName;
                Insert(sitePermissionsInfo);
            }
        }
    }
}


//using System.Collections.Generic;
//using System.Data;
//using SiteServer.CMS.Database.Core;
//using SiteServer.CMS.Database.Models;
//using SiteServer.CMS.Plugin.Impl;
//using SiteServer.Plugin;
//using SiteServer.Utils;

//namespace SiteServer.CMS.Database.Repositories
//{
//    public class SitePermissions : DataProviderBase
//    {
//        public override string TableName => "siteserver_SitePermissions";

//        public override List<TableColumn> TableColumns => new List<TableColumn>
//        {
//            new TableColumn
//            {
//                AttributeName = nameof(SitePermissionsInfo.Id),
//                DataType = DataType.Integer,
//                IsIdentity = true,
//                IsPrimaryKey = true
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(SitePermissionsInfo.RoleName),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(SitePermissionsInfo.SiteId),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(SitePermissionsInfo.ChannelIdCollection),
//                DataType = DataType.Text
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(SitePermissionsInfo.ChannelPermissions),
//                DataType = DataType.Text
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(SitePermissionsInfo.WebsitePermissions),
//                DataType = DataType.Text
//            }
//        };

//        private const string SqlSelectAllByRoleName = "SELECT RoleName, SiteId, ChannelIdCollection, ChannelPermissions, WebsitePermissions FROM siteserver_SitePermissions WHERE RoleName = @RoleName ORDER BY SiteId DESC";

//        private const string SqlInsert = "INSERT INTO siteserver_SitePermissions (RoleName, SiteId, ChannelIdCollection, ChannelPermissions, WebsitePermissions) VALUES (@RoleName, @SiteId, @ChannelIdCollection, @ChannelPermissions, @WebsitePermissions)";

//        private const string SqlDelete = "DELETE FROM siteserver_SitePermissions WHERE RoleName = @RoleName";

//        private const string SqlUpdate = "UPDATE siteserver_SitePermissions SET ChannelIdCollection = @ChannelIdCollection, ChannelPermissions = @ChannelPermissions, WebsitePermissions = @WebsitePermissions WHERE RoleName = @RoleName AND SiteId = @SiteId";

//        private const string ParamRoleRoleName = "@RoleName";
//        private const string ParamSiteId = "@SiteId";
//        private const string ParamChannelIdCollection = "@ChannelIdCollection";
//        private const string ParamChannelPermissions = "@ChannelPermissions";
//        private const string ParamWebsitePermissions = "@WebsitePermissions";

//        public void InsertWithTrans(SitePermissionsInfo permissionsInfo, IDbTransaction trans)
//        {
//            if (IsExists(permissionsInfo.RoleName, permissionsInfo.SiteId, trans))
//            {
//                DeleteWithTrans(permissionsInfo.RoleName, permissionsInfo.SiteId, trans);
//            }

//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamRoleRoleName, permissionsInfo.RoleName),
//				GetParameter(ParamSiteId, permissionsInfo.SiteId),
//				GetParameter(ParamChannelIdCollection,permissionsInfo.ChannelIdCollection),
//				GetParameter(ParamChannelPermissions,permissionsInfo.ChannelPermissions),
//				GetParameter(ParamWebsitePermissions,permissionsInfo.WebsitePermissions)
//			};

//            DatabaseApi.ExecuteNonQuery(trans, SqlInsert, parameters);
//        }


//        public void DeleteWithTrans(string roleName, IDbTransaction trans)
//        {
//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamRoleRoleName, roleName)
//			};

//            DatabaseApi.ExecuteNonQuery(trans, SqlDelete, parameters);
//        }

//        private void DeleteWithTrans(string roleName, int siteId, IDbTransaction trans)
//        {
//            const string sqlString = "DELETE FROM siteserver_SitePermissions WHERE RoleName = @RoleName AND SiteId = @SiteId";

//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamRoleRoleName, roleName),
//                GetParameter(ParamSiteId, siteId)
//			};

//            DatabaseApi.ExecuteNonQuery(trans, sqlString, parameters);
//        }

//        private bool IsExists(string roleName, int siteId, IDbTransaction trans)
//        {
//            var isExists = false;

//            const string sqlString = "SELECT RoleName FROM siteserver_SitePermissions WHERE RoleName = @RoleName AND SiteId = @SiteId";

//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamRoleRoleName, roleName),
//                GetParameter(ParamSiteId, siteId)
//			};

//            using (var rdr = DatabaseApi.ExecuteReader(trans, sqlString, parameters))
//            {
//                if (rdr.Read())
//                {
//                    isExists = true;
//                }
//                rdr.Close();
//            }

//            return isExists;
//        }

//        public List<SitePermissionsInfo> GetSystemPermissionsInfoList(string roleName)
//        {
//            var list = new List<SitePermissionsInfo>();

//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamRoleRoleName, roleName)
//			};

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectAllByRoleName, parameters))
//            {
//                while (rdr.Read())
//                {
//                    var i = 0;
//                    var permissionsInfo = new SitePermissionsInfo(DatabaseApi.GetString(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i));
//                    list.Add(permissionsInfo);
//                }
//                rdr.Close();
//            }

//            return list;
//        }

//        public Dictionary<int, List<string>> GetWebsitePermissionSortedList(IEnumerable<string> roles)
//        {
//            var sortedList = new Dictionary<int, List<string>>();
//            if (roles == null) return sortedList;

//            foreach (var roleName in roles)
//            {
//                var systemPermissionsList = GetSystemPermissionsInfoList(roleName);
//                foreach (var systemPermissionsInfo in systemPermissionsList)
//                {
//                    var list = new List<string>();
//                    var websitePermissionList = TranslateUtils.StringCollectionToStringList(systemPermissionsInfo.WebsitePermissions);
//                    foreach (var websitePermission in websitePermissionList)
//                    {
//                        if (!list.Contains(websitePermission)) list.Add(websitePermission);
//                    }
//                    sortedList[systemPermissionsInfo.SiteId] = list;
//                }
//            }

//            return sortedList;
//        }

//        public Dictionary<string, List<string>> GetChannelPermissionSortedList(IList<string> roles)
//        {
//            var dict = new Dictionary<string, List<string>>();
//            if (roles == null) return dict;

//            foreach (var roleName in roles)
//            {
//                var systemPermissionsInfoList = GetSystemPermissionsInfoList(roleName);
//                foreach (var systemPermissionsInfo in systemPermissionsInfoList)
//                {
//                    var channelIdList = TranslateUtils.StringCollectionToIntList(systemPermissionsInfo.ChannelIdCollection);
//                    foreach (var channelId in channelIdList)
//                    {
//                        var key = PermissionsImpl.GetChannelPermissionDictKey(systemPermissionsInfo.SiteId, channelId);

//                        if (!dict.TryGetValue(key, out var list))
//                        {
//                            list = new List<string>();
//                            dict[key] = list;
//                        }

//                        var channelPermissionList = TranslateUtils.StringCollectionToStringList(systemPermissionsInfo.ChannelPermissions);
//                        foreach (var channelPermission in channelPermissionList)
//                        {
//                            if (!list.Contains(channelPermission)) list.Add(channelPermission);
//                        }
//                    }
//                }
//            }

//            return dict;
//        }

//        public List<string> GetChannelPermissionListIgnoreChannelId(IList<string> roles)
//        {
//            var list = new List<string>();
//            if (roles == null) return list;

//            foreach (var roleName in roles)
//            {
//                var systemPermissionsInfoList = GetSystemPermissionsInfoList(roleName);
//                foreach (var systemPermissionsInfo in systemPermissionsInfoList)
//                {
//                    var channelPermissionList = TranslateUtils.StringCollectionToStringList(systemPermissionsInfo.ChannelPermissions);
//                    foreach (var channelPermission in channelPermissionList)
//                    {
//                        if (!list.Contains(channelPermission))
//                        {
//                            list.Add(channelPermission);
//                        }
//                    }
//                }
//            }

//            return list;
//        }

//        public void UpdateObject(SitePermissionsInfo permissionsInfo)
//        {
//            IDataParameter[] parameters =
//            {
//                GetParameter(ParamRoleRoleName, permissionsInfo.RoleName),
//                GetParameter(ParamSiteId, permissionsInfo.SiteId),
//                GetParameter(ParamChannelIdCollection,permissionsInfo.ChannelIdCollection),
//                GetParameter(ParamChannelPermissions,permissionsInfo.ChannelPermissions),
//                GetParameter(ParamWebsitePermissions,permissionsInfo.WebsitePermissions)
//            };

//            DatabaseApi.ExecuteNonQuery(ConnectionString, SqlUpdate, parameters);
//        }

//        public void InsertRoleAndPermissions(string roleName, string creatorUserName, string description, List<string> generalPermissionList, List<SitePermissionsInfo> systemPermissionsInfoList)
//        {
//            using (var conn = GetConnection())
//            {
//                conn.Open();
//                using (var trans = conn.BeginTransaction())
//                {
//                    try
//                    {
//                        if (generalPermissionList != null && generalPermissionList.Count > 0)
//                        {
//                            var permissionsInRolesInfo = new PermissionsInRolesInfo(0, roleName, TranslateUtils.ObjectCollectionToString(generalPermissionList));
//                            DataProvider.PermissionsInRoles.InsertWithTrans(permissionsInRolesInfo, trans);
//                        }

//                        foreach (var systemPermissionsInfo in systemPermissionsInfoList)
//                        {
//                            systemPermissionsInfo.RoleName = roleName;
//                            InsertWithTrans(systemPermissionsInfo, trans);
//                        }

//                        trans.Commit();
//                    }
//                    catch
//                    {
//                        trans.Rollback();
//                        throw;
//                    }
//                }
//            }
//            DataProvider.Role.InsertRole(new RoleInfo
//            {
//                RoleName = roleName,
//                CreatorUserName = creatorUserName,
//                Description = description
//            });
//        }

//        public void UpdateSitePermissions(string roleName, List<SitePermissionsInfo> sitePermissionsInfoList)
//        {
//            using (var conn = GetConnection())
//            {
//                conn.Open();
//                using (var trans = conn.BeginTransaction())
//                {
//                    try
//                    {
//                        DeleteWithTrans(roleName, trans);
//                        foreach (var sitePermissionsInfo in sitePermissionsInfoList)
//                        {
//                            sitePermissionsInfo.RoleName = roleName;
//                            InsertWithTrans(sitePermissionsInfo, trans);
//                        }

//                        trans.Commit();
//                    }
//                    catch
//                    {
//                        trans.Rollback();
//                        throw;
//                    }
//                }
//            }
//        }
//    }
//}
