// ***************************************************************
// <copyright file="FormatStore.cs" company="Microsoft">
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
    using System.Text;
    using System.Runtime.InteropServices;
    using System.Collections;
    using System.Collections.Generic;
    using Data.Internal;
    using Globalization;
    using Strings = CtsResources.TextConvertersStrings;

    

    internal enum FormatContainerType : byte
    {
        Null                    = 0x00,                 

        Root                    = 0x01 | BlockFlag,     

        Document                = 0x02 | BlockFlag,     
        Fragment                = 0x03 | BlockFlag,     

        Block                   = 0x04 | BlockFlag,     
        BlockQuote              = 0x05 | BlockFlag,     

        HorizontalLine          = 0x06 | BlockFlag,     

        TableContainer          = 0x07,                 
        TableDefinition         = 0x08,                 
        TableColumnGroup        = 0x09,                 
        TableColumn             = 0x0A,                 
        TableCaption            = 0x0B | BlockFlag,     
        TableExtraContent       = 0x0C | BlockFlag,     
        Table                   = 0x0D | BlockFlag,     
        TableRow                = 0x0E | BlockFlag,     
        TableCell               = 0x0F | BlockFlag,     

        List                    = 0x10 | BlockFlag,     
        ListItem                = 0x11 | BlockFlag,     
        
                                                        
                                                        

        Inline                  = 0x12,                 

        HyperLink               = 0x13,                 
                                                        

        Bookmark                = 0x14,                 

        Image                   = 0x15 | InlineObjectFlag,  

        Area                    = 0x16,
        Map                     = 0x17 | BlockFlag,

        BaseFont                = 0x18,

        Form                    = 0x19,
        FieldSet                = 0x1A,
        Label                   = 0x1B,
        Input                   = 0x1C,
        Button                  = 0x1D,
        Legend                  = 0x1E,
        TextArea                = 0x1F,
        Select                  = 0x20,
        OptionGroup             = 0x21,
        Option                  = 0x22,

        Text                    = 0x24,                 
                                                        

        PropertyContainer       = 0x25,                 
                                                        
                                                        

        InlineObjectFlag        = 0x40,                 
        BlockFlag               = 0x80,                 
    }

    

    internal class FormatStore
    {
        
        internal NodeStore nodes;   

        
        internal StyleStore styles;

        
        internal StringValueStore strings;   

        
        internal MultiValueStore multiValues;

        internal TextStore text;

        public FormatStore()
        {
            nodes = new NodeStore(this);

            styles = new StyleStore(this, FormatStoreData.GlobalStyles);

            strings = new StringValueStore(FormatStoreData.GlobalStringValues);

            multiValues = new MultiValueStore(this, FormatStoreData.GlobalMultiValues);

            text = new TextStore();

            Initialize();
        }

        public void Initialize()
        {
            nodes.Initialize();

            styles.Initialize(FormatStoreData.GlobalStyles);

            strings.Initialize(FormatStoreData.GlobalStringValues);

            multiValues.Initialize(FormatStoreData.GlobalMultiValues);

            text.Initialize();
        }

        public FormatNode RootNode => new FormatNode(this, 1);

        public uint CurrentTextPosition => text.CurrentPosition;

        public void ReleaseValue(PropertyValue value)
        {
            InternalDebug.Assert(value.IsRefCountedHandle);
            if (value.IsString)
            {
                GetStringValue(value).Release();
            }
            else if (value.IsMultiValue)
            {
                GetMultiValue(value).Release();
            }
        }

        public void AddRefValue(PropertyValue value)
        {
            InternalDebug.Assert(value.IsRefCountedHandle);
            if (value.IsString)
            {
                GetStringValue(value).AddRef();
            }
            else if (value.IsMultiValue)
            {
                GetMultiValue(value).AddRef();
            }
        }

        public FormatNode GetNode(int nodeHandle)
        {
            return new FormatNode(this, nodeHandle);
        }

        public FormatNode AllocateNode(FormatContainerType type)
        {
            return new FormatNode(nodes, nodes.Allocate(type, CurrentTextPosition));
        }

        public FormatNode AllocateNode(FormatContainerType type, uint beginTextPosition)
        {
            return new FormatNode(nodes, nodes.Allocate(type, beginTextPosition));
        }

        public void FreeNode(FormatNode node)
        {
            nodes.Free(node.Handle);
        }

        public FormatStyle AllocateStyle(bool isStatic)
        {
            return new FormatStyle(this, styles.Allocate(isStatic));
        }

        public FormatStyle GetStyle(int styleHandle)
        {
            return new FormatStyle(this, styleHandle);
        }

        public void FreeStyle(FormatStyle style)
        {
            InternalDebug.Assert(style.RefCount == 0);
            styles.Free(style.Handle);
        }

        public StringValue AllocateStringValue(bool isStatic)
        {
            return new StringValue(this, strings.Allocate(isStatic));
        }

        public StringValue AllocateStringValue(bool isStatic, string value)
        {
            var sv = AllocateStringValue(isStatic);
            sv.SetString(value);
            return sv;
        }

        public StringValue GetStringValue(PropertyValue propertyValue)
        {
            return new StringValue(this, propertyValue.StringHandle);
        }

        public void FreeStringValue(StringValue str)
        {
            InternalDebug.Assert(str.RefCount == 0);
            strings.Free(str.Handle);
        }

        public MultiValue AllocateMultiValue(bool isStatic)
        {
            return new MultiValue(this, multiValues.Allocate(isStatic));
        }

        public MultiValue GetMultiValue(PropertyValue propertyValue)
        {
            return new MultiValue(this, propertyValue.MultiValueHandle);
        }

        public void FreeMultiValue(MultiValue multi)
        {
            InternalDebug.Assert(multi.RefCount == 0);
            multiValues.Free(multi.Handle);
        }

        

        public void InitializeCodepageDetector()
        {
            text.InitializeCodepageDetector();
        }

        public int GetBestWindowsCodePage()
        {
            return text.GetBestWindowsCodePage();
        }

        public int GetBestWindowsCodePage(int preferredCodePage)
        {
            return text.GetBestWindowsCodePage(preferredCodePage);
        }

        

        public void SetTextBoundary()
        {
            
            text.DoNotMergeNextRun();
        }

        

        public void AddBlockBoundary()
        {
            if (text.LastRunType != TextRunType.BlockBoundary)
            {
                text.AddSimpleRun(TextRunType.BlockBoundary, 1);
            }
        }

        

        public void AddText(char[] textBuffer, int offset, int count)
        {
            text.AddText(TextRunType.NonSpace, textBuffer, offset, count);
        }

        public void AddInlineObject()
        {
            text.AddSimpleRun(TextRunType.InlineObject, 1);
            text.DoNotMergeNextRun(); 
        }

        public void AddSpace(int count)
        {
            text.AddSimpleRun(TextRunType.Space, count);
        }

        public void AddLineBreak(int count)
        {
            text.AddSimpleRun(TextRunType.NewLine, count);
        }

        public void AddNbsp(int count)
        {
            text.AddSimpleRun(TextRunType.NbSp, count);
        }

        public void AddTabulation(int count)
        {
            text.AddSimpleRun(TextRunType.Tabulation, count);
        }

        internal TextRun GetTextRun(uint position)
        {
            InternalDebug.Assert(position <= CurrentTextPosition);

            if (position < CurrentTextPosition)
            {
                return GetTextRunReally(position);
            }

            return TextRun.Invalid;
        }

        internal TextRun GetTextRunReally(uint position)
        {
            return new TextRun(text, position);
        }

        
#if DEBUG
        internal void Dump(Stream stream, bool close)
        {
            var dumpWriter = new StreamWriter(stream);
            uint runningPosition = 0;

            DumpNode(dumpWriter, RootNode, 0, false, ref runningPosition);

            CalculateAndDumpStatistics(dumpWriter);

            if (close)
            {
                dumpWriter.Close();
            }
            else
            {
                dumpWriter.Flush();
            }
        }

        private static void DumpIndent(TextWriter dumpWriter, int level)
        {
            if (level < 20)
            {
                for (var i = 0; i < level; i++)
                {
                    dumpWriter.Write("   ");
                }
            }
            else
            {
                for (var i = 0; i < level; i++)
                {
                    dumpWriter.Write("   ");
                }
                dumpWriter.Write("{0}: ", level);
            }
        }

        private void DumpNode(TextWriter dumpWriter, FormatNode node, int level, bool outOfOrder, ref uint runningPosition)
        {
            DumpIndent(dumpWriter, level);

            dumpWriter.WriteLine("{0} /{1:X}/ tp:/{2:X}/{3:X}/{4}", node.NodeType.ToString(), node.Handle, node.BeginTextPosition, node.EndTextPosition, node.IsInOrder ? "" : "  out-of-order!");

            var enumerator = new NodePropertiesEnumerator(node);
            foreach (var prop in enumerator)
            {
                DumpIndent(dumpWriter, level + 1);

                dumpWriter.WriteLine("| PROP " + prop.ToString());

                if (prop.Value.IsString)
                {
                    DumpIndent(dumpWriter, level + 1);

                    var sv = GetStringValue(prop.Value);
                    dumpWriter.WriteLine("|   = \"" + sv.GetString() + "\"");
                }
                else if (prop.Value.IsMultiValue)
                {
                    var mv = GetMultiValue(prop.Value);

                    for (var k = 0; k < mv.Length; k++)
                    {
                        DumpIndent(dumpWriter, level + 1);

                        dumpWriter.WriteLine("|   = " + mv[k].ToString());

                        if (mv[k].IsString)
                        {
                            DumpIndent(dumpWriter, level + 1);

                            var sv = GetStringValue(mv[k]);
                            dumpWriter.WriteLine("|      = \"" + sv.GetString() + "\"");
                        }
                    }
                }
            }
#if false
            if (!outOfOrder && !node.IsText && node.IsInOrder)
            {
                this.DumpStartMarkup(dumpWriter, node, level, ref runningPosition);
            }
#endif
            if (node.IsText)
            {
                
                DumpTextRuns(node.BeginTextPosition, node.EndTextPosition, dumpWriter, level);
                if (!outOfOrder)
                {
                    runningPosition = node.EndTextPosition;
                }
            }
            else
            {
                var child = node.FirstChild;
                while (!child.IsNull)
                {
                    DumpNode(dumpWriter, child, level + 1, outOfOrder || !node.IsInOrder, ref runningPosition);
                    child = child.NextSibling;
                }
            }
#if false
            if (!outOfOrder && node.IsInOrder && !node.Parent.IsNull)
            {
                this.DumpEndMarkup(dumpWriter, node, level, ref runningPosition);
            }
#endif
        }
#if false
        private void DumpStartMarkup(TextWriter dumpWriter, FormatNode node, int level, ref uint runningPosition)
        {
            uint start = node.BeginTextPosition;
            uint end = start;

            FormatNode firstChild = node.FirstChild;
            while (!firstChild.IsNull && !firstChild.IsInOrder)
            {
                firstChild = firstChild.NextSibling;
            }

            if (!firstChild.IsNull)
            {
                end = firstChild.BeginTextPosition;
            }
            else
            {
                end = node.EndTextPosition;
            }

            InternalDebug.Assert(start == runningPosition);

            if (start != end)
            {
                InternalDebug.Assert(start < end);
                this.DumpTextRuns(start, end, dumpWriter, level);
                runningPosition = end;
            }
        }

        private void DumpEndMarkup(TextWriter dumpWriter, FormatNode node, int level, ref uint runningPosition)
        {
            uint start = node.EndTextPosition;
            uint end = start;

            FormatNode nextSibling = node.NextSibling;
            while (!nextSibling.IsNull && !nextSibling.IsInOrder)
            {
                nextSibling = nextSibling.NextSibling;
            }

            if (!nextSibling.IsNull)
            {
                end = nextSibling.BeginTextPosition;
            }
            else
            {
                end = node.Parent.EndTextPosition;
            }

            InternalDebug.Assert(start <= runningPosition);

            if (start < end)
            {
                if (end >= runningPosition)
                {
                    if (start < runningPosition)
                    {
                        DumpIndent(dumpWriter, level + 1);
                        dumpWriter.WriteLine("| OVERLAPPED JUMP FORWARD FROM {0:X} TO {1:X}", start, runningPosition);
                        dumpWriter.Flush();

                        start = runningPosition;
                        InternalDebug.Assert(start <= end);
                    }

                    if (start != end)
                    {
                        this.DumpTextRuns(start, end, dumpWriter, level);
                        runningPosition = end;
                    }
                }
            }
            else if (start != end)
            {
                DumpIndent(dumpWriter, level + 1);
                dumpWriter.WriteLine("| OVERLAPPED STEP BACK FROM {0:X} TO {1:X}", start, end);
            }
        }
#endif
        private void DumpTextRuns(uint start, uint end, TextWriter dumpWriter, int level)
        {
            char[] buffer;
            int offset;
            int count;

            DumpIndent(dumpWriter, level + 1);
            dumpWriter.Write("| ");

            var saveStart = start;
            var newLine = false;

            if (start != end)
            {
                var run = GetTextRun(start);

                do
                {
                    if (run.Position != saveStart && !newLine)
                    {
                        dumpWriter.Write(", ");
                    }
                    else if (newLine)
                    {
                        dumpWriter.WriteLine();
                        DumpIndent(dumpWriter, level + 1);
                        dumpWriter.Write("|           ");
                        newLine = false;
                    }

                    switch (run.Type)
                    {
                        case TextRunType.NewLine:

                            dumpWriter.Write("nl({0})", run.EffectiveLength);
                            newLine = true;
                            break;

                        case TextRunType.Tabulation:

                            dumpWriter.Write("tab({0})", run.EffectiveLength);
                            break;

                        case TextRunType.Space:

                            dumpWriter.Write("sp({0})", run.EffectiveLength);
                            break;

                        case TextRunType.NbSp:

                            dumpWriter.Write("nbsp({0})", run.EffectiveLength);
                            break;
                                
                        case TextRunType.NonSpace:

                            var pos = 0;

                            dumpWriter.Write("\"");

                            while (pos != run.EffectiveLength)
                            {
                                run.GetChunk(pos, out buffer, out offset, out count);
                                dumpWriter.Write(buffer, offset, count);
                                pos += count;
                            }

                            dumpWriter.Write("\"");
                            break;
                    }

                    run.MoveNext();
                }
                while (run.Position < end);
            }

            dumpWriter.WriteLine();
        }

#if PRIVATEBUILD
        internal long bytesTotal;
#endif
        public long CalculateAndDumpStatistics(TextWriter dumpWriter)
        {
#if !PRIVATEBUILD
            long bytesTotal;
#endif
            bytesTotal = 0;

            bytesTotal += text.DumpStat(dumpWriter);

            if (dumpWriter != null)
            {
                dumpWriter.WriteLine();
            }

            bytesTotal += nodes.DumpStat(dumpWriter);

            if (dumpWriter != null)
            {
                dumpWriter.WriteLine();
            }

            bytesTotal += styles.DumpStat(dumpWriter);

            if (dumpWriter != null)
            {
                dumpWriter.WriteLine();
            }

            bytesTotal += strings.DumpStat(dumpWriter);

            if (dumpWriter != null)
            {
                dumpWriter.WriteLine();
            }

            bytesTotal += multiValues.DumpStat(dumpWriter);

            if (dumpWriter != null)
            {
                dumpWriter.WriteLine();
                dumpWriter.WriteLine("Total bytes for store: {0}", bytesTotal);
            }

            return bytesTotal;
        }
#endif
        

        [Flags]
        internal enum NodeFlags : byte
        {
            OnRightEdge = 0x01,         
            OnLeftEdge = 0x02,          
            CanFlush = 0x04,            
            OutOfOrder = 0x08,          
            Visited = 0x10,             
                                        
        }

        [StructLayout(LayoutKind.Sequential, Pack=4)]
        internal struct NodeEntry
        {
            
            

            internal FormatContainerType type;
            internal NodeFlags nodeFlags;
            internal TextMapping textMapping;

            internal int parent;
            internal int lastChild;              
            internal int nextSibling;            

            internal uint beginTextPosition;
            internal uint endTextPosition;

            internal int inheritanceMaskIndex;
            internal FlagProperties flagProperties;          
            internal PropertyBitMask propertyMask;           
            internal Property[] properties;                  

            
            internal int NextFree
            {
                get { return nextSibling; }
                set { nextSibling = value; }
            }

            public void Clean()
            {
                this = new NodeEntry();
            }

            public override string ToString()
            {
                return type.ToString() + " (" + parent.ToString("X") + ", " + lastChild.ToString("X") + ", " + nextSibling.ToString("X") + ") " + beginTextPosition.ToString("X") + " - " + endTextPosition.ToString("X");
            }
        }

        

        internal struct StyleEntry
        {
            internal int refCount;
            internal FlagProperties flagProperties;          
            internal PropertyBitMask propertyMask;           
            internal Property[] propertyList;                

            public StyleEntry(FlagProperties flagProperties, PropertyBitMask propertyMask, Property[] propertyList)
            {
                refCount = Int32.MaxValue;     
                this.flagProperties = flagProperties;
                this.propertyMask = propertyMask;
                this.propertyList = propertyList;
            }

            
            internal int NextFree
            {
                get { return flagProperties.IntegerBag; }
                set { flagProperties.IntegerBag = value; }
            }

            public void Clean()
            {
                this = new StyleEntry();
            }
        }

        

        internal struct MultiValueEntry
        {
            internal int refCount;
            internal int nextFree;                           
            internal PropertyValue[] values;                

            public MultiValueEntry(PropertyValue[] values)
            {
                refCount = Int32.MaxValue;     
                this.values = values;
                nextFree = 0;
            }

            
            
            internal int NextFree
            {
                get { return nextFree; }
                set { nextFree = value; }
            }

            public void Clean()
            {
                this = new MultiValueEntry();
            }
        }

        

        internal struct StringValueEntry
        {
            internal int refCount;
            internal int nextFree;                           
            internal string str;                            

            public StringValueEntry(string str)
            {
                refCount = Int32.MaxValue;     
                this.str = str;
                nextFree = 0;
            }
     
            
            
            internal int NextFree
            {
                get { return nextFree; }
                set { nextFree = value; }
            }

            public void Clean()
            {
                this = new StringValueEntry();
            }
        }

        internal struct InheritaceMask
        {
            internal FlagProperties flagProperties;          
            internal PropertyBitMask propertyMask;           

            public InheritaceMask(FlagProperties flagProperties, PropertyBitMask propertyMask)
            {
                this.flagProperties = flagProperties;
                this.propertyMask = propertyMask;
            }
        }

        

        internal class NodeStore
        {
            
            
            
            internal const int MaxElementsPerPlane = 1024;

            
            
            
            internal const int MaxPlanes = (1024 * 1024) / MaxElementsPerPlane;

            
            internal const int InitialPlanes = 32;

            
            internal const int InitialElements = 16;

            private FormatStore store;
            private NodeEntry[][] planes;
            private int freeListHead;
            private int top;

#if PRIVATEBUILD
            internal long countAllocated;
            internal long countUsed;
            internal long countPropLists;
            internal long totalPropListsLength;
            internal long averagePropListLength;
            internal long maxPropListLength;
            internal long bytesTotal;
#endif
            public NodeStore(FormatStore store)
            {
                InternalDebug.Assert(InitialElements < MaxElementsPerPlane);
                InternalDebug.Assert(Marshal.SizeOf(typeof(NodeEntry)) * MaxElementsPerPlane < 85000, "exceeding LOH threshold!");

                this.store = store;
                planes = new NodeEntry[InitialPlanes][];       
                planes[0] = new NodeEntry[InitialElements];
                freeListHead = 0;
                top = 0;
            }

            public NodeEntry[] Plane(int handle) { return planes[handle / MaxElementsPerPlane]; }
            public int Index(int handle) { return handle % MaxElementsPerPlane; }

            public void Initialize()
            {
                

                freeListHead = -1;

                top = 1;          

                
                planes[0][1].type = FormatContainerType.Root;
                planes[0][1].nodeFlags = NodeFlags.OnRightEdge | NodeFlags.CanFlush;
                planes[0][1].textMapping = TextMapping.Unicode;
                planes[0][1].parent = 0;
                planes[0][1].nextSibling = 1;
                planes[0][1].lastChild = 0;
                planes[0][1].beginTextPosition = 0;
                planes[0][1].endTextPosition = uint.MaxValue;
                planes[0][1].flagProperties = new FlagProperties();
                planes[0][1].propertyMask = new PropertyBitMask();
                planes[0][1].properties = null;

                top ++;
            }

            public int Allocate(FormatContainerType type, uint currentTextPosition)
            {
                InternalDebug.Assert(planes != null);

                NodeEntry[] plane;
                int index;

                var handle = freeListHead;

                if (handle != -1)
                {
                    

                    index = handle % MaxElementsPerPlane;
                    plane = planes[handle / MaxElementsPerPlane];

                    freeListHead = plane[index].NextFree;
                }
                else
                {
                    InternalDebug.Assert(top <= planes.Length * MaxElementsPerPlane);

                    handle = top++;

                    index = handle % MaxElementsPerPlane;
                    var planeIndex = handle / MaxElementsPerPlane;

                    if (index == 0)
                    {
                        

                        

                        if (planeIndex == MaxPlanes)
                        {
                            
                            throw new TextConvertersException(Strings.InputDocumentTooComplex);
                        }

                        if (planeIndex == planes.Length)
                        {
                            

                            var newLength = Math.Min(planes.Length * 2, MaxPlanes);
                            var newPlanes = new NodeEntry[newLength][];
                            Array.Copy(planes, 0, newPlanes, 0, planes.Length);
                            planes = newPlanes;
                        }

                        if (planes[planeIndex] == null)
                        {
                            

                            planes[planeIndex] = new NodeEntry[MaxElementsPerPlane];
                        }
                    }
                    else if (planeIndex == 0 && index == planes[planeIndex].Length)
                    {
                        

                        var newLength = Math.Min(planes[0].Length * 2, MaxElementsPerPlane);
                        var newPlane = new NodeEntry[newLength];
                        Array.Copy(planes[0], 0, newPlane, 0, planes[0].Length);
                        planes[0] = newPlane;
                    }

                    plane = planes[planeIndex];
                }

                
                plane[index].type = type;
                plane[index].nodeFlags = 0;
                plane[index].textMapping = TextMapping.Unicode;
                plane[index].parent = 0;
                plane[index].lastChild = 0;
                plane[index].nextSibling = handle;
                plane[index].beginTextPosition = currentTextPosition;
                plane[index].endTextPosition = uint.MaxValue;
                plane[index].flagProperties.ClearAll();
                plane[index].propertyMask.ClearAll();
                plane[index].properties = null;

                return handle;
            }

            public void Free(int handle)
            {
                InternalDebug.Assert(planes != null &&
                    handle / MaxElementsPerPlane < planes.Length &&
                    planes[handle / MaxElementsPerPlane] != null &&
                    handle %  MaxElementsPerPlane < planes[handle / MaxElementsPerPlane].Length);

                var index = handle % MaxElementsPerPlane;
                var plane = planes[handle / MaxElementsPerPlane];

                

                if (null != plane[index].properties)
                {
                    for (var i = 0; i < plane[index].properties.Length; i++)
                    {
                        if (plane[index].properties[i].Value.IsRefCountedHandle)
                        {
                            store.ReleaseValue(plane[index].properties[i].Value);
                        }
                    }
                }

                plane[index].NextFree = freeListHead;
                freeListHead = handle;
            }

            public long DumpStat(TextWriter dumpWriter)
            {
#if !PRIVATEBUILD
                long countAllocated;
                long countUsed;
                long countPropLists;
                long totalPropListsLength;
                long averagePropListLength;
                long maxPropListLength;
                long bytesTotal;
#endif
                var numPlanes = (top < MaxElementsPerPlane) ? 1 : ((top % MaxElementsPerPlane == 0) ? (top / MaxElementsPerPlane) : (top / MaxElementsPerPlane + 1));
                countAllocated = (numPlanes == 1) ? planes[0].Length : (numPlanes * MaxElementsPerPlane);
                bytesTotal = 12 + planes.Length * 4 + 12 * numPlanes  + countAllocated * Marshal.SizeOf(typeof(NodeEntry));
                countUsed = 0;
                countPropLists = 0;
                totalPropListsLength = 0;
                maxPropListLength = 0;

                NodeEntry[] plane;
                int index;

                for (var i = 0; i < top; i++)
                {
                    InternalDebug.Assert(planes != null &&
                        i / MaxElementsPerPlane < planes.Length &&
                        planes[i / MaxElementsPerPlane] != null &&
                        i % MaxElementsPerPlane < planes[i / MaxElementsPerPlane].Length);

                    index = i % MaxElementsPerPlane;
                    plane = planes[i / MaxElementsPerPlane];

                    if (plane[index].type != FormatContainerType.Null)
                    {
                        countUsed ++;

                        if (plane[index].properties != null)
                        {
                            bytesTotal += 12 + plane[index].properties.Length * Marshal.SizeOf(typeof(Property));

                            countPropLists ++;
                            totalPropListsLength += plane[index].properties.Length;
                            if (plane[index].properties.Length > maxPropListLength)
                            {
                                maxPropListLength = plane[index].properties.Length;
                            }
                        }
                    }
                }

                averagePropListLength = countPropLists == 0 ? 0 : (totalPropListsLength + countPropLists - 1) / countPropLists;

                #if DEBUG
                var countFree = 0;
                var next = freeListHead;

                while (next != -1)
                {
                    index = next % MaxElementsPerPlane;
                    plane = planes[next / MaxElementsPerPlane];

                    countFree ++;
                    next = plane[index].NextFree;
                }

                InternalDebug.Assert(countUsed == top - 1 - countFree);
                #endif

                if (dumpWriter != null)
                {
                    dumpWriter.WriteLine("Nodes alloc: {0}", countAllocated);
                    dumpWriter.WriteLine("Nodes used: {0}", countUsed);
                    dumpWriter.WriteLine("Nodes proplists: {0}", countPropLists);
                    if (countPropLists != 0)
                    {
                        dumpWriter.WriteLine("Nodes props: {0}", totalPropListsLength);
                        dumpWriter.WriteLine("Nodes average proplist: {0}", averagePropListLength);
                        dumpWriter.WriteLine("Nodes max proplist: {0}", maxPropListLength);
                    }
                    dumpWriter.WriteLine("Nodes bytes: {0}", bytesTotal);
                }

                return bytesTotal;
            }
        }

        

        internal class StyleStore
        {
            
            
            
            internal const int MaxElementsPerPlane = 2048;

            
            
            
            internal const int MaxPlanes = (1024 * 1024) / MaxElementsPerPlane;

            
            internal const int InitialPlanes = 16;

            
            internal const int InitialElements = 32;

            private FormatStore store;
            private StyleEntry[][] planes;
            private int freeListHead;
            private int top;

#if PRIVATEBUILD
            internal long countAllocated;
            internal long countUsed;
            internal long countPropLists;
            internal long totalPropListsLength;
            internal long averagePropListLength;
            internal long maxPropListLength;
            internal long bytesTotal;
#endif
            public StyleStore(FormatStore store, StyleEntry[] globalStyles)
            {
                InternalDebug.Assert(InitialElements < MaxElementsPerPlane);
                InternalDebug.Assert(Marshal.SizeOf(typeof(StyleEntry)) * MaxElementsPerPlane < 85000, "exceeding LOH threshold!");
                InternalDebug.Assert(globalStyles.Length < MaxElementsPerPlane);

                this.store = store;
                planes = new StyleEntry[InitialPlanes][];       
                planes[0] = new StyleEntry[Math.Max(InitialElements, globalStyles.Length + 1)];
                freeListHead = 0;
                top = 0;

                if (globalStyles != null && globalStyles.Length != 0)
                {
                    Array.Copy(globalStyles, 0, planes[0], 0, globalStyles.Length);
                }
            }

            public StyleEntry[] Plane(int handle) { return planes[handle / MaxElementsPerPlane]; }
            public int Index(int handle) { return handle % MaxElementsPerPlane; }

            public void Initialize(StyleEntry[] globalStyles)
            {
                

                freeListHead = -1;

                if (globalStyles != null && globalStyles.Length != 0)
                {
                    top = globalStyles.Length;
                }
                else
                {
                    top = 1;          
                }
            }

            public int Allocate(bool isStatic)
            {
                InternalDebug.Assert(planes != null);

                StyleEntry[] plane;
                int index;

                var handle = freeListHead;

                if (handle != -1)
                {
                    

                    index = handle % MaxElementsPerPlane;
                    plane = planes[handle / MaxElementsPerPlane];

                    freeListHead = plane[index].NextFree;
                }
                else
                {
                    InternalDebug.Assert(top <= planes.Length * MaxElementsPerPlane);

                    handle = top++;

                    index = handle % MaxElementsPerPlane;
                    var planeIndex = handle / MaxElementsPerPlane;

                    if (index == 0)
                    {
                        

                        

                        if (planeIndex == MaxPlanes)
                        {
                            
                            throw new TextConvertersException(Strings.InputDocumentTooComplex);
                        }

                        if (planeIndex == planes.Length)
                        {
                            

                            var newLength = Math.Min(planes.Length * 2, MaxPlanes);
                            var newPlanes = new StyleEntry[newLength][];
                            Array.Copy(planes, 0, newPlanes, 0, planes.Length);
                            planes = newPlanes;
                        }

                        if (planes[planeIndex] == null)
                        {
                            

                            planes[planeIndex] = new StyleEntry[MaxElementsPerPlane];
                        }
                    }
                    else if (planeIndex == 0 && index == planes[planeIndex].Length)
                    {
                        

                        var newLength = Math.Min(planes[0].Length * 2, MaxElementsPerPlane);
                        var newPlane = new StyleEntry[newLength];
                        Array.Copy(planes[0], 0, newPlane, 0, planes[0].Length);
                        planes[0] = newPlane;
                    }

                    plane = planes[planeIndex];
                }

                
                plane[index].propertyList = null;
                plane[index].refCount = isStatic ? Int32.MaxValue : 1;
                plane[index].flagProperties.ClearAll();
                plane[index].propertyMask.ClearAll();

                return handle;
            }

            public void Free(int handle)
            {
                InternalDebug.Assert(planes != null &&
                    handle / MaxElementsPerPlane < planes.Length &&
                    planes[handle / MaxElementsPerPlane] != null &&
                    handle %  MaxElementsPerPlane < planes[handle / MaxElementsPerPlane].Length);

                var index = handle % MaxElementsPerPlane;
                var plane = planes[handle / MaxElementsPerPlane];

                if (null != plane[index].propertyList)
                {
                    for (var i = 0; i < plane[index].propertyList.Length; i++)
                    {
                        if (plane[index].propertyList[i].Value.IsRefCountedHandle)
                        {
                            store.ReleaseValue(plane[index].propertyList[i].Value);
                        }
                    }
                }

                plane[index].NextFree = freeListHead;
                freeListHead = handle;
            }

            public long DumpStat(TextWriter dumpWriter)
            {
#if !PRIVATEBUILD
                long countAllocated;
                long countUsed;
                long countPropLists;
                long totalPropListsLength;
                long averagePropListLength;
                long maxPropListLength;
                long bytesTotal;
#endif
                var numPlanes = (top < MaxElementsPerPlane) ? 1 : ((top % MaxElementsPerPlane == 0) ? (top / MaxElementsPerPlane) : (top / MaxElementsPerPlane + 1));
                countAllocated = (numPlanes == 1) ? planes[0].Length : (numPlanes * MaxElementsPerPlane);
                bytesTotal = 12 + planes.Length * 4 + 12 * numPlanes  + countAllocated * Marshal.SizeOf(typeof(StyleEntry));
                countUsed = 0;
                countPropLists = 0;
                totalPropListsLength = 0;
                maxPropListLength = 0;

                StyleEntry[] plane;
                int index;

                for (var i = 0; i < top; i++)
                {
                    InternalDebug.Assert(planes != null &&
                        i / MaxElementsPerPlane < planes.Length &&
                        planes[i / MaxElementsPerPlane] != null &&
                        i % MaxElementsPerPlane < planes[i / MaxElementsPerPlane].Length);

                    index = i % MaxElementsPerPlane;
                    plane = planes[i / MaxElementsPerPlane];

                    if (plane[index].refCount != 0)
                    {
                        countUsed ++;

                        if (plane[index].propertyList != null)
                        {
                            bytesTotal += 12 + plane[index].propertyList.Length * Marshal.SizeOf(typeof(Property));

                            countPropLists ++;
                            totalPropListsLength += plane[index].propertyList.Length;
                            if (plane[index].propertyList.Length > maxPropListLength)
                            {
                                maxPropListLength = plane[index].propertyList.Length;
                            }
                        }
                    }
                }

                averagePropListLength = countPropLists == 0 ? 0 : (totalPropListsLength + countPropLists - 1) / countPropLists;

                #if DEBUG
                var countFree = 0;
                var next = freeListHead;

                while (next != -1)
                {
                    index = next % MaxElementsPerPlane;
                    plane = planes[next / MaxElementsPerPlane];

                    countFree ++;
                    next = plane[index].NextFree;
                }

                InternalDebug.Assert(countUsed == top - 1 - countFree);
                #endif

                if (dumpWriter != null)
                {
                    dumpWriter.WriteLine("Styles alloc: {0}", countAllocated);
                    dumpWriter.WriteLine("Styles used: {0}", countUsed);
                    dumpWriter.WriteLine("Styles non-null prop lists: {0}", countPropLists);
                    if (countPropLists != 0)
                    {
                        dumpWriter.WriteLine("Styles total prop lists length: {0}", totalPropListsLength);
                        dumpWriter.WriteLine("Styles average prop list length: {0}", averagePropListLength);
                        dumpWriter.WriteLine("Styles max prop list length: {0}", maxPropListLength);
                    }
                    dumpWriter.WriteLine("Styles bytes: {0}", bytesTotal);
                }

                return bytesTotal;
            }
        }

        

        internal class StringValueStore
        {
            
            
            
            internal const int MaxElementsPerPlane = 4096;

            
            
            
            internal const int MaxPlanes = (1024 * 1024) / MaxElementsPerPlane;

            
            internal const int InitialPlanes = 16;

            
            internal const int InitialElements = 16;

            private StringValueEntry[][] planes;
            private int freeListHead;
            private int top;

#if PRIVATEBUILD
            internal long countAllocated;
            internal long countUsed;
            internal long countNonNullStrings;
            internal long totalStringsLength;
            internal long averageStringLength;
            internal long maxStringLength;
            internal long bytesTotal;
#endif
            public StringValueStore(StringValueEntry[] globalStrings)
            {
                InternalDebug.Assert(InitialElements < MaxElementsPerPlane);
                InternalDebug.Assert(Marshal.SizeOf(typeof(StringValueEntry)) * MaxElementsPerPlane < 85000, "exceeding LOH threshold!");
                InternalDebug.Assert(globalStrings.Length < MaxElementsPerPlane);

                planes = new StringValueEntry[InitialPlanes][];       
                planes[0] = new StringValueEntry[Math.Max(InitialElements, globalStrings.Length + 1)];
                freeListHead = 0;
                top = 0;

                if (globalStrings != null && globalStrings.Length != 0)
                {
                    Array.Copy(globalStrings, 0, planes[0], 0, globalStrings.Length);
                }
            }

            public StringValueEntry[] Plane(int handle) { return planes[handle / MaxElementsPerPlane]; }
            public int Index(int handle) { return handle % MaxElementsPerPlane; }

            public void Initialize(StringValueEntry[] globalStrings)
            {
                

                freeListHead = -1;

                if (globalStrings != null && globalStrings.Length != 0)
                {
                    top = globalStrings.Length;
                }
                else
                {
                    top = 1;          
                }
            }

            public int Allocate(bool isStatic)
            {
                InternalDebug.Assert(planes != null);

                StringValueEntry[] plane;
                int index;

                var handle = freeListHead;

                if (handle != -1)
                {
                    

                    index = handle % MaxElementsPerPlane;
                    plane = planes[handle / MaxElementsPerPlane];

                    freeListHead = plane[index].NextFree;
                }
                else
                {
                    InternalDebug.Assert(top <= planes.Length * MaxElementsPerPlane);

                    handle = top++;

                    index = handle % MaxElementsPerPlane;
                    var planeIndex = handle / MaxElementsPerPlane;

                    if (index == 0)
                    {
                        

                        

                        if (planeIndex == MaxPlanes)
                        {
                            
                            throw new TextConvertersException(Strings.InputDocumentTooComplex);
                        }

                        if (planeIndex == planes.Length)
                        {
                            

                            var newLength = Math.Min(planes.Length * 2, MaxPlanes);
                            var newPlanes = new StringValueEntry[newLength][];
                            Array.Copy(planes, 0, newPlanes, 0, planes.Length);
                            planes = newPlanes;
                        }

                        if (planes[planeIndex] == null)
                        {
                            

                            planes[planeIndex] = new StringValueEntry[MaxElementsPerPlane];
                        }
                    }
                    else if (planeIndex == 0 && index == planes[planeIndex].Length)
                    {
                        

                        var newLength = Math.Min(planes[0].Length * 2, MaxElementsPerPlane);
                        var newPlane = new StringValueEntry[newLength];
                        Array.Copy(planes[0], 0, newPlane, 0, planes[0].Length);
                        planes[0] = newPlane;
                    }

                    plane = planes[planeIndex];
                }

                
                plane[index].str = null;
                plane[index].refCount = isStatic ? Int32.MaxValue : 1;
                plane[index].nextFree = -1;

                return handle;
            }

            public void Free(int handle)
            {
                InternalDebug.Assert(planes != null &&
                    handle / MaxElementsPerPlane < planes.Length &&
                    planes[handle / MaxElementsPerPlane] != null &&
                    handle %  MaxElementsPerPlane < planes[handle / MaxElementsPerPlane].Length);

                var index = handle % MaxElementsPerPlane;
                var plane = planes[handle / MaxElementsPerPlane];

                plane[index].NextFree = freeListHead;
                freeListHead = handle;
            }

            public long DumpStat(TextWriter dumpWriter)
            {
#if !PRIVATEBUILD
                long countAllocated;
                long countUsed;
                long countNonNullStrings;
                long totalStringsLength;
                long averageStringLength;
                long maxStringLength;
                long bytesTotal;
#endif
                var numPlanes = (top < MaxElementsPerPlane) ? 1 : ((top % MaxElementsPerPlane == 0) ? (top / MaxElementsPerPlane) : (top / MaxElementsPerPlane + 1));
                countAllocated = (numPlanes == 1) ? planes[0].Length : (numPlanes * MaxElementsPerPlane);
                bytesTotal = 12 + planes.Length * 4 + 12 * numPlanes  + countAllocated * Marshal.SizeOf(typeof(StringValueEntry));
                countUsed = 0;
                countNonNullStrings = 0;
                totalStringsLength = 0;
                maxStringLength = 0;

                StringValueEntry[] plane;
                int index;

                for (var i = 0; i < top; i++)
                {
                    InternalDebug.Assert(planes != null &&
                        i / MaxElementsPerPlane < planes.Length &&
                        planes[i / MaxElementsPerPlane] != null &&
                        i % MaxElementsPerPlane < planes[i / MaxElementsPerPlane].Length);

                    index = i % MaxElementsPerPlane;
                    plane = planes[i / MaxElementsPerPlane];

                    if (plane[index].refCount != 0)
                    {
                        countUsed ++;

                        if (plane[index].str != null)
                        {
                            bytesTotal += 12 + plane[index].str.Length * 2;    

                            countNonNullStrings ++;
                            totalStringsLength += plane[index].str.Length;
                            if (plane[index].str.Length > maxStringLength)
                            {
                                maxStringLength = plane[index].str.Length;
                            }
                        }
                    }
                }

                averageStringLength = countNonNullStrings == 0 ? 0 : (totalStringsLength + countNonNullStrings - 1) / countNonNullStrings;

                #if DEBUG
                var countFree = 0;
                var next = freeListHead;

                while (next != -1)
                {
                    index = next % MaxElementsPerPlane;
                    plane = planes[next / MaxElementsPerPlane];

                    countFree ++;
                    next = plane[index].NextFree;
                }

                InternalDebug.Assert(countUsed == top - 1 - countFree);
                #endif

                if (dumpWriter != null)
                {
                    dumpWriter.WriteLine("StringValues alloc: {0}", countAllocated);
                    dumpWriter.WriteLine("StringValues used: {0}", countUsed);
                    dumpWriter.WriteLine("StringValues non-null strings: {0}", countNonNullStrings);
                    if (countNonNullStrings != 0)
                    {
                        dumpWriter.WriteLine("StringValues total string length: {0}", totalStringsLength);
                        dumpWriter.WriteLine("StringValues average string length: {0}", averageStringLength);
                        dumpWriter.WriteLine("StringValues max string length: {0}", maxStringLength);
                    }
                    dumpWriter.WriteLine("StringValues bytes: {0}", bytesTotal);
                }

                return bytesTotal;
            }
        }

        

        internal class MultiValueStore
        {
            
            
            
            internal const int MaxElementsPerPlane = 4096;

            
            
            
            internal const int MaxPlanes = (1024 * 1024) / MaxElementsPerPlane;

            
            internal const int InitialPlanes = 16;

            
            internal const int InitialElements = 16;

            private FormatStore store;
            private MultiValueEntry[][] planes;
            private int freeListHead;
            private int top;

#if PRIVATEBUILD
            internal long countAllocated;
            internal long countUsed;
            internal long countNonNullValueLists;
            internal long totalValueListsLength;
            internal long averageValueListLength;
            internal long maxValueListLength;
            internal long bytesTotal;
#endif
            public MultiValueStore(FormatStore store, MultiValueEntry[] globaMultiValues)
            {
                InternalDebug.Assert(InitialElements < MaxElementsPerPlane);
                InternalDebug.Assert(Marshal.SizeOf(typeof(MultiValueEntry)) * MaxElementsPerPlane < 85000, "exceeding LOH threshold!");
                InternalDebug.Assert(globaMultiValues.Length < MaxElementsPerPlane);

                this.store = store;
                planes = new MultiValueEntry[InitialPlanes][];       
                planes[0] = new MultiValueEntry[Math.Max(InitialElements, globaMultiValues.Length + 1)];
                freeListHead = 0;
                top = 0;

                if (globaMultiValues != null && globaMultiValues.Length != 0)
                {
                    Array.Copy(globaMultiValues, 0, planes[0], 0, globaMultiValues.Length);
                }
            }

            public FormatStore Store => store;

            public MultiValueEntry[] Plane(int handle) { return planes[handle / MaxElementsPerPlane]; }
            public int Index(int handle) { return handle % MaxElementsPerPlane; }

            public void Initialize(MultiValueEntry[] globaMultiValues)
            {
                

                freeListHead = -1;

                if (globaMultiValues != null && globaMultiValues.Length != 0)
                {
                    top = globaMultiValues.Length;
                }
                else
                {
                    top = 1;          
                }
            }

            public int Allocate(bool isStatic)
            {
                InternalDebug.Assert(planes != null);

                MultiValueEntry[] plane;
                int index;

                var handle = freeListHead;

                if (handle != -1)
                {
                    

                    index = handle % MaxElementsPerPlane;
                    plane = planes[handle / MaxElementsPerPlane];

                    freeListHead = plane[index].NextFree;
                }
                else
                {
                    InternalDebug.Assert(top <= planes.Length * MaxElementsPerPlane);

                    handle = top++;

                    index = handle % MaxElementsPerPlane;
                    var planeIndex = handle / MaxElementsPerPlane;

                    if (index == 0)
                    {
                        

                        

                        if (planeIndex == MaxPlanes)
                        {
                            
                            throw new TextConvertersException(Strings.InputDocumentTooComplex);
                        }

                        if (planeIndex == planes.Length)
                        {
                            

                            var newLength = Math.Min(planes.Length * 2, MaxPlanes);
                            var newPlanes = new MultiValueEntry[newLength][];
                            Array.Copy(planes, 0, newPlanes, 0, planes.Length);
                            planes = newPlanes;
                        }

                        if (planes[planeIndex] == null)
                        {
                            

                            planes[planeIndex] = new MultiValueEntry[MaxElementsPerPlane];
                        }
                    }
                    else if (planeIndex == 0 && index == planes[planeIndex].Length)
                    {
                        

                        var newLength = Math.Min(planes[0].Length * 2, MaxElementsPerPlane);
                        var newPlane = new MultiValueEntry[newLength];
                        Array.Copy(planes[0], 0, newPlane, 0, planes[0].Length);
                        planes[0] = newPlane;
                    }

                    plane = planes[planeIndex];
                }

                
                plane[index].values = null;
                plane[index].refCount = isStatic ? Int32.MaxValue : 1;
                plane[index].nextFree = -1;

                return handle;
            }

            public void Free(int handle)
            {
                InternalDebug.Assert(planes != null &&
                    handle / MaxElementsPerPlane < planes.Length &&
                    planes[handle / MaxElementsPerPlane] != null &&
                    handle %  MaxElementsPerPlane < planes[handle / MaxElementsPerPlane].Length);

                var index = handle % MaxElementsPerPlane;
                var plane = planes[handle / MaxElementsPerPlane];

                if (null != plane[index].values)
                {
                    for (var i = 0; i < plane[index].values.Length; i++)
                    {
                        if (plane[index].values[i].IsRefCountedHandle)
                        {
                            store.ReleaseValue(plane[index].values[i]);
                        }
                    }
                }

                plane[index].NextFree = freeListHead;
                freeListHead = handle;
            }

            public long DumpStat(TextWriter dumpWriter)
            {
#if !PRIVATEBUILD
                long countAllocated;
                long countUsed;
                long countNonNullValueLists;
                long totalValueListsLength;
                long averageValueListLength;
                long maxValueListLength;
                long bytesTotal;
#endif
                var numPlanes = (top < MaxElementsPerPlane) ? 1 : ((top % MaxElementsPerPlane == 0) ? (top / MaxElementsPerPlane) : (top / MaxElementsPerPlane + 1));
                countAllocated = (numPlanes == 1) ? planes[0].Length : (numPlanes * MaxElementsPerPlane);
                bytesTotal = 12 + planes.Length * 4 + 12 * numPlanes  + countAllocated * Marshal.SizeOf(typeof(MultiValueEntry));
                countUsed = 0;
                countNonNullValueLists = 0;
                totalValueListsLength = 0;
                maxValueListLength = 0;

                MultiValueEntry[] plane;
                int index;

                for (var i = 0; i < top; i++)
                {
                    InternalDebug.Assert(planes != null &&
                        i / MaxElementsPerPlane < planes.Length &&
                        planes[i / MaxElementsPerPlane] != null &&
                        i % MaxElementsPerPlane < planes[i / MaxElementsPerPlane].Length);

                    index = i % MaxElementsPerPlane;
                    plane = planes[i / MaxElementsPerPlane];

                    if (plane[index].refCount != 0)
                    {
                        countUsed ++;

                        if (plane[index].values != null)
                        {
                            bytesTotal += 12 + plane[index].values.Length * Marshal.SizeOf(typeof(PropertyValue));

                            countNonNullValueLists ++;
                            totalValueListsLength += plane[index].values.Length;
                            if (plane[index].values.Length > maxValueListLength)
                            {
                                maxValueListLength = plane[index].values.Length;
                            }
                        }
                    }
                }

                averageValueListLength = countNonNullValueLists == 0 ? 0 : (totalValueListsLength + countNonNullValueLists - 1) / countNonNullValueLists;

                #if DEBUG
                var countFree = 0;
                var next = freeListHead;

                while (next != -1)
                {
                    index = next % MaxElementsPerPlane;
                    plane = planes[next / MaxElementsPerPlane];

                    countFree ++;
                    next = plane[index].NextFree;
                }

                InternalDebug.Assert(countUsed == top - 1 - countFree);
                #endif

                if (dumpWriter != null)
                {
                    dumpWriter.WriteLine("MultiValues alloc: {0}", countAllocated);
                    dumpWriter.WriteLine("MultiValues used: {0}", countUsed);
                    dumpWriter.WriteLine("MultiValues non-null value lists: {0}", countNonNullValueLists);
                    if (countNonNullValueLists != 0)
                    {
                        dumpWriter.WriteLine("MultiValues total value lists length: {0}", totalValueListsLength);
                        dumpWriter.WriteLine("MultiValues average value list length: {0}", averageValueListLength);
                        dumpWriter.WriteLine("MultiValues max value list length: {0}", maxValueListLength);
                    }
                    dumpWriter.WriteLine("MultiValues bytes: {0}", bytesTotal);
                }

                return bytesTotal;
            }
        }

        

        internal class TextStore
        {
            
            
            internal const int LogMaxCharactersPerPlane = 15;

            
            
            

            internal const int MaxCharactersPerPlane = 1 << LogMaxCharactersPerPlane;

            
            
            internal const int MaxPlanes = (20 * 1024 * 1024) / MaxCharactersPerPlane;

            
            internal const int InitialPlanes = 16;

            
            internal const int InitialCharacters = 2048;

            internal const int MaxRunEffectivelength = 0x1FFF;

            private OutboundCodePageDetector detector;

            private char[][] planes;
            private uint position;

            private TextRunType lastRunType;
            private uint lastRunPosition;

#if PRIVATEBUILD
            internal long charactersAllocated;
            internal long charactersUsed;
            internal long bytesTotal;
#endif
            public TextStore()
            {
                InternalDebug.Assert(InitialCharacters <= MaxCharactersPerPlane);
                InternalDebug.Assert(Marshal.SizeOf(typeof(char)) * MaxCharactersPerPlane < 85000, "exceeding LOH threshold!");

                planes = new char[InitialPlanes][];       
                planes[0] = new char[InitialCharacters];
            }

            public char[] Plane(uint position) { return planes[position >> LogMaxCharactersPerPlane]; }
            public int Index(uint position) { return (int)(position & (MaxCharactersPerPlane - 1)); }
            public char Pick(uint position) { return planes[position >> LogMaxCharactersPerPlane][position & (MaxCharactersPerPlane - 1)]; }

            public uint CurrentPosition => position;

            public TextRunType LastRunType => lastRunType;

            public void Initialize()
            {
                position = 0;
                lastRunType = TextRunType.Invalid;
                lastRunPosition = 0;

                if (detector != null)
                {
                    detector.Reset();
                }
            }

            public void InitializeCodepageDetector()
            {
                if (detector == null)
                {
                    detector = new OutboundCodePageDetector();
                }
            }

            public int GetBestWindowsCodePage()
            {
                InternalDebug.Assert(detector != null);
                return detector.GetBestWindowsCodePage();
            }

            public int GetBestWindowsCodePage(int preferredCodePage)
            {
                InternalDebug.Assert(detector != null);
                return detector.GetBestWindowsCodePage(preferredCodePage);
            }

            

            public void AddText(TextRunType runType, char[] textBuffer, int offset, int count)
            {
                InternalDebug.Assert(count > 0);
                InternalDebug.Assert(runType == TextRunType.NonSpace);

                if (detector != null)
                {
                    detector.AddText(textBuffer, offset, count);
                }

                var index = (int)(position & (MaxCharactersPerPlane - 1));
                var planeIndex = (int)(position >> LogMaxCharactersPerPlane);

                if (lastRunType == runType && index != 0)
                {
                    
                    

                    InternalDebug.Assert((lastRunPosition >> LogMaxCharactersPerPlane) == planeIndex);

                    var lastRunPlane = planes[planeIndex];
                    var lastRunIndex = (int)(lastRunPosition & (MaxCharactersPerPlane - 1));

                    InternalDebug.Assert(TypeFromRunHeader(lastRunPlane[lastRunIndex]) == runType);
                    InternalDebug.Assert(lastRunIndex + 1 + LengthFromRunHeader(lastRunPlane[lastRunIndex]) == index);

                    var appendLength = Math.Min(Math.Min(count, MaxRunEffectivelength - LengthFromRunHeader(lastRunPlane[lastRunIndex])), MaxCharactersPerPlane - index);

                    InternalDebug.Assert(appendLength >= 0);

                    if (appendLength != 0)
                    {
                        if (planeIndex == 0 && index + appendLength > lastRunPlane.Length)
                        {
                            

                            var newLength = Math.Min(Math.Max(planes[0].Length * 2, index + appendLength), MaxCharactersPerPlane);
                            var newPlane = new char[newLength];
                            Buffer.BlockCopy(planes[0], 0, newPlane, 0, (int)position * 2);
                            planes[0] = lastRunPlane = newPlane;
                        }

                        InternalDebug.Assert(index + appendLength <= lastRunPlane.Length);

                        lastRunPlane[lastRunIndex] = MakeTextRunHeader(runType, (int)(appendLength + LengthFromRunHeader(lastRunPlane[lastRunIndex])));
                        Buffer.BlockCopy(textBuffer, offset * 2, lastRunPlane, (int)index * 2, (int)appendLength * 2);

                        offset += appendLength;
                        count -= appendLength;

                        position += (uint)(appendLength);
                    }
                }

                InternalDebug.Assert(index <= MaxCharactersPerPlane);

                while (count != 0)
                {
                    index = (int)(position & (MaxCharactersPerPlane - 1));
                    planeIndex = (int)(position >> LogMaxCharactersPerPlane);

                    if (MaxCharactersPerPlane - index < 20 + 1)
                    {
                        

                        InternalDebug.Assert(planes[planeIndex].Length == MaxCharactersPerPlane);

                        
                        
                        

                        

                        planes[planeIndex][index] = MakeTextRunHeader(TextRunType.Invalid, (int)(MaxCharactersPerPlane - index - 1));
                        position += (uint)(MaxCharactersPerPlane - index);
                    }
                    else
                    {
                        var newRunLength = Math.Min(Math.Min(count, MaxRunEffectivelength), MaxCharactersPerPlane - index - 1);

                        if (planeIndex == 0 && index + newRunLength + 1 > planes[0].Length)
                        {
                            

                            InternalDebug.Assert(planes[planeIndex].Length < MaxCharactersPerPlane);

                            var newLength = Math.Min(Math.Max(planes[0].Length * 2, index + newRunLength + 1), MaxCharactersPerPlane);
                            var newPlane = new char[newLength];
                            Buffer.BlockCopy(planes[0], 0, newPlane, 0, (int)(position * 2));
                            planes[0] = newPlane;

                            InternalDebug.Assert(position + 1 + newRunLength <= planes[0].Length);
                        }
                        else if (index == 0)
                        {
                            if (planeIndex == planes.Length)
                            {
                                if (planeIndex == MaxPlanes)
                                {
                                    
                                    throw new TextConvertersException(Strings.InputDocumentTooComplex);
                                }

                                

                                var newLength = Math.Min(planes.Length * 2, MaxPlanes);
                                var newPlanes = new char[newLength][];
                                Array.Copy(planes, 0, newPlanes, 0, planes.Length);
                                planes = newPlanes;
                            }

                            if (planes[planeIndex] == null)
                            {
                                
                                planes[planeIndex] = new char[MaxCharactersPerPlane];
                            }
                        }

                        lastRunType = runType;
                        lastRunPosition = position;

                        planes[planeIndex][index] = MakeTextRunHeader(runType, newRunLength);
                        Buffer.BlockCopy(textBuffer, offset * 2, planes[planeIndex], (index + 1) * 2, newRunLength * 2);

                        offset += newRunLength;
                        count -= newRunLength;

                        position += (uint)(newRunLength + 1);
                    }
                }
            }

            

            public void AddSimpleRun(TextRunType runType, int count)
            {
                InternalDebug.Assert(count <= MaxRunEffectivelength);

                if (lastRunType == runType)
                {
                    
                    

                    InternalDebug.Assert(lastRunPosition + 1 == position);

                    var lastRunPlane = planes[lastRunPosition >> LogMaxCharactersPerPlane];
                    var lastRunIndex = (int)(lastRunPosition & (MaxCharactersPerPlane - 1));

                    InternalDebug.Assert(TypeFromRunHeader(lastRunPlane[lastRunIndex]) == runType);
                    InternalDebug.Assert(((lastRunIndex + 1 - position) & (MaxCharactersPerPlane - 1)) == 0);

                    var appendLength = Math.Min(count, MaxRunEffectivelength - LengthFromRunHeader(lastRunPlane[lastRunIndex]));

                    InternalDebug.Assert(appendLength >= 0);

                    if (appendLength != 0)
                    {
                        lastRunPlane[lastRunIndex] = MakeTextRunHeader(runType, appendLength + LengthFromRunHeader(lastRunPlane[lastRunIndex]));

                        count -= appendLength;
                    }
                }

                if (count != 0)
                {
                    

                    var index = (int)(position & (MaxCharactersPerPlane - 1));
                    var planeIndex = (int)(position >> LogMaxCharactersPerPlane);

                    if (index == 0)
                    {
                        

                        if (planeIndex == planes.Length)
                        {
                            if (planeIndex == MaxPlanes)
                            {
                                
                                throw new TextConvertersException(Strings.InputDocumentTooComplex);
                            }

                            

                            var newLength = Math.Min(planes.Length * 2, MaxPlanes);
                            var newPlanes = new char[newLength][];
                            Array.Copy(planes, 0, newPlanes, 0, planes.Length);
                            planes = newPlanes;
                        }

                        if (planes[planeIndex] == null)
                        {
                            
                            planes[planeIndex] = new char[MaxCharactersPerPlane];
                        }
                    }
                    else
                    {
                        if (planeIndex == 0 && position + 1 > planes[0].Length)
                        {
                            

                            var newLength = Math.Min(planes[0].Length * 2, MaxCharactersPerPlane);
                            var newPlane = new char[newLength];
                            Buffer.BlockCopy(planes[0], 0, newPlane, 0, (int)(position * 2));
                            planes[0] = newPlane;

                            InternalDebug.Assert(position + 1 <= planes[0].Length);
                        }
                    }

                    lastRunType = runType;
                    lastRunPosition = position;

                    planes[planeIndex][index] = MakeTextRunHeader(runType, count);

                    position ++;
                }
            }

            public void ConvertToInvalid(uint startPosition)
            {
                InternalDebug.Assert(startPosition < position);

                var buffer = Plane(startPosition);
                var index = Index(startPosition);

                var runLength = buffer[index] >= (char)TextRunType.FirstShort ? 1 : LengthFromRunHeader(buffer[index]) + 1;

                buffer[index] = MakeTextRunHeader(TextRunType.Invalid, runLength - 1);
            }

            public void ConvertToInvalid(uint startPosition, int countToConvert)
            {
                InternalDebug.Assert(startPosition < position && countToConvert > 0);

                var buffer = Plane(startPosition);
                var index = Index(startPosition);

                InternalDebug.Assert(TypeFromRunHeader(buffer[index]) == TextRunType.NonSpace);

                var runEffectiveLength = LengthFromRunHeader(buffer[index]);

                InternalDebug.Assert(countToConvert < runEffectiveLength);

                var remainingLength = runEffectiveLength - countToConvert;
                var headLength = runEffectiveLength + 1 - (remainingLength + 1);

                InternalDebug.Assert(headLength > 0);

                buffer[index] = MakeTextRunHeader(TextRunType.Invalid, headLength - 1);
                buffer[index + headLength] = MakeTextRunHeader(TextRunType.NonSpace, remainingLength);
            }

            public void ConvertShortRun(uint startPosition, TextRunType type, int newEffectiveLength)
            {
                InternalDebug.Assert(startPosition < position && newEffectiveLength > 0);

                var buffer = Plane(startPosition);
                var index = Index(startPosition);

                InternalDebug.Assert(TypeFromRunHeader(buffer[index]) >= TextRunType.FirstShort);

                buffer[index] = MakeTextRunHeader(type, newEffectiveLength);
            }

            

            public void DoNotMergeNextRun()
            {
                
                

                if (lastRunType != TextRunType.BlockBoundary)
                {
                    
                    
                    lastRunType = TextRunType.Invalid;
                }
            }

            

            internal char MakeTextRunHeader(TextRunType runType, int length)
            {
                InternalDebug.Assert(length <= MaxRunEffectivelength);
                return (char)((int)runType | length);
            }

            internal static TextRunType TypeFromRunHeader(char runHeader)
            {
                return (TextRunType)(runHeader & 0xE000);
            }

            internal static int LengthFromRunHeader(char runHeader)
            {
                return (int)runHeader & MaxRunEffectivelength;
            }

            public long DumpStat(TextWriter dumpWriter)
            {
#if !PRIVATEBUILD
                long charactersAllocated;
                long charactersUsed;
                long bytesTotal;
#endif
                var numPlanes = (int)((position + MaxCharactersPerPlane - 1) >> LogMaxCharactersPerPlane);
                if (numPlanes == 0)
                {
                    numPlanes = 1;
                }

                charactersAllocated = (numPlanes == 1) ? planes[0].Length : (numPlanes * MaxCharactersPerPlane);
                charactersUsed = position;
                bytesTotal = 12 + planes.Length * 4 + numPlanes * 12 + charactersAllocated * 2;

                if (dumpWriter != null)
                {
                    dumpWriter.WriteLine("Text alloc: {0}", charactersAllocated);
                    dumpWriter.WriteLine("Text used: {0}", charactersUsed);
                    dumpWriter.WriteLine("Text bytes: {0}", bytesTotal);
                }

                return bytesTotal;
            }
          }
    }

    

    internal struct FormatNode
    {
        internal FormatStore.NodeStore nodes;
        private int nodeHandle;

        internal FormatNode(FormatStore.NodeStore nodes, int nodeHandle)
        {
            this.nodes = nodes;
            this.nodeHandle = nodeHandle;
        }

        internal FormatNode(FormatStore store, int nodeHandle)
        {
            nodes = store.nodes;
            this.nodeHandle = nodeHandle;
        }

        public static readonly FormatNode Null = new FormatNode();

        public int Handle => nodeHandle;

        public bool IsNull => nodeHandle == 0;

        public bool IsInOrder => 0 == (nodes.Plane(nodeHandle)[nodes.Index(nodeHandle)].nodeFlags & FormatStore.NodeFlags.OutOfOrder);

        public bool OnRightEdge => 0 != (nodes.Plane(nodeHandle)[nodes.Index(nodeHandle)].nodeFlags & FormatStore.NodeFlags.OnRightEdge);

        public bool OnLeftEdge => 0 != (nodes.Plane(nodeHandle)[nodes.Index(nodeHandle)].nodeFlags & FormatStore.NodeFlags.OnLeftEdge);

        public bool IsVisited => 0 != (nodes.Plane(nodeHandle)[nodes.Index(nodeHandle)].nodeFlags & FormatStore.NodeFlags.Visited);

        public bool IsEmptyBlockNode => 0 != (NodeType & FormatContainerType.BlockFlag) && BeginTextPosition + 1 == EndTextPosition;

        public bool CanFlush => 0 != (nodes.Plane(nodeHandle)[nodes.Index(nodeHandle)].nodeFlags & FormatStore.NodeFlags.CanFlush);

        public FormatContainerType NodeType
        {
            get { return nodes.Plane(nodeHandle)[nodes.Index(nodeHandle)].type; }
            set { nodes.Plane(nodeHandle)[nodes.Index(nodeHandle)].type = value; }
        }

        public bool IsText => nodes.Plane(nodeHandle)[nodes.Index(nodeHandle)].type == FormatContainerType.Text;

        public FormatNode Parent
        {
            get
            {
                var parentHandle = nodes.Plane(nodeHandle)[nodes.Index(nodeHandle)].parent;
                return parentHandle == 0 ?
                            Null :
                            new FormatNode(nodes, parentHandle);
            }
        }

        public bool IsOnlySibling => nodes.Plane(nodeHandle)[nodes.Index(nodeHandle)].nextSibling == nodeHandle;

        public FormatNode FirstChild
        {
            get
            {
                var lastChildHandle = nodes.Plane(nodeHandle)[nodes.Index(nodeHandle)].lastChild;
                return lastChildHandle == 0 ?
                            Null :
                            new FormatNode(nodes, nodes.Plane(lastChildHandle)[nodes.Index(lastChildHandle)].nextSibling);
            }
        }

        public FormatNode LastChild
        {
            get
            {
                var lastChildHandle = nodes.Plane(nodeHandle)[nodes.Index(nodeHandle)].lastChild;
                return lastChildHandle == 0 ?
                            Null :
                            new FormatNode(nodes, lastChildHandle);
            }
        }

        public FormatNode NextSibling
        {
            get
            {
                var parentHandle = nodes.Plane(nodeHandle)[nodes.Index(nodeHandle)].parent;
                return (parentHandle == 0 ||
                        nodeHandle == nodes.Plane(parentHandle)[nodes.Index(parentHandle)].lastChild) ?
                            Null :
                            new FormatNode(nodes, nodes.Plane(nodeHandle)[nodes.Index(nodeHandle)].nextSibling);
            }
        }

        public FormatNode PreviousSibling
        {
            get
            {
                var leftSibling = Parent.FirstChild;
                if (this == leftSibling)
                {
                    
                    return Null;
                }

                while (leftSibling.NextSibling != this)
                {
                    leftSibling = leftSibling.NextSibling;
                }

                return leftSibling;
            }
        }

        public uint BeginTextPosition
        {
            get { return nodes.Plane(nodeHandle)[nodes.Index(nodeHandle)].beginTextPosition; }
            set { nodes.Plane(nodeHandle)[nodes.Index(nodeHandle)].beginTextPosition = value; }
        }

        public uint EndTextPosition
        {
            get { return nodes.Plane(nodeHandle)[nodes.Index(nodeHandle)].endTextPosition; }
            set { nodes.Plane(nodeHandle)[nodes.Index(nodeHandle)].endTextPosition = value; }
        }

        public int InheritanceMaskIndex
        {
            get { return nodes.Plane(nodeHandle)[nodes.Index(nodeHandle)].inheritanceMaskIndex; }
            set { nodes.Plane(nodeHandle)[nodes.Index(nodeHandle)].inheritanceMaskIndex = value; }
        }

        public FlagProperties FlagProperties
        {
            get { return nodes.Plane(nodeHandle)[nodes.Index(nodeHandle)].flagProperties; }
            set { nodes.Plane(nodeHandle)[nodes.Index(nodeHandle)].flagProperties = value; }
        }

        public PropertyBitMask PropertyMask
        {
            get { return nodes.Plane(nodeHandle)[nodes.Index(nodeHandle)].propertyMask; }
            set { nodes.Plane(nodeHandle)[nodes.Index(nodeHandle)].propertyMask = value; }
        }

        public Property[] Properties
        {
            get { return nodes.Plane(nodeHandle)[nodes.Index(nodeHandle)].properties; }
            set { nodes.Plane(nodeHandle)[nodes.Index(nodeHandle)].properties = value; }
        }

        public NodePropertiesEnumerator PropertiesEnumerator => new NodePropertiesEnumerator(this);

        public bool IsBlockNode => 0 != (nodes.Plane(nodeHandle)[nodes.Index(nodeHandle)].type & FormatContainerType.BlockFlag);

        public void SetOutOfOrder()
        {
            nodes.Plane(nodeHandle)[nodes.Index(nodeHandle)].nodeFlags |= FormatStore.NodeFlags.OutOfOrder;
        }

        public void SetOnLeftEdge()
        {
            nodes.Plane(nodeHandle)[nodes.Index(nodeHandle)].nodeFlags |= FormatStore.NodeFlags.OnLeftEdge;
        }

        public void ResetOnLeftEdge()
        {
            nodes.Plane(nodeHandle)[nodes.Index(nodeHandle)].nodeFlags &= ~FormatStore.NodeFlags.OnLeftEdge;
        }

        public void SetOnRightEdge()
        {
            nodes.Plane(nodeHandle)[nodes.Index(nodeHandle)].nodeFlags |= FormatStore.NodeFlags.OnRightEdge;
        }

        public void SetVisited()
        {
            nodes.Plane(nodeHandle)[nodes.Index(nodeHandle)].nodeFlags |= FormatStore.NodeFlags.Visited;
        }

        public void ResetVisited()
        {
            nodes.Plane(nodeHandle)[nodes.Index(nodeHandle)].nodeFlags &= ~FormatStore.NodeFlags.Visited;
        }

        public PropertyValue GetProperty(PropertyId id)
        {
            var thisPlane = nodes.Plane(nodeHandle);
            var thisIndex = nodes.Index(nodeHandle);

            if (FlagProperties.IsFlagProperty(id))
            {
                return thisPlane[thisIndex].flagProperties.GetPropertyValue(id);
            }
            else if (thisPlane[thisIndex].propertyMask.IsSet(id))
            {
                InternalDebug.Assert(thisPlane[thisIndex].properties != null);

                for (var i = 0; i < thisPlane[thisIndex].properties.Length; i++)
                {
                    var prop = thisPlane[thisIndex].properties[i];
                    if (prop.Id == id)
                    {
                        return prop.Value;
                    }
                    else if (prop.Id > id)
                    {
                        break;
                    }
                }

                InternalDebug.Assert(false);
            }

            return PropertyValue.Null;
        }

        public void SetProperty(PropertyId id, PropertyValue value)
        {
            var thisPlane = nodes.Plane(nodeHandle);
            var thisIndex = nodes.Index(nodeHandle);

            if (FlagProperties.IsFlagProperty(id))
            {
                thisPlane[thisIndex].flagProperties.SetPropertyValue(id, value);
                return;
            }

            var i = 0;

            if (thisPlane[thisIndex].properties != null)
            {
                
                for (; i < thisPlane[thisIndex].properties.Length; i++)
                {
                    var prop = thisPlane[thisIndex].properties[i];
                    if (prop.Id == id)
                    {
                        InternalDebug.Assert(thisPlane[thisIndex].propertyMask.IsSet(id));

                        
                        thisPlane[thisIndex].properties[i].Set(id, value);
                        return;
                    }
                    else if (prop.Id > id)
                    {
                        break;
                    }
                }
            }

            if (thisPlane[thisIndex].properties == null)
            {
                InternalDebug.Assert(i == 0);

                thisPlane[thisIndex].properties = new Property[1];

                thisPlane[thisIndex].properties[0].Set(id, value);

                thisPlane[thisIndex].propertyMask.Set(id);
            }
            else
            {
                
                

                var newProperties = new Property[thisPlane[thisIndex].properties.Length + 1];

                if (i != 0)
                {
                    Array.Copy(thisPlane[thisIndex].properties, 0, newProperties, 0, i);
                }

                if (i != thisPlane[thisIndex].properties.Length)
                {
                    Array.Copy(thisPlane[thisIndex].properties, i, newProperties, i + 1, thisPlane[thisIndex].properties.Length - i);
                }

                newProperties[i].Set(id, value);

                thisPlane[thisIndex].properties = newProperties;

                thisPlane[thisIndex].propertyMask.Set(id);
            }
        }

        internal static void InternalAppendChild(FormatStore.NodeStore nodes, int thisNode, int newChildNode)
        {
            
            InternalPrependChild(nodes, thisNode, newChildNode);
            nodes.Plane(thisNode)[nodes.Index(thisNode)].lastChild = newChildNode;
        }

        internal static void InternalPrependChild(FormatStore.NodeStore nodes, int thisNode, int newChildNode)
        {
            var thisPlane = nodes.Plane(thisNode);
            var thisIndex = nodes.Index(thisNode);

            var childPlane = nodes.Plane(newChildNode);
            var childIndex = nodes.Index(newChildNode);

            if (thisPlane[thisIndex].lastChild != 0)  
            {
                var oldLastChild = thisPlane[thisIndex].lastChild;

                var oldLastChildPlane = nodes.Plane(oldLastChild);
                var oldLastChildIndex = nodes.Index(oldLastChild);

                childPlane[childIndex].nextSibling = oldLastChildPlane[oldLastChildIndex].nextSibling;
                oldLastChildPlane[oldLastChildIndex].nextSibling = newChildNode;
                childPlane[childIndex].parent = thisNode;
            }
            else
            {
                
                childPlane[childIndex].nextSibling = newChildNode;
                childPlane[childIndex].parent = thisNode;
                thisPlane[thisIndex].lastChild = newChildNode;
            }
        }


        public void AppendChild(FormatNode newChildNode)
        {
            InternalAppendChild(nodes, nodeHandle, newChildNode.Handle);
        }

        public void PrependChild(FormatNode newChildNode)
        {
            InternalPrependChild(nodes, nodeHandle, newChildNode.Handle);
        }

        public void InsertSiblingAfter(FormatNode newSiblingNode)
        {
            var newSibling = newSiblingNode.nodeHandle;

            var newSiblingPlane = nodes.Plane(newSibling);
            var newSiblingIndex = nodes.Index(newSibling);

            var thisPlane = nodes.Plane(nodeHandle);
            var thisIndex = nodes.Index(nodeHandle);

            var parent = thisPlane[thisIndex].parent;

            var parentPlane = nodes.Plane(parent);
            var parentIndex = nodes.Index(parent);

            newSiblingPlane[newSiblingIndex].parent = parent;
            newSiblingPlane[newSiblingIndex].nextSibling = thisPlane[thisIndex].nextSibling;
            thisPlane[thisIndex].nextSibling = newSibling;

            if (nodeHandle == parentPlane[parentIndex].lastChild)
            {
                parentPlane[parentIndex].lastChild = newSibling;
            }
        }

        public void InsertSiblingBefore(FormatNode newSiblingNode)
        {
            var newSibling = newSiblingNode.nodeHandle;

            var newSiblingPlane = nodes.Plane(newSibling);
            var newSiblingIndex = nodes.Index(newSibling);

            var thisPlane = nodes.Plane(nodeHandle);
            var thisIndex = nodes.Index(nodeHandle);

            var parent = thisPlane[thisIndex].parent;

            var parentPlane = nodes.Plane(parent);
            var parentIndex = nodes.Index(parent);

            var previousHandle = parentPlane[parentIndex].lastChild;

            var previousPlane = nodes.Plane(previousHandle);
            var previousIndex = nodes.Index(previousHandle);

            while (previousPlane[previousIndex].nextSibling != nodeHandle)
            {
                previousHandle = previousPlane[previousIndex].nextSibling;
                previousPlane = nodes.Plane(previousHandle);
                previousIndex = nodes.Index(previousHandle);
            }

            newSiblingPlane[newSiblingIndex].parent = parent;
            newSiblingPlane[newSiblingIndex].nextSibling = nodeHandle;

            previousPlane[previousIndex].nextSibling = newSibling;
        }

        public void RemoveFromParent()
        {
            

            

            InternalDebug.Assert(nodeHandle != 0);

            var thisPlane = nodes.Plane(nodeHandle);
            var thisIndex = nodes.Index(nodeHandle);

            InternalDebug.Assert(thisPlane[thisIndex].parent != 0);

            var parent = thisPlane[thisIndex].parent;

            var parentPlane = nodes.Plane(parent);
            var parentIndex = nodes.Index(parent);

            var nextSibling = thisPlane[thisIndex].nextSibling;

            if (nodeHandle == nextSibling)
            {
                
                InternalDebug.Assert(parentPlane[parentIndex].lastChild == nodeHandle);

                parentPlane[parentIndex].lastChild = 0;
            }
            else
            {
                var previousSibling = parentPlane[parentIndex].lastChild;

                var previousSiblingPlane = nodes.Plane(previousSibling);
                var previousSiblingIndex = nodes.Index(previousSibling);

                while (previousSiblingPlane[previousSiblingIndex].nextSibling != nodeHandle)
                {
                    previousSibling = previousSiblingPlane[previousSiblingIndex].nextSibling;

                    previousSiblingPlane = nodes.Plane(previousSibling);
                    previousSiblingIndex = nodes.Index(previousSibling);
                }

                previousSiblingPlane[previousSiblingIndex].nextSibling = thisPlane[thisIndex].nextSibling;

                if (parentPlane[parentIndex].lastChild == nodeHandle)
                {
                    
                    parentPlane[parentIndex].lastChild = previousSibling;
                }
            }

            thisPlane[thisIndex].parent = 0;
        }

        public void MoveAllChildrenToNewParent(FormatNode newParent)
        {
            while (!FirstChild.IsNull)
            {
                var child = FirstChild;
                child.RemoveFromParent();
                newParent.AppendChild(child);
            }
        }

        public void ChangeNodeType(FormatContainerType newType)
        {
            nodes.Plane(nodeHandle)[nodes.Index(nodeHandle)].type = newType;
        }

        public void PrepareToClose(uint endTextPosition)
        {
            var thisPlane = nodes.Plane(nodeHandle);
            var thisIndex = nodes.Index(nodeHandle);

            thisPlane[thisIndex].endTextPosition = endTextPosition;
            thisPlane[thisIndex].nodeFlags &= ~FormatStore.NodeFlags.OnRightEdge;
            thisPlane[thisIndex].nodeFlags |= FormatStore.NodeFlags.CanFlush;
        }

        public void SetProps(FlagProperties flagProperties, PropertyBitMask propertyMask, Property[] properties, int inheritanceMaskIndex)
        {
            var thisPlane = nodes.Plane(nodeHandle);
            var thisIndex = nodes.Index(nodeHandle);

            thisPlane[thisIndex].flagProperties = flagProperties;
            thisPlane[thisIndex].propertyMask = propertyMask;
            thisPlane[thisIndex].properties = properties;
            thisPlane[thisIndex].inheritanceMaskIndex = inheritanceMaskIndex;
        }

        public FormatNode SplitTextNode(uint splitPosition)
        {
            InternalDebug.Assert(splitPosition >= BeginTextPosition && splitPosition <= EndTextPosition);

            InternalDebug.Assert(NodeType == FormatContainerType.Text &&
                                FirstChild == Null &&
                                Parent != Null);

            

            var thisPlane = nodes.Plane(nodeHandle);
            var thisIndex = nodes.Index(nodeHandle);

            var newNodeHandle = nodes.Allocate(FormatContainerType.Text, thisPlane[thisIndex].beginTextPosition);

            var newPlane = nodes.Plane(newNodeHandle);
            var newIndex = nodes.Index(newNodeHandle);

            
            newPlane[newIndex].nodeFlags = thisPlane[thisIndex].nodeFlags;
            newPlane[newIndex].textMapping = thisPlane[thisIndex].textMapping;
            InternalDebug.Assert(newPlane[newIndex].beginTextPosition == thisPlane[thisIndex].beginTextPosition);
            newPlane[newIndex].endTextPosition = splitPosition;
            newPlane[newIndex].flagProperties = thisPlane[thisIndex].flagProperties;
            newPlane[newIndex].propertyMask = thisPlane[thisIndex].propertyMask;
            
            
            
            newPlane[newIndex].properties = thisPlane[thisIndex].properties;

            
            thisPlane[thisIndex].beginTextPosition = splitPosition;

            var newNode = new FormatNode(nodes, newNodeHandle);

            
            InsertSiblingBefore(newNode);

            
            return newNode;
        }

        public FormatNode SplitNodeBeforeChild(FormatNode child)
        {
            

            InternalDebug.Assert(child.Parent == this &&
                                Parent != Null);

            var thisPlane = nodes.Plane(nodeHandle);
            var thisIndex = nodes.Index(nodeHandle);

            var newNodeHandle = nodes.Allocate(NodeType, thisPlane[thisIndex].beginTextPosition);

            var newPlane = nodes.Plane(newNodeHandle);
            var newIndex = nodes.Index(newNodeHandle);

            
            newPlane[newIndex].nodeFlags = thisPlane[thisIndex].nodeFlags;
            newPlane[newIndex].textMapping = thisPlane[thisIndex].textMapping;
            InternalDebug.Assert(newPlane[newIndex].beginTextPosition == thisPlane[thisIndex].beginTextPosition);
            newPlane[newIndex].endTextPosition = child.BeginTextPosition;
            newPlane[newIndex].flagProperties = thisPlane[thisIndex].flagProperties;
            newPlane[newIndex].propertyMask = thisPlane[thisIndex].propertyMask;
            
            
            newPlane[newIndex].properties = thisPlane[thisIndex].properties;

            
            thisPlane[thisIndex].beginTextPosition = child.BeginTextPosition;

            var newNode = new FormatNode(nodes, newNodeHandle);

            

            FormatNode next;

            do
            {
                next = FirstChild;
                next.RemoveFromParent();
                newNode.AppendChild(next);
            }
            while (FirstChild != child);

            
            InsertSiblingBefore(newNode);

            return newNode;
        }

        public FormatNode DuplicateInsertAsChild()
        {
            

            var newNodeHandle = nodes.Allocate(NodeType, BeginTextPosition);

            var thisPlane = nodes.Plane(nodeHandle);
            var thisIndex = nodes.Index(nodeHandle);

            var newPlane = nodes.Plane(newNodeHandle);
            var newIndex = nodes.Index(newNodeHandle);

            
            newPlane[newIndex].nodeFlags = thisPlane[thisIndex].nodeFlags;
            newPlane[newIndex].textMapping = thisPlane[thisIndex].textMapping;
            InternalDebug.Assert(newPlane[newIndex].beginTextPosition == thisPlane[thisIndex].beginTextPosition);
            newPlane[newIndex].endTextPosition = thisPlane[thisIndex].endTextPosition;
            newPlane[newIndex].flagProperties = thisPlane[thisIndex].flagProperties;
            newPlane[newIndex].propertyMask = thisPlane[thisIndex].propertyMask;
            
            
            newPlane[newIndex].properties = thisPlane[thisIndex].properties;

            var newNode = new FormatNode(nodes, newNodeHandle);

            
            MoveAllChildrenToNewParent(newNode);

            
            AppendChild(newNode);

            
            return newNode;
        }

        
        public static bool operator ==(FormatNode x, FormatNode y) 
        {
            return x.nodes == y.nodes && x.nodeHandle == y.nodeHandle;
        }

        
        public static bool operator !=(FormatNode x, FormatNode y) 
        {
            return x.nodes != y.nodes || x.nodeHandle != y.nodeHandle;
        }

        public override bool Equals(object obj)
        {
            return (obj is FormatNode) && nodes == ((FormatNode)obj).nodes && nodeHandle == ((FormatNode)obj).nodeHandle;
        }

        public override int GetHashCode()
        {
            return nodeHandle;
        }

        public NodeSubtree Subtree => new NodeSubtree(this);

        public NodeChildren Children => new NodeChildren(this);

        internal struct NodeSubtree : IEnumerable<FormatNode>
        {
            private FormatNode node;

            internal NodeSubtree(FormatNode node)
            {
                this.node = node;
            }

            public SubtreeEnumerator GetEnumerator()
            {
                return new SubtreeEnumerator(node, false/*revisitParent*/);
            }

            public SubtreeEnumerator GetEnumerator(bool revisitParent)
            {
                return new SubtreeEnumerator(node, revisitParent);
            }

            IEnumerator<FormatNode> IEnumerable<FormatNode>.GetEnumerator()
            {
                return new SubtreeEnumerator(node, false/*revisitParent*/);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new SubtreeEnumerator(node, false/*revisitParent*/);
            }
        }

        internal struct NodeChildren : IEnumerable<FormatNode>
        {
            private FormatNode node;

            internal NodeChildren(FormatNode node)
            {
                this.node = node;
            }

            public ChildrenEnumerator GetEnumerator()
            {
                return new ChildrenEnumerator(node);
            }

            IEnumerator<FormatNode> IEnumerable<FormatNode>.GetEnumerator()
            {
                return new ChildrenEnumerator(node);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new ChildrenEnumerator(node);
            }
        }

        internal struct ChildrenEnumerator : IEnumerator<FormatNode>
        {
            private FormatNode node;
            private FormatNode current;
            private FormatNode next;

            internal ChildrenEnumerator(FormatNode node)
            {
                this.node = node;
                current = Null;
                next = this.node.FirstChild;
            }

            object IEnumerator.Current
            {
                get
                {
                    if (current.IsNull)
                    {
                        throw new InvalidOperationException(next.IsNull ? "Strings.ErrorAfterLast" : "Strings.ErrorBeforeFirst");

                    }

                    return current;
                }
            }

            public FormatNode Current
            {
                get
                {
                    if (current.IsNull)
                    {
                        throw new InvalidOperationException(next.IsNull ? "Strings.ErrorAfterLast" : "Strings.ErrorBeforeFirst");
                    }

                    return current;
                }
            }

            public bool MoveNext()
            {
                current = next;

                if (current.IsNull)
                {
                    return false;
                }

                next = current.NextSibling;
                return true;
            }

            public void Reset()
            {
                current = Null;
                next = node.FirstChild;
            }

            public void Dispose()
            {
                Reset();
            }
        }

        internal struct SubtreeEnumerator : IEnumerator<FormatNode>
        {
            [Flags]
            private enum EnumeratorDisposition : byte
            {
                Begin = 0x01,
                End = 0x02,
            }

            private bool revisitParent;
            private EnumeratorDisposition currentDisposition;
            private FormatNode root;
            private FormatNode current;
            private FormatNode nextChild;
            private int depth;

            internal SubtreeEnumerator(FormatNode node, bool revisitParent)
            {
                this.revisitParent = revisitParent;
                root = node;
                current = Null;
                currentDisposition = 0;
                nextChild = node;
                depth = -1;
            }

            public FormatNode Current
            {
                get
                {
                    InternalDebug.Assert(current != Null);

                    return current;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    InternalDebug.Assert(current != Null);

                    return current;
                }
            }

            
            
            
            
            
            
            
            
            public bool FirstVisit
            {
                get
                {
                    InternalDebug.Assert(current != Null);

                    return 0 != (currentDisposition & EnumeratorDisposition.Begin);
                }
            }

            
            
            
            
            
            
            
            
            public bool LastVisit
            {
                get
                {
                    InternalDebug.Assert(current != Null);

                    return 0 != (currentDisposition & EnumeratorDisposition.End);
                }
            }

            
            
            
            
            public int Depth
            {
                get
                {
                    InternalDebug.Assert(current != Null);

                    return depth;
                }
            }

            public bool MoveNext()
            {
                InternalDebug.Assert(depth >= -1);

                if (nextChild != Null)
                {
                    depth ++;

                    current = nextChild;

                    nextChild = current.FirstChild;

                    currentDisposition = EnumeratorDisposition.Begin | (nextChild == Null ? EnumeratorDisposition.End : 0);
                    return true;
                }

                if (depth < 0)
                {
                    InternalDebug.Assert(current == Null);

                    
                    return false;
                }

                do
                {
                    depth --;

                    if (depth < 0)
                    {
                        current = Null;
                        nextChild = Null;
                        currentDisposition = 0;
                        return false;
                    }

                    nextChild = current.NextSibling;

                    current = current.Parent;
                    currentDisposition = (nextChild == Null ? EnumeratorDisposition.End : 0);
                }
                while (!revisitParent && nextChild == Null);

                
                
                

                InternalDebug.Assert(nextChild != Null || revisitParent);
                return revisitParent || MoveNext();
            }

            public FormatNode PreviewNextNode()
            {
                InternalDebug.Assert(this.depth >= -1);

                if (this.nextChild != Null)
                {
                    
                    return this.nextChild;
                }

                if (this.depth < 0)
                {
                    InternalDebug.Assert(this.current == Null);

                    
                    return Null;
                }

                var depth = this.depth;
                var current = this.current;
                FormatNode nextChild;

                do
                {
                    depth --;

                    if (depth < 0)
                    {
                        return Null;
                    }

                    nextChild = current.NextSibling;
                    current = current.Parent;
                }
                while (!revisitParent && nextChild == Null);

                InternalDebug.Assert(nextChild != Null || revisitParent);
                return revisitParent ? current : nextChild;
            }

            
            
            
            
            public void SkipChildren()
            {
                InternalDebug.Assert(current != Null);

                if (nextChild != Null)
                {
                    nextChild = Null;
                    currentDisposition |= EnumeratorDisposition.End;
                }
            }

            void IEnumerator.Reset()
            {
                current = Null;
                currentDisposition = 0;
                nextChild = root;
                depth = -1;
            }

            void IDisposable.Dispose()
            {
                ((IEnumerator)this).Reset();
                GC.SuppressFinalize(this);
            }
        }

    }


    

    internal struct FormatStyle
    {
        internal FormatStore.StyleStore styles;
        internal int styleHandle;

        public static readonly FormatStyle Null = new FormatStyle();

        internal FormatStyle(FormatStore store, int styleHandle)
        {
            styles = store.styles;
            this.styleHandle = styleHandle;
        }

        internal FormatStyle(FormatStore.StyleStore styles, int styleHandle)
        {
            this.styles = styles;
            this.styleHandle = styleHandle;
        }

        public int Handle => styleHandle;

        public bool IsNull => styleHandle == 0;

        internal int RefCount => styles.Plane(styleHandle)[styles.Index(styleHandle)].refCount;

        public bool IsEmpty => styles.Plane(styleHandle)[styles.Index(styleHandle)].propertyMask.IsClear &&
                               (styles.Plane(styleHandle)[styles.Index(styleHandle)].propertyList == null ||
                                styles.Plane(styleHandle)[styles.Index(styleHandle)].propertyList.Length == 0);

        public FlagProperties FlagProperties
        {
            get { return styles.Plane(styleHandle)[styles.Index(styleHandle)].flagProperties; }
            set { styles.Plane(styleHandle)[styles.Index(styleHandle)].flagProperties = value; }
        }

        public PropertyBitMask PropertyMask
        {
            get { return styles.Plane(styleHandle)[styles.Index(styleHandle)].propertyMask; }
            set { styles.Plane(styleHandle)[styles.Index(styleHandle)].propertyMask = value; }
        }

        public Property[] PropertyList
        {
            get { return styles.Plane(styleHandle)[styles.Index(styleHandle)].propertyList; }
            set { styles.Plane(styleHandle)[styles.Index(styleHandle)].propertyList = value; }
        }

        public void AddRef()
        {
            InternalDebug.Assert(styles.Plane(styleHandle)[styles.Index(styleHandle)].refCount > 0);

            if (styles.Plane(styleHandle)[styles.Index(styleHandle)].refCount != Int32.MaxValue)
            {
                styles.Plane(styleHandle)[styles.Index(styleHandle)].refCount ++;
            }
        }

        public void Release()
        {
            InternalDebug.Assert(styles.Plane(styleHandle)[styles.Index(styleHandle)].refCount > 0);

            if (styles.Plane(styleHandle)[styles.Index(styleHandle)].refCount != Int32.MaxValue)
            {
                if (--styles.Plane(styleHandle)[styles.Index(styleHandle)].refCount == 0)
                {
                    styles.Free(styleHandle);
                }
            }

            styleHandle = -1;
        }

    }

    

    internal struct StringValue
    {
        internal FormatStore.StringValueStore strings;
        internal int stringHandle;

        internal StringValue(FormatStore store, int stringHandle)
        {
            strings = store.strings;
            this.stringHandle = stringHandle;
        }

        internal StringValue(FormatStore.StringValueStore strings, int stringHandle)
        {
            this.strings = strings;
            this.stringHandle = stringHandle;
        }

        public PropertyValue PropertyValue => new PropertyValue(PropertyType.String, stringHandle);

        internal int Handle => stringHandle;

        public int Length => strings.Plane(stringHandle)[strings.Index(stringHandle)].str.Length;

        public int RefCount => strings.Plane(stringHandle)[strings.Index(stringHandle)].refCount;

        public string GetString()
        {
            return strings.Plane(stringHandle)[strings.Index(stringHandle)].str;
        }

        internal void SetString(string str)
        {
            strings.Plane(stringHandle)[strings.Index(stringHandle)].str = str;
        }

        public void CopyTo(int sourceOffset, char[] buffer, int offset, int count)
        {
            strings.Plane(stringHandle)[strings.Index(stringHandle)].str.CopyTo(sourceOffset, buffer, offset, count);
        }

        public void AddRef()
        {
            InternalDebug.Assert(strings.Plane(stringHandle)[strings.Index(stringHandle)].refCount > 0);

            if (strings.Plane(stringHandle)[strings.Index(stringHandle)].refCount != Int32.MaxValue)
            {
                

                strings.Plane(stringHandle)[strings.Index(stringHandle)].refCount ++;
            }
        }

        public void Release()
        {
            InternalDebug.Assert(strings.Plane(stringHandle)[strings.Index(stringHandle)].refCount > 0);

            if (strings.Plane(stringHandle)[strings.Index(stringHandle)].refCount != Int32.MaxValue)
            {
                

                if (--strings.Plane(stringHandle)[strings.Index(stringHandle)].refCount == 0)
                {
                    strings.Free(stringHandle);
                }
            }

            stringHandle = -1;
        }
    }

    

    internal struct MultiValue
    {
        internal FormatStore.MultiValueStore multiValues;
        internal int multiValueHandle;

        internal MultiValue(FormatStore store, int multiValueHandle)
        {
            multiValues = store.multiValues;
            this.multiValueHandle = multiValueHandle;
        }

        internal MultiValue(FormatStore.MultiValueStore multiValues, int multiValueHandle)
        {
            this.multiValues = multiValues;
            this.multiValueHandle = multiValueHandle;
        }

        public PropertyValue PropertyValue => new PropertyValue(PropertyType.MultiValue, multiValueHandle);

        internal int Handle => multiValueHandle;

        public int Length => multiValues.Plane(multiValueHandle)[multiValues.Index(multiValueHandle)].values.Length;

        internal int RefCount => multiValues.Plane(multiValueHandle)[multiValues.Index(multiValueHandle)].refCount;

        public PropertyValue this[int index] => multiValues.Plane(multiValueHandle)[multiValues.Index(multiValueHandle)].values[index];

        public StringValue GetStringValue(int index)
        {
            return multiValues.Store.GetStringValue(multiValues.Plane(multiValueHandle)[multiValues.Index(multiValueHandle)].values[index]);
        }

        public void AddRef()
        {
            InternalDebug.Assert(multiValues.Plane(multiValueHandle)[multiValues.Index(multiValueHandle)].refCount > 0);

            if (multiValues.Plane(multiValueHandle)[multiValues.Index(multiValueHandle)].refCount != Int32.MaxValue)
            {
                
                multiValues.Plane(multiValueHandle)[multiValues.Index(multiValueHandle)].refCount ++;
            }
        }

        public void Release()
        {
            InternalDebug.Assert(multiValues.Plane(multiValueHandle)[multiValues.Index(multiValueHandle)].refCount > 0);

            if (multiValues.Plane(multiValueHandle)[multiValues.Index(multiValueHandle)].refCount != Int32.MaxValue)
            {
                

                if (--multiValues.Plane(multiValueHandle)[multiValues.Index(multiValueHandle)].refCount == 0)
                {
                    multiValues.Free(multiValueHandle);
                }
            }

            multiValueHandle = -1;
        }
    }

    

    internal enum TextRunType : ushort
    {
        

        Invalid = (0 << 13),
        Skip = Invalid,

        NonSpace = (1 << 13),

        FirstShort = InlineObject,  

        InlineObject = (2 << 13),

        

        NbSp = (3 << 13),
        Space = (4 << 13),
        Tabulation = (5 << 13),

        NewLine = (6 << 13),
        BlockBoundary = (7 << 13),
    }

    internal struct TextRun
    {
        private FormatStore.TextStore text;
        private uint position;
        private bool isImmutable;

        public static readonly TextRun Invalid = new TextRun();

        internal TextRun(FormatStore.TextStore text, uint position)
        {
            isImmutable = false;
            this.text = text;
            this.position = position;
        }

        public const int MaxEffectiveLength = FormatStore.TextStore.MaxRunEffectivelength;

        

        public uint Position => position;
        public TextRunType Type => FormatStore.TextStore.TypeFromRunHeader(text.Pick(position));
        public int EffectiveLength => FormatStore.TextStore.LengthFromRunHeader(text.Pick(position));
        public int Length { get { var ch = text.Pick(position); return ch >= (char)TextRunType.FirstShort ? 1 : FormatStore.TextStore.LengthFromRunHeader(ch) + 1; } }

        
        
        
        public int WordLength
        {
            get
            {
                var returnValue = 0;

                
                
                
                
                var nextRun = this;

                for(;
                    !nextRun.IsEnd() && nextRun.Type == TextRunType.NonSpace && returnValue < 1024;
                    nextRun = nextRun.GetNext())
                {
                    returnValue += nextRun.EffectiveLength;
                };

                return returnValue;
            }
        }

        private bool IsLong => Type < TextRunType.FirstShort;

        public char this[int index]
        {
            get
            {
                InternalDebug.Assert(Type == TextRunType.NonSpace && index < EffectiveLength);
                return text.Plane(position)[text.Index(position) + 1 + index];
            }
        }

        
        
        
        public char GetWordChar(int index)
        {
            if (index < EffectiveLength)
            {
                return this[index];
            }
            else
            {
                var nextRun = GetNext();
                index -= EffectiveLength;

                while(!nextRun.IsEnd())
                {
                    if (index < nextRun.EffectiveLength)
                    {
                        return nextRun[index];
                    }
                    else if (nextRun.Type != TextRunType.NonSpace)
                    {
                        break;
                    }

                    index -= nextRun.EffectiveLength;
                    nextRun = nextRun.GetNext();
                };

                throw new ArgumentOutOfRangeException("index");
            }
        }

        public void MoveNext()
        {
            if (isImmutable)
            {
                throw new InvalidOperationException("This run is immutable");
            }
            position += (uint)Length;
        }

        
        
        
        internal void MakeImmutable()
        {
            isImmutable = true;
        }

        public void SkipInvalid()
        {
            if (isImmutable)
            {
                throw new InvalidOperationException("This run is immutable");
            }
            while (!IsEnd() && Type == TextRunType.Invalid)
            {
                MoveNext();
            }
        }

        public bool IsEnd()
        {
            InternalDebug.Assert(position <= text.CurrentPosition);
            return position >= text.CurrentPosition;
        }

        public TextRun GetNext()
        {
            return new TextRun(text, position + (uint)Length);
        }

        public void GetChunk(int start, out char[] buffer, out int offset, out int count)
        {
            InternalDebug.Assert(IsLong);
            InternalDebug.Assert(start <= EffectiveLength);

            buffer = text.Plane(position);
            offset = text.Index(position) + 1 + start;
            count = EffectiveLength - start;
        }

        public int AppendFragment(int start, ref ScratchBuffer scratchBuffer, int maxLength)
        {
            InternalDebug.Assert(IsLong);
            InternalDebug.Assert(start <= EffectiveLength);

            var copyOffset = text.Index(position) + 1 + start;
            var copyLength = Math.Min(EffectiveLength - start, maxLength);

            if (copyLength != 0)
            {
                scratchBuffer.Append(text.Plane(position), copyOffset, copyLength);
            }

            return copyLength;
        }

        public void ConvertToInvalid()
        {
            text.ConvertToInvalid(position);
        }

        public void ConvertToInvalid(int count)
        {
            InternalDebug.Assert(Type == TextRunType.NonSpace);
            text.ConvertToInvalid(position, count);
        }

        public void ConvertShort(TextRunType type, int newEffectiveLength)
        {
            InternalDebug.Assert(!IsLong);
            text.ConvertShortRun(position, type, newEffectiveLength);
        }

        public override string ToString()
        {
            var wordLength = WordLength;
            var stringBuilder = new StringBuilder(wordLength);
            for (var iWord = 0; iWord < wordLength; ++iWord)
            {
                stringBuilder.Append(GetWordChar(iWord));
            }

            return stringBuilder.ToString();
        }
    }

    

    internal struct NodePropertiesEnumerator : IEnumerable<Property>, IEnumerator<Property>
    {
        internal FlagProperties flagProperties;          
        internal Property[] properties;                  
        internal int currentPropertyIndex;
        internal Property currentProperty;

        public NodePropertiesEnumerator(FormatNode node)
        {
            flagProperties = node.FlagProperties;
            properties = node.Properties;
            currentPropertyIndex = 0;
            currentProperty = Property.Null;
        }

        public IEnumerator<Property> GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }

        public Property Current => currentProperty;

        Object IEnumerator.Current => currentProperty;

        public bool MoveNext()
        {
            while (currentPropertyIndex < (int)PropertyId.LastFlag)
            {
                currentPropertyIndex ++;

                InternalDebug.Assert(FlagProperties.IsFlagProperty((PropertyId)currentPropertyIndex));

                if (flagProperties.IsDefined((PropertyId)currentPropertyIndex))
                {
                    currentProperty.Set((PropertyId)currentPropertyIndex, new PropertyValue(flagProperties.IsOn((PropertyId)currentPropertyIndex)));
                    return true;
                }
            }

            if (properties != null)
            {
                if (currentPropertyIndex < properties.Length + ((int)PropertyId.LastFlag + 1))
                {
                    currentPropertyIndex ++;

                    if (currentPropertyIndex < properties.Length + ((int)PropertyId.LastFlag + 1))
                    {
                        currentProperty = properties[currentPropertyIndex - ((int)PropertyId.LastFlag + 1)];
                        return true;
                    }
                }
            }

            currentProperty = Property.Null;
            return false;
        }

        public void Reset()
        {
            currentPropertyIndex = 0;
            currentProperty = Property.Null;
        }

        public void Dispose()
        {
        }
    }
}

