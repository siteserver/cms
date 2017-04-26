// ***************************************************************
// <copyright file="TextCache.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************
namespace Microsoft.Exchange.Data.TextConverters
{
#if PRIVATEBUILD

    using System;
    using System.IO;
    using System.Text;
    using Strings = Microsoft.Exchange.CtsResources.TextConvertersStrings;

    

    internal class TextWriterSink : ITextSink
    {
        protected int maxSize;
        protected int currentSize;
        protected TextWriter writer;

        public TextWriterSink(TextWriter sink, int maxSize)
        {
            this.writer = sink;
            this.maxSize = maxSize;
        }

        
        

        public bool IsEnough { get { return this.currentSize >= this.maxSize; } }

        public void Write(char[] buffer, int offset, int count)
        {
            this.writer.Write(buffer, offset, count);
            this.currentSize += count;
        }

        public void Write(int ucs32Char)
        {
            this.writer.Write(Token.LiteralFirstChar(ucs32Char));
            this.currentSize ++;

            if (Token.LiteralLength(ucs32Char) > 1)
            {
                this.writer.Write(Token.LiteralLastChar(ucs32Char));
                this.currentSize ++;
            }
        }

        public void Write(string value)
        {
            
        }

        public void WriteNewLine()
        {
            
        }

        public void Flush()
        {
            this.writer.Flush();
        }

        public void BreakLine()
        {
            this.writer.Write("\n");
        }

    }

#endif
}
