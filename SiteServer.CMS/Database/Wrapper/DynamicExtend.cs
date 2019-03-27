using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using SiteServer.CMS.Database.Core;
using SiteServer.Utils;

namespace SiteServer.CMS.Database.Wrapper
{
    public class DynamicExtend<TEntity, TExtend> : DynamicObject where TEntity : class, IEntity, new() where TExtend : class, IExtend, new()
    {
        private const string AttrExtend1 = "SettingsXml";
        private const string AttrExtend2 = "ExtendValues";

        public TEntity Entity { get; }
        public TExtend Extend { get; }
        private readonly List<string> _entityAttributes = new List<string>();
        private readonly List<string> _extendAttributes = new List<string>();
        
        public DynamicExtend(TEntity obj)
        {
            if (obj == null) return;

            Entity = obj;

            _entityAttributes = ReflectionUtils.GetTypeProperties(typeof(TEntity)).Select(x => x.Name).ToList();
            _extendAttributes = ReflectionUtils.GetTypeProperties(typeof(TExtend)).Select(x => x.Name).ToList();

            var extendAttributeName = _entityAttributes.FirstOrDefault(x =>
                StringUtils.EqualsIgnoreCase(x, AttrExtend1) ||
                StringUtils.EqualsIgnoreCase(x, AttrExtend2));
            if (!string.IsNullOrEmpty(extendAttributeName))
            {
                var json = ReflectionUtils.GetValue<string>(obj, extendAttributeName);
                if (string.IsNullOrEmpty(json))
                {
                    var extendDictionary = TranslateUtils.JsonDeserialize<Dictionary<string, object>>(json);
                    Extend = ReflectionUtils.ToObject<TExtend>(extendDictionary);
                }
            }

            //var tableColumns = ReflectionUtils.GetTableColumns(typeof(T));

            //var extendColumn = tableColumns.FirstOrDefault(x =>
            //    StringUtils.EqualsIgnoreCase(x.AttributeName, AttrExtend1) ||
            //    StringUtils.EqualsIgnoreCase(x.AttributeName, AttrExtend2));
            //if (extendColumn != null)
            //{
            //    var json = ReflectionUtils.GetValue<string>(obj, extendColumn.AttributeName);
            //    if (string.IsNullOrEmpty(json))
            //    {
            //        var extendDictionary = TranslateUtils.JsonDeserialize<Dictionary<string, object>>(json);
            //        Extend = ReflectionUtils.ToObject<E>(extendDictionary);
            //    }
            //}
        }

        // If you try to get a value of a property 
        // not defined in the class, this method is called.
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            // Converting the property name to lowercase
            // so that property names become case-insensitive.
            var attributeName = binder.Name;

            if (_entityAttributes.Contains(attributeName, StringComparer.OrdinalIgnoreCase))
            {
                return ReflectionUtils.GetValue(Entity, attributeName, out result);

            }

            if (_extendAttributes.Contains(attributeName, StringComparer.OrdinalIgnoreCase))
            {
                return ReflectionUtils.GetValue(Extend, attributeName, out result);
            }

            // If the property name is found in a dictionary,
            // set the result parameter to the property value and return true.
            // Otherwise, return false.
            result = null;
            return true;
        }

        // If you try to set a value of a property that is
        // not defined in the class, this method is called.
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var attributeName = binder.Name;

            if (_entityAttributes.Contains(attributeName, StringComparer.OrdinalIgnoreCase))
            {
                ReflectionUtils.SetValue(Entity, attributeName, value);
            }
            else if (_extendAttributes.Contains(attributeName, StringComparer.OrdinalIgnoreCase))
            {
                ReflectionUtils.SetValue(Extend, attributeName, value);
            }
            //else
            //{
            //    // Converting the property name to lowercase
            //    // so that property names become case-insensitive.
            //    _dictionary[binder.Name] = value;
            //}

            // You can always add a value to a dictionary,
            // so this method always returns true.
            return true;
        }
    }
}
