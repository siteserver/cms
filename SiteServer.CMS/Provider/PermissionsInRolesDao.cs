using System.Collections.Generic;
using System.Data;
using SiteServer.CMS.Core;
using SiteServer.CMS.Data;
using SiteServer.CMS.Model;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Provider
{
    public class PermissionsInRolesDao : DataProviderBase
	{
        public override string TableName => "siteserver_PermissionsInRoles";

        public override List<TableColumn> TableColumns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(PermissionsInRolesInfo.Id),
                DataType = DataType.Integer,
                IsPrimaryKey = true,
                IsIdentity = true
            },
            new TableColumn
            {
                AttributeName = nameof(PermissionsInRolesInfo.RoleName),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(PermissionsInRolesInfo.GeneralPermissions),
                DataType = DataType.Text
            }
        };

        private const string SqlSelect = "SELECT Id, RoleName, GeneralPermissions FROM siteserver_PermissionsInRoles WHERE RoleName = @RoleName";

		private const string SqlInsert = "INSERT INTO siteserver_PermissionsInRoles (RoleName, GeneralPermissions) VALUES (@RoleName, @GeneralPermissions)";
		private const string SqlDelete = "DELETE FROM siteserver_PermissionsInRoles WHERE RoleName = @RoleName";

		private const string ParmRoleRoleName = "@RoleName";
		private const string ParmGeneralPermissions = "@GeneralPermissions";

        public void InsertRoleAndPermissions(string roleName, string creatorUserName, string description, List<string> generalPermissionList)
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

		public void InsertWithTrans(PermissionsInRolesInfo info, IDbTransaction trans) 
		{
			var insertParms = new IDataParameter[]
			{
				GetParameter(ParmRoleRoleName, DataType.VarChar, 255, info.RoleName),
				GetParameter(ParmGeneralPermissions, DataType.Text, info.GeneralPermissions)
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

        public void Delete(string roleName)
        {
            var parms = new IDataParameter[]
			{
				GetParameter(ParmRoleRoleName, DataType.VarChar, 255, roleName)
			};

            ExecuteNonQuery(SqlDelete, parms);
        }

        public void UpdateRoleAndGeneralPermissions(string roleName, string description, List<string> generalPermissionList)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        DataProvider.PermissionsInRolesDao.DeleteWithTrans(roleName, trans);
                        if (generalPermissionList != null && generalPermissionList.Count > 0)
                        {
                            var permissionsInRolesInfo = new PermissionsInRolesInfo(0, roleName, TranslateUtils.ObjectCollectionToString(generalPermissionList));
                            DataProvider.PermissionsInRolesDao.InsertWithTrans(permissionsInRolesInfo, trans);
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
            DataProvider.RoleDao.UpdateRole(roleName, description);
        }

		private PermissionsInRolesInfo GetPermissionsInRolesInfo(string roleName)
		{
            PermissionsInRolesInfo info = null;
			
			var parms = new IDataParameter[]
			{
				GetParameter(ParmRoleRoleName, DataType.VarChar, 255, roleName)
			};
			
			using (var rdr = ExecuteReader(SqlSelect, parms)) 
			{
				if (rdr.Read())
				{
				    var i = 0;
                    info = new PermissionsInRolesInfo(GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i)); 					
				}
				rdr.Close();
			}
			return info;
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
                    var permissionList = TranslateUtils.StringCollectionToStringList(permissionsInRolesInfo.GeneralPermissions);
                    foreach (var permission in permissionList)
					{
                        if (!list.Contains(permission)) list.Add(permission);
					}
				}
			}

			return list;
		}
	}
}
