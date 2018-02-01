// ***************************************************************
// <copyright file="FormatProperty.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters.Internal.Format
{
    using System;
    using System.Runtime.InteropServices;
    using Data.Internal;

    

    internal enum PropertyId : byte
    {
        Null = 0,

        
        

        FirstFlag = 1,
        Bold = FirstFlag,       
        Italic,                 
        Underline,              
        Subscript,              
        Superscript,            
        Strikethrough,          
        SmallCaps,              
        Capitalize,             
        RightToLeft,            
        Preformatted,           
        NoBreak,                
        Visible,                
        Overloaded1,            
        Overloaded2,            
        Overloaded3,            
        MergedCell,             

        

        TableLayoutFixed = Overloaded1,     
        SelectMultiple = Overloaded1,       
        OptionSelected = Overloaded1,       
        ReadOnly = Overloaded1,             

        TableBorderCollapse = Overloaded2,  
        Disabled = Overloaded2,             

        Checked = Overloaded3,              

        LastFlag = 16,

        

        FontColor,              
        FontSize,               
        FontFace,               

        TextAlignment,          
        FirstLineIndent,        

        BlockAlignment,         
        HorizontalAlignment = BlockAlignment, 
        VerticalAlignment = BlockAlignment, 

        BackColor,              

        Display,                

        Language,               
        UnicodeBiDi,            

        Width,                  
        Height,                 

        
        Margins,                
        TopMargin = Margins,    
        RightMargin,            
        BottomMargin,           
        LeftMargin,             

        
        Paddings,               
        TopPadding = Paddings,  
        RightPadding,           
        BottomPadding,          
        LeftPadding,            

        
        BorderWidths,           
        TopBorderWidth = BorderWidths, 
        RightBorderWidth,       
        BottomBorderWidth,      
        LeftBorderWidth,        

        
        BorderStyles,           
        TopBorderStyle = BorderStyles, 
        RightBorderStyle,       
        BottomBorderStyle,      
        LeftBorderStyle,        

        
        BorderColors,           
        TopBorderColor = BorderColors, 
        RightBorderColor,       
        BottomBorderColor,      
        LeftBorderColor,        

        
        ListLevel,              
        ListStyle,              
        ListStart,              

        
        NumColumns,             
        NumRows,                

        TableShowEmptyCells,    
        TableCaptionSideTop,    
        TableCellNoWrap,        

        TableBorderSpacingVertical, 
        TableBorderSpacingHorizontal, 

        TableBorder,            

        TableFrame,             
        TableRules,             
        TableCellSpacing,       
        TableCellPadding,       

        BookmarkName,           

        HyperlinkUrl,           
        HyperlinkTarget,        

        ImageUrl,               
        ImageAltText,           

        QuotingLevelDelta,      

        MaxValue,    

        

        ImageBorder = TableBorder,
        IFrameUrl = ImageUrl,

        FormAction = HyperlinkUrl,
        FormMethod = HyperlinkTarget,
        FormEncodingType = ImageUrl,
        FormAcceptContentTypes = ImageAltText,
        FormAcceptCharsets = QuotingLevelDelta,

        Label = ImageAltText,

        OptionValue = QuotingLevelDelta,

        InputValue = QuotingLevelDelta,
        InputType = TableBorder,
        InputSize = TableFrame,
        InputMaxLength = TableRules,
        
        

        ButtonType = InputType,

        Prompt = ImageAltText,
    }

    internal enum Side
    {
        
        Top,
        Right,
        Bottom,
        Left,
    }

    

    internal enum PropertyType : byte
    {
        Null,                       

        Calculated,                 

        Bool,                       

        String,                     
        MultiValue,                 

        Enum,                       

        Color,                      
                                    

        Integer,                    
        Fractional,                 

        Percentage,                 

        AbsLength,                  
                                    
        RelLength,                  
                                    
                                    
        Pixels,                     
                                    
                                    
                                    
                                    
        Ems,                        
        Exs,                        

        HtmlFontUnits,              
        RelHtmlFontUnits,           

        Multiple,                   

        Milliseconds,               
        kHz,                        
        Degrees,                    

        FirstLength = AbsLength,
        LastLength = Multiple,
    }

    

    enum LengthUnits : byte
    {
        
        BaseUnits,                  
        Twips,                      
        Points,                     
        Picas,                      
        Inches,                     
        Centimeters,                
        Millimeters,                

        HtmlFontUnits,              

        
        Pixels,                     
                                    
                                    
        Ems,                        
        Exs,                        

        RelativeHtmlFontUnits,      

        Percents,                   
        Multiple,                   
    }

    

    internal struct RGBT
    {
        private uint        rawValue;

        public RGBT(uint rawValue)
        {
            InternalDebug.Assert(rawValue <= 0x07FFFFFF);
            this.rawValue = rawValue;
        }

        public RGBT(uint red, uint green, uint blue)
        {
            InternalDebug.Assert(red <= 255 && green <= 255 && blue <= 255);
            rawValue = (red << 16) | (green << 8) | blue;
        }

        public RGBT(float redPercentage, float greenPercentage, float bluePercentage)
        {
            InternalDebug.Assert(redPercentage >= 0.0f && greenPercentage >= 0.0f && bluePercentage >= 0.0f);
            InternalDebug.Assert(redPercentage <= 100.0f && 
                            greenPercentage <= 100.0 && 
                            bluePercentage <= 100.0f);
            rawValue = ((uint)(redPercentage*255.0f/100.0f) << 16) | 
                            ((uint)(greenPercentage*255.0f/100.0f) << 8) | 
                                (uint)(bluePercentage*255.0f/100.0f);
        }

        public RGBT(uint red, uint green, uint blue, uint transparency)
        {
            InternalDebug.Assert(red <= 255 && green <= 255 && blue <= 255 && transparency <= 7);
            rawValue = (red << 16) | (green << 8) | blue | (transparency << 24);
        }

        public RGBT(float redPercentage, float greenPercentage, float bluePercentage, float transparencyPercentage)
        {
            InternalDebug.Assert(redPercentage >= 0.0f && greenPercentage >= 0.0f && bluePercentage >= 0.0f && transparencyPercentage >= 0.0f);
            InternalDebug.Assert(redPercentage <= 100.0f && 
                            greenPercentage <= 100.0 && 
                            bluePercentage <= 100.0f && 
                            transparencyPercentage <= 100.0f);
            rawValue = ((uint)(redPercentage*255.0f/100.0f) << 16) | 
                            ((uint)(greenPercentage*255.0f/100.0f) << 8) | 
                                (uint)(bluePercentage*255.0f/100.0f) |
                                (uint)(transparencyPercentage*7.0f/100.0f);
        }

        public uint     RawValue => rawValue;
        public uint     RGB => rawValue & 0xFFFFFF;

        public bool     IsTransparent => Transparency == 7;
        public bool     IsOpaque => Transparency == 0;

        public uint     Transparency => (rawValue >> 24) & 0x07;
        public uint     Red => (rawValue >> 16) & 0xFF;
        public uint     Green => (rawValue >> 8) & 0xFF;
        public uint     Blue => rawValue & 0xFF;

        public float    RedPercentage => (float)((rawValue >> 16) & 0xFF) * (100.0f / 255.0f);
        public float    GreenPercentage => (float)((rawValue >> 8) & 0xFF) * (100.0f / 255.0f);
        public float    BluePercentage => (float)(rawValue & 0xFF) * (100.0f / 255.0f);
        public float    TransparencyPercentage => (float)((rawValue >> 24) & 0x07) * (100.0f / 7.0f);

        public override string ToString()
        {
            return IsTransparent ? "transparent" :
                "rgb(" + Red.ToString() + ", " + Green.ToString() + ", " + Blue.ToString() + ")" + (Transparency != 0 ? "+t" + Transparency.ToString() : "");
        }
    }


    

    internal struct PropertyValue
    {
        private const uint  TypeMask = 0xF8000000;
        private const int   TypeShift = 27;
        private const uint  ValueMask = 0x07FFFFFF;
        private const int   ValueShift = 5;

        public const int    ValueMax = 0x03FFFFFF;
        public const int    ValueMin = -ValueMax;

        private uint        rawValue;

        public static readonly PropertyValue Null = new PropertyValue();
        public static readonly PropertyValue True = new PropertyValue(true);
        public static readonly PropertyValue False = new PropertyValue(false);

        internal static readonly int[] sizesInTwips = { 151, 200, 240, 271, 360, 480, 720 };
        internal static readonly int[] maxSizesInTwips = { 160, 220, 260, 320, 420, 620 };

        

        public PropertyValue(uint rawValue)
        {
            this.rawValue = rawValue;
        }

        

        public PropertyValue(bool value)
        {
            rawValue = ComposeRawValue(value);
        }

        

        public PropertyValue(PropertyType type, int value)
        {
            rawValue = ComposeRawValue(type, value);
        }

        

        public PropertyValue(PropertyType type, uint value)
        {
            rawValue = ComposeRawValue(type, value);
        }

        
#if false
        public PropertyValue(StringHandle handle)
        {
            this.rawValue = ComposeRawValue(handle);
        }

        

        public PropertyValue(MultiValueHandle handle)
        {
            this.rawValue = ComposeRawValue(handle);
        }
#endif
        

        public PropertyValue(LengthUnits lengthUnits, float value)
        {
            rawValue = ComposeRawValue(lengthUnits, value);
        }

        

        public PropertyValue(LengthUnits lengthUnits, int value)
        {
            rawValue = ComposeRawValue(lengthUnits, value);
        }

        

        public PropertyValue(PropertyType type, float value)
        {
            rawValue = ComposeRawValue(type, value);
        }

        

        public PropertyValue(RGBT color)
        {
            rawValue = ComposeRawValue(color);
        }

        public PropertyValue(Enum value)
        {
            rawValue = ComposeRawValue(value);
        }

        

        public void Set(uint rawValue)
        {
            this.rawValue = rawValue;
        }

        

        public void Set(bool value)
        {
            rawValue = ComposeRawValue(value);
        }

        

        public void Set(PropertyType type, int value)
        {
            rawValue = ComposeRawValue(type, value);
        }

        

        public void Set(PropertyType type, uint value)
        {
            rawValue = ComposeRawValue(type, value);
        }

        
#if false
        public void Set(StringHandle handle)
        {
            this.rawValue = ComposeRawValue(handle);
        }

        

        public void Set(MultiValueHandle handle)
        {
            this.rawValue = ComposeRawValue(handle);
        }
#endif
        

        public void Set(LengthUnits lengthUnits, float value)
        {
            rawValue = ComposeRawValue(lengthUnits, value);
        }

        

        public void Set(PropertyType type, float value)
        {
            rawValue = ComposeRawValue(type, value);
        }

        

        public void Set(RGBT color)
        {
            rawValue = ComposeRawValue(color);
        }

        public void Set(Enum value)
        {
            rawValue = ComposeRawValue(value);
        }


        

        private static uint ComposeRawValue(bool value)
        {
            return (GetRawType(PropertyType.Bool) | (value ? 1u : 0u));
        }

        

        private static uint ComposeRawValue(PropertyType type, int value)
        {
            InternalDebug.Assert(ValueMin <= value && value <= ValueMax);
            return (((uint)type<<TypeShift) | ((uint)value & ValueMask));
        }

        

        private static uint ComposeRawValue(PropertyType type, uint value)
        {
            InternalDebug.Assert(value <= ValueMask);
            return (((uint)type<<TypeShift) | value);
        }

        
#if false
        private static uint ComposeRawValue(StringHandle handle)
        {
            InternalDebug.Assert(handle.Handle < ValueMask);
            return (GetRawType(PropertyType.String) | handle.Handle);
        }

        

        private static uint ComposeRawValue(MultiValueHandle handle)
        {
            InternalDebug.Assert(handle.Handle < ValueMask);
            return (GetRawType(PropertyType.MultiValue) | handle.Handle);
        }
#endif
        

        private static uint ComposeRawValue(LengthUnits lengthUnits, float len)
        {
            switch (lengthUnits)
            {
                
                case LengthUnits.BaseUnits:                 
                        return (GetRawType(PropertyType.AbsLength) | 
                                        ((uint)len & ValueMask));
                case LengthUnits.Twips:                     
                        return (GetRawType(PropertyType.AbsLength) | 
                                        ((uint)(len * (8.0f)) & ValueMask));
                case LengthUnits.Points:                    
                        return (GetRawType(PropertyType.AbsLength) | 
                                        ((uint)(len * (8.0f * 20.0f)) & ValueMask));
                case LengthUnits.Picas:                     
                        return (GetRawType(PropertyType.AbsLength) | 
                                        ((uint)(len * (8.0f * 20.0f * 12.0f)) & ValueMask));
                case LengthUnits.Inches:                    
                        return (GetRawType(PropertyType.AbsLength) |
                                        ((uint)(len * (8.0f * 20.0f * 72.0f)) & ValueMask));
                case LengthUnits.Centimeters:               
                        return (GetRawType(PropertyType.AbsLength) | 
                                        ((uint)(len * ((8.0f * 20.0f * 72.0f) / 2.54f)) & ValueMask));
                case LengthUnits.Millimeters:               
                        return (GetRawType(PropertyType.AbsLength) | 
                                        ((uint)(len * ((8.0f * 20.0f * 72.0f) / 25.4f)) & ValueMask));
                case LengthUnits.HtmlFontUnits:             
                        return (GetRawType(PropertyType.HtmlFontUnits) | 
                                        ((uint)len & ValueMask));

                
                case LengthUnits.Pixels:                    
                                                            
                        return (GetRawType(PropertyType.Pixels) | 
                                        ((uint)(len * ((8.0f * 20.0f * 72.0f) / 120.0f)) & ValueMask));
                case LengthUnits.Ems:                       
                        return (GetRawType(PropertyType.Ems) | 
                                        ((uint)(len * (8.0f * 20.0f)) & ValueMask));
                case LengthUnits.Exs:                       
                        return (GetRawType(PropertyType.Exs) | 
                                        ((uint)(len * (8.0f * 20.0f)) & ValueMask));
                case LengthUnits.RelativeHtmlFontUnits:     
                        return (GetRawType(PropertyType.RelHtmlFontUnits) | 
                                        (((uint)(int)len) & ValueMask));
                case LengthUnits.Percents:                  
                        return (GetRawType(PropertyType.Percentage) | 
                                        ((uint)len & ValueMask));
            }

            InternalDebug.Assert(false, "invalid length unit type");
            return 0;
        }

        

        private static uint ComposeRawValue(LengthUnits lengthUnits, int len)
        {
            switch (lengthUnits)
            {
                

                case LengthUnits.BaseUnits:                 
                        return (GetRawType(PropertyType.AbsLength) | 
                                        ((uint)len & ValueMask));
                case LengthUnits.Twips:                     
                        return (GetRawType(PropertyType.AbsLength) | 
                                        ((uint)(len * (8)) & ValueMask));
                case LengthUnits.Points:                    
                        return (GetRawType(PropertyType.AbsLength) | 
                                        ((uint)(len * (8 * 20)) & ValueMask));
                case LengthUnits.Picas:                     
                        return (GetRawType(PropertyType.AbsLength) | 
                                        ((uint)(len * (8 * 20 * 12)) & ValueMask));
                case LengthUnits.Inches:                    
                        return (GetRawType(PropertyType.AbsLength) |
                                        ((uint)(len * (8 * 20 * 72)) & ValueMask));
                case LengthUnits.Centimeters:               
                        return (GetRawType(PropertyType.AbsLength) | 
                                        ((uint)(len * ((8 * 20 * 72 * 100) / 254)) & ValueMask));
                case LengthUnits.Millimeters:               
                        return (GetRawType(PropertyType.AbsLength) | 
                                        ((uint)(len * ((8 * 20 * 72 * 10) / 254)) & ValueMask));
                case LengthUnits.HtmlFontUnits:             
                        return (GetRawType(PropertyType.HtmlFontUnits) | 
                                        ((uint)len & ValueMask));

                

                case LengthUnits.Pixels:                    
                                                            
                        return (GetRawType(PropertyType.Pixels) | 
                                        ((uint)(len * ((8 * 20 * 72) / 120)) & ValueMask));
                case LengthUnits.Ems:                       
                        return (GetRawType(PropertyType.Ems) | 
                                        ((uint)(len * (8 * 20)) & ValueMask));
                case LengthUnits.Exs:                       
                        return (GetRawType(PropertyType.Exs) | 
                                        ((uint)(len * (8 * 20)) & ValueMask));
                case LengthUnits.RelativeHtmlFontUnits:     
                        return (GetRawType(PropertyType.RelHtmlFontUnits) | 
                                        ((uint)len & ValueMask));
                case LengthUnits.Percents:                  
                        return (GetRawType(PropertyType.Percentage) | 
                                        ((uint)len & ValueMask));
            }

            InternalDebug.Assert(false, "invalid length unit type");
            return 0;
        }

        

        private static uint ComposeRawValue(PropertyType type, float value)
        {
            InternalDebug.Assert(type == PropertyType.Fractional || type == PropertyType.Percentage);

            return (GetRawType(type) | ((uint)(value * 10000.0f) & ValueMask));
        }

        

        private static uint ComposeRawValue(RGBT color)
        {
            return (GetRawType(PropertyType.Color) | ((uint)color.RawValue & ValueMask));
        }

        private static uint ComposeRawValue(Enum value)
        {
            
            return (GetRawType(PropertyType.Enum) | ((uint)((IConvertible)value).ToInt32(null) & ValueMask));
        }


        

        public uint             RawValue        { get { return rawValue; } set { rawValue = value; } }
        public uint             RawType => (rawValue & TypeMask);

        public static uint GetRawType(PropertyType type)
        { 
            return ((uint)type << TypeShift); 
        }

        

        public PropertyType     Type => (PropertyType) ((rawValue & TypeMask) >> TypeShift);
        public int              Value => ((int)((rawValue & ValueMask) << ValueShift) >> ValueShift);
        public uint             UnsignedValue => rawValue & ValueMask;

        public bool             IsNull => RawType == GetRawType(PropertyType.Null);
        public bool             IsCalculated => RawType == GetRawType(PropertyType.Calculated);
        public bool             IsBool => RawType == GetRawType(PropertyType.Bool);
        public bool             IsEnum => RawType == GetRawType(PropertyType.Enum);

        public bool             IsString => RawType == GetRawType(PropertyType.String);
        public bool             IsMultiValue => RawType == GetRawType(PropertyType.MultiValue);
        public bool             IsRefCountedHandle => IsString || IsMultiValue;

        public bool             IsColor => RawType == GetRawType(PropertyType.Color);
        public bool             IsInteger => RawType == GetRawType(PropertyType.Integer);
        public bool             IsFractional => RawType == GetRawType(PropertyType.Fractional);
        public bool             IsPercentage => RawType == GetRawType(PropertyType.Percentage);
        public bool             IsAbsLength => RawType == GetRawType(PropertyType.AbsLength);
        public bool             IsPixels => RawType == GetRawType(PropertyType.Pixels);
        public bool             IsEms => RawType == GetRawType(PropertyType.Ems);
        public bool             IsExs => RawType == GetRawType(PropertyType.Exs);
        public bool             IsMilliseconds => RawType == GetRawType(PropertyType.Milliseconds);
        public bool             IsKHz => RawType == GetRawType(PropertyType.kHz);
        public bool             IsDegrees => RawType == GetRawType(PropertyType.Degrees);
        public bool             IsHtmlFontUnits => RawType == GetRawType(PropertyType.HtmlFontUnits);
        public bool             IsRelativeHtmlFontUnits => RawType == GetRawType(PropertyType.RelHtmlFontUnits);

        public bool             IsAbsRelLength => RawType == GetRawType(PropertyType.AbsLength) ||
                                                  RawType == GetRawType(PropertyType.RelLength) ||
                                                  RawType == GetRawType(PropertyType.Pixels);

        public int              StringHandle    { get { InternalDebug.Assert(Type == PropertyType.String); return Value; } }
        public int              MultiValueHandle { get { InternalDebug.Assert(Type == PropertyType.MultiValue); return Value; } }
        public bool             Bool            { get { InternalDebug.Assert(Type == PropertyType.Bool); return UnsignedValue != 0; } }
        public int              Enum            { get { InternalDebug.Assert(Type == PropertyType.Enum); return (int)UnsignedValue; } }
        public RGBT             Color           { get { InternalDebug.Assert(Type == PropertyType.Color); return new RGBT(UnsignedValue); } }
        public float            Percentage      { get { InternalDebug.Assert(Type == PropertyType.Percentage); return (float) Value / (10000.0f); } }
        public int              Percentage10K   { get { InternalDebug.Assert(Type == PropertyType.Percentage); return Value; } }
        public float            Fractional      { get { InternalDebug.Assert(Type == PropertyType.Fractional); return (float) Value / (10000.0f); } }
        public int              Integer         { get { InternalDebug.Assert(Type == PropertyType.Integer); return Value; } }
        public int              Milliseconds    { get { InternalDebug.Assert(Type == PropertyType.Milliseconds); return Value; } }
        public int              kHz             { get { InternalDebug.Assert(Type == PropertyType.kHz); return Value; } }
        public int              Degrees         { get { InternalDebug.Assert(Type == PropertyType.Degrees); return Value; } }

        public int              BaseUnits       { get { InternalDebug.Assert(IsAbsRelLength); return Value; } }
        public float            Twips           { get { InternalDebug.Assert(IsAbsRelLength); return (float) Value / (8.0f); } }
        public int              TwipsInteger    { get { InternalDebug.Assert(IsAbsRelLength); return Value / 8; } }
        public float            Points          { get { InternalDebug.Assert(IsAbsRelLength); return (float) Value / (8.0f * 20.0f) ; } }
        public int              PointsInteger   { get { InternalDebug.Assert(IsAbsRelLength); return Value / (8 * 20) ; } }
        public int              PointsInteger160 { get { InternalDebug.Assert(IsAbsRelLength); return Value; } }
        public float            Picas           { get { InternalDebug.Assert(IsAbsRelLength); return (float) Value / (8.0f * 20.0f * 12.0f); } }
        public float            Inches          { get { InternalDebug.Assert(IsAbsRelLength); return (float) Value / (8.0f * 20.0f * 72.0f); } }
        public float            Centimeters     { get { InternalDebug.Assert(IsAbsRelLength); return (float) Value / ((8.0f * 20.0f * 72.0f) / 2.54f); } }
        public int              MillimetersInteger { get { InternalDebug.Assert(IsAbsRelLength); return Value / 454; } }
        public float            Millimeters     { get { InternalDebug.Assert(IsAbsRelLength); return (float) Value / ((8.0f * 20.0f * 72.0f) / 25.4f); } }

        public int              HtmlFontUnits   { get { InternalDebug.Assert(Type == PropertyType.HtmlFontUnits); return Value; } }

        public float            Pixels          { get { InternalDebug.Assert(IsAbsRelLength); return (float) Value / ((8.0f * 20.0f * 72.0f) / 120.0f); } }
        public int              PixelsInteger   { get { InternalDebug.Assert(IsAbsRelLength); return Value / 96; } }
        public int              PixelsInteger96 { get { InternalDebug.Assert(IsAbsRelLength); return Value; } }
        public float            Ems             { get { InternalDebug.Assert(Type == PropertyType.Ems); return (float) Value / (8.0f * 20.0f); } }
        public int              EmsInteger      { get { InternalDebug.Assert(Type == PropertyType.Ems); return Value / (8 * 20); } }
        public int              EmsInteger160   { get { InternalDebug.Assert(Type == PropertyType.Ems); return Value; } }
        public float            Exs             { get { InternalDebug.Assert(Type == PropertyType.Exs); return (float) Value / (8.0f * 20.0f); } }
        public int              ExsInteger      { get { InternalDebug.Assert(Type == PropertyType.Exs); return Value / (8 * 20); } }
        public int              ExsInteger160   { get { InternalDebug.Assert(Type == PropertyType.Exs); return Value; } }

        public int              RelativeHtmlFontUnits { get { InternalDebug.Assert(Type == PropertyType.RelHtmlFontUnits); return Value; } }

        

        internal static int ConvertHtmlFontUnitsToTwips(int nHtmlSize)
        {
            
            
            nHtmlSize = Math.Max(1, Math.Min(7, nHtmlSize));
            return sizesInTwips[nHtmlSize - 1];
        }

        internal static int ConvertTwipsToHtmlFontUnits(int twips)
        {
            
            
            for (var i = 0; i < maxSizesInTwips.Length; i++)
            {
                if (twips <= maxSizesInTwips[i])
                    return i + 1;
            }

            return maxSizesInTwips.Length + 1;
        }

        public override string ToString()
        {
            switch (Type)
            {
                case PropertyType.Null:                 return "null";
                case PropertyType.Calculated:           return "calculated";
                case PropertyType.Bool:                 return Bool.ToString();
                case PropertyType.String:               return "string: " + StringHandle.ToString();
                case PropertyType.MultiValue:           return "multi: " + MultiValueHandle.ToString();
                case PropertyType.Enum:                 return "enum: " + Enum.ToString();
                case PropertyType.Color:                return "color: " + Color.ToString();
                case PropertyType.Integer:              return Integer.ToString() + " (integer)";
                case PropertyType.Fractional:           return Fractional.ToString() + " (fractional)";
                case PropertyType.Percentage:           return Percentage.ToString() + "%";
                case PropertyType.AbsLength:            return Points.ToString() + "pt (" + Inches.ToString() + "in, " + Millimeters.ToString() + "mm) (abs)";
                case PropertyType.RelLength:            return Points.ToString() + "pt (" + Inches.ToString() + "in, " + Millimeters.ToString() + "mm) (rel)";
                case PropertyType.HtmlFontUnits:        return HtmlFontUnits.ToString() + " (html font units)";
                case PropertyType.RelHtmlFontUnits:     return RelativeHtmlFontUnits.ToString() + " (relative html font units)";
                case PropertyType.Multiple:             return Integer.ToString() + "*";
                case PropertyType.Pixels:               return Pixels.ToString() + "px";
                case PropertyType.Ems:                  return Ems.ToString() + "em";
                case PropertyType.Exs:                  return Exs.ToString() + "ex";
                case PropertyType.Milliseconds:         return Milliseconds.ToString() + "ms";
                case PropertyType.kHz:                  return kHz.ToString() + "kHz";
                case PropertyType.Degrees:              return Degrees.ToString() + "deg";
            }

            return "unknown value type";
        }

        
        public static bool operator ==(PropertyValue x, PropertyValue y) 
        {
            return x.rawValue == y.rawValue;
        }

        
        public static bool operator !=(PropertyValue x, PropertyValue y) 
        {
            return x.rawValue != y.rawValue;
        }

        public override bool Equals(object obj)
        {
            return (obj is PropertyValue) && rawValue == ((PropertyValue)obj).rawValue;
        }

        public override int GetHashCode()
        {
            return (int)rawValue;
        }
    }

    

    [StructLayout(LayoutKind.Sequential, Pack=2)]
    internal struct Property
    {
        private PropertyId id;
        private PropertyValue value;

        public static readonly Property Null = new Property();

        

        public Property(PropertyId id, PropertyValue value)
        {
            this.id = id;
            this.value = value;
        }

        public bool IsNull => id == PropertyId.Null;

        public PropertyId Id { get { return id; } set { id = value; } }
        public PropertyValue Value { get { return value; } set { this.value = value; } }

        public void Set(PropertyId id, PropertyValue value)
        {
            this.id = id;
            this.value = value;
        }

        public override string ToString()
        {
            return id.ToString() + " = " + value.ToString();
        }
    }

    

    internal enum ListStyle
    {
        None,
        Bullet,
        Decimal,
        LowerAlpha,
        UpperAlpha,
        LowerRoman,
        UpperRoman,
#if false
        Disc,
        Bullet = Disc,
        Circle,
        Square,
        Decimal,
        DecimalLeadingZero,
        LowerRoman,
        UpperRoman,
        LowerGreek,
        LowerAlpha,
        LowerLatin,
        UpperAlpha,
        UpperLatin,
        Hebrew,
        Armenian,
        Georgian,
        CjkIdeographic,
        Hiragana,
        Katakana,
        HiraganaIroha,
        KatakanaIroha,
        None,
#endif
    }

    internal enum Align
    {
        Top = 0,
        Middle = 1,
        Center = Middle,
        Bottom = 2,
        Left = 3,
        Right = 4,
        BaseLine = 5,
        Justify = 6,
        Sub = 7,
        Super = 8,
        TextTop = 9,
        TextBottom = 10,
        Char = 11,
    }

    internal enum TextAlign
    {
        Left = Align.Left,
        Center = Align.Center,
        Right = Align.Right,
        Justify = Align.Justify,
    }

    internal enum BlockAlign
    {
        Top = Align.Top,
        Middle = Align.Middle,
        Bottom = Align.Bottom,
        Left = Align.Left,
        Right = Align.Right,
        Baseline = Align.BaseLine,
    }

    internal enum BlockHorizontalAlign
    {
        Left = Align.Left,
        Center = Align.Center,
        Right = Align.Right,
    }

    internal enum BlockVerticalAlign
    {
        Top = Align.Top,
        Middle = Align.Middle,
        Bottom = Align.Bottom,
        Baseline = Align.BaseLine,
    }

    internal enum LinkTarget
    {
        Self,
        Top,
        Blank,
        Parent
    }

    internal enum BorderStyle
    {
        None,
        Hidden,
        Dotted,
        Dashed,
        Solid,
        Double,
        Groove,
        Ridge,
        Inset,
        Outset,
    }

    internal enum UnicodeBiDi : byte
    {
        Normal,
        Embed,
        Override,
    }

    internal enum Display
    {
        None,
        Inline,
        Block,
        ListItem,
        RunIn,
        InlineBlock,
        Table,
        InlineTable,
        TableRowGroup,
        TableHeaderGroup,
        TableFooterGroup,
        TableRow,
        TableColumnGroup,
        TableColumn,
        TableCell,
        TableCaption,
    }
}
