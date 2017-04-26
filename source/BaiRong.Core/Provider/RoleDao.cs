using System.Collections;
using System.Data;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;

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
				GetParameter(ParmRoleName, EDataType.NVarChar, 255, roleName)
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
				GetParameter(ParmRoleName, EDataType.NVarChar, 255, roleName)
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
				    GetParameter(ParmCreatorUsername, EDataType.NVarChar, 255, creatorUserName)
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

        public string[] GetRolesForUser(string userName)
        {
            var tmpRoleNames = string.Empty;
            var sqlString = "SELECT RoleName FROM bairong_AdministratorsInRoles WHERE UserName = @UserName ORDER BY RoleName";
            var parms = new IDataParameter[]
			{
				GetParameter("@UserName", EDataType.NVarChar, 255, userName)
			};

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                while (rdr.Read())
                {
                    tmpRoleNames += GetString(rdr, 0) + ",";
                }
                rdr.Close();
            }

            if (tmpRoleNames.Length > 0)
            {
                tmpRoleNames = tmpRoleNames.Substring(0, tmpRoleNames.Length - 1);
                return tmpRoleNames.Split(',');
            }

            return new string[0];
        }

        public string[] GetUsersInRole(string roleName)
        {
            var tmpUserNames = string.Empty;
            var sqlString = "SELECT UserName FROM bairong_AdministratorsInRoles WHERE RoleName = @RoleName ORDER BY userName";
            var parms = new IDataParameter[]
			{
				GetParameter(ParmRoleName, EDataType.NVarChar, 255, roleName)
			};

            using (var rdr = ExecuteReader(sqlString, parms))
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

        public void RemoveUserFromRoles(string userName, string[] roleNames)
        {
            var sqlString = "DELETE FROM bairong_AdministratorsInRoles WHERE UserName = @UserName AND RoleName = @RoleName";
            foreach (var roleName in roleNames)
            {
                var parms = new IDataParameter[]
			    {
				    GetParameter("@UserName", EDataType.NVarChar, 255, userName),
                    GetParameter("@RoleName", EDataType.NVarChar, 255, roleName)
			    };
                ExecuteNonQuery(sqlString, parms);
            }
        }

        public void RemoveUserFromRole(string userName, string roleName)
        {
            var sqlString = "DELETE FROM bairong_AdministratorsInRoles WHERE UserName = @UserName AND RoleName = @RoleName";
            var parms = new IDataParameter[]
			{
				GetParameter("@UserName", EDataType.NVarChar, 255, userName),
                GetParameter("@RoleName", EDataType.NVarChar, 255, roleName)
			};

            ExecuteNonQuery(sqlString, parms);
        }

        public void InsertRole(string roleName, string creatorUserName, string description)
        {
            var sqlString = "INSERT INTO bairong_Roles (RoleName, CreatorUserName, Description) VALUES (@RoleName, @CreatorUserName, @Description)";

            var parms = new IDataParameter[]
			{
				GetParameter(ParmRoleName, EDataType.NVarChar, 255, roleName),
                GetParameter(ParmCreatorUsername, EDataType.NVarChar, 255, creatorUserName),
                GetParameter(ParmDescription, EDataType.NVarChar, 255, description)
			};

            ExecuteNonQuery(sqlString, parms);
        }

        public virtual void UpdateRole(string roleName, string description) 
		{
            var sqlString = "UPDATE bairong_Roles SET Description = @Description WHERE RoleName = @RoleName";

            var parms = new IDataParameter[]
			{
                GetParameter(ParmDescription, EDataType.NVarChar, 255, description),
                GetParameter(ParmRoleName, EDataType.NVarChar, 255, roleName)
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
                    GetParameter(ParmRoleName, EDataType.NVarChar, 255, roleName)
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
                GetParameter("@RoleName", EDataType.NVarChar, 255, roleName)
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

        public string[] FindUsersInRole(string roleName, string userNameToMatch)
        {
            var tmpUserNames = string.Empty;
            string sqlString =
                $"SELECT UserName FROM bairong_AdministratorsInRoles WHERE RoleName = @RoleName AND UserName LIKE '%{PageUtils.FilterSql(userNameToMatch)}%'";

            var parms = new IDataParameter[]
			{
                GetParameter("@RoleName", EDataType.NVarChar, 255, roleName)
			};

            using (var rdr = ExecuteReader(sqlString, parms))
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

        public bool IsUserInRole(string userName, string roleName)
        {
            var isUserInRole = false;
            var sqlString = "SELECT * FROM bairong_AdministratorsInRoles WHERE UserName = @UserName AND RoleName = @RoleName";
            var parms = new IDataParameter[]
			{
                GetParameter("@UserName", EDataType.NVarChar, 255, userName),
                GetParameter("@RoleName", EDataType.NVarChar, 255, roleName)
			};
            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    if (!rdr.IsDBNull(0))
                    {
                        isUserInRole = true;
                    }
                }
                rdr.Close();
            }
            return isUserInRole;
        }

        public void AddUserToRoles(string userName, string[] roleNames)
        {
            foreach (var roleName in roleNames)
            {
                AddUserToRole(userName, roleName);
            }
        }

        public void AddUserToRole(string userName, string roleName)
        {
            if (!IsRoleExists(roleName)) return;
            if (!BaiRongDataProvider.AdministratorDao.IsUserNameExists(userName)) return;
            if (!IsUserInRole(userName, roleName))
            {
                var sqlString = "INSERT INTO bairong_AdministratorsInRoles (UserName, RoleName) VALUES (@UserName, @RoleName)";

                var parms = new IDataParameter[]
			    {
                    GetParameter("@UserName", EDataType.NVarChar, 255, userName),
                    GetParameter("@RoleName", EDataType.NVarChar, 255, roleName)
			    };

                ExecuteNonQuery(sqlString, parms);
            }
        }
	}
}
