// ***************************************************************
// <copyright file="HtmlConverterData.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters.Internal.Html
{
    using Format;

    internal delegate PropertyValue PropertyValueParsingMethod(BufferString value, FormatConverter formatConverter);
    internal delegate void MultiPropertyParsingMethod(BufferString value, FormatConverter formatConverter, PropertyId basePropertyId, Property[] outputProperties, out int parsedPropertiesCount);

    internal struct HtmlAttributeInstruction
    {
        private HtmlNameIndex attributeNameId;
        private PropertyId propertyId;
        private PropertyValueParsingMethod parsingMethod;

        public HtmlAttributeInstruction(HtmlNameIndex attributeNameId, PropertyId propertyId, PropertyValueParsingMethod parsingMethod)
        {
            this.attributeNameId = attributeNameId;
            this.propertyId = propertyId;
            this.parsingMethod = parsingMethod;
        }

        public HtmlNameIndex AttributeNameId => attributeNameId;
        public PropertyId PropertyId => propertyId;
        public PropertyValueParsingMethod ParsingMethod => parsingMethod;
    }

    internal struct HtmlTagInstruction
    {
        private FormatContainerType containerType;
        private int defaultStyle;
        private int inheritanceMaskIndex;
        private HtmlAttributeInstruction[] attributeInstructions;

        public HtmlTagInstruction(
            FormatContainerType containerType,
            int defaultStyle,
            int inheritanceMaskIndex,
            HtmlAttributeInstruction[] attributeInstructions)
        {
            this.containerType = containerType;
            this.defaultStyle = defaultStyle;
            this.inheritanceMaskIndex = inheritanceMaskIndex;
            this.attributeInstructions = attributeInstructions;
        }

        public FormatContainerType ContainerType => containerType;
        public int DefaultStyle => defaultStyle;
        public int InheritanceMaskIndex => inheritanceMaskIndex;
        public HtmlAttributeInstruction[] AttributeInstructions => attributeInstructions;
    }

    internal struct CssPropertyInstruction
    {
        private PropertyId propertyId;
        private PropertyValueParsingMethod parsingMethod;
        private MultiPropertyParsingMethod multiPropertyParsingMethod;

        public CssPropertyInstruction(PropertyId propertyId, PropertyValueParsingMethod parsingMethod, MultiPropertyParsingMethod multiPropertyParsingMethod)
        {
            this.propertyId = propertyId;
            this.parsingMethod = parsingMethod;
            this.multiPropertyParsingMethod = multiPropertyParsingMethod;
        }

        public PropertyId PropertyId => propertyId;
        public PropertyValueParsingMethod ParsingMethod => parsingMethod;
        public MultiPropertyParsingMethod MultiPropertyParsingMethod => multiPropertyParsingMethod;
    }

    internal static class HtmlConverterData
    {
        public static class DefaultStyle
        {
            public const int None = 0;
            public const int B = 1;
            public const int Big = 2;
            public const int Del = 3;
            public const int EM = 4;
            public const int I = 4;
            public const int Ins = 5;
            public const int S = 3;
            public const int Small = 6;
            public const int Strike = 3;
            public const int Strong = 1;
            public const int Sub = 7;
            public const int Sup = 8;
            public const int U = 5;
            public const int Var = 4;
            public const int Code = 9;
            public const int Cite = 4;
            public const int Doofn = 4;
            public const int Kbd = 9;
            public const int Samp = 9;
            public const int TT = 9;
            public const int Bdo = 10;
            public const int NoBR = 11;
            public const int Center = 12;
            public const int Xmp = 13;
            public const int Pre = 13;
            public const int Listing = 14;
            public const int PlainText = 13;
            public const int BlockQuote = 15;
            public const int Address = 4;
            public const int H1 = 16;
            public const int H2 = 17;
            public const int H3 = 18;
            public const int H4 = 19;
            public const int H5 = 20;
            public const int H6 = 21;
            public const int OL = 22;
            public const int UL = 23;
            public const int Dir = 23;
            public const int Menu = 23;
            public const int DT = 24;
            public const int DD = 24;
            public const int Caption = 12;
        }

        public static class PropertyValueParsingMethods
        {
            public static readonly PropertyValueParsingMethod ParseBlockAlignment = HtmlSupport.ParseBlockAlignment;
            public static readonly PropertyValueParsingMethod ParseBooleanAttribute = HtmlSupport.ParseBooleanAttribute;
            public static readonly PropertyValueParsingMethod ParseBorderCollapse = HtmlSupport.ParseBorderCollapse;
            public static readonly PropertyValueParsingMethod ParseBorderStyle = HtmlSupport.ParseBorderStyle;
            public static readonly PropertyValueParsingMethod ParseBorderWidth = HtmlSupport.ParseBorderWidth;
            public static readonly PropertyValueParsingMethod ParseCaptionSide = HtmlSupport.ParseCaptionSide;
            public static readonly PropertyValueParsingMethod ParseColor = HtmlSupport.ParseColor;
            public static readonly PropertyValueParsingMethod ParseColorCss = HtmlSupport.ParseColorCss;
            public static readonly PropertyValueParsingMethod ParseCssFontSize = HtmlSupport.ParseCssFontSize;
            public static readonly PropertyValueParsingMethod ParseDirection = HtmlSupport.ParseDirection;
            public static readonly PropertyValueParsingMethod ParseDisplay = HtmlSupport.ParseDisplay;
            public static readonly PropertyValueParsingMethod ParseEmptyCells = HtmlSupport.ParseEmptyCells;
            public static readonly PropertyValueParsingMethod ParseFontFace = HtmlSupport.ParseFontFace;
            public static readonly PropertyValueParsingMethod ParseFontSize = HtmlSupport.ParseFontSize;
            public static readonly PropertyValueParsingMethod ParseFontStyle = HtmlSupport.ParseFontStyle;
            public static readonly PropertyValueParsingMethod ParseFontVariant = HtmlSupport.ParseFontVariant;
            public static readonly PropertyValueParsingMethod ParseFontWeight = HtmlSupport.ParseFontWeight;
            public static readonly PropertyValueParsingMethod ParseHorizontalAlignment = HtmlSupport.ParseHorizontalAlignment;
            public static readonly PropertyValueParsingMethod ParseLanguage = HtmlSupport.ParseLanguage;
            public static readonly PropertyValueParsingMethod ParseLength = HtmlSupport.ParseLength;
            public static readonly PropertyValueParsingMethod ParseNonNegativeInteger = HtmlSupport.ParseNonNegativeInteger;
            public static readonly PropertyValueParsingMethod ParseNonNegativeLength = HtmlSupport.ParseNonNegativeLength;
            public static readonly PropertyValueParsingMethod ParseStringProperty = HtmlSupport.ParseStringProperty;
            public static readonly PropertyValueParsingMethod ParseTableFrame = HtmlSupport.ParseTableFrame;
            public static readonly PropertyValueParsingMethod ParseTableLayout = HtmlSupport.ParseTableLayout;
            public static readonly PropertyValueParsingMethod ParseTableRules = HtmlSupport.ParseTableRules;
            public static readonly PropertyValueParsingMethod ParseTarget = HtmlSupport.ParseTarget;
            public static readonly PropertyValueParsingMethod ParseTextAlignment = HtmlSupport.ParseTextAlignment;
            public static readonly PropertyValueParsingMethod ParseUnicodeBiDi = HtmlSupport.ParseUnicodeBiDi;
            public static readonly PropertyValueParsingMethod ParseUrl = HtmlSupport.ParseUrl;
            public static readonly PropertyValueParsingMethod ParseVerticalAlignment = HtmlSupport.ParseVerticalAlignment;
            public static readonly PropertyValueParsingMethod ParseVisibility = HtmlSupport.ParseVisibility;
        }

        public static class MultiPropertyParsingMethods
        {
            public static readonly MultiPropertyParsingMethod ParseCompositeAllBorders = HtmlSupport.ParseCompositeAllBorders;
            public static readonly MultiPropertyParsingMethod ParseCompositeBackground = HtmlSupport.ParseCompositeBackground;
            public static readonly MultiPropertyParsingMethod ParseCompositeBorder = HtmlSupport.ParseCompositeBorder;
            public static readonly MultiPropertyParsingMethod ParseCompositeBorderStyle = HtmlSupport.ParseCompositeBorderStyle;
            public static readonly MultiPropertyParsingMethod ParseCompositeBorderWidth = HtmlSupport.ParseCompositeBorderWidth;
            public static readonly MultiPropertyParsingMethod ParseCompositeColor = HtmlSupport.ParseCompositeColor;
            public static readonly MultiPropertyParsingMethod ParseCompositeFont = HtmlSupport.ParseCompositeFont;
            public static readonly MultiPropertyParsingMethod ParseCompositeLength = HtmlSupport.ParseCompositeLength;
            public static readonly MultiPropertyParsingMethod ParseCompositeNonNegativeLength = HtmlSupport.ParseCompositeNonNegativeLength;
            public static readonly MultiPropertyParsingMethod ParseCompoundBorderSpacing = HtmlSupport.ParseCompoundBorderSpacing;
            public static readonly MultiPropertyParsingMethod ParseCssTextDecoration = HtmlSupport.ParseCssTextDecoration;
            public static readonly MultiPropertyParsingMethod ParseCssTextTransform = HtmlSupport.ParseCssTextTransform;
            public static readonly MultiPropertyParsingMethod ParseCssVerticalAlignment = HtmlSupport.ParseCssVerticalAlignment;
            public static readonly MultiPropertyParsingMethod ParseCssWhiteSpace = HtmlSupport.ParseCssWhiteSpace;
        }

        public static HtmlTagInstruction[] tagInstructions =
        {
            new HtmlTagInstruction(),    
            new HtmlTagInstruction(),    
            new HtmlTagInstruction(),    
            new HtmlTagInstruction(),    
            new HtmlTagInstruction(),    
            new HtmlTagInstruction(),    
            new HtmlTagInstruction(),    
            new HtmlTagInstruction(),    
            new HtmlTagInstruction(    
                        FormatContainerType.HyperLink,
                        DefaultStyle.None,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Name,
                                    PropertyId.BookmarkName,
                                    PropertyValueParsingMethods.ParseUrl),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Href,
                                    PropertyId.HyperlinkUrl,
                                    PropertyValueParsingMethods.ParseUrl),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Target,
                                    PropertyId.HyperlinkTarget,
                                    PropertyValueParsingMethods.ParseTarget),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(),    
            new HtmlTagInstruction(    
                        FormatContainerType.PropertyContainer,
                        DefaultStyle.None,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.Block,
                        DefaultStyle.Address,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.PropertyContainer,
                        DefaultStyle.None,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        null),
            new HtmlTagInstruction(    
                        FormatContainerType.Area,
                        DefaultStyle.None,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Href,
                                    PropertyId.HyperlinkUrl,
                                    PropertyValueParsingMethods.ParseUrl),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Target,
                                    PropertyId.HyperlinkTarget,
                                    PropertyValueParsingMethods.ParseTarget),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Alt,
                                    PropertyId.ImageAltText,
                                    PropertyValueParsingMethods.ParseStringProperty),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.PropertyContainer,
                        DefaultStyle.B,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(),    
            new HtmlTagInstruction(    
                        FormatContainerType.BaseFont,
                        DefaultStyle.None,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Face,
                                    PropertyId.FontFace,
                                    PropertyValueParsingMethods.ParseFontFace),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Size,
                                    PropertyId.FontSize,
                                    PropertyValueParsingMethods.ParseFontSize),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Color,
                                    PropertyId.FontColor,
                                    PropertyValueParsingMethods.ParseColor),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.Inline,
                        DefaultStyle.Bdo,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(),    
            new HtmlTagInstruction(    
                        FormatContainerType.PropertyContainer,
                        DefaultStyle.Big,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.PropertyContainer,
                        DefaultStyle.None,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.BlockQuote,
                        DefaultStyle.BlockQuote,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.Block,
                        DefaultStyle.None,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.BGColor,
                                    PropertyId.BackColor,
                                    PropertyValueParsingMethods.ParseColor),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.PropertyContainer,
                        DefaultStyle.None,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        null),
            new HtmlTagInstruction(    
                        FormatContainerType.Button,
                        DefaultStyle.None,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Name,
                                    PropertyId.BookmarkName,
                                    PropertyValueParsingMethods.ParseUrl),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Value,
                                    PropertyId.FormAcceptCharsets,
                                    PropertyValueParsingMethods.ParseStringProperty),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Disabled,
                                    PropertyId.TableBorderCollapse,
                                    PropertyValueParsingMethods.ParseBooleanAttribute),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.TableCaption,
                        DefaultStyle.Caption,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Align,
                                    PropertyId.BlockAlignment,
                                    PropertyValueParsingMethods.ParseBlockAlignment),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.Block,
                        DefaultStyle.Center,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.PropertyContainer,
                        DefaultStyle.Cite,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.PropertyContainer,
                        DefaultStyle.Code,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.TableColumn,
                        DefaultStyle.None,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Width,
                                    PropertyId.Width,
                                    PropertyValueParsingMethods.ParseNonNegativeLength),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Span,
                                    PropertyId.NumColumns,
                                    PropertyValueParsingMethods.ParseNonNegativeInteger),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Align,
                                    PropertyId.TextAlignment,
                                    PropertyValueParsingMethods.ParseTextAlignment),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Valign,
                                    PropertyId.BlockAlignment,
                                    PropertyValueParsingMethods.ParseVerticalAlignment),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.TableColumnGroup,
                        DefaultStyle.None,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Width,
                                    PropertyId.Width,
                                    PropertyValueParsingMethods.ParseNonNegativeLength),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Span,
                                    PropertyId.NumColumns,
                                    PropertyValueParsingMethods.ParseNonNegativeInteger),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Align,
                                    PropertyId.TextAlignment,
                                    PropertyValueParsingMethods.ParseTextAlignment),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Valign,
                                    PropertyId.BlockAlignment,
                                    PropertyValueParsingMethods.ParseVerticalAlignment),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(),    
            new HtmlTagInstruction(    
                        FormatContainerType.Block,
                        DefaultStyle.DD,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.PropertyContainer,
                        DefaultStyle.Del,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.PropertyContainer,
                        DefaultStyle.Doofn,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.List,
                        DefaultStyle.Dir,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.Block,
                        DefaultStyle.None,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Align,
                                    PropertyId.TextAlignment,
                                    PropertyValueParsingMethods.ParseTextAlignment),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.Block,
                        DefaultStyle.None,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.Block,
                        DefaultStyle.DT,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.PropertyContainer,
                        DefaultStyle.EM,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.PropertyContainer,
                        DefaultStyle.None,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        null),
            new HtmlTagInstruction(    
                        FormatContainerType.FieldSet,
                        DefaultStyle.None,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.PropertyContainer,
                        DefaultStyle.None,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Face,
                                    PropertyId.FontFace,
                                    PropertyValueParsingMethods.ParseFontFace),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Size,
                                    PropertyId.FontSize,
                                    PropertyValueParsingMethods.ParseFontSize),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Color,
                                    PropertyId.FontColor,
                                    PropertyValueParsingMethods.ParseColor),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.Form,
                        DefaultStyle.None,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Action,
                                    PropertyId.HyperlinkUrl,
                                    PropertyValueParsingMethods.ParseUrl),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.EncType,
                                    PropertyId.IFrameUrl,
                                    PropertyValueParsingMethods.ParseStringProperty),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Accept,
                                    PropertyId.ImageAltText,
                                    PropertyValueParsingMethods.ParseStringProperty),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.AcceptCharset,
                                    PropertyId.FormAcceptCharsets,
                                    PropertyValueParsingMethods.ParseStringProperty),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(),    
            new HtmlTagInstruction(),    
            new HtmlTagInstruction(    
                        FormatContainerType.Block,
                        DefaultStyle.H1,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Align,
                                    PropertyId.TextAlignment,
                                    PropertyValueParsingMethods.ParseTextAlignment),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.Block,
                        DefaultStyle.H2,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Align,
                                    PropertyId.TextAlignment,
                                    PropertyValueParsingMethods.ParseTextAlignment),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.Block,
                        DefaultStyle.H3,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Align,
                                    PropertyId.TextAlignment,
                                    PropertyValueParsingMethods.ParseTextAlignment),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.Block,
                        DefaultStyle.H4,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Align,
                                    PropertyId.TextAlignment,
                                    PropertyValueParsingMethods.ParseTextAlignment),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.Block,
                        DefaultStyle.H5,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Align,
                                    PropertyId.TextAlignment,
                                    PropertyValueParsingMethods.ParseTextAlignment),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.Block,
                        DefaultStyle.H6,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Align,
                                    PropertyId.TextAlignment,
                                    PropertyValueParsingMethods.ParseTextAlignment),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(),    
            new HtmlTagInstruction(    
                        FormatContainerType.HorizontalLine,
                        DefaultStyle.None,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Size,
                                    PropertyId.Height,
                                    PropertyValueParsingMethods.ParseNonNegativeLength),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Width,
                                    PropertyId.Width,
                                    PropertyValueParsingMethods.ParseNonNegativeLength),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Color,
                                    PropertyId.FontColor,
                                    PropertyValueParsingMethods.ParseColor),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Align,
                                    PropertyId.BlockAlignment,
                                    PropertyValueParsingMethods.ParseHorizontalAlignment),
                        }),
            new HtmlTagInstruction(),    
            new HtmlTagInstruction(    
                        FormatContainerType.PropertyContainer,
                        DefaultStyle.I,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.Block,
                        DefaultStyle.None,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Align,
                                    PropertyId.BlockAlignment,
                                    PropertyValueParsingMethods.ParseBlockAlignment),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Height,
                                    PropertyId.Height,
                                    PropertyValueParsingMethods.ParseNonNegativeLength),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Width,
                                    PropertyId.Width,
                                    PropertyValueParsingMethods.ParseNonNegativeLength),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Src,
                                    PropertyId.IFrameUrl,
                                    PropertyValueParsingMethods.ParseUrl),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.Image,
                        DefaultStyle.None,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Align,
                                    PropertyId.BlockAlignment,
                                    PropertyValueParsingMethods.ParseBlockAlignment),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Src,
                                    PropertyId.IFrameUrl,
                                    PropertyValueParsingMethods.ParseUrl),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Width,
                                    PropertyId.Width,
                                    PropertyValueParsingMethods.ParseNonNegativeLength),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Height,
                                    PropertyId.Height,
                                    PropertyValueParsingMethods.ParseNonNegativeLength),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Border,
                                    PropertyId.ImageBorder,
                                    PropertyValueParsingMethods.ParseNonNegativeLength),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Alt,
                                    PropertyId.ImageAltText,
                                    PropertyValueParsingMethods.ParseStringProperty),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.Image,
                        DefaultStyle.None,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Align,
                                    PropertyId.BlockAlignment,
                                    PropertyValueParsingMethods.ParseBlockAlignment),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Src,
                                    PropertyId.IFrameUrl,
                                    PropertyValueParsingMethods.ParseUrl),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Width,
                                    PropertyId.Width,
                                    PropertyValueParsingMethods.ParseNonNegativeLength),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Height,
                                    PropertyId.Height,
                                    PropertyValueParsingMethods.ParseNonNegativeLength),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Border,
                                    PropertyId.ImageBorder,
                                    PropertyValueParsingMethods.ParseNonNegativeLength),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Alt,
                                    PropertyId.ImageAltText,
                                    PropertyValueParsingMethods.ParseStringProperty),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.Input,
                        DefaultStyle.None,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Name,
                                    PropertyId.BookmarkName,
                                    PropertyValueParsingMethods.ParseUrl),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.ReadOnly,
                                    PropertyId.ReadOnly,
                                    PropertyValueParsingMethods.ParseBooleanAttribute),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Disabled,
                                    PropertyId.TableBorderCollapse,
                                    PropertyValueParsingMethods.ParseBooleanAttribute),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Checked,
                                    PropertyId.Checked,
                                    PropertyValueParsingMethods.ParseBooleanAttribute),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Value,
                                    PropertyId.FormAcceptCharsets,
                                    PropertyValueParsingMethods.ParseStringProperty),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Size,
                                    PropertyId.TableFrame,
                                    PropertyValueParsingMethods.ParseNonNegativeInteger),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.MaxLength,
                                    PropertyId.InputMaxLength,
                                    PropertyValueParsingMethods.ParseNonNegativeInteger),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Src,
                                    PropertyId.IFrameUrl,
                                    PropertyValueParsingMethods.ParseUrl),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Alt,
                                    PropertyId.ImageAltText,
                                    PropertyValueParsingMethods.ParseStringProperty),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.PropertyContainer,
                        DefaultStyle.Ins,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.Block,
                        DefaultStyle.None,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Name,
                                    PropertyId.BookmarkName,
                                    PropertyValueParsingMethods.ParseUrl),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Prompt,
                                    PropertyId.ImageAltText,
                                    PropertyValueParsingMethods.ParseStringProperty),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Disabled,
                                    PropertyId.TableBorderCollapse,
                                    PropertyValueParsingMethods.ParseBooleanAttribute),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.PropertyContainer,
                        DefaultStyle.Kbd,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.Label,
                        DefaultStyle.None,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.For,
                                    PropertyId.HyperlinkUrl,
                                    PropertyValueParsingMethods.ParseUrl),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.Legend,
                        DefaultStyle.None,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Align,
                                    PropertyId.BlockAlignment,
                                    PropertyValueParsingMethods.ParseBlockAlignment),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.ListItem,
                        DefaultStyle.None,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Start,
                                    PropertyId.ListStart,
                                    PropertyValueParsingMethods.ParseNonNegativeInteger),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(),    
            new HtmlTagInstruction(    
                        FormatContainerType.Block,
                        DefaultStyle.Listing,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.Map,
                        DefaultStyle.None,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        null),
            new HtmlTagInstruction(    
                        FormatContainerType.Block,
                        DefaultStyle.None,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        null),
            new HtmlTagInstruction(    
                        FormatContainerType.List,
                        DefaultStyle.Menu,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(),    
            new HtmlTagInstruction(),    
            new HtmlTagInstruction(    
                        FormatContainerType.PropertyContainer,
                        DefaultStyle.NoBR,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        null),
            new HtmlTagInstruction(),    
            new HtmlTagInstruction(),    
            new HtmlTagInstruction(),    
            new HtmlTagInstruction(    
                        FormatContainerType.PropertyContainer,
                        DefaultStyle.None,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        null),
            new HtmlTagInstruction(    
                        FormatContainerType.List,
                        DefaultStyle.OL,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Start,
                                    PropertyId.ListStart,
                                    PropertyValueParsingMethods.ParseNonNegativeInteger),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.OptionGroup,
                        DefaultStyle.None,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Label,
                                    PropertyId.ImageAltText,
                                    PropertyValueParsingMethods.ParseStringProperty),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Disabled,
                                    PropertyId.TableBorderCollapse,
                                    PropertyValueParsingMethods.ParseBooleanAttribute),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.Option,
                        DefaultStyle.None,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Selected,
                                    PropertyId.ReadOnly,
                                    PropertyValueParsingMethods.ParseBooleanAttribute),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Label,
                                    PropertyId.ImageAltText,
                                    PropertyValueParsingMethods.ParseStringProperty),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Value,
                                    PropertyId.FormAcceptCharsets,
                                    PropertyValueParsingMethods.ParseStringProperty),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Disabled,
                                    PropertyId.TableBorderCollapse,
                                    PropertyValueParsingMethods.ParseBooleanAttribute),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.Block,
                        DefaultStyle.None,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Align,
                                    PropertyId.TextAlignment,
                                    PropertyValueParsingMethods.ParseTextAlignment),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.PropertyContainer,
                        DefaultStyle.None,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        null),
            new HtmlTagInstruction(    
                        FormatContainerType.Block,
                        DefaultStyle.PlainText,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.Block,
                        DefaultStyle.Pre,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Width,
                                    PropertyId.Width,
                                    PropertyValueParsingMethods.ParseNonNegativeLength),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.PropertyContainer,
                        DefaultStyle.None,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.PropertyContainer,
                        DefaultStyle.None,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        null),
            new HtmlTagInstruction(    
                        FormatContainerType.PropertyContainer,
                        DefaultStyle.None,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        null),
            new HtmlTagInstruction(    
                        FormatContainerType.PropertyContainer,
                        DefaultStyle.None,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        null),
            new HtmlTagInstruction(    
                        FormatContainerType.PropertyContainer,
                        DefaultStyle.S,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.PropertyContainer,
                        DefaultStyle.Samp,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(),    
            new HtmlTagInstruction(    
                        FormatContainerType.Select,
                        DefaultStyle.None,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Name,
                                    PropertyId.BookmarkName,
                                    PropertyValueParsingMethods.ParseUrl),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Multiple,
                                    PropertyId.ReadOnly,
                                    PropertyValueParsingMethods.ParseBooleanAttribute),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Disabled,
                                    PropertyId.TableBorderCollapse,
                                    PropertyValueParsingMethods.ParseBooleanAttribute),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.PropertyContainer,
                        DefaultStyle.Small,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.PropertyContainer,
                        DefaultStyle.None,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.PropertyContainer,
                        DefaultStyle.Strike,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.PropertyContainer,
                        DefaultStyle.Strong,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(),    
            new HtmlTagInstruction(    
                        FormatContainerType.PropertyContainer,
                        DefaultStyle.Sub,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.PropertyContainer,
                        DefaultStyle.Sup,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.Table,
                        DefaultStyle.None,
                        FormatStoreData.DefaultInheritanceMaskIndex.Table,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Width,
                                    PropertyId.Width,
                                    PropertyValueParsingMethods.ParseNonNegativeLength),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Align,
                                    PropertyId.BlockAlignment,
                                    PropertyValueParsingMethods.ParseHorizontalAlignment),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Border,
                                    PropertyId.ImageBorder,
                                    PropertyValueParsingMethods.ParseNonNegativeLength),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Frame,
                                    PropertyId.TableFrame,
                                    PropertyValueParsingMethods.ParseTableFrame),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Rules,
                                    PropertyId.InputMaxLength,
                                    PropertyValueParsingMethods.ParseTableRules),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.BGColor,
                                    PropertyId.BackColor,
                                    PropertyValueParsingMethods.ParseColor),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.CellSpacing,
                                    PropertyId.TableCellSpacing,
                                    PropertyValueParsingMethods.ParseNonNegativeLength),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.CellPadding,
                                    PropertyId.TableCellPadding,
                                    PropertyValueParsingMethods.ParseNonNegativeLength),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(),    
            new HtmlTagInstruction(    
                        FormatContainerType.TableExtraContent,
                        DefaultStyle.None,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        null),
            new HtmlTagInstruction(    
                        FormatContainerType.TableCell,
                        DefaultStyle.None,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Height,
                                    PropertyId.Height,
                                    PropertyValueParsingMethods.ParseNonNegativeLength),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Width,
                                    PropertyId.Width,
                                    PropertyValueParsingMethods.ParseNonNegativeLength),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.ColSpan,
                                    PropertyId.NumColumns,
                                    PropertyValueParsingMethods.ParseNonNegativeInteger),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.RowSpan,
                                    PropertyId.NumRows,
                                    PropertyValueParsingMethods.ParseNonNegativeInteger),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Align,
                                    PropertyId.TextAlignment,
                                    PropertyValueParsingMethods.ParseTextAlignment),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Valign,
                                    PropertyId.BlockAlignment,
                                    PropertyValueParsingMethods.ParseVerticalAlignment),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.NoWrap,
                                    PropertyId.TableCellNoWrap,
                                    PropertyValueParsingMethods.ParseBooleanAttribute),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.BGColor,
                                    PropertyId.BackColor,
                                    PropertyValueParsingMethods.ParseColor),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.TextArea,
                        DefaultStyle.None,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Name,
                                    PropertyId.BookmarkName,
                                    PropertyValueParsingMethods.ParseUrl),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.ReadOnly,
                                    PropertyId.ReadOnly,
                                    PropertyValueParsingMethods.ParseBooleanAttribute),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Disabled,
                                    PropertyId.TableBorderCollapse,
                                    PropertyValueParsingMethods.ParseBooleanAttribute),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Cols,
                                    PropertyId.NumColumns,
                                    PropertyValueParsingMethods.ParseNonNegativeInteger),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Rows,
                                    PropertyId.NumRows,
                                    PropertyValueParsingMethods.ParseNonNegativeInteger),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(),    
            new HtmlTagInstruction(    
                        FormatContainerType.TableCell,
                        DefaultStyle.None,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Height,
                                    PropertyId.Height,
                                    PropertyValueParsingMethods.ParseNonNegativeLength),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Width,
                                    PropertyId.Width,
                                    PropertyValueParsingMethods.ParseNonNegativeLength),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.ColSpan,
                                    PropertyId.NumColumns,
                                    PropertyValueParsingMethods.ParseNonNegativeInteger),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.RowSpan,
                                    PropertyId.NumRows,
                                    PropertyValueParsingMethods.ParseNonNegativeInteger),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Align,
                                    PropertyId.TextAlignment,
                                    PropertyValueParsingMethods.ParseTextAlignment),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Valign,
                                    PropertyId.BlockAlignment,
                                    PropertyValueParsingMethods.ParseVerticalAlignment),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.NoWrap,
                                    PropertyId.TableCellNoWrap,
                                    PropertyValueParsingMethods.ParseBooleanAttribute),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.BGColor,
                                    PropertyId.BackColor,
                                    PropertyValueParsingMethods.ParseColor),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(),    
            new HtmlTagInstruction(),    
            new HtmlTagInstruction(    
                        FormatContainerType.TableRow,
                        DefaultStyle.None,
                        FormatStoreData.DefaultInheritanceMaskIndex.TableRow,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Height,
                                    PropertyId.Height,
                                    PropertyValueParsingMethods.ParseNonNegativeLength),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Align,
                                    PropertyId.TextAlignment,
                                    PropertyValueParsingMethods.ParseTextAlignment),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Valign,
                                    PropertyId.BlockAlignment,
                                    PropertyValueParsingMethods.ParseVerticalAlignment),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.BGColor,
                                    PropertyId.BackColor,
                                    PropertyValueParsingMethods.ParseColor),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.PropertyContainer,
                        DefaultStyle.TT,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.PropertyContainer,
                        DefaultStyle.U,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.List,
                        DefaultStyle.UL,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(    
                        FormatContainerType.PropertyContainer,
                        DefaultStyle.Var,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(),    
            new HtmlTagInstruction(    
                        FormatContainerType.Block,
                        DefaultStyle.Xmp,
                        FormatStoreData.DefaultInheritanceMaskIndex.Normal,
                        new HtmlAttributeInstruction[]
                        {
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Dir,
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection),
                            new HtmlAttributeInstruction(
                                    HtmlNameIndex.Lang,
                                    PropertyId.Language,
                                    PropertyValueParsingMethods.ParseLanguage),
                        }),
            new HtmlTagInstruction(),    
            new HtmlTagInstruction(),    
            new HtmlTagInstruction(),    
            new HtmlTagInstruction(),    
        };
        public static CssPropertyInstruction[] cssPropertyInstructions =
        {
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(    
                                    PropertyId.Null,
                                    null,
                                    MultiPropertyParsingMethods.ParseCssWhiteSpace),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(    
                                    PropertyId.BlockAlignment,
                                    null,
                                    MultiPropertyParsingMethods.ParseCssVerticalAlignment),    
            new CssPropertyInstruction(    
                                    PropertyId.RightBorderWidth,
                                    null,
                                    MultiPropertyParsingMethods.ParseCompositeBorder),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(    
                                    PropertyId.FontFace,
                                    PropertyValueParsingMethods.ParseFontFace,     
                                    null),
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(    
                                    PropertyId.BottomBorderWidth,
                                    null,
                                    MultiPropertyParsingMethods.ParseCompositeBorder),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(    
                                    PropertyId.BackColor,
                                    PropertyValueParsingMethods.ParseColorCss,     
                                    null),
            new CssPropertyInstruction(    
                                    PropertyId.TopBorderStyle,
                                    PropertyValueParsingMethods.ParseBorderStyle,     
                                    null),
            new CssPropertyInstruction(    
                                    PropertyId.TableShowEmptyCells,
                                    PropertyValueParsingMethods.ParseEmptyCells,     
                                    null),
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(    
                                    PropertyId.TextAlignment,
                                    PropertyValueParsingMethods.ParseTextAlignment,     
                                    null),
            new CssPropertyInstruction(    
                                    PropertyId.Bold,
                                    PropertyValueParsingMethods.ParseFontWeight,     
                                    null),
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(    
                                    PropertyId.TableCaptionSideTop,
                                    PropertyValueParsingMethods.ParseCaptionSide,     
                                    null),
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(    
                                    PropertyId.LeftMargin,
                                    PropertyValueParsingMethods.ParseLength,     
                                    null),
            new CssPropertyInstruction(    
                                    PropertyId.BorderWidths,
                                    PropertyValueParsingMethods.ParseBorderWidth,     
                                    null),
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(    
                                    PropertyId.UnicodeBiDi,
                                    PropertyValueParsingMethods.ParseUnicodeBiDi,     
                                    null),
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(    
                                    PropertyId.Paddings,
                                    null,
                                    MultiPropertyParsingMethods.ParseCompositeNonNegativeLength),    
            new CssPropertyInstruction(    
                                    PropertyId.BottomBorderWidth,
                                    PropertyValueParsingMethods.ParseBorderWidth,     
                                    null),
            new CssPropertyInstruction(    
                                    PropertyId.Visible,
                                    PropertyValueParsingMethods.ParseVisibility,     
                                    null),
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(    
                                    PropertyId.ReadOnly,
                                    PropertyValueParsingMethods.ParseTableLayout,     
                                    null),
            new CssPropertyInstruction(    
                                    PropertyId.LeftBorderColor,
                                    PropertyValueParsingMethods.ParseColorCss,     
                                    null),
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(    
                                    PropertyId.Height,
                                    PropertyValueParsingMethods.ParseNonNegativeLength,     
                                    null),
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(    
                                    PropertyId.TopMargin,
                                    null,
                                    MultiPropertyParsingMethods.ParseCompositeLength),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(    
                                    PropertyId.BottomBorderStyle,
                                    PropertyValueParsingMethods.ParseBorderStyle,     
                                    null),
            new CssPropertyInstruction(    
                                    PropertyId.TopBorderColor,
                                    null,
                                    MultiPropertyParsingMethods.ParseCompositeColor),    
            new CssPropertyInstruction(    
                                    PropertyId.Null,
                                    null,
                                    MultiPropertyParsingMethods.ParseCssTextDecoration),    
            new CssPropertyInstruction(    
                                    PropertyId.Display,
                                    PropertyValueParsingMethods.ParseDisplay,     
                                    null),
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(    
                                    PropertyId.BottomMargin,
                                    PropertyValueParsingMethods.ParseLength,     
                                    null),
            new CssPropertyInstruction(    
                                    PropertyId.TopBorderStyle,
                                    null,
                                    MultiPropertyParsingMethods.ParseCompositeBorderStyle),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(    
                                    PropertyId.Null,
                                    null,
                                    MultiPropertyParsingMethods.ParseCompositeAllBorders),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(    
                                    PropertyId.Width,
                                    PropertyValueParsingMethods.ParseNonNegativeLength,     
                                    null),
            new CssPropertyInstruction(    
                                    PropertyId.FontColor,
                                    PropertyValueParsingMethods.ParseColorCss,     
                                    null),
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(    
                                    PropertyId.TableBorderCollapse,
                                    PropertyValueParsingMethods.ParseBorderCollapse,     
                                    null),
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(    
                                    PropertyId.TableBorderSpacingVertical,
                                    null,
                                    MultiPropertyParsingMethods.ParseCompoundBorderSpacing),    
            new CssPropertyInstruction(    
                                    PropertyId.Null,
                                    null,
                                    MultiPropertyParsingMethods.ParseCssTextTransform),    
            new CssPropertyInstruction(    
                                    PropertyId.RightBorderWidth,
                                    PropertyValueParsingMethods.ParseBorderWidth,     
                                    null),
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(    
                                    PropertyId.FirstLineIndent,
                                    PropertyValueParsingMethods.ParseLength,     
                                    null),
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(    
                                    PropertyId.BottomBorderColor,
                                    PropertyValueParsingMethods.ParseColorCss,     
                                    null),
            new CssPropertyInstruction(    
                                    PropertyId.RightMargin,
                                    PropertyValueParsingMethods.ParseLength,     
                                    null),
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(    
                                    PropertyId.RightPadding,
                                    PropertyValueParsingMethods.ParseNonNegativeLength,     
                                    null),
            new CssPropertyInstruction(    
                                    PropertyId.RightBorderStyle,
                                    PropertyValueParsingMethods.ParseBorderStyle,     
                                    null),
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(    
                                    PropertyId.BackColor,
                                    null,
                                    MultiPropertyParsingMethods.ParseCompositeBackground),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(    
                                    PropertyId.BorderWidths,
                                    null,
                                    MultiPropertyParsingMethods.ParseCompositeBorderWidth),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(    
                                    PropertyId.TopBorderColor,
                                    PropertyValueParsingMethods.ParseColorCss,     
                                    null),
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(    
                                    PropertyId.RightToLeft,
                                    PropertyValueParsingMethods.ParseDirection,     
                                    null),
            new CssPropertyInstruction(    
                                    PropertyId.SmallCaps,
                                    PropertyValueParsingMethods.ParseFontVariant,     
                                    null),
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(    
                                    PropertyId.FontSize,
                                    null,
                                    MultiPropertyParsingMethods.ParseCompositeFont),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(    
                                    PropertyId.RightBorderColor,
                                    PropertyValueParsingMethods.ParseColorCss,     
                                    null),
            new CssPropertyInstruction(    
                                    PropertyId.Italic,
                                    PropertyValueParsingMethods.ParseFontStyle,     
                                    null),
            new CssPropertyInstruction(    
                                    PropertyId.TopMargin,
                                    PropertyValueParsingMethods.ParseLength,     
                                    null),
            new CssPropertyInstruction(    
                                    PropertyId.LeftBorderWidth,
                                    null,
                                    MultiPropertyParsingMethods.ParseCompositeBorder),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(    
                                    PropertyId.LeftBorderWidth,
                                    PropertyValueParsingMethods.ParseBorderWidth,     
                                    null),
            new CssPropertyInstruction(    
                                    PropertyId.BottomPadding,
                                    PropertyValueParsingMethods.ParseNonNegativeLength,     
                                    null),
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(    
                                    PropertyId.FontSize,
                                    PropertyValueParsingMethods.ParseCssFontSize,     
                                    null),
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(    
                                    PropertyId.LeftBorderStyle,
                                    PropertyValueParsingMethods.ParseBorderStyle,     
                                    null),
            new CssPropertyInstruction(    
                                    PropertyId.Paddings,
                                    PropertyValueParsingMethods.ParseNonNegativeLength,     
                                    null),
            new CssPropertyInstruction(    
                                    PropertyId.LeftPadding,
                                    PropertyValueParsingMethods.ParseNonNegativeLength,     
                                    null),
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(),    
            new CssPropertyInstruction(    
                                    PropertyId.BorderWidths,
                                    null,
                                    MultiPropertyParsingMethods.ParseCompositeBorder),    
            new CssPropertyInstruction(),    
        };
    }
}
