using System;
using System.Runtime.Serialization;

namespace Top.Api.Security
{

    public class SecretException : TopException
    {
        public SecretException()
            : base()
        {
        }

        public SecretException(string message)
            : base(message)
        {
        }

        protected SecretException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public SecretException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public SecretException(string errorCode, string errorMsg)
            : base(errorCode , errorMsg)
        {
        }

        public SecretException(string errorCode, string errorMsg, string subErrorCode, string subErrorMsg)
            : base(errorCode, errorMsg, subErrorCode, subErrorMsg)
        {
        }
        
    }
}
