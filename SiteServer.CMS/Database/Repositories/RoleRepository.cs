using System.Collections.Generic;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Database.Repositories
{
    public class RoleRepository : GenericRepository<RoleInfo>
    {
        private static class Attr
        {
            public const string RoleName = nameof(RoleInfo.RoleName);
            public const string CreatorUserName = nameof(RoleInfo.CreatorUserName);
            public const string Description = nameof(RoleInfo.Description);
        }

        public string GetRoleDescription(string roleName)
        {
            //var roleDescription = string.Empty;
            //const string sqlString = "SELECT Description FROM siteserver_Role WHERE RoleName = @RoleName";
            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamRoleName, roleName)
            //};

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
            //{
            //    if (rdr.Read())
            //    {
            //        roleDescription = DatabaseApi.GetString(rdr, 0);
            //    }
            //    rdr.Close();
            //}
            //return roleDescription;

            return GetValue<string>(Q
                .Select(Attr.Description)
                .Where(Attr.RoleName, roleName));
        }

        public IList<string> GetRoleNameList()
        {
            //var list = new List<string>();
            //const string sqlSelect = "SELECT RoleName FROM siteserver_Role ORDER BY RoleName";

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlSelect))
            //{
            //    while (rdr.Read())
            //    {
            //        list.Add(DatabaseApi.GetString(rdr, 0));
            //    }
            //    rdr.Close();
            //}

            //return list;

            return GetValueList<string>(Q
                .Select(Attr.RoleName)
                .OrderBy(Attr.RoleName));
        }

        public IList<string> GetRoleNameListByCreatorUserName(string creatorUserName)
        {
            //var list = new List<string>();

            //if (string.IsNullOrEmpty(creatorUserName)) return list;

            //const string sqlString = "SELECT RoleName FROM siteserver_Role WHERE CreatorUserName = @CreatorUserName";
            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamCreatorUsername, creatorUserName)
            //};

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
            //{
            //    while (rdr.Read())
            //    {
            //        list.Add(DatabaseApi.GetString(rdr, 0));
            //    }
            //    rdr.Close();
            //}
            //return list;

            if (string.IsNullOrEmpty(creatorUserName)) return new List<string>();

            return GetValueList<string>(Q
                .Select(Attr.RoleName)
                .Where(Attr.CreatorUserName, creatorUserName)
                .OrderBy(Attr.RoleName));
        }

        public void InsertRole(RoleInfo roleInfo)
        {
            if (EPredefinedRoleUtils.IsPredefinedRole(roleInfo.RoleName)) return;

            //const string sqlString = "INSERT INTO siteserver_Role (RoleName, CreatorUserName, Description) VALUES (@RoleName, @CreatorUserName, @Description)";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamRoleName, roleInfo.RoleName),
            //    GetParameter(ParamCreatorUsername, roleInfo.CreatorUserName),
            //    GetParameter(ParamDescription, roleInfo.Description)
            //};

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

            InsertObject(roleInfo);
        }

        public void UpdateRole(string roleName, string description)
        {
            //const string sqlString = "UPDATE siteserver_Role SET Description = @Description WHERE RoleName = @RoleName";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamDescription, description),
            //    GetParameter(ParamRoleName, roleName)
            //};

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);
            
            UpdateAll(Q
                .Set(Attr.Description, description)
                .Where(Attr.RoleName, roleName)
            );
        }


        public void DeleteRole(string roleName)
        {
            //var isSuccess = false;
            //try
            //{
            //    const string sqlString = "DELETE FROM siteserver_Role WHERE RoleName = @RoleName";

            //    IDataParameter[] parameters =
            //    {
            //        GetParameter(ParamRoleName, roleName)
            //    };

            //    DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);
            //    isSuccess = true;
            //}
            //catch
            //{
            //    // ignored
            //}
            //return isSuccess;

            DeleteAll(Q.Where(Attr.RoleName, roleName));
        }

        public bool IsRoleExists(string roleName)
        {
            //var exists = false;
            //const string sqlString = "SELECT RoleName FROM siteserver_Role WHERE RoleName = @RoleName";
            //IDataParameter[] parameters =
            //{
            //    GetParameter("@RoleName", roleName)
            //};
            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
            //{
            //    if (rdr.Read())
            //    {
            //        if (!rdr.IsDBNull(0))
            //        {
            //            exists = true;
            //        }
            //    }
            //    rdr.Close();
            //}
            //return exists;

            return Exists(Q.Where(Attr.RoleName, roleName));
        }
    }
}


//using System.Collections.Generic;
//using System.Data;
//using SiteServer.CMS.Database.Core;
//using SiteServer.CMS.Database.Models;
//using SiteServer.Plugin;
//using SiteServer.Utils.Enumerations;

//namespace SiteServer.CMS.Database.Repositories
//{
//    public class Role : DataProviderBase
//	{
//        public override string TableName => "siteserver_Role";

//        public override List<TableColumn> TableColumns => new List<TableColumn>
//        {
//            new TableColumn
//            {
//                AttributeName = nameof(RoleInfo.Id),
//                DataType = DataType.Integer,
//                IsIdentity = true,
//                IsPrimaryKey = true
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(RoleInfo.RoleName),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(RoleInfo.CreatorUserName),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(RoleInfo.Description),
//                DataType = DataType.VarChar
//            }
//        };

//        private const string ParamRoleName = "@RoleName";
//        private const string ParamCreatorUsername= "@CreatorUserName";
//        private const string ParamDescription = "@Description";

//		public string GetRoleDescription(string roleName)
//		{
//			var roleDescription = string.Empty;
//            const string sqlString = "SELECT Description FROM siteserver_Role WHERE RoleName = @RoleName";
//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamRoleName, roleName)
//			};

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
//			{
//				if (rdr.Read())
//				{
//                    roleDescription = DatabaseApi.GetString(rdr, 0);
//                }
//				rdr.Close();
//			}
//			return roleDescription;
//		}

//        public List<string> GetRoleNameList()
//        {
//            var list = new List<string>();
//            const string sqlSelect = "SELECT RoleName FROM siteserver_Role ORDER BY RoleName";

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlSelect))
//            {
//                while (rdr.Read())
//                {
//                    list.Add(DatabaseApi.GetString(rdr, 0));
//                }
//                rdr.Close();
//            }

//            return list;
//        }

//		public List<string> GetRoleNameListByCreatorUserName(string creatorUserName)
//		{
//			var list = new List<string>();

//		    if (string.IsNullOrEmpty(creatorUserName)) return list;

//		    const string sqlString = "SELECT RoleName FROM siteserver_Role WHERE CreatorUserName = @CreatorUserName";
//		    IDataParameter[] parameters =
//		    {
//		        GetParameter(ParamCreatorUsername, creatorUserName)
//		    };

//		    using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters)) 
//		    {
//		        while (rdr.Read()) 
//		        {
//		            list.Add(DatabaseApi.GetString(rdr, 0));
//		        }
//		        rdr.Close();
//		    }
//		    return list;
//		}

//        public void InsertRole(RoleInfo roleInfo)
//        {
//            if (EPredefinedRoleUtils.IsPredefinedRole(roleInfo.RoleName)) return;

//            const string sqlString = "INSERT INTO siteserver_Role (RoleName, CreatorUserName, Description) VALUES (@RoleName, @CreatorUserName, @Description)";

//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamRoleName, roleInfo.RoleName),
//                GetParameter(ParamCreatorUsername, roleInfo.CreatorUserName),
//                GetParameter(ParamDescription, roleInfo.Description)
//			};

//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);
//        }

//        public virtual void UpdateRole(string roleName, string description) 
//		{
//            const string sqlString = "UPDATE siteserver_Role SET Description = @Description WHERE RoleName = @RoleName";

//            IDataParameter[] parameters =
//			{
//                GetParameter(ParamDescription, description),
//                GetParameter(ParamRoleName, roleName)
//			};

//		    DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);
//		}


//		public bool DeleteRole(string roleName)
//		{
//            var isSuccess = false;
//            try
//            {
//                const string sqlString = "DELETE FROM siteserver_Role WHERE RoleName = @RoleName";

//                IDataParameter[] parameters =
//			    {
//                    GetParameter(ParamRoleName, roleName)
//			    };

//                DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);
//                isSuccess = true;
//            }
//		    catch
//		    {
//		        // ignored
//		    }
//		    return isSuccess;
//		}

//        public bool IsRoleExists(string roleName)
//        {
//            var exists = false;
//            const string sqlString = "SELECT RoleName FROM siteserver_Role WHERE RoleName = @RoleName";
//            IDataParameter[] parameters =
//			{
//                GetParameter("@RoleName", roleName)
//			};
//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
//            {
//                if (rdr.Read())
//                {
//                    if (!rdr.IsDBNull(0))
//                    {
//                        exists = true;
//                    }
//                }
//                rdr.Close();
//            }
//            return exists;
//        }
//	}
//}
