using System;

namespace SS.CMS.Data
{
    [AttributeUsage(AttributeTargets.Property)]
    [Serializable]
    public class DataColumnAttribute : Attribute
    {
        public int Length { get; set; }

        public bool Text { get; set; }

        public bool Extend { get; set; }
    }
}
