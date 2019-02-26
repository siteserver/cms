using System;

namespace SiteServer.CMS.Database.Core
{
    /// <summary>
    /// Specifies that this field is a primary key in the database
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class VarCharAttribute : Attribute
    {
        /// <summary>
        /// Creates a table mapping to a specific name for Dapper.Contrib commands
        /// </summary>
        /// <param name="length">The length of this column in the database.</param>
        public VarCharAttribute(int length)
        {
            Length = length;
        }

        /// <summary>
        /// The length of the column in the database
        /// </summary>
        public int Length { get; set; }
    }
}
