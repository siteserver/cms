using SiteServer.Utils.Enumerations;
using System.Collections.Generic;
using System.Linq;
using SiteServer.CMS.Database.Core;

namespace SiteServer.CMS.Core
{
    public static class RoleManager
    {
        public static List<KeyValuePair<string, string>> GetRestRoles(bool isSuperAdmin, string adminName)
        {
            var list = new List<KeyValuePair<string, string>>();

            IList<string> roleNames;
            if (isSuperAdmin)
            {
                foreach (var predefinedRuleName in EPredefinedRoleUtils.GetAllPredefinedRoleName())
                {
                    list.Add(new KeyValuePair<string, string>(predefinedRuleName, EPredefinedRoleUtils.GetText(EPredefinedRoleUtils.GetEnumType(predefinedRuleName))));
                }

                roleNames = DataProvider.Role.GetRoleNameList();
            }
            else
            {
                roleNames = DataProvider.Role.GetRoleNameListByCreatorUserName(adminName);
            }

            foreach (var roleName in roleNames)
            {
                if (list.Any(x=> x.Key == roleName)) continue;

                list.Add(new KeyValuePair<string, string>(roleName, roleName));
            }

            return list;
        }
    }
}
