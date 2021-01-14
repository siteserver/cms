using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Datory.Annotations;
using Newtonsoft.Json;
using Datory.Utils;

namespace Datory
{
    [JsonConverter(typeof(EntityBaseConverter))]
    public class Entity : IEntity
    {
        [DataColumn]
        public int Id { get; set; }

        [DataColumn(Length = 50)]
        public string Guid { get; set; }

        [DataColumn(Text = true)]
        internal string ExtendValues { get; set; }

        [DataColumn]
        public DateTime? CreatedDate { get; set; }

        [DataColumn]
        public DateTime? LastModifiedDate { get; set; }

        private readonly Type _type;

        private readonly List<string> _propertyNames;

        private readonly List<string> _columnNames;

        private readonly List<string> _dataIgnoreNames;

        private readonly List<string> _jsonIgnoreNames;

        private readonly Dictionary<string, object> _extendDictionary;

        public Entity()
        {
            _type = GetType();
            _propertyNames = ReflectionUtils.GetPropertyNames(_type);
            _columnNames = ReflectionUtils.GetColumnNames(_type);
            _dataIgnoreNames = ReflectionUtils.GetDataIgnoreNames(_type);
            _jsonIgnoreNames = ReflectionUtils.GetJsonIgnoreNames(_type);
            _extendDictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public Entity(Type type)
        {
            _type = type;
            _propertyNames = ReflectionUtils.GetPropertyNames(_type);
            _columnNames = ReflectionUtils.GetColumnNames(_type);
            _dataIgnoreNames = ReflectionUtils.GetDataIgnoreNames(_type);
            _jsonIgnoreNames = ReflectionUtils.GetJsonIgnoreNames(_type);
            _extendDictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public Entity(IDictionary<string, object> dict) : this()
        {
            LoadDict(dict);
        }

        public void LoadExtend()
        {
            LoadExtend(ExtendValues);
        }

        public void LoadExtend(string extendValue)
        {
            if (string.IsNullOrEmpty(extendValue)) return;

            var dict = Utilities.ToDictionary(extendValue);
            if (dict == null) return;

            foreach (var o in dict)
            {
                if (!Utilities.ContainsIgnoreCase(_columnNames, o.Key))
                {
                    Set(o.Key, o.Value);
                }
            }
        }

        public void LoadDict(IDictionary<string, object> dict)
        {
            if (dict == null) return;

            foreach (var o in dict)
            {
                Set(o.Key, o.Value);
            }
        }

        public List<string> GetPropertyNames()
        {
            return _propertyNames;
        }

        public List<string> GetColumnNames()
        {
            return new List<string>(_columnNames);
        }

        public string GetExtendColumnValue()
        {
            var excludeKeys = GetColumnNames();
            excludeKeys.AddRange(GetDataIgnoreNames());
            return Utilities.JsonSerialize(ToDictionary(excludeKeys));
        }

        public List<string> GetDataIgnoreNames()
        {
            return new List<string>(_dataIgnoreNames);
        }

        public List<string> GetJsonIgnoreNames()
        {
            return new List<string>(_jsonIgnoreNames);
        }

        public ICollection<string> GetKeys(bool excludeProperties = false, bool excludeDatabase = false)
        {
            var keys = new List<string>();
            keys.AddRange(_extendDictionary.Keys);

            if (!excludeProperties)
            {
                foreach (var key in _propertyNames)
                {
                    if (keys.Contains(key, StringComparer.OrdinalIgnoreCase) || excludeDatabase && _columnNames.Contains(key, StringComparer.OrdinalIgnoreCase)) continue;

                    keys.Add(key);
                }
            }

            return keys;
        }

        public IDictionary<string, object> ToDictionary()
        {
            return ToDictionary(_jsonIgnoreNames);
        }

        public IDictionary<string, object> ToDictionary(List<string> excludeKeys)
        {
            var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            foreach (var key in _extendDictionary.Keys)
            {
                if (excludeKeys != null && excludeKeys.Contains(key, StringComparer.OrdinalIgnoreCase)) continue;

                dict[key] = Get(key);
            }
            foreach (var key in _propertyNames)
            {
                if (Utilities.IsExtend(key)) continue;
                if (excludeKeys != null && excludeKeys.Contains(key, StringComparer.OrdinalIgnoreCase)) continue;

                dict[key] = Get(key);
            }

            return dict;
        }

        public bool ContainsKey(string key)
        {
            return _extendDictionary.ContainsKey(key) || _propertyNames.Contains(key, StringComparer.OrdinalIgnoreCase);
        }

        private bool ContainsIgnoreCase(IEnumerable<string> list, string name, out string realName)
        {
            realName = null;
            foreach (var x in list)
            {
                if (!StringComparer.OrdinalIgnoreCase.Equals(x, name)) continue;

                realName = x;
                return true;
            }

            return false;
        }

        public void Set(string name, object value)
        {
            if (string.IsNullOrEmpty(name)) return;

            if (ContainsIgnoreCase(_propertyNames, name, out var realName))
            {
                ValueUtils.SetValue(this, realName, value);
            }
            else
            {
                _extendDictionary[name] = value;
            }
        }

        public object Get(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;

            if (Utilities.IsExtend(name))
            {
                return GetExtendColumnValue();
            }

            return ContainsIgnoreCase(_propertyNames, name, out var realName)
                ? ValueUtils.GetValue(this, realName)
                : Utilities.Get(_extendDictionary, name);
        }

        public T Get<T>(string name, T defaultValue = default)
        {
            return Utilities.Get(Get(name), defaultValue);
        }

        public object Clone()
        {
            var dict = ToDictionary();
            var instance = Activator.CreateInstance(_type);
            ((Entity)instance).LoadDict(dict);
            return instance;
        }

        public T Clone<T>() where T : Entity
        {
            var dict = ToDictionary();
            var instance = (T)Activator.CreateInstance(_type);
            instance.LoadDict(dict);
            return instance;
        }
    }

    public class EntityBaseConverter : JsonConverter
    {
        /// <summary>
        /// 确定此实例是否可以转换指定的对象类型。
        /// </summary>
        /// <param name="objectType">对象实例</param>
        /// <returns>
        /// <c>true</c> 如果这个实例可以转换指定的对象类型; 否则, <c>false</c>。
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            return typeof(Entity).IsAssignableFrom(objectType);
        }

        /// <summary>
        /// 编写对象的JSON表示。
        /// </summary>
        /// <param name="writer">JsonWriter</param>
        /// <param name="value">值</param>
        /// <param name="serializer">序列化类</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var entity = value as Entity;
            serializer.Serialize(writer, entity?.ToDictionary());
        }

        ///// <summary>
        ///// 读取对象的JSON表示。
        ///// </summary>
        ///// <param name="reader">JsonReader</param>
        ///// <param name="objectType">对象类型</param>
        ///// <param name="existingValue">正在读取的对象的现有值</param>
        ///// <param name="serializer">序列化类</param>
        ///// <returns>返回对象</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            //var dict = serializer.Deserialize<Dictionary<string, object>>(reader);
            //var instance = Activator.CreateInstance(objectType);
            //((Entity)instance).LoadDict(dict);
            //return instance;

            var instance = Activator.CreateInstance(objectType);
            var dict = serializer.Deserialize<Dictionary<string, object>>(reader);
            ((Entity)instance).LoadDict(dict);
            return instance;

            //var value = (string)reader.Value;
            //if (!string.IsNullOrEmpty(value))
            //{
            //    var instance = Activator.CreateInstance(objectType);
            //    ((EntityBase)instance).Sync(value);
            //    return instance;
            //}

            //return null;


            //return string.IsNullOrEmpty(value) ? null : serializer.Deserialize<Type>(reader);
        }
    }
}
