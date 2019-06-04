using System.Collections.Generic;

namespace SS.CMS.Core.StlParser.Models
{
    public partial class Container
    {
        public class Sql
        {
            public int ItemIndex { get; set; }

            public Dictionary<string, object> Dictionary { get; set; }
        }

    }
}