using System;
using System.Collections.Specialized;
using System.Text;
using SiteServer.CMS.Model.Attributes;
using SiteServer.Utils;

namespace SiteServer.CMS.StlParser.Model
{
    public partial class Container
    {
        public class Site
        {
            public static readonly string SqlColumns = $"{SiteAttribute.Id}, {SiteAttribute.IsRoot}, {SiteAttribute.ParentId}, {SiteAttribute.Taxis}";

            public int ItemIndex { get; set; }

            public int Id { get; set; }
        }

    }
}