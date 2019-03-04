using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Plugin.Impl
{
    [Serializable]
    public class AttributesImpl : IAttributes
    {
        private const string SettingsXml = nameof(SettingsXml);
        private const string ExtendedValues = nameof(ExtendedValues);

        private readonly Dictionary<string, object> _dataDict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public AttributesImpl()
        {
        }

        public AttributesImpl(IDataReader reader)
        {
            Load(reader);
        }

        public AttributesImpl(AttributesImpl attributes)
        {
            Load(attributes);
        }

        public void Load(AttributesImpl attributes)
        {
            if (attributes == null) return;
            foreach (var entry in attributes._dataDict)
            {
                _dataDict[entry.Key] = entry.Value;
            }
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

        public AttributesImpl(IDataRecord record)
        {
            Load(record);
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

        public AttributesImpl(DataRowView view)
        {
            Load(view);
        }

        public void Load(DataRowView rowView)
        {
            if (rowView == null) return;

            Load(rowView.Row);
        }

        public AttributesImpl(DataRow row)
        {
            Load(row);
        }

        public void Load(DataRow row)
        {
            if (row == null) return;

            var dict = row.Table.Columns
                .Cast<DataColumn>()
                .ToDictionary(c => c.ColumnName, c => row[c]);

            Load(dict);
        }

        public AttributesImpl(NameValueCollection attributes)
        {
            Load(attributes);
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

        public AttributesImpl(Dictionary<string, object> dict)
        {
            Load(dict);
        }

        public void Load(Dictionary<string, object> dict)
        {
            if (dict == null) return;

            foreach (var key in dict.Keys)
            {
                Set(key, dict[key]);
            }
        }

        public AttributesImpl(string json)
        {
            Load(json);
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
                json = json.Replace("/u0026", "&");

                var attributes = new NameValueCollection();

                var pairs = json.Split('&');
                foreach (var pair in pairs)
                {
                    if (pair.IndexOf("=", StringComparison.Ordinal) == -1) continue;
                    var name = pair.Split('=')[0];
                    if (string.IsNullOrEmpty(name)) continue;

                    name = name.Replace("_equals_", "=").Replace("_and_", "&").Replace("_question_", "?").Replace("_quote_", "'").Replace("_add_", "+").Replace("_return_", "\r").Replace("_newline_", "\n");
                    var value = pair.Split('=')[1];
                    if (!string.IsNullOrEmpty(value))
                    {
                        value = value.Replace("_equals_", "=").Replace("_and_", "&").Replace("_question_", "?").Replace("_quote_", "'").Replace("_add_", "+").Replace("_return_", "\r").Replace("_newline_", "\n");
                    }
                    attributes.Add(name.ToLower(), value);
                }

                foreach (string key in attributes.Keys)
                {
                    Set(key, attributes[key]);
                }
            }
        }

        public AttributesImpl(object anonymous)
        {
            Load(anonymous);
        }

        public void Load(object anonymous)
        {
            if (anonymous == null) return;

            foreach (var p in anonymous.GetType().GetProperties())
            {
                Set(p.Name.ToCamelCase(), p.GetValue(anonymous));
            }
        }

        public object Get(string key)
        {
            if (string.IsNullOrEmpty(key)) return null;

            return _dataDict.TryGetValue(key, out var value) ? value : null;
        }

        public string GetString(string key, string defaultValue = "")
        {
            var value = Get(key);
            if (value == null) return defaultValue;
            if (value is string) return (string)value;
            return value.ToString();
        }

        public bool GetBool(string key, bool defaultValue = false)
        {
            var value = Get(key);
            if (value == null) return defaultValue;
            if (value is bool) return (bool)value;
            return TranslateUtils.ToBool(value.ToString(), defaultValue);
        }

        public int GetInt(string key, int defaultValue = 0)
        {
            var value = Get(key);
            if (value == null) return defaultValue;
            if (value is int) return (int)value;
            return TranslateUtils.ToIntWithNagetive(value.ToString(), defaultValue);
        }

        public decimal GetDecimal(string key, decimal defaultValue = 0)
        {
            var value = Get(key);
            if (value == null) return defaultValue;
            if (value is decimal) return (decimal)value;
            return TranslateUtils.ToDecimalWithNagetive(value.ToString(), defaultValue);
        }

        public DateTime GetDateTime(string key, DateTime defaultValue)
        {
            var value = Get(key);
            if (value == null) return defaultValue;
            if (value is DateTime) return (DateTime)value;
            return TranslateUtils.ToDateTime(value.ToString(), defaultValue);
        }

        public void Remove(string key)
        {
            if (string.IsNullOrEmpty(key)) return;

            _dataDict.Remove(key);
        }

        public void Set(string name, object value)
        {
            if (string.IsNullOrEmpty(name)) return;

            if (value == null)
            {
                _dataDict.Remove(name);
            }
            else
            {
                if (StringUtils.EqualsIgnoreCase(name, SettingsXml) || StringUtils.EqualsIgnoreCase(name, ExtendedValues))
                {
                    Load(value.ToString());
                }
                else
                {
                    _dataDict[name] = value;
                }
            }
        }

        public bool ContainsKey(string key)
        {
            if (string.IsNullOrEmpty(key)) return false;

            return _dataDict.ContainsKey(key);
        }

        public override string ToString()
        {
            return TranslateUtils.JsonSerialize(_dataDict);
        }

        public string ToString(List<string> excludeKeys)
        {
            if (excludeKeys == null || excludeKeys.Count == 0) return ToString();

            var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            foreach (var key in _dataDict.Keys)
            {
                if (!StringUtils.ContainsIgnoreCase(excludeKeys, key))
                {
                    dict[key] = _dataDict[key];
                }
            }
            return TranslateUtils.JsonSerialize(dict);
        }

        public virtual Dictionary<string, object> ToDictionary()
        {
            var ret = new Dictionary<string, object>(_dataDict.Count, _dataDict.Comparer);
            foreach (KeyValuePair<string, object> entry in _dataDict)
            {
                ret.Add(entry.Key, entry.Value);
            }
            return ret;
        }
    }
}
