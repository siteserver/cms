// ***************************************************************
// <copyright file="HtmlFormatConverters.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters.Internal.Html
{
    using System;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;
    using Data.Internal;
    using Globalization;
    using Css;
    
    using Format;
    using Strings = CtsResources.TextConvertersStrings;

    

    internal class HtmlFormatConverter : FormatConverter, IProducerConsumer, IRestartable, IDisposable
    {
        

        protected bool convertFragment;

        protected HtmlNormalizingParser parser;

        private FormatOutput output;

        protected HtmlToken token;

        protected bool treatNbspAsBreakable;

        protected bool insideComment;
        protected bool insideStyle;
        protected bool insidePre;

        protected int insideList;

        protected int temporarilyClosedLevels;

        protected char[] literalBuffer = new char[2];

        private CssParser cssParser;
        private ConverterBufferInput cssParserInput;

        private Dictionary<StyleSelector, int> styleDictionary;
        private int[] styleHandleIndex;
        private int styleHandleIndexCount;

        private ScratchBuffer scratch;

        private Property[] parsedProperties;    

        private PropertyValueParsingMethod attributeParsingMethod;
        private PropertyId attributePropertyId;

        private IProgressMonitor progressMonitor;

        

        public HtmlFormatConverter(
                    HtmlNormalizingParser parser, 
                    FormatOutput output,
                    bool testTreatNbspAsBreakable, 
                    Stream traceStream, 
                    bool traceShowTokenNum, 
                    int traceStopOnTokenNum,
                    Stream formatConverterTraceStream,
                    IProgressMonitor progressMonitor) :
            base(formatConverterTraceStream)
        {

            this.parser = parser;
            this.parser.SetRestartConsumer(this);

            this.progressMonitor = progressMonitor;

            this.output = output;

            if (this.output != null)
            {
                this.output.Initialize(
                        Store,
                        SourceFormat.Html,
                        "converted from html");
            }

            treatNbspAsBreakable = testTreatNbspAsBreakable;

            InitializeDocument();
        }

        

        public HtmlFormatConverter(
                    HtmlNormalizingParser parser, 
                    FormatStore formatStore,
                    bool fragment,
                    bool testTreatNbspAsBreakable, 
                    Stream traceStream, 
                    bool traceShowTokenNum, 
                    int traceStopOnTokenNum,
                    Stream formatConverterTraceStream,
                    IProgressMonitor progressMonitor) :
            base(formatStore, formatConverterTraceStream)
        {

            this.parser = parser;
            this.parser.SetRestartConsumer(this);

            this.progressMonitor = progressMonitor;

            treatNbspAsBreakable = testTreatNbspAsBreakable;

            convertFragment = fragment;

            if (!fragment)
            {
                InitializeDocument();
            }
        }

        

        public FormatNode Initialize(string fragment)
        {
            InternalDebug.Assert(convertFragment);

            parser.Initialize(fragment, false);

            
            var fragmentNode = InitializeFragment();

            Initialize();

            return fragmentNode;
        }

        

        private void Initialize()
        {
            insideComment = false;
            insidePre = false;
            insideStyle = false;
            insideList = 0;

            temporarilyClosedLevels = 0;

            if (styleDictionary != null)
            {
                styleDictionary.Clear();
            }

            if (cssParserInput != null)
            {
                cssParserInput.Reset();
                cssParser.Reset();
            }
        }

        

        bool IRestartable.CanRestart()
        {
            return output == null || ((IRestartable)output).CanRestart();
        }

        

        void IRestartable.Restart()
        {
            InternalDebug.Assert(!convertFragment);
            InternalDebug.Assert(((IRestartable)this).CanRestart());

            if (output != null)
            {
                ((IRestartable)output).Restart();
            }

            
            store.Initialize();

            
            InitializeDocument();

            Initialize();
        }

        

        void IRestartable.DisableRestart()
        {
            if (output != null)
            {
                ((IRestartable)output).DisableRestart();
            }
        }

        

        public override void Run()
        {
            if (output != null && MustFlush)
            {
                if (CanFlush)
                {
                    FlushOutput();
                }
            }
            else if (!EndOfFile)
            {
                var tokenId = parser.Parse();

                if (HtmlTokenId.None != tokenId)
                {
                    Process(tokenId);
                }
            }
        }

        

        public bool Flush()
        {
            Run();
            return EndOfFile && !MustFlush;
        }

        
        

        void IDisposable.Dispose()
        {
            if (parser != null /*&& this.parser is IDisposable*/)
            {
                ((IDisposable)parser).Dispose();
            }

            if (token != null && token is IDisposable)
            {
                ((IDisposable)token).Dispose();
            }

            parser = null;

            token = null;
            literalBuffer = null;

            GC.SuppressFinalize(this);
        }

        

        private bool CanFlush => output.CanAcceptMoreOutput;


        private bool FlushOutput()
        {
            InternalDebug.Assert(MustFlush);

            if (output.Flush())
            {
                MustFlush = false;
                return true;
            }

            return false;
        }

        
        protected void Process(HtmlTokenId tokenId)
        {
            token = parser.Token;

            switch (tokenId)
            {
                

                case HtmlTokenId.Tag:

                    

                    if (token.TagIndex <= HtmlTagIndex.Unknown)
                    {
                        if (insideStyle && token.TagIndex == HtmlTagIndex._COMMENT)
                        {
                            
                            token.Text.WriteTo(cssParserInput);
                        }

                        break;
                    }

                    var tagDef = GetTagDefinition(token.TagIndex);

                    if (!token.IsEndTag)
                    {
                        

                        if (token.IsTagBegin)
                        {
                            

                            PushElement(tagDef, token.IsEmptyScope);
                        }

                        ProcessStartTagAttributes(tagDef);
                    }
                    else
                    {
                        

                        if (token.IsTagEnd)
                        {
                            PopElement(buildStackTop - 1 - temporarilyClosedLevels, token.Argument != 1);
                        }
                    }
                    break;

                

                case HtmlTokenId.Text:

                    if (insideStyle)
                    {
                        
                        token.Text.WriteTo(cssParserInput);
                    }
                    else if (insideComment)
                    {
                    }
                    else if (insidePre)
                    {
                        ProcessPreformatedText();
                    }
                    else
                    {
                        ProcessText();
                    }
                    break;

                

                case HtmlTokenId.OverlappedClose:

                    
                    temporarilyClosedLevels = token.Argument;
                    break;

                case HtmlTokenId.OverlappedReopen:

                    InternalDebug.Assert(temporarilyClosedLevels == token.Argument);

                    
                    temporarilyClosedLevels = 0;
                    break;

                

                case HtmlTokenId.Restart:

                    break;

                

                case HtmlTokenId.EncodingChange:

                    if (output != null && output.OutputCodePageSameAsInput)
                    {
                        var codePage = token.Argument;

                        
                        #if DEBUG
                        Encoding newOutputEncoding;
                        InternalDebug.Assert(Charset.TryGetEncoding(codePage, out newOutputEncoding));
                        #endif
                        output.OutputEncoding = Charset.GetEncoding(codePage);
                    }

                    break;

                

                case HtmlTokenId.EndOfFile:

                    
                    CloseAllContainersAndSetEOF();
                    break;
            }
        }

        
        private int PushElement(HtmlDtd.TagDefinition tagDef, bool emptyScope)
        {
            

            var stackPos = buildStackTop;

            var last = FormatConverterContainer.Null;

            if (HtmlConverterData.tagInstructions[(int)tagDef.tagIndex].ContainerType != FormatContainerType.Null)
            {
                var defaultStyle = HtmlConverterData.tagInstructions[(int)tagDef.tagIndex].DefaultStyle;

                last = OpenContainer(
                        HtmlConverterData.tagInstructions[(int)tagDef.tagIndex].ContainerType,
                        emptyScope,
                        HtmlConverterData.tagInstructions[(int)tagDef.tagIndex].InheritanceMaskIndex,
                        GetStyle(defaultStyle), 
                        tagDef.tagIndex);      
            }
            else if (!emptyScope)
            {
                
                

                last = OpenContainer(
                        FormatContainerType.PropertyContainer,
                        false,
                        HtmlConverterData.tagInstructions[(int)tagDef.tagIndex].InheritanceMaskIndex,
                        FormatStyle.Null,
                        tagDef.tagIndex);      
            }

            InternalDebug.Assert(last.IsNull || last == Last);

            if (!last.IsNull)
            {
                

                switch (tagDef.tagIndex)
                {
                    case HtmlTagIndex.Title:
                            insideComment = !token.IsEndTag;
                            break;

                    case HtmlTagIndex.Comment:
                    case HtmlTagIndex.Script:
                    case HtmlTagIndex.Xml:

                            insideComment = true;
                            break;

                    case HtmlTagIndex.Style:

                            if (null == cssParserInput)
                            {
                                cssParserInput = new ConverterBufferInput(CssParser.MaxCssLength, progressMonitor);
                                cssParser = new CssParser(cssParserInput, 1024, false);
                            }

                            insideStyle = true;
                            break;

                    case HtmlTagIndex.Pre:
                    case HtmlTagIndex.PlainText:
                    case HtmlTagIndex.Listing:
                    case HtmlTagIndex.Xmp:

                            insidePre = true;
                            
                            goto case HtmlTagIndex.P;

                    case HtmlTagIndex.OL:
                    case HtmlTagIndex.UL:
                    case HtmlTagIndex.DL:

                            var previousInsideList = insideList;
                            insideList ++;
                            if (previousInsideList == 0)
                            {
                                
                                goto case HtmlTagIndex.P;
                            }
                            break;

                    case HtmlTagIndex.P:
                    case HtmlTagIndex.BlockQuote:
                    case HtmlTagIndex.H1:
                    case HtmlTagIndex.H2:
                    case HtmlTagIndex.H3:
                    case HtmlTagIndex.H4:
                    case HtmlTagIndex.H5:
                    case HtmlTagIndex.H6:

                            
                            InternalDebug.Assert(!LastNode.IsNull);

                            
                            
                            

                            if (!LastNode.FirstChild.IsNull ||
                                (LastNode.NodeType != FormatContainerType.Document &&
                                LastNode.NodeType != FormatContainerType.Fragment &&
                                LastNode.NodeType != FormatContainerType.TableCell))
                            {
                                last.SetProperty(PropertyPrecedence.TagDefault, PropertyId.TopMargin, new PropertyValue(LengthUnits.Points, 14));

                                
                                
                                
                                last.SetProperty(PropertyPrecedence.TagDefault, PropertyId.BottomMargin, new PropertyValue(LengthUnits.Points, 14));
                            }
                            break;
                }

                
                FindAndApplyStyle(new StyleSelector(tagDef.nameIndex, null, null));
            }

            return stackPos;
        }

        
        private void PopElement(int stackPos, bool explicitClose/*whether the element was closed automatically*/)
        {
            
            var node = Last.Node;

            switch (buildStack[stackPos].tagIndex)
            {
                case HtmlTagIndex.Title:
                        insideComment = !token.IsEndTag;
                        break;

                case HtmlTagIndex.Comment:
                case HtmlTagIndex.Script:
                case HtmlTagIndex.Xml:

                        insideComment = false;
                        break;

                case HtmlTagIndex.Style:

                        if (insideStyle)
                        {
                            
                            ProcessStylesheet();
                            insideStyle = false;
                        }
                        break;

                case HtmlTagIndex.Pre:
                case HtmlTagIndex.PlainText:
                case HtmlTagIndex.Listing:
                case HtmlTagIndex.Xmp:

                        insidePre = false;
                        break;

                case HtmlTagIndex.OL:
                case HtmlTagIndex.UL:
                case HtmlTagIndex.DL:

                        InternalDebug.Assert(insideList != 0);
                        insideList --;
                        break;
            }

            if (stackPos == buildStackTop - 1)
            {
                CloseContainer();
            }
            else
            {
                CloseOverlappingContainer(buildStackTop - 1 - stackPos);
            }

            if (!node.IsNull)
            {
                if (node.NodeType == FormatContainerType.Table)
                {
                    while (!node.LastChild.IsNull && node.LastChild.NodeType == FormatContainerType.TableRow)
                    {
                        var emptyRow = true;

                        var height = node.LastChild.GetProperty(PropertyId.Height);
                        if (!height.IsNull)
                        {
                            break;
                        }

                        foreach (var cellNode in node.LastChild.Children)
                        {
                            if (!cellNode.FirstChild.IsNull)
                            {
                                emptyRow = false;
                                break;
                            }
                        }

                        if (!emptyRow)
                        {
                            break;
                        }

                        
                        node.LastChild.RemoveFromParent();
                    }
                }
            }
        }

        
        protected override FormatNode GetParentForNewNode(FormatNode node, FormatNode parent, int stackPos, out int propContainerInheritanceStopLevel)
        {
            switch (node.NodeType)
            {
                case FormatContainerType.TableRow:

                        
                        
                        if (parent.NodeType != FormatContainerType.Table)
                        {
                            parent = FindStackAncestor(stackPos, FormatContainerType.Table);
                            
                            InternalDebug.Assert(!parent.IsNull);
                        }
                        propContainerInheritanceStopLevel = stackPos;
                        break;

                case FormatContainerType.TableCell:

                        
                        if (parent.NodeType != FormatContainerType.TableRow)
                        {
                            parent = FindStackAncestor(stackPos, FormatContainerType.TableRow);
                            
                            InternalDebug.Assert(!parent.IsNull);
                        }
                        propContainerInheritanceStopLevel = stackPos;
                        break;

                    case FormatContainerType.TableExtraContent:
                        {
                            
                            
                            
                            

                            propContainerInheritanceStopLevel = stackPos;

                            var newParent = parent.NodeType != FormatContainerType.Table &&
                                                    parent.NodeType != FormatContainerType.TableRow &&
                                                    parent.NodeType != FormatContainerType.TableColumnGroup ?
                                                        parent :
                                                        FindStackParentForExtraContent(stackPos, out propContainerInheritanceStopLevel);

                            if (newParent.IsNull)
                            {
                                

                                var table = parent.NodeType == FormatContainerType.Table ? parent : FindStackAncestor(stackPos, FormatContainerType.Table);
                                
                                InternalDebug.Assert(!table.IsNull);

                                var tableContainer = table.Parent;
                                var extraContentContainer = FormatNode.Null;

                                if (tableContainer.NodeType != FormatContainerType.TableContainer)
                                {
                                    
                                    

                                    tableContainer = store.AllocateNode(FormatContainerType.TableContainer, table.BeginTextPosition);
                                    tableContainer.InheritanceMaskIndex = FormatStoreData.DefaultInheritanceMaskIndex.Any;

                                    table.InsertSiblingAfter(tableContainer);
                                    table.RemoveFromParent();
                                    tableContainer.AppendChild(table);

                                    tableContainer.SetOnRightEdge();

                                    extraContentContainer = store.AllocateNode(FormatContainerType.TableExtraContent);
                                    extraContentContainer.InheritanceMaskIndex = FormatStoreData.DefaultInheritanceMaskIndex.Any;
                                    extraContentContainer.SetOutOfOrder();

                                    if (table.OnLeftEdge)
                                    {
                                        
                                        
                                        
                                        InternalDebug.Assert(false);

                                        tableContainer.SetOnLeftEdge();

                                        tableContainer.AppendChild(extraContentContainer);
                                    }
                                    else
                                    {
                                        tableContainer.PrependChild(extraContentContainer);
                                    }
                                }
                                else
                                {
                                    foreach (var nodeT in tableContainer.Children)
                                    {
                                        if (nodeT.NodeType == FormatContainerType.TableExtraContent)
                                        {
                                            extraContentContainer = nodeT;
                                        }
                                    }
                                }

                                newParent = extraContentContainer;
                            }

                            parent = newParent;
                        }
                        break;

                    case FormatContainerType.TableCaption:
                        {
                            propContainerInheritanceStopLevel = stackPos;

                            var table = parent.NodeType == FormatContainerType.Table ? parent : FindStackAncestor(stackPos, FormatContainerType.Table);
                            
                            InternalDebug.Assert(!table.IsNull);

                            var tableCaptionContainer = table.FirstChild;

                            if (tableCaptionContainer.IsNull ||
                                tableCaptionContainer.NodeType != FormatContainerType.TableCaption)
                            {
                                tableCaptionContainer = store.AllocateNode(FormatContainerType.TableCaption);
                                tableCaptionContainer.InheritanceMaskIndex = FormatStoreData.DefaultInheritanceMaskIndex.Any;
                                tableCaptionContainer.SetOutOfOrder();
                                table.PrependChild(tableCaptionContainer);
                            }

                            parent = tableCaptionContainer;
                        }
                        break;

                    case FormatContainerType.TableColumn:
                        {
                            var newParent = parent.NodeType == FormatContainerType.TableColumnGroup ? parent : FindStackParentForColumn(stackPos);

                            if (!newParent.IsNull)
                            {
                                propContainerInheritanceStopLevel = stackPos;
                                parent = newParent;
                                break;
                            }
                            goto case FormatContainerType.TableColumnGroup;
                        }

                    case FormatContainerType.TableColumnGroup:
                        {
                            propContainerInheritanceStopLevel = stackPos;

                            var table = parent.NodeType == FormatContainerType.Table ? parent : FindStackAncestor(stackPos, FormatContainerType.Table);
                            
                            InternalDebug.Assert(!table.IsNull);

                            var tableCaptionContainer = FormatNode.Null;
                            var tableDefinitionContainer = table.FirstChild;

                            if (!tableDefinitionContainer.IsNull &&
                                tableDefinitionContainer.NodeType == FormatContainerType.TableCaption)
                            {
                                tableCaptionContainer = tableDefinitionContainer;
                                tableDefinitionContainer = tableDefinitionContainer.NextSibling;
                            }

                            if (tableDefinitionContainer.IsNull ||
                                tableDefinitionContainer.NodeType != FormatContainerType.TableDefinition)
                            {
                                tableDefinitionContainer = store.AllocateNode(FormatContainerType.TableDefinition);
                                tableDefinitionContainer.InheritanceMaskIndex = FormatStoreData.DefaultInheritanceMaskIndex.Any;
                                tableDefinitionContainer.SetOutOfOrder();

                                if (tableCaptionContainer.IsNull)
                                {
                                    table.PrependChild(tableDefinitionContainer);
                                }
                                else
                                {
                                    tableCaptionContainer.InsertSiblingAfter(tableDefinitionContainer);
                                }
                            }

                            parent = tableDefinitionContainer;
                        }
                        break;

                default:

                        if (parent.NodeType == FormatContainerType.Table ||
                            parent.NodeType == FormatContainerType.TableRow ||
                            parent.NodeType == FormatContainerType.TableColumnGroup)
                        {
                            goto case FormatContainerType.TableExtraContent;
                        }

                        propContainerInheritanceStopLevel = DefaultPropContainerInheritanceStopLevel(stackPos);
                        break;
            }

            return parent;
        }

        
        protected override FormatContainerType FixContainerType(FormatContainerType type, StyleBuildHelper styleBuilderWithContainerProperties)
        {
            
            
            
            

            

            var display = styleBuilderWithContainerProperties.GetProperty(PropertyId.Display);
            if (display.IsEnum)
            {
                switch ((Display)display.Enum)
                {
                    case Display.None:

                            
                            
                            
                            break;

                    case Display.Inline:

                            
                            if (type == FormatContainerType.Block)
                            {
                                 type = FormatContainerType.Inline;
                            }
                            break;

                    case Display.Block:

                            
                            if (type == FormatContainerType.PropertyContainer ||
                                type == FormatContainerType.Inline)
                            {
                                 type = FormatContainerType.Block;
                            }
                            break;
/*
                    
                    

                    case Display.InlineBlock:
                    case Display.ListItem:
                    case Display.TableHeaderGroup:
                    case Display.TableFooterGroup:

                    

                    case Display.RunIn:
                    case Display.Table:
                    case Display.InlineTable:
                    case Display.TableRowGroup:
                    case Display.TableRow:
                    case Display.TableColumnGroup:
                    case Display.TableColumn:
                    case Display.TableCell:
                    case Display.TableCaption:
*/
                }
            }

            if (type == FormatContainerType.PropertyContainer)
            {
                
                
                

                var unicodeBiDi = styleBuilderWithContainerProperties.GetProperty(PropertyId.UnicodeBiDi);
                if (unicodeBiDi.IsEnum)
                {
                    if ((UnicodeBiDi)unicodeBiDi.Enum != UnicodeBiDi.Normal)
                    {
                        type = FormatContainerType.Inline;
                    }
                }
            }

            if (type == FormatContainerType.HyperLink)
            {
                var href = styleBuilderWithContainerProperties.GetProperty(PropertyId.HyperlinkUrl);
                if (href.IsNull)
                {
                    type = FormatContainerType.Bookmark;
                }
            }

            return type;
        }

        
        private FormatNode FindStackAncestor(int stackPosOfNewContainer, FormatContainerType type)
        {
            for (var i = stackPosOfNewContainer - 1; i >= 0; i--)
            {
                if (buildStack[i].type == type)
                {
                    InternalDebug.Assert(buildStack[i].node != 0);
                    return store.GetNode(buildStack[i].node);
                }
            }

            return FormatNode.Null;
        }

        
        private FormatNode FindStackParentForExtraContent(int stackPosOfNewContainer, out int ancestorContainerLevel)
        {
            var tableSeen = false;
            for (var i = stackPosOfNewContainer - 1; i >= 0; i--)
            {
                if (buildStack[i].node != 0)
                {
                    if (buildStack[i].type == FormatContainerType.Table)
                    {
                        tableSeen = true;
                        #if false
                        
                        ancestorContainerLevel = i + 1;
                        break;
                        #endif
                    }
                    else if (buildStack[i].type != FormatContainerType.TableRow &&
                        buildStack[i].type != FormatContainerType.TableColumnGroup)
                    {
                        ancestorContainerLevel = i + 1;
                        return tableSeen ? FormatNode.Null : store.GetNode(buildStack[i].node);
                    }
                }
            }

            ancestorContainerLevel = stackPosOfNewContainer;
            return FormatNode.Null;
        }

        
        private FormatNode FindStackParentForColumn(int stackPosOfNewContainer)
        {
            for (var i = stackPosOfNewContainer - 1; i >= 0; i--)
            {
                if (buildStack[i].node != 0)
                {
                    if (buildStack[i].type == FormatContainerType.Table)
                    {
                        
                        break;
                    }

                    if (buildStack[i].type == FormatContainerType.TableColumnGroup)
                    {
                        return store.GetNode(buildStack[i].node);
                    }
                }
            }

            return FormatNode.Null;
        }

        
        private bool StartTagHasAttribute(HtmlNameIndex attributeNameIndex)
        {
            foreach (var attr in token.Attributes)
            {
                if (attr.NameIndex == attributeNameIndex)
                {
                    return true;
                }
            }

            return false;
        }

        
        private void ProcessStartTagAttributes(HtmlDtd.TagDefinition tagDef)
        {
            if (HtmlConverterData.tagInstructions[(int)token.TagIndex].ContainerType != FormatContainerType.Null)
            {
                token.Attributes.Rewind();

                foreach (var attr in token.Attributes)
                {
                    if (attr.NameIndex == HtmlNameIndex.Style)
                    {
                        
                        ProcessStyleAttribute(tagDef, attr);
                    }
                    else if (attr.NameIndex == HtmlNameIndex.Id)
                    {
                        

                        var id = attr.Value.GetString(60);

                        FindAndApplyStyle(new StyleSelector(token.NameIndex, null, id));

                        FindAndApplyStyle(new StyleSelector(HtmlNameIndex.Unknown, null, id));
                    }
                    else if (attr.NameIndex == HtmlNameIndex.Class)
                    {
                        

                        var cls = attr.Value.GetString(60);

                        FindAndApplyStyle(new StyleSelector(token.NameIndex, cls, null));

                        FindAndApplyStyle(new StyleSelector(HtmlNameIndex.Unknown, cls, null));

                        if (!LastNonEmpty.Node.IsNull)
                        {
                            if (cls.Equals("EmailQuote", StringComparison.OrdinalIgnoreCase))
                            {
                                Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.QuotingLevelDelta, new PropertyValue(PropertyType.Integer, 1/*this.quotingLevel*/));
                            }
                        }
                    }
                    else if (attr.NameIndex != HtmlNameIndex.Unknown)
                    {
                        

                        ProcessNonStyleAttribute(attr);
                    }
                }
            }

            if (token.IsTagEnd)
            {
                

                if (token.TagIndex == HtmlTagIndex.BR)
                {
                    AddLineBreak(1);
                }
            }
        }

        
        private void ProcessPreformatedText()
        {
            foreach (var run in token.Runs)
            {
                if (run.IsTextRun)
                {
                    if (run.IsAnyWhitespace)
                    {
                        switch (run.TextType)
                        {
                            case RunTextType.NewLine:

                                    
                                    AddLineBreak(1);
                                    break;

                            case RunTextType.Space:
                            default:
#if false
                                    if (this.treatNbspAsBreakable)
                                    {
#endif
                                        AddSpace(run.Length);
#if false
                                    }
                                    else
                                    {
                                        this.AddNbsp(run.Length);
                                    }
#endif
                                    break;

                            case RunTextType.Tabulation:

                                    AddTabulation(run.Length);
                                    break;
                        }
                    }
                    else if (run.TextType == RunTextType.Nbsp)
                    {
                        if (treatNbspAsBreakable)
                        {
                            AddSpace(run.Length);
                        }
                        else
                        {
                            AddNbsp(run.Length);
                        }
                    }
                    else 
                    {
                        OutputNonspace(run);
                    }
                }
            }
        }

        
        private void ProcessText()
        {
            foreach (var run in token.Runs)
            {
                if (run.IsTextRun)
                {
                    if (run.IsAnyWhitespace)
                    {
                        AddSpace(1);
                    }
                    else if (run.TextType == RunTextType.Nbsp)
                    {
                        if (treatNbspAsBreakable)
                        {
                            AddSpace(run.Length);
                        }
                        else
                        {
                            AddNbsp(run.Length);
                        }
                    }
                    else 
                    {
                        OutputNonspace(run);
                    }
                }
            }
        }

        
        private void OutputNonspace(TokenRun run)
        {
            

            if (run.IsLiteral)
            {
                AddNonSpaceText(literalBuffer, 0, run.ReadLiteral(literalBuffer));
            }
            else
            {
                AddNonSpaceText(run.RawBuffer, run.RawOffset, run.RawLength);
            }
        }

        
        private static HtmlDtd.TagDefinition GetTagDefinition(HtmlTagIndex tagIndex)
        {
            return tagIndex != HtmlTagIndex._NULL ? HtmlDtd.tags[(int)tagIndex] : null;
        }

        
        private void ProcessStyleAttribute(HtmlDtd.TagDefinition tagDef, HtmlAttribute attr)
        {
            if (attr.IsAttrBegin)
            {
                if (null == cssParserInput)
                {
                    cssParserInput = new ConverterBufferInput(CssParser.MaxCssLength, progressMonitor);
                    cssParser = new CssParser(cssParserInput, 1024, false);
                }

                cssParser.SetParseMode(CssParseMode.StyleAttribute);
            }

            attr.Value.Rewind();
            attr.Value.WriteTo(cssParserInput);

            if (attr.IsAttrEnd)
            {
                CssTokenId cssTokenId;
                do
                {
                    cssTokenId = cssParser.Parse();

                    if (CssTokenId.Declarations == cssTokenId && cssParser.Token.Properties.ValidCount != 0)
                    {
                        cssParser.Token.Properties.Rewind();

                        foreach (var property in cssParser.Token.Properties)
                        {
                            if (property.NameId != CssNameIndex.Unknown)
                            {
                                if (null != HtmlConverterData.cssPropertyInstructions[(int)property.NameId].ParsingMethod)
                                {
                                    PropertyValue value;

                                    if (!property.Value.IsEmpty && property.Value.IsContiguous)
                                    {
                                        value = HtmlConverterData.cssPropertyInstructions[(int)property.NameId].ParsingMethod(property.Value.ContiguousBufferString, this);
                                    }
                                    else
                                    {
                                        scratch.AppendCssPropertyValue(property, HtmlSupport.MaxCssPropertySize);
                                        value = HtmlConverterData.cssPropertyInstructions[(int)property.NameId].ParsingMethod(scratch.BufferString, this);
                                        scratch.Reset();
                                    }

                                    if (!value.IsNull)
                                    {
                                        var propId = HtmlConverterData.cssPropertyInstructions[(int)property.NameId].PropertyId;

                                        Last.SetProperty(PropertyPrecedence.InlineStyle, propId, value);
                                    }
                                }
                                else if (null != HtmlConverterData.cssPropertyInstructions[(int)property.NameId].MultiPropertyParsingMethod)
                                {
                                    

                                    if (parsedProperties == null)
                                    {
                                        
                                        parsedProperties = new Property[12];
                                    }

                                    int numParsedProperties;

                                    if (!property.Value.IsEmpty && property.Value.IsContiguous)
                                    {
                                        HtmlConverterData.cssPropertyInstructions[(int)property.NameId].MultiPropertyParsingMethod(property.Value.ContiguousBufferString, this, HtmlConverterData.cssPropertyInstructions[(int)property.NameId].PropertyId, parsedProperties, out numParsedProperties);
                                    }
                                    else
                                    {
                                        scratch.AppendCssPropertyValue(property, HtmlSupport.MaxCssPropertySize);
                                        HtmlConverterData.cssPropertyInstructions[(int)property.NameId].MultiPropertyParsingMethod(scratch.BufferString, this, HtmlConverterData.cssPropertyInstructions[(int)property.NameId].PropertyId, parsedProperties, out numParsedProperties);
                                        scratch.Reset();
                                    }

                                    if (numParsedProperties != 0)
                                    {
                                        Last.SetProperties(PropertyPrecedence.InlineStyle, parsedProperties, numParsedProperties);
                                    }
                                }
                            }
                        }
                    }
                }
                while (CssTokenId.EndOfFile != cssTokenId);

                cssParserInput.Reset();
                cssParser.Reset();
            }
        }

        
        private void ProcessNonStyleAttribute(HtmlAttribute attr)
        {
            if (attr.IsAttrBegin)
            {
                if (token.TagIndex == HtmlTagIndex.A && attr.NameIndex == HtmlNameIndex.Href)
                {
                    
                    Last.SetProperty(PropertyPrecedence.TagDefault, PropertyId.Underline, new PropertyValue(true));
                    Last.SetProperty(PropertyPrecedence.TagDefault, PropertyId.FontColor, new PropertyValue(new RGBT(0,0,255)));
                }

                attributeParsingMethod = null;

                if (HtmlConverterData.tagInstructions[(int)token.TagIndex].AttributeInstructions != null)
                {
                    for (var i = 0; i < HtmlConverterData.tagInstructions[(int)token.TagIndex].AttributeInstructions.Length; i++)
                    {
                        if (attr.NameIndex == HtmlConverterData.tagInstructions[(int)token.TagIndex].AttributeInstructions[i].AttributeNameId)
                        {
                            attributeParsingMethod = HtmlConverterData.tagInstructions[(int)token.TagIndex].AttributeInstructions[i].ParsingMethod;
                            attributePropertyId = HtmlConverterData.tagInstructions[(int)token.TagIndex].AttributeInstructions[i].PropertyId;

                            if (attr.IsAttrEnd && !attr.Value.IsEmpty && attr.Value.IsContiguous)
                            {
                                

                                var value = attributeParsingMethod(attr.Value.ContiguousBufferString, this);

                                if (!value.IsNull)
                                {
                                    Last.SetProperty(PropertyPrecedence.NonStyle, attributePropertyId, value);
                                }

                                return;
                            }

                            break;
                        }
                    }
                }

                if (attributeParsingMethod == null)
                {
                    
                }
            }

            if (attributeParsingMethod == null)
            {
                return;
            }

            scratch.AppendHtmlAttributeValue(attr, HtmlSupport.MaxAttributeSize);

            if (attr.IsAttrEnd)
            {
                var value = attributeParsingMethod(scratch.BufferString, this);

                if (!value.IsNull)
                {
                    Last.SetProperty(PropertyPrecedence.NonStyle, attributePropertyId, value);
                }

                scratch.Reset();
            }
        }

        
        private void ProcessStylesheet()
        {
            cssParser.SetParseMode(CssParseMode.StyleTag);

            CssTokenId cssTokenId;
            do
            {
                cssTokenId = cssParser.Parse();

                if (CssTokenId.RuleSet == cssTokenId &&
                    cssParser.Token.Selectors.ValidCount != 0 &&
                    cssParser.Token.Properties.ValidCount != 0)
                {
                    var slotAllocated = false;
                    var msoNormalClassSelector = false;

                    cssParser.Token.Selectors.Rewind();

                    foreach (var selector in cssParser.Token.Selectors)
                    {
                        if (!selector.IsSimple)
                        {
                            
                            continue;
                        }

                        var selectorDescriptor = new StyleSelector();
                        var goodSelector = false;

                        if (selector.HasClassFragment)
                        {
                            if (selector.ClassType == CssSelectorClassType.Regular ||
                                selector.ClassType == CssSelectorClassType.Hash)
                            {
                                

                                var className = selector.ClassName.GetString(60);

                                
                                InternalDebug.Assert(className.Length > 0);

                                if (selector.ClassType == CssSelectorClassType.Regular)
                                {
                                    selectorDescriptor = new StyleSelector(selector.NameId, className, null);
                                    goodSelector = true;

                                    msoNormalClassSelector = msoNormalClassSelector || className.Equals("MsoNormal", StringComparison.OrdinalIgnoreCase);
                                }
                                else
                                {
                                    selectorDescriptor = new StyleSelector(selector.NameId, null, className);
                                    goodSelector = true;
                                }
                            }
                        }
                        else if (selector.NameId != HtmlNameIndex.Unknown)
                        {
                            
                            selectorDescriptor = new StyleSelector(selector.NameId, null, null);
                            goodSelector = true;
                        }
                        else
                        {
                            
                            
                            
                        }

                        

                        if (goodSelector && (slotAllocated || styleHandleIndexCount < HtmlSupport.MaxNumberOfNonInlineStyles))
                        {
                            if (!slotAllocated)
                            {
                                if (styleHandleIndex == null)
                                {
                                    InternalDebug.Assert(styleHandleIndexCount == 0);
                                    styleHandleIndex = new int[32];
                                }
                                else if (styleHandleIndexCount == styleHandleIndex.Length)
                                {
                                    var newIndex = new int[styleHandleIndexCount * 2];
                                    Array.Copy(styleHandleIndex, 0, newIndex, 0, styleHandleIndexCount);
                                    styleHandleIndex = newIndex;
                                }

                                styleHandleIndexCount ++;
                                slotAllocated = true;
                            }

                            if (styleDictionary == null)
                            {
                                styleDictionary = new Dictionary<StyleSelector, int>(new StyleSelectorComparer());
                            }

                            if (!styleDictionary.ContainsKey(selectorDescriptor))
                            {
                                styleDictionary.Add(selectorDescriptor, styleHandleIndexCount - 1);
                            }
                            else
                            {
                                styleDictionary[selectorDescriptor] = styleHandleIndexCount - 1;
                            }
                        }
                    }

                    if (slotAllocated)
                    {
                        StyleBuilder builder;

                        var style = RegisterStyle(false, out builder);

                        cssParser.Token.Properties.Rewind();

                        foreach (var property in cssParser.Token.Properties)
                        {
                            if (property.NameId != CssNameIndex.Unknown)
                            {
                                if (null != HtmlConverterData.cssPropertyInstructions[(int)property.NameId].ParsingMethod)
                                {
                                    

                                    PropertyValue value;

                                    if (!property.Value.IsEmpty && property.Value.IsContiguous)
                                    {
                                        value = HtmlConverterData.cssPropertyInstructions[(int)property.NameId].ParsingMethod(property.Value.ContiguousBufferString, this);
                                    }
                                    else
                                    {
                                        scratch.AppendCssPropertyValue(property, HtmlSupport.MaxCssPropertySize);
                                        value = HtmlConverterData.cssPropertyInstructions[(int)property.NameId].ParsingMethod(scratch.BufferString, this);
                                        scratch.Reset();
                                    }

                                    if (!value.IsNull)
                                    {
                                        builder.SetProperty(HtmlConverterData.cssPropertyInstructions[(int)property.NameId].PropertyId, value);
                                    }
                                }
                                else if (null != HtmlConverterData.cssPropertyInstructions[(int)property.NameId].MultiPropertyParsingMethod)
                                {
                                    

                                    if (parsedProperties == null)
                                    {
                                        
                                        parsedProperties = new Property[12];
                                    }

                                    int numParsedProperties;

                                    if (!property.Value.IsEmpty && property.Value.IsContiguous)
                                    {
                                        HtmlConverterData.cssPropertyInstructions[(int)property.NameId].MultiPropertyParsingMethod(property.Value.ContiguousBufferString, this, HtmlConverterData.cssPropertyInstructions[(int)property.NameId].PropertyId, parsedProperties, out numParsedProperties);
                                    }
                                    else
                                    {
                                        scratch.AppendCssPropertyValue(property, HtmlSupport.MaxCssPropertySize);
                                        HtmlConverterData.cssPropertyInstructions[(int)property.NameId].MultiPropertyParsingMethod(scratch.BufferString, this, HtmlConverterData.cssPropertyInstructions[(int)property.NameId].PropertyId, parsedProperties, out numParsedProperties);
                                        scratch.Reset();
                                    }

                                    if (numParsedProperties != 0)
                                    {
                                        builder.SetProperties(parsedProperties, numParsedProperties);
                                    }
                                }
                            }
                        }

                        builder.Flush();

                        if (style.IsEmpty)
                        {
                            style.Release();
                            styleHandleIndex[styleHandleIndexCount - 1] = 0;
                        }
                        else
                        {
                            styleHandleIndex[styleHandleIndexCount - 1] = style.Handle;
                        }
                    }
                }
            }
            while (CssTokenId.EndOfFile != cssTokenId);

            cssParserInput.Reset();
            cssParser.Reset();
        }

        
        private void FindAndApplyStyle(StyleSelector styleSelector)
        {
            int slotIndex;

            if (styleDictionary != null && styleDictionary.TryGetValue(styleSelector, out slotIndex))
            {
                if (styleHandleIndex[slotIndex] != 0)
                {
                    Last.SetStyleReference((int)PropertyPrecedence.StyleBase + 8 - styleSelector.Specificity, styleHandleIndex[slotIndex]);
                }
            }
        }

        

        private struct StyleSelector
        {
            public HtmlNameIndex nameId;
            public string cls;
            public string id;

            public StyleSelector(HtmlNameIndex nameId, string cls, string id)
            {
                this.nameId = nameId;
                this.cls = (cls == null || cls.Length == 0 || cls.Equals("*")) ? null : cls;
                this.id = (string.IsNullOrEmpty(id) || id.Equals("*")) ? null : id;
            }

            public int Specificity => (id == null ? 0 : 4) +
                                      (cls == null ? 0 : 2) +
                                      (nameId == HtmlNameIndex.Unknown ? 0 : 1);
        }

        

        class StyleSelectorComparer : IEqualityComparer<StyleSelector>, IComparer<StyleSelector>
        {
            public int Compare(StyleSelector x, StyleSelector y)
            {
                var specificityX = x.Specificity;
                int cmp;

                if (specificityX != y.Specificity)
                {
                    return specificityX - y.Specificity;
                }

                switch (specificityX)
                {
                    case 1:     return (int)x.nameId - (int)y.nameId;
                    case 2:     return String.Compare(x.cls, y.cls, StringComparison.OrdinalIgnoreCase);
                    case 3:     return 0 != (cmp = String.Compare(x.cls, y.cls, StringComparison.OrdinalIgnoreCase)) ? cmp : (int)x.nameId - (int)y.nameId;
                    case 4:     return String.Compare(x.id, y.id, StringComparison.OrdinalIgnoreCase);
                    case 5:     return 0 != (cmp = String.Compare(x.id, y.id, StringComparison.OrdinalIgnoreCase)) ? cmp : (int)x.nameId - (int)y.nameId;
                    case 6:     return 0 != (cmp = String.Compare(x.id, y.id, StringComparison.OrdinalIgnoreCase)) ? cmp : String.Compare(x.cls, y.cls, StringComparison.OrdinalIgnoreCase);
                    case 7:     return 0 != (cmp = String.Compare(x.id, y.id, StringComparison.OrdinalIgnoreCase)) ? cmp : 0 != (cmp = String.Compare(x.cls, y.cls, StringComparison.OrdinalIgnoreCase)) ? cmp : (int)x.nameId - (int)y.nameId;
                }

                InternalDebug.Assert(specificityX == 0);
                return 0;
            }

            public bool Equals(StyleSelector x, StyleSelector y)
            {
                return Compare(x, y) == 0;
            }

            public int GetHashCode(StyleSelector x)
            {
                var specificityX = x.Specificity;
                return (int)x.nameId ^ (0 == (specificityX & 4) ? 0 : x.id.GetHashCode()) ^ (0 == (specificityX & 2) ? 0 : x.cls.GetHashCode());
            }
        }
    }
}

