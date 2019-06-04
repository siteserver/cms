using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace SiteServer.BackgroundPages.Common
{
    public class FormFile : IFormFile
    {
        public readonly HttpPostedFile _file;

        public FormFile(HttpPostedFile file)
        {
            _file = file;
        }

        public Stream OpenReadStream()
        {
            return _file.InputStream;
        }

        public void CopyTo(Stream target)
        {

        }

        public Task CopyToAsync(Stream target, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public string ContentType { get; }
        public string ContentDisposition { get; }
        public IHeaderDictionary Headers { get; }
        public long Length { get; }
        public string Name { get; }
        public string FileName { get; }
    }
}
