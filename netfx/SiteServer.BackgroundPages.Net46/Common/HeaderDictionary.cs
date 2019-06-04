using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace SiteServer.BackgroundPages.Common
{
    public class HeaderDictionary : IHeaderDictionary
    {
        private readonly Microsoft.Owin.IHeaderDictionary _headerDictionary;

        public HeaderDictionary(Microsoft.Owin.IHeaderDictionary headerDictionary)
        {
            _headerDictionary = headerDictionary;
        }

        //public IEnumerator<KeyValuePair<string, StringValues>> GetEnumerator()
        //{
        //    return _headerDictionary.ToList().Select(x => new KeyValuePair<string, StringValues>(x.Key, x.Value)).GetEnumerator();
        //}

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return GetEnumerator();
        //}

        //public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        //{
        //    return _cookies.GetEnumerator();
        //}

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return ((IEnumerable)this).GetEnumerator();
        //}

        public void Add(KeyValuePair<string, StringValues> item)
        {
            _headerDictionary.Add(new KeyValuePair<string, string[]>(item.Key, item.Value));
        }

        public void Clear()
        {
            _headerDictionary.Clear();
        }

        public bool Contains(KeyValuePair<string, StringValues> item)
        {
            return _headerDictionary.Contains(new KeyValuePair<string, string[]>(item.Key, item.Value));
        }

        public void CopyTo(KeyValuePair<string, StringValues>[] array, int arrayIndex)
        {
            _headerDictionary.CopyTo(array.Select(keyValuePair => new KeyValuePair<string, string[]>(keyValuePair.Key, keyValuePair.Value)).ToArray(), arrayIndex);
        }

        public bool Remove(KeyValuePair<string, StringValues> item)
        {
            return _headerDictionary.Remove(new KeyValuePair<string, string[]>(item.Key, item.Value));
        }

        public int Count => _headerDictionary.Count;
        public bool IsReadOnly => _headerDictionary.IsReadOnly;
        public bool ContainsKey(string key)
        {
            return _headerDictionary.ContainsKey(key);
        }

        public void Add(string key, StringValues value)
        {
            _headerDictionary.Add(key, value);
        }

        public bool Remove(string key)
        {
            return _headerDictionary.Remove(key);
        }

        public bool TryGetValue(string key, out StringValues value)
        {
            if (!_headerDictionary.TryGetValue(key, out var val)) return false;
            value = val;
            return true;
        }

        public StringValues this[string key]
        {
            get => _headerDictionary[key];
            set => _headerDictionary[key] = value;
        }

        public long? ContentLength { get; set; }
        public ICollection<string> Keys => _headerDictionary.Keys;
        public ICollection<StringValues> Values => _headerDictionary.Values.Select(x => new StringValues(x)).ToList();
        IEnumerator<KeyValuePair<string, StringValues>> IEnumerable<KeyValuePair<string, StringValues>>.GetEnumerator()
        {
            return _headerDictionary.ToList().Select(x => new KeyValuePair<string, StringValues>(x.Key, x.Value)).GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable) this).GetEnumerator();
        }
    }
}
