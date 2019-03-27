using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Newtonsoft.Json;

namespace SiteServer.CMS.Database.Wrapper
{
    public class DynamicWrapper<T> : DynamicObject where T : class, new()
    {
        public DynamicWrapper(T obj, bool isJsonIgnore = false)
        {
            if (obj == null) return;

            foreach (var property in typeof(T).GetProperties())
            {
                if (!property.CanRead) continue;
                if (isJsonIgnore && property.GetCustomAttributes(true).Any(a => a is JsonIgnoreAttribute)) continue;

                Set(property.Name, property.GetValue(obj, null));
            }
        }

        public DynamicWrapper(Dictionary<string, object> dict)
        {
            if (dict == null) return;

            foreach (var key in dict.Keys)
            {
                Set(key, dict[key]);
            }
        }

        // The inner dictionary.
        private readonly Dictionary<string, object> _dictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public IDictionary<string, object> ToDictionary()
        {
            var ret = new Dictionary<string, object>(_dictionary.Count, _dictionary.Comparer);
            foreach (var entry in _dictionary)
            {
                ret.Add(entry.Key, entry.Value);
            }
            return ret;
        }

        public void Remove(string key)
        {
            if (string.IsNullOrEmpty(key)) return;

            _dictionary.Remove(key);
        }

        public void Set(string name, object value)
        {
            if (string.IsNullOrEmpty(name)) return;

            _dictionary[name] = value;
        }

        public bool ContainsKey(string key)
        {
            if (string.IsNullOrEmpty(key)) return false;

            return _dictionary.ContainsKey(key);
        }

        public object Get(string key)
        {
            if (string.IsNullOrEmpty(key)) return null;

            return _dictionary.TryGetValue(key, out var value) ? value : null;
        }

        // If you try to get a value of a property 
        // not defined in the class, this method is called.
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            // Converting the property name to lowercase
            // so that property names become case-insensitive.
            var name = binder.Name;

            // If the property name is found in a dictionary,
            // set the result parameter to the property value and return true.
            // Otherwise, return false.
            return _dictionary.TryGetValue(name, out result);
        }

        // If you try to set a value of a property that is
        // not defined in the class, this method is called.
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            // Converting the property name to lowercase
            // so that property names become case-insensitive.
            _dictionary[binder.Name] = value;

            // You can always add a value to a dictionary,
            // so this method always returns true.
            return true;
        }
    }
}
