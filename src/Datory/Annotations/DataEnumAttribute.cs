using System;

namespace Datory.Annotations
{
    /// <summary>
    /// Defines the value and display name of a enum to use in Datory commands.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DataEnumAttribute : Attribute
    {
        /// <summary>
        /// The display name of the enum
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The value of the enum in the database
        /// </summary>
        public string Value { get; set; }
    }
}
