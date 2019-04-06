using System.Collections.Generic;
using Datory;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.Utils;

namespace SiteServer.CMS.Database.Repositories
{
    public class PermissionsInRolesRepository : Repository<PermissionsInRolesInfo>
    {
        public PermissionsInRolesRepository() : base(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString)
        {
        }

        private static class Attr
        {
            public const string RoleName = nameof(PermissionsInRolesInfo.RoleName);
        }

        //public override void Insert(PermissionsInRolesInfo info)
        //{
        //    //IDataParameter[] parameters =
        //    //{
        //    //    GetParameter(ParamRoleRoleName, info.RoleName),
        //    //    GetParameter(ParamGeneralPermissions,info.GeneralPermissions)
        //    //};
        //    //string SqlInsert = "INSERT INTO siteserver_PermissionsInRoles (RoleName, GeneralPermissions) VALUES (@RoleName, @GeneralPermissions)";
        //    //DatabaseApi.ExecuteNonQuery(trans, SqlInsert, parameters);

        //    InsertObject(info);
        //}

        public override bool Delete(string roleName)
        {
            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamRoleRoleName, roleName)
            //};

            //DatabaseApi.ExecuteNonQuery(ConnectionString, SqlDelete, parameters);

            return base.Delete(Q
                .Where(Attr.RoleName, roleName)) == 1;
        }

        public void UpdateRoleAndGeneralPermissions(string roleName, string description, List<string> generalPermissionList)
        {
            Delete(roleName);
            if (generalPermissionList != null && generalPermissionList.Count > 0)
            {
                var permissionsInRolesInfo = new PermissionsInRolesInfo
                {
                    RoleName = roleName,
                    GeneralPermissionList = generalPermissionList
                };
                Insert(permissionsInRolesInfo);
            }

            DataProvider.Role.UpdateRole(roleName, description);
        }

        private PermissionsInRolesInfo GetPermissionsInRolesInfo(string roleName)
        {
            //PermissionsInRolesInfo info = null;

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamRoleRoleName, roleName)
            //};
            //string SqlSelect = "SELECT Id, RoleName, GeneralPermissions FROM siteserver_PermissionsInRoles WHERE RoleName = @RoleName";
            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelect, parameters))
            //{
            //    if (rdr.Read())
            //    {
            //        var i = 0;
            //        info = new PermissionsInRolesInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i));
            //    }
            //    rdr.Close();
            //}
            //return info;

            return Get(Q.Where(Attr.RoleName, roleName));
        }

        public List<string> GetGeneralPermissionList(IEnumerable<string> roles)
        {
            var list = new List<string>();
            if (roles == null) return list;

            foreach (var roleName in roles)
            {
                var permissionsInRolesInfo = GetPermissionsInRolesInfo(roleName);
                if (permissionsInRolesInfo != null)
                {
                    foreach (var permission in permissionsInRolesInfo.GeneralPermissionList)
                    {
                        if (!list.Contains(permission)) list.Add(permission);
                    }
                }
            }

            return list;
        }
    }
}


//using System.Collections.Generic;
//using System.Data;
//using SiteServer.CMS.Database.Core;
//using SiteServer.CMS.Database.Models;
//using SiteServer.Plugin;
//using SiteServer.Utils;

//namespace SiteServer.CMS.Database.Repositories
//{
//    public class PermissionsInRoles : DataProviderBase
//	{
//        public override string TableName => "siteserver_PermissionsInRoles";

//        public override List<TableColumn> TableColumns => new List<TableColumn>
//        {
//            new TableColumn
//            {
//                AttributeName = nameof(PermissionsInRolesInfo.Id),
//                DataType = DataType.Integer,
//                IsPrimaryKey = true,
//                IsIdentity = true
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(PermissionsInRolesInfo.RoleName),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(PermissionsInRolesInfo.GeneralPermissions),
//                DataType = DataType.Text
//            }
//        };

//        private const string SqlSelect = "SELECT Id, RoleName, GeneralPermissions FROM siteserver_PermissionsInRoles WHERE RoleName = @RoleName";

//		private const string SqlInsert = "INSERT INTO siteserver_PermissionsInRoles (RoleName, GeneralPermissions) VALUES (@RoleName, @GeneralPermissions)";
//		private const string SqlDelete = "DELETE FROM siteserver_PermissionsInRoles WHERE RoleName = @RoleName";

//		private const string ParamRoleRoleName = "@RoleName";
//		private const string ParamGeneralPermissions = "@GeneralPermissions";

//		public void InsertWithTrans(PermissionsInRolesInfo info, IDbTransaction trans) 
//		{
//		    IDataParameter[] parameters =
//			{
//				GetParameter(ParamRoleRoleName, info.RoleName),
//				GetParameter(ParamGeneralPermissions,info.GeneralPermissions)
//			};

//		    DatabaseApi.ExecuteNonQuery(trans, SqlInsert, parameters);
//		}

//		public void DeleteWithTrans(string roleName, IDbTransaction trans)
//		{
//			IDataParameter[] parameters =
//			{
//				GetParameter(ParamRoleRoleName, roleName)
//			};

//		    DatabaseApi.ExecuteNonQuery(trans, SqlDelete, parameters);
//		}

//        public void DeleteById(string roleName)
//        {
//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamRoleRoleName, roleName)
//			};

//            DatabaseApi.ExecuteNonQuery(ConnectionString, SqlDelete, parameters);
//        }

//        public void UpdateRoleAndGeneralPermissions(string roleName, string description, List<string> generalPermissionList)
//        {
//            using (var conn = GetConnection())
//            {
//                conn.Open();
//                using (var trans = conn.BeginTransaction())
//                {
//                    try
//                    {
//                        DataProvider.PermissionsInRoles.DeleteWithTrans(roleName, trans);
//                        if (generalPermissionList != null && generalPermissionList.Count > 0)
//                        {
//                            var permissionsInRolesInfo = new PermissionsInRolesInfo(0, roleName, TranslateUtils.ObjectCollectionToString(generalPermissionList));
//                            DataProvider.PermissionsInRoles.InsertWithTrans(permissionsInRolesInfo, trans);
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

//            DataProvider.Role.UpdateRole(roleName, description);
//        }

//		private PermissionsInRolesInfo GetPermissionsInRolesInfo(string roleName)
//		{
//            PermissionsInRolesInfo info = null;

//			IDataParameter[] parameters =
//			{
//				GetParameter(ParamRoleRoleName, roleName)
//			};

//			using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelect, parameters)) 
//			{
//				if (rdr.Read())
//				{
//				    var i = 0;
//                    info = new PermissionsInRolesInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i)); 					
//				}
//				rdr.Close();
//			}
//			return info;
//		}

//		public List<string> GetGeneralPermissionList(IEnumerable<string> roles)
//		{
//            var list = new List<string>();
//		    if (roles == null) return list;

//			foreach (var roleName in roles)
//			{
//                var permissionsInRolesInfo = GetPermissionsInRolesInfo(roleName);
//                if (permissionsInRolesInfo != null)
//				{
//                    var permissionList = TranslateUtils.StringCollectionToStringList(permissionsInRolesInfo.GeneralPermissions);
//                    foreach (var permission in permissionList)
//					{
//                        if (!list.Contains(permission)) list.Add(permission);
//					}
//				}
//			}

//			return list;
//		}
//	}
//}
