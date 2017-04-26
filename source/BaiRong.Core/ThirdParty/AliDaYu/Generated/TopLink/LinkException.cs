using System;

namespace Taobao.Top.Link
{
    public class LinkException : Exception
    {
        public int ErrorCode { get; private set; }
        public LinkException() : this(string.Empty) { }
        public LinkException(string message) : this(message, null) { }
        public LinkException(string message, Exception innerException) : this(0, message, innerException) { }
        public LinkException(int errorCode, string message) : this(0, message, null) { }
        public LinkException(int errorCode, string message, Exception innerException)
            : base(message, innerException)
        {
            this.ErrorCode = errorCode;
        }
    }
}