using System;

namespace SiteServer.CMS.Database.Core
{
    /// <summary>
    /// Specifies that this field is a primary key in the database
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TextAttribute : Attribute
    {
    }
}
