using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Data;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using BaiRong.Core.Cryptography;
using System.Text.RegularExpressions;
using BaiRong.Core.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace BaiRong.Core
{
    public class TranslateUtils
    {

        //添加枚举：(fileAttributes | FileAttributes.ReadOnly)   判断枚举：((fileAttributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)   去除枚举：(fileAttributes ^ FileAttributes.ReadOnly)

        /// <summary>
        /// 将字符串类型转换为对应的枚举类型
        /// </summary>
        public static object ToEnum(Type enumType, string value, object defaultType)
        {
            object retval;
            try
            {
                retval = Enum.Parse(enumType, value, true);
            }
            catch
            {
                retval = defaultType;
            }
            return retval;
        }

        public static string EnumToString(Enum enumType)
        {
            return enumType.ToString();
        }

        public static SqlDbType ToSqlDbType(string typeStr)
        {
            return (SqlDbType)ToEnum(typeof(SqlDbType), typeStr, SqlDbType.VarChar);
        }

        public static System.Data.OleDb.OleDbType ToOleDbType(string typeStr)
        {
            return (System.Data.OleDb.OleDbType)ToEnum(typeof(System.Data.OleDb.OleDbType), typeStr, System.Data.OleDb.OleDbType.VarChar);
        }

        public static List<int> ToIntList(int intValue)
        {
            return new List<int> {intValue};
        }

        public static int ToInt(string intStr)
        {
            return ToInt(intStr, 0);
        }

        public static int ToInt(string intStr, int defaultValue)
        {
            int i;
            if (!int.TryParse(intStr?.Trim().TrimStart('0'), out i))
            {
                i = defaultValue;
            }
            if (i < 0)
            {
                i = defaultValue;
            }
            return i;
        }

        public static int ToIntWithNagetive(string intStr)
        {
            return ToIntWithNagetive(intStr, 0);
        }

        public static int ToIntWithNagetive(string intStr, int defaultValue)
        {
            int i;
            if (!int.TryParse(intStr?.Trim(), out i))
            {
                i = defaultValue;
            }
            return i;
        }

        public static decimal ToDecimal(string intStr)
        {
            return ToDecimal(intStr, 0);
        }

        public static decimal ToDecimal(string intStr, decimal defaultValue)
        {
            decimal i;
            if (!decimal.TryParse(intStr?.Trim(), out i))
            {
                i = defaultValue;
            }
            if (i < 0)
            {
                i = defaultValue;
            }
            return i;
        }

        public static decimal ToDecimalWithNagetive(string intStr)
        {
            return ToDecimalWithNagetive(intStr, 0);
        }

        public static decimal ToDecimalWithNagetive(string intStr, decimal defaultValue)
        {
            decimal i;
            if (!decimal.TryParse(intStr?.Trim(), out i))
            {
                i = defaultValue;
            }
            return i;
        }

        public static double ToDouble(string intStr)
        {
            return ToDouble(intStr, 0);
        }

        public static double ToDouble(string intStr, double defaultValue)
        {
            double i;
            if (!double.TryParse(intStr?.Trim(), out i))
            {
                i = defaultValue;
            }
            if (i < 0)
            {
                i = defaultValue;
            }
            return i;
        }

        public static long ToLong(string intStr)
        {
            return ToLong(intStr, 0);
        }

        public static long ToLong(string intStr, long defaultValue)
        {
            long l;
            if (!long.TryParse(intStr?.Trim(), out l))
            {
                l = defaultValue;
            }
            if (l < 0)
            {
                l = defaultValue;
            }
            return l;
        }

        public static bool ToBool(string boolStr)
        {
            bool boolean;
            if (!bool.TryParse(boolStr?.Trim(), out boolean))
            {
                boolean = false;
            }
            return boolean;
        }

        public static bool ToBool(string boolStr, bool defaultValue)
        {
            bool boolean;
            if (!bool.TryParse(boolStr?.Trim(), out boolean))
            {
                boolean = defaultValue;
            }
            return boolean;
        }

        public static DateTime ToDateTime(string dateTimeStr)
        {
            var datetime = DateUtils.SqlMinValue;
            if (!string.IsNullOrEmpty(dateTimeStr))
            {
                if (!DateTime.TryParse(dateTimeStr.Trim(), out datetime))
                {
                    datetime = DateUtils.SqlMinValue;
                }
            }
            if (datetime < DateUtils.SqlMinValue)
            {
                datetime = DateUtils.SqlMinValue;
            }
            return datetime;
        }

        public static DateTime ToDateTime(string dateTimeStr, DateTime defaultValue)
        {
            var datetime = defaultValue;
            if (!string.IsNullOrEmpty(dateTimeStr))
            {
                if (!DateTime.TryParse(dateTimeStr.Trim(), out datetime))
                {
                    datetime = defaultValue;
                }
                return datetime;
            }
            return datetime;
        }

        public static Color ToColor(string colorStr)
        {
            var color = Color.Empty;
            try
            {
                color = Color.FromName(colorStr.Trim());
            }
            catch
            {
                // ignored
            }
            return color;
        }

        public static string ToCurrency(decimal i)
        {
            return i.ToString("c");
        }

        public static string ToWidth(string width)
        {
            if (!string.IsNullOrEmpty(width) && !width.EndsWith("%") && !width.EndsWith("px"))
            {
                return width + "px";
            }
            return width;
        }

        public static Unit ToUnit(string unitStr)
        {
            var type = Unit.Empty;
            try
            {
                type = Unit.Parse(unitStr.Trim());
            }
            catch
            {
                // ignored
            }
            return type;
        }


        public static string ToTwoCharString(int i)
        {
            return i >= 0 && i <= 9 ? $"0{i}" : i.ToString();
        }

        public static string Censor(string censorRegex, string inputContent)
        {
            return RegexUtils.Replace(censorRegex, inputContent, "***");
        }

        public static StringCollection StringCollectionToStringCollection(string collection)
        {
            return StringCollectionToStringCollection(collection, ',');
        }

        public static List<int> StringCollectionToIntList(string collection)
        {
            var list = new List<int>();
            if (!string.IsNullOrEmpty(collection))
            {
                var array = collection.Split(',');
                foreach (var s in array)
                {
                    int i;
                    int.TryParse(s.Trim(), out i);
                    list.Add(i);
                }
            }
            return list;
        }

        public static List<decimal> StringCollectionToDecimalList(string collection)
        {
            var list = new List<decimal>();
            if (!string.IsNullOrEmpty(collection))
            {
                var array = collection.Split(',');
                foreach (var s in array)
                {
                    decimal i;
                    decimal.TryParse(s.Trim(), out i);
                    list.Add(i);
                }
            }
            return list;
        }

        public static List<string> StringCollectionToStringList(string collection)
        {
            return StringCollectionToStringList(collection, ',');
        }

        public static List<string> StringCollectionToStringList(string collection, char split)
        {
            var list = new List<string>();
            if (!string.IsNullOrEmpty(collection))
            {
                var array = collection.Split(split);
                foreach (var s in array)
                {
                    list.Add(s);
                }
            }
            return list;
        }

        public static string IntDictionaryToStringCollection(Dictionary<int, int> dictionary)
        {
            return IntDictionaryToStringCollection(dictionary, ',', '_');
        }

        public static string IntDictionaryToStringCollection(Dictionary<int, int> dictionary, char split1, char split2)
        {
            var builder = new StringBuilder();

            if (dictionary != null && dictionary.Count > 0)
            {
                foreach (var item in dictionary)
                {
                    builder.Append($"{item.Key}{split2}{item.Value}{split1}");
                }
            }

            if (builder.Length > 0) builder.Length--;
            return builder.ToString();
        }

        public static Dictionary<int, int> StringCollectionToIntDictionary(string collection)
        {
            return StringCollectionToIntDictionary(collection, ',', '_');
        }

        public static Dictionary<int, int> StringCollectionToIntDictionary(string collection, char split1, char split2)
        {
            var dictionary = new Dictionary<int, int>();
            if (!string.IsNullOrEmpty(collection))
            {
                var array1 = collection.Split(split1);
                foreach (var string1 in array1)
                {
                    if (!string.IsNullOrEmpty(string1))
                    {
                        var array2 = string1.Split(split2);
                        if (array2.Length == 2)
                        {
                            var key = ToInt(array2[0]);
                            var value = ToInt(array2[1]);
                            if (key > 0)
                            {
                                dictionary[key] = value;
                            }
                        }
                    }
                }
            }
            return dictionary;
        }

        public static StringCollection StringCollectionToStringCollection(string collection, char separator)
        {
            var arraylist = new StringCollection();
            if (!string.IsNullOrEmpty(collection))
            {
                var array = collection.Split(separator);
                foreach (var s in array)
                {
                    arraylist.Add(s.Trim());
                }
            }
            return arraylist;
        }

        public static ArrayList ObjectCollectionToArrayList(ICollection collection)
        {
            var arraylist = new ArrayList();
            if (collection != null)
            {
                foreach (var obj in collection)
                {
                    arraylist.Add(obj);
                }
            }
            return arraylist;
        }

        public static List<string> ObjectCollectionToStringList(ICollection collection)
        {
            var list = new List<string>();
            if (collection != null)
            {
                foreach (var obj in collection)
                {
                    list.Add(obj.ToString());
                }
            }
            return list;
        }

        public static string ObjectCollectionToString(ICollection collection)
        {
            var builder = new StringBuilder();
            if (collection != null)
            {
                foreach (var obj in collection)
                {
                    builder.Append(obj.ToString().Trim()).Append(",");
                }
                if (builder.Length != 0) builder.Remove(builder.Length - 1, 1);
            }
            return builder.ToString();
        }

        public static string ObjectCollectionToString(ICollection collection, string separatorStr)
        {
            var builder = new StringBuilder();
            if (collection != null)
            {
                foreach (var obj in collection)
                {
                    builder.Append(obj.ToString().Trim()).Append(separatorStr);
                }
                if (builder.Length != 0) builder.Remove(builder.Length - separatorStr.Length, separatorStr.Length);
            }
            return builder.ToString();
        }

        public static string ObjectCollectionToString(ICollection collection, string separatorStr, string prefixStr)
        {
            var builder = new StringBuilder();
            if (collection != null)
            {
                foreach (var obj in collection)
                {
                    builder.Append(prefixStr + obj.ToString().Trim()).Append(separatorStr);
                }
                if (builder.Length != 0) builder.Remove(builder.Length - separatorStr.Length, separatorStr.Length);
            }
            return builder.ToString();
        }

        public static string ObjectCollectionToString(ICollection collection, string separatorStr, string prefixStr, string appendixStr)
        {
            var builder = new StringBuilder();
            if (collection != null)
            {
                foreach (var obj in collection)
                {
                    builder.Append(prefixStr + obj.ToString().Trim() + appendixStr).Append(separatorStr);
                }
                if (builder.Length != 0) builder.Remove(builder.Length - separatorStr.Length, separatorStr.Length);
            }
            return builder.ToString();
        }

        /// <summary>
        /// 将对象集合转化为可供Sql语句查询的In()条件，如将集合{'ss','xx','ww'}转化为字符串"'ss','xx','ww'"。
        /// </summary>
        /// <param name="collection">非数字的集合</param>
        /// <returns>可供Sql语句查询的In()条件字符串，各元素用单引号包围</returns>
        public static string ToSqlInStringWithQuote(ICollection collection)
        {
            var builder = new StringBuilder();
            if (collection != null)
            {
                foreach (var obj in collection)
                {
                    builder.Append("'").Append(obj).Append("'").Append(",");
                }
                if (builder.Length != 0) builder.Remove(builder.Length - 1, 1);
            }
            return builder.Length == 0 ? "null" : builder.ToString();
        }

        /// <summary>
        /// 将数字集合转化为可供Sql语句查询的In()条件，如将集合{2,3,4}转化为字符串"2,3,4"。
        /// </summary>
        /// <param name="collection">非数字的集合</param>
        /// <returns>可供Sql语句查询的In()条件字符串，各元素不使用单引号包围</returns>
        public static string ToSqlInStringWithoutQuote(ICollection collection)
        {
            var builder = new StringBuilder();
            if (collection != null)
            {
                foreach (var obj in collection)
                {
                    builder.Append(obj).Append(",");
                }
                if (builder.Length != 0) builder.Remove(builder.Length - 1, 1);
            }
            return builder.Length == 0 ? "null" : builder.ToString();
        }

        public static DataView ObjectCollectionToDataView(string columnName, ICollection collection)
        {
            var myTable = new DataTable("myTable");
            var column = new DataColumn(columnName);
            myTable.Columns.Add(column);
            foreach (var value in collection)
            {
                var row = myTable.NewRow();
                row[columnName] = value;
                myTable.Rows.Add(row);
            }
            var myDataView = new DataView(myTable);
            return myDataView;
        }


        public static string[] ArrayListToStringArray(ArrayList arraylist)
        {
            return (string[])arraylist.ToArray(typeof(string));
        }

        public static ArrayList StringArrayToArrayList(string[] array)
        {
            return new ArrayList(array);
        }

        public static List<string> StringArrayToStringList(string[] array)
        {
            return new List<string>(array);
        }

        //将IDictionary转换为NameValueCollection
        public static NameValueCollection ToNameValueCollection(IDictionary dictionary)
        {
            var nameValueMap = new NameValueCollection();
            foreach (var key in dictionary.Keys)
            {
                var value = dictionary[key];
                nameValueMap.Add(key.ToString(), value.ToString());
            }
            return nameValueMap;
        }

        public static NameValueCollection ToNameValueCollection(string separateString)
        {
            if (!string.IsNullOrEmpty(separateString))
            {
                separateString = separateString.Replace("/u0026", "&");
            }
            return ToNameValueCollection(separateString, '&');
        }

        public static NameValueCollection ToNameValueCollection(string separateString, char seperator)
        {
            var attributes = new NameValueCollection();
            if (!string.IsNullOrEmpty(separateString))
            {
                var pairs = separateString.Split(seperator);
                foreach (var pair in pairs)
                {
                    if (pair.IndexOf("=", StringComparison.Ordinal) != -1)
                    {
                        var name = StringUtils.ValueFromUrl(pair.Split('=')[0]);
                        var value = StringUtils.ValueFromUrl(pair.Split('=')[1]);
                        attributes.Add(name, value);
                    }
                }
            }
            return attributes;
        }

        public static Dictionary<string, object> ObjectToDictionary(object source)
        {
            return source.GetType().GetProperties().ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => propInfo.GetValue(source, null)
            );
        }

        public static Dictionary<string, string> ToDictionary(NameValueCollection attributes)
        {
            var dic = new Dictionary<string, string>();
            if (attributes != null && attributes.Count > 0)
            {
                foreach (string key in attributes.Keys)
                {
                    dic[key] = attributes[key];
                }
            }
            return dic;
        }

        public static Dictionary<string, string> ToDictionary(string separateString)
        {
            if (!string.IsNullOrEmpty(separateString))
            {
                separateString = separateString.Replace("/u0026", "&");
            }
            return ToDictionary(separateString, '&');
        }

        public static Dictionary<string, string> ToDictionary(string separateString, char seperator)
        {
            var attributes = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(separateString))
            {
                var pairs = separateString.Split(seperator);
                foreach (var pair in pairs)
                {
                    if (pair.IndexOf("=", StringComparison.Ordinal) != -1)
                    {
                        var name = StringUtils.ValueFromUrl(pair.Split('=')[0]);
                        var value = StringUtils.ValueFromUrl(pair.Split('=')[1]);
                        attributes.Add(name, value);
                    }
                }
            }
            return attributes;
        }

        public static string NameValueCollectionToString(NameValueCollection attributes)
        {
            return NameValueCollectionToString(attributes, '&');
        }

        public static string NameValueCollectionToString(NameValueCollection attributes, char seperator)
        {
            var builder = new StringBuilder();
            if (attributes != null && attributes.Count > 0)
            {
                foreach (string key in attributes.Keys)
                {
                    builder.Append(
                        $@"{StringUtils.ValueToUrl(key)}={StringUtils.ValueToUrl(attributes[key])}{seperator}");
                }
                builder.Length--;
            }
            return builder.ToString();
        }

        public static string DictionaryToString(Dictionary<string, string> attributes)
        {
            return DictionaryToString(attributes, '&');
        }

        public static string DictionaryToString(Dictionary<string, string> attributes, char seperator)
        {
            var builder = new StringBuilder();
            if (attributes != null && attributes.Count > 0)
            {
                foreach (var key in attributes.Keys)
                {
                    builder.Append(
                        $@"{StringUtils.ValueToUrl(key)}={StringUtils.ValueToUrl(attributes[key])}{seperator}");
                }
                builder.Length--;
            }
            return builder.ToString();
        }

        public static string ToAttributesString(LowerNameValueCollection attributes)
        {
            var builder = new StringBuilder();
            if (attributes != null && attributes.Count > 0)
            {
                foreach (var key in attributes.Keys)
                {
                    var value = attributes.Get(key);
                    if (!string.IsNullOrEmpty(value))
                    {
                        value = value.Replace("\"", "'");
                    }
                    builder.Append($@"{key}=""{value}"" ");
                }
                builder.Length--;
            }
            return builder.ToString();
        }

        public static string ToAttributesString(NameValueCollection attributes)
        {
            var builder = new StringBuilder();
            if (attributes != null && attributes.Count > 0)
            {
                foreach (string key in attributes.Keys)
                {
                    var value = attributes[key];
                    if (!string.IsNullOrEmpty(value))
                    {
                        value = value.Replace("\"", "'");
                    }
                    builder.Append($@"{key}=""{value}"" ");
                }
                builder.Length--;
            }
            return builder.ToString();
        }

        public static string ToAttributesString(StringDictionary attributes)
        {
            var builder = new StringBuilder();
            if (attributes != null && attributes.Count > 0)
            {
                foreach (string key in attributes.Keys)
                {
                    var value = attributes[key];
                    if (!string.IsNullOrEmpty(value))
                    {
                        value = value.Replace("\"", "'");
                    }
                    builder.Append($@"{key}=""{value}"" ");
                }
                builder.Length--;
            }
            return builder.ToString();
        }

        public static NameValueCollection ParseJsonStringToNameValueCollection(string jsonString)
        {
            var nameValueCollection = new NameValueCollection();
            if (!string.IsNullOrEmpty(jsonString))
            {
                jsonString = jsonString.Trim().TrimStart('{').TrimEnd('}');
                var array1 = jsonString.Split(',');
                foreach (var s1 in array1)
                {
                    if (s1.IndexOf(':') != -1)
                    {
                        var name = s1.Substring(0, s1.IndexOf(':'));
                        var value = s1.Substring(s1.IndexOf(':') + 1);

                        nameValueCollection.Set(name.Trim().Trim('"', '\''), value.Trim().Trim('"', '\''));
                    }
                }
            }
            return nameValueCollection;
        }

        public static NameValueCollection ParseJsonStringToNameValueCollection(string jsonString, bool isRecursive)
        {
            if (!isRecursive)
                return ParseJsonStringToNameValueCollection(jsonString);

            var nameValueCollection = new NameValueCollection();
            if (!string.IsNullOrEmpty(jsonString))
            {
                jsonString = jsonString.Trim().TrimStart('{').TrimEnd('}');
                var array1 = jsonString.Split(',');
                foreach (var s1 in array1)
                {
                    if (s1.IndexOf(':') != -1)
                    {
                        var name = s1.Substring(0, s1.IndexOf(':'));
                        var value = s1.Substring(s1.IndexOf(':') + 1);
                        if (value.IndexOf("{", StringComparison.Ordinal) != -1)
                        {
                            nameValueCollection.Add(ParseJsonStringToNameValueCollection(value, true));
                        }
                        else
                        {
                            nameValueCollection.Set(name.Trim().Trim('"', '\''), value.Trim().Trim('"', '\''));
                        }
                    }
                }
            }
            return nameValueCollection;
        }


        public static string NameValueCollectionToJsonString(NameValueCollection attributes)
        {
            var jsonString = new StringBuilder("{");
            if (attributes != null && attributes.Count > 0)
            {
                foreach (string key in attributes.Keys)
                {
                    var value = attributes[key];
                    value = value?.Replace("\\", "\\\\").Replace("\"", "\\\\\\\"").Replace("\r\n", string.Empty);
                    jsonString.AppendFormat(@"""{0}"": ""{1}"",", key, value);
                }
                jsonString.Length--;
            }
            jsonString.Append("}");
            return jsonString.ToString();
        }

        public static string NameValueCollectionToXmlString(NameValueCollection attributes)
        {
            var xmlString = new StringBuilder("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n");
            if (attributes != null && attributes.Count > 0)
            {
                foreach (string key in attributes.Keys)
                {
                    var value = attributes[key];
                    value = value?.Replace("\\", "\\\\").Replace("\"", "\\\\\\\"").Replace("\r\n", string.Empty);
                    xmlString.AppendFormat("<{0}>{1}</{0}>\r\n", key, value);
                }
            }
            return xmlString.ToString();
        }

        public static void SetOrRemoveAttribute(NameValueCollection attributes, string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                attributes[key] = value;
            }
            else
            {
                attributes.Remove(key);
            }
        }

        public static int GetMbSize(int kbSize)
        {
            var retval = 0;
            if (kbSize >= 1024 && kbSize % 1024 == 0)
            {
                retval = kbSize / 1024;
            }
            return retval;
        }

        public static long GetKbSize(long byteSize)
        {
            long fileKbSize = Convert.ToUInt32(Math.Ceiling((double)byteSize / 1024));
            if (fileKbSize == 0)
            {
                fileKbSize = 1;
            }
            return fileKbSize;
        }

        public static int GetIntFromQueryString(NameValueCollection queryString, string key)
        {
            var queryStringValue = queryString[key];

            if (string.IsNullOrEmpty(queryStringValue)) return 0;

            if (queryStringValue.IndexOf("#", StringComparison.Ordinal) > 0)
            {
                queryStringValue = queryStringValue.Substring(0, queryStringValue.IndexOf("#", StringComparison.Ordinal));
            }

            return ToInt(queryStringValue);
        }

        public static IList GetList(IList list, int startNum, int totalNum)
        {
            if (list == null) return null;
            IList retval = new List<object>();
            for (var i = 0; i < list.Count; i++)
            {
                if (i + 1 >= startNum)
                {
                    retval.Add(list[i]);
                }
                if (totalNum > 0 && retval.Count >= totalNum) break;
            }
            return retval;
        }

        public static string EscapeHtml(string content)
        {
            var sb = new StringBuilder();
            var ba = Encoding.Unicode.GetBytes(content);
            for (var i = 0; i < ba.Length; i += 2)
            {
                sb.Append("%u");
                sb.Append(ba[i + 1].ToString("X2"));
                sb.Append(ba[i].ToString("X2"));
            }
            return sb.ToString();
        }

        #region 汉字转拼音

        private static readonly int[] Pyvalue =
        {
            -20319, -20317, -20304, -20295, -20292, -20283, -20265, -20257, -20242, -20230, -20051, -20036, -20032,
            -20026, -20002, -19990, -19986, -19982, -19976, -19805, -19784, -19775, -19774, -19763, -19756, -19751,
            -19746, -19741, -19739, -19728,
            -19725, -19715, -19540, -19531, -19525, -19515, -19500, -19484, -19479, -19467, -19289, -19288, -19281,
            -19275, -19270, -19263,
            -19261, -19249, -19243, -19242, -19238, -19235, -19227, -19224, -19218, -19212, -19038, -19023, -19018,
            -19006, -19003, -18996,
            -18977, -18961, -18952, -18783, -18774, -18773, -18763, -18756, -18741, -18735, -18731, -18722, -18710,
            -18697, -18696, -18526,
            -18518, -18501, -18490, -18478, -18463, -18448, -18447, -18446, -18239, -18237, -18231, -18220, -18211,
            -18201, -18184, -18183,
            -18181, -18012, -17997, -17988, -17970, -17964, -17961, -17950, -17947, -17931, -17928, -17922, -17759,
            -17752, -17733, -17730,
            -17721, -17703, -17701, -17697, -17692, -17683, -17676, -17496, -17487, -17482, -17468, -17454, -17433,
            -17427, -17417, -17202,
            -17185, -16983, -16970, -16942, -16915, -16733, -16708, -16706, -16689, -16664, -16657, -16647, -16474,
            -16470, -16465, -16459,
            -16452, -16448, -16433, -16429, -16427, -16423, -16419, -16412, -16407, -16403, -16401, -16393, -16220,
            -16216, -16212, -16205,
            -16202, -16187, -16180, -16171, -16169, -16158, -16155, -15959, -15958, -15944, -15933, -15920, -15915,
            -15903, -15889, -15878,
            -15707, -15701, -15681, -15667, -15661, -15659, -15652, -15640, -15631, -15625, -15454, -15448, -15436,
            -15435, -15419, -15416,
            -15408, -15394, -15385, -15377, -15375, -15369, -15363, -15362, -15183, -15180, -15165, -15158, -15153,
            -15150, -15149, -15144,
            -15143, -15141, -15140, -15139, -15128, -15121, -15119, -15117, -15110, -15109, -14941, -14937, -14933,
            -14930, -14929, -14928,
            -14926, -14922, -14921, -14914, -14908, -14902, -14894, -14889, -14882, -14873, -14871, -14857, -14678,
            -14674, -14670, -14668,
            -14663, -14654, -14645, -14630, -14594, -14429, -14407, -14399, -14384, -14379, -14368, -14355, -14353,
            -14345, -14170, -14159,
            -14151, -14149, -14145, -14140, -14137, -14135, -14125, -14123, -14122, -14112, -14109, -14099, -14097,
            -14094, -14092, -14090,
            -14087, -14083, -13917, -13914, -13910, -13907, -13906, -13905, -13896, -13894, -13878, -13870, -13859,
            -13847, -13831, -13658,
            -13611, -13601, -13406, -13404, -13400, -13398, -13395, -13391, -13387, -13383, -13367, -13359, -13356,
            -13343, -13340, -13329,
            -13326, -13318, -13147, -13138, -13120, -13107, -13096, -13095, -13091, -13076, -13068, -13063, -13060,
            -12888, -12875, -12871,
            -12860, -12858, -12852, -12849, -12838, -12831, -12829, -12812, -12802, -12607, -12597, -12594, -12585,
            -12556, -12359, -12346,
            -12320, -12300, -12120, -12099, -12089, -12074, -12067, -12058, -12039, -11867, -11861, -11847, -11831,
            -11798, -11781, -11604,
            -11589, -11536, -11358, -11340, -11339, -11324, -11303, -11097, -11077, -11067, -11055, -11052, -11045,
            -11041, -11038, -11024,
            -11020, -11019, -11018, -11014, -10838, -10832, -10815, -10800, -10790, -10780, -10764, -10587, -10544,
            -10533, -10519, -10331,
            -10329, -10328, -10322, -10315, -10309, -10307, -10296, -10281, -10274, -10270, -10262, -10260, -10256,
            -10254
        };

        private static readonly string[] Pystr =
        {
            "a", "ai", "an", "ang", "ao", "ba", "bai", "ban", "bang", "bao", "bei", "ben", "beng", "bi", "bian", "biao",
            "bie", "bin", "bing", "bo", "bu", "ca", "cai", "can", "cang", "cao", "ce", "ceng", "cha", "chai", "chan",
            "chang", "chao", "che", "chen",
            "cheng", "chi", "chong", "chou", "chu", "chuai", "chuan", "chuang", "chui", "chun", "chuo", "ci", "cong",
            "cou", "cu", "cuan", "cui",
            "cun", "cuo", "da", "dai", "dan", "dang", "dao", "de", "deng", "di", "dian", "diao", "die", "ding", "diu",
            "dong", "dou", "du", "duan",
            "dui", "dun", "duo", "e", "en", "er", "fa", "fan", "fang", "fei", "fen", "feng", "fo", "fou", "fu", "ga",
            "gai", "gan", "gang", "gao",
            "ge", "gei", "gen", "geng", "gong", "gou", "gu", "gua", "guai", "guan", "guang", "gui", "gun", "guo", "ha",
            "hai", "han", "hang",
            "hao", "he", "hei", "hen", "heng", "hong", "hou", "hu", "hua", "huai", "huan", "huang", "hui", "hun", "huo",
            "ji", "jia", "jian",
            "jiang", "jiao", "jie", "jin", "jing", "jiong", "jiu", "ju", "juan", "jue", "jun", "ka", "kai", "kan",
            "kang", "kao", "ke", "ken",
            "keng", "kong", "kou", "ku", "kua", "kuai", "kuan", "kuang", "kui", "kun", "kuo", "la", "lai", "lan", "lang",
            "lao", "le", "lei",
            "leng", "li", "lia", "lian", "liang", "liao", "lie", "lin", "ling", "liu", "long", "lou", "lu", "lv", "luan",
            "lue", "lun", "luo",
            "ma", "mai", "man", "mang", "mao", "me", "mei", "men", "meng", "mi", "mian", "miao", "mie", "min", "ming",
            "miu", "mo", "mou", "mu",
            "na", "nai", "nan", "nang", "nao", "ne", "nei", "nen", "neng", "ni", "nian", "niang", "niao", "nie", "nin",
            "ning", "niu", "nong",
            "nu", "nv", "nuan", "nue", "nuo", "o", "ou", "pa", "pai", "pan", "pang", "pao", "pei", "pen", "peng", "pi",
            "pian", "piao", "pie",
            "pin", "ping", "po", "pu", "qi", "qia", "qian", "qiang", "qiao", "qie", "qin", "qing", "qiong", "qiu", "qu",
            "quan", "que", "qun",
            "ran", "rang", "rao", "re", "ren", "reng", "ri", "rong", "rou", "ru", "ruan", "rui", "run", "ruo", "sa",
            "sai", "san", "sang",
            "sao", "se", "sen", "seng", "sha", "shai", "shan", "shang", "shao", "she", "shen", "sheng", "shi", "shou",
            "shu", "shua",
            "shuai", "shuan", "shuang", "shui", "shun", "shuo", "si", "song", "sou", "su", "suan", "sui", "sun", "suo",
            "ta", "tai",
            "tan", "tang", "tao", "te", "teng", "ti", "tian", "tiao", "tie", "ting", "tong", "tou", "tu", "tuan", "tui",
            "tun", "tuo",
            "wa", "wai", "wan", "wang", "wei", "wen", "weng", "wo", "wu", "xi", "xia", "xian", "xiang", "xiao", "xie",
            "xin", "xing",
            "xiong", "xiu", "xu", "xuan", "xue", "xun", "ya", "yan", "yang", "yao", "ye", "yi", "yin", "ying", "yo",
            "yong", "you",
            "yu", "yuan", "yue", "yun", "za", "zai", "zan", "zang", "zao", "ze", "zei", "zen", "zeng", "zha", "zhai",
            "zhan", "zhang",
            "zhao", "zhe", "zhen", "zheng", "zhi", "zhong", "zhou", "zhu", "zhua", "zhuai", "zhuan", "zhuang", "zhui",
            "zhun", "zhuo",
            "zi", "zong", "zou", "zu", "zuan", "zui", "zun", "zuo"
        };

        public static string ToPinYin(string chrstr)
        {
            var returnstr = string.Empty;
            var nowchar = chrstr.ToCharArray();
            foreach (var t in nowchar)
            {
                var array = Encoding.Default.GetBytes(t.ToString());
                int i1 = array[0];
                int i2 = array[1];
                var chrasc = i1 * 256 + i2 - 65536;
                if (chrasc > 0 && chrasc < 160)
                {
                    returnstr += t;
                }
                else
                {
                    for (var i = (Pyvalue.Length - 1); i >= 0; i--)
                    {
                        if (Pyvalue[i] <= chrasc)
                        {
                            returnstr += Pystr[i];
                            break;
                        }
                    }
                }
            }
            return returnstr;
        }

        #endregion

        [Obsolete("use JsonSerialize")]
        public static string ObjectToJson(object obj)
        {
            var retval = string.Empty;
            try
            {
                var serializer = new DataContractJsonSerializer(obj.GetType());
                var stream = new MemoryStream();
                serializer.WriteObject(stream, obj);
                var dataBytes = new byte[stream.Length];
                stream.Position = 0;
                stream.Read(dataBytes, 0, (int)stream.Length);
                retval = Encoding.UTF8.GetString(dataBytes);
            }
            catch
            {
                // ignored
            }
            return retval;
        }

        [Obsolete("use JsonDeserialize")]
        public static object JsonToObject(string jsonString, object obj)
        {
            object retval = null;
            try
            {
                var serializer = new DataContractJsonSerializer(obj.GetType());
                var mStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
                retval = serializer.ReadObject(mStream);
            }
            catch
            {
                // ignored
            }
            return retval;
        }


        [Obsolete("use JsonDeserialize")]
        public static T JsonToObject<T>(string jsonString)
        {
            var js = new JavaScriptSerializer();
            try
            {
                jsonString = Regex.Replace(jsonString, @"\\/Date\((-?\d+)\)\\/", match =>
                {
                    var dt = new DateTime(1970, 1, 1);
                    dt = dt.AddMilliseconds(long.Parse(match.Groups[1].Value));
                    dt = dt.ToLocalTime();
                    return dt.ToString("yyyy-MM-dd HH:mm:ss");
                });
                return js.Deserialize<T>(jsonString);
            }
            catch
            {
                return default(T);
            }
        }

        [Obsolete("use JsonSerialize")]
        public static string ObjectToJson<T>(T obj)
        {
            var js = new JavaScriptSerializer();
            try
            {
                var jsonString = js.Serialize(obj);

                jsonString = Regex.Replace(jsonString, @"\\/Date\((-?\d+)\)\\/", match =>
                {
                    var dt = new DateTime(1970, 1, 1);
                    dt = dt.AddMilliseconds(long.Parse(match.Groups[1].Value));
                    dt = dt.ToLocalTime();
                    return dt.ToString("yyyy-MM-dd HH:mm:ss");
                });

                return jsonString;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string JsonSerialize(object obj)
        {
            var settings = new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()};
            var timeFormat = new IsoDateTimeConverter {DateTimeFormat = "yyyy-MM-dd HH:mm:ss"};
            settings.Converters.Add(timeFormat);

            return JsonConvert.SerializeObject(obj, settings);
        }

        public static T JsonDeserialize<T>(string json)
        {
            var settings = new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()};
            var timeFormat = new IsoDateTimeConverter {DateTimeFormat = "yyyy-MM-dd HH:mm:ss"};
            settings.Converters.Add(timeFormat);

            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        public static string EncryptStringBySecretKey(string inputString)
        {
            if (string.IsNullOrEmpty(inputString)) return string.Empty;

            var encryptor = new DESEncryptor
            {
                InputString = inputString,
                EncryptKey = FileConfigManager.Instance.SecretKey
            };
            encryptor.DesEncrypt();

            var retval = encryptor.OutString;
            retval = retval.Replace("+", "0add0").Replace("=", "0equals0").Replace("&", "0and0").Replace("?", "0question0").Replace("'", "0quote0").Replace("/", "0slash0");

            return retval;
        }

        public static string DecryptStringBySecretKey(string inputString)
        {
            if (string.IsNullOrEmpty(inputString)) return string.Empty;

            inputString = inputString.Replace("0add0", "+").Replace("0equals0", "=").Replace("0and0", "&").Replace("0question0", "?").Replace("0quote0", "'").Replace("0slash0", "/");

            var encryptor = new DESEncryptor
            {
                InputString = inputString,
                DecryptKey = FileConfigManager.Instance.SecretKey
            };
            encryptor.DesDecrypt();

            return encryptor.OutString;
        }
    }
}
