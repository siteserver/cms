using System;
using System.Collections.Generic;

namespace SS.CMS.Models
{
    [Serializable]
    public class PermissionsSettings
    {
        public IList<Permission> App { get; set; }
        public IList<Permission> Site { get; set; }
        public IList<Permission> Channel { get; set; }
    }
}