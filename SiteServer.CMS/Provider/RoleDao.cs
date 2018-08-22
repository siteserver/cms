using System.Collections.Generic;
using System.Data;
using SiteServer.CMS.Data;
using SiteServer.CMS.Model;
using SiteServer.Plugin;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Provider
{
    public class RoleDao : DataProviderBase
	{
        public override string TableName => "siteserver_Role";

        public override List<TableColumn> TableColumns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(RoleInfo.Id),
                DataType = DataType.Integer,
                IsIdentity = true,
                IsPrimaryKey = true
            },
            new TableColumn
            {
                AttributeName = nameof(RoleInfo.RoleName),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(RoleInfo.CreatorUserName),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(RoleInfo.Description),
                DataType = DataType.VarChar,
                DataLength = 255
            }
        };

        private const string ParmRoleName = "@RoleName";
        private const string ParmCreatorUsername= "@CreatorUserName";
        private const string ParmDescription = "@Description";

		public string GetRoleDescription(string roleName)
		{
			var roleDescription = string.Empty;
            var sqlString = "SELECT Description FROM siteserver_Role WHERE RoleName = @RoleName";
            var parms = new IDataParameter[]
			{
				GetParameter(ParmRoleName, DataType.VarChar, 255, roleName)
			};

            using (var rdr = ExecuteReader(sqlString, parms))
			{
				if (rdr.Read())
				{
                    roleDescription = GetString(rdr, 0);
                }
				rdr.Close();
			}
			return roleDescription;
		}

		public string GetRolesCreatorUserName(string roleName)
		{
			var creatorUserName = string.Empty;
            var sqlString = "SELECT CreatorUserName FROM siteserver_Role WHERE RoleName = @RoleName";
            var parms = new IDataParameter[]
			{
				GetParameter(ParmRoleName, DataType.VarChar, 255, roleName)
			};

            using (var rdr = ExecuteReader(sqlString, parms)) 
			{
				if (rdr.Read()) 
				{
                    creatorUserName = GetString(rdr, 0);
                }
				rdr.Close();
			}
			return creatorUserName;
		}

        public List<string> GetRoleNameList()
        {
            var list = new List<string>();
            const string sqlSelect = "SELECT RoleName FROM siteserver_Role ORDER BY RoleName";

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    list.Add(GetString(rdr, 0));
                }
                rdr.Close();
            }

            return list;
        }

		public List<string> GetRoleNameListByCreatorUserName(string creatorUserName)
		{
			var list = new List<string>();

		    if (string.IsNullOrEmpty(creatorUserName)) return list;

		    const string sqlString = "SELECT RoleName FROM siteserver_Role WHERE CreatorUserName = @CreatorUserName";
		    var parms = new IDataParameter[]
		    {
		        GetParameter(ParmCreatorUsername, DataType.VarChar, 255, creatorUserName)
		    };

		    using (var rdr = ExecuteReader(sqlString, parms)) 
		    {
		        while (rdr.Read()) 
		        {
		            list.Add(GetString(rdr, 0));
		        }
		        rdr.Close();
		    }
		    return list;
		}

        public void InsertRole(RoleInfo roleInfo)
        {
            if (EPredefinedRoleUtils.IsPredefinedRole(roleInfo.RoleName)) return;

            const string sqlString = "INSERT INTO siteserver_Role (RoleName, CreatorUserName, Description) VALUES (@RoleName, @CreatorUserName, @Description)";

            var parms = new IDataParameter[]
			{
				GetParameter(ParmRoleName, DataType.VarChar, 255, roleInfo.RoleName),
                GetParameter(ParmCreatorUsername, DataType.VarChar, 255, roleInfo.CreatorUserName),
                GetParameter(ParmDescription, DataType.VarChar, 255, roleInfo.Description)
			};

            ExecuteNonQuery(sqlString, parms);
        }

        public virtual void UpdateRole(string roleName, string description) 
		{
            var sqlString = "UPDATE siteserver_Role SET Description = @Description WHERE RoleName = @RoleName";

            var parms = new IDataParameter[]
			{
                GetParameter(ParmDescription, DataType.VarChar, 255, description),
                GetParameter(ParmRoleName, DataType.VarChar, 255, roleName)
			};

            ExecuteNonQuery(sqlString, parms);
		}


		public bool DeleteRole(string roleName)
		{
            var isSuccess = false;
            try
            {
                var sqlString = "DELETE FROM siteserver_Role WHERE RoleName = @RoleName";

                var parms = new IDataParameter[]
			    {
                    GetParameter(ParmRoleName, DataType.VarChar, 255, roleName)
			    };

                ExecuteNonQuery(sqlString, parms);
                isSuccess = true;
            }
		    catch
		    {
		        // ignored
		    }
		    return isSuccess;
		}

        public bool IsRoleExists(string roleName)
        {
            var exists = false;
            var sqlString = "SELECT RoleName FROM siteserver_Role WHERE RoleName = @RoleName";
            var parms = new IDataParameter[]
			{
                GetParameter("@RoleName", DataType.VarChar, 255, roleName)
			};
            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    if (!rdr.IsDBNull(0))
                    {
                        exists = true;
                    }
                }
                rdr.Close();
            }
            return exists;
        }
	}
}
