// ***************************************************************
// <copyright file="GlobalizationExceptions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.Globalization
{
    using System;
    using System.Runtime.Serialization;
    using GlobalizationStrings = CtsResources.GlobalizationStrings;

    
    
    
    
    [Serializable()]
    internal class InvalidCharsetException : ExchangeDataException
    {
        private int codePage;
        private string charsetName;

        
        
        public InvalidCharsetException(int codePage) :
            base(GlobalizationStrings.InvalidCodePage(codePage))
        {
            this.codePage = codePage;
        }

        
        
        public InvalidCharsetException(string charsetName) :
            base(GlobalizationStrings.InvalidCharset(charsetName == null ? "<null>" : charsetName))
        {
            this.charsetName = charsetName;
        }

        
        
        
        public InvalidCharsetException(int codePage, string message) :
            base(message)
        {
            this.codePage = codePage;
        }

        
        
        
        public InvalidCharsetException(string charsetName, string message) :
            base(message)
        {
            this.charsetName = charsetName;
        }

        
        
        
        
        internal InvalidCharsetException(string charsetName, int codePage, string message) :
            base(message)
        {
            this.codePage = codePage;
            this.charsetName = charsetName;
        }

        
        
        
        
        public InvalidCharsetException(int codePage, string message, Exception innerException) :
            base(message, innerException)
        {
            this.codePage = codePage;
        }

        
        
        
        
        public InvalidCharsetException(string charsetName, string message, Exception innerException) :
            base(message, innerException)
        {
            this.charsetName = charsetName;
        }

        
        
        
        protected InvalidCharsetException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
            codePage = info.GetInt32("codePage");
            charsetName = info.GetString("charsetName");
        }

        
        
        
        
        public int CodePage => codePage;


        public string CharsetName => charsetName;
    }

    
    
    
    
    [Serializable()]
    internal class CharsetNotInstalledException : InvalidCharsetException
    {
        
        
        public CharsetNotInstalledException(int codePage) :
            base(codePage, GlobalizationStrings.NotInstalledCodePage(codePage))
        {
        }

        
        
        public CharsetNotInstalledException(string charsetName) :
            base(charsetName, GlobalizationStrings.NotInstalledCharset(charsetName == null ? "<null>" : charsetName))
        {
        }

        
        
        
        internal CharsetNotInstalledException(string charsetName, int codePage) :
            base(charsetName, codePage, GlobalizationStrings.NotInstalledCharsetCodePage(codePage, charsetName == null ? "<null>" : charsetName))
        {
        }

        
        
        
        public CharsetNotInstalledException(int codePage, string message) :
            base(codePage, message)
        {
        }

        
        
        
        public CharsetNotInstalledException(string charsetName, string message) :
            base(charsetName, message)
        {
        }

        
        
        
        
        public CharsetNotInstalledException(int codePage, string message, Exception innerException) :
            base(codePage, message, innerException)
        {
        }

        
        
        
        
        public CharsetNotInstalledException(string charsetName, string message, Exception innerException) :
            base(charsetName, message, innerException)
        {
        }

        
        
        
        protected CharsetNotInstalledException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }
    }

    
    
    
    
    [Serializable()]
    internal class UnknownCultureException : ExchangeDataException
    {
        private int localeId;
        private string cultureName;

        
        
        public UnknownCultureException(int localeId) :
            base(GlobalizationStrings.InvalidLocaleId(localeId))
        {
            this.localeId = localeId;
        }

        
        
        public UnknownCultureException(string cultureName) :
            base(GlobalizationStrings.InvalidCultureName(cultureName == null ? "<null>" : cultureName))
        {
            this.cultureName = cultureName;
        }

        
        
        
        public UnknownCultureException(int localeId, string message) :
            base(message)
        {
            this.localeId = localeId;
        }

        
        
        
        public UnknownCultureException(string cultureName, string message) :
            base(message)
        {
            this.cultureName = cultureName;
        }

        
        
        
        
        public UnknownCultureException(int localeId, string message, Exception innerException) :
            base(message, innerException)
        {
            this.localeId = localeId;
        }

        
        
        
        
        public UnknownCultureException(string cultureName, string message, Exception innerException) :
            base(message, innerException)
        {
            this.cultureName = cultureName;
        }

        
        
        
        protected UnknownCultureException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
            localeId = info.GetInt32("localeId");
            cultureName = info.GetString("cultureName");
        }

       
        
        
        
        public int LocaleId => localeId;


        public string CultureName => cultureName;
    }

}
