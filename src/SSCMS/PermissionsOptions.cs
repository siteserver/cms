using System;
using System.Collections.Generic;

namespace SSCMS
{
    [Serializable]
    public class PermissionsOptions
    {
        public IList<MenuPermission> App { get; set; }
        public IList<MenuPermission> Site { get; set; }
        public IList<MenuPermission> Channel { get; set; }
        public IList<MenuPermission> Content { get; set; }
    }
}
