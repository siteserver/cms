using System;
using System.Linq;
using System.Reflection;
using Datory.Annotations;

namespace Datory
{
    /// <summary>
    /// https://stackoverflow.com/a/35273581
    /// </summary>
    public static class EnumExtensions
    {
        /// returns the localized Name, if a [Display] attribute is applied to the enum member
        /// returns null if there is no attribute
        public static string GetDisplayName(this Enum value) =>
            value.GetEnumMemberAttribute<DataEnumAttribute>()?.DisplayName;

        public static string GetValue(this Enum value) => value.GetEnumMemberAttribute<DataEnumAttribute>()?.Value ?? Enum.GetName(value.GetType(), value);

        public static string ToString(this Enum value) => value.GetEnumMemberAttribute<DataEnumAttribute>()?.Value ?? Enum.GetName(value.GetType(), value);

        private static TAttribute GetEnumMemberAttribute<TAttribute>(this Enum value) where TAttribute : Attribute =>
            value.GetType().GetEnumMemberAttribute<TAttribute>(value.ToString());

        private static TAttribute GetEnumMemberAttribute<TAttribute>(this Type enumType, string enumMemberName)
            where TAttribute : Attribute =>
            enumType.GetMember(enumMemberName).Single().GetCustomAttribute<TAttribute>();
    }
}
