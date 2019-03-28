using System.Collections.Generic;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;

namespace SiteServer.CMS.Database.Repositories
{
    public class AdministratorsInRolesRepository : GenericRepository<AdministratorsInRolesInfo>
    {
        //public override string TableName => "siteserver_AdministratorsInRoles";

        //public override List<TableColumn> TableColumns => new List<TableColumn>
        //{
        //    new TableColumn
        //    {
        //        AttributeName = nameof(AdministratorsInRolesInfo.Id),
        //        DataType = DataType.Integer,
        //        IsIdentity = true,
        //        IsPrimaryKey = true
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = nameof(AdministratorsInRolesInfo.RoleName),
        //        DataType = DataType.VarChar
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = nameof(AdministratorsInRolesInfo.UserName),
        //        DataType = DataType.VarChar
        //    }
        //};

        private static class Attr
        {
            public const string Id = nameof(AdministratorsInRolesInfo.Id);
            public const string Guid = nameof(AdministratorsInRolesInfo.Guid);
            public const string LastModifiedDate = nameof(AdministratorsInRolesInfo.LastModifiedDate);
            public const string RoleName = nameof(AdministratorsInRolesInfo.RoleName);
            public const string UserName = nameof(AdministratorsInRolesInfo.UserName);
        }

        public IEnumerable<string> GetUserNameListByRoleName(string roleName)
        {
            return GetValueList<string>(Q
                .Select(Attr.UserName)
                .Where(Attr.RoleName, roleName)
                .Distinct());
            //var list = new List<string>();

            //const string sqlString = "SELECT DISTINCT UserName FROM siteserver_AdministratorsInRoles WHERE RoleName = @RoleName";

            //IDataParameter[] parameters =
            //{
            //    GetParameter("@RoleName", roleName)
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
        }

        public IList<string> GetRolesForUser(string userName)
        {
            return GetValueList<string>(Q
                .Select(Attr.RoleName)
                .Where(Attr.UserName, userName)
                .Distinct());
            //var tmpRoleNames = string.Empty;
            //const string sqlString = "SELECT RoleName FROM siteserver_AdministratorsInRoles WHERE UserName = @UserName ORDER BY RoleName";

            //IDataParameter[] parameters =
            //{
            //    GetParameter("@UserName", userName)
            //};

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
            //{
            //    while (rdr.Read())
            //    {
            //        tmpRoleNames += DatabaseApi.GetString(rdr, 0) + ",";
            //    }
            //    rdr.Close();
            //}

            //if (tmpRoleNames.Length > 0)
            //{
            //    tmpRoleNames = tmpRoleNames.Substring(0, tmpRoleNames.Length - 1);
            //    return tmpRoleNames.Split(',');
            //}

            //return new string[0];
        }

        public void RemoveUser(string userName)
        {
            DeleteAll(Q
                .Where(Attr.UserName, userName));
            //const string sqlString = "DELETE FROM siteserver_AdministratorsInRoles WHERE UserName = @UserName";
            //IDataParameter[] parameters =
            //{
            //    GetParameter("@UserName", userName)
            //};
            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);
        }

        //public void RemoveUserFromRoles(string userName, string[] roleNames)
        //{
        //    foreach (var roleName in roleNames)
        //    {
        //        DeleteAll(new Query()
        //            .Equal(Attr.UserName, userName)
        //            .Equal(Attr.RoleName, roleName));
        //    }
        //    //const string sqlString = "DELETE FROM siteserver_AdministratorsInRoles WHERE UserName = @UserName AND RoleName = @RoleName";
        //    //foreach (var roleName in roleNames)
        //    //{
        //    //    IDataParameter[] parameters =
        //    //    {
        //    //        GetParameter("@UserName", userName),
        //    //        GetParameter("@RoleName", roleName)
        //    //    };
        //    //    DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);
        //    //}
        //}

        public void RemoveUserFromRole(string userName, string roleName)
        {
            DeleteAll(Q
                .Where(Attr.UserName, userName)
                .Where(Attr.RoleName, roleName));

            //const string sqlString = "DELETE FROM siteserver_AdministratorsInRoles WHERE UserName = @UserName AND RoleName = @RoleName";

            //IDataParameter[] parameters =
            //{
            //    GetParameter("@UserName", userName),
            //    GetParameter("@RoleName", roleName)
            //};

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);
        }
        
        public bool IsUserInRole(string userName, string roleName)
        {
            return Exists(Q
                .Where(Attr.UserName, userName)
                .Where(Attr.RoleName, roleName));

            //var isUserInRole = false;
            //const string sqlString = "SELECT * FROM siteserver_AdministratorsInRoles WHERE UserName = @UserName AND RoleName = @RoleName";

            //IDataParameter[] parameters = 
            //{
            //    GetParameter("@UserName", userName),
            //    GetParameter("@RoleName", roleName)
            //};
            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
            //{
            //    if (rdr.Read())
            //    {
            //        if (!rdr.IsDBNull(0))
            //        {
            //            isUserInRole = true;
            //        }
            //    }
            //    rdr.Close();
            //}
            //return isUserInRole;
        }

        public int AddUserToRole(string userName, string roleName)
        {
            if (!DataProvider.Administrator.IsUserNameExists(userName)) return 0;
            if (!IsUserInRole(userName, roleName))
            {
                return InsertObject(new AdministratorsInRolesInfo
                {
                    UserName = userName,
                    RoleName = roleName
                });
                //const string sqlString = "INSERT INTO siteserver_AdministratorsInRoles (UserName, RoleName) VALUES (@UserName, @RoleName)";

                //IDataParameter[] parameters = 
                //{
                //    GetParameter("@UserName", userName),
                //    GetParameter("@RoleName", roleName)
                //};

                //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);
            }

            return 0;
        }
    }
}
