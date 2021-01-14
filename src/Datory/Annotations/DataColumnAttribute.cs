using System;

namespace Datory.Annotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DataColumnAttribute : Attribute
    {
        public int Length { get; set; }

        public bool Text { get; set; }
    }
}
