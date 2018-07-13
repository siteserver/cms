using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Model.Attributes
{
    /// <summary>
    /// Provides standard implementation for simple extendent data storage
    /// </summary>
    [Serializable]
    public class ExtendedAttributes : IAttributes
    {
        private readonly Dictionary<string, object> _dataDict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public ExtendedAttributes()
        {
        }

        public ExtendedAttributes(IDataReader reader)
        {
            Load(reader);
        }

        public ExtendedAttributes(IDataRecord record)
        {
            Load(record);
        }

        public ExtendedAttributes(DataRowView view)
        {
            Load(view);
        }

        public ExtendedAttributes(DataRow row)
        {
            Load(row);
        }

        public ExtendedAttributes(NameValueCollection attributes)
        {
            Load(attributes);
        }

        public ExtendedAttributes(Dictionary<string, object> dict)
        {
            Load(dict);
        }

        public ExtendedAttributes(string json)
        {
            Load(json);
        }

        public void Load(DataRowView rowView)
        {
            if (rowView == null) return;

            Load(rowView.Row);
        }

        public void Load(DataRow row)
        {
            if (row == null) return;

            var dict = row.Table.Columns
                .Cast<DataColumn>()
                .ToDictionary(c => c.ColumnName, c => row[c]);

            Load(dict);
        }

        public void Load(IDataReader reader)
        {
            if (reader == null) return;

            for (var i = 0; i < reader.FieldCount; i++)
            {
                var name = reader.GetName(i);
                var value = reader.GetValue(i);
                
                if (value is string && WebConfigUtils.DatabaseType == DatabaseType.Oracle && (string)value == SqlUtils.OracleEmptyValue)
                {
                    value = string.Empty;
                }
                Set(name, value);
            }
        }

        public void Load(IDataRecord record)
        {
            if (record == null) return;

            for (var i = 0; i < record.FieldCount; i++)
            {
                var name = record.GetName(i);
                var value = record.GetValue(i);

                if (value is string && WebConfigUtils.DatabaseType == DatabaseType.Oracle && (string)value == SqlUtils.OracleEmptyValue)
                {
                    value = string.Empty;
                }
                Set(name, value);
            }
        }

        public void Load(NameValueCollection attributes)
        {
            if (attributes == null) return;

            foreach (string name in attributes)
            {
                var value = attributes[name];
                Set(name, value);
            }
        }

        public void Load(Dictionary<string, object> dict)
        {
            if (dict == null) return;

            foreach (var key in dict.Keys)
            {
                Set(key, dict[key]);
            }
        }

        public void Load(string json)
        {
            if (string.IsNullOrEmpty(json)) return;

            if (json.StartsWith("{") && json.EndsWith("}"))
            {
                var dict = TranslateUtils.JsonDeserialize<Dictionary<string, object>>(json);
                foreach (var key in dict.Keys)
                {
                    _dataDict[key] = dict[key];
                }
            }
            else
            {
                var nameValues = Utils.ToNameValueCollection(json);
                foreach (string key in nameValues.Keys)
                {
                    Set(key, nameValues[key]);
                }
            }
        }

        public object Get(string name)
        {
            object value;
            if (_dataDict.TryGetValue(name, out value))
            {
                return value;
            }

            return null;
        }

        public string GetString(string name, string defaultValue = "")
        {
            var value = Get(name);
            if (value == null) return defaultValue;
            if (value is string) return (string)value;
            return value.ToString();
        }

        public bool GetBool(string name, bool defaultValue = false)
        {
            var value = Get(name);
            if (value == null) return defaultValue;
            if (value is bool) return (bool)value;
            return TranslateUtils.ToBool(value.ToString(), defaultValue);
        }

        public int GetInt(string name, int defaultValue = 0)
        {
            var value = Get(name);
            if (value == null) return defaultValue;
            if (value is int) return (int)value;
            return TranslateUtils.ToIntWithNagetive(value.ToString(), defaultValue);
        }

        public decimal GetDecimal(string name, decimal defaultValue = 0)
        {
            var value = Get(name);
            if (value == null) return defaultValue;
            if (value is decimal) return (decimal)value;
            return TranslateUtils.ToDecimalWithNagetive(value.ToString(), defaultValue);
        }

        public DateTime GetDateTime(string name, DateTime defaultValue)
        {
            var value = Get(name);
            if (value == null) return defaultValue;
            if (value is DateTime) return (DateTime)value;
            return TranslateUtils.ToDateTime(value.ToString(), defaultValue);
        }

        public void Remove(string name)
        {
            _dataDict.Remove(name);
        }

        public void Set(string name, object value)
        {
            if (value == null)
            {
                _dataDict.Remove(name);
            }
            else
            {
                _dataDict[name] = value;
            }
        }

        public bool ContainsKey(string name)
        {
            return _dataDict.ContainsKey(name);
        }

        public override string ToString()
        {
            return TranslateUtils.JsonSerialize(_dataDict);
        }

        public string ToString(List<string> excludeAttributeNames)
        {
            if (excludeAttributeNames == null || excludeAttributeNames.Count == 0) return ToString();

            var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            foreach (var key in _dataDict.Keys)
            {
                if (!StringUtils.ContainsIgnoreCase(excludeAttributeNames, key))
                {
                    dict[key] = _dataDict[key];
                }
            }
            return TranslateUtils.JsonSerialize(dict);
        }

        public int Count => _dataDict.Count;

        public virtual Dictionary<string, object> ToDictionary()
        {
            var ret = new Dictionary<string, object>(_dataDict.Count, _dataDict.Comparer);
            foreach (KeyValuePair<string, object> entry in _dataDict)
            {
                ret.Add(entry.Key, entry.Value);
            }
            return ret;
        }

        #region private utils

        private static class Utils
        {
            private static string ValueFromUrl(string value)
            {
                var retval = string.Empty;
                if (!string.IsNullOrEmpty(value))
                {
                    retval = value.Replace("_equals_", "=").Replace("_and_", "&").Replace("_question_", "?").Replace("_quote_", "'").Replace("_add_", "+").Replace("_return_", "\r").Replace("_newline_", "\n");
                }
                return retval;
            }

            public static NameValueCollection ToNameValueCollection(string separateString)
            {
                if (!string.IsNullOrEmpty(separateString))
                {
                    separateString = separateString.Replace("/u0026", "&");
                }
                return ToNameValueCollection(separateString, '&');
            }

            private static NameValueCollection ToNameValueCollection(string separateString, char seperator)
            {
                var attributes = new NameValueCollection();
                if (!string.IsNullOrEmpty(separateString))
                {
                    var pairs = separateString.Split(seperator);
                    foreach (var pair in pairs)
                    {
                        if (pair.IndexOf("=", StringComparison.Ordinal) != -1)
                        {
                            var name = ValueFromUrl(pair.Split('=')[0]);
                            var value = ValueFromUrl(pair.Split('=')[1]);
                            attributes.Add(name.ToLower(), value);
                        }
                    }
                }
                return attributes;
            }
        }

        #endregion private utils
    }
}
