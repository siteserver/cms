using System;
using System.Net;
using Pingpp.Models;

namespace Pingpp.Exception
{
    [Serializable]
    public class PingppException : ApplicationException
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public Error Error { get; set; }

        public PingppException()
        {
        }

        public PingppException(string message)
            : base(message)
        {
        }

        public PingppException(Error pingppError, HttpStatusCode httpStatusCode, string type = null, string message = null)
            : base(message)
        {
            HttpStatusCode = httpStatusCode;
            Error = pingppError;
        }

    }
}
