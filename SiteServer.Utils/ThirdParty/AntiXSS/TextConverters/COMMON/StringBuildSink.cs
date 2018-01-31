// ***************************************************************
// <copyright file="StringBuildSink.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters
{
    using System;
    using System.Text;
    using Data.Internal;

    

    internal class StringBuildSink : ITextSinkEx
    {
        private StringBuilder sb;
        int maxLength;

        public StringBuildSink()
        {
            
            
            
            

            sb = new StringBuilder();
        }

        public bool IsEnough => sb.Length >= maxLength;

        public void Reset(int maxLength)
        {
            this.maxLength = maxLength;
            sb.Length = 0;
        }

        public void Write(char[] buffer, int offset, int count)
        {
            InternalDebug.Assert(!IsEnough);

            count = Math.Min(count, maxLength - sb.Length);
            sb.Append(buffer, offset, count);
        }

        public void Write(int ucs32Char)
        {
            InternalDebug.Assert(!IsEnough);

            if (Token.LiteralLength(ucs32Char) == 1)
            {
                sb.Append((char)ucs32Char);
            }
            else
            {
                sb.Append(Token.LiteralFirstChar(ucs32Char));
                if (!IsEnough)
                {
                    sb.Append(Token.LiteralLastChar(ucs32Char));
                }
            }
        }

        public void Write(string value)
        {
            InternalDebug.Assert(!IsEnough);

            
            sb.Append(value);
        }

        public void WriteNewLine()
        {
            InternalDebug.Assert(!IsEnough);

            sb.Append('\r');

            if (!IsEnough)
            {
                sb.Append('\n');
            }
        }

        public override string ToString()
        {
            return sb.ToString();
        }
    }
}

