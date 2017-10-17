using System.Collections;
using System.Data;
using BaiRong.Core.Data;
using SiteServer.Plugin.Models;

namespace BaiRong.Core.Provider
{
    public class RoleDao : DataProviderBase
	{
        private const string ParmRoleName = "@RoleName";
        private const string ParmCreatorUsername= "@CreatorUserName";
        private const string ParmDescription = "@Description";

		public string GetRoleDescription(string roleName)
		{
			var roleDescription = string.Empty;
            var sqlString = "SELECT Description FROM bairong_Roles WHERE RoleName = @RoleName";
            var parms = new IDataParameter[]
			{
				GetParameter(ParmRoleName, DataType.NVarChar, 255, roleName)
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
            var sqlString = "SELECT CreatorUserName FROM bairong_Roles WHERE RoleName = @RoleName";
            var parms = new IDataParameter[]
			{
				GetParameter(ParmRoleName, DataType.NVarChar, 255, roleName)
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

        public string[] GetAllRoles()
        {
            var tmpUserNames = string.Empty;
            const string sqlSelect = "SELECT RoleName FROM bairong_Roles ORDER BY RoleName";

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    tmpUserNames += GetString(rdr, 0) + ",";
                }
                rdr.Close();
            }

            if (tmpUserNames.Length > 0)
            {
                tmpUserNames = tmpUserNames.Substring(0, tmpUserNames.Length - 1);
                return tmpUserNames.Split(',');
            }

            return new string[0];
        }

		public ArrayList GetRoleNameArrayListByCreatorUserName(string creatorUserName)
		{
			var arraylist = new ArrayList();

			if (!string.IsNullOrEmpty(creatorUserName))
			{
                var sqlString = "SELECT RoleName FROM bairong_Roles WHERE CreatorUserName = @CreatorUserName";
                var parms = new IDataParameter[]
			    {
				    GetParameter(ParmCreatorUsername, DataType.NVarChar, 255, creatorUserName)
			    };

                using (var rdr = ExecuteReader(sqlString, parms)) 
				{
					while (rdr.Read()) 
					{
                        arraylist.Add(GetString(rdr, 0));
					}
					rdr.Close();
				}
			}
			return arraylist;
		}

		public string[] GetAllRolesByCreatorUserName(string creatorUserName)
		{
			var roleNameArrayList = GetRoleNameArrayListByCreatorUserName(creatorUserName);
			var roleArray = new string[roleNameArrayList.Count];
			roleNameArrayList.CopyTo(roleArray);
			return roleArray;
		}

        public void InsertRole(string roleName, string creatorUserName, string description)
        {
            var sqlString = "INSERT INTO bairong_Roles (RoleName, CreatorUserName, Description) VALUES (@RoleName, @CreatorUserName, @Description)";

            var parms = new IDataParameter[]
			{
				GetParameter(ParmRoleName, DataType.NVarChar, 255, roleName),
                GetParameter(ParmCreatorUsername, DataType.NVarChar, 255, creatorUserName),
                GetParameter(ParmDescription, DataType.NVarChar, 255, description)
			};

            ExecuteNonQuery(sqlString, parms);
        }

        public virtual void UpdateRole(string roleName, string description) 
		{
            var sqlString = "UPDATE bairong_Roles SET Description = @Description WHERE RoleName = @RoleName";

            var parms = new IDataParameter[]
			{
                GetParameter(ParmDescription, DataType.NVarChar, 255, description),
                GetParameter(ParmRoleName, DataType.NVarChar, 255, roleName)
			};

            ExecuteNonQuery(sqlString, parms);
		}


		public bool DeleteRole(string roleName)
		{
            var isSuccess = false;
            try
            {
                var sqlString = "DELETE FROM bairong_Roles WHERE RoleName = @RoleName";

                var parms = new IDataParameter[]
			    {
                    GetParameter(ParmRoleName, DataType.NVarChar, 255, roleName)
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
            var sqlString = "SELECT RoleName FROM bairong_Roles WHERE RoleName = @RoleName";
            var parms = new IDataParameter[]
			{
                GetParameter("@RoleName", DataType.NVarChar, 255, roleName)
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
