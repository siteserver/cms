using System;

namespace SiteServer.CMS.StlParser.Model
{
    [AttributeUsage(AttributeTargets.Field)]
    public class StlFieldAttribute : Attribute
    {
        public string Description { get; set; }
    }
}
