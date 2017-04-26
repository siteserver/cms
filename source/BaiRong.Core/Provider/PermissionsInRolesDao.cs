using System.Collections;
using System.Collections.Generic;
using System.Data;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;

namespace BaiRong.Core.Provider
{
    public class PermissionsInRolesDao : DataProviderBase
	{
		private const string SqlSelect = "SELECT RoleName, GeneralPermissions FROM bairong_PermissionsInRoles WHERE RoleName = @RoleName";

		private const string SqlInsert = "INSERT INTO bairong_PermissionsInRoles (RoleName, GeneralPermissions) VALUES (@RoleName, @GeneralPermissions)";
		private const string SqlDelete = "DELETE FROM bairong_PermissionsInRoles WHERE RoleName = @RoleName";

		private const string ParmRoleRoleName = "@RoleName";
		private const string ParmGeneralPermissions = "@GeneralPermissions";

        public void InsertRoleAndPermissions(string roleName, string creatorUserName, string description, ArrayList generalPermissionArrayList)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        if (generalPermissionArrayList != null && generalPermissionArrayList.Count > 0)
                        {
                            var permissionsInRolesInfo = new PermissionsInRolesInfo(roleName, TranslateUtils.ObjectCollectionToString(generalPermissionArrayList));
                            BaiRongDataProvider.PermissionsInRolesDao.InsertWithTrans(permissionsInRolesInfo, trans);
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
            BaiRongDataProvider.RoleDao.InsertRole(roleName, creatorUserName, description);
        }

		public void InsertWithTrans(PermissionsInRolesInfo info, IDbTransaction trans) 
		{
			var insertParms = new IDataParameter[]
			{
				GetParameter(ParmRoleRoleName, EDataType.NVarChar, 255, info.RoleName),
				GetParameter(ParmGeneralPermissions, EDataType.Text, info.GeneralPermissions)
			};
							
			ExecuteNonQuery(trans, SqlInsert, insertParms);
		}


		public void DeleteWithTrans(string roleName, IDbTransaction trans)
		{
			var parms = new IDataParameter[]
			{
				GetParameter(ParmRoleRoleName, EDataType.NVarChar, 255, roleName)
			};

			ExecuteNonQuery(trans, SqlDelete, parms);
		}

        public void Delete(string roleName)
        {
            var parms = new IDataParameter[]
			{
				GetParameter(ParmRoleRoleName, EDataType.NVarChar, 255, roleName)
			};

            ExecuteNonQuery(SqlDelete, parms);
        }

        public void UpdateRoleAndGeneralPermissions(string roleName, string description, ArrayList generalPermissionArrayList)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        BaiRongDataProvider.PermissionsInRolesDao.DeleteWithTrans(roleName, trans);
                        if (generalPermissionArrayList != null && generalPermissionArrayList.Count > 0)
                        {
                            var permissionsInRolesInfo = new PermissionsInRolesInfo(roleName, TranslateUtils.ObjectCollectionToString(generalPermissionArrayList));
                            BaiRongDataProvider.PermissionsInRolesDao.InsertWithTrans(permissionsInRolesInfo, trans);
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
            BaiRongDataProvider.RoleDao.UpdateRole(roleName, description);
        }

		private PermissionsInRolesInfo GetPermissionsInRolesInfo(string roleName)
		{
            PermissionsInRolesInfo info = null;
			
			var parms = new IDataParameter[]
			{
				GetParameter(ParmRoleRoleName, EDataType.NVarChar, 255, roleName)
			};
			
			using (var rdr = ExecuteReader(SqlSelect, parms)) 
			{
				if (rdr.Read())
				{
				    var i = 0;
                    info = new PermissionsInRolesInfo(GetString(rdr, i++), GetString(rdr, i)); 					
				}
				rdr.Close();
			}
			return info;
		}


		public List<string> GetGeneralPermissionList(string[] roles)
		{
            var list = new List<string>();
            var roleNameCollection = new List<string>(roles);
			foreach (var roleName in roleNameCollection)
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
