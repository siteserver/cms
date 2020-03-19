using System.Collections.Generic;
using SSCMS.Abstractions;

namespace SSCMS.Controllers.Admin.Settings.Administrators
{
    public partial class AdministratorsRoleController
    {
        public class ListRequest
        {
            public List<Role> Roles { get; set; }
        }
    }
}
