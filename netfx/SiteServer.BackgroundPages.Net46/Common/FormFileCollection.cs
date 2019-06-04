using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace SiteServer.BackgroundPages.Common
{
    public class FormFileCollection : IFormFileCollection
    {
        private readonly HttpFileCollection _files;
        private readonly List<FormFile> _formFileList = new List<FormFile>();

        public FormFileCollection(HttpFileCollection files)
        {
            _files = files;
            foreach (HttpPostedFile httpPostedFile in _files)
            {
                _formFileList.Add(new FormFile(httpPostedFile));
            }
        }

        public IEnumerator<IFormFile> GetEnumerator()
        {
            return _formFileList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _formFileList.GetEnumerator();
        }

        public int Count => _formFileList.Count;

        public IFormFile this[int index] => _formFileList[index];

        public IFormFile GetFile(string name)
        {
            return _formFileList.FirstOrDefault(x => x.FileName == name);
        }

        public IReadOnlyList<IFormFile> GetFiles(string name)
        {
            return _formFileList.FindAll(x => x.FileName == name);
        }

        public IFormFile this[string name] => _formFileList.FirstOrDefault(x => x.FileName == name);
    }
}
