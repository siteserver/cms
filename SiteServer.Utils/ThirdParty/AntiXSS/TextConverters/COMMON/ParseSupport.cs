// ***************************************************************
// <copyright file="ParseSupport.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters
{
    using System;
    using Data.Internal;

    

    [Flags]
    internal enum CharClass : uint
    {
        
        Invalid = 0,    
        NotInterestingText = 0x00000001,
        Control = 0x00000002,
        Whitespace = 0x00000004,
        Alpha = 0x00000008,
        Numeric = 0x00000010,
        Backslash = 0x00000020,
        LessThan = 0x00000040,
        Equals = 0x00000080,
        GreaterThan = 0x00000100,
        Solidus = 0x00000200,
        Ampersand = 0x00000400,
        Nbsp = 0x00000800,
        Comma = 0x00001000,
        SingleQuote = 0x00002000,
        DoubleQuote = 0x00004000,
        GraveAccent = 0x00008000,
        Circumflex = 0x00010000,
        VerticalLine = 0x00020000,
        Parentheses = 0x00040000,
        CurlyBrackets = 0x00080000,
        SquareBrackets = 0x00100000,
        Tilde = 0x00200000,
        Colon = 0x00400000,
        UniqueMask = 0x00FFFFFF,

        
        AlphaHex = 0x80000000,
        HtmlSuffix = 0x40000000,
        RtfInteresting = 0x20000000,
        OverlappedMask = 0xFF000000,

        
        Quote = SingleQuote | DoubleQuote | GraveAccent,
        Brackets = CurlyBrackets | SquareBrackets,
        NonWhitespaceText = UniqueMask & ~(Whitespace | Nbsp),
        NonWhitespaceNonControlText = NonWhitespaceText & ~Control,
        HtmlNonWhitespaceText = NonWhitespaceText & ~(LessThan | Ampersand),
        NonWhitespaceNonUri = LessThan | GreaterThan | Brackets | DoubleQuote | GraveAccent | Ampersand | Circumflex | VerticalLine | Tilde,
        NonWhitespaceUri = UniqueMask & ~(Whitespace | Nbsp | NonWhitespaceNonUri),
        
        HtmlTagName = UniqueMask & ~(Whitespace | GreaterThan | Solidus),
        HtmlTagNamePrefix = UniqueMask & ~(Whitespace | GreaterThan | Solidus | Colon),
        HtmlAttrName = UniqueMask & ~(Whitespace | GreaterThan | Solidus | Equals),
        HtmlAttrNamePrefix = UniqueMask & ~(Whitespace | GreaterThan | Solidus | Equals | Colon),
        HtmlAttrValue = UniqueMask & ~(Whitespace | GreaterThan | Quote | Ampersand),
        HtmlScanQuoteSensitive = Whitespace | Equals,
        HtmlEntity = Alpha | Numeric,

        HtmlSimpleTagName = UniqueMask & ~(Whitespace | GreaterThan | Solidus | Quote | LessThan | Colon),
        HtmlEndTagName = Whitespace | GreaterThan | Solidus,
        HtmlSimpleAttrName = UniqueMask & ~(Whitespace | GreaterThan | Solidus | Equals | Quote | Colon),
        HtmlEndAttrName = Whitespace | GreaterThan | Solidus | Equals,
        HtmlSimpleAttrQuotedValue = UniqueMask & ~(Ampersand | Quote),
        HtmlSimpleAttrUnquotedValue = UniqueMask & ~(Whitespace | GreaterThan | Quote | Ampersand),
        HtmlEndAttrUnquotedValue = Whitespace | GreaterThan,

        Hex = Numeric | AlphaHex,
    }

    [Flags]
    internal enum DbcsLeadBits : byte
    {
        Lead1361 = 0x01,
        Lead10001 = 0x02,
        Lead10002 = 0x04,
        Lead10003 = 0x08,
        Lead10008 = 0x10,
        Lead932 = 0x20,
        Lead9XX = 0x40,
    }

    

    internal static class ParseSupport
    {
        
        
        
        
        
        
        
        
        private static readonly char[] latin1MappingInUnicodeControlArea =
        {
            (char) 0x20ac,  
            (char) 0x0081,  
            (char) 0x201a,  
            (char) 0x0192,  
            (char) 0x201e,  
            (char) 0x2026,  
            (char) 0x2020,  
            (char) 0x2021,  
            (char) 0x02c6,  
            (char) 0x2030,  
            (char) 0x0160,  
            (char) 0x2039,  
            (char) 0x0152,  
            (char) 0x008d,  
            (char) 0x017d,  
            (char) 0x008f,  
            (char) 0x0090,  
            (char) 0x2018,  
            (char) 0x2019,  
            (char) 0x201c,  
            (char) 0x201d,  
            (char) 0x2022,  
            (char) 0x2013,  
            (char) 0x2014,  
            (char) 0x02dc,  
            (char) 0x2122,  
            (char) 0x0161,  
            (char) 0x203a,  
            (char) 0x0153,  
            (char) 0x009d,  
            (char) 0x017e,  
            (char) 0x0178   
        };

        
        private static readonly DbcsLeadBits[] dbcsLeadTable = 
        {
            0,                                                                                                                                      
            DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,                                                      
            DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,                                                      
            DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,                                                      
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,                                 
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,                                 
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,                                 
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,                                 
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,                                 
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,                                 
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,                                 
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,                                 
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,                                 
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,                                 
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,                                 
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,                                 
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,                                 
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,                                 
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,                                 
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,                                 
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,                                 
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,                                 
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,                                 
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,                                 
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,                                 
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,                                 
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,                                 
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,                                 
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,                                 
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,                                 
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,                                 
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,                                 

            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead9XX,                                                                           
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead9XX,                                                     
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead9XX,                                                     
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead9XX,                                                     
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead9XX,                                                                           
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead9XX,                                                                           
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead9XX,                                                                           
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               

            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                                                     
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                                                    
            DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                                                    
            DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                                                    
            DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                                                    
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                               
            DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,                                                    

            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,       
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,       
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,       
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,       
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,       
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,       
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,       
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,       
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,       
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,       
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,       
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,       
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,       
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,       
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,       
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,       
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,       
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,       
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,       
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,       
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,       
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,       
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,       
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,       
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,           
            DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,           
            DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,                                
            DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,                                
            DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,                                
            DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead9XX,                                                                                                
            DbcsLeadBits.Lead9XX,                                                                                                                      
            0,                                                                                                                                      
        };

        
        private static readonly byte[] charToHexTable = 
        {
            0xFF, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,     
            0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,     
        };

        
        private static readonly byte[] octetBitsCount = 
        {
        
            0, 1, 1, 2, 1, 2, 2, 3, 1, 2, 2, 3, 2, 3, 3, 4
        };

        
        private static readonly CharClass[] lowCharClass =
        {
            CharClass.Invalid | CharClass.RtfInteresting,                           
            CharClass.Control,                                                      
            CharClass.Control,                                                      
            CharClass.Control,                                                      
            CharClass.Control,                                                      
            CharClass.Control,                                                      
            CharClass.Control,                                                      
            CharClass.Control,                                                      
            CharClass.Control,                                                      
            CharClass.Whitespace | CharClass.RtfInteresting,                        
            CharClass.Whitespace | CharClass.RtfInteresting,                        
            CharClass.Whitespace,                                                   
            CharClass.Whitespace,                                                   
            CharClass.Whitespace | CharClass.RtfInteresting,                        
            CharClass.Control,                                                      
            CharClass.Control,                                                      
            CharClass.Control,                                                      
            CharClass.Control,                                                      
            CharClass.Control,                                                      
            CharClass.Control,                                                      
            CharClass.Control,                                                      
            CharClass.Control,                                                      
            CharClass.Control,                                                      
            CharClass.Control,                                                      
            CharClass.Control,                                                      
            CharClass.Control,                                                      
            CharClass.Control,                                                      
            CharClass.Control,                                                      
            CharClass.Control,                                                      
            CharClass.Control,                                                      
            CharClass.Control,                                                      
            CharClass.Control,                                                      
            CharClass.Whitespace,                                                   
            CharClass.NotInterestingText,                                           
            CharClass.DoubleQuote,                                                  
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText | CharClass.HtmlSuffix,                    
            CharClass.Ampersand,                                                    
            CharClass.SingleQuote,                                                  
            CharClass.Parentheses,                                                  
            CharClass.Parentheses,                                                  
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.Comma,                                                        
            CharClass.NotInterestingText | CharClass.HtmlSuffix,                    
            CharClass.NotInterestingText,                                           
            CharClass.Solidus,                                                      
            CharClass.Numeric,                                                      
            CharClass.Numeric,                                                      
            CharClass.Numeric,                                                      
            CharClass.Numeric,                                                      
            CharClass.Numeric,                                                      
            CharClass.Numeric,                                                      
            CharClass.Numeric,                                                      
            CharClass.Numeric,                                                      
            CharClass.Numeric,                                                      
            CharClass.Numeric,                                                      
            CharClass.Colon,                                                        
            CharClass.NotInterestingText,                                           
            CharClass.LessThan,                                                     
            CharClass.Equals,                                                       
            CharClass.GreaterThan | CharClass.HtmlSuffix,                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.Alpha | CharClass.AlphaHex,                                   
            CharClass.Alpha | CharClass.AlphaHex,                                   
            CharClass.Alpha | CharClass.AlphaHex,                                   
            CharClass.Alpha | CharClass.AlphaHex,                                   
            CharClass.Alpha | CharClass.AlphaHex,                                   
            CharClass.Alpha | CharClass.AlphaHex,                                   
            CharClass.Alpha,                                                        
            CharClass.Alpha,                                                        
            CharClass.Alpha,                                                        
            CharClass.Alpha,                                                        
            CharClass.Alpha,                                                        
            CharClass.Alpha,                                                        
            CharClass.Alpha,                                                        
            CharClass.Alpha,                                                        
            CharClass.Alpha,                                                        
            CharClass.Alpha,                                                        
            CharClass.Alpha,                                                        
            CharClass.Alpha,                                                        
            CharClass.Alpha,                                                        
            CharClass.Alpha,                                                        
            CharClass.Alpha,                                                        
            CharClass.Alpha,                                                        
            CharClass.Alpha,                                                        
            CharClass.Alpha,                                                        
            CharClass.Alpha,                                                        
            CharClass.Alpha,                                                        
            CharClass.SquareBrackets,                                               
            CharClass.Backslash | CharClass.RtfInteresting,                         
            CharClass.SquareBrackets | CharClass.HtmlSuffix,                        
            CharClass.Circumflex,                                                   
            CharClass.NotInterestingText,                                           
            CharClass.GraveAccent,                                                  
            CharClass.Alpha | CharClass.AlphaHex,                                   
            CharClass.Alpha | CharClass.AlphaHex,                                   
            CharClass.Alpha | CharClass.AlphaHex,                                   
            CharClass.Alpha | CharClass.AlphaHex,                                   
            CharClass.Alpha | CharClass.AlphaHex,                                   
            CharClass.Alpha | CharClass.AlphaHex,                                   
            CharClass.Alpha,                                                        
            CharClass.Alpha,                                                        
            CharClass.Alpha,                                                        
            CharClass.Alpha,                                                        
            CharClass.Alpha,                                                        
            CharClass.Alpha,                                                        
            CharClass.Alpha,                                                        
            CharClass.Alpha,                                                        
            CharClass.Alpha,                                                        
            CharClass.Alpha,                                                        
            CharClass.Alpha,                                                        
            CharClass.Alpha,                                                        
            CharClass.Alpha,                                                        
            CharClass.Alpha,                                                        
            CharClass.Alpha,                                                        
            CharClass.Alpha,                                                        
            CharClass.Alpha,                                                        
            CharClass.Alpha,                                                        
            CharClass.Alpha,                                                        
            CharClass.Alpha,                                                        
            CharClass.CurlyBrackets | CharClass.RtfInteresting,                     
            CharClass.VerticalLine,                                                 
            CharClass.CurlyBrackets | CharClass.RtfInteresting,                     
            CharClass.Tilde,                                                        
            CharClass.Control,                                                      

            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           

            CharClass.Nbsp,                                                         
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
            CharClass.NotInterestingText,                                           
        };

        
        public static int CharToDecimal(char ch)
        {
            unchecked
            {
                InternalDebug.Assert(NumericCharacter(GetCharClass(ch)));
                return (int)(ch - '0');
            }
        }

        
        public static int CharToHex(char ch)
        {
            InternalDebug.Assert(HexCharacter(GetCharClass(ch)));
            return (int)charToHexTable[ch & 0x1F];
        }

        
        public static int BitCount(byte v)
        {
            return (int)octetBitsCount[v & 0x0F] + octetBitsCount[(v >> 4) & 0x0F];
        }

        
        public static int BitCount(short v)
        {
            return BitCount((byte)v) + BitCount((byte)(v >> 8));
        }

        
        public static int BitCount(int v)
        {
            return BitCount((short)v) + BitCount((short)(v >> 16));
        }

        
        public static char HighSurrogateCharFromUcs4(int ich)
        {
            return (char)(0xd800 + ((ich - 0x10000) >> 10));
        }

        
        public static char LowSurrogateCharFromUcs4(int ich)
        {
            return (char)(0xdc00 + (ich & 0x3ff));
        }

        
        public static int Ucs4FromSurrogatePair(char low, char high)
        {
            InternalDebug.Assert(IsHighSurrogate(high) && IsLowSurrogate(low));

            return (low & 0x3FF) + ((high & 0x3FF) << 10) + 0x10000;
        }

        
        public static bool IsHighSurrogate(char ch)
        {
            return ch >= (char)0xd800 && ch < (char)0xdc00;
        }

        
        public static bool IsLowSurrogate(char ch)
        {
            return ch >= (char)0xdc00 && ch < (char)0xde00;
        }

        
        public static bool IsCharClassOneOf(CharClass charClass, CharClass charClassSet)
        {
            return (charClass & charClassSet) != 0;
        }

        
        public static bool InvalidUnicodeCharacter(CharClass charClass)
        {
            return (charClass & CharClass.UniqueMask) == 0;
        }

        
        public static bool HtmlTextCharacter(CharClass charClass)
        {
            return (charClass & CharClass.HtmlNonWhitespaceText) != 0;
        }

        
        public static bool TextCharacter(CharClass charClass)
        {
            return (charClass & CharClass.NonWhitespaceText) != 0;
        }

        
        public static bool TextNonUriCharacter(CharClass charClass)
        {
            return (charClass & CharClass.NonWhitespaceNonUri) != 0;
        }

        
        public static bool TextUriCharacter(CharClass charClass)
        {
            return (charClass & CharClass.NonWhitespaceUri) != 0;
        }

        
        public static bool NonControlTextCharacter(CharClass charClass)
        {
            return (charClass & CharClass.NonWhitespaceNonControlText) != 0;
        }

        
        public static bool ControlCharacter(CharClass charClass)
        {
            return (charClass & CharClass.Control) != 0;
        }

        
        public static bool WhitespaceCharacter(CharClass charClass)
        {
            return (charClass & CharClass.Whitespace) != 0;
        }

        
        public static bool WhitespaceCharacter(char ch)
        {
            return (GetCharClass(ch) & CharClass.Whitespace) != 0;
        }

        
        public static bool NbspCharacter(CharClass charClass)
        {
            return (charClass & CharClass.Nbsp) != 0;
        }

        
        public static bool AlphaCharacter(CharClass charClass)
        {
            return (charClass & CharClass.Alpha) != 0;
        }

        
        public static bool AlphaCharacter(char ch)
        {
            return (GetCharClass(ch) & CharClass.Alpha) != 0;
        }

        
        public static bool QuoteCharacter(CharClass charClass)
        {
            return (charClass & CharClass.Quote) != 0;
        }

        
        public static bool HtmlTagNamePrefixCharacter(CharClass charClass)
        {
            return (charClass & CharClass.HtmlTagNamePrefix) != 0;
        }

        
        public static bool HtmlTagNameCharacter(CharClass charClass)
        {
            return (charClass & CharClass.HtmlTagName) != 0;
        }

        
        public static bool HtmlAttrNamePrefixCharacter(CharClass charClass)
        {
            return (charClass & CharClass.HtmlAttrNamePrefix) != 0;
        }

        
        public static bool HtmlAttrNameCharacter(CharClass charClass)
        {
            return (charClass & CharClass.HtmlAttrName) != 0;
        }

        
        public static bool HtmlAttrValueCharacter(CharClass charClass)
        {
            return (charClass & CharClass.HtmlAttrValue) != 0;
        }

        
        public static bool HtmlScanQuoteSensitiveCharacter(CharClass charClass)
        {
            return (charClass & CharClass.HtmlScanQuoteSensitive) != 0;
        }

        
        public static bool HtmlSimpleTagNameCharacter(CharClass charClass)
        {
            return (charClass & CharClass.HtmlSimpleTagName) != 0;
        }

        
        public static bool HtmlEndTagNameCharacter(CharClass charClass)
        {
            return (charClass & CharClass.HtmlEndTagName) != 0;
        }

        
        public static bool HtmlSimpleAttrNameCharacter(CharClass charClass)
        {
            return (charClass & CharClass.HtmlSimpleAttrName) != 0;
        }

        
        public static bool HtmlEndAttrNameCharacter(CharClass charClass)
        {
            return (charClass & CharClass.HtmlEndAttrName) != 0;
        }

        
        public static bool HtmlSimpleAttrQuotedValueCharacter(CharClass charClass)
        {
            return (charClass & CharClass.HtmlSimpleAttrQuotedValue) != 0;
        }

        
        public static bool HtmlSimpleAttrUnquotedValueCharacter(CharClass charClass)
        {
            return (charClass & CharClass.HtmlSimpleAttrUnquotedValue) != 0;
        }

        
        public static bool HtmlEndAttrUnquotedValueCharacter(CharClass charClass)
        {
            return (charClass & CharClass.HtmlEndAttrUnquotedValue) != 0;
        }

        
        public static bool NumericCharacter(CharClass charClass)
        {
            return (charClass & CharClass.Numeric) != 0;
        }

        
        public static bool NumericCharacter(char ch)
        {
            return (GetCharClass(ch) & CharClass.Numeric) != 0;
        }

        
        public static bool HexCharacter(CharClass charClass)
        {
            return (charClass & CharClass.Hex) != 0;
        }

        
        public static bool HtmlEntityCharacter(CharClass charClass)
        {
            return (charClass & CharClass.HtmlEntity) != 0;
        }

        
        public static bool HtmlSuffixCharacter(CharClass charClass)
        {
            return (charClass & CharClass.HtmlSuffix) != 0;
        }

        
        public static bool RtfInterestingCharacter(CharClass charClass)
        {
            return (charClass & CharClass.RtfInteresting) != 0;
        }

        
        public static CharClass GetCharClass(byte ch)
        {
            return lowCharClass[ch];
        }

        
        public static CharClass GetCharClass(char ch)
        {
            return (int)ch <= 0xFF/*lowCharClass.Length*/ ? lowCharClass[(int)ch] : GetHighCharClass(ch);
        }

        
        public static CharClass GetHighCharClass(char ch)
        {
            return ch < (char)0xFDD0 ? CharClass.NotInterestingText : 
                    (!((char)0xFFF9 <= ch && ch <= (char)0xFFFD) && !((char)0xFDF0 <= ch && ch <= (char)0xFFEF))
                        ? CharClass.Invalid : CharClass.NotInterestingText;
        }

        
        
        public static bool IsLeadByte(byte bt, DbcsLeadBits codePageMask)
        {
            return codePageMask != 0 && IsLeadByteEx(bt, codePageMask);
        }

        
        private static bool IsLeadByteEx(byte bt, DbcsLeadBits codePageMask)
        {
            return bt >= 0x80 && (dbcsLeadTable[bt - 0x80] & codePageMask) != 0;
        }

        
        public static DbcsLeadBits GetCodePageLeadMask(int codePage)
        {
            DbcsLeadBits leadMask = 0;

            if (codePage >= 1361)                               
            {
                if (codePage == 1361)                               
                {
                    leadMask = DbcsLeadBits.Lead1361;
                }
                else if (codePage == 10001)                         
                {
                    leadMask = DbcsLeadBits.Lead10001;
                }
                else if (codePage == 10002)                         
                {
                    leadMask = DbcsLeadBits.Lead10002;
                }
                else if (codePage == 10003)                         
                {
                    leadMask = DbcsLeadBits.Lead10003;
                }
                else if (codePage == 10008)                         
                {
                    leadMask = DbcsLeadBits.Lead10008;
                }
            }
            else if (codePage <= 950)
            {
                if (codePage == 950 || codePage == 949 || codePage == 936)
                {                                                   
                    leadMask = DbcsLeadBits.Lead9XX;
                }
                else if (codePage == 932)                           
                {
                    leadMask = DbcsLeadBits.Lead932;
                }
            }

            return leadMask;
        }

        
        public static bool IsUpperCase(char ch)
        {
            return (uint)(ch - 'A') <= ('Z' - 'A');
        }

        
        public static bool IsLowerCase(char ch)
        {
            return (uint)(ch - 'a') <= ('z' - 'a');
        }

        
        public static char ToLowerCase(char ch)
        {
            return IsUpperCase(ch) ? (char)(ch + ('a' - 'A')) : ch;
        }

        
        public static int Latin1MappingInUnicodeControlArea(int value)
        {
            InternalDebug.Assert(0x80 <= value && value <= 0x9F);
            return (int)latin1MappingInUnicodeControlArea[value - 0x80];
        }

        
        
        
        
        
        
        public static bool TwoFarEastNonHanguelChars(char ch1, char ch2)
        {
            if (ch1 < (char)0x3000 || ch2 < (char)0x3000)
            {
                return false;
            }

            return !HanguelRange(ch1) && !HanguelRange(ch2);
        }

        
        public static bool FarEastNonHanguelChar(char ch)
        {
            return ch >= (char)0x3000 && !HanguelRange(ch);
        }

        
        private static bool HanguelRange(char ch)
        {
            
            InternalDebug.Assert(ch >= (char)0x3000);
            
            return /*ch > 0x10ff &&*/
                (/*(0x1100 <= ch && ch <= 0x11f9) ||*/
                (0x3130 <= ch && ch <= 0x318f) ||
                (0xac00 <= ch && ch <= 0xd7a3) ||
                (0xffa1 <= ch && ch <= 0xffdc));
        }

        
        public static string TrimWhitespace(string value)
        {
            var result = value;

            if (!string.IsNullOrEmpty(value))
            {
                var start = 0;
                var end = value.Length;
                if (WhitespaceCharacter(GetCharClass(value[0])))
                {
                    for (start = 1; start < end && WhitespaceCharacter(GetCharClass(value[start])); start++)
                    {
                    }
                }

                if (start != end)
                {
                    if (WhitespaceCharacter(GetCharClass(value[end - 1])))
                    {
                        for (end = end - 1; WhitespaceCharacter(GetCharClass(value[end - 1])); end--)
                        {
                        }
                    }

                    if (end - start != value.Length)
                    {
                        result = value.Substring(start, end - start);
                    }
                }
                else
                {
                    result = String.Empty;
                }
            }

            return result;
        }
    }
}

