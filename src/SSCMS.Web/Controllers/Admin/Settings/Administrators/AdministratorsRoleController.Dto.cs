using System.Collections.Generic;
using SSCMS;

namespace SSCMS.Web.Controllers.Admin.Settings.Administrators
{
    public partial class AdministratorsRoleController
    {
        public class ListRequest
        {
            public List<Role> Roles { get; set; }
        }
    }
}
