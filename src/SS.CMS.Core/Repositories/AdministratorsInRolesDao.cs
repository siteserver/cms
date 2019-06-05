using System.Collections.Generic;
using SS.CMS.Core.Common;
using SS.CMS.Core.Models;
using SS.CMS.Data;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public class AdministratorsInRolesDao : IDatabaseDao
    {
        private readonly Repository<AdministratorsInRolesInfo> _repository;
        public AdministratorsInRolesDao()
        {
            _repository = new Repository<AdministratorsInRolesInfo>(AppSettings.DatabaseType, AppSettings.ConnectionString);
        }

        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

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
            return _repository.GetAll<string>(Q
                .Select(Attr.UserName)
                .Where(Attr.RoleName, roleName)
                .Distinct());
        }

        public IList<string> GetRolesForUser(string userName)
        {
            return _repository.GetAll<string>(Q
                .Select(Attr.RoleName)
                .Where(Attr.UserName, userName)
                .Distinct());
        }

        public void RemoveUser(string userName)
        {
            _repository.Delete(Q
                .Where(Attr.UserName, userName));
        }

        public void RemoveUserFromRole(string userName, string roleName)
        {
            _repository.Delete(Q
                .Where(Attr.UserName, userName)
                .Where(Attr.RoleName, roleName));
        }

        public bool IsUserInRole(string userName, string roleName)
        {
            return _repository.Exists(Q
                .Where(Attr.UserName, userName)
                .Where(Attr.RoleName, roleName));
        }

        public int AddUserToRole(string userName, string roleName)
        {
            if (!DataProvider.AdministratorDao.IsUserNameExists(userName)) return 0;
            if (!IsUserInRole(userName, roleName))
            {
                return _repository.Insert(new AdministratorsInRolesInfo
                {
                    UserName = userName,
                    RoleName = roleName
                });
            }

            return 0;
        }
    }
}
