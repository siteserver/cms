using System;

namespace SiteServer.CMS.StlParser.Model
{
    [AttributeUsage(AttributeTargets.Class)]
    public class StlClassAttribute : Attribute
    {
        public string Usage { get; set; }
        public string Description { get; set; }
        public bool Obsolete { get; set; }
    }
}
