using System.Collections.Generic;
using System.Data;
using Datory;
using SiteServer.CMS.Data;
using SiteServer.CMS.Model;
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

		private const string ParamRoleRoleName = "@RoleName";
		private const string ParamGeneralPermissions = "@GeneralPermissions";

        public void Insert(PermissionsInRolesInfo info) 
		{
			var parameters = new IDataParameter[]
			{
				GetParameter(ParamRoleRoleName, DataType.VarChar, 255, info.RoleName),
				GetParameter(ParamGeneralPermissions, DataType.Text, info.GeneralPermissions)
			};
							
			ExecuteNonQuery(SqlInsert, parameters);
		}

        public void Delete(string roleName)
        {
            var parameters = new IDataParameter[]
			{
				GetParameter(ParamRoleRoleName, DataType.VarChar, 255, roleName)
			};

            ExecuteNonQuery(SqlDelete, parameters);
        }

        private PermissionsInRolesInfo GetPermissionsInRolesInfo(string roleName)
		{
            PermissionsInRolesInfo info = null;
			
			var parameters = new IDataParameter[]
			{
				GetParameter(ParamRoleRoleName, DataType.VarChar, 255, roleName)
			};
			
			using (var rdr = ExecuteReader(SqlSelect, parameters)) 
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
