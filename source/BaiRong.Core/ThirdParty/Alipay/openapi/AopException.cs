using System;
using System.Runtime.Serialization;

namespace Aop.Api
{
    /// <summary>
    /// AOP客户端异常。
    /// </summary>
    public class AopException : Exception
    {
        private string errorCode;
        private string errorMsg;

        public AopException()
            : base()
        {
        }

        public AopException(string message)
            : base(message)
        {
        }

        protected AopException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public AopException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public AopException(string errorCode, string errorMsg)
            : base(errorCode + ":" + errorMsg)
        {
            this.errorCode = errorCode;
            this.errorMsg = errorMsg;
        }

        public string ErrorCode
        {
            get { return this.errorCode; }
        }

        public string ErrorMsg
        {
            get { return this.errorMsg; }
        }
    }
}
