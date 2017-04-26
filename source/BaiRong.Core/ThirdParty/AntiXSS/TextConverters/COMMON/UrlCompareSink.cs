// ***************************************************************
// <copyright file="UrlCompareSink.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters
{
    internal class UrlCompareSink : ITextSink
    {
        private string url;
        private int urlPosition;

        public UrlCompareSink()
        {
        }

        public void Initialize(string url)
        {
            this.url = url;
            urlPosition = 0;
        }

        public void Reset()
        {
            urlPosition = -1;
        }

        public bool IsActive => urlPosition >= 0;
        public bool IsMatch => urlPosition == url.Length;

        public bool IsEnough => urlPosition < 0;

        public void Write(char[] buffer, int offset, int count)
        {
            if (IsActive)
            {
                var end = offset + count;

                while (offset < end)
                {
                    if (urlPosition == 0)
                    {
                        if (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(buffer[offset])))
                        {
                            
                            offset++;
                            continue;
                        }
                    }
                    else if (urlPosition == url.Length)
                    {
                        if (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(buffer[offset])))
                        {
                            
                            offset++;
                            continue;
                        }

                        urlPosition = -1;
                        break;
                    }

                    if (buffer[offset] != url[urlPosition])
                    {
                        urlPosition = -1;
                        break;
                    }

                    offset++;
                    urlPosition ++;
                }
            }
        }

        public void Write(int ucs32Char)
        {
            if (Token.LiteralLength(ucs32Char) != 1)
            {
                
                
                urlPosition = -1;
                return;
            }

            if (urlPosition == 0)
            {
                if (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass((char)ucs32Char)))
                {
                    
                    return;
                }
            }
            else if (urlPosition == url.Length)
            {
                if (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass((char)ucs32Char)))
                {
                    
                    return;
                }

                urlPosition = -1;
                return;
            }

            if ((char)ucs32Char != url[urlPosition])
            {
                urlPosition = -1;
                return;
            }

            urlPosition ++;
        }
    }
}

