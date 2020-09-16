using System.Collections.Generic;
using System.Linq;
using SSCMS.Configuration;

namespace SSCMS.Core.Services
{
    public partial class AuthManager
    {
        public bool IsMenuValid(Menu menu, IList<string> permissions)
        {
            if (string.IsNullOrEmpty(menu.Text)) return false;
            if (menu.Permissions == null || menu.Permissions.Count <= 0) return true;

            if (permissions != null && permissions.Count > 0)
            {
                return menu.Permissions.Any(permissions.Contains);
            }

            return false;
        }
    }
}
