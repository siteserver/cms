using System;
using System.Collections.Generic;

namespace SSCMS
{
    [Serializable]
    public class PermissionsOptions
    {
        public IList<Permission> App { get; set; }
        public IList<Permission> Site { get; set; }
        public IList<Permission> Channel { get; set; }
        public IList<Permission> Content { get; set; }
    }
}
