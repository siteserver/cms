using System.Collections.Generic;
using SSCMS.Abstractions;

namespace SSCMS.Controllers.Admin.Settings.Users
{
    public partial class UsersGroupController
    {
        public class GetResult
        {
            public IEnumerable<UserGroup> Groups { get; set; }
            public IEnumerable<string> AdminNames { get; set; }
        }
    }
}