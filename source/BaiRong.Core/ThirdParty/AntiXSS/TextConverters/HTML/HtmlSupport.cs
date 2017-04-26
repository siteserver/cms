// ***************************************************************
// <copyright file="HtmlSupport.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters.Internal.Html
{
    using System;
    using System.Collections.Generic;
    using Data.Internal;
    using Format;

    

    internal static class HtmlSupport
    {
        public const int HtmlNestingLimit = 4096;
        public const int MaxAttributeSize = 4096;
        public const int MaxCssPropertySize = 4096;
        public const int MaxNumberOfNonInlineStyles = 128;

        

        public static readonly byte[] UnsafeAsciiMap =
        {
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0,                                                                      
            0,                                                                      
            0x02,                                                                   
            0x02,                                                                   
            0,                                                                      
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0,                                                                      
            0x02,                                                                   
              0x03,                                                                 
            0x02,                                                                   
            0x02,                                                                   
            0x02,                                                                   
              0x03,                                                                 
            0x02,                                                                   
            0x02,                                                                   
            0x02,                                                                   
            0x02,                                                                   
              0x03,                                                                 
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0x02,                                                                   
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0x02,                                                                   
            0x02,                                                                   
              0x03,                                                                 
            0x02,                                                                   
              0x03,                                                                 
            0x02,                                                                   
            0x02,                                                                   
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0x02,                                                                   
            0x02,                                                                   
            0x02,                                                                   
            0x02,                                                                   
            0,                                                                      
            0x02,                                                                   
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0x02,                                                                   
            0x02,                                                                   
            0x02,                                                                   
            0x02,                                                                   
            0x03,                                                                   

            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   
            0x03,                                                                   

            0x03,                                                                   
        };

        

        public static readonly HtmlEntityIndex[] EntityMap =
        {
#if false
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            HtmlEntityIndex.quot,                                                  
            0,                                                                      
            0,                                                                      
            0,                                                                      
            HtmlEntityIndex.amp,                                                   
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            HtmlEntityIndex.lt,                                                    
            0,                                                                      
            HtmlEntityIndex.gt,                                                    
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
            0,                                                                      
#endif
            HtmlEntityIndex.nbsp,                                                  
            HtmlEntityIndex.iexcl,                                                 
            HtmlEntityIndex.cent,                                                  
            HtmlEntityIndex.pound,                                                 
            HtmlEntityIndex.curren,                                                
            HtmlEntityIndex.yen,                                                   
            HtmlEntityIndex.brvbar,                                                
            HtmlEntityIndex.sect,                                                  
            HtmlEntityIndex.uml,                                                   
            HtmlEntityIndex.copy,                                                  
            HtmlEntityIndex.ordf,                                                  
            HtmlEntityIndex.laquo,                                                 
            HtmlEntityIndex.not,                                                   
            HtmlEntityIndex.shy,                                                   
            HtmlEntityIndex.reg,                                                   
            HtmlEntityIndex.macr,                                                  
            HtmlEntityIndex.deg,                                                   
            HtmlEntityIndex.plusmn,                                                
            HtmlEntityIndex.sup2,                                                  
            HtmlEntityIndex.sup3,                                                  
            HtmlEntityIndex.acute,                                                 
            HtmlEntityIndex.micro,                                                 
            HtmlEntityIndex.para,                                                  
            HtmlEntityIndex.middot,                                                
            HtmlEntityIndex.cedil,                                                 
            HtmlEntityIndex.sup1,                                                  
            HtmlEntityIndex.ordm,                                                  
            HtmlEntityIndex.raquo,                                                 
            HtmlEntityIndex.frac14,                                                
            HtmlEntityIndex.frac12,                                                
            HtmlEntityIndex.frac34,                                                
            HtmlEntityIndex.iquest,                                                
            HtmlEntityIndex.Agrave,                                                
            HtmlEntityIndex.Aacute,                                                
            HtmlEntityIndex.Acirc,                                                 
            HtmlEntityIndex.Atilde,                                                
            HtmlEntityIndex.Auml,                                                  
            HtmlEntityIndex.Aring,                                                 
            HtmlEntityIndex.AElig,                                                 
            HtmlEntityIndex.Ccedil,                                                
            HtmlEntityIndex.Egrave,                                                
            HtmlEntityIndex.Eacute,                                                
            HtmlEntityIndex.Ecirc,                                                 
            HtmlEntityIndex.Euml,                                                  
            HtmlEntityIndex.Igrave,                                                
            HtmlEntityIndex.Iacute,                                                
            HtmlEntityIndex.Icirc,                                                 
            HtmlEntityIndex.Iuml,                                                  
            HtmlEntityIndex.ETH,                                                   
            HtmlEntityIndex.Ntilde,                                                
            HtmlEntityIndex.Ograve,                                                
            HtmlEntityIndex.Oacute,                                                
            HtmlEntityIndex.Ocirc,                                                 
            HtmlEntityIndex.Otilde,                                                
            HtmlEntityIndex.Ouml,                                                  
            HtmlEntityIndex.times,                                                 
            HtmlEntityIndex.Oslash,                                                
            HtmlEntityIndex.Ugrave,                                                
            HtmlEntityIndex.Uacute,                                                
            HtmlEntityIndex.Ucirc,                                                 
            HtmlEntityIndex.Uuml,                                                  
            HtmlEntityIndex.Yacute,                                                
            HtmlEntityIndex.THORN,                                                 
            HtmlEntityIndex.szlig,                                                 
            HtmlEntityIndex.agrave,                                                
            HtmlEntityIndex.aacute,                                                
            HtmlEntityIndex.acirc,                                                 
            HtmlEntityIndex.atilde,                                                
            HtmlEntityIndex.auml,                                                  
            HtmlEntityIndex.aring,                                                 
            HtmlEntityIndex.aelig,                                                 
            HtmlEntityIndex.ccedil,                                                
            HtmlEntityIndex.egrave,                                                
            HtmlEntityIndex.eacute,                                                
            HtmlEntityIndex.ecirc,                                                 
            HtmlEntityIndex.euml,                                                  
            HtmlEntityIndex.igrave,                                                
            HtmlEntityIndex.iacute,                                                
            HtmlEntityIndex.icirc,                                                 
            HtmlEntityIndex.iuml,                                                  
            HtmlEntityIndex.eth,                                                   
            HtmlEntityIndex.ntilde,                                                
            HtmlEntityIndex.ograve,                                                
            HtmlEntityIndex.oacute,                                                
            HtmlEntityIndex.ocirc,                                                 
            HtmlEntityIndex.otilde,                                                
            HtmlEntityIndex.ouml,                                                  
            HtmlEntityIndex.divide,                                                
            HtmlEntityIndex.oslash,                                                
            HtmlEntityIndex.ugrave,                                                
            HtmlEntityIndex.uacute,                                                
            HtmlEntityIndex.ucirc,                                                 
            HtmlEntityIndex.uuml,                                                  
            HtmlEntityIndex.yacute,                                                
            HtmlEntityIndex.thorn,                                                 
            HtmlEntityIndex.yuml,                                                  
        };

        
        
        
        
        
        
        
        

        [Flags]
        public enum NumberParseFlags
        {
            Integer = 0x0001,
            Float = 0x0002,
            AbsoluteLength = 0x0004,        
            EmExLength = 0x0008,
            Percentage = 0x0010,
            Multiple = 0x0020,
            HtmlFontUnits = 0x0040,

            NonNegative = 0x2000,
            StyleSheetProperty = 0x4000,
            Strict = 0x8000,

            Length = AbsoluteLength | EmExLength | Percentage,
            NonNegativeLength = AbsoluteLength | EmExLength | Percentage | NonNegative,
            FontSize = AbsoluteLength | EmExLength | Percentage | HtmlFontUnits | NonNegative,
        }

        public static PropertyValue ParseNumber(BufferString value, NumberParseFlags parseFlags)
        {
            var isValidNumber = false;
            var isSigned = false;
            var isNegative = false;
            ulong result = 0;
            var exponent = 0;
            var scientificExponent = 0;
            var floatNumber = false;
            var offset = 0;
            var end = value.Length;

            
            while (offset < end && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(value[offset])))
            {
                offset++;
            }

            if (offset == end)
            {
                return PropertyValue.Null;
            }

            if (offset < end && (value[offset] == '-' || value[offset] == '+'))
            {
                isSigned = true;
                isNegative = (value[offset] == '-');
                offset ++;
            }

            while (offset < end && ParseSupport.NumericCharacter(ParseSupport.GetCharClass(value[offset])))
            {
                isValidNumber = true;
                if (result < ulong.MaxValue / 10 - 9)
                {
                    result = unchecked(result * 10u + (uint)(value[offset] - '0'));
                }
                else
                {
                    
                    exponent++;
                }
                offset++;
            }

            if (offset < end && value[offset] == '.')
            {
                floatNumber = true;
                offset++;

                while (offset < end && ParseSupport.NumericCharacter(ParseSupport.GetCharClass(value[offset])))
                {
                    isValidNumber = true;

                    
                    if (result < ulong.MaxValue / 10 - 9)
                    {
                        result = unchecked(result * 10u + (uint)(value[offset] - '0'));
                        exponent--;
                    }

                    offset++;
                }

                if (exponent >= 0 && 0 != (parseFlags & NumberParseFlags.Strict))
                {
                    return PropertyValue.Null;
                }
            }

            if (!isValidNumber)
            {
                return PropertyValue.Null;
            }

            while (offset < end && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(value[offset])))
            {
                offset++;
            }

            if (offset < end && (value[offset] | '\x20') == 'e')
            {
                if (offset + 1 < end && (value[offset + 1] == '-' || value[offset + 1] == '+' || ParseSupport.NumericCharacter(ParseSupport.GetCharClass(value[offset + 1]))))
                {
                    

                    floatNumber = true;
                    offset ++;  

                    var isNegativeScientificExponent = false;

                    if (value[offset] == '-' || value[offset] == '+')
                    {
                        isNegativeScientificExponent = (value[offset] == '-');
                        offset ++;
                    }

                    while (offset < end && ParseSupport.NumericCharacter(ParseSupport.GetCharClass(value[offset])))
                    {
                        scientificExponent = unchecked(scientificExponent * 10 + (value[offset++] - '0'));
                    }

                    if (isNegativeScientificExponent)
                    {
                        scientificExponent = -scientificExponent;
                    }

                    while (offset < end && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(value[offset])))
                    {
                        offset++;
                    }
                }
            }

            var mul = floatNumber ? 10000u : 1;
            uint div = 1;
            var units = floatNumber ? PropertyType.Fractional : PropertyType.Integer;
            var recognized = false;
            var typeLength = 0;

            if (offset + 1 < end)
            {
                

                if ((value[offset] | '\x20') == 'p')
                {
                    

                    if ((value[offset + 1] | '\x20') == 'c')
                    {
                        

                        mul = 8 * 20 * 12;
                        div = 1;
                        recognized = true;
                        units = PropertyType.AbsLength;
                        typeLength = 2;
                    }
                    else if ((value[offset + 1] | '\x20') == 't')
                    {
                        

                        mul = 8 * 20;
                        div = 1;
                        recognized = true;
                        units = PropertyType.AbsLength;
                        typeLength = 2;
                    }
                    else if ((value[offset + 1] | '\x20') == 'x')
                    {
                        

                        mul = 8 * 20 * 72;
                        div = 120;
                        units = PropertyType.Pixels;
                        recognized = true;
                        typeLength = 2;
                    }
                }
                else if ((value[offset] | '\x20') == 'e')
                {
                    

                    if ((value[offset + 1] | '\x20') == 'm')
                    {
                        

                        mul = 8 * 20;
                        div = 1;
                        units = PropertyType.Ems;
                        recognized = true;
                        typeLength = 2;
                    }
                    else if ((value[offset + 1] | '\x20') == 'x')
                    {
                        

                        mul = 8 * 20;
                        div = 1;
                        units = PropertyType.Exs;
                        recognized = true;
                        typeLength = 2;
                    }
                }
                else if ((value[offset] | '\x20') == 'i')
                {
                    

                    if ((value[offset + 1] | '\x20') == 'n')
                    {
                        

                        mul = 8 * 20 * 72;
                        div = 1;
                        recognized = true;
                        units = PropertyType.AbsLength;
                        typeLength = 2;
                    }
                }
                else if ((value[offset] | '\x20') == 'c')
                {
                    

                    if ((value[offset + 1] | '\x20') == 'm')
                    {
                        

                        mul = 8 * 20 * 72 * 100;
                        div = 254;
                        recognized = true;
                        units = PropertyType.AbsLength;
                        typeLength = 2;
                    }
                }
                else if ((value[offset] | '\x20') == 'm')
                {
                    

                    if ((value[offset + 1] | '\x20') == 'm')
                    {
                        

                        mul = 8 * 20 * 72 * 10;
                        div = 254;
                        recognized = true;
                        units = PropertyType.AbsLength;
                        typeLength = 2;
                    }
                }
            }

            if (!recognized && offset < end)
            {
                if (value[offset] == '%')
                {
                    

                    mul = 10000;
                    div = 1;
                    units = PropertyType.Percentage;
                    recognized = true;
                    typeLength = 1;
                }
                else if (value[offset] == '*')
                {
                    

                    mul = 1;
                    div = 1;
                    units = PropertyType.Multiple;
                    recognized = true;
                    typeLength = 1;
                }
            }

            offset += typeLength;

            if (offset < end)
            {
                while (offset < end && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(value[offset])))
                {
                    offset++;
                }

                if (offset < end)
                {
                    if (0 != (parseFlags & (NumberParseFlags.Strict | NumberParseFlags.StyleSheetProperty)))
                    {
                        return PropertyValue.Null;
                    }
                }
            }

            if (result != 0)
            {
                
                
                
                var actualExponent = exponent + scientificExponent;

                

                if (actualExponent > 0)
                {
                    if (actualExponent > 20)
                    {
                        actualExponent = 0;
                        result = ulong.MaxValue;
                    }
                    else
                    {
                        while (actualExponent != 0)
                        {
                            if (result > ulong.MaxValue / 10)
                            {
                                actualExponent = 0;
                                result = ulong.MaxValue;
                                break;
                            }
                            else
                            {
                                result = unchecked(result * 10);
                            }
                            actualExponent --;
                        }
                    }
                }
                else if (actualExponent < -10)
                {
                    if (actualExponent < -21)
                    {
                        actualExponent = 0;
                        result = 0;
                    }
                    else
                    {
                        while (actualExponent != -10)
                        {
                            result /= 10;
                            actualExponent ++;
                        }
                    }
                }

                result *= mul;
                result /= div;

                while (actualExponent != 0)
                {
                    result /= 10;
                    actualExponent ++;
                }

                if (result > PropertyValue.ValueMax)
                {
                    result = PropertyValue.ValueMax;
                }
            }

            var intValue = unchecked((int)result);
            if (isNegative)
            {
                intValue = -intValue;
            }

            if (units == PropertyType.Integer)
            {
                if (0 != (parseFlags & NumberParseFlags.Integer))
                {
                    
                }
                else if (0 != (parseFlags & NumberParseFlags.HtmlFontUnits))
                {
                    if (isSigned)
                    {
                        if (intValue < -7)
                        {
                            intValue = -7;
                        }
                        else if (intValue > 7)
                        {
                            intValue = 7;
                        }
                        units = PropertyType.RelHtmlFontUnits;
                    }
                    else
                    {
                        if (intValue < 1)
                        {
                            intValue = 1;
                        }
                        else if (intValue > 7)
                        {
                            intValue = 7;
                        }
                        units = PropertyType.HtmlFontUnits;
                    }
                }
                
                else if (0 != (parseFlags & NumberParseFlags.AbsoluteLength))
                {
                    
                    
                    result = result * (8 * 20 * 72) / 120;
                    if (result > PropertyValue.ValueMax)
                    {
                        result = PropertyValue.ValueMax;
                    }

                    intValue = unchecked((int)result);
                    if (isNegative)
                    {
                        intValue = -intValue;
                    }

                    units = PropertyType.Pixels;
                }
                else if (0 != (parseFlags & NumberParseFlags.Float))
                {
                    result = result * 10000;
                    if (result > PropertyValue.ValueMax)
                    {
                        result = PropertyValue.ValueMax;
                    }

                    intValue = unchecked((int)result);
                    if (isNegative)
                    {
                        intValue = -intValue;
                    }

                    units = PropertyType.Fractional;
                }
                else
                {
                    return PropertyValue.Null;
                }
            }
            else if (units == PropertyType.Fractional)
            {
                if (0 != (parseFlags & NumberParseFlags.Float))
                {
                    
                }
                
                else if (0 != (parseFlags & NumberParseFlags.AbsoluteLength))
                {
                    
                    
                    result = result * (8 * 20 * 72) / 120 / 10000;
                    if (result > PropertyValue.ValueMax)
                    {
                        result = PropertyValue.ValueMax;
                    }

                    intValue = unchecked((int)result);
                    if (isNegative)
                    {
                        intValue = -intValue;
                    }

                    units = PropertyType.Pixels;
                }
                else
                {
                    return PropertyValue.Null;
                }
            }
            else if (units == PropertyType.AbsLength || units == PropertyType.Pixels)
            {
                if (0 == (parseFlags & NumberParseFlags.AbsoluteLength))
                {
                    return PropertyValue.Null;
                }
            }
            else if (units == PropertyType.Ems || units == PropertyType.Exs)
            {
                if (0 == (parseFlags & NumberParseFlags.EmExLength))
                {
                    return PropertyValue.Null;
                }
            }
            else if (units == PropertyType.Percentage)
            {
                if (0 == (parseFlags & NumberParseFlags.Percentage))
                {
                    return PropertyValue.Null;
                }
            }
            else if (units == PropertyType.Multiple)
            {
                if (0 == (parseFlags & NumberParseFlags.Multiple))
                {
                    return PropertyValue.Null;
                }
            }

            if (intValue < 0 && 0 != (parseFlags & NumberParseFlags.NonNegative) && units != PropertyType.RelHtmlFontUnits)
            {
                
                return PropertyValue.Null;
            }

            return new PropertyValue(units, intValue);
        }

        public static BufferString FormatPixelOrPercentageLength(ref ScratchBuffer scratchBuffer, PropertyValue value)
        {
            scratchBuffer.Reset();
            AppendNumber(ref scratchBuffer, value, NumberParseFlags.Integer | NumberParseFlags.Percentage);
            return scratchBuffer.BufferString;
        }

        public static BufferString FormatPixelLength(ref ScratchBuffer scratchBuffer, PropertyValue value)
        {
            scratchBuffer.Reset();
            AppendNumber(ref scratchBuffer, value, NumberParseFlags.Integer);
            return scratchBuffer.BufferString;
        }

        public static BufferString FormatLength(ref ScratchBuffer scratchBuffer, PropertyValue value)
        {
            scratchBuffer.Reset();
            AppendNumber(ref scratchBuffer, value, NumberParseFlags.AbsoluteLength | NumberParseFlags.Percentage | NumberParseFlags.EmExLength);
            return scratchBuffer.BufferString;
        }

        public static BufferString FormatFontSize(ref ScratchBuffer scratchBuffer, PropertyValue value)
        {
            scratchBuffer.Reset();
            AppendNumber(ref scratchBuffer, value, NumberParseFlags.HtmlFontUnits);
            return scratchBuffer.BufferString;
        }

        public static void AppendPixelOrPercentageLength(ref ScratchBuffer scratchBuffer, PropertyValue value)
        {
            AppendNumber(ref scratchBuffer, value, NumberParseFlags.Integer | NumberParseFlags.Percentage);
        }

        public static void AppendPixelLength(ref ScratchBuffer scratchBuffer, PropertyValue value)
        {
            AppendNumber(ref scratchBuffer, value, NumberParseFlags.Integer);
        }

        public static void AppendLength(ref ScratchBuffer scratchBuffer, PropertyValue value)
        {
            AppendNumber(ref scratchBuffer, value, NumberParseFlags.AbsoluteLength | NumberParseFlags.Percentage | NumberParseFlags.EmExLength);
        }

        public static void AppendFontSize(ref ScratchBuffer scratchBuffer, PropertyValue value)
        {
            AppendNumber(ref scratchBuffer, value, NumberParseFlags.HtmlFontUnits);
        }

        public static void AppendCssFontSize(ref ScratchBuffer scratchBuffer, PropertyValue value)
        {
            AppendNumber(ref scratchBuffer, value, NumberParseFlags.FontSize);
        }

        private static void AppendNumber(ref ScratchBuffer scratchBuffer, PropertyValue value, NumberParseFlags formatFlags)
        {
            

            if (value.IsPercentage)
            {
                if (0 != (formatFlags & NumberParseFlags.Percentage))
                {
                    scratchBuffer.AppendFractional(value.Percentage10K, 10000);
                    scratchBuffer.Append('%');
                }
            }
            else if (value.IsAbsRelLength)
            {
                if (0 != (formatFlags & NumberParseFlags.Integer))
                {
                    scratchBuffer.AppendInt(value.PixelsInteger);
                }
                else if (0 != (formatFlags & NumberParseFlags.AbsoluteLength))
                {
                    if (value.IsPixels)
                    {
                        var pixels96 = value.PixelsInteger96;

                        scratchBuffer.AppendFractional(pixels96, 96);

                        if (pixels96 != 0)
                        {
                            scratchBuffer.Append("px");
                        }
                    }
                    else
                    {
                        var points160 = value.PointsInteger160;

                        scratchBuffer.AppendFractional(points160, 160);

                        if (points160 != 0)
                        {
                            scratchBuffer.Append("pt");
                        }
                    }
                }
                else if (0 != (formatFlags & NumberParseFlags.HtmlFontUnits))
                {
                    scratchBuffer.AppendInt(PropertyValue.ConvertTwipsToHtmlFontUnits(value.TwipsInteger));
                }
            }
            else if (value.IsEms)
            {
                if (0 != (formatFlags & NumberParseFlags.EmExLength))
                {
                    scratchBuffer.AppendFractional(value.EmsInteger160, 160);
                    scratchBuffer.Append("em");
                }
            }
            else if (value.IsExs)
            {
                if (0 != (formatFlags & NumberParseFlags.EmExLength))
                {
                    scratchBuffer.AppendFractional(value.ExsInteger160, 160);
                    scratchBuffer.Append("ex");
                }
            }
            else if (value.IsHtmlFontUnits)
            {
                if (0 != (formatFlags & NumberParseFlags.HtmlFontUnits))
                {
                    scratchBuffer.AppendInt(value.HtmlFontUnits);
                }
            }
            else if (value.IsRelativeHtmlFontUnits)
            {
                if (0 != (formatFlags & NumberParseFlags.HtmlFontUnits))
                {
                    if (value.RelativeHtmlFontUnits > 0)
                    {
                        scratchBuffer.Append("+");
                        scratchBuffer.AppendInt(value.RelativeHtmlFontUnits);
                    }
                    else if (value.RelativeHtmlFontUnits < 0)
                    {
                        scratchBuffer.AppendInt(value.RelativeHtmlFontUnits);
                    }
                }
            }
        }

        

        public struct EnumerationDef
        {
            public string name;
            public PropertyValue value;

            public EnumerationDef(string name, PropertyValue value)
            {
                this.name = name;
                this.value = value;
            }
        }

        

        public static PropertyValue ParseEnum(BufferString value, EnumerationDef[] enumerationDefs)
        {
            value.TrimWhitespace();
            if (value.Length == 0)
            {
                return PropertyValue.Null;
            }

            for (var i = 0; i < enumerationDefs.Length; i++)
            {
                if (value.EqualsToLowerCaseStringIgnoreCase(enumerationDefs[i].name))
                {
                    return enumerationDefs[i].value;
                }
            }

            
            return PropertyValue.Null;
        }

        public static string GetEnumString(PropertyValue value, EnumerationDef[] enumerationDefs)
        {
            for (var i = 0; i < enumerationDefs.Length; i++)
            {
                if (value.RawValue == enumerationDefs[i].value.RawValue)
                {
                    return enumerationDefs[i].name;
                }
            }

            return null;
        }

        

        public static PropertyValue ParseBooleanAttribute(BufferString value, FormatConverter formatConverter)
        {
            
            return PropertyValue.True;
        }

        

        private static EnumerationDef[] DirectionEnumeration =
        {
            new EnumerationDef("rtl", PropertyValue.True),
            new EnumerationDef("ltr", PropertyValue.False),
        };

        internal static PropertyValue ParseDirection(BufferString value, FormatConverter formatConverter)
        {
            return ParseEnum(value, DirectionEnumeration);
        }

        

        internal static EnumerationDef[] TextAlignmentEnumeration =
        {
            new EnumerationDef("left", new PropertyValue(TextAlign.Left)),
            new EnumerationDef("center", new PropertyValue(TextAlign.Center)),
            new EnumerationDef("right", new PropertyValue(TextAlign.Right)),
            new EnumerationDef("justify", new PropertyValue(TextAlign.Justify)),
        };

        internal static PropertyValue ParseTextAlignment(BufferString value, FormatConverter formatConverter)
        {
            return ParseEnum(value, TextAlignmentEnumeration);
        }

        internal static string GetTextAlignmentString(PropertyValue value)
        {
            return GetEnumString(value, TextAlignmentEnumeration);
        }

        

        internal static EnumerationDef[] HorizontalAlignmentEnumeration =
        {
            new EnumerationDef("left", new PropertyValue(BlockHorizontalAlign.Left)),
            new EnumerationDef("center", new PropertyValue(BlockHorizontalAlign.Center)),
            new EnumerationDef("right", new PropertyValue(BlockHorizontalAlign.Right)),
        };

        internal static PropertyValue ParseHorizontalAlignment(BufferString value, FormatConverter formatConverter)
        {
            return ParseEnum(value, HorizontalAlignmentEnumeration);
        }

        internal static string GetHorizontalAlignmentString(PropertyValue value)
        {
            return GetEnumString(value, HorizontalAlignmentEnumeration);
        }

        

        private static EnumerationDef[] VerticalAlignmentEnumeration =
        {
            new EnumerationDef("top", new PropertyValue(BlockVerticalAlign.Top)),
            new EnumerationDef("middle", new PropertyValue(BlockVerticalAlign.Middle)),
            new EnumerationDef("bottom", new PropertyValue(BlockVerticalAlign.Bottom)),
            new EnumerationDef("baseline", new PropertyValue(BlockVerticalAlign.Middle)),
        };

        internal static PropertyValue ParseVerticalAlignment(BufferString value, FormatConverter formatConverter)
        {
            return ParseEnum(value, VerticalAlignmentEnumeration);
        }

        internal static string GetVerticalAlignmentString(PropertyValue value)
        {
            return GetEnumString(value, VerticalAlignmentEnumeration);
        }

        

        internal static EnumerationDef[] BlockAlignmentEnumeration =
        {
            new EnumerationDef("top", new PropertyValue(BlockHorizontalAlign.Left)),
            new EnumerationDef("middle", new PropertyValue(BlockHorizontalAlign.Left)),
            new EnumerationDef("bottom", new PropertyValue(BlockHorizontalAlign.Left)),
            new EnumerationDef("left", new PropertyValue(BlockHorizontalAlign.Center)),
            new EnumerationDef("right", new PropertyValue(BlockHorizontalAlign.Right)),
        };

        internal static PropertyValue ParseBlockAlignment(BufferString value, FormatConverter formatConverter)
        {
            return ParseEnum(value, BlockAlignmentEnumeration);
        }

        internal static string GetBlockAlignmentString(PropertyValue value)
        {
            return GetEnumString(value, BlockAlignmentEnumeration);
        }

        

        internal static EnumerationDef[] BorderStyleEnumeration =
        {
            new EnumerationDef("none", new PropertyValue(BorderStyle.None)),
            new EnumerationDef("hidden", new PropertyValue(BorderStyle.Hidden)),
            new EnumerationDef("dotted", new PropertyValue(BorderStyle.Dotted)),
            new EnumerationDef("dashed", new PropertyValue(BorderStyle.Dashed)),
            new EnumerationDef("solid", new PropertyValue(BorderStyle.Solid)),
            new EnumerationDef("double", new PropertyValue(BorderStyle.Double)),
            new EnumerationDef("groove", new PropertyValue(BorderStyle.Groove)),
            new EnumerationDef("ridge", new PropertyValue(BorderStyle.Ridge)),
            new EnumerationDef("inset", new PropertyValue(BorderStyle.Inset)),
            new EnumerationDef("outset", new PropertyValue(BorderStyle.Outset)),
        };

        internal static PropertyValue ParseBorderStyle(BufferString value, FormatConverter formatConverter)
        {
            return ParseEnum(value, BorderStyleEnumeration);
        }

        internal static string GetBorderStyleString(PropertyValue value)
        {
            return GetEnumString(value, BorderStyleEnumeration);
        }

        

        private static EnumerationDef[] TargetEnumeration =
        {
            new EnumerationDef("_self", new PropertyValue(LinkTarget.Self)),
            new EnumerationDef("_top", new PropertyValue(LinkTarget.Top)),
            new EnumerationDef("_blank", new PropertyValue(LinkTarget.Blank)),
            new EnumerationDef("_parent", new PropertyValue(LinkTarget.Parent)),
        };

        internal static PropertyValue ParseTarget(BufferString value, FormatConverter formatConverter)
        {
            return ParseEnum(value, TargetEnumeration);
        }

        internal static string GetTargetString(PropertyValue value)
        {
            return GetEnumString(value, TargetEnumeration);
        }

        
        

        private static EnumerationDef[] FontWeightEnumeration =
        {
            new EnumerationDef("normal", PropertyValue.False),
            new EnumerationDef("bold", PropertyValue.True),
            new EnumerationDef("lighter", PropertyValue.False),
            new EnumerationDef("bolder", PropertyValue.True),
            new EnumerationDef("100", PropertyValue.False),
            new EnumerationDef("200", PropertyValue.False),
            new EnumerationDef("300", PropertyValue.False),
            new EnumerationDef("400", PropertyValue.False), 
            new EnumerationDef("500", PropertyValue.False),
            new EnumerationDef("600", PropertyValue.True),
            new EnumerationDef("700", PropertyValue.True), 
            new EnumerationDef("800", PropertyValue.True),
            new EnumerationDef("900", PropertyValue.True),
        };

        internal static PropertyValue ParseFontWeight(BufferString value, FormatConverter formatConverter)
        {
            return ParseEnum(value, FontWeightEnumeration);
        }

        

        private static EnumerationDef[] FontSizeEnumeration =
        {
            new EnumerationDef("xx-small", new PropertyValue(LengthUnits.HtmlFontUnits, 1)),
            new EnumerationDef("x-small", new PropertyValue(LengthUnits.HtmlFontUnits, 2)),
            new EnumerationDef("small", new PropertyValue(LengthUnits.HtmlFontUnits, 2)),
            new EnumerationDef("medium", new PropertyValue(LengthUnits.HtmlFontUnits, 3)),
            new EnumerationDef("large", new PropertyValue(LengthUnits.HtmlFontUnits, 4)),
            new EnumerationDef("x-large", new PropertyValue(LengthUnits.HtmlFontUnits, 5)),
            new EnumerationDef("xx-large", new PropertyValue(LengthUnits.HtmlFontUnits, 6)),
        };

        internal static PropertyValue ParseCssFontSize(BufferString value, FormatConverter formatConverter)
        {
            var result = ParseEnum(value, FontSizeEnumeration);
            if (result.IsNull)
            {
                result = ParseFontSize(value, formatConverter);
            }

            return result;
        }

        

        private static EnumerationDef[] FontStyleEnumeration =
        {
            
            new EnumerationDef("normal", PropertyValue.False),
            new EnumerationDef("italic", PropertyValue.True),
            new EnumerationDef("oblique", PropertyValue.True),    
        };

        internal static PropertyValue ParseFontStyle(BufferString value, FormatConverter formatConverter)
        {
            return ParseEnum(value, FontStyleEnumeration);
        }

        

        private static EnumerationDef[] FontVariantEnumeration =
        {
            
            new EnumerationDef("normal", PropertyValue.False),
            new EnumerationDef("small-caps", PropertyValue.True),
        };

        internal static PropertyValue ParseFontVariant(BufferString value, FormatConverter formatConverter)
        {
            return ParseEnum(value, FontVariantEnumeration);
        }


        

        private static EnumerationDef[] TableLayoutEnumeration =
        {
            
            new EnumerationDef("auto", PropertyValue.False),
            new EnumerationDef("fixed", PropertyValue.True),
        };

        internal static PropertyValue ParseTableLayout(BufferString value, FormatConverter formatConverter)
        {
            return ParseEnum(value, TableLayoutEnumeration);
        }

        

        private static EnumerationDef[] BorderCollapseEnumeration =
        {
            
            new EnumerationDef("collapse", PropertyValue.True),
            new EnumerationDef("separate", PropertyValue.False),
        };

        internal static PropertyValue ParseBorderCollapse(BufferString value, FormatConverter formatConverter)
        {
            return ParseEnum(value, BorderCollapseEnumeration);
        }

        

        private static EnumerationDef[] EmptyCellsEnumeration =
        {
            
            new EnumerationDef("show", PropertyValue.True),
            new EnumerationDef("hide", PropertyValue.False),
        };

        internal static PropertyValue ParseEmptyCells(BufferString value, FormatConverter formatConverter)
        {
            return ParseEnum(value, EmptyCellsEnumeration);
        }

        

        private static EnumerationDef[] CaptionSideEnumeration =
        {
            
            new EnumerationDef("bottom", PropertyValue.True),
            new EnumerationDef("top", PropertyValue.False),
        };

        internal static PropertyValue ParseCaptionSide(BufferString value, FormatConverter formatConverter)
        {
            return ParseEnum(value, CaptionSideEnumeration);
        }

        

        private static EnumerationDef[] BorderWidthEnumeration =
        {
            
            new EnumerationDef("thin", new PropertyValue(LengthUnits.Pixels, 2)),
            new EnumerationDef("medium", new PropertyValue(LengthUnits.Pixels, 4)),
            new EnumerationDef("thick", new PropertyValue(LengthUnits.Pixels, 6)),
        };

        internal static PropertyValue ParseBorderWidth(BufferString value, FormatConverter formatConverter)
        {
            var result = ParseEnum(value, BorderWidthEnumeration);
            if (result.IsNull)
            {
                result = ParseNonNegativeLength(value, formatConverter);
            }

            return result;
        }

        

        internal enum TableFrame
        {
            Void,
            Above,
            Below,
            Border,
            Box,
            Hsides,
            Lhs,
            Rhs,
            Vsides,
        }

        private static EnumerationDef[] TableFrameEnumeration =
        {
            new EnumerationDef("void", new PropertyValue(TableFrame.Void)),
            new EnumerationDef("above", new PropertyValue(TableFrame.Above)),
            new EnumerationDef("below", new PropertyValue(TableFrame.Below)),
            new EnumerationDef("border", new PropertyValue(TableFrame.Border)),
            new EnumerationDef("box", new PropertyValue(TableFrame.Box)),
            new EnumerationDef("hsides", new PropertyValue(TableFrame.Hsides)),
            new EnumerationDef("lhs", new PropertyValue(TableFrame.Lhs)),
            new EnumerationDef("rhs", new PropertyValue(TableFrame.Rhs)),
            new EnumerationDef("vsides", new PropertyValue(TableFrame.Vsides)),
        };

        internal static PropertyValue ParseTableFrame(BufferString value, FormatConverter formatConverter)
        {
            return ParseEnum(value, TableFrameEnumeration);
        }

        internal static string GetTableFrameString(PropertyValue value)
        {
            return GetEnumString(value, TableFrameEnumeration);
        }

        

        internal enum TableRules
        {
            None,
            Groups,
            Rows,
            Cells,
            All,
        }

        private static EnumerationDef[] TableRulesEnumeration =
        {
            new EnumerationDef("none", new PropertyValue(TableRules.None)),
            new EnumerationDef("groups", new PropertyValue(TableRules.Groups)),
            new EnumerationDef("rows", new PropertyValue(TableRules.Rows)),
            new EnumerationDef("cells", new PropertyValue(TableRules.Cells)),
            new EnumerationDef("all", new PropertyValue(TableRules.All)),
        };

        internal static PropertyValue ParseTableRules(BufferString value, FormatConverter formatConverter)
        {
            return ParseEnum(value, TableRulesEnumeration);
        }

        internal static string GetTableRulesString(PropertyValue value)
        {
            return GetEnumString(value, TableRulesEnumeration);
        }

        

        private static EnumerationDef[] UnicodeBiDiEnumeration =
        {
            new EnumerationDef("normal", new PropertyValue(UnicodeBiDi.Normal)),
            new EnumerationDef("embed", new PropertyValue(UnicodeBiDi.Embed)),
            new EnumerationDef("bidi-override", new PropertyValue(UnicodeBiDi.Override)),
        };

        internal static PropertyValue ParseUnicodeBiDi(BufferString value, FormatConverter formatConverter)
        {
            return ParseEnum(value, UnicodeBiDiEnumeration);
        }

        internal static string GetUnicodeBiDiString(PropertyValue value)
        {
            return GetEnumString(value, UnicodeBiDiEnumeration);
        }

        
        

        private static EnumerationDef[] DisplayEnumeration =
        {
            new EnumerationDef("none", new PropertyValue(Display.None)),
            new EnumerationDef("inline", new PropertyValue(Display.Inline)),
            new EnumerationDef("block", new PropertyValue(Display.Block)),
            new EnumerationDef("list-item", new PropertyValue(Display.ListItem)),
            new EnumerationDef("run-in", new PropertyValue(Display.RunIn)),
            new EnumerationDef("inline-block", new PropertyValue(Display.InlineBlock)),
            new EnumerationDef("table", new PropertyValue(Display.Table)),
            new EnumerationDef("inline-table", new PropertyValue(Display.InlineTable)),
            new EnumerationDef("table-row-group", new PropertyValue(Display.TableRowGroup)),
            new EnumerationDef("table-header-group", new PropertyValue(Display.TableHeaderGroup)),
            new EnumerationDef("table-footer-group", new PropertyValue(Display.TableFooterGroup)),
            new EnumerationDef("table-row", new PropertyValue(Display.TableRow)),
            new EnumerationDef("table-column-group", new PropertyValue(Display.TableColumnGroup)),
            new EnumerationDef("table-column", new PropertyValue(Display.TableColumn)),
            new EnumerationDef("table-cell", new PropertyValue(Display.TableCell)),
            new EnumerationDef("table-caption", new PropertyValue(Display.TableCaption)),
        };

        internal static PropertyValue ParseDisplay(BufferString value, FormatConverter formatConverter)
        {
            return ParseEnum(value, DisplayEnumeration);
        }

        internal static string GetDisplayString(PropertyValue value)
        {
            return GetEnumString(value, DisplayEnumeration);
        }

        
        

        private static EnumerationDef[] VisibilityEnumeration =
        {
            new EnumerationDef("visible", PropertyValue.True),
            new EnumerationDef("hidden", PropertyValue.False),
            new EnumerationDef("collapse", PropertyValue.False),
        };

        internal static PropertyValue ParseVisibility(BufferString value, FormatConverter formatConverter)
        {
            return ParseEnum(value, VisibilityEnumeration);
        }

        

        internal static PropertyValue ParseLanguage(BufferString value, FormatConverter formatConverter)
        {
            value.TrimWhitespace();
            if (value.Length == 0)
            {
                return PropertyValue.Null;
            }

            Globalization.Culture culture;

            if (Globalization.Culture.TryGetCulture(value.ToString(), out culture) && culture.LCID != 0)
            {
                return new PropertyValue(PropertyType.Integer, culture.LCID);
            }

            return PropertyValue.Null;
        }

        
        

        public static PropertyValue ParseColor(BufferString value, bool enriched, bool css)
        {
            var offset = 0;
            RGBT rgbt;

            if (value.Length == 0 || value[0] != '#' || enriched)
            {
                
                int i;
                var notAName = false;
                var foundNonHex = false;

                for (i = 0; i < value.Length; i++)
                {
                    if (!ParseSupport.AlphaCharacter(ParseSupport.GetCharClass(value[i])))
                    {
                        notAName = true;
                        break;
                    }
                    if (!foundNonHex && !ParseSupport.HexCharacter(ParseSupport.GetCharClass(value[i])))
                        foundNonHex = true;
                }

                
                if (!notAName && foundNonHex)
                {
                    var pv = ParseNamedColor(value);
                    if (!pv.IsNull)
                    {
                        return pv;
                    }
                }

                if (!enriched)
                {
                    
                    var pv = ParseRgbColor(value);
                    if (!pv.IsNull)
                    {
                        return pv;
                    }
                }

                
                
                
                
            }
            else
            {
                 
                 offset++;
            }

            if (enriched)
            {
                if (ParseHexColorEnriched(value, offset, out rgbt))
                {
                    return new PropertyValue(rgbt);
                }
            }
            else
            {
                
                
                if (ParseHexColor(value, offset, css, out rgbt))
                {
                    return new PropertyValue(rgbt);
                }
            }

            return PropertyValue.Null;
        }

        internal enum SystemColors
        {
            

            ActiveBorder = 10,   
            ActiveCaption = 2,   
            AppWorkspace = 12,   
            Background = 1,      
            ButtonFace = 15,     
            ButtonHighlight = 20, 
            ButtonShadow = 16,  
            ButtonText = 18,         
            CaptionText = 9,     
            GrayText = 17,        
            Highlight = 13,       
            HighlightText = 14,   
            InactiveBorder = 11,  
            InactiveCaption = 3,  
            InactiveCaptionText = 19, 
            InfoBackground = 24,  
            InfoText = 23,        
            Menu = 4,            
            MenuText = 7,        
            Scrollbar = 0,       
            ThreeDDarkShadow = 21,   
            ThreeDFace  = ButtonFace,
            ThreeDHighlight = ButtonHighlight,  
            ThreeDLightShadow = 22, 
            ThreeDShadow = ButtonShadow,    
            Window = 5,          
            WindowFrame = 6,     
            WindowText = 8,      
        }

        private static readonly EnumerationDef[] colorNames =
        {
            
            new EnumerationDef("activeborder",          new PropertyValue(SystemColors.ActiveBorder)), 
            new EnumerationDef("activecaption",         new PropertyValue(SystemColors.ActiveCaption)), 
            new EnumerationDef("aliceblue",             new PropertyValue(new RGBT(0x00F0F8FFu))),
            new EnumerationDef("antiquewhite",          new PropertyValue(new RGBT(0x00FAEBD7u))),
            new EnumerationDef("appworkspace",          new PropertyValue(SystemColors.AppWorkspace)), 
            new EnumerationDef("aqua",                  new PropertyValue(new RGBT(0x0000FFFFu))),
            new EnumerationDef("aquamarine",            new PropertyValue(new RGBT(0x007FFFD4u))),
            new EnumerationDef("azure",                 new PropertyValue(new RGBT(0x00F0FFFFu))),
            new EnumerationDef("background",            new PropertyValue(SystemColors.Background)), 
            new EnumerationDef("beige",                 new PropertyValue(new RGBT(0x00F5F5DCu))),
            new EnumerationDef("bisque",                new PropertyValue(new RGBT(0x00FFE4C4u))),
            new EnumerationDef("black",                 new PropertyValue(new RGBT(0x00000000u))),
            new EnumerationDef("blanchedalmond",        new PropertyValue(new RGBT(0x00FFEBCDu))),
            new EnumerationDef("blue",                  new PropertyValue(new RGBT(0x000000FFu))),
            new EnumerationDef("blueviolet",            new PropertyValue(new RGBT(0x008A2BE2u))),
            new EnumerationDef("brown",                 new PropertyValue(new RGBT(0x00A52A2Au))),
            new EnumerationDef("burlywood",             new PropertyValue(new RGBT(0x00DEB887u))),
            new EnumerationDef("buttonface",            new PropertyValue(SystemColors.ButtonFace)), 
            new EnumerationDef("buttonhighlight",       new PropertyValue(SystemColors.ButtonHighlight)), 
            new EnumerationDef("buttonshadow",          new PropertyValue(SystemColors.ButtonShadow)), 
            new EnumerationDef("buttontext",            new PropertyValue(SystemColors.ButtonText)), 
            new EnumerationDef("cadetblue",             new PropertyValue(new RGBT(0x005F9EA0u))),
            new EnumerationDef("captiontext",           new PropertyValue(SystemColors.CaptionText)), 
            new EnumerationDef("chartreuse",            new PropertyValue(new RGBT(0x007FFF00u))),
            new EnumerationDef("chocolate",             new PropertyValue(new RGBT(0x00D2691Eu))),
            new EnumerationDef("coral",                 new PropertyValue(new RGBT(0x00FF7F50u))),
            new EnumerationDef("cornflowerblue",        new PropertyValue(new RGBT(0x006495EDu))),
            new EnumerationDef("cornsilk",              new PropertyValue(new RGBT(0x00FFF8DCu))),
            new EnumerationDef("crimson",               new PropertyValue(new RGBT(0x00DC143Cu))),
            new EnumerationDef("cyan",                  new PropertyValue(new RGBT(0x0000FFFFu))),
            new EnumerationDef("darkblue",              new PropertyValue(new RGBT(0x0000008Bu))),
            new EnumerationDef("darkcyan",              new PropertyValue(new RGBT(0x00008B8Bu))),
            new EnumerationDef("darkgoldenrod",         new PropertyValue(new RGBT(0x00B8860Bu))),
            new EnumerationDef("darkgray",              new PropertyValue(new RGBT(0x00A9A9A9u))),
            new EnumerationDef("darkgreen",             new PropertyValue(new RGBT(0x00006400u))),
            new EnumerationDef("darkkhaki",             new PropertyValue(new RGBT(0x00BDB76Bu))),
            new EnumerationDef("darkmagenta",           new PropertyValue(new RGBT(0x008B008Bu))),
            new EnumerationDef("darkolivegreen",        new PropertyValue(new RGBT(0x00556B2Fu))),
            new EnumerationDef("darkorange",            new PropertyValue(new RGBT(0x00FF8C00u))),
            new EnumerationDef("darkorchid",            new PropertyValue(new RGBT(0x009932CCu))),
            new EnumerationDef("darkred",               new PropertyValue(new RGBT(0x008B0000u))),
            new EnumerationDef("darksalmon",            new PropertyValue(new RGBT(0x00E9967Au))),
            new EnumerationDef("darkseagreen",          new PropertyValue(new RGBT(0x008FBC8Fu))),
            new EnumerationDef("darkslateblue",         new PropertyValue(new RGBT(0x00483D8Bu))),
            new EnumerationDef("darkslategray",         new PropertyValue(new RGBT(0x002F4F4Fu))),
            new EnumerationDef("darkturquoise",         new PropertyValue(new RGBT(0x0000CED1u))),
            new EnumerationDef("darkviolet",            new PropertyValue(new RGBT(0x009400D3u))),
            new EnumerationDef("deeppink",              new PropertyValue(new RGBT(0x00FF1493u))),
            new EnumerationDef("deepskyblue",           new PropertyValue(new RGBT(0x0000BFFFu))),
            new EnumerationDef("dimgray",               new PropertyValue(new RGBT(0x00696969u))),
            new EnumerationDef("dodgerblue",            new PropertyValue(new RGBT(0x001E90FFu))),
            new EnumerationDef("firebrick",             new PropertyValue(new RGBT(0x00B22222u))),
            new EnumerationDef("floralwhite",           new PropertyValue(new RGBT(0x00FFFAF0u))),
            new EnumerationDef("forestgreen",           new PropertyValue(new RGBT(0x00228B22u))),
            new EnumerationDef("fuchsia",               new PropertyValue(new RGBT(0x00FF00FFu))),
            new EnumerationDef("gainsboro",             new PropertyValue(new RGBT(0x00DCDCDCu))),
            new EnumerationDef("ghostwhite",            new PropertyValue(new RGBT(0x00F8F8FFu))),
            new EnumerationDef("gold",                  new PropertyValue(new RGBT(0x00FFD700u))),
            new EnumerationDef("goldenrod",             new PropertyValue(new RGBT(0x00DAA520u))),
            new EnumerationDef("gray",                  new PropertyValue(new RGBT(0x00808080u))),
            new EnumerationDef("graytext",              new PropertyValue(SystemColors.GrayText)), 
            new EnumerationDef("green",                 new PropertyValue(new RGBT(0x00008000u))),
            new EnumerationDef("greenyellow",           new PropertyValue(new RGBT(0x00ADFF2Fu))),
            new EnumerationDef("highlight",             new PropertyValue(SystemColors.Highlight)), 
            new EnumerationDef("highlighttext",         new PropertyValue(SystemColors.HighlightText)), 
            new EnumerationDef("honeydew",              new PropertyValue(new RGBT(0x00F0FFF0u))),
            new EnumerationDef("hotpink",               new PropertyValue(new RGBT(0x00FF69B4u))),
            new EnumerationDef("inactiveborder",        new PropertyValue(SystemColors.InactiveBorder)), 
            new EnumerationDef("inactivecaption",       new PropertyValue(SystemColors.InactiveCaption)), 
            new EnumerationDef("inactivecaptiontext",   new PropertyValue(SystemColors.InactiveCaptionText)), 
            new EnumerationDef("indianred",             new PropertyValue(new RGBT(0x00CD5C5Cu))),
            new EnumerationDef("indigo",                new PropertyValue(new RGBT(0x004B0082u))),
            new EnumerationDef("infobackground",        new PropertyValue(SystemColors.InfoBackground)), 
            new EnumerationDef("infotext",              new PropertyValue(SystemColors.InfoText)), 
            new EnumerationDef("ivory",                 new PropertyValue(new RGBT(0x00FFFFF0u))),
            new EnumerationDef("khaki",                 new PropertyValue(new RGBT(0x00F0E68Cu))),
            new EnumerationDef("lavender",              new PropertyValue(new RGBT(0x00E6E6FAu))),
            new EnumerationDef("lavenderblush",         new PropertyValue(new RGBT(0x00FFF0F5u))),
            new EnumerationDef("lawngreen",             new PropertyValue(new RGBT(0x007CFC00u))),
            new EnumerationDef("lemonchiffon",          new PropertyValue(new RGBT(0x00FFFACDu))),
            new EnumerationDef("lightblue",             new PropertyValue(new RGBT(0x00ADD8E6u))),
            new EnumerationDef("lightcoral",            new PropertyValue(new RGBT(0x00F08080u))),
            new EnumerationDef("lightcyan",             new PropertyValue(new RGBT(0x00E0FFFFu))),
            new EnumerationDef("lightgoldenrodyellow",  new PropertyValue(new RGBT(0x00FAFAD2u))),
            new EnumerationDef("lightgreen",            new PropertyValue(new RGBT(0x0090EE90u))),
            new EnumerationDef("lightgrey",             new PropertyValue(new RGBT(0x00D3D3D3u))),
            new EnumerationDef("lightpink",             new PropertyValue(new RGBT(0x00FFB6C1u))),
            new EnumerationDef("lightsalmon",           new PropertyValue(new RGBT(0x00FFA07Au))),
            new EnumerationDef("lightseagreen",         new PropertyValue(new RGBT(0x0020B2AAu))),
            new EnumerationDef("lightskyblue",          new PropertyValue(new RGBT(0x0087CEFAu))),
            new EnumerationDef("lightslategray",        new PropertyValue(new RGBT(0x00778899u))),
            new EnumerationDef("lightsteelblue",        new PropertyValue(new RGBT(0x00B0C4DEu))),
            new EnumerationDef("lightyellow",           new PropertyValue(new RGBT(0x00FFFFE0u))),
            new EnumerationDef("lime",                  new PropertyValue(new RGBT(0x0000FF00u))),
            new EnumerationDef("limegreen",             new PropertyValue(new RGBT(0x0032CD32u))),
            new EnumerationDef("linen",                 new PropertyValue(new RGBT(0x00FAF0E6u))),
            new EnumerationDef("magenta",               new PropertyValue(new RGBT(0x00FF00FFu))),
            new EnumerationDef("maroon",                new PropertyValue(new RGBT(0x00800000u))),
            new EnumerationDef("mediumaquamarine",      new PropertyValue(new RGBT(0x0066CDAAu))),
            new EnumerationDef("mediumblue",            new PropertyValue(new RGBT(0x000000CDu))),
            new EnumerationDef("mediumorchid",          new PropertyValue(new RGBT(0x00BA55D3u))),
            new EnumerationDef("mediumpurple",          new PropertyValue(new RGBT(0x009370DBu))),
            new EnumerationDef("mediumseagreen",        new PropertyValue(new RGBT(0x003CB371u))),
            new EnumerationDef("mediumslateblue",       new PropertyValue(new RGBT(0x007B68EEu))),
            new EnumerationDef("mediumspringgreen",     new PropertyValue(new RGBT(0x0000FA9Au))),
            new EnumerationDef("mediumturquoise",       new PropertyValue(new RGBT(0x0048D1CCu))),
            new EnumerationDef("mediumvioletred",       new PropertyValue(new RGBT(0x00C71585u))),
            new EnumerationDef("menu",                  new PropertyValue(SystemColors.Menu)), 
            new EnumerationDef("menutext",              new PropertyValue(SystemColors.MenuText)), 
            new EnumerationDef("midnightblue",          new PropertyValue(new RGBT(0x00191970u))),
            new EnumerationDef("mintcream",             new PropertyValue(new RGBT(0x00F5FFFAu))),
            new EnumerationDef("mistyrose",             new PropertyValue(new RGBT(0x00FFE4E1u))),
            new EnumerationDef("moccasin",              new PropertyValue(new RGBT(0x00FFE4B5u))),
            new EnumerationDef("navajowhite",           new PropertyValue(new RGBT(0x00FFDEADu))),
            new EnumerationDef("navy",                  new PropertyValue(new RGBT(0x00000080u))),
            new EnumerationDef("oldlace",               new PropertyValue(new RGBT(0x00FDF5E6u))),
            new EnumerationDef("olive",                 new PropertyValue(new RGBT(0x00808000u))),
            new EnumerationDef("olivedrab",             new PropertyValue(new RGBT(0x006B8E23u))),
            new EnumerationDef("orange",                new PropertyValue(new RGBT(0x00FFA500u))),
            new EnumerationDef("orangered",             new PropertyValue(new RGBT(0x00FF4500u))),
            new EnumerationDef("orchid",                new PropertyValue(new RGBT(0x00DA70D6u))),
            new EnumerationDef("palegoldenrod",         new PropertyValue(new RGBT(0x00EEE8AAu))),
            new EnumerationDef("palegreen",             new PropertyValue(new RGBT(0x0098FB98u))),
            new EnumerationDef("paleturquoise",         new PropertyValue(new RGBT(0x00AFEEEEu))),
            new EnumerationDef("palevioletred",         new PropertyValue(new RGBT(0x00DB7093u))),
            new EnumerationDef("papayawhip",            new PropertyValue(new RGBT(0x00FFEFD5u))),
            new EnumerationDef("peachpuff",             new PropertyValue(new RGBT(0x00FFDAB9u))),
            new EnumerationDef("peru",                  new PropertyValue(new RGBT(0x00CD853Fu))),
            new EnumerationDef("pink",                  new PropertyValue(new RGBT(0x00FFC0CBu))),
            new EnumerationDef("plum",                  new PropertyValue(new RGBT(0x00DDA0DDu))),
            new EnumerationDef("powderblue",            new PropertyValue(new RGBT(0x00B0E0E6u))),
            new EnumerationDef("purple",                new PropertyValue(new RGBT(0x00800080u))),
            new EnumerationDef("red",                   new PropertyValue(new RGBT(0x00FF0000u))),
            new EnumerationDef("rosybrown",             new PropertyValue(new RGBT(0x00BC8F8Fu))),
            new EnumerationDef("royalblue",             new PropertyValue(new RGBT(0x004169E1u))),
            new EnumerationDef("saddlebrown",           new PropertyValue(new RGBT(0x008B4513u))),
            new EnumerationDef("salmon",                new PropertyValue(new RGBT(0x00FA8072u))),
            new EnumerationDef("sandybrown",            new PropertyValue(new RGBT(0x00F4A460u))),
            new EnumerationDef("scrollbar",             new PropertyValue(SystemColors.Scrollbar)), 
            new EnumerationDef("seagreen",              new PropertyValue(new RGBT(0x002E8B57u))),
            new EnumerationDef("seashell",              new PropertyValue(new RGBT(0x00FFF5EEu))),
            new EnumerationDef("sienna",                new PropertyValue(new RGBT(0x00A0522Du))),
            new EnumerationDef("silver",                new PropertyValue(new RGBT(0x00C0C0C0u))),
            new EnumerationDef("skyblue",               new PropertyValue(new RGBT(0x0087CEEBu))),
            new EnumerationDef("slateblue",             new PropertyValue(new RGBT(0x006A5ACDu))),
            new EnumerationDef("slategray",             new PropertyValue(new RGBT(0x00708090u))),
            new EnumerationDef("snow",                  new PropertyValue(new RGBT(0x00FFFAFAu))),
            new EnumerationDef("springgreen",           new PropertyValue(new RGBT(0x0000FF7Fu))),
            new EnumerationDef("steelblue",             new PropertyValue(new RGBT(0x004682B4u))),
            new EnumerationDef("tan",                   new PropertyValue(new RGBT(0x00D2B48Cu))),
            new EnumerationDef("teal",                  new PropertyValue(new RGBT(0x00008080u))),
            new EnumerationDef("thistle",               new PropertyValue(new RGBT(0x00D8BFD8u))),
            new EnumerationDef("threeddarkshadow",      new PropertyValue(SystemColors.ThreeDDarkShadow)), 
            new EnumerationDef("threedface",            new PropertyValue(SystemColors.ThreeDFace)),
            new EnumerationDef("threedhighlight",       new PropertyValue(SystemColors.ThreeDHighlight)), 
            new EnumerationDef("threedlightshadow",     new PropertyValue(SystemColors.ThreeDLightShadow)), 
            new EnumerationDef("threedshadow",          new PropertyValue(SystemColors.ThreeDShadow)), 
            new EnumerationDef("tomato",                new PropertyValue(new RGBT(0x00FF6347u))),
            new EnumerationDef("transparent",           new PropertyValue(new RGBT(0u, 0u, 0u, 7u))), 
            new EnumerationDef("turquoise",             new PropertyValue(new RGBT(0x0040E0D0u))),
            new EnumerationDef("violet",                new PropertyValue(new RGBT(0x00EE82EEu))),
            new EnumerationDef("wheat",                 new PropertyValue(new RGBT(0x00F5DEB3u))),
            new EnumerationDef("white",                 new PropertyValue(new RGBT(0x00FFFFFFu))),
            new EnumerationDef("whitesmoke",            new PropertyValue(new RGBT(0x00F5F5F5u))),
            new EnumerationDef("window",                new PropertyValue(SystemColors.Window)), 
            new EnumerationDef("windowframe",           new PropertyValue(SystemColors.WindowFrame)), 
            new EnumerationDef("windowtext",            new PropertyValue(SystemColors.WindowText)), 
            new EnumerationDef("yellow",                new PropertyValue(new RGBT(0x00FFFF00u))),
            new EnumerationDef("yellowgreen",           new PropertyValue(new RGBT(0x009ACD32u))),
        };

        private static Dictionary<PropertyValue, string> colorToNameDictionary = BuildColorToNameDictionary();

        private static Dictionary<PropertyValue, string> BuildColorToNameDictionary()
        {
            var result = new Dictionary<PropertyValue, string>();

            foreach (var def in colorNames)
            {
                if (!result.ContainsKey(def.value))
                {
                    result.Add(def.value, def.name);
                }
            }

            return result;
        }

        private static PropertyValue ParseNamedColor(BufferString value)
        {
            

            var lo = 0;
            var hi = colorNames.Length - 1;

            while (lo <= hi)
            {
                var i = lo + ((hi -lo) >> 1);

                var order = BufferString.CompareLowerCaseStringToBufferStringIgnoreCase(colorNames[i].name, value);
                if (order == 0)
                {
                    return colorNames[i].value;
                }

                if (order < 0)
                {
                    lo = i + 1;
                }
                else
                {
                    hi = i - 1;
                }
            }
            
            return PropertyValue.Null;
        }

        internal static PropertyValue TranslateSystemColor(PropertyValue value)
        {
            InternalDebug.Assert(value.IsEnum);

            switch ((SystemColors)value.Enum)
            {
                
                case SystemColors.ActiveBorder: return new PropertyValue(new RGBT(0x00000000));
                case SystemColors.ActiveCaption: return new PropertyValue(new RGBT(0x00FFFFFF));
                case SystemColors.AppWorkspace: return new PropertyValue(new RGBT(0x00FFFFFF));
                case SystemColors.Background: return new PropertyValue(new RGBT(0x00FFFFFF));
                case SystemColors.ButtonFace: return new PropertyValue(new RGBT(0x00FFFFFF));
                case SystemColors.ButtonHighlight: return new PropertyValue(new RGBT(0x00FFFFFF));
                case SystemColors.ButtonShadow: return new PropertyValue(new RGBT(0x00FFFFFF));
                case SystemColors.ButtonText: return new PropertyValue(new RGBT(0x00000000));
                case SystemColors.CaptionText: return new PropertyValue(new RGBT(0x00000000));
                case SystemColors.GrayText: return new PropertyValue(new RGBT(0x00000000));
                case SystemColors.Highlight: return new PropertyValue(new RGBT(0x00FFFFFF));
                case SystemColors.HighlightText: return new PropertyValue(new RGBT(0x00000000));
                case SystemColors.InactiveBorder: return new PropertyValue(new RGBT(0x00000000));
                case SystemColors.InactiveCaption: return new PropertyValue(new RGBT(0x00FFFFFF));
                case SystemColors.InactiveCaptionText: return new PropertyValue(new RGBT(0x00000000));
                case SystemColors.InfoBackground: return new PropertyValue(new RGBT(0x00FFFFFF));
                case SystemColors.InfoText: return new PropertyValue(new RGBT(0x00000000));
                case SystemColors.Menu: return new PropertyValue(new RGBT(0x00FFFFFF));
                case SystemColors.MenuText: return new PropertyValue(new RGBT(0x00000000));
                case SystemColors.Scrollbar: return new PropertyValue(new RGBT(0x00FFFFFF));
                case SystemColors.ThreeDDarkShadow: return new PropertyValue(new RGBT(0x00FFFFFF));
                case SystemColors.ThreeDLightShadow: return new PropertyValue(new RGBT(0x00FFFFFF));
                case SystemColors.Window: return new PropertyValue(new RGBT(0x00FFFFFF));       
                case SystemColors.WindowFrame: return new PropertyValue(new RGBT(0x00000000));
                case SystemColors.WindowText: return new PropertyValue(new RGBT(0x00000000));
            }

            InternalDebug.Assert(false);
            return PropertyValue.Null;
        }

        private static PropertyValue ParseRgbColor(BufferString value)
        {
            if (value.Length <= 4 || !value.StartsWithLowerCaseStringIgnoreCase("rgb("))
            {
                return PropertyValue.Null;
            }

            var offset = 4;
            uint r, g, b;

            if (!ParseRgbParam(value, ref offset, out r))
            {
                return PropertyValue.Null;
            }

            if (!ParseRgbParam(value, ref offset, out g))
            {
                return PropertyValue.Null;
            }

            if (!ParseRgbParam(value, ref offset, out b))
            {
                return PropertyValue.Null;
            }

            if (offset != value.Length - 1 || value[offset] != ')')
            {
                return PropertyValue.Null;
            }

            return new PropertyValue(new RGBT(r, g, b));
        }

        private static bool ParseRgbParam(BufferString str, ref int offset, out uint result)
        {
            uint value = 0;
            uint fractionDecimal = 1;
            var isNegative = false;

            while (offset < str.Length && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(str[offset])))
            {
                offset++;
            }

            if (offset < str.Length && str[offset] == '-')
            {
                isNegative = true;
                offset++;
            }

            if (offset == str.Length || !ParseSupport.NumericCharacter(ParseSupport.GetCharClass(str[offset])))
            {
                result = 0;
                return false;
            }

            while (offset < str.Length && ParseSupport.NumericCharacter(ParseSupport.GetCharClass(str[offset])))
            {
                value *= 10;
                value += (uint)str[offset] - '0';
                offset++;
                if (value > 255)
                {   
                    while (offset < str.Length && ParseSupport.NumericCharacter(ParseSupport.GetCharClass(str[offset])))
                    {
                        offset++;
                    }
                }
            }

            if (offset < str.Length && str[offset] == '.')
            {
                offset++;
                while (offset < str.Length && ParseSupport.NumericCharacter(ParseSupport.GetCharClass(str[offset])))
                {
                    value *= 10;
                    value += (uint)str[offset] - '0';
                    fractionDecimal *= 10;
                    offset++;

                    if (value > (Int32.MaxValue / 5100)) 
                    {   
                        while (offset < str.Length && ParseSupport.NumericCharacter(ParseSupport.GetCharClass(str[offset])))
                        {
                            offset++;
                        }
                    }
                }
            }

            if (offset < str.Length && str[offset] == '%')
            {
                if (value / fractionDecimal >= 100)
                    result = 255;
                else
                    result = (value * 255) / (fractionDecimal * 100);
                offset++;
            }
            else
            {
                if (value / fractionDecimal > 255)
                    result = 255;
                else
                    result = value / fractionDecimal;
            }

            while (offset < str.Length && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(str[offset])))
            {
                offset++;
            }

            if (offset < str.Length && str[offset] == ',')
            {
                offset++;
            }

            if ( isNegative )
                result = 0;

            return true;
        }

        
        
        
        
        
        
        
        
        
        
        
        

        private static bool ParseHexColor(BufferString str, int offset, bool css, out RGBT rgbt)
        {
            var length = str.Length - offset;
            if (css && length != 3 && length != 6 && length != 9)
            {
                rgbt = new RGBT();
                return false;
            }

            var vlen = (str.Length - offset + 3 - 1) / 3;   
            uint r, g, b;
            uint max = 0;

            
            if (!ParseHexColorPart(vlen, str, ref offset, ref max, css, out r) ||
                !ParseHexColorPart(vlen, str, ref offset, ref max, css, out g) ||
                !ParseHexColorPart(vlen, str, ref offset, ref max, css, out b))
            {
                rgbt = new RGBT();
                return false;
            }

            
            
            
            int i;
            for (i = 0 ; max > 255 ; i++) 
            {
                max >>= 4; 
            }

            if (i > 0)
            {
                r >>= i * 4;
                g >>= i * 4;
                b >>= i * 4;
            }

            
            
            
            if (css)
            {
                
                if (vlen == 1)  
                {
                    r += r << 4;
                    g += g << 4;
                    b += b << 4;
                }
            }

            rgbt = new RGBT(r, g, b);
            return true;
        }

        private static bool ParseHexColorPart(int vlen, BufferString str, ref int offset, ref uint max, bool css, out uint result)
        {
            result = 0;

            for (var j = 0; j < vlen; j++)
            {
                int hex;
                if (offset >= str.Length)
                {
                    if (css)
                    {
                        return false;
                    }
                    hex = 0;
                }
                else
                {
                    if (ParseSupport.HexCharacter(ParseSupport.GetCharClass(str[offset])))
                    {
                        hex = ParseSupport.CharToHex(str[offset]);
                    }
                    else
                    {
                        if (css)
                        {
                            return false;
                        }
                        hex = 0; 
                    }
                }

                result = (result << 4) + (uint)hex;
                offset ++;
            }

            if (result > max)
            {
                max = result;
            }
            return true;
        }

        private static bool ParseHexColorEnriched(BufferString str, int offset, out RGBT rgbt)
        {
            uint r, g, b;

            
            if (!ParseHexColorPartEnriched(str, ref offset, out r) ||
                !ParseHexColorPartEnriched(str, ref offset, out g) ||
                !ParseHexColorPartEnriched(str, ref offset, out b))
            {
                rgbt = new RGBT();
                return false;
            }

            
            
            r >>= 8;
            g >>= 8;
            b >>= 8;

            rgbt = new RGBT(r, g, b);
            return true;
        }

        private static bool ParseHexColorPartEnriched(BufferString str, ref int offset, out uint result)
        {
            result = 0;

            for (var j = 0; j < 4; j++)
            {
                int hex;

                if (offset >= str.Length)
                {
                    hex = 0;
                }
                else
                {
                    if (ParseSupport.HexCharacter(ParseSupport.GetCharClass(str[offset])))
                    {
                        hex = ParseSupport.CharToHex(str[offset]);
                    }
                    else
                    {
                        hex = 0; 
                    }

                    offset ++;
                }

                result = (result << 4) + (uint)hex;
            }

            if (offset < str.Length && str[offset] == ',')
            {
                offset ++;
            }

            return true;
        }

        

        internal static PropertyValue ParseStringProperty(BufferString value, FormatConverter formatConverter)
        {
            value.TrimWhitespace();
            if (value.Length == 0)
            {
                return PropertyValue.Null;
            }

            var sv = formatConverter.RegisterStringValue(false, value.ToString(), 0, value.Length);
            return sv.PropertyValue;
        }

        

        internal static PropertyValue ParseFontFace(BufferString value, FormatConverter formatConverter)
        {
            var offset = 0;
            var end = value.Length;

            
            while (offset < end && (value[offset] == ',' || ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(value[offset]))))
            {
                offset++;
            }

            if (offset == end)
            {
                return PropertyValue.Null;
            }

            var chStop = ',';
            if (value[offset] == '\'' || value[offset] == '\"')
            {
                chStop = value[offset];
                offset ++;
            }

            var endName = offset;
            var nextName = offset;
            while (endName < end && value[endName] != chStop)
            {
                
                endName++;
                nextName ++;
            }

            
            while (offset < endName && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(value[endName - 1])))
            {
                endName--;
            }

            var pv = formatConverter.RegisterFaceName(false, value.SubString(offset, endName - offset));

            if (nextName < end)
            {
                
                nextName ++;
            }

            offset = nextName;
            
            
            while (offset < end && (value[offset] == ',' || ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(value[offset]))))
            {
                offset++;
            }

            if (offset == end)
            {
                
                return pv;
            }

            

            MultiValueBuilder mvBuilder;
            var mv = formatConverter.RegisterMultiValue(false, out mvBuilder);

            if (!pv.IsNull)
            {
                mvBuilder.AddValue(pv);
            }

            do
            {
                chStop = ',';
                if (value[offset] == '\'' || value[offset] == '\"')
                {
                    chStop = value[offset];
                    offset ++;
                }

                endName = offset;
                nextName = offset;
                while (endName < end && value[endName] != chStop)
                {
                    
                    endName++;
                    nextName ++;
                }

                nextName = endName;

                
                while (offset < endName && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(value[endName - 1])))
                {
                    endName--;
                }

                pv = formatConverter.RegisterFaceName(false, value.SubString(offset, endName - offset));
                if (!pv.IsNull)
                {
                    mvBuilder.AddValue(pv);
                }

                if (nextName < end)
                {
                    
                    nextName ++;
                }

                offset = nextName;

                
                while (offset < end && (value[offset] == ',' || ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(value[offset]))))
                {
                    offset++;
                }
            }
            while (offset < end);

            
            if (mvBuilder.Count == 0)
            {
                mvBuilder.Cancel();
                mv.Release();
                return PropertyValue.Null;
            }
            else if (mvBuilder.Count == 1)
            {
                
                pv = mvBuilder[0];
                if (pv.IsString)
                {
                    formatConverter.GetStringValue(pv).AddRef(); 
                }
                mvBuilder.Cancel();
                mv.Release();
                return pv;
            }

            mvBuilder.Flush();
            return mv.PropertyValue;
        }

        

        internal static PropertyValue ParseColor(BufferString value, FormatConverter formatConverter)
        {
            return ParseColor(value, false, false);
        }

        internal static PropertyValue ParseColorCss(BufferString value, FormatConverter formatConverter)
        {
            return ParseColor(value, false, true);
        }

        

        public static BufferString FormatColor(ref ScratchBuffer scratchBuffer, PropertyValue value)
        {
            scratchBuffer.Reset();
            AppendColor(ref scratchBuffer, value);
            return scratchBuffer.BufferString;
        }

        public static void AppendColor(ref ScratchBuffer scratchBuffer, PropertyValue value)
        {
            if (value.IsColor || value.IsEnum)
            {
                string name;
                if (colorToNameDictionary.TryGetValue(value, out name))
                {
                    scratchBuffer.Append(name);
                }
                else
                {
                    scratchBuffer.Append("#");
                    scratchBuffer.AppendHex2(value.Color.Red);
                    scratchBuffer.AppendHex2(value.Color.Green);
                    scratchBuffer.AppendHex2(value.Color.Blue);
                }
            }
        }

        

        internal static PropertyValue ParseFontSize(BufferString value, FormatConverter formatConverter)
        {
            return ParseNumber(value, NumberParseFlags.FontSize);
        }

        

        internal static PropertyValue ParseInteger(BufferString value, FormatConverter formatConverter)
        {
            return ParseNumber(value, NumberParseFlags.Integer);
        }

        

        internal static PropertyValue ParseNonNegativeInteger(BufferString value, FormatConverter formatConverter)
        {
            return ParseNumber(value, NumberParseFlags.Integer | NumberParseFlags.NonNegative);
        }

        

        internal static PropertyValue ParseLength(BufferString value, FormatConverter formatConverter)
        {
            return ParseNumber(value, NumberParseFlags.Length);
        }

        

        internal static PropertyValue ParseNonNegativeLength(BufferString value, FormatConverter formatConverter)
        {
            return ParseNumber(value, NumberParseFlags.NonNegativeLength);
        }

        

        internal static PropertyValue ParseUrl(BufferString value, FormatConverter formatConverter)
        {
            return ParseStringProperty(value, formatConverter);
        }

        

        public static void ScanSkipWhitespace(ref BufferString value)
        {
            var count = value.Length;
            var offset = value.Offset;

            while (count != 0 && ParseSupport.WhitespaceCharacter(value.Buffer[offset]))
            {
                offset++;
                count --;
            }

            value.Trim(offset - value.Offset, count);
        }

        

        public static void ScanRevertLastToken(ref BufferString value, BufferString token)
        {
            InternalDebug.Assert(value.Offset == token.Offset + token.Length && value.Buffer == token.Buffer);

            value.Set(value.Buffer, token.Offset, value.Length + token.Length);
        }

        

        public static BufferString ScanNextNonWhitespaceToken(ref BufferString value)
        {
            
            

            var count = value.Length;
            var offset = value.Offset;

            while (count != 0 && !ParseSupport.WhitespaceCharacter(value.Buffer[offset]))
            {
                offset++;
                count --;
            }

            var result = new BufferString(value.Buffer, value.Offset, offset - value.Offset);

            value.Trim(offset - value.Offset, count);

            return result;
        }

        

        public static BufferString ScanNextParenthesizedToken(ref BufferString value)
        {
            
            

            var count = value.Length;
            var offset = value.Offset;

            var quoteChar = '\"';
            var inQuotes = false;
            var parenDepth = 0;

            while (count != 0 && (parenDepth != 0 || !ParseSupport.WhitespaceCharacter(value.Buffer[offset])))
            {
                var ch = value.Buffer[offset];

                if (!inQuotes)
                {
                    if (ch == '\'' || ch == '\"')
                    {
                        inQuotes = true;
                        quoteChar = ch;
                    }
                    else if (parenDepth != 0 && ch == ')')
                    {
                        parenDepth --;
                    }
                    else if (ch == '(')
                    {
                        parenDepth ++;
                    }
                }
                else if (ch == quoteChar)
                {
                    inQuotes = false;
                }

                offset++;
                count --;
            }

            var result = new BufferString(value.Buffer, value.Offset, offset - value.Offset);

            value.Trim(offset - value.Offset, count);

            return result;
        }

        

        public static BufferString ScanNextSize(ref BufferString value)
        {
            var count = value.Length;
            var offset = value.Offset;

            var ch = '\0';

            
            if (count != 0)
            {
                ch = value.Buffer[offset];
                if (ch == '-' || ch == '+')
                {
                    offset++;
                    count--;
                }

                
                if (count == 0 || (!ParseSupport.NumericCharacter(ch = value.Buffer[offset]) && ch != '.'))
                {
                    

                    while (count != 0 && (ParseSupport.AlphaCharacter(ch = value.Buffer[offset]) || ch == '-'))
                    {
                        offset++;
                        count--;
                    }
                }
                else
                {
                    

                    
                    while (count != 0 && (ParseSupport.NumericCharacter(ch = value.Buffer[offset]) || ch == '.'))
                    {
                        offset++;
                        count--;
                    }

                    
                    var offsetLastSpace = offset;
                    while (count != 0 && ParseSupport.WhitespaceCharacter(value.Buffer[offset]))
                    {
                        offset++;
                        count --;
                    }

                    
                    if (count >= 2 
                        && (((ch = ParseSupport.ToLowerCase(value.Buffer[offset])) == 'i' && ParseSupport.ToLowerCase(value.Buffer[offset + 1]) == 'n') 
                            || (ch == 'c' && ParseSupport.ToLowerCase(value.Buffer[offset + 1]) == 'm')     
                            || (ch == 'm' && ParseSupport.ToLowerCase(value.Buffer[offset + 1]) == 'm')     
                            || (ch == 'e' && (ParseSupport.ToLowerCase(value.Buffer[offset + 1]) == 'm'     
                                         || ParseSupport.ToLowerCase(value.Buffer[offset + 1]) == 'x'))     
                            || (ch == 'p' && (ParseSupport.ToLowerCase(value.Buffer[offset + 1]) == 't'     
                                        || ParseSupport.ToLowerCase(value.Buffer[offset + 1]) == 'c'        
                                        || ParseSupport.ToLowerCase(value.Buffer[offset + 1]) == 'x'))))    
                    {
                        offset += 2;
                        count -= 2;
                    }
                    else if (count != 0 && value.Buffer[offset] == '%')  
                    {
                        offset ++;
                        count --;
                    }
                    else
                    {
                        

                        
                        count += offset - offsetLastSpace;
                        offset = offsetLastSpace;
                    }
                }
            }

            var result = new BufferString(value.Buffer, value.Offset, offset - value.Offset);

            value.Trim(offset - value.Offset, count);

            return result;
        }

        

        internal static void ParseCompositeFourSidesValue(BufferString value, FormatConverter formatConverter, PropertyId groupPropertyId, Property[] outputProperties, out int parsedPropertiesCount, PropertyValueParsingMethod valueParsingMethod, bool measurements)
        {
            InternalDebug.Assert(outputProperties.Length >= 4);

            var word = BufferString.Null;

            ScanSkipWhitespace(ref value);

            var numValues = 0;

            if (!value.IsEmpty)
            {
                if (!measurements)
                {
                    word = ScanNextParenthesizedToken(ref value);
                }
                else
                {
                    word = ScanNextSize(ref value);
                }

                outputProperties[numValues].Value = valueParsingMethod(word, formatConverter);

                if (!outputProperties[numValues].Value.IsNull)
                {
                    numValues ++;

                    ScanSkipWhitespace(ref value);

                    if (!value.IsEmpty)
                    {
                        if (!measurements)
                        {
                            word = ScanNextParenthesizedToken(ref value);
                        }
                        else
                        {
                            word = ScanNextSize(ref value);
                        }

                        outputProperties[numValues].Value = valueParsingMethod(word, formatConverter);

                        if (!outputProperties[numValues].Value.IsNull)
                        {
                            numValues ++;

                            ScanSkipWhitespace(ref value);

                            if (!value.IsEmpty)
                            {
                                if (!measurements)
                                {
                                    word = ScanNextParenthesizedToken(ref value);
                                }
                                else
                                {
                                    word = ScanNextSize(ref value);
                                }

                                outputProperties[numValues].Value = valueParsingMethod(word, formatConverter);

                                if (!outputProperties[numValues].Value.IsNull)
                                {
                                    numValues ++;

                                    ScanSkipWhitespace(ref value);

                                    if (!value.IsEmpty)
                                    {
                                        if (!measurements)
                                        {
                                            word = ScanNextParenthesizedToken(ref value);
                                        }
                                        else
                                        {
                                            word = ScanNextSize(ref value);
                                        }

                                        outputProperties[numValues].Value = valueParsingMethod(word, formatConverter);

                                        if (!outputProperties[numValues].Value.IsNull)
                                        {
                                            numValues ++;

                                            
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (numValues == 1)
            {
                outputProperties[0].Id = groupPropertyId + (int)Side.Top;
                outputProperties[1].Set(groupPropertyId + (int)Side.Right, outputProperties[0].Value);
                outputProperties[2].Set(groupPropertyId + (int)Side.Bottom, outputProperties[0].Value);
                outputProperties[3].Set(groupPropertyId + (int)Side.Left, outputProperties[0].Value);
                parsedPropertiesCount = 4;
            }
            else if (numValues == 2)
            {
                outputProperties[0].Id = groupPropertyId + (int)Side.Top;
                outputProperties[1].Id = groupPropertyId + (int)Side.Right;
                outputProperties[2].Set(groupPropertyId + (int)Side.Bottom, outputProperties[0].Value);
                outputProperties[3].Set(groupPropertyId + (int)Side.Left, outputProperties[1].Value);
                parsedPropertiesCount = 4;
            }
            else if (numValues == 3)
            {
                outputProperties[0].Id = groupPropertyId + (int)Side.Top;
                outputProperties[1].Id = groupPropertyId + (int)Side.Right;
                outputProperties[2].Id = groupPropertyId + (int)Side.Bottom;
                outputProperties[3].Set(groupPropertyId + (int)Side.Left, outputProperties[1].Value);
                parsedPropertiesCount = 4;
            }
            else if (numValues == 4)
            {
                outputProperties[0].Id = groupPropertyId + (int)Side.Top;
                outputProperties[1].Id = groupPropertyId + (int)Side.Right;
                outputProperties[2].Id = groupPropertyId + (int)Side.Bottom;
                outputProperties[3].Id = groupPropertyId + (int)Side.Left;
                parsedPropertiesCount = 4;
            }
            else
            {
                parsedPropertiesCount = 0;
            }
        }

        

        internal static void ParseCompositeLength(BufferString value, FormatConverter formatConverter, PropertyId groupPropertyId, Property[] outputProperties, out int parsedPropertiesCount)
        {
            ParseCompositeFourSidesValue(value, formatConverter, groupPropertyId, outputProperties, out parsedPropertiesCount, HtmlConverterData.PropertyValueParsingMethods.ParseLength, true);
        }

        

        internal static void ParseCompositeNonNegativeLength(BufferString value, FormatConverter formatConverter, PropertyId groupPropertyId, Property[] outputProperties, out int parsedPropertiesCount)
        {
            ParseCompositeFourSidesValue(value, formatConverter, groupPropertyId, outputProperties, out parsedPropertiesCount, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength, true);
        }

        

        internal static void ParseCompositeColor(BufferString value, FormatConverter formatConverter, PropertyId groupPropertyId, Property[] outputProperties, out int parsedPropertiesCount)
        {
            ParseCompositeFourSidesValue(value, formatConverter, groupPropertyId, outputProperties, out parsedPropertiesCount, HtmlConverterData.PropertyValueParsingMethods.ParseColorCss, false);
        }

        

        internal static void ParseCompositeBorderWidth(BufferString value, FormatConverter formatConverter, PropertyId groupPropertyId, Property[] outputProperties, out int parsedPropertiesCount)
        {
            ParseCompositeFourSidesValue(value, formatConverter, groupPropertyId, outputProperties, out parsedPropertiesCount, HtmlConverterData.PropertyValueParsingMethods.ParseBorderWidth, true);
        }

        

        internal static void ParseCompositeBorderStyle(BufferString value, FormatConverter formatConverter, PropertyId groupPropertyId, Property[] outputProperties, out int parsedPropertiesCount)
        {
            ParseCompositeFourSidesValue(value, formatConverter, groupPropertyId, outputProperties, out parsedPropertiesCount, HtmlConverterData.PropertyValueParsingMethods.ParseBorderStyle, false);
        }

        

        internal static void ParseCompoundBorderSpacing(BufferString value, FormatConverter formatConverter, PropertyId groupPropertyId, Property[] outputProperties, out int parsedPropertiesCount)
        {
            InternalDebug.Assert(outputProperties.Length >= 4);

            var word = BufferString.Null;

            ScanSkipWhitespace(ref value);

            var numValues = 0;

            if (!value.IsEmpty)
            {
                word = ScanNextSize(ref value);

                outputProperties[numValues].Value = HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength(word, formatConverter);

                if (!outputProperties[numValues].Value.IsNull)
                {
                    numValues ++;

                    ScanSkipWhitespace(ref value);

                    if (!value.IsEmpty)
                    {
                        word = ScanNextSize(ref value);

                        outputProperties[numValues].Value = HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength(word, formatConverter);

                        if (!outputProperties[numValues].Value.IsNull)
                        {
                            numValues ++;
                        }
                    }
                }
            }

            if (numValues == 1)
            {
                outputProperties[0].Id = groupPropertyId;
                outputProperties[1].Set(groupPropertyId + 1, outputProperties[0].Value);
                parsedPropertiesCount = 2;
            }
            else if (numValues == 2)
            {
                outputProperties[0].Id = groupPropertyId;
                outputProperties[1].Id = groupPropertyId + 1;
                parsedPropertiesCount = 2;
            }
            else
            {
                parsedPropertiesCount = 0;
            }
        }

        

        internal static void ParseCompositeBorder(BufferString value, FormatConverter formatConverter, PropertyId groupPropertyId, Property[] outputProperties, out int parsedPropertiesCount)
        {
            InternalDebug.Assert(outputProperties.Length >= 3);

            var word = BufferString.Null;

            ScanSkipWhitespace(ref value);

            var numValues = 0;

            InternalDebug.Assert(groupPropertyId == PropertyId.TopBorderWidth || groupPropertyId == PropertyId.RightBorderWidth || groupPropertyId == PropertyId.BottomBorderWidth || groupPropertyId == PropertyId.LeftBorderWidth);
            InternalDebug.Assert(PropertyId.BorderWidths + 4 == PropertyId.BorderStyles && PropertyId.BorderWidths + 8 == PropertyId.BorderColors);

            var widthIndex = -1;
            var styleIndex = -1;
            var colorIndex = -1;

            while (!value.IsEmpty)
            {
                word = ScanNextParenthesizedToken(ref value);

                
                var val = ParseBorderWidth(word, formatConverter);

                if (val.IsNull)
                {
                    
                    val = ParseBorderStyle(word, formatConverter);

                    if (val.IsNull)
                    {
                        
                        val = ParseColorCss(word, formatConverter);

                        if (val.IsNull)
                        {
                            break;
                        }
                        else
                        {
                            

                            if (colorIndex == -1)
                            {
                                colorIndex = numValues;
                                outputProperties[numValues++].Set(groupPropertyId + 8, val);
                            }
                            else
                            {
                                outputProperties[colorIndex].Set(groupPropertyId + 8, val);
                            }
                        }
                    }
                    else
                    {
                        

                        if (styleIndex == -1)
                        {
                            styleIndex = numValues;
                            outputProperties[numValues++].Set(groupPropertyId + 4, val);
                        }
                        else
                        {
                            outputProperties[styleIndex].Set(groupPropertyId + 4, val);
                        }
                    }
                }
                else
                {
                    
                    if (widthIndex == -1)
                    {
                        widthIndex = numValues;
                        outputProperties[numValues++].Set(groupPropertyId, val);
                    }
                    else
                    {
                        outputProperties[widthIndex].Set(groupPropertyId, val);
                    }
                }

                ScanSkipWhitespace(ref value);
            }

            parsedPropertiesCount = numValues;
        }

        

        internal static void ParseCompositeAllBorders(BufferString value, FormatConverter formatConverter, PropertyId groupPropertyId, Property[] outputProperties, out int parsedPropertiesCount)
        {
            InternalDebug.Assert(outputProperties.Length >= 12);

            
            ParseCompositeBorder(value, formatConverter, PropertyId.TopBorderWidth, outputProperties, out parsedPropertiesCount);

            InternalDebug.Assert(parsedPropertiesCount <= 3);

            

            for (var i = 0; i < parsedPropertiesCount; i++) 
            {
                for (var j = 1; j < 4; j++) 
                {
                    outputProperties[parsedPropertiesCount * j + i].Set((PropertyId)((int)outputProperties[i].Id + j), outputProperties[i].Value);
                }
            }

            parsedPropertiesCount += parsedPropertiesCount * 3;
        }

        

        internal static void ParseCompositeBackground(BufferString value, FormatConverter formatConverter, PropertyId groupPropertyId, Property[] outputProperties, out int parsedPropertiesCount)
        {
            InternalDebug.Assert(outputProperties.Length >= 1);

            
            

            var word = BufferString.Null;

            ScanSkipWhitespace(ref value);

            var numValues = 0;

            if (!value.IsEmpty)
            {
                word = ScanNextNonWhitespaceToken(ref value);
                outputProperties[numValues].Set(PropertyId.BackColor, ParseColorCss(word, formatConverter));
                if (!outputProperties[numValues].Value.IsNull)
                {
                    numValues++;
                }

                ScanSkipWhitespace(ref value);
            }

            

            parsedPropertiesCount = numValues;
        }

        
        

        private enum TextDecoration
        {
            None,
            Underline,
            Overline,
            LineThrough,
            Blink,
        }

        private static EnumerationDef[] CssTextDecorationEnumeration =
        {
            new EnumerationDef("underline",new PropertyValue(TextDecoration.Underline)),
            new EnumerationDef("overline", new PropertyValue(TextDecoration.Overline)),
            new EnumerationDef("line-through", new PropertyValue(TextDecoration.LineThrough)),
            new EnumerationDef("blink", new PropertyValue(TextDecoration.Blink)),
            new EnumerationDef("none", new PropertyValue(TextDecoration.None)),
        };

        internal static void ParseCssTextDecoration(BufferString value, FormatConverter formatConverter, PropertyId groupPropertyId, Property[] outputProperties, out int parsedPropertiesCount)
        {
            InternalDebug.Assert(outputProperties.Length >= 3);

            var word = BufferString.Null;

            ScanSkipWhitespace(ref value);

            var numValues = 0;

            

            word = ScanNextNonWhitespaceToken(ref value);

            var val = ParseEnum(word, CssTextDecorationEnumeration);
            if (!val.IsNull)
            {
                switch ((TextDecoration)val.Enum)
                {
                    case TextDecoration.None:
                    case TextDecoration.Overline:
                    case TextDecoration.Blink:

                        outputProperties[numValues++].Set(PropertyId.Underline, PropertyValue.False);
                        outputProperties[numValues++].Set(PropertyId.Strikethrough, PropertyValue.False);
                        break;

                    case TextDecoration.Underline:

                        outputProperties[numValues++].Set(PropertyId.Underline, PropertyValue.True);
                        outputProperties[numValues++].Set(PropertyId.Strikethrough, PropertyValue.False);
                        break;

                    case TextDecoration.LineThrough:

                        outputProperties[numValues++].Set(PropertyId.Underline, PropertyValue.False);
                        outputProperties[numValues++].Set(PropertyId.Strikethrough, PropertyValue.True);
                        break;
                }
            }

            parsedPropertiesCount = numValues;
        }

        
        

        private enum TextTransform
        {
            Capitalize,
            Uppercase,
            Lowercase,
            None,
        }

        private static EnumerationDef[] CssTextTransformEnumeration =
        {
            new EnumerationDef("capitalize", new PropertyValue(TextTransform.Capitalize)),
            new EnumerationDef("uppercase", new PropertyValue(TextTransform.Uppercase)),
            new EnumerationDef("lowercase", new PropertyValue(TextTransform.Lowercase)),
            new EnumerationDef("none", new PropertyValue(TextTransform.None)),
        };

        internal static void ParseCssTextTransform(BufferString value, FormatConverter formatConverter, PropertyId groupPropertyId, Property[] outputProperties, out int parsedPropertiesCount)
        {
            InternalDebug.Assert(outputProperties.Length >= 3);

            var word = BufferString.Null;

            ScanSkipWhitespace(ref value);

            var numValues = 0;

            word = ScanNextNonWhitespaceToken(ref value);

            
            

            var val = ParseEnum(word, CssTextTransformEnumeration);
            if (!val.IsNull)
            {
                switch ((TextTransform)val.Enum)
                {
                    case TextTransform.None:
                    case TextTransform.Uppercase:
                    case TextTransform.Lowercase:

                        outputProperties[numValues++].Set(PropertyId.Capitalize, PropertyValue.False);
                        break;

                    case TextTransform.Capitalize:

                        outputProperties[numValues++].Set(PropertyId.Capitalize, PropertyValue.True);
                        break;
                }
            }

            parsedPropertiesCount = numValues;
        }

        
        

        private static EnumerationDef[] CssVerticalAlignmentEnumeration =
        {
            new EnumerationDef("baseline", new PropertyValue(Align.BaseLine)),
            new EnumerationDef("sub", new PropertyValue(Align.Sub)),
            new EnumerationDef("super", new PropertyValue(Align.Super)),
            new EnumerationDef("top", new PropertyValue(Align.Top)),
            new EnumerationDef("text-top", new PropertyValue(Align.TextTop)),
            new EnumerationDef("middle", new PropertyValue(Align.Middle)),
            new EnumerationDef("bottom", new PropertyValue(Align.Bottom)),
            new EnumerationDef("text-bottom", new PropertyValue(Align.TextBottom)),
        };

        internal static void ParseCssVerticalAlignment(BufferString value, FormatConverter formatConverter, PropertyId groupPropertyId, Property[] outputProperties, out int parsedPropertiesCount)
        {
            InternalDebug.Assert(outputProperties.Length >= 3);

            var word = BufferString.Null;

            ScanSkipWhitespace(ref value);

            var numValues = 0;

            word = ScanNextNonWhitespaceToken(ref value);

            
            

            var val = ParseEnum(word, CssVerticalAlignmentEnumeration);
            if (!val.IsNull)
            {
                switch ((Align)val.Enum)
                {
                    case Align.TextTop:
                    case Align.TextBottom:

                        break;

                    case Align.Top:
                    case Align.Middle:
                    case Align.Bottom:
                    case Align.BaseLine:

                        outputProperties[numValues++].Set(PropertyId.VerticalAlignment, val);
                        break;

                    case Align.Sub:

                        outputProperties[numValues++].Set(PropertyId.Subscript, PropertyValue.True);
                        outputProperties[numValues++].Set(PropertyId.Superscript, PropertyValue.False);
                        break;

                    case Align.Super:

                        outputProperties[numValues++].Set(PropertyId.Superscript, PropertyValue.True);
                        outputProperties[numValues++].Set(PropertyId.Subscript, PropertyValue.False);
                        break;
                }
            }
            else
            {
                
            }

            parsedPropertiesCount = numValues;
        }

        
        

        private enum CssWhiteSpace
        {
            Normal,
            Pre,
            Nowrap,
            PreWrap,
            PreLine,
        }

        private static EnumerationDef[] CssWhiteSpaceEnumeration =
        {
            new EnumerationDef("normal", new PropertyValue(CssWhiteSpace.Normal)),
            new EnumerationDef("pre", new PropertyValue(CssWhiteSpace.Pre)),
            new EnumerationDef("nowrap", new PropertyValue(CssWhiteSpace.Nowrap)),
            new EnumerationDef("pre-wrap", new PropertyValue(CssWhiteSpace.PreWrap)),
            new EnumerationDef("pre-line", new PropertyValue(CssWhiteSpace.PreLine)),
        };

        internal static void ParseCssWhiteSpace(BufferString value, FormatConverter formatConverter, PropertyId groupPropertyId, Property[] outputProperties, out int parsedPropertiesCount)
        {
            InternalDebug.Assert(outputProperties.Length >= 3);

            var word = BufferString.Null;

            ScanSkipWhitespace(ref value);

            var numValues = 0;

            word = ScanNextNonWhitespaceToken(ref value);

            
            

            var val = ParseEnum(word, CssWhiteSpaceEnumeration);
            if (!val.IsNull)
            {
                switch ((CssWhiteSpace)val.Enum)
                {
                    case CssWhiteSpace.Normal:

                        break;

                    case CssWhiteSpace.Pre:
                    case CssWhiteSpace.PreWrap:
                    case CssWhiteSpace.PreLine:
                    case CssWhiteSpace.Nowrap:

                        outputProperties[numValues++].Set(PropertyId.Preformatted, PropertyValue.True);
                        break;
                }
            }

            parsedPropertiesCount = numValues;
        }

        

        internal static void ParseCompositeFont(BufferString value, FormatConverter formatConverter, PropertyId groupPropertyId, Property[] outputProperties, out int parsedPropertiesCount)
        {
            InternalDebug.Assert(outputProperties.Length >= 6);

            var word = BufferString.Null;

            ScanSkipWhitespace(ref value);

            var numValues = 0;

            var weightIndex = -1;
            var variantIndex = -1;
            var styleIndex = -1;

            while (!value.IsEmpty)
            {
                word = ScanNextNonWhitespaceToken(ref value);

                
                var val = ParseFontWeight(word, formatConverter);

                

                if (val.IsNull)
                {
                    
                    val = ParseFontStyle(word, formatConverter);

                    if (val.IsNull)
                    {
                        
                        val = ParseFontVariant(word, formatConverter);

                        if (val.IsNull)
                        {
                            
                            break;
                        }
                        else
                        {
                            

                            if (variantIndex == -1)
                            {
                                variantIndex = numValues;
                                outputProperties[numValues++].Set(PropertyId.SmallCaps, val);
                            }
                            else
                            {
                                outputProperties[variantIndex].Set(PropertyId.SmallCaps, val);
                            }
                        }
                    }
                    else
                    {
                        

                        if (styleIndex == -1)
                        {
                            styleIndex = numValues;
                            outputProperties[numValues++].Set(PropertyId.Italic, val);
                        }
                        else
                        {
                            outputProperties[styleIndex].Set(PropertyId.Italic, val);
                        }
                    }
                }
                else
                {
                    
                    if (weightIndex == -1)
                    {
                        weightIndex = numValues;
                        outputProperties[numValues++].Set(PropertyId.Bold, val);
                    }
                    else
                    {
                        outputProperties[weightIndex].Set(PropertyId.Bold, val);
                    }
                }

                
                word = BufferString.Null;

                ScanSkipWhitespace(ref value);
            }

            if (!word.IsEmpty)
            {
                
                ScanRevertLastToken(ref value, word);

                word = ScanNextSize(ref value);

                outputProperties[numValues].Set(PropertyId.FontSize, ParseCssFontSize(word, formatConverter));
                if (!outputProperties[numValues].Value.IsNull)
                {
                    numValues ++;
                }
            }

            ScanSkipWhitespace(ref value);

            if (!value.IsEmpty && value[0] == '/')
            {
                value.Trim(1, value.Length - 1);    

                ScanSkipWhitespace(ref value);

                word = ScanNextSize(ref value);

                var lineHeight = ParseNonNegativeLength(word, formatConverter);  

                

                ScanSkipWhitespace(ref value);
            }

            if (!value.IsEmpty)
            {
                outputProperties[numValues].Set(PropertyId.FontSize, ParseFontFace(value, formatConverter));
                if (!outputProperties[numValues].Value.IsNull)
                {
                    numValues ++;
                }
            }

            parsedPropertiesCount = numValues;
        }
    }
}

