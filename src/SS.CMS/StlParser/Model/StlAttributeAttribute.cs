using System;

namespace SS.CMS.StlParser.Model
{
    [AttributeUsage(AttributeTargets.Field)]
    public class StlAttributeAttribute : Attribute
    {
        public string Title { get; set; }
    }
}
