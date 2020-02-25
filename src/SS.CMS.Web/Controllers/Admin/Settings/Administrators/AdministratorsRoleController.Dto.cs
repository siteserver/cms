using System.Collections.Generic;
using SS.CMS.Abstractions;

namespace SS.CMS.Web.Controllers.Admin.Settings.Administrators
{
    public partial class AdministratorsRoleController
    {
        public class ListRequest
        {
            public List<Role> Roles { get; set; }
        }
    }
}
