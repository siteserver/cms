using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Datory.Utils;
using SSCMS.Configuration;
using SSCMS.Dto;

namespace SSCMS.Utils
{
    public static class ListUtils
    {
        public static T FirstOrDefault<T>(IEnumerable<T> list, Func<T, bool> predicate)
        {
            return list == null ? default : list.FirstOrDefault(predicate);
        }

        public static bool Equals<T>(List<T> list1, List<T> list2)
        {
            if (list1 == null && list2 == null) return true;
            if (list1 == null || list2 == null) return false;
            return list1.All(list2.Contains) && list1.Count == list2.Count;
        }

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

        public static bool Remove<T>(List<T> list, T value)
        {
            return list != null && list.Remove(value);
        }

        public static bool RemoveIgnoreCase(List<string> list, string value)
        {
            if (list == null) return false;

            var item = list.FirstOrDefault(x => StringUtils.EqualsIgnoreCase(x, value));
            return list.Remove(item);
        }

        public static List<T> Add<T>(List<T> list, T value)
        {
            if (list == null)
            {
                list = new List<T>();
            }
            list.Add(value);
            return list;
        }

        public static List<T> AddIfNotExists<T>(List<T> list, T value)
        {
            var retVal = list == null ? new List<T>() : new List<T>(list);

            if (!retVal.Contains(value))
            {
                retVal.Add(value);
            }
            return retVal;
        }

        public static int Count<T>(List<T> list)
        {
            return list?.Count ?? 0;
        }

        public static List<string> GetStringList(string collection, char split = ',')
        {
            return Utilities.GetStringList(collection, split);
        }

        public static List<string> GetStringList(IEnumerable<string> collection)
        {
            return Utilities.GetStringList(collection);
        }

        public static List<string> GetStringListByReturnAndNewline(string collection)
        {
            var list = new List<string>();
            if (string.IsNullOrEmpty(collection)) return list;

            var array = collection.Split(Constants.Newline);
            list.AddRange(from item in array where !string.IsNullOrEmpty(item) select item.Trim());

            return list;
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

        public static List<Select<string>> GetSelects<T>() where T : Enum
        {
            return GetEnums<T>().Select(x => new Select<string>(x)).ToList();
        }

        public static string ToString(string[] collection, string separator = ",")
        {
            return collection != null && collection.Length > 0 ? string.Join(separator, collection) : string.Empty;
        }

        public static string ToString(List<string> collection, string separator = ",")
        {
            return Utilities.ToString(collection, separator);
        }

        public static string ToString(List<int> collection, string separator = ",")
        {
            return Utilities.ToString(collection, separator);
        }

        public static string ToString(List<object> collection, string separator = ",")
        {
            return Utilities.ToString(collection, separator);
        }

        public static string ToStringByReturnAndNewline(List<string> collection)
        {
            return Utilities.ToString(collection, Constants.ReturnAndNewline);
        }

        public static Dictionary<string, object> ToDictionary(string json)
        {
            return Utilities.ToDictionary(json);
        }

        public static object GetValueIgnoreCase(IDictionary<string, object> dict, string key)
        {
            var item = dict?.FirstOrDefault(x => StringUtils.EqualsIgnoreCase(x.Key, key));
            return item?.Value;
        }

        public static List<string> ToList(object value)
        {
            if (value == null) return new List<string>();

            if (value is List<string> list)
            {
                return list;
            }

            if (value is string str)
            {
                return GetStringList(str);
            }

            if (value is IEnumerable enumerable)
            {
                var retVal = new List<string>();
                foreach (var item in enumerable)
                {
                    if (item != null)
                    {
                        retVal.Add(item.ToString());
                    }
                }

                return retVal;
            }

            return new List<string>();
        }
    }
}
