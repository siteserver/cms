using System.Collections.Generic;

namespace SS.CMS.Abstractions.Settings
{
    public class PermissionsSettings
    {
        public IList<Permission> App { get; set; }
        public IList<Permission> Site { get; set; }
        public IList<Permission> Channel { get; set; }
    }
}