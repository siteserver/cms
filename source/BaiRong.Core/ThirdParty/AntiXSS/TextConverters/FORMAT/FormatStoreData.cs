// ***************************************************************
// <copyright file="FormatStoreData.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters.Internal.Format
{
    internal class FormatStoreData
    {
        public static class DefaultInheritanceMaskIndex
        {
            public const int Null = 0;
            public const int Any = 1;
            public const int Normal = 2;
            public const int Table = 3;
            public const int TableRow = 4;
            public const int Text = 5;
        }

        internal const int GlobalStringValuesCount = 2;
        internal static FormatStore.StringValueEntry[] GlobalStringValues =
        {
            new FormatStore.StringValueEntry(),  
            new FormatStore.StringValueEntry("Courier New"),
        };
        internal const int GlobalMultiValuesCount = 1;
        internal static FormatStore.MultiValueEntry[] GlobalMultiValues = 
        {
            new FormatStore.MultiValueEntry(),  
        };
        internal const int GlobalStylesCount = 25;
        internal static FormatStore.StyleEntry[] GlobalStyles =
        {
            new FormatStore.StyleEntry(),  
            new FormatStore.StyleEntry( 
                    new FlagProperties(0x00000003),    
                    new PropertyBitMask(0x00000000, 0x00000000),
                    null),
            new FormatStore.StyleEntry( 
                    new FlagProperties(0x00000003),    
                    new PropertyBitMask(0x00000002, 0x00000000),
                    new Property[]
                    {
                        new Property(PropertyId.FontSize, new PropertyValue(PropertyType.RelHtmlFontUnits, 1)),    
                    }),
            new FormatStore.StyleEntry( 
                    new FlagProperties(0x00000C00),    
                    new PropertyBitMask(0x00000000, 0x00000000),
                    null),
            new FormatStore.StyleEntry( 
                    new FlagProperties(0x0000000C),    
                    new PropertyBitMask(0x00000000, 0x00000000),
                    null),
            new FormatStore.StyleEntry( 
                    new FlagProperties(0x00000030),    
                    new PropertyBitMask(0x00000000, 0x00000000),
                    null),
            new FormatStore.StyleEntry( 
                    new FlagProperties(0x00000002),    
                    new PropertyBitMask(0x00000002, 0x00000000),
                    new Property[]
                    {
                        new Property(PropertyId.FontSize, new PropertyValue(PropertyType.RelHtmlFontUnits, -1)),    
                    }),
            new FormatStore.StyleEntry( 
                    new FlagProperties(0x000000C0),    
                    new PropertyBitMask(0x00000002, 0x00000000),
                    new Property[]
                    {
                        new Property(PropertyId.FontSize, new PropertyValue(PropertyType.RelHtmlFontUnits, -1)),    
                    }),
            new FormatStore.StyleEntry( 
                    new FlagProperties(0x00000300),    
                    new PropertyBitMask(0x00000002, 0x00000000),
                    new Property[]
                    {
                        new Property(PropertyId.FontSize, new PropertyValue(PropertyType.RelHtmlFontUnits, -1)),    
                    }),
            new FormatStore.StyleEntry( 
                    new FlagProperties(0x00000000),
                    new PropertyBitMask(0x00000006, 0x00000000),
                    new Property[]
                    {
                        new Property(PropertyId.FontSize, new PropertyValue(PropertyType.RelHtmlFontUnits, -1)),    
                        new Property(PropertyId.FontFace, new PropertyValue(PropertyType.String, 1)),    
                    }),
            new FormatStore.StyleEntry( 
                    new FlagProperties(0x00000000),
                    new PropertyBitMask(0x00000200, 0x00000000),
                    new Property[]
                    {
                        new Property(PropertyId.UnicodeBiDi, new PropertyValue(PropertyType.Enum, 2)),    
                    }),
            new FormatStore.StyleEntry( 
                    new FlagProperties(0x00300000),    
                    new PropertyBitMask(0x00000000, 0x00000000),
                    null),
            new FormatStore.StyleEntry( 
                    new FlagProperties(0x00000000),
                    new PropertyBitMask(0x00000008, 0x00000000),
                    new Property[]
                    {
                        new Property(PropertyId.TextAlignment, new PropertyValue(PropertyType.Enum, 1)),    
                    }),
            new FormatStore.StyleEntry( 
                    new FlagProperties(0x003C0000),    
                    new PropertyBitMask(0x00000006, 0x00000000),
                    new Property[]
                    {
                        new Property(PropertyId.FontSize, new PropertyValue(PropertyType.RelHtmlFontUnits, -1)),    
                        new Property(PropertyId.FontFace, new PropertyValue(PropertyType.String, 1)),    
                    }),
            new FormatStore.StyleEntry( 
                    new FlagProperties(0x003C0000),    
                    new PropertyBitMask(0x00000006, 0x00000000),
                    new Property[]
                    {
                        new Property(PropertyId.FontSize, new PropertyValue(PropertyType.RelHtmlFontUnits, -2)),    
                        new Property(PropertyId.FontFace, new PropertyValue(PropertyType.String, 1)),    
                    }),
            new FormatStore.StyleEntry( 
                    new FlagProperties(0x00000000),
                    new PropertyBitMask(0x0000A000, 0x00000000),
                    new Property[]
                    {
                        new Property(PropertyId.RightMargin, new PropertyValue(PropertyType.FirstLength, 4800)),    
                        new Property(PropertyId.LeftMargin, new PropertyValue(PropertyType.FirstLength, 4800)),    
                    }),
            new FormatStore.StyleEntry( 
                    new FlagProperties(0x00000003),    
                    new PropertyBitMask(0x00000002, 0x00000000),
                    new Property[]
                    {
                        new Property(PropertyId.FontSize, new PropertyValue(PropertyType.HtmlFontUnits, 6)),    
                    }),
            new FormatStore.StyleEntry( 
                    new FlagProperties(0x00000003),    
                    new PropertyBitMask(0x00000002, 0x00000000),
                    new Property[]
                    {
                        new Property(PropertyId.FontSize, new PropertyValue(PropertyType.HtmlFontUnits, 5)),    
                    }),
            new FormatStore.StyleEntry( 
                    new FlagProperties(0x00000003),    
                    new PropertyBitMask(0x00000002, 0x00000000),
                    new Property[]
                    {
                        new Property(PropertyId.FontSize, new PropertyValue(PropertyType.HtmlFontUnits, 4)),    
                    }),
            new FormatStore.StyleEntry( 
                    new FlagProperties(0x00000003),    
                    new PropertyBitMask(0x00000002, 0x00000000),
                    new Property[]
                    {
                        new Property(PropertyId.FontSize, new PropertyValue(PropertyType.HtmlFontUnits, 3)),    
                    }),
            new FormatStore.StyleEntry( 
                    new FlagProperties(0x00000003),    
                    new PropertyBitMask(0x00000002, 0x00000000),
                    new Property[]
                    {
                        new Property(PropertyId.FontSize, new PropertyValue(PropertyType.HtmlFontUnits, 2)),    
                    }),
            new FormatStore.StyleEntry( 
                    new FlagProperties(0x00000003),    
                    new PropertyBitMask(0x00000002, 0x00000000),
                    new Property[]
                    {
                        new Property(PropertyId.FontSize, new PropertyValue(PropertyType.HtmlFontUnits, 1)),    
                    }),
            new FormatStore.StyleEntry( 
                    new FlagProperties(0x00000000),
                    new PropertyBitMask(0x00000000, 0x00000006),
                    new Property[]
                    {
                        new Property(PropertyId.ListStyle, new PropertyValue(PropertyType.Enum, 2)),    
                        new Property(PropertyId.ListStart, new PropertyValue(PropertyType.Integer, 1)),    
                    }),
            new FormatStore.StyleEntry( 
                    new FlagProperties(0x00000000),
                    new PropertyBitMask(0x00000000, 0x00000002),
                    new Property[]
                    {
                        new Property(PropertyId.ListStyle, new PropertyValue(PropertyType.Enum, 1)),    
                    }),
            new FormatStore.StyleEntry( 
                    new FlagProperties(0x00000000),
                    new PropertyBitMask(0x00005000, 0x00000000),
                    new Property[]
                    {
                        new Property(PropertyId.TopMargin, new PropertyValue(PropertyType.FirstLength, 0)),    
                        new Property(PropertyId.BottomMargin, new PropertyValue(PropertyType.FirstLength, 0)),    
                    }),
        };
        internal const int GlobalInheritanceMasksCount = 6;
        internal static FormatStore.InheritaceMask[] GlobalInheritanceMasks =
        {
            new FormatStore.InheritaceMask(new FlagProperties(0x00000000), new PropertyBitMask(0x00000000, 0x00000000)),
            new FormatStore.InheritaceMask(new FlagProperties(0xFFFFFFFF), new PropertyBitMask(0xFFFFFFFF, 0xFFFFFFFF)),
            new FormatStore.InheritaceMask(new FlagProperties(0x00FFFFFF), new PropertyBitMask(0x0000015F, 0x00000002)),
            new FormatStore.InheritaceMask(new FlagProperties(0x00C30000), new PropertyBitMask(0x00000144, 0x00000002)),
            new FormatStore.InheritaceMask(new FlagProperties(0x0FFFFFFF), new PropertyBitMask(0x0000055F, 0x00007FE2)),
            new FormatStore.InheritaceMask(new FlagProperties(0x00FFFFFF), new PropertyBitMask(0x00000147, 0x00000000)),
        };
    }
}
