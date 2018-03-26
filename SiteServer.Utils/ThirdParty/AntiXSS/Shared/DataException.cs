// ***************************************************************
// <copyright file="DataException.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data
{
    using System;
    using System.Runtime.Serialization;

    
    [Serializable()]
#if EXCHANGECOMMONEXCEPTIONS
    internal class ExchangeDataException : Microsoft.Exchange.Data.Common.LocalizedException
#else
    internal class ExchangeDataException : Exception
#endif
    {
        
        
        
        
        public ExchangeDataException(string message) :
#if EXCHANGECOMMONEXCEPTIONS
            base(new Microsoft.Exchange.Data.Common.LocalizedString(message))
#else
            base(message)
#endif
        {
        }

        
        
        
        
        
        public ExchangeDataException(string message, Exception innerException) :
#if EXCHANGECOMMONEXCEPTIONS
            base(new Microsoft.Exchange.Data.Common.LocalizedString(message), innerException)
#else
            base(message, innerException)
#endif
        {
        }

        
        
        
        
        
        protected ExchangeDataException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }
    }
}

