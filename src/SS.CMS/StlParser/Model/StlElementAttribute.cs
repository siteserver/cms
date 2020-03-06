using System;

namespace SS.CMS.StlParser.Model
{
    [AttributeUsage(AttributeTargets.Class)]
    public class StlElementAttribute : Attribute
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
