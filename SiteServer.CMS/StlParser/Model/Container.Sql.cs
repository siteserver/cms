using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using SiteServer.CMS.Model.Attributes;
using SiteServer.Utils;

namespace SiteServer.CMS.StlParser.Model
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