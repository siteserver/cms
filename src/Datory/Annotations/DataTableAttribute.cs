using System;

namespace Datory.Annotations
{
    /// <summary>
    /// Defines the name of a table to use in Datory commands.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DataTableAttribute : Attribute
    {
        /// <summary>
        /// Creates a table mapping to a specific name for Datory commands
        /// </summary>
        /// <param name="tableName">The name of this table in the database.</param>
        public DataTableAttribute(string tableName)
        {
            Name = tableName;
        }

        /// <summary>
        /// The name of the table in the database
        /// </summary>
        public string Name { get; set; }
    }
}
