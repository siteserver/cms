using System;

namespace SS.CMS.Core.StlParser.Models
{
    [AttributeUsage(AttributeTargets.Class)]
    public class StlElementAttribute : Attribute
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
