using System;

namespace SSCMS.Core.StlParser.Model
{
    [AttributeUsage(AttributeTargets.Field)]
    public class StlAttributeAttribute : Attribute
    {
        public string Title { get; set; }
    }
}
