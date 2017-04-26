using System;
using System.Runtime.Serialization;

namespace Top.Api
{
    /// <summary>
    /// TOP客户端异常。
    /// </summary>
    public class TopException : Exception
    {
        private string errorCode;
        private string errorMsg;
        private string subErrorCode;
        private string subErrorMsg;

        public TopException()
            : base()
        {
        }

        public TopException(string message)
            : base(message)
        {
        }

        protected TopException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public TopException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public TopException(string errorCode, string errorMsg)
            : base(errorCode + ":" + errorMsg)
        {
            this.errorCode = errorCode;
            this.errorMsg = errorMsg;
        }

        public TopException(string errorCode, string errorMsg, string subErrorCode, string subErrorMsg)
            : base(errorCode + ":" + errorMsg + ":" + subErrorCode + ":" + subErrorMsg)
        {
            this.errorCode = errorCode;
            this.errorMsg = errorMsg;
            this.subErrorCode = subErrorCode;
            this.subErrorMsg = subErrorMsg;
        }

        public string ErrorCode => this.errorCode;

        public string ErrorMsg => this.errorMsg;

        public string SubErrorCode => this.subErrorCode;

        public string SubErrorMsg => this.subErrorMsg;
    }
}
