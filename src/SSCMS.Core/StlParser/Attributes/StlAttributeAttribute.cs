using System;

namespace SSCMS.Core.StlParser.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class StlAttributeAttribute : Attribute
    {
        public string Title { get; set; }
    }
}
