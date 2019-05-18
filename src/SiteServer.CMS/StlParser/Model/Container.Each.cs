using System;
using System.Collections.Specialized;
using System.Text;
using SiteServer.CMS.Model.Attributes;
using SiteServer.Utils;

namespace SiteServer.CMS.StlParser.Model
{
    public partial class Container
    {
        public class Each
        {
            public int ItemIndex { get; set; }

            public object Value { get; set; }
        }

    }
}