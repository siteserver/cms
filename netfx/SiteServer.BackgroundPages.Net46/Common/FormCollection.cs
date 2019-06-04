using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AngleSharp.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.Owin;
using HttpContext = System.Web.HttpContext;
using IFormCollection = Microsoft.AspNetCore.Http.IFormCollection;

namespace SiteServer.BackgroundPages.Common
{
    public class FormCollection : IFormCollection
    {
        private readonly Microsoft.Owin.IFormCollection _formCollection;
        private readonly HttpFileCollection _files;

        public FormCollection(Microsoft.Owin.IFormCollection formCollection, HttpFileCollection files)
        {
            _formCollection = formCollection;
            _files = files;
        }

        public IEnumerator<KeyValuePair<string, StringValues>> GetEnumerator()
        {
            return _formCollection.Select(x => new KeyValuePair<string, StringValues>(x.Key, x.Value)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool ContainsKey(string key)
        {
            return _formCollection.Any(x => x.Key == key);
        }

        public bool TryGetValue(string key, out StringValues value)
        {
            value = StringValues.Empty;
            if (!ContainsKey(key)) return false;
            value = _formCollection.Get(key);
            return true;
        }

        public int Count => _formCollection.Count();
        public ICollection<string> Keys => _formCollection.Select(x => x.Key).ToList();

        public StringValues this[string key] => _formCollection[key];

        public IFormFileCollection Files => new FormFileCollection(_files);
    }
}
