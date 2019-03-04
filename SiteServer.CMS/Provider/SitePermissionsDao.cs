using System.Collections.Generic;
using System.Data;
using SiteServer.CMS.Core;
using SiteServer.CMS.Data;
using SiteServer.CMS.DataCache;
using SiteServer.Utils;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;

namespace SiteServer.CMS.Provider
{
    public class SitePermissionsDao : DataProviderBase
    {
        public override string TableName => "siteserver_SitePermissions";

        public override List<TableColumn> TableColumns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(SitePermissionsInfo.Id),
                DataType = DataType.Integer,
                IsIdentity = true,
                IsPrimaryKey = true
            },
            new TableColumn
            {
                AttributeName = nameof(SitePermissionsInfo.RoleName),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(SitePermissionsInfo.SiteId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(SitePermissionsInfo.ChannelIdCollection),
                DataType = DataType.Text
            },
            new TableColumn
            {
                AttributeName = nameof(SitePermissionsInfo.ChannelPermissions),
                DataType = DataType.Text
            },
            new TableColumn
            {
                AttributeName = nameof(SitePermissionsInfo.WebsitePermissions),
                DataType = DataType.Text
            }
        };

        private const string SqlSelectAllByRoleName = "SELECT RoleName, SiteId, ChannelIdCollection, ChannelPermissions, WebsitePermissions FROM siteserver_SitePermissions WHERE RoleName = @RoleName ORDER BY SiteId DESC";

        private const string SqlInsert = "INSERT INTO siteserver_SitePermissions (RoleName, SiteId, ChannelIdCollection, ChannelPermissions, WebsitePermissions) VALUES (@RoleName, @SiteId, @ChannelIdCollection, @ChannelPermissions, @WebsitePermissions)";
        private const string SqlDelete = "DELETE FROM siteserver_SitePermissions WHERE RoleName = @RoleName";

        private const string ParmRoleRoleName = "@RoleName";
        private const string ParmSiteId = "@SiteId";
        private const string ParmChannelIdCollection = "@ChannelIdCollection";
        private const string ParmChannelPermissions = "@ChannelPermissions";
        private const string ParmWebsitePermissions = "@WebsitePermissions";

        public void InsertWithTrans(SitePermissionsInfo info, IDbTransaction trans)
        {
            if (IsExists(info.RoleName, info.SiteId, trans))
            {
                DeleteWithTrans(info.RoleName, info.SiteId, trans);
            }

            var insertParms = new IDataParameter[]
			{
				GetParameter(ParmRoleRoleName, DataType.VarChar, 255, info.RoleName),
				GetParameter(ParmSiteId, DataType.Integer, info.SiteId),
				GetParameter(ParmChannelIdCollection, DataType.Text, info.ChannelIdCollection),
				GetParameter(ParmChannelPermissions, DataType.Text, info.ChannelPermissions),
				GetParameter(ParmWebsitePermissions, DataType.Text, info.WebsitePermissions)
			};

            ExecuteNonQuery(trans, SqlInsert, insertParms);
        }


        public void DeleteWithTrans(string roleName, IDbTransaction trans)
        {
            var parms = new IDataParameter[]
			{
				GetParameter(ParmRoleRoleName, DataType.VarChar, 255, roleName)
			};

            ExecuteNonQuery(trans, SqlDelete, parms);
        }

        private void DeleteWithTrans(string roleName, int siteId, IDbTransaction trans)
        {
            var sqlString = "DELETE FROM siteserver_SitePermissions WHERE RoleName = @RoleName AND SiteId = @SiteId";

            var parms = new IDataParameter[]
			{
				GetParameter(ParmRoleRoleName, DataType.VarChar, 255, roleName),
                GetParameter(ParmSiteId, DataType.Integer, siteId)
			};

            ExecuteNonQuery(trans, sqlString, parms);
        }

        private bool IsExists(string roleName, int siteId, IDbTransaction trans)
        {
            var isExists = false;

            var sqlString = "SELECT RoleName FROM siteserver_SitePermissions WHERE RoleName = @RoleName AND SiteId = @SiteId";

            var parms = new IDataParameter[]
			{
				GetParameter(ParmRoleRoleName, DataType.VarChar, 255, roleName),
                GetParameter(ParmSiteId, DataType.Integer, siteId)
			};

            using (var rdr = ExecuteReader(trans, sqlString, parms))
            {
                if (rdr.Read())
                {
                    isExists = true;
                }
                rdr.Close();
            }

            return isExists;
        }

        public List<SitePermissionsInfo> GetSystemPermissionsInfoList(string roleName)
        {
            var list = new List<SitePermissionsInfo>();

            var parms = new IDataParameter[]
			{
				GetParameter(ParmRoleRoleName, DataType.VarChar, 255, roleName)
			};

            using (var rdr = ExecuteReader(SqlSelectAllByRoleName, parms))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var info = new SitePermissionsInfo(GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i));
                    list.Add(info);
                }
                rdr.Close();
            }

            return list;
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
                foreach (SitePermissionsInfo systemPermissionsInfo in systemPermissionsInfoList)
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



        public List<SitePermissionsInfo> GetSystemPermissionsInfoListBySiteId(int siteId, string whereStr)
        {
            var list = new List<SitePermissionsInfo>();

            var parms = new IDataParameter[]
            {
                GetParameter(ParmSiteId, DataType.Integer,  siteId)
            };

            using (var rdr = ExecuteReader(
                $"SELECT RoleName, SiteId, ChannelIdCollection, ChannelPermissions, WebsitePermissions FROM siteserver_SitePermissions WHERE SiteId = @SiteId {whereStr} ", parms))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var info = new SitePermissionsInfo(GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i));
                    list.Add(info);
                }
                rdr.Close();
            }

            return list;
        }


        private const string SqlSelectAllByRp = "SELECT RoleName, SiteId, ChannelIdCollection, ChannelPermissions, WebsitePermissions FROM siteserver_SitePermissions WHERE RoleName = @RoleName AND SiteId=@SiteId ORDER BY SiteId DESC";
        /// <summary>
        /// 根据角色名和站点ID获取角色权限信息
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="siteId"></param>
        /// <returns></returns>
        public SitePermissionsInfo GetSystemPermissionsInfoByRp(string roleName, int siteId)
        {
            SitePermissionsInfo info = null;

            var parms = new IDataParameter[]
            {
                GetParameter(ParmRoleRoleName, DataType.VarChar, 255, roleName),
                GetParameter(ParmSiteId, DataType.Integer, siteId)
            };

            using (var rdr = ExecuteReader(SqlSelectAllByRp, parms))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    info = new SitePermissionsInfo(GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i));
                }
                rdr.Close();
            }

            return info;
        }

        /// <summary>
        /// 根据角色名和站点ID获取角色的站点权限信息
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="siteId"></param>
        /// <returns></returns>
        public List<string> GetWebsitePermissionListByRp(string roleName, int siteId)
        {
            var systemPermissionsInfo = GetSystemPermissionsInfoByRp(roleName, siteId);

            return TranslateUtils.StringCollectionToStringList(systemPermissionsInfo.WebsitePermissions);
        }

        string _sqlUpdate = "update siteserver_SitePermissions set  ChannelIdCollection=@ChannelIdCollection, ChannelPermissions=@ChannelPermissions, WebsitePermissions=@WebsitePermissions where RoleName =@RoleName and SiteId = @SiteId";

        public void Update(SitePermissionsInfo info)
        {
            var updateParms = new IDataParameter[]
            {
                GetParameter(ParmRoleRoleName, DataType.VarChar, 255, info.RoleName),
                GetParameter(ParmSiteId, DataType.Integer, info.SiteId),
                GetParameter(ParmChannelIdCollection, DataType.Text, info.ChannelIdCollection),
                GetParameter(ParmChannelPermissions, DataType.Text, info.ChannelPermissions),
                GetParameter(ParmWebsitePermissions, DataType.Text, info.WebsitePermissions)
            };

            ExecuteNonQuery(_sqlUpdate, updateParms);
        }

        public List<SitePermissionsInfo> GetAllPermissionList(string[] roles, int siteId, bool iscc)
        {
            var permissionList = new List<SitePermissionsInfo>();
            var roleNameCollection = new List<string>(roles);
            foreach (var roleName in roleNameCollection)
            {
                var systemPermissionsInfoList = GetSystemPermissionsInfoList(roleName);
                foreach (var systemPermissionsInfo in systemPermissionsInfoList)
                {
                    if (siteId != 0)
                    {
                        if (iscc)
                        {
                            if (systemPermissionsInfo.SiteId == siteId &&
                                !string.IsNullOrEmpty(systemPermissionsInfo.ChannelIdCollection))
                            {
                                permissionList.Add(systemPermissionsInfo);
                            }
                        }
                        else
                        {
                            if (systemPermissionsInfo.SiteId == siteId)
                            {
                                permissionList.Add(systemPermissionsInfo);
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(systemPermissionsInfo.ChannelIdCollection))
                        {
                            permissionList.Add(systemPermissionsInfo);
                        }
                        else
                        {
                            permissionList.Add(systemPermissionsInfo);
                        }
                    }
                }
            }

            return permissionList;
        }

        public void InsertRoleAndPermissions(string roleName, string creatorUserName, string description, List<string> generalPermissionList, List<SitePermissionsInfo> systemPermissionsInfoList)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        if (generalPermissionList != null && generalPermissionList.Count > 0)
                        {
                            var permissionsInRolesInfo = new PermissionsInRolesInfo(0, roleName, TranslateUtils.ObjectCollectionToString(generalPermissionList));
                            DataProvider.PermissionsInRolesDao.InsertWithTrans(permissionsInRolesInfo, trans);
                        }

                        foreach (var systemPermissionsInfo in systemPermissionsInfoList)
                        {
                            systemPermissionsInfo.RoleName = roleName;
                            InsertWithTrans(systemPermissionsInfo, trans);
                        }

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
            DataProvider.RoleDao.InsertRole(new RoleInfo
            {
                RoleName = roleName,
                CreatorUserName = creatorUserName,
                Description = description
            });
        }

        public void UpdateSitePermissions(string roleName, List<SitePermissionsInfo> sitePermissionsInfoList)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        DeleteWithTrans(roleName, trans);
                        foreach (var sitePermissionsInfo in sitePermissionsInfoList)
                        {
                            sitePermissionsInfo.RoleName = roleName;
                            InsertWithTrans(sitePermissionsInfo, trans);
                        }

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        public void DeleteRoleAndPermissions(string roleName)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        DeleteWithTrans(roleName, trans);

                        DataProvider.PermissionsInRolesDao.DeleteWithTrans(roleName, trans);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            DataProvider.RoleDao.DeleteRole(roleName);
        }
    }
}
