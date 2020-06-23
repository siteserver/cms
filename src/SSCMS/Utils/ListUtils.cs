using System;
using System.Collections.Generic;
using System.Linq;
using Datory.Utils;
using SSCMS.Dto;

namespace SSCMS.Utils
{
    public static class ListUtils
    {
        public static bool Contains(string strCollection, int inInt)
        {
            return Contains(GetIntList(strCollection), inInt);
        }

        public static bool ContainsIgnoreCase(IEnumerable<string> list, string target)
        {
            return list != null && list.Any(element => StringUtils.EqualsIgnoreCase(element, target));
        }

        public static bool Contains<T>(IEnumerable<T> list, T value)
        {
            return list != null && list.Contains(value);
        }

        public static List<string> GetStringList(string collection, char split = ',')
        {
            return Utilities.GetStringList(collection, split);
        }

        public static List<string> GetStringList(IEnumerable<string> collection)
        {
            return Utilities.GetStringList(collection);
        }

        public static List<int> GetIntList(string collection, char split = ',')
        {
            return Utilities.GetIntList(collection, split);
        }

        public static List<int> GetIntList(IEnumerable<int> collection)
        {
            return Utilities.GetIntList(collection);
        }

        public static List<T> GetEnums<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }

        public static IEnumerable<Select<string>> GetSelects<T>() where T : Enum
        {
            return GetEnums<T>().Select(x => new Select<string>(x));
        }

        public static string ToString(IEnumerable<string> collection, string separator = ",")
        {
            return Utilities.ToString(collection, separator);
        }

        public static string ToString(IEnumerable<int> collection, string separator = ",")
        {
            return Utilities.ToString(collection, separator);
        }

        public static string ToString(IEnumerable<object> collection, string separator = ",")
        {
            return Utilities.ToString(collection, separator);
        }

        public static Dictionary<string, object> ToDictionary(string json)
        {
            return Utilities.ToDictionary(json);
        }
    }
}
