using System;
using System.Collections.Generic;

namespace SSCMS
{
    [Serializable]
    public class MenusOptions
    {
        public IList<Menu> App { get; set; }
        public IList<Menu> Site { get; set; }
        public IList<Menu> Channel { get; set; }
        public IList<Menu> Content { get; set; }
    }
}
