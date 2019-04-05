using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Datory.Utils;
using Newtonsoft.Json;

namespace Datory
{
    [JsonConverter(typeof(EntityBaseConverter))]
    public class Entity : DynamicObject, IEntity
    {
        [TableColumn]
        public int Id { get; set; }

        [TableColumn(Length = 50)]
        public string Guid { get; set; }

        [TableColumn]
        public DateTime? LastModifiedDate { get; set; }

        private readonly List<string> _propertyNames;

        private readonly List<string> _columnNames;

        private readonly string _extendColumnName;

        private readonly Dictionary<string, object> _extendDictionary;

        protected Entity()
        {
            var type = GetType();
            _propertyNames = ReflectionUtils.GetPropertyNames(type);
            _columnNames = ReflectionUtils.GetColumnNames(type);
            _extendColumnName = ReflectionUtils.GetTableExtendColumnName(type);
            _extendDictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        protected Entity(IDictionary<string, object> dict): this()
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
            return _columnNames;
        }

        public string GetExtendColumnName()
        {
            return _extendColumnName;
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

        public IDictionary<string, object> ToDictionary(List<string> excludeKeys = null)
        {
            var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            foreach (var key in _extendDictionary.Keys)
            {
                if (excludeKeys != null && excludeKeys.Contains(key, StringComparer.OrdinalIgnoreCase)) continue;

                dict[key] = Get(key);
            }
            foreach (var key in _propertyNames)
            {
                if (excludeKeys != null && excludeKeys.Contains(key, StringComparer.OrdinalIgnoreCase)) continue;

                dict[key] = Get(key);
            }

            return dict;
        }

        public bool ContainsKey(string key)
        {
            return _extendDictionary.ContainsKey(key) || _propertyNames.Contains(key, StringComparer.OrdinalIgnoreCase);
        }

        public void Sync(string json)
        {
            var dict = ConvertUtils.ToDictionary(json);
            Sync(dict);
        }

        public void Sync(Dictionary<string, object> dictionary)
        {
            if (dictionary == null) return;

            foreach (var o in dictionary)
            {
                //if (!_columnNames.Contains(o.Key, StringComparer.OrdinalIgnoreCase))
                //{
                    Set(o.Key, o.Value);
                //}
            }
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
                ReflectionUtils.SetValue(this, realName, value);
            }
            else
            {
                _extendDictionary[name] = value;
            }



            //if (!string.IsNullOrEmpty(_extendAttribute))
            //{
            //    ReflectionUtils.SetValue(this, _extendAttribute, TranslateUtils.JsonSerialize(Dictionary));
            //}

            //if (StringUtils.EqualsIgnoreCase(_extendAttribute, name))
            //{
            //    var dict = GetExtendValue(value as string);
            //    if (dict != null)
            //    {
            //        foreach (var entry in dict)
            //        {
            //            if (!_entityAttributes.Contains(entry.Key, StringComparer.OrdinalIgnoreCase))
            //            {
            //                Dictionary[entry.Key] = entry.Value;
            //            }
            //        }
            //    }

            //    SetExtendValue();
            //}
            //else if (!_entityAttributes.Contains(name, StringComparer.OrdinalIgnoreCase))
            //{
            //    SetExtendValue();
            //}

            //if (_entityAttributes.Contains(name, StringComparer.OrdinalIgnoreCase))
            //{
            //    ReflectionUtils.SetValue(this, name, value);
            //}
            //else
            //{
            //    _dictionary[name] = value;
            //    if (!string.IsNullOrEmpty(_extendAttribute))
            //    {
            //        ReflectionUtils.SetValue(this, _extendAttribute, TranslateUtils.JsonSerialize(_dictionary));
            //    }
            //}
        }

        public object Get(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;

            return ContainsIgnoreCase(_propertyNames, name, out var realName)
                ? ReflectionUtils.GetValue(this, realName)
                : ConvertUtils.Get(_extendDictionary, name);
        }

        public T Get<T>(string name, T defaultValue = default(T))
        {
            return ConvertUtils.Get(Get(name), defaultValue);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = Get(binder.Name);
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            Set(binder.Name, value);
            return true;
        }
    }

    public class EntityBaseConverter : JsonConverter
    {
        //public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        //{
        //    throw new NotImplementedException();
        //}

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
            var dict = serializer.Deserialize<Dictionary<string, object>>(reader);
            var instance = Activator.CreateInstance(objectType);
            ((Entity)instance).Sync(dict);
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
