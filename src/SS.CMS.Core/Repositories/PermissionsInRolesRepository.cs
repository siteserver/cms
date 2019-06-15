using System.Collections.Generic;
using SS.CMS.Abstractions.Models;
using SS.CMS.Abstractions.Repositories;
using SS.CMS.Abstractions.Services;
using SS.CMS.Core.Common;
using SS.CMS.Data;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public class PermissionsInRolesRepository : IPermissionsInRolesRepository
    {
        private readonly Repository<PermissionsInRolesInfo> _repository;
        public PermissionsInRolesRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<PermissionsInRolesInfo>(new Db(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
        }

        public IDb Db => _repository.Db;

        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string RoleName = nameof(PermissionsInRolesInfo.RoleName);
        }

        public void Insert(PermissionsInRolesInfo info)
        {
            _repository.Insert(info);
        }

        public bool Delete(string roleName)
        {
            return _repository.Delete(Q
                .Where(Attr.RoleName, roleName)) == 1;
        }

        public void UpdateRoleAndGeneralPermissions(string roleName, string description, List<string> generalPermissionList)
        {
            Delete(roleName);
            if (generalPermissionList != null && generalPermissionList.Count > 0)
            {
                var permissionsInRolesInfo = new PermissionsInRolesInfo
                {
                    RoleName = roleName,
                    GeneralPermissions = TranslateUtils.ObjectCollectionToString(generalPermissionList)
                };
                Insert(permissionsInRolesInfo);
            }

            DataProvider.RoleRepository.UpdateRole(roleName, description);
        }

        private PermissionsInRolesInfo GetPermissionsInRolesInfo(string roleName)
        {
            return _repository.Get(Q.Where(Attr.RoleName, roleName));
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
                    var permissionList =
                        TranslateUtils.StringCollectionToStringList(permissionsInRolesInfo.GeneralPermissions);
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
