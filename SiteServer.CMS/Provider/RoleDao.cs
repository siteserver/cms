using System.Collections.Generic;
using System.Data;
using Datory;
using SiteServer.CMS.Data;
using SiteServer.CMS.Model;
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

        private const string ParamId = "@Id";
        private const string ParamRoleName = "@RoleName";
        private const string ParamCreatorUsername= "@CreatorUserName";
        private const string ParamDescription = "@Description";

        public RoleInfo GetRoleInfo(int roleId)
        {
            RoleInfo roleInfo = null;
            var sqlString = "SELECT Id, RoleName, CreatorUserName, Description FROM siteserver_Role WHERE Id = @Id";
            var parameters = new IDataParameter[]
            {
                GetParameter(ParamId, DataType.Integer, roleId)
            };

            using (var rdr = ExecuteReader(sqlString, parameters))
            {
                if (rdr.Read())
                {
                    roleInfo = new RoleInfo
                    {
                        Id = GetInt(rdr, 0),
                        RoleName = GetString(rdr, 1),
                        CreatorUserName = GetString(rdr, 2),
                        Description = GetString(rdr, 3)
                    };
                }
                rdr.Close();
            }
            return roleInfo;
        }

        public List<RoleInfo> GetRoleInfoList()
        {
            var list = new List<RoleInfo>();
            const string sqlSelect = "SELECT Id, RoleName, CreatorUserName, Description FROM siteserver_Role ORDER BY RoleName";

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    list.Add(new RoleInfo
                    {
                        Id = GetInt(rdr, 0),
                        RoleName = GetString(rdr, 1),
                        CreatorUserName = GetString(rdr, 2),
                        Description = GetString(rdr, 3)
                    });
                }
                rdr.Close();
            }

            return list;
        }

        public List<RoleInfo> GetRoleInfoListByCreatorUserName(string creatorUserName)
        {
            var list = new List<RoleInfo>();

            if (string.IsNullOrEmpty(creatorUserName)) return list;

            const string sqlString = "SELECT Id, RoleName, CreatorUserName, Description FROM siteserver_Role WHERE CreatorUserName = @CreatorUserName ORDER BY RoleName";
            var parameters = new IDataParameter[]
            {
                GetParameter(ParamCreatorUsername, DataType.VarChar, 255, creatorUserName)
            };

            using (var rdr = ExecuteReader(sqlString, parameters))
            {
                while (rdr.Read())
                {
                    list.Add(new RoleInfo
                    {
                        Id = GetInt(rdr, 0),
                        RoleName = GetString(rdr, 1),
                        CreatorUserName = GetString(rdr, 2),
                        Description = GetString(rdr, 3)
                    });
                }
                rdr.Close();
            }

            return list;
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
		    var parameters = new IDataParameter[]
		    {
		        GetParameter(ParamCreatorUsername, DataType.VarChar, 255, creatorUserName)
		    };

		    using (var rdr = ExecuteReader(sqlString, parameters)) 
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

            var parameters = new IDataParameter[]
			{
				GetParameter(ParamRoleName, DataType.VarChar, 255, roleInfo.RoleName),
                GetParameter(ParamCreatorUsername, DataType.VarChar, 255, roleInfo.CreatorUserName),
                GetParameter(ParamDescription, DataType.VarChar, 255, roleInfo.Description)
			};

            ExecuteNonQuery(sqlString, parameters);
        }

        public virtual void UpdateRole(RoleInfo roleInfo) 
		{
            var sqlString = "UPDATE siteserver_Role SET RoleName = @RoleName, Description = @Description WHERE Id = @Id";

            var parameters = new IDataParameter[]
			{
                GetParameter(ParamRoleName, DataType.VarChar, 255, roleInfo.RoleName),
                GetParameter(ParamDescription, DataType.VarChar, 255, roleInfo.Description),
                GetParameter(ParamId, DataType.Integer, roleInfo.Id)
			};

            ExecuteNonQuery(sqlString, parameters);
		}

        public bool DeleteRole(int roleId)
		{
            var isSuccess = false;
            try
            {
                var sqlString = "DELETE FROM siteserver_Role WHERE Id = @Id";

                var parameters = new IDataParameter[]
			    {
                    GetParameter(ParamId, DataType.Integer, roleId)
			    };

                ExecuteNonQuery(sqlString, parameters);
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
            var parameters = new IDataParameter[]
			{
                GetParameter("@RoleName", DataType.VarChar, 255, roleName)
			};
            using (var rdr = ExecuteReader(sqlString, parameters))
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
