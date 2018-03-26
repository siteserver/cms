// ***************************************************************
// <copyright file="TextOutput.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters.Internal.Text
{
    using System;
    using System.IO;
    using System.Text;
    using Data.Internal;
    using Html;
    
    using Globalization;

    internal delegate bool ImageRenderingCallbackInternal(string attachmentUrl, int approximateRenderingPosition);

    

    internal class TextOutput : IRestartable, IReusable, IFallback, IDisposable
    {
        

        protected ConverterOutput output;

        
        protected bool lineWrapping;
        protected bool rfc2646;                                
        protected int longestNonWrappedParagraph;
        protected int wrapBeforePosition;
        protected bool preserveTrailingSpace;
        protected bool preserveTabulation;
        protected bool preserveNbsp;

        
        protected int lineLength;
        protected int lineLengthBeforeSoftWrap;
        protected int flushedLength;
        protected int tailSpace;
        protected int breakOpportunity;
        protected int nextBreakOpportunity;
        protected int quotingLevel;                           
        protected bool seenSpace;
        protected bool wrapped;
        protected char[] wrapBuffer;
        protected bool signaturePossible = true;
        protected bool anyNewlines;

        private bool fallbacks;
        private bool htmlEscape;

        protected bool endParagraph;

        private string anchorUrl;
        private int linePosition;
        private ImageRenderingCallbackInternal imageRenderingCallback;

        static readonly char[] Whitespaces = { ' ', '\t', '\r', '\n', '\f' };

        

        public TextOutput(
                ConverterOutput output,
                bool lineWrapping,
                bool flowed,
                int wrapBeforePosition,
                int longestNonWrappedParagraph,
                ImageRenderingCallbackInternal imageRenderingCallback,
                bool fallbacks,
                bool htmlEscape,
                bool preserveSpace,
                Stream testTraceStream)
        {

            rfc2646 = flowed;
            this.lineWrapping = lineWrapping;
            this.wrapBeforePosition = wrapBeforePosition;
            this.longestNonWrappedParagraph = longestNonWrappedParagraph;

            if (!this.lineWrapping)
            {
                preserveTrailingSpace = preserveSpace;
                preserveTabulation = preserveSpace;
                preserveNbsp = preserveSpace;
            }

            this.output = output;

            this.fallbacks = fallbacks;
            this.htmlEscape = htmlEscape;

            this.imageRenderingCallback = imageRenderingCallback;

            
            
            
            
            
            
            
            
            
            
            wrapBuffer = new char[(this.longestNonWrappedParagraph + 1) * 5];
        }

        
        private void Reinitialize()
        {
            anchorUrl = null;
            linePosition = 0;
            lineLength = 0;
            lineLengthBeforeSoftWrap = 0;
            flushedLength = 0;
            tailSpace = 0;
            breakOpportunity = 0;
            nextBreakOpportunity = 0;
            quotingLevel = 0;
            seenSpace = false;
            wrapped = false;
            signaturePossible = true;
            anyNewlines = false;
            endParagraph = false;

        }

        

        public bool OutputCodePageSameAsInput
        {
            get
            {
                if (output is ConverterEncodingOutput)
                {
                    return (output as ConverterEncodingOutput).CodePageSameAsInput;
                }

                return false;
            }
        }

        

        public Encoding OutputEncoding
        {
            set
            {
                if (output is ConverterEncodingOutput)
                {
                    (output as ConverterEncodingOutput).Encoding = value;
                    return;
                }

                InternalDebug.Assert(false, "this should never happen");
                throw new InvalidOperationException();
            }
        }

        

        public bool LineEmpty => lineLength == 0 && tailSpace == 0;


        public bool ImageRenderingCallbackDefined => imageRenderingCallback != null;


        public void OpenDocument()
        {

        }

        

        public void CloseDocument()
        {

            if (!anyNewlines)
            {
                
                output.Write("\r\n");
            }

            endParagraph = false;
        }

        

        public void SetQuotingLevel(int quotingLevel)
        {

            
            
            this.quotingLevel = Math.Min(quotingLevel, wrapBeforePosition / 2);
        }

        

        public void CloseParagraph()
        {

            if (lineLength != 0 || tailSpace != 0)
            {
                OutputNewLine();
            }

            endParagraph = true;
        }

        

        public void OutputNewLine()
        {

            

            

            if (lineWrapping)
            {
                FlushLine('\n');

                if (signaturePossible && lineLength == 2 && tailSpace == 1)
                {
                    
                    output.Write(' ');
                    lineLength++;
                }
            }
            else if (preserveTrailingSpace && tailSpace != 0)
            {
                FlushTailSpace();
            }

            if (!endParagraph)
            {
                output.Write("\r\n");
                anyNewlines = true;
                linePosition += 2;
            }

            linePosition += lineLength;

            lineLength = 0;
            lineLengthBeforeSoftWrap = 0;
            flushedLength = 0;
            tailSpace = 0;
            breakOpportunity = 0;
            nextBreakOpportunity = 0;
            wrapped = false;
            
            seenSpace = false;
            signaturePossible = true;
        }

        

        public void OutputTabulation(int count)
        {

            if (preserveTabulation)
            {
                while (count != 0)
                {
                    OutputNonspace("\t", TextMapping.Unicode);
                    count--;
                }
            }
            else
            {
                var tabPosition = (lineLengthBeforeSoftWrap + lineLength + tailSpace) / 8 * 8 + 8 * count;
                count = tabPosition - (lineLengthBeforeSoftWrap + lineLength + tailSpace);

                OutputSpace(count);
            }
        }

        

        public void OutputSpace(int count)
        {

            

            InternalDebug.Assert(count != 0);

            if (lineWrapping)
            {
                if (breakOpportunity == 0 || lineLength + tailSpace <= WrapBeforePosition())
                {
                    breakOpportunity = lineLength + tailSpace;

                    InternalDebug.Assert(breakOpportunity >= 0);

                    if (lineLength + tailSpace < WrapBeforePosition() && count > 1)
                    {
                        breakOpportunity += Math.Min(WrapBeforePosition() - (lineLength + tailSpace), count - 1);
                    }

                    if (breakOpportunity < lineLength + tailSpace + count - 1)
                    {
                        
                        
                        nextBreakOpportunity = lineLength + tailSpace + count - 1;
                    }

                    if (lineLength > flushedLength)
                    {
                        FlushLine(' ');
                    }
                }
                else
                {
                    
                    
                    
                    

                    nextBreakOpportunity = lineLength + tailSpace + count - 1;
                }
            }

            tailSpace += count;

            InternalDebug.Assert(breakOpportunity == 0 || breakOpportunity < lineLength + tailSpace);
            InternalDebug.Assert(nextBreakOpportunity == 0 || nextBreakOpportunity < lineLength + tailSpace);
        }

        

        public void OutputNbsp(int count)
        {

            

            if (preserveNbsp)
            {
                while (count != 0)
                {
                    OutputNonspace("\xA0", TextMapping.Unicode);
                    count--;
                }
            }
            else
            {
                tailSpace += count;
            }
        }

        

        public void OutputNonspace(char[] buffer, int offset, int count, TextMapping textMapping)
        {

            if (!lineWrapping && !endParagraph && textMapping == TextMapping.Unicode)
            {
                if (tailSpace != 0)
                {
                    FlushTailSpace();
                }

                output.Write(buffer, offset, count, fallbacks ? this : null);

                lineLength += count;
            }
            else
            {
                OutputNonspaceImpl(buffer, offset, count, textMapping);
            }
        }

        

        private void OutputNonspaceImpl(char[] buffer, int offset, int count, TextMapping textMapping)
        {
            if (count != 0)
            {
                if (textMapping != TextMapping.Unicode)
                {
                    
                    
                    
                    
                    for (var i = 0; i < count; i++)
                    {
                        MapAndOutputSymbolCharacter(buffer[offset++], textMapping);
                    }
                    return;
                }

                if (endParagraph)
                {
                    InternalDebug.Assert(lineLength == 0);

                    output.Write("\r\n");
                    linePosition += 2;
                    anyNewlines = true;
                    endParagraph = false;
                }

                if (lineWrapping)
                {
                    
                    
                    
                    
                    
                    

                    WrapPrepareToAppendNonspace(count);

                    if (breakOpportunity == 0)
                    {
                        FlushLine(buffer[offset]);

                        output.Write(buffer, offset, count, fallbacks ? this : null);

                        flushedLength += count;
                    }
                    else
                    {
                        
                        

                        Buffer.BlockCopy(buffer, offset * 2, wrapBuffer, (lineLength - flushedLength) * 2, count * 2);
                    }

                    lineLength += count;

                    if (lineLength > 2 || buffer[offset] != '-' || (count == 2 && buffer[offset + 1] != '-'))
                    {
                        signaturePossible = false;
                    }
                }
                else
                {
                    if (tailSpace != 0)
                    {
                        FlushTailSpace();
                    }

                    output.Write(buffer, offset, count, fallbacks ? this : null);

                    lineLength += count;
                }
            }
        }

        

        public void OutputNonspace(string text, TextMapping textMapping)
        {
            OutputNonspace(text, 0, text.Length, textMapping);
        }

        

        public void OutputNonspace(string text, int offset, int length, TextMapping textMapping)
        {

            if (textMapping != TextMapping.Unicode)
            {
                for (var i = offset; i < length; i++)
                {
                    MapAndOutputSymbolCharacter(text[i], textMapping);
                }
                return;
            }

            if (endParagraph)
            {
                InternalDebug.Assert(lineLength == 0);

                output.Write("\r\n");
                linePosition += 2;
                anyNewlines = true;
                endParagraph = false;
            }

            if (lineWrapping)
            {
                if (length != 0)
                {
                    
                    
                    
                    
                    
                    

                    WrapPrepareToAppendNonspace(length);

                    if (breakOpportunity == 0)
                    {
                        FlushLine(text[offset]);

                        output.Write(text, offset, length, fallbacks ? this : null);

                        flushedLength += length;
                    }
                    else
                    {
                        text.CopyTo(offset, wrapBuffer, lineLength - flushedLength, length);
                    }

                    lineLength += length;

                    if (lineLength > 2 || text[offset] != '-' || (length == 2 && text[offset + 1] != '-'))
                    {
                        signaturePossible = false;
                    }
                }
            }
            else
            {
                if (tailSpace != 0)
                {
                    FlushTailSpace();
                }

                output.Write(text, offset, length, fallbacks ? this : null);

                lineLength += length;
            }
        }

        

        public void OutputNonspace(int ucs32Literal, TextMapping textMapping)
        {

            if (textMapping != TextMapping.Unicode)
            {
                MapAndOutputSymbolCharacter((char)ucs32Literal, textMapping);
                return;
            }

            if (endParagraph)
            {
                InternalDebug.Assert(lineLength == 0);

                output.Write("\r\n");
                linePosition += 2;
                anyNewlines = true;
                endParagraph = false;
            }

            if (lineWrapping)
            {
                var count = Token.LiteralLength(ucs32Literal);

                
                
                
                
                
                

                WrapPrepareToAppendNonspace(count);

                if (breakOpportunity == 0)
                {
                    FlushLine(Token.LiteralFirstChar(ucs32Literal));

                    output.Write(ucs32Literal, fallbacks ? this : null);

                    flushedLength += count;
                }
                else
                {
                    wrapBuffer[lineLength - flushedLength] = Token.LiteralFirstChar(ucs32Literal);
                    if (count != 1)
                    {
                        wrapBuffer[lineLength - flushedLength + 1] = Token.LiteralLastChar(ucs32Literal);
                    }
                }

                lineLength += count;

                if (lineLength > 2 || count != 1 || (char)ucs32Literal != '-')
                {
                    signaturePossible = false;
                }
            }
            else
            {
                if (tailSpace != 0)
                {
                    FlushTailSpace();
                }

                output.Write(ucs32Literal, fallbacks ? this : null);

                lineLength += Token.LiteralLength(ucs32Literal);
            }
        }

        

        public void OpenAnchor(string anchorUrl)
        {
            

            this.anchorUrl = anchorUrl;
        }

        

        public void CloseAnchor()
        {
            
            

            if (anchorUrl != null)
            {
                var addSpace = (tailSpace != 0);

                var urlString = anchorUrl;

                if (urlString.IndexOf(' ') != -1)
                {
                    urlString = urlString.Replace(" ", "%20");
                }

                OutputNonspace("<", TextMapping.Unicode);
                OutputNonspace(urlString, TextMapping.Unicode);
                OutputNonspace(">", TextMapping.Unicode);

                if (addSpace)
                {
                    OutputSpace(1);
                }

                anchorUrl = null;
            }
        }

        

        public void CancelAnchor()
        {

            anchorUrl = null;
        }

        

        public void OutputImage(string imageUrl, string imageAltText, int wdthPixels, int heightPixels)
        {
            
            

            if (imageRenderingCallback != null && imageRenderingCallback(imageUrl, RenderingPosition()))
            {
                
                OutputSpace(1);
            }
            else
            {
                if ((wdthPixels == 0 || wdthPixels >= 8) && (heightPixels == 0 || heightPixels >= 8))
                {
                    var addSpace = (tailSpace != 0);

                    
                    OutputNonspace("[", TextMapping.Unicode);

                    if (!string.IsNullOrEmpty(imageAltText))
                    {
                        

                        var offset = 0;
                        while (offset != imageAltText.Length)
                        {
                            var nextOffset = imageAltText.IndexOfAny(Whitespaces, offset);

                            if (nextOffset == -1)
                            {
                                InternalDebug.Assert(imageAltText.Length - offset > 0);
                                OutputNonspace(imageAltText, offset, imageAltText.Length - offset, TextMapping.Unicode);
                                break;
                            }

                            if (nextOffset != offset)
                            {
                                OutputNonspace(imageAltText, offset, nextOffset - offset, TextMapping.Unicode);
                            }

                            if (imageAltText[offset] == '\t')
                            {
                                OutputTabulation(1);
                            }
                            else
                            {
                                
                                OutputSpace(1);
                            }

                            offset = nextOffset + 1;
                        }
                    }
                    else if (!string.IsNullOrEmpty(imageUrl))
                    {
                        
                        
                        
                        
                        if (imageUrl.Contains("/") &&
                            !imageUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                            !imageUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase) )
                        {
                            imageUrl = "X";
                        }
                        else if (imageUrl.IndexOf(' ') != -1)
                        {
                            imageUrl = imageUrl.Replace(" ", "%20");
                        }

                        OutputNonspace(imageUrl, TextMapping.Unicode);
                    }
                    else
                    {
                        
                        OutputNonspace("X", TextMapping.Unicode);
                    }
                    OutputNonspace("]", TextMapping.Unicode);

                    if (addSpace)
                    {
                        OutputSpace(1);
                    }
                }
            }
        }

        

        public int RenderingPosition()
        {
            return linePosition + lineLength + tailSpace;
        }

        

        public void Flush()
        {
    if (lineWrapping)
            {
                if (lineLength != 0)
                {
                    FlushLine('\r');

                    OutputNewLine();
                }
            }
            else if (lineLength != 0)
            {
                OutputNewLine();
            }

            output.Flush();

        }

        

        private int WrapBeforePosition()
        {
            return wrapBeforePosition - (rfc2646 ? quotingLevel + 1 : 0);
        }

        

        private int LongestNonWrappedParagraph()
        {
            return longestNonWrappedParagraph - (rfc2646 ? quotingLevel + 1 : 0);
        }

        

        private void WrapPrepareToAppendNonspace(int count)
        {
            InternalDebug.Assert(lineWrapping);
            InternalDebug.Assert(nextBreakOpportunity == 0 || nextBreakOpportunity > breakOpportunity);

            while (breakOpportunity != 0 &&
                lineLength + tailSpace + count
                    > (wrapped ? WrapBeforePosition() : LongestNonWrappedParagraph()))
            {
                InternalDebug.Assert(breakOpportunity >= flushedLength);

                if (flushedLength == 0 && rfc2646)
                {
                    for (var i = 0; i < quotingLevel; i++)
                    {
                        output.Write('>');
                    }

                    if (quotingLevel != 0 || wrapBuffer[0] == '>' || wrapBuffer[0] == ' ')
                    {
                        output.Write(' ');
                    }
                }

                if (breakOpportunity >= lineLength)
                {
                    InternalDebug.Assert(tailSpace >= breakOpportunity + 1 - lineLength);
                    InternalDebug.Assert(flushedLength == lineLength);

                    do
                    {
                        
                        
                        
                        if (lineLength - flushedLength == wrapBuffer.Length)
                        {
                            output.Write(wrapBuffer, 0, wrapBuffer.Length, fallbacks ? this : null);
                            flushedLength += wrapBuffer.Length;
                        }

                        wrapBuffer[lineLength - flushedLength] = ' ';

                        lineLength++;
                        tailSpace--;
                    }
                    while (lineLength != breakOpportunity + 1);
                }

                output.Write(wrapBuffer, 0, breakOpportunity + 1 - flushedLength, fallbacks ? this : null);

                anyNewlines = true;
                output.Write("\r\n");

                wrapped = true;
                lineLengthBeforeSoftWrap += breakOpportunity + 1;

                linePosition += breakOpportunity + 1 + 2;
                lineLength -= breakOpportunity + 1;

                InternalDebug.Assert(lineLength >= 0);

                var oldFlushedLength = flushedLength;
                flushedLength = 0;

                if (lineLength != 0)
                {
                    

                    if (nextBreakOpportunity == 0 ||
                        nextBreakOpportunity - (breakOpportunity + 1) >= lineLength ||
                        nextBreakOpportunity - (breakOpportunity + 1) == 0)
                    {
                        

                        if (rfc2646)
                        {
                            for (var i = 0; i < quotingLevel; i++)
                            {
                                output.Write('>');
                            }

                            if (quotingLevel != 0 || wrapBuffer[breakOpportunity + 1 - oldFlushedLength] == '>' || wrapBuffer[breakOpportunity + 1 - oldFlushedLength] == ' ')
                            {
                                output.Write(' ');
                            }
                        }

                        output.Write(wrapBuffer, breakOpportunity + 1 - oldFlushedLength, lineLength, fallbacks ? this : null);
                        flushedLength = lineLength;
                    }
                    else
                    {
                        

                        Buffer.BlockCopy(wrapBuffer, (breakOpportunity + 1 - oldFlushedLength) * 2, wrapBuffer, 0, lineLength * 2);
                    }
                }

                if (nextBreakOpportunity != 0)
                {
                    InternalDebug.Assert(nextBreakOpportunity > breakOpportunity);

                    breakOpportunity = nextBreakOpportunity - (breakOpportunity + 1);

                    InternalDebug.Assert(breakOpportunity >= 0);
                    InternalDebug.Assert(breakOpportunity < lineLength || tailSpace >= breakOpportunity + 1 - lineLength);

                    if (breakOpportunity > WrapBeforePosition())
                    {
                        
                        
                        
                        
                        
                        

                        if (lineLength < WrapBeforePosition())
                        {
                            InternalDebug.Assert(tailSpace != 0);

                            nextBreakOpportunity = breakOpportunity;
                            breakOpportunity = WrapBeforePosition();
                        }
                        else if (breakOpportunity > lineLength)
                        {
                            InternalDebug.Assert(tailSpace != 0);

                            nextBreakOpportunity = breakOpportunity;
                            breakOpportunity = lineLength;
                        }
                        else
                        {
                            nextBreakOpportunity = 0;
                        }

                        InternalDebug.Assert(breakOpportunity == 0 || breakOpportunity < lineLength + tailSpace);
                        InternalDebug.Assert(nextBreakOpportunity == 0 || nextBreakOpportunity < lineLength + tailSpace);
                    }
                    else
                    {
                        nextBreakOpportunity = 0;
                    }
                }
                else
                {
                    breakOpportunity = 0;
                }

                InternalDebug.Assert(breakOpportunity == 0 || breakOpportunity < lineLength + tailSpace);
                InternalDebug.Assert(nextBreakOpportunity == 0 || nextBreakOpportunity < lineLength + tailSpace);
            }

            if (tailSpace != 0)
            {
                if (breakOpportunity == 0)
                {
                    InternalDebug.Assert(lineLength == flushedLength);

                    if (flushedLength == 0 && rfc2646)
                    {
                        for (var i = 0; i < quotingLevel; i++)
                        {
                            output.Write('>');
                        }

                        
                        output.Write(' ');
                    }

                    flushedLength += tailSpace;
                    FlushTailSpace();
                }
                else
                {
                    InternalDebug.Assert(lineLength + tailSpace + count - flushedLength
                                                <= LongestNonWrappedParagraph());

                    do
                    {
                        wrapBuffer[lineLength - flushedLength] = ' ';

                        lineLength++;
                        tailSpace--;
                    }
                    while (tailSpace != 0);
                }
            }

            InternalDebug.Assert(breakOpportunity == 0 || breakOpportunity < lineLength + tailSpace);
            InternalDebug.Assert(nextBreakOpportunity == 0 || nextBreakOpportunity < lineLength + tailSpace);
        }

        private void FlushLine(char nextChar)
        {
            if (flushedLength == 0 && rfc2646)
            {
                for (var i = 0; i < quotingLevel; i++)
                {
                    output.Write('>');
                }

                var firstChar = lineLength != 0 ? wrapBuffer[0] : nextChar;
                if (quotingLevel != 0 || firstChar == '>' || firstChar == ' ')
                {
                    output.Write(' ');
                }
            }

            if (lineLength != flushedLength)
            {
                output.Write(wrapBuffer, 0, lineLength - flushedLength, fallbacks ? this : null);
                flushedLength = lineLength;
            }
        }

        private void FlushTailSpace()
        {
            InternalDebug.Assert(tailSpace != 0);

            lineLength += tailSpace;
            do
            {
                output.Write(' ');
                tailSpace--;
            }
            while (tailSpace != 0);
        }

        

        private void MapAndOutputSymbolCharacter(char ch, TextMapping textMapping)
        {
            if (ch == ' ' || ch == '\t' || ch == '\r' || ch == '\n')
            {
                OutputNonspace((int)ch, TextMapping.Unicode);
                return;
            }

            string substitute = null;

            if (textMapping == TextMapping.Wingdings)
            {
                switch ((int)ch)
                {
                    case 74:	
                        substitute = "\x263A";
                        break;
                    case 75:	
                        substitute = ":|";
                        break;
                    case 76:	
                        substitute = "\x2639";
                        break;
                    case 216:	
                        substitute = ">";
                        break;
                    case 223:	
                        substitute = "<--";
                        break;
                    case 224:	
                        substitute = "-->";
                        break;
                    case 231:	
                        substitute = "<==";
                        break;
                    case 232:	
                        substitute = "==>";
                        break;
                    case 239:	
                        substitute = "<=";
                        break;
                    case 240:	
                        substitute = "=>";
                        break;
                    case 243:	
                        substitute = "<=>";
                        break;
                }
            }

            if (substitute == null)
            {
                substitute = "\x2022";  
            }

            OutputNonspace(substitute, TextMapping.Unicode);
        }

        

        byte[] IFallback.GetUnsafeAsciiMap(out byte unsafeAsciiMask)
        {
            if (htmlEscape)
            {
                
                unsafeAsciiMask = 0x01;
                return HtmlSupport.UnsafeAsciiMap;
            }

            unsafeAsciiMask = 0;
            return null;
        }

        

        bool IFallback.HasUnsafeUnicode()
        {
            return htmlEscape;
        }

        

        bool IFallback.TreatNonAsciiAsUnsafe(string charset)
        {
            return false;
        }

        
        bool IFallback.IsUnsafeUnicode(char ch, bool isFirstChar)
        {
            return htmlEscape &&
                ((byte)(ch & 0xFF) == (byte)'<' ||
                (byte)((ch >> 8) & 0xFF) == (byte)'<');
        }

        

        bool IFallback.FallBackChar(char ch, char[] outputBuffer, ref int outputBufferCount, int outputEnd)
        {
            if (htmlEscape)
            {
                

                
                
                
                
                
                

                HtmlEntityIndex namedEntityId = 0;

                if (ch <= '>')
                {
                    if (ch == '>')
                    {
                        namedEntityId = HtmlEntityIndex.gt;
                    }
                    else if (ch == '<')
                    {
                        namedEntityId = HtmlEntityIndex.lt;
                    }
                    else if (ch == '&')
                    {
                        namedEntityId = HtmlEntityIndex.amp;
                    }
                    else if (ch == '\"')
                    {
                        namedEntityId = HtmlEntityIndex.quot;
                    }
                }
                else if ((char)0xA0 <= ch && ch <= (char)0xFF)
                {
                    namedEntityId = HtmlSupport.EntityMap[(int)ch - 0xA0];
                }

                if ((int)namedEntityId != 0)
                {
                    

                    var strQuote = HtmlNameData.entities[(int)namedEntityId].name;

                    if (outputEnd - outputBufferCount < strQuote.Length + 2)
                    {
                        return false;
                    }

                    outputBuffer[outputBufferCount++] = '&';
                    strQuote.CopyTo(0, outputBuffer, outputBufferCount, strQuote.Length);
                    outputBufferCount += strQuote.Length;
                    outputBuffer[outputBufferCount++] = ';';
                }
                else
                {
                    

                    var value = (uint)ch;
                    var len = (value < 0x10) ? 1 : (value < 0x100) ? 2 : (value < 0x1000) ? 3 : 4;
                    if (outputEnd - outputBufferCount < len + 4)
                    {
                        return false;
                    }

                    outputBuffer[outputBufferCount++] = '&';
                    outputBuffer[outputBufferCount++] = '#';
                    outputBuffer[outputBufferCount++] = 'x';

                    var offset = outputBufferCount + len;
                    while (value != 0)
                    {
                        var digit = value & 0xF;
                        outputBuffer[--offset] = (char)(digit + (digit < 10 ? '0' : 'A' - 10));
                        value >>= 4;
                    }
                    outputBufferCount += len;

                    outputBuffer[outputBufferCount++] = ';';
                }
            }
            else
            {
                var substitute = AsciiEncoderFallback.GetCharacterFallback(ch);

                if (substitute != null)
                {
                    if (outputEnd - outputBufferCount < substitute.Length)
                    {
                        return false;
                    }

                    substitute.CopyTo(0, outputBuffer, outputBufferCount, substitute.Length);
                    outputBufferCount += substitute.Length;
                }
                else
                {
                    InternalDebug.Assert(outputEnd - outputBufferCount > 0);

                    
                    
                    outputBuffer[outputBufferCount++] = ch;
                }
            }

            return true;
        }

        

        void IDisposable.Dispose()
        {
            if (output != null /*&& this.output is IDisposable*/)
            {
                ((IDisposable)output).Dispose();
            }

            output = null;
            wrapBuffer = null;

            GC.SuppressFinalize(this);
        }

        

        bool IRestartable.CanRestart()
        {
            if (output is IRestartable)
            {
                return ((IRestartable)output).CanRestart();
            }

            return false;
        }

        

        void IRestartable.Restart()
        {
            InternalDebug.Assert(((IRestartable)this).CanRestart());

            ((IRestartable)output).Restart();

            Reinitialize();
        }

        

        void IRestartable.DisableRestart()
        {
            if (output is IRestartable)
            {
                ((IRestartable)output).DisableRestart();
            }
        }

        
        void IReusable.Initialize(object newSourceOrDestination)
        {
            InternalDebug.Assert(output is IReusable);

            ((IReusable)output).Initialize(newSourceOrDestination);

            Reinitialize();
        }
    }
}

