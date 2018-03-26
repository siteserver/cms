// ***************************************************************
// <copyright file="ScratchBuffer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters
{
    using System;
    
    
    using System.Diagnostics;
    using Data.Internal;
    
    using Internal.Html;
    using Internal.Css;
    

    

    internal struct BufferString
    {
        private char[] buffer;
        private int offset;
        private int count;

        public static readonly BufferString Null = new BufferString();

        public BufferString(char[] buffer, int offset, int count)
        {
            this.buffer = buffer;
            this.offset = offset;
            this.count = count;
        }

        public void Set(char[] buffer, int offset, int count)
        {
            this.buffer = buffer;
            this.offset = offset;
            this.count = count;
        }

        public char this[int index]
        {
            get
            {
                InternalDebug.Assert(index < count);
                return buffer[offset + index];
            }
        }

        public char[] Buffer => buffer;

        public int Offset => offset;

        public int Length => count;

        public bool IsEmpty => count == 0;

        public BufferString SubString(int offset, int count)
        {
            InternalDebug.Assert(offset >= 0 && offset <= this.count && count >= 0 && offset + count <= this.count);
            return new BufferString(buffer, this.offset + offset, count);
        }

        public void Trim(int offset, int count)
        {
            InternalDebug.Assert(offset >= 0 && offset <= this.count && count >= 0 && offset + count <= this.count);
            this.offset += offset;
            this.count = count;
        }

        public void TrimWhitespace()
        {
            
            while (count != 0 && ParseSupport.WhitespaceCharacter(buffer[offset]))
            {
                offset++;
                count --;
            }

            if (count != 0)
            {
                
                var end = offset + count - 1;
                while (ParseSupport.WhitespaceCharacter(buffer[end--]))
                {
                    count --;
                }
            }
        }

        public bool EqualsToString(string rightPart)
        {
            
            if (count != rightPart.Length)
            {
                return false;
            }


            for (var i = 0; i < rightPart.Length; i++)
            {
                if (buffer[offset + i] != rightPart[i])
                {
                    return false;
                }
            }

            return true;
        }

        public bool EqualsToLowerCaseStringIgnoreCase(string rightPart)
        {
            AssertStringIsLowerCase(rightPart);

            
            if (count != rightPart.Length)
            {
                return false;
            }


            for (var i = 0; i < rightPart.Length; i++)
            {
                if (ParseSupport.ToLowerCase(buffer[offset + i]) != rightPart[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static int CompareLowerCaseStringToBufferStringIgnoreCase(string left, BufferString right)
        {
            var len = Math.Min(left.Length, right.Length);

            for (var i = 0; i < len; i++)
            {
                var cmp = (int)left[i] - (int)ParseSupport.ToLowerCase(right[i]);
                if (cmp != 0)
                {
                    return cmp;
                }
            }

            
            
            return left.Length - right.Length;                            
        }

        public bool StartsWithLowerCaseStringIgnoreCase(string rightPart)
        {
            AssertStringIsLowerCase(rightPart);

            

            if (count < rightPart.Length)
            {
                return false;
            }

            for (var i = 0; i < rightPart.Length; i++)
            {
                if (ParseSupport.ToLowerCase(buffer[offset + i]) != rightPart[i])
                {
                    return false;
                }
            }

            return true;
        }

        public bool StartsWithString(string rightPart)
        {
            

            if (count < rightPart.Length)
            {
                return false;
            }

            for (var i = 0; i < rightPart.Length; i++)
            {
                if (buffer[offset + i] != rightPart[i])
                {
                    return false;
                }
            }

            return true;
        }

        public bool EndsWithLowerCaseStringIgnoreCase(string rightPart)
        {
            AssertStringIsLowerCase(rightPart);

            

            if (count < rightPart.Length)
            {
                return false;
            }

            var offset = this.offset + count - rightPart.Length;

            for (var i = 0; i < rightPart.Length; i++)
            {
                if (ParseSupport.ToLowerCase(buffer[offset + i]) != rightPart[i])
                {
                    return false;
                }
            }

            return true;
        }

        public bool EndsWithString(string rightPart)
        {
            

            if (count < rightPart.Length)
            {
                return false;
            }

            var offset = this.offset + count - rightPart.Length;

            for (var i = 0; i < rightPart.Length; i++)
            {
                if (buffer[offset + i] != rightPart[i])
                {
                    return false;
                }
            }

            return true;
        }

        public override string ToString()
        {
            return buffer == null ? null : count == 0 ? String.Empty : new String(buffer, offset, count);
        }

        [Conditional("DEBUG")]
        private static void AssertStringIsLowerCase(string rightPart)
        {
#if DEBUG
            foreach (var ch in rightPart)
            {
                if (ch > (char)0x7F || char.ToLowerInvariant(ch) != ch)
                {
                    InternalDebug.Assert(false, "right part string is supposed to be in lower case for this method.");
                    break;
                }
            }
#endif
        }
    }

    

    internal struct ScratchBuffer
    {
        private char[] buffer;
        private int count;

        public char[] Buffer => buffer;

        public int Offset => 0;

        public int Length
        {
            get { return count; }
            set { count = value; }
        }

        public int Capacity => buffer == null ? 64 : buffer.Length;

        public BufferString BufferString => new BufferString(buffer, 0, count);

        public BufferString SubString(int offset, int count)
        {
            InternalDebug.Assert(offset >= 0 && offset <= buffer.Length && count >= 0 && offset + count <= buffer.Length);
            return new BufferString(buffer, offset, count);
        }

        public char this[int offset]
        {
            get
            {
                InternalDebug.Assert(offset < buffer.Length);
                
                return buffer[offset];
            }
            set
            {
                InternalDebug.Assert(offset < buffer.Length);
                
                buffer[offset] = value;
            }
        }

        public void Reset()
        {
            count = 0;
        }

        public void Reset(int space)
        {
            count = 0;

            if (buffer == null || buffer.Length < space)
            {
                buffer = new char[space];
            }
        }

        
        
        
        

        
        
        
        
        
        

        
        

        public bool AppendTokenText(Token token, int maxSize)
        {
            int countRead;
            var countTotal = 0;

            while (0 != (countRead = GetSpace(maxSize)) && 
                0 != (countRead = token.Text.Read(buffer, count, countRead)))
            {
                count += countRead;
                countTotal += countRead;
            }

            return countTotal != 0;
        }

        public bool AppendHtmlAttributeValue(HtmlAttribute attr, int maxSize)
        {
            int countRead;
            var countTotal = 0;

            while (0 != (countRead = GetSpace(maxSize)) && 
                0 != (countRead = attr.Value.Read(buffer, count, countRead)))
            {
                count += countRead;
                countTotal += countRead;
            }

            return countTotal != 0;
        }

        public bool AppendCssPropertyValue(CssProperty prop, int maxSize)
        {
            int countRead;
            var countTotal = 0;

            while (0 != (countRead = GetSpace(maxSize)) && 
                0 != (countRead = prop.Value.Read(buffer, count, countRead)))
            {
                count += countRead;
                countTotal += countRead;
            }

            return countTotal != 0;
        }

        public int AppendInt(int value)
        {
            var len = 1;
            var negative = false;

            if (value < 0)
            {
                negative = true;
                value = -value;
                len ++;

                if (value < 0)
                {
                    value = int.MaxValue;
                }
            }

            var t = value;
            while (t >= 10)
            {
                t /= 10;
                len ++;
            }

            EnsureSpace(len);

            var offset = count + len;
            while (value >= 10)
            {
                buffer[--offset] = (char)(value % 10 + '0');
                value /= 10;
            }

            buffer[--offset] = (char)(value + '0');
            if (negative)
            {
                buffer[--offset] = '-';
            }

            count += len;

            return len;
        }

        public int AppendFractional(int value, int decimalPoint)
        {
            var len = AppendInt(value / decimalPoint);

            if (value % decimalPoint != 0)
            {
                if (value < 0)
                {
                    value = -value;
                }

                var fraction = (int)(((long)value * 100 + decimalPoint / 2) / decimalPoint) % 100;

                if (fraction != 0)
                {
                    len += Append('.');

                    if (fraction % 10 == 0)
                    {
                        fraction /= 10;
                    }

                    len += AppendInt(fraction);
                }
            }

            return len;
        }

        public int AppendHex2(uint value)
        {
            EnsureSpace(2);

            var h = (value >> 4) & 0xF;
            if (h < 10)
            {
                buffer[count++] = (char)(h + '0');
            }
            else
            {
                buffer[count++] = (char)(h - 10 + 'A');
            }

            h = value & 0xF;
            if (h < 10)
            {
                buffer[count++] = (char)(h + '0');
            }
            else
            {
                buffer[count++] = (char)(h - 10 + 'A');
            }

            return 2;
        }

        public int Append(char ch)
        {
            return Append(ch, int.MaxValue);
        }

        public int Append(char ch, int maxSize)
        {
            if (0 == GetSpace(maxSize))
            {
                return 0;
            }

            buffer[count++] = ch;
            return 1;
        }

        public int Append(string str)
        {
            return Append(str, int.MaxValue);
        }

        public int Append(string str, int maxSize)
        {
            int countRead;
            var countTotal = 0;

            while (0 != (countRead = Math.Min(GetSpace(maxSize), str.Length - countTotal)))
            {
                str.CopyTo(countTotal, buffer, count, countRead);
                count += countRead;
                countTotal += countRead;
            }

            return countTotal;
        }

        public int Append(char[] buffer, int offset, int length)
        {
            return Append(buffer, offset, length, int.MaxValue);
        }

        public int Append(char[] buffer, int offset, int length, int maxSize)
        {
            int countRead;
            var countTotal = 0;

            while (0 != (countRead = Math.Min(GetSpace(maxSize), length)))
            {
                System.Buffer.BlockCopy(buffer, offset * 2, this.buffer, count * 2, countRead * 2);
                count += countRead;
                offset += countRead;
                length -= countRead;
                countTotal += countRead;
            }

            return countTotal;
        }

        public string ToString(int offset, int count)
        {
            
            return new String(buffer, offset, count);
        }

        public void DisposeBuffer()
        {
            buffer = null;
            count = 0;
        }

        private int GetSpace(int maxSize)
        {
            InternalDebug.Assert((buffer == null && count == 0) || count <= buffer.Length);

            if (count >= maxSize)
            {
                return 0;
            }

            if (buffer == null)
            {
                buffer = new char[64];
            }
            else if (buffer.Length == count)
            {
                var newBuffer = new char[buffer.Length * 2];
                System.Buffer.BlockCopy(buffer, 0, newBuffer, 0, count * 2);
                buffer = newBuffer;
            }

            return buffer.Length - count;
        }

        private void EnsureSpace(int space)
        {
            InternalDebug.Assert(buffer != null || count == 0);
            InternalDebug.Assert(buffer == null || count <= buffer.Length);

            if (buffer == null)
            {
                buffer = new char[Math.Max(space, 64)];
            }
            else if (buffer.Length - count < space)
            {
                var newBuffer = new char[Math.Max(buffer.Length * 2, count + space)];
                System.Buffer.BlockCopy(buffer, 0, newBuffer, 0, count * 2);
                buffer = newBuffer;
            }
        }
    }
}

