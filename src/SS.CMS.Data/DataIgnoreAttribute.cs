using System;

namespace SS.CMS.Data
{
    [AttributeUsage(AttributeTargets.Property)]
    [Serializable]
    public class DataIgnoreAttribute : Attribute
    {
    }
}
