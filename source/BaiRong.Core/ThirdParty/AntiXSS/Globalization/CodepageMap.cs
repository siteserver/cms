// ***************************************************************
// <copyright file="CodepageMap.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.Globalization
{
    using Internal;

    
    
    internal class CodePageMap : CodePageMapData
    {
        private int codePage;
        private CodePageRange[] ranges;
        private int lastRangeIndex;
        private CodePageRange lastRange;

        
        
        
        
        public bool ChoseCodePage(int codePage)
        {
            if (codePage == this.codePage)
            {
                return true;
            }

            this.codePage = codePage;
            ranges = null;

            if (codePage == 1200)
            {
                return true;
            }

            for (var i = codePages.Length - 1; i >= 0; i--)
            {
                if (codePages[i].cpid == codePage)
                {
                    

                    ranges = codePages[i].ranges;
                    lastRangeIndex = ranges.Length / 2;
                    lastRange = ranges[lastRangeIndex];

                    return true;
                }
            }

            return false;
        }

        
        
        
        
        public bool IsUnsafeExtendedCharacter(char ch)
        {
            

            if (ranges == null)
            {
                

                
                
                
                

                InternalDebug.Assert(false);

                
                
                

                
                
                

                return false;
            }

            if (ch <= lastRange.last)
            {
                if (ch >= lastRange.first)
                {
                    
                    return lastRange.offset != 0xFFFFu && (bitmap[lastRange.offset + (ch - lastRange.first)] & lastRange.mask) == 0;
                }
                else
                {
                    

                    var i = lastRangeIndex;

                    while (--i >= 0)
                    {
                        if (ch >= ranges[i].first)
                        {
                            if (ch <= ranges[i].last)
                            {
                                if (ch == ranges[i].first)
                                {
                                    
                                    return false;
                                }

                                
                                
                                lastRangeIndex = i;
                                lastRange = ranges[i];

                                return lastRange.offset != 0xFFFFu && (bitmap[lastRange.offset + (ch - lastRange.first)] & lastRange.mask) == 0;
                            }

                            break;
                        }
                    }
                }
            }
            else
            {
                

                var i = lastRangeIndex;

                while (++ i < ranges.Length)
                {
                    if (ch <= ranges[i].last)
                    {
                        if (ch >= ranges[i].first)
                        {
                            if (ch == ranges[i].first)
                            {
                                
                                return false;
                            }

                            
                            
                            lastRangeIndex = i;
                            lastRange = ranges[i];

                            return lastRange.offset != 0xFFFFu && (bitmap[lastRange.offset + (ch - lastRange.first)] & lastRange.mask) == 0;
                        }

                        break;
                    }
                }
            }

            
            return true;
        }
    }
}

