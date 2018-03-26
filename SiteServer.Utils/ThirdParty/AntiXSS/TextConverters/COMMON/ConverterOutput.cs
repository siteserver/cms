// ***************************************************************
// <copyright file="ConverterOutput.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters
{
    using System;


    internal interface IFallback
    {
        byte[] GetUnsafeAsciiMap(out byte unsafeAsciiMask);

        bool HasUnsafeUnicode();
        bool TreatNonAsciiAsUnsafe(string charset);
        bool IsUnsafeUnicode(char ch, bool isFirstChar);

        bool FallBackChar(char ch, char[] outputBuffer, ref int outputBufferCount, int lineBufferEnd);
    }

    
    
    internal abstract class ConverterOutput : ITextSink, IDisposable
    {
        
        
        protected char[] stringBuffer;

        protected const int stringBufferMax = 128; 
        protected const int stringBufferReserve = 20;
        protected const int stringBufferThreshold = stringBufferMax - stringBufferReserve;

        
        private IFallback fallback;

        
        public ConverterOutput()
        {
            stringBuffer = new char[stringBufferMax];
        }

        public abstract bool CanAcceptMore { get; }

        
        
        
        
        
        
        
        public abstract void Write(char[] buffer, int offset, int count, IFallback fallback);

        
        
        public abstract void Flush();

        
        
        
        
        
        
        public void Write(char[] buffer, int offset, int count)
        {
            Write(buffer, offset, count, null);
        }

        
        
        
        
        public virtual void Write(string text)
        {
            
            Write(text, 0, text.Length, null);
        }

        
        
        
        
        
        public void Write(string text, IFallback fallback)
        {
            Write(text, 0, text.Length, fallback);
        }

        
        
        
        
        
        
        public void Write(string text, int offset, int count)
        {
            Write(text, offset, count, null);
        }

        
        
        
        
        
        
        
        public void Write(string text, int offset, int count, IFallback fallback)
        {
            

            
            
            
            
            
            
            
            
            
            

            if (stringBuffer.Length < count)
            {
                
                stringBuffer = new char[count * 2];
            }

            text.CopyTo(offset, stringBuffer, 0, count);

            Write(stringBuffer, 0, count, fallback);
        }

        
        
        
        
        public void Write(char ch)
        {
            Write(ch, null);
        }

        
        
        
        
        
        public void Write(char ch, IFallback fallback)
        {
            stringBuffer[0] = ch;
            Write(stringBuffer, 0, 1, fallback);
        }

        
        
        
        
        public void Write(int ucs32Literal)
        {
            Write(ucs32Literal, null);
        }

        
        
        
        
        
        public void Write(int ucs32Literal, IFallback fallback)
        {
            if (ucs32Literal > 0xFFFF)
            {
                stringBuffer[0] = ParseSupport.HighSurrogateCharFromUcs4(ucs32Literal);
                stringBuffer[1] = ParseSupport.LowSurrogateCharFromUcs4(ucs32Literal);
            }
            else
            {
                stringBuffer[0] = (char)ucs32Literal;
            }

            Write(stringBuffer, 0, ucs32Literal > 0xFFFF ? 2 : 1, fallback);
        }

        
        public ITextSink PrepareSink(IFallback fallback)
        {
            this.fallback = fallback;
            return this as ITextSink;
        }

        
        bool ITextSink.IsEnough => false;

        void ITextSink.Write(char[] buffer, int offset, int count)
        {
            Write(buffer, offset, count, fallback);
        }

        void ITextSink.Write(int ucs32Literal)
        {
            Write(ucs32Literal, fallback);
        }

        
        
        void IDisposable.Dispose()
        {
            Dispose();
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose()
        {
        }
    }
}
