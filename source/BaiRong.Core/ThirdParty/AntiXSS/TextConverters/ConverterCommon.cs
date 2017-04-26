// ***************************************************************
// <copyright file="ConverterCommon.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters
{
    using System;
    using System.Runtime.Serialization;
    using Strings = CtsResources.TextConvertersStrings;

    
    
    
    internal enum HeaderFooterFormat
    {
        
        
        Text,
        
        Html,
    }

    
    
    
    
    [Serializable()]
    internal class TextConvertersException : ExchangeDataException
    {
        
        internal TextConvertersException() :                 
            base("internal text conversion error (document too complex)")
        {
        }

        
        
        public TextConvertersException(string message) :
            base(message)
        {
        }

        
        
        
        public TextConvertersException(string message, Exception innerException) :
            base(message, innerException)
        {
        }

        
        
        
        protected TextConvertersException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }
    }
}
