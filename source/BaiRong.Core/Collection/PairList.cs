using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BaiRong.Core.Collection
{
    public class PairList
	{
        private readonly List<Pair> _list = new List<Pair>();

        public Pair GetPair(string key)
        {
            return _list.FirstOrDefault(pair => pair != null && pair.Key == key);
        }

        public object GetValue(string key)
        {
            object value = null;
            var pair = GetPair(key);
            if (pair != null)
            {
                value = pair.Value;
            }
            return value;
        }

        public void Add(Pair pair)
        {
            _list.Add(pair);
        }

        public void Insert(int index, Pair pair)
        {
            _list.Insert(index, pair);
        }

        public List<string> Keys
        {
            get
            {
                return _list.Select(pair => pair.Key).ToList();
            }
        }

        public bool ContainsKey(string key)
        {
            return _list.Any(pair => pair.Key == key);
        }

        public List<string> GetKeys(string startKey)
        {
            return (from pair in _list where pair.Key.StartsWith(startKey) select pair.Key).ToList();
        }
    }
}