// ***************************************************************
// <copyright file="FormatConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters.Internal.Format
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using Data.Internal;
    using Html;
    using Strings = CtsResources.TextConvertersStrings;

    
    internal enum PropertyPrecedence : byte
    {
        InlineStyle = 0,
        StyleBase = 1,
        NonStyle = 9,
        TagDefault = 10,
        Inherited = 11,
    }

    

    internal abstract class FormatConverter : IProgressMonitor
    {
        internal FormatStore store;

        
        internal BuildStackEntry[] buildStack;
        internal int buildStackTop;
        internal int lastNode;

        #if DEBUG
        internal static int buildStackNextStamp;
        #endif

        private bool mustFlush;             

        
        internal bool emptyContainer;
        internal bool containerFlushed;

        internal StyleBuildHelper containerStyleBuildHelper;

        internal StyleBuildHelper styleBuildHelper;
        internal bool styleBuildHelperLocked;
#if false
        internal StringBuildHelper stringBuildHelper;
        internal bool stringBuildHelperLocked;
#endif
        internal MultiValueBuildHelper multiValueBuildHelper;
        internal bool multiValueBuildHelperLocked;

        internal Dictionary<string, PropertyValue> fontFaceDictionary;
#if DEBUG
        internal TextWriter formatConverterTraceWriter;
#endif
        private bool endOfFile;

        protected bool madeProgress;

        bool newLine;
        bool textQuotingExpected;

        

        internal FormatConverter(Stream formatConverterTraceStream)
        {
            store = new FormatStore();
#if DEBUG
            if (formatConverterTraceStream != null)
            {
                formatConverterTraceWriter = new StreamWriter(formatConverterTraceStream);
            }
#endif
            buildStack = new BuildStackEntry[16];

            containerStyleBuildHelper = new StyleBuildHelper(store);

            styleBuildHelper = new StyleBuildHelper(store);
            multiValueBuildHelper = new MultiValueBuildHelper(store);
#if false
            this.stringBuildHelper = new StringBuildHelper(this.store);
#endif
            fontFaceDictionary = new Dictionary<string, PropertyValue>(StringComparer.OrdinalIgnoreCase);
        }

        

        internal FormatConverter(FormatStore formatStore, Stream formatConverterTraceStream)
        {
            store = formatStore;
#if DEBUG
            if (formatConverterTraceStream != null)
            {
                formatConverterTraceWriter = new StreamWriter(formatConverterTraceStream);
            }
#endif
            buildStack = new BuildStackEntry[16];

            containerStyleBuildHelper = new StyleBuildHelper(store);

            styleBuildHelper = new StyleBuildHelper(store);
            multiValueBuildHelper = new MultiValueBuildHelper(store);
#if false
            this.stringBuildHelper = new StringBuildHelper(this.store);
#endif
            fontFaceDictionary = new Dictionary<string, PropertyValue>(StringComparer.OrdinalIgnoreCase);
        }

        internal FormatStore Store => store;


        protected bool MustFlush
        {
            get
            {
                
                
                

                return mustFlush;
            }

            set
            {
                mustFlush = value;
            }
        }

        

        private void Initialize()
        {
            

            buildStackTop = 0;

            containerStyleBuildHelper.Clean();

            styleBuildHelper.Clean();
            styleBuildHelperLocked = false;
#if false
            this.stringBuildHelper.Clean();
            this.stringBuildHelperLocked = false;
#endif
            multiValueBuildHelper.Cancel();
            multiValueBuildHelperLocked = false;

            fontFaceDictionary.Clear();

            lastNode = store.RootNode.Handle;

            

            buildStack[buildStackTop].type= FormatContainerType.Root;
            buildStack[buildStackTop].node = store.RootNode.Handle;
            buildStackTop ++;

            emptyContainer = false;
            containerFlushed = true;

            mustFlush = false;
            endOfFile = false;

            newLine = true;
            textQuotingExpected = true;
        }

        

        internal FormatNode InitializeDocument()
        {
            Initialize();

            
            var documentContainer = OpenContainer(FormatContainerType.Document, false);
            return documentContainer.Node;
        }

        

        internal FormatNode InitializeFragment()
        {
            Initialize();

            
            var fragmentContainer = OpenContainer(FormatContainerType.Fragment, false);

            OpenContainer(FormatContainerType.PropertyContainer, false);

            Last.SetProperty(0, PropertyId.FontFace, RegisterFaceName(false, "Times New Roman"));
            Last.SetProperty(0, PropertyId.FontSize, new PropertyValue(LengthUnits.Points, 11));

            return fragmentContainer.Node;
        }

        

        public FormatConverterContainer Root
        {
            get
            {
                InternalDebug.Assert(buildStackTop >= 0);
                return new FormatConverterContainer(this, 0);
            }
        }

        

        public FormatConverterContainer Last
        {
            get
            {
                InternalDebug.Assert(buildStackTop >= 0);
                return new FormatConverterContainer(this, emptyContainer ? buildStackTop : buildStackTop - 1);
            }
        }

        

        public FormatNode LastNode => new FormatNode(store, lastNode);


        public FormatConverterContainer LastNonEmpty
        {
            get
            {
                InternalDebug.Assert(buildStackTop > 0);
                return new FormatConverterContainer(this, buildStackTop - 1);
            }
        }

        public abstract void Run();

        public bool EndOfFile => endOfFile;

#if true
        public virtual FormatStore ConvertToStore()
        {
            long loopsWithoutProgress = 0;

            while (!endOfFile)
            {
                

                Run();

                if (madeProgress)
                {
                    madeProgress = false;
                    loopsWithoutProgress = 0;
                }
                else if (200000 == loopsWithoutProgress++)
                {
                    InternalDebug.Assert(false);
                    throw new TextConvertersException(Strings.TooManyIterationsToProduceOutput);
                }
            }

            return Store;
        }
#endif
        

        public FormatConverterContainer OpenContainer(FormatContainerType nodeType, bool empty)
        {
            InternalDebug.Assert(buildStackTop > 0);

            if (!containerFlushed)
            {
                FlushContainer(emptyContainer ? buildStackTop : buildStackTop - 1);
            }

            if (emptyContainer)
            {
#if DEBUG
                if (formatConverterTraceWriter != null)
                {
                    formatConverterTraceWriter.WriteLine("{0}{1} CloseEmpty {2}", Indent(buildStackTop), buildStackTop, buildStack[buildStackTop].type.ToString());
                }
#endif
                PrepareToCloseContainer(buildStackTop);
            }
#if DEBUG
            if (formatConverterTraceWriter != null)
            {
                formatConverterTraceWriter.WriteLine("{0}{1} OpenContainer: {2}{3}", Indent(buildStackTop), buildStackTop, nodeType.ToString(), empty ? " (empty)" : "");
            }
#endif
            
            var stackPos = PushContainer(nodeType, empty, FormatStoreData.DefaultInheritanceMaskIndex.Any);

            return new FormatConverterContainer(this, stackPos);
        }

        

        public FormatConverterContainer OpenContainer(FormatContainerType nodeType, bool empty, int inheritanceMaskIndex, FormatStyle baseStyle, HtmlNameIndex tagName)
        {
            InternalDebug.Assert(buildStackTop > 0);

            if (!containerFlushed)
            {
                FlushContainer(emptyContainer ? buildStackTop : buildStackTop - 1);
            }

            if (emptyContainer)
            {
#if DEBUG
                if (formatConverterTraceWriter != null)
                {
                    formatConverterTraceWriter.WriteLine("{0}{1} CloseEmpty {2}", Indent(buildStackTop), buildStackTop, buildStack[buildStackTop].type.ToString());
                }
#endif
                PrepareToCloseContainer(buildStackTop);
            }
#if DEBUG
            if (formatConverterTraceWriter != null)
            {
                formatConverterTraceWriter.WriteLine("{0}{1} OpenContainer: {2}{3} ({4})", Indent(buildStackTop), buildStackTop, nodeType.ToString(), empty ? " (empty)" : "", tagName.ToString());
            }
#endif
            
            var stackPos = PushContainer(nodeType, empty, inheritanceMaskIndex);

            if (!baseStyle.IsNull)
            {
                baseStyle.AddRef();     
                containerStyleBuildHelper.AddStyle((int)PropertyPrecedence.TagDefault, baseStyle.Handle);
            }

            buildStack[stackPos].tagName = tagName;

            return new FormatConverterContainer(this, stackPos);
        }

        

        public FormatConverterContainer OpenContainer(FormatContainerType nodeType, bool empty, int inheritanceMaskIndex, FormatStyle baseStyle, HtmlTagIndex tagIndex)
        {
            InternalDebug.Assert(buildStackTop > 0);

            if (!containerFlushed)
            {
                FlushContainer(emptyContainer ? buildStackTop : buildStackTop - 1);
            }

            if (emptyContainer)
            {
#if DEBUG
                if (formatConverterTraceWriter != null)
                {
                    formatConverterTraceWriter.WriteLine("{0}{1} CloseEmpty {2}", Indent(buildStackTop), buildStackTop, buildStack[buildStackTop].type.ToString());
                }
#endif
                PrepareToCloseContainer(buildStackTop);
            }
#if DEBUG
            if (formatConverterTraceWriter != null)
            {
                formatConverterTraceWriter.WriteLine("{0}{1} OpenContainer: {2}{3} ({4})", Indent(buildStackTop), buildStackTop, nodeType.ToString(), empty ? " (empty)" : "", tagIndex.ToString());
            }
#endif
            
            var stackPos = PushContainer(nodeType, empty, inheritanceMaskIndex);

            if (!baseStyle.IsNull)
            {
                baseStyle.AddRef();     
                containerStyleBuildHelper.AddStyle((int)PropertyPrecedence.TagDefault, baseStyle.Handle);
            }

            buildStack[stackPos].tagIndex = tagIndex;

            return new FormatConverterContainer(this, stackPos);
        }

        

        public void OpenTextContainer(/*TextMapping textMapping*/)
        {
            InternalDebug.Assert(buildStackTop > 0);

            if (!containerFlushed)
            {
                FlushContainer(emptyContainer ? buildStackTop : buildStackTop - 1);
            }

            if (emptyContainer)
            {
#if DEBUG
                if (formatConverterTraceWriter != null)
                {
                    formatConverterTraceWriter.WriteLine("{0}{1} CloseEmpty {2}", Indent(buildStackTop), buildStackTop, buildStack[buildStackTop].type.ToString());
                }
#endif
                PrepareToCloseContainer(buildStackTop);
            }

            PrepareToAddText(/*textMapping*/);
        }

        

        public void CloseContainer()
        {
            InternalDebug.Assert(buildStackTop > 1, "should never close the root container");

            if (!containerFlushed)
            {
                FlushContainer(emptyContainer ? buildStackTop : buildStackTop - 1);
            }

            if (emptyContainer)
            {
#if DEBUG
                if (formatConverterTraceWriter != null)
                {
                    formatConverterTraceWriter.WriteLine("{0}{1} CloseEmpty {2}", Indent(buildStackTop), buildStackTop, buildStack[buildStackTop].type.ToString());
                }
#endif
                PrepareToCloseContainer(buildStackTop);
            }
#if DEBUG
            if (formatConverterTraceWriter != null)
            {
                formatConverterTraceWriter.WriteLine("{0}{1} CloseContainer : {2}", Indent(buildStackTop - 1), buildStackTop - 1, buildStack[buildStackTop - 1].type.ToString());
            }
#endif
            PopContainer();
        }

        

        public void CloseOverlappingContainer(int countLevelsToKeepOpen)
        {
            InternalDebug.Assert(countLevelsToKeepOpen > 0 && countLevelsToKeepOpen < buildStackTop - 2);

            if (!containerFlushed)
            {
                FlushContainer(emptyContainer ? buildStackTop : buildStackTop - 1);
            }

            if (emptyContainer)
            {
#if DEBUG
                if (formatConverterTraceWriter != null)
                {
                    formatConverterTraceWriter.WriteLine("{0}{1} CloseEmpty {2}", Indent(buildStackTop), buildStackTop, buildStack[buildStackTop].type.ToString());
                }
#endif
                PrepareToCloseContainer(buildStackTop);
            }
#if DEBUG
            if (formatConverterTraceWriter != null)
            {
                formatConverterTraceWriter.WriteLine("{0}{1} CloseOverlappedContainer({2}): {3}", Indent(buildStackTop - 1 - countLevelsToKeepOpen), buildStackTop - 1 - countLevelsToKeepOpen, countLevelsToKeepOpen, buildStack[buildStackTop - 1 - countLevelsToKeepOpen].type.ToString());
            }
#endif
            PopContainer(buildStackTop - 1 - countLevelsToKeepOpen);
        }

        

        public void CloseAllContainersAndSetEOF()
        {
            

            while (buildStackTop > 1)
            {
                CloseContainer();
            }

            
            store.GetNode(buildStack[0].node).PrepareToClose(store.CurrentTextPosition);

            mustFlush = true;
            endOfFile = true;
        }

        

        protected void CloseContainer(FormatContainerType containerType)
        {
            if (!containerFlushed)
            {
                FlushContainer(emptyContainer ? buildStackTop : buildStackTop - 1);
            }

            if (emptyContainer)
            {
#if DEBUG
                if (formatConverterTraceWriter != null)
                {
                    formatConverterTraceWriter.WriteLine("{0}{1} CloseEmpty {2}", Indent(buildStackTop), buildStackTop, buildStack[buildStackTop].type.ToString());
                }
#endif
                PrepareToCloseContainer(buildStackTop);
            }

            for (var i = buildStackTop - 1; i > 0; i--)
            {
                if (buildStack[i].type == containerType)
                {
#if DEBUG
                    if (formatConverterTraceWriter != null)
                    {
                        formatConverterTraceWriter.WriteLine("{0}{1} CloseContainer({2}): {3}", Indent(i), i, containerType.ToString(), buildStack[i].type.ToString());
                    }
#endif
                    PopContainer(i);
                    break;
                }
            }
        }

        

        protected void CloseContainer(HtmlNameIndex tagName)
        {
            if (!containerFlushed)
            {
                FlushContainer(emptyContainer ? buildStackTop : buildStackTop - 1);
            }

            if (emptyContainer)
            {
#if DEBUG
                if (formatConverterTraceWriter != null)
                {
                    formatConverterTraceWriter.WriteLine("{0}{1} CloseEmpty {2}", Indent(buildStackTop), buildStackTop, buildStack[buildStackTop].type.ToString());
                }
#endif
                PrepareToCloseContainer(buildStackTop);
            }

            for (var i = buildStackTop - 1; i > 0; i--)
            {
                if (buildStack[i].tagName == tagName)
                {
#if DEBUG
                    if (formatConverterTraceWriter != null)
                    {
                        formatConverterTraceWriter.WriteLine("{0}{1} CloseContainer({2}): {3}", Indent(i), i, tagName.ToString(), buildStack[i].type.ToString());
                    }
#endif
                    PopContainer(i);
                    break;
                }
            }
        }

        

        protected FormatNode CreateNode(FormatContainerType type)
        {
            var node = store.AllocateNode(type);
            node.EndTextPosition = node.BeginTextPosition;
            node.SetOutOfOrder();

            return node;
        }

        

        public void AddNonSpaceText(char[] buffer, int offset, int count)
        {
            InternalDebug.Assert(count > 0);

            PrepareToAddText();

            InternalDebug.Assert(emptyContainer);

            
            if (!containerFlushed)
            {
                FlushContainer(buildStackTop);
            }
#if DEBUG
            if (formatConverterTraceWriter != null)
            {
                formatConverterTraceWriter.WriteLine("{0}AddText: {1} \"{2}\"", Indent(buildStackTop), count, new string(buffer, offset, count));
            }
#endif
            newLine = false;

            if (textQuotingExpected)
            {
                
                

                if (buffer[offset] == '>')
                {
                    do
                    {
                        store.AddText(buffer, offset, 1);
                        offset++;
                        count--;
                    }
                    while (count != 0 && buffer[offset] == '>');

                    if (count == 0)
                    {
                        return;
                    }
                }

                store.SetTextBoundary();       
                textQuotingExpected = false;
            }

            store.AddText(buffer, offset, count);

        }

        

        public void AddSpace(int count)
        {
            PrepareToAddText();

            InternalDebug.Assert(emptyContainer);

            
            if (!containerFlushed)
            {
                FlushContainer(buildStackTop);
            }
#if DEBUG
            if (formatConverterTraceWriter != null)
            {
                formatConverterTraceWriter.WriteLine("{0}AddSpace: {1}", Indent(buildStackTop), count);
            }
#endif
            store.AddSpace(count);

            newLine = false;
        }

        

        public void AddLineBreak(int count)
        {
            InternalDebug.Assert(count >= 1);

            PrepareToAddText();

            InternalDebug.Assert(emptyContainer);

            
            if (!containerFlushed)
            {
                FlushContainer(buildStackTop);
            }
#if DEBUG
            if (formatConverterTraceWriter != null)
            {
                formatConverterTraceWriter.WriteLine("{0}AddSpace: {1}", Indent(buildStackTop), count);
            }
#endif

            if (!newLine)
            {
                store.AddLineBreak(1);
                store.SetTextBoundary();       

                if (count > 1)
                {
                    store.AddLineBreak(count - 1);
                }

                newLine = true;
                textQuotingExpected = true;
            }
            else
            {
                store.AddLineBreak(count);
            }
        }

        

        public void AddNbsp(int count)
        {
            PrepareToAddText();

            InternalDebug.Assert(emptyContainer);

            
            if (!containerFlushed)
            {
                FlushContainer(buildStackTop);
            }
#if DEBUG
            if (formatConverterTraceWriter != null)
            {
                formatConverterTraceWriter.WriteLine("{0}AddNbsp: {1}", Indent(buildStackTop), count);
            }
#endif
            store.AddNbsp(count);

            newLine = false;
        }

        

        public void AddTabulation(int count)
        {
            PrepareToAddText();

            InternalDebug.Assert(emptyContainer);

            
            if (!containerFlushed)
            {
                FlushContainer(buildStackTop);
            }
#if DEBUG
            if (formatConverterTraceWriter != null)
            {
                formatConverterTraceWriter.WriteLine("{0}AddTabulation: {1}", Indent(buildStackTop), count);
            }
#endif
            store.AddTabulation(count);

            newLine = false;
            textQuotingExpected = false;
        }

        

        private void PrepareToAddText()
        {
            if (!emptyContainer || !buildStack[buildStackTop].IsText)
            {
                if (!containerFlushed)
                {
                    FlushContainer(emptyContainer ? buildStackTop : buildStackTop - 1);
                }

                if (emptyContainer)
                {
#if DEBUG
                    if (formatConverterTraceWriter != null)
                    {
                        formatConverterTraceWriter.WriteLine("{0}{1} CloseEmpty {2}", Indent(buildStackTop), buildStackTop, buildStack[buildStackTop].type.ToString());
                    }
#endif
                    PrepareToCloseContainer(buildStackTop);
                }
#if DEBUG
                if (formatConverterTraceWriter != null)
                {
                    formatConverterTraceWriter.WriteLine("{0}{1} OpenText", Indent(buildStackTop), buildStackTop);
                }
#endif
                
                PushContainer(FormatContainerType.Text, true, FormatStoreData.DefaultInheritanceMaskIndex.Text);
            }
        }

        

        public StringValue RegisterStringValue(bool isStatic, string value)
        {
            
            return store.AllocateStringValue(isStatic, value);
        }

        

        public StringValue RegisterStringValue(bool isStatic, string str, int offset, int count)
        {
            var value = str;
            if (offset != 0 || count != str.Length)
            {
                value = str.Substring(offset, count);
            }

            
            return store.AllocateStringValue(isStatic, value);
        }

        

        public StringValue RegisterStringValue(bool isStatic, BufferString value)
        {
            
            return store.AllocateStringValue(isStatic, value.ToString());
        }

        

        public PropertyValue RegisterFaceName(bool isStatic, BufferString value)
        {
            if (value.Length == 0)
            {
                return PropertyValue.Null;
            }

            
            return RegisterFaceName(isStatic, value.ToString());
        }

        

        public PropertyValue RegisterFaceName(bool isStatic, string faceName)
        {
            if (string.IsNullOrEmpty(faceName))
            {
                return PropertyValue.Null;
            }

            PropertyValue value;
            if (fontFaceDictionary.TryGetValue(faceName, out value))
            {
                if (value.IsString)
                {
                    store.AddRefValue(value);
                }
                return value;
            }

            var registeredValue = RegisterStringValue(isStatic, faceName);
            
            value = registeredValue.PropertyValue;

            if (fontFaceDictionary.Count < 100)
            {
                registeredValue.AddRef();
                fontFaceDictionary.Add(faceName, value);
            }

            return value;
        }

#if false
        

        public StringValue RegisterStringValue(bool isStatic, out StringValueBuilder builder)
        {
            StringValue registeredValue = this.store.AllocateStringValue(isStatic);
            builder = new StringValueBuilder(this, registeredValue.Handle);

            
            return registeredValue;
        }
#endif
        

        public MultiValue RegisterMultiValue(bool isStatic, out MultiValueBuilder builder)
        {
            var registeredValue = store.AllocateMultiValue(isStatic);
            builder = new MultiValueBuilder(this, registeredValue.Handle);

            
            return registeredValue;
        }

        

        public FormatStyle RegisterStyle(bool isStatic, out StyleBuilder builder)
        {
            var style = store.AllocateStyle(isStatic);
            builder = new StyleBuilder(this, style.Handle);

            
            return style;
        }

        

        public FormatStyle GetStyle(int styleHandle)
        {
            return store.GetStyle(styleHandle);
        }

        

        public StringValue GetStringValue(PropertyValue pv)
        {
            return store.GetStringValue(pv);
        }

        

        public MultiValue GetMultiValue(PropertyValue pv)
        {
            return store.GetMultiValue(pv);
        }

        

        public void ReleasePropertyValue(PropertyValue pv)
        {
            store.ReleaseValue(pv);
        }

        

        private void FlushContainer(int stackPos)
        {
            

            InternalDebug.Assert(!containerFlushed);
            InternalDebug.Assert((!emptyContainer && stackPos == buildStackTop - 1) || (emptyContainer && stackPos == buildStackTop));
#if DEBUG
            if (formatConverterTraceWriter != null)
            {
                formatConverterTraceWriter.WriteLine("{0}{1} Flush {2}", Indent(stackPos), stackPos, buildStack[stackPos].type.ToString());
            }
#endif
            

            var newType = FixContainerType(buildStack[stackPos].type, containerStyleBuildHelper);

            if (newType != buildStack[stackPos].type)
            {
#if DEBUG
                if (formatConverterTraceWriter != null)
                {
                    formatConverterTraceWriter.WriteLine("{0}{1} Flush converting {2} to {3}", Indent(stackPos), stackPos, buildStack[stackPos].type.ToString(), newType.ToString());
                }
#endif
                buildStack[stackPos].type = newType;
            }

            containerStyleBuildHelper.GetPropertyList(
                            out buildStack[stackPos].properties,
                            out buildStack[stackPos].flagProperties,
                            out buildStack[stackPos].propertyMask);

            if (!buildStack[stackPos].IsPropertyContainerOrNull)
            {
                

                if (!newLine && 0 != (buildStack[stackPos].type & FormatContainerType.BlockFlag))
                {
                    
                    store.AddBlockBoundary();

                    newLine = true;
                    textQuotingExpected = true;
                }

                var node = store.AllocateNode(buildStack[stackPos].type);
                node.SetOnRightEdge();
#if DEBUG
                if (formatConverterTraceWriter != null)
                {
                    formatConverterTraceWriter.WriteLine("{0}{1} Flush creating a {2} node {3} with parent {4}", Indent(stackPos), stackPos, buildStack[stackPos].type.ToString(), node.Handle, lastNode);
                }
#endif
                if (0 != (buildStack[stackPos].type & FormatContainerType.InlineObjectFlag))
                {
                    store.AddInlineObject();
                }

                var parent = store.GetNode(lastNode);

                int propContainerInheritanceStopLevel; 

                parent = GetParentForNewNode(node, parent, stackPos, out propContainerInheritanceStopLevel);

                parent.AppendChild(node);

                buildStack[stackPos].node = node.Handle;

                lastNode = node.Handle;

                FlagProperties flagProperties;
                PropertyBitMask propertyMask;
                Property[] properties;

                if (propContainerInheritanceStopLevel < stackPos)
                {
                    
                    

                    var remainingFlagsMask = FlagProperties.AllOn;
                    var remainingPropertiesMask = PropertyBitMask.AllOn;

                    for (var i = stackPos; i >= propContainerInheritanceStopLevel && (!remainingFlagsMask.IsClear || !remainingPropertiesMask.IsClear); i--)
                    {
                        
                        

                        if (i == stackPos || buildStack[i].type == FormatContainerType.PropertyContainer)
                        {
                            
                            flagProperties = buildStack[i].flagProperties & remainingFlagsMask;

                            
                            
                            containerStyleBuildHelper.AddProperties((int)PropertyPrecedence.Inherited, flagProperties, remainingPropertiesMask, buildStack[i].properties);

                            
                            remainingFlagsMask &= ~buildStack[i].flagProperties;
                            remainingPropertiesMask &= ~buildStack[i].propertyMask;

                            
                            remainingFlagsMask &= FormatStoreData.GlobalInheritanceMasks[buildStack[i].inheritanceMaskIndex].flagProperties;
                            remainingPropertiesMask &= FormatStoreData.GlobalInheritanceMasks[buildStack[i].inheritanceMaskIndex].propertyMask;
                        }
                    }

                    containerStyleBuildHelper.GetPropertyList(out properties, out flagProperties, out propertyMask);
                }
                else
                {
                    flagProperties = buildStack[stackPos].flagProperties;
                    propertyMask = buildStack[stackPos].propertyMask;

                    properties = buildStack[stackPos].properties;
                    if (properties != null)
                    {
                        
                        for (var j = 0; j < properties.Length; j++)
                        {
                            if (properties[j].Value.IsRefCountedHandle)
                            {
                                store.AddRefValue(properties[j].Value);
                            }
                        }
                    }
                }

                node.SetProps(flagProperties, propertyMask, properties, buildStack[stackPos].inheritanceMaskIndex);
            }

            containerStyleBuildHelper.Clean();
            containerFlushed = true;
        }

        

        protected virtual FormatContainerType FixContainerType(FormatContainerType type, StyleBuildHelper styleBuilderWithContainerProperties)
        {
            return type;
        }

        

        protected virtual FormatNode GetParentForNewNode(FormatNode node, FormatNode defaultParent, int stackPos, out int propContainerInheritanceStopLevel)
        {
            propContainerInheritanceStopLevel = DefaultPropContainerInheritanceStopLevel(stackPos);
            return defaultParent;
        }

        

        protected int DefaultPropContainerInheritanceStopLevel(int stackPos)
        {
            var i = stackPos - 1;

            for (; i >= 0; i--)
            {
                if (buildStack[i].node != 0)
                {
                    break;
                }
            }

            return i + 1;
        }

        

        private int PushContainer(FormatContainerType type, bool empty, int inheritanceMaskIndex)
        {
            
            InternalDebug.Assert(!emptyContainer);

            var stackPos = buildStackTop;

            if (stackPos == buildStack.Length)
            {
                if (buildStack.Length >= HtmlSupport.HtmlNestingLimit)
                {
                    throw new TextConvertersException(Strings.InputDocumentTooComplex);
                }

                var newSize = (HtmlSupport.HtmlNestingLimit / 2 > buildStack.Length) ? buildStack.Length * 2 : HtmlSupport.HtmlNestingLimit;

                var newBuildStack = new BuildStackEntry[newSize];
                Array.Copy(buildStack, 0, newBuildStack, 0, buildStackTop);
                buildStack = newBuildStack;
                newBuildStack = null;
            }

            store.SetTextBoundary();
            
            
            buildStack[stackPos].type = type;
            buildStack[stackPos].tagName = HtmlNameIndex._NOTANAME;
            buildStack[stackPos].inheritanceMaskIndex = inheritanceMaskIndex;
            buildStack[stackPos].tagIndex = HtmlTagIndex._NULL;
            buildStack[stackPos].flagProperties.ClearAll();
            buildStack[stackPos].propertyMask.ClearAll();
            buildStack[stackPos].properties = null;

            #if DEBUG
            buildStack[stackPos].stamp = buildStackNextStamp++;
            #endif

            buildStack[stackPos].node = 0;

            if (!empty)
            {
                buildStackTop ++;
            }

            emptyContainer = empty;
            containerFlushed = false;

            return stackPos;
        }

        

        private void PopContainer()
        {
            InternalDebug.Assert(!emptyContainer);
            InternalDebug.Assert(buildStackTop > 1 && buildStack[0].node != 0);

            PrepareToCloseContainer(buildStackTop - 1);

            buildStackTop --;

            #if DEBUG
            buildStack[buildStackTop].stamp = 0;
            #endif
        }

        

        private void PopContainer(int level)
        {
            
            InternalDebug.Assert(!emptyContainer);
            InternalDebug.Assert(level >= 0 && level < buildStackTop && buildStack[0].node != 0);

            PrepareToCloseContainer(level);

            
            Array.Copy(buildStack, level + 1, buildStack, level, buildStackTop - level - 1);

            buildStackTop --;

            #if DEBUG
            buildStack[buildStackTop].stamp = 0;
            #endif
        }

        

        private void PrepareToCloseContainer(int stackPosition)
        {
            InternalDebug.Assert(stackPosition > 0 && stackPosition <= buildStackTop);

            

            if (null != buildStack[stackPosition].properties)
            {
                for (var i = 0; i < buildStack[stackPosition].properties.Length; i++)
                {
                    if (buildStack[stackPosition].properties[i].Value.IsRefCountedHandle)
                    {
                        store.ReleaseValue(buildStack[stackPosition].properties[i].Value);
                    }
                }

                buildStack[stackPosition].properties = null;
            }

            if (buildStack[stackPosition].node != 0)
            {
                var node = store.GetNode(buildStack[stackPosition].node);

                if (!newLine && 0 != (node.NodeType & FormatContainerType.BlockFlag))
                {
                    store.AddBlockBoundary();

                    newLine = true;
                    textQuotingExpected = true;
                }

                node.PrepareToClose(store.CurrentTextPosition);

                if (!node.Parent.IsNull && node.Parent.NodeType == FormatContainerType.TableContainer)
                {
                    node.Parent.PrepareToClose(store.CurrentTextPosition);
                }

                
                
                

                
                

                
                
                
                
                
                
                
                

                if (buildStack[stackPosition].node == lastNode)
                {
                    for (var i = stackPosition - 1; i >= 0; i--)
                    {
                        if (buildStack[i].node != 0)
                        {
                            lastNode = buildStack[i].node;
                            break;
                        }
                    }
                }
            }

            store.SetTextBoundary();

            emptyContainer = false;
        }

        void IProgressMonitor.ReportProgress()
        {
            madeProgress = true;
        }

        private static string Indent(int level)
        {
            const string spaces = "                                                  ";
            return spaces.Substring(0, Math.Min(spaces.Length, level * 2));
        }

        

        
        internal struct BuildStackEntry
        {
            internal FormatContainerType type;

            internal HtmlNameIndex tagName;
            internal HtmlTagIndex tagIndex;

            internal int node;                                  

            internal int inheritanceMaskIndex;                  
            internal FlagProperties flagProperties;             
            internal PropertyBitMask propertyMask;              
            internal Property[] properties;                     

            #if DEBUG
            internal int stamp;
            #endif

            public bool IsText => type == FormatContainerType.Text;

            public bool IsPropertyContainer => type == FormatContainerType.PropertyContainer;

            public bool IsPropertyContainerOrNull => type == FormatContainerType.PropertyContainer || type == FormatContainerType.Null;

#if DEBUG
            public int Stamp
            {
                get { return stamp; }
                set { stamp = value; }
            }
            #endif

            public FormatContainerType NodeType
            {
                get { return type; }
                set { type = value; }
            }

            public FlagProperties FlagProperties
            {
                get { return flagProperties; }
                set { flagProperties = value; }
            }

            public PropertyBitMask PropertyMask
            {
                get { return propertyMask; }
                set { propertyMask = value; }
            }

            public void Clean()
            {
                this = new BuildStackEntry();
            }
        }
    }


    

    internal struct FormatConverterContainer
    {
        private FormatConverter converter;
        private int level;
        #if DEBUG
        private int stamp;
        #endif

        public static readonly FormatConverterContainer Null = new FormatConverterContainer(null, -1);

        internal FormatConverterContainer(FormatConverter converter, int level)
        {
            InternalDebug.Assert((converter == null && level == -1) || (level >= 0 && level <= converter.buildStackTop));

            this.converter = converter;
            this.level = level;

            #if DEBUG
            stamp = (converter != null) ? converter.buildStack[level].stamp : -1;
            #endif
        }

        public bool IsNull => converter == null;

        public FormatContainerType Type
        {
            get
            {
                AssertValid();
                return IsNull ? FormatContainerType.Null : converter.buildStack[level].type;
            }
        }

        public HtmlNameIndex TagName
        {
            get
            {
                AssertValid();
                return IsNull ? HtmlNameIndex._NOTANAME : converter.buildStack[level].tagName;
            }
        }

        public FormatConverterContainer Parent
        {
            get
            {
                AssertValidAndNotNull();
                return level <= 1 ? Null : new FormatConverterContainer(converter, level - 1);
            }
        }

        public FormatConverterContainer Child
        {
            get
            {
                AssertValidAndNotNull();
                return level == converter.buildStackTop - 1 ? Null : new FormatConverterContainer(converter, level + 1);
            }
        }

        public FlagProperties FlagProperties
        {
            get { AssertValidAndNotNull(); return converter.buildStack[level].flagProperties; }
            set { AssertValidAndNotNull(); converter.buildStack[level].flagProperties = value; }
        }

        public PropertyBitMask PropertyMask
        {
            get { AssertValidAndNotNull(); return converter.buildStack[level].propertyMask; }
            set { AssertValidAndNotNull(); converter.buildStack[level].propertyMask = value; }
        }

        public Property[] Properties
        {
            get { AssertValidAndNotNull(); return converter.buildStack[level].properties; }
        }

        public FormatNode Node
        {
            get { AssertValidAndNotNull(); return new FormatNode(converter.store, converter.buildStack[level].node); }
        }

        
        
        
        
        

        public void SetProperty(PropertyPrecedence propertyPrecedence, PropertyId propertyId, PropertyValue value)
        {
            AssertValidNotFlushed();

            
            if (value.IsString)
            {
                converter.GetStringValue(value).AddRef();
            }
            else if (value.IsMultiValue)
            {
                converter.GetMultiValue(value).AddRef();
            }

            converter.containerStyleBuildHelper.SetProperty(
                        (int)propertyPrecedence, 
                        propertyId, 
                        value);
        }

        public void SetProperties(PropertyPrecedence propertyPrecedence, Property[] properties, int propertyCount)
        {
            AssertValidNotFlushed();

            for (var i = 0; i < propertyCount; i++)
            {
                SetProperty(propertyPrecedence, properties[i].Id, properties[i].Value);
            }
        }

        public void SetStringProperty(PropertyPrecedence propertyPrecedence, PropertyId propertyId, StringValue value)
        {
            AssertValidNotFlushed();

            
            value.AddRef();

            converter.containerStyleBuildHelper.SetProperty(
                        (int)propertyPrecedence, 
                        propertyId, 
                        value.PropertyValue);
        }

        public void SetStringProperty(PropertyPrecedence propertyPrecedence, PropertyId propertyId, string value)
        {
            AssertValidNotFlushed();

            var registeredValue = converter.RegisterStringValue(false, value);

            

            converter.containerStyleBuildHelper.SetProperty(
                        (int)propertyPrecedence, 
                        propertyId, 
                        registeredValue.PropertyValue);
        }

        public void SetStringProperty(PropertyPrecedence propertyPrecedence, PropertyId propertyId, BufferString value)
        {
            AssertValidNotFlushed();

            var registeredValue = converter.RegisterStringValue(false, value);

            

            converter.containerStyleBuildHelper.SetProperty(
                        (int)propertyPrecedence, 
                        propertyId, 
                        registeredValue.PropertyValue);
        }

        public void SetStringProperty(PropertyPrecedence propertyPrecedence, PropertyId propertyId, char[] buffer, int offset, int count)
        {
            AssertValidNotFlushed();

            var registeredValue = converter.RegisterStringValue(false, new BufferString(buffer, offset, count));

            

            converter.containerStyleBuildHelper.SetProperty(
                        (int)propertyPrecedence, 
                        propertyId, 
                        registeredValue.PropertyValue);
        }

#if false
        public void SetStringProperty(PropertyPrecedence propertyPrecedence, PropertyId propertyId, out StringValueBuilder stringBuilder)
        {
            this.AssertValidNotFlushed();

            StringValue registeredValue = this.converter.RegisterStringValue(false, out stringBuilder);

            

            this.converter.containerStyleBuildHelper.SetProperty(
                        (int)propertyPrecedence, 
                        propertyId, 
                        registeredValue.PropertyValue);
        }
#endif
        public void SetMultiValueProperty(PropertyPrecedence propertyPrecedence, PropertyId propertyId, MultiValue value)
        {
            AssertValidNotFlushed();

            
            value.AddRef();

            converter.containerStyleBuildHelper.SetProperty(
                        (int)propertyPrecedence, 
                        propertyId, 
                        value.PropertyValue);
        }

        public void SetMultiValueProperty(PropertyPrecedence propertyPrecedence, PropertyId propertyId, out MultiValueBuilder multiValueBuilder)
        {
            AssertValidNotFlushed();

            var registeredValue = converter.RegisterMultiValue(false, out multiValueBuilder);

            

            converter.containerStyleBuildHelper.SetProperty(
                        (int)propertyPrecedence, 
                        propertyId, 
                        registeredValue.PropertyValue);
        }

        public void SetStyleReference(int stylePrecedence, int styleHandle)
        {
            AssertValidNotFlushed();
            InternalDebug.Assert(stylePrecedence > 0);

            converter.containerStyleBuildHelper.AddStyle(stylePrecedence, styleHandle);
        }

        public void SetStyleReference(int stylePrecedence, FormatStyle style)
        {
            AssertValidNotFlushed();
            InternalDebug.Assert(stylePrecedence > 0);

            converter.containerStyleBuildHelper.AddStyle(stylePrecedence, style.Handle);
        }

        
        public static bool operator ==(FormatConverterContainer x, FormatConverterContainer y) 
        {
            return x.converter == y.converter && x.level == y.level;
        }

        
        public static bool operator !=(FormatConverterContainer x, FormatConverterContainer y) 
        {
            return x.converter != y.converter || x.level != y.level;
        }

        public override bool Equals(object obj)
        {
            return (obj is FormatConverterContainer) && converter == ((FormatConverterContainer)obj).converter && level == ((FormatConverterContainer)obj).level;
        }

        public override int GetHashCode()
        {
            return level;
        }


        [System.Diagnostics.Conditional("DEBUG")]
        private void AssertValidAndNotNull()
        {
            AssertValid();
            InternalDebug.Assert(!IsNull);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        private void AssertValid()
        {
            #if DEBUG
            InternalDebug.Assert(IsNull || (level > 0 && level <= converter.buildStackTop && stamp == converter.buildStack[level].stamp));
            #endif
        }

        [System.Diagnostics.Conditional("DEBUG")]
        private void AssertValidNotFlushed()
        {
            AssertValidAndNotNull();
            InternalDebug.Assert(!converter.containerFlushed &&
                ((converter.emptyContainer && level == converter.buildStackTop) ||
                    (!converter.emptyContainer && level == converter.buildStackTop - 1)));
        }
    }

    

    internal struct StyleBuilder
    {
        private FormatConverter converter;
        private int handle;

        internal StyleBuilder(FormatConverter converter, int handle)
        {
            this.converter = converter;
            this.handle = handle;

            InternalDebug.Assert(!this.converter.styleBuildHelperLocked);
            #if DEBUG
            this.converter.styleBuildHelperLocked = true;
            #endif
        }

        public void SetProperty(PropertyId propertyId, PropertyValue value)
        {
            InternalDebug.Assert(converter.styleBuildHelperLocked);

            converter.styleBuildHelper.SetProperty(0, propertyId, value);
        }

        public void SetProperties(Property[] properties, int propertyCount)
        {
            for (var i = 0; i < propertyCount; i++)
            {
                SetProperty(properties[i].Id, properties[i].Value);
            }
        }

        public void SetStringProperty(PropertyId propertyId, StringValue value)
        {
            InternalDebug.Assert(converter.styleBuildHelperLocked);

            converter.styleBuildHelper.SetProperty(0, propertyId, value.PropertyValue);
        }

        public void SetStringProperty(PropertyId propertyId, string value)
        {
            InternalDebug.Assert(converter.styleBuildHelperLocked);

            var registeredValue = converter.RegisterStringValue(false, value);

            converter.styleBuildHelper.SetProperty(0, propertyId, registeredValue.PropertyValue);
        }

        public void SetStringProperty(PropertyId propertyId, BufferString value)
        {
            InternalDebug.Assert(converter.styleBuildHelperLocked);

            var registeredValue = converter.RegisterStringValue(false, value);

            converter.styleBuildHelper.SetProperty(0, propertyId, registeredValue.PropertyValue);
        }

        public void SetStringProperty(PropertyId propertyId, char[] buffer, int offset, int count)
        {
            InternalDebug.Assert(converter.styleBuildHelperLocked);

            var registeredValue = converter.RegisterStringValue(false, new BufferString(buffer, offset, count));

            converter.styleBuildHelper.SetProperty(0, propertyId, registeredValue.PropertyValue);
        }

#if false
        public void SetStringProperty(PropertyId propertyId, out StringValueBuilder stringBuilder)
        {
            InternalDebug.Assert(this.converter.styleBuildHelperLocked);

            StringValue registeredValue = this.converter.RegisterStringValue(false, out stringBuilder);

            this.converter.styleBuildHelper.SetProperty(0, propertyId, registeredValue.PropertyValue);
        }
#endif
        public void SetMultiValueProperty(PropertyId propertyId, MultiValue value)
        {
            InternalDebug.Assert(converter.styleBuildHelperLocked);

            
            value.AddRef();

            converter.styleBuildHelper.SetProperty(0, propertyId, value.PropertyValue);
        }

        public void SetMultiValueProperty(PropertyId propertyId, out MultiValueBuilder multiValueBuilder)
        {
            InternalDebug.Assert(converter.styleBuildHelperLocked);

            var registeredValue = converter.RegisterMultiValue(false, out multiValueBuilder);

            converter.styleBuildHelper.SetProperty(0, propertyId, registeredValue.PropertyValue);
        }

        public void Flush()
        {
            InternalDebug.Assert(converter.styleBuildHelperLocked);
            converter.styleBuildHelper.GetPropertyList(
                            out converter.store.styles.Plane(handle)[converter.store.styles.Index(handle)].propertyList,
                            out converter.store.styles.Plane(handle)[converter.store.styles.Index(handle)].flagProperties,
                            out converter.store.styles.Plane(handle)[converter.store.styles.Index(handle)].propertyMask);
            #if DEBUG
            converter.styleBuildHelperLocked = false;
            #endif
        }
    }

#if false

    

    internal struct StringValueBuilder
    {
        private FormatConverter converter;
        private int handle;

        internal StringValueBuilder(FormatConverter converter, int handle)
        {
            this.converter = converter;
            this.handle = handle;

            InternalDebug.Assert(!this.converter.stringBuildHelperLocked);
            #if DEBUG
            this.converter.stringBuildHelperLocked = true;
            #endif
        }

        public void AddText(char[] buffer, int offset, int count)
        {
            InternalDebug.Assert(this.converter.stringBuildHelperLocked);
            this.converter.stringBuildHelper.AddText(buffer, offset, count);
        }

        public void AddText(string value)
        {
            InternalDebug.Assert(this.converter.stringBuildHelperLocked);
            this.converter.stringBuildHelper.AddText(value);
        }

        public void Flush()
        {
            InternalDebug.Assert(this.converter.stringBuildHelperLocked);
            this.converter.store.strings[this.handle].str = this.converter.stringBuildHelper.GetString();
            #if DEBUG
            this.converter.stringBuildHelperLocked = false;
            #endif
        }
    }

#endif

    

    internal struct MultiValueBuilder
    {
        private FormatConverter converter;
        private int handle;

        internal MultiValueBuilder(FormatConverter converter, int handle)
        {
            this.converter = converter;
            this.handle = handle;

            InternalDebug.Assert(!this.converter.multiValueBuildHelperLocked);
            #if DEBUG
            this.converter.multiValueBuildHelperLocked = true;
            #endif
        }

        public int Count => converter.multiValueBuildHelper.Count;

        public PropertyValue this[int i] => converter.multiValueBuildHelper[i];

        public void AddValue(PropertyValue value)
        {
            InternalDebug.Assert(converter.multiValueBuildHelperLocked);
            
            InternalDebug.Assert(!value.IsMultiValue);

            
            converter.multiValueBuildHelper.AddValue(value);
        }

        public void AddStringValue(StringValue value)
        {
            InternalDebug.Assert(converter.multiValueBuildHelperLocked);
            
            converter.multiValueBuildHelper.AddValue(value.PropertyValue);
        }

        public void AddStringValue(string value)
        {
            InternalDebug.Assert(converter.multiValueBuildHelperLocked);

            var registeredValue = converter.RegisterStringValue(false, value);

            converter.multiValueBuildHelper.AddValue(registeredValue.PropertyValue);
        }

        public void AddStringValue(char[] buffer, int offset, int count)
        {
            InternalDebug.Assert(converter.multiValueBuildHelperLocked);

            var registeredValue = converter.RegisterStringValue(false, new BufferString(buffer, offset, count));

            converter.multiValueBuildHelper.AddValue(registeredValue.PropertyValue);
        }
#if false
        public void AddStringValue(out StringValueBuilder stringBuilder)
        {
            InternalDebug.Assert(this.converter.multiValueBuildHelperLocked);

            StringValue registeredValue = this.converter.RegisterStringValue(false, out stringBuilder);

            this.converter.multiValueBuildHelper.AddValue(registeredValue.PropertyValue);
        }
#endif
        public void Flush()
        {
            InternalDebug.Assert(converter.multiValueBuildHelperLocked);
            converter.store.multiValues.Plane(handle)[converter.store.multiValues.Index(handle)].values = converter.multiValueBuildHelper.GetValues();
            #if DEBUG
            converter.multiValueBuildHelperLocked = false;
            #endif
        }

        public void Cancel()
        {
            converter.multiValueBuildHelper.Cancel();
            #if DEBUG
            converter.multiValueBuildHelperLocked = false;
            #endif
        }
    }

    

    internal struct StyleBuildHelper
    {
        internal FormatStore store;

        internal PropertyBitMask propertyMask;
        private PrecedenceEntry[] entries;
        private int topEntry;
        private int currentEntry;
        private int nonFlagPropertiesCount;

        private class PrecedenceEntry
        {
            public int precedence;
            public FlagProperties flagProperties;
            public PropertyBitMask propertyMask;
            public Property[] properties;
            public int count;
            public int nextSearchIndex;

            public PrecedenceEntry(int precedence)
            {
                this.precedence = precedence;
                properties = new Property[(int)PropertyId.MaxValue];
            }

            public void ReInitialize(int precedence)
            {
                this.precedence = precedence;
                flagProperties.ClearAll();
                propertyMask.ClearAll();
                InternalDebug.Assert(properties != null);
                count = 0;
                nextSearchIndex = 0;
            }
        }

        internal StyleBuildHelper(FormatStore store)
        {
            this.store = store;
            propertyMask = new PropertyBitMask();
            entries = null; 
            topEntry = 0;
            currentEntry = -1;
            nonFlagPropertiesCount = 0;
        }

        public void Clean()
        {
            propertyMask.ClearAll();
            topEntry = 0;
            currentEntry = -1;
            nonFlagPropertiesCount = 0;
        }

        private void InitializeEntry(int entry, int precedence)
        {
            if (entries == null)
            {
                InternalDebug.Assert(entry == 0);
                entries = new PrecedenceEntry[4];
            }
            else if (entry == entries.Length)
            {
                if (entry == 16)
                {
                    throw new TextConvertersException(Strings.InputDocumentTooComplex);
                }

                var newEntries = new PrecedenceEntry[entries.Length * 2];
                Array.Copy(entries, 0, newEntries, 0, entries.Length);
                entries = newEntries;
            }

            if (entries[entry] == null)
            {
                entries[entry] = new PrecedenceEntry(precedence);
            }
            else
            {
                entries[entry].ReInitialize(precedence);
            }
        }

        private int GetEntry(int precedence)
        {
            var entry = currentEntry;

            if (entry != -1)
            {
                if (entries[entry].precedence != precedence)
                {
                    

                    for (entry = topEntry - 1; entry >= 0; entry--)
                    {
                        if (entries[entry].precedence < precedence)
                        {
                            break;
                        }
                    }

                    entry++;

                    if (entry == topEntry || entries[entry].precedence != precedence)
                    {
                        InternalDebug.Assert(entry == topEntry || entries[entry].precedence > precedence);

                        InitializeEntry(topEntry, precedence);

                        if (entry < topEntry)
                        {
                            var saveEntry = entries[topEntry];

                            for (var i = topEntry - 1; i >= entry; i--)
                            {
                                entries[i + 1] = entries[i];
                            }

                            entries[entry] = saveEntry;
                        }

                        topEntry ++;
                    }
                }
            }
            else
            {
                
                InternalDebug.Assert(topEntry == 0);

                entry = topEntry++;
                InitializeEntry(entry, precedence);
            }

            currentEntry = entry;
            return entry;
        }

        private void AddProperty(int entry, PropertyId id, PropertyValue value)
        {
            int index;

            for (index = entries[entry].count - 1; index >= 0; index--)
            {
                if (entries[entry].properties[index].Id <= id)
                {
                    break;
                }

                entries[entry].properties[index + 1] = entries[entry].properties[index];
            }

            entries[entry].count ++;
            entries[entry].properties[index + 1].Set(id, value);
            entries[entry].propertyMask.Set(id);
            propertyMask.Set(id);
            nonFlagPropertiesCount ++;
        }

        private void RemoveProperty(int entry, int index)
        {
            
            entries[entry].propertyMask.Clear(entries[entry].properties[index].Id);

            if (entries[entry].properties[index].Value.IsRefCountedHandle)
            {
                store.ReleaseValue(entries[entry].properties[index].Value);
            }

            entries[entry].count--;
            for (var i = index; i < entries[entry].count; i++)
            {
                entries[entry].properties[i] = entries[entry].properties[i + 1];
            }
            nonFlagPropertiesCount --;
        }

        private void FindProperty(PropertyId id, out int entryFound, out int indexFound)
        {
            InternalDebug.Assert(propertyMask.IsSet(id));

            for (entryFound = 0; entryFound < topEntry; entryFound++)
            {
                if (entries[entryFound].propertyMask.IsSet(id))
                {
                    

                    var start = 0;
                    var end = entries[entryFound].count;
                    var properties = entries[entryFound].properties;

                    indexFound = entries[entryFound].nextSearchIndex;

                    
                    

                    if (indexFound < end)
                    {
                        if (properties[indexFound].Id == id)
                        {
                            entries[entryFound].nextSearchIndex++;
                            return;
                        }
                        else if (properties[indexFound].Id < id)
                        {
                            start = indexFound + 1;
                        }
                    }

                    for (indexFound = start; indexFound < end; indexFound++)
                    {
                        if (properties[indexFound].Id == id)
                        {
                            entries[entryFound].nextSearchIndex = indexFound + 1;
                            return;
                        }
                    }

                    
                }
            }

            
            
            InternalDebug.Assert(false);
            indexFound = -1;
        }

        private void SetPropertyImpl(int entry, PropertyId id, PropertyValue value)
        {
            if (FlagProperties.IsFlagProperty(id))
            {
                if (!value.IsNull)
                {
                    InternalDebug.Assert(value.IsBool);
                    entries[entry].flagProperties.Set(id, value.Bool);
                }
                else
                {
                    entries[entry].flagProperties.Remove(id);
                }
            }
            else
            {
                if (propertyMask.IsSet(id))
                {
                    

                    int entryFound, indexFound;

                    FindProperty(id, out entryFound, out indexFound);

                    if (entryFound < entry)
                    {
                        
                        return;
                    }

                    if (entryFound == entry)
                    {
                        if (entries[entry].properties[indexFound].Value.IsRefCountedHandle)
                        {
                            store.ReleaseValue(entries[entry].properties[indexFound].Value);
                        }
                        else if (entries[entry].properties[indexFound].Value.IsRelativeHtmlFontUnits && value.IsRelativeHtmlFontUnits)
                        {
                            
                            value = new PropertyValue(PropertyType.RelHtmlFontUnits, entries[entry].properties[indexFound].Value.RelativeHtmlFontUnits + value.RelativeHtmlFontUnits);
                        }

                        entries[entry].properties[indexFound].Value = value;
                        return;
                    }

                    RemoveProperty(entryFound, indexFound);
                }

                if (!value.IsNull)
                {
                    AddProperty(entry, id, value);
                }
            }
        }

        public void SetProperty(int precedence, PropertyId id, PropertyValue value)
        {
            

            InternalDebug.Assert(id > PropertyId.Null);

            var entry = GetEntry(precedence);

            SetPropertyImpl(entry, id, value);
        }

        public void SetProperty(int precedence, Property prop)
        {
            

            InternalDebug.Assert(prop.Id > PropertyId.Null);

            var entry = GetEntry(precedence);

            SetPropertyImpl(entry, prop.Id, prop.Value);
        }

        public void AddStyle(int precedence, int handle)
        {
            InternalDebug.Assert(handle != 0);

            var entry = GetEntry(precedence);

            var style = store.GetStyle(handle);

            entries[entry].flagProperties.Merge(style.FlagProperties);

            var propList = style.PropertyList;
            if (propList != null)
            {
                for (var i = 0; i < propList.Length; i++)
                {
                    var prop = propList[i];

                    if (prop.Value.IsRefCountedHandle)
                    {
                        store.AddRefValue(prop.Value);
                    }

                    SetPropertyImpl(entry, prop.Id, prop.Value);
                }
            }
        }

        public void AddProperties(int precedence, FlagProperties flagProperties, PropertyBitMask propertyMask, Property[] propList)
        {
            var entry = GetEntry(precedence);

            entries[entry].flagProperties.Merge(flagProperties);

            if (propList != null)
            {
                for (var i = 0; i < propList.Length; i++)
                {
                    var prop = propList[i];
                    if (propertyMask.IsSet(prop.Id))
                    {
                        if (prop.Value.IsRefCountedHandle)
                        {
                            store.AddRefValue(prop.Value);
                        }

                        SetPropertyImpl(entry, prop.Id, prop.Value);
                    }
                }
            }
        }

        public PropertyValue GetProperty(PropertyId id)
        {
            if (FlagProperties.IsFlagProperty(id))
            {
                for (var i = 0; i < topEntry; i++)
                {
                    if (entries[i].flagProperties.IsDefined(id))
                    {
                        return entries[i].flagProperties.GetPropertyValue(id);
                    }
                }
            }
            else if (propertyMask.IsSet(id))
            {
                int entryFound, indexFound;

                FindProperty(id, out entryFound, out indexFound);

                return entries[entryFound].properties[indexFound].Value;
            }

            return PropertyValue.Null;
        }

        public void GetPropertyList(
                    out Property[] propertyList,
                    out FlagProperties effectiveFlagProperties,
                    out PropertyBitMask effectivePropertyMask)
        {
            effectiveFlagProperties = new FlagProperties();
            effectivePropertyMask = propertyMask;

            #if DEBUG
            var debugMask = new PropertyBitMask();
            #endif

            for (var i = 0; i < topEntry; i++)
            {
                effectiveFlagProperties.Merge(entries[i].flagProperties);

                #if DEBUG
                debugMask.Or(entries[i].propertyMask);
                #endif
            }

            #if DEBUG
            InternalDebug.Assert(debugMask == propertyMask);
            #endif

            if (nonFlagPropertiesCount != 0)
            {
                InternalDebug.Assert(!propertyMask.IsClear);

                propertyList = new Property[nonFlagPropertiesCount];

                if (topEntry == 1)
                {
                    

                    InternalDebug.Assert(entries[0].count == nonFlagPropertiesCount);
                    Array.Copy(entries[0].properties, 0, propertyList, 0, nonFlagPropertiesCount);
                }
                else if (topEntry == 2)
                {
                    

                    var p0 = entries[0].properties;
                    var p1 = entries[1].properties;
                    int i0 = 0, i1 = 0;
                    var max0 = entries[0].count;
                    var max1 = entries[1].count;
                    var resultIndex = 0;

                    while (true)
                    {
                        if (i0 < max0)
                        {
                            if (i1 == max1 || p0[i0].Id <= p1[i1].Id)
                            {
                                propertyList[resultIndex++] = p0[i0++];
                            }
                            else
                            {
                                propertyList[resultIndex++] = p1[i1++];
                            }
                        }
                        else if (i1 < max1)
                        {
                            propertyList[resultIndex++] = p1[i1++];
                        }
                        else
                        {
                            break;
                        }
                    }
                    
                }
                else
                {
                    

                    var id = PropertyId.LastFlag;
                    var resultIndex = 0;

                    while (++id < PropertyId.MaxValue)
                    {
                        if (propertyMask.IsSet(id))
                        {
                            int entryFound, indexFound;

                            FindProperty(id, out entryFound, out indexFound);

                            propertyList[resultIndex++] = entries[entryFound].properties[indexFound];
                        }
                    }

                    InternalDebug.Assert(resultIndex == propertyList.Length);
                }
            }
            else
            {
                propertyList = null;
            }

            Clean();
        }
    }

#if false
    

    internal struct StringBuildHelper
    {
        internal FormatStore store;
        internal char[] buffer;
        internal int count;
        
        internal StringBuildHelper(FormatStore store)
        {
            this.store = store;
            this.buffer = null; 
            this.count = 0;
        }

        public void AddText(char[] buffer, int offset, int count)
        {
            
            if (this.count + count > this.buffer.Length)
            {
                throw new Microsoft.Exchange.Data.TextConverters.TextConvertersException();
            }

            Buffer.BlockCopy(buffer, offset * 2, this.buffer, this.count * 2, count * 2);
            this.count += count;
        }

        public void AddText(string value)
        {
            
            if (this.count + value.Length > this.buffer.Length)
            {
                throw new Microsoft.Exchange.Data.TextConverters.TextConvertersException();
            }

            value.CopyTo(0, this.buffer, this.count, value.Length);
            this.count += value.Length;
        }

        public string GetString()
        {
            if (this.count == 0)
            {
                return null;
            }

            string result = new string(this.buffer, 0, this.count);
            this.count = 0;
            return result;
        }
    }
#endif

    

    internal struct MultiValueBuildHelper
    {
        internal FormatStore store;
        internal PropertyValue[] values;
        internal int valuesCount;

        internal MultiValueBuildHelper(FormatStore store)
        {
            this.store = store;
            values = null;
            valuesCount = 0;
        }

        public int Count => valuesCount;
        public PropertyValue this[int i] => values[i];

        public void AddValue(PropertyValue value)
        {
            
            InternalDebug.Assert(!value.IsMultiValue);

            if (values == null)
            {
                values = new PropertyValue[4];
            }
            else if (valuesCount == values.Length)
            {
                if (valuesCount == 32)
                {
                    throw new TextConvertersException(Strings.InputDocumentTooComplex);
                }

                var newValues = new PropertyValue[valuesCount * 2];
                Array.Copy(values, 0, newValues, 0, valuesCount);
                values = newValues;
            }

            values[valuesCount++] = value;
        }

        public PropertyValue[] GetValues()
        {
            if (valuesCount == 0)
            {
                return null;
            }

            var result = new PropertyValue[valuesCount];
            Array.Copy(values, 0, result, 0, valuesCount);
            valuesCount = 0;
            return result;
        }

        public void Cancel()
        {
            for (var i = 0; i < valuesCount; i++)
            {
                if (values[i].IsRefCountedHandle)
                {
                    store.ReleaseValue(values[i]);
                }
            }
            valuesCount = 0;
        }
    }
}

