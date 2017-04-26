#if net4
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace fastJSON
{
    internal class DynamicJson : DynamicObject
    {
        private IDictionary<string, object> _dictionary { get; set; }
        private List<object> _list { get; set; }

        public DynamicJson(string json)
        {
            var parse = fastJSON.JSON.Parse(json);

            if (parse is IDictionary<string, object>)
                _dictionary = (IDictionary<string, object>)parse;
            else
                _list = (List<object>)parse;
        }

        private DynamicJson(object dictionary)
        {
            if (dictionary is IDictionary<string, object>)
                _dictionary = (IDictionary<string, object>)dictionary;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _dictionary.Keys.ToList();
        }

        public override bool TryGetIndex(GetIndexBinder binder, Object[] indexes, out Object result)
        {
            var index = indexes[0];
            if (index is int)
            {
                result = _list[(int) index];
            }
            else
            {
                result = _dictionary[(string) index];
            } 
            if (result is IDictionary<string, object>)
                result = new DynamicJson(result as IDictionary<string, object>);
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (_dictionary.TryGetValue(binder.Name, out result) == false)
                if (_dictionary.TryGetValue(binder.Name.ToLower(), out result) == false)
                    return false;// throw new Exception("property not found " + binder.Name);

            if (result is IDictionary<string, object>)
            {
                result = new DynamicJson(result as IDictionary<string, object>);
            }
            else if (result is List<object>)
            {
                List<object> list = new List<object>();
                foreach (object item in (List<object>)result)
                {
                    if (item is IDictionary<string, object>)
                        list.Add(new DynamicJson(item as IDictionary<string, object>));
                    else
                        list.Add(item);
                }
                result = list;
            }

            return _dictionary.ContainsKey(binder.Name);
        }
    }
}
#endif