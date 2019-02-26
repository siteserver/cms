using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SiteServer.Utils
{
    public static class ReflectionUtils
    {
        public static PropertyInfo[] GetAllInstancePropertyInfos(Type type)
        {
            return type.GetProperties(
                BindingFlags.NonPublic
                | BindingFlags.Public
                | BindingFlags.Instance);
        }

        public static IList<KeyValuePair<string, object>> ToKeyValueList(object parameters)
        {
            if (parameters == null) return new List<KeyValuePair<string, object>>();

            var type = parameters.GetType();
            var props = type.GetProperties();
            return props.Select(x => new KeyValuePair<string, object>(x.Name, x.GetValue(parameters, null))).ToList();
        }

        public static T GetValue<T>(object obj, string propertyName)
        {
            var value = GetValue(obj, propertyName);
            switch (value)
            {
                case null:
                    return default(T);
                case T variable:
                    return variable;
                default:
                    return default(T);
            }
        }

        public static object GetValue(object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName)?.GetValue(obj, null);
        }
    }
}
