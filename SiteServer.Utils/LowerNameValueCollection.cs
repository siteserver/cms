using System.Collections.Generic;
using System.Collections.Specialized;

namespace SiteServer.Utils
{
    public class LowerNameValueCollection
    {
        private readonly NameValueCollection _nvcLower;

        public LowerNameValueCollection()
        {
            Keys = new List<string>();
            _nvcLower = new NameValueCollection();
        }

        public void Set(string name, string value)
        {
            if (string.IsNullOrEmpty(name)) return;

            _nvcLower.Set(name.ToLower(), value);
            if (!Keys.Contains(name))
            {
                Keys.Add(name);
            }
        }

        public string Get(string name)
        {
            return string.IsNullOrEmpty(name) ? null : _nvcLower.Get(name.ToLower());
        }

        public void Remove(string name)
        {
            if (string.IsNullOrEmpty(name)) return;

            _nvcLower.Remove(name.ToLower());

            Keys.Remove(name);
        }

        public List<string> Keys { get; }

        public int Count => Keys.Count;
    }
}
