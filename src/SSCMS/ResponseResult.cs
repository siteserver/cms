using System.Net;

namespace SSCMS
{
    public class ResponseResult<T>
    {
        public HttpStatusCode StatusCode { get; set; }

        public string ContentType { get; set; }

        public T Body { get; set; }
    }
}
