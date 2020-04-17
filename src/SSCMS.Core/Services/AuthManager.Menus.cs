using System.Collections.Generic;
using System.Linq;

namespace SSCMS.Core.Services
{
    public partial class AuthManager
    {
        public bool IsMenuValid(Menu menu, IList<string> permissions)
        {
            if (string.IsNullOrEmpty(menu.Text) || menu.Permissions == null || menu.Permissions.Count <= 0) return false;

            if (permissions != null && permissions.Count > 0)
            {
                return menu.Permissions.Any(permissions.Contains);
            }

            return false;
        }
    }
}
