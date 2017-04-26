// ***************************************************************
// <copyright file="FormatOutput.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters.Internal.Format
{
    using System;
    using System.Text;
    using System.IO;
    using Data.Internal;
    using Html;
    using Strings = CtsResources.TextConvertersStrings;

    internal enum SourceFormat
    {
        Text,
        Html,
        Rtf,
        Enriched,
    }

    
    internal abstract class FormatOutput : IDisposable
    {
        private SourceFormat sourceFormat;
        private string comment;

        private FormatStore formatStore;
        private FormatNode rootNode;

        private OutputStackEntry currentOutputLevel;
        private OutputStackEntry[] outputStack;
        private int outputStackTop;

        internal ScratchBuffer scratchBuffer;
        internal ScratchBuffer scratchValueBuffer;

        internal PropertyState propertyState = new PropertyState();
#if DEBUG
        internal TextWriter formatOutputTraceWriter;
#endif
        

        private enum OutputState : byte
        {
            NotStarted,
            Started,
            EndPending,
            Ended
        }

        

        private struct OutputStackEntry
        {
            public OutputState state;
            public FormatNode node;
            public int index;
            public int childIndex;
            public int propertyUndoLevel;
        }

        public virtual bool OutputCodePageSameAsInput => false;

        public virtual Encoding OutputEncoding
        {
            set
            {
                InternalDebug.Assert(false, "this should never happen");
                throw new InvalidOperationException();
            }
        }

        public virtual bool CanAcceptMoreOutput => true;

        protected FormatStore FormatStore => formatStore;
        protected SourceFormat SourceFormat => sourceFormat;
        protected string Comment => comment;

        protected FormatNode CurrentNode => currentOutputLevel.node;
        protected int CurrentNodeIndex => currentOutputLevel.index;

        protected FormatOutput(Stream formatOutputTraceStream)
        {
#if DEBUG
            if (formatOutputTraceStream != null)
            {
                formatOutputTraceWriter = new StreamWriter(formatOutputTraceStream);
            }
#endif
        }

        public virtual void Initialize(FormatStore store, SourceFormat sourceFormat, string comment)
        {
            this.sourceFormat = sourceFormat;
            this.comment = comment;

            formatStore = store;

            Restart(formatStore.RootNode);
        }

        public void Restart(FormatNode rootNode)
        {
            outputStackTop = 0;

            currentOutputLevel.node = rootNode;
            currentOutputLevel.state = OutputState.NotStarted;

            this.rootNode = rootNode;
        }

        protected void Restart()
        {
            Restart(rootNode);
        }

        

        public bool HaveSomethingToFlush()
        {
            return currentOutputLevel.node.CanFlush;
        }

        public FlagProperties GetEffectiveFlags()
        {
            return propertyState.GetEffectiveFlags();
        }

        public FlagProperties GetDistinctFlags()
        {
            return propertyState.GetDistinctFlags();
        }

        public PropertyValue GetEffectiveProperty(PropertyId id)
        {
            return propertyState.GetEffectiveProperty(id);
        }

        public PropertyValue GetDistinctProperty(PropertyId id)
        {
            return propertyState.GetDistinctProperty(id);
        }

        public void SubtractDefaultContainerPropertiesFromDistinct(FlagProperties flags, Property[] properties)
        {
            propertyState.SubtractDefaultFromDistinct(flags, properties);
        }

        

        public virtual bool Flush()
        {
            while (CanAcceptMoreOutput && currentOutputLevel.state != OutputState.Ended)
            {
                if (currentOutputLevel.state == OutputState.NotStarted)
                {
                    if (StartCurrentLevel())
                    {
                        PushFirstChild();
                    }
                    else
                    {
                        
                        PopPushNextSibling();
                    }
                }
                else if (currentOutputLevel.state == OutputState.Started)
                {
                    if (ContinueCurrentLevel())
                    {
                        currentOutputLevel.state = OutputState.EndPending;
                    }
                }
                else 
                {
                    InternalDebug.Assert(currentOutputLevel.state == OutputState.EndPending);

                    EndCurrentLevel();

                    currentOutputLevel.state = OutputState.Ended;

                    if (outputStackTop != 0)
                    {
                        
                        PopPushNextSibling();
                    }
                }
            }

            InternalDebug.Assert(currentOutputLevel.state != OutputState.Ended || outputStackTop == 0);
            return currentOutputLevel.state == OutputState.Ended;
        }

        

        public void OutputFragment(FormatNode fragmentNode)
        {
            Restart(fragmentNode);
            FlushFragment();
        }

        

        public void OutputFragment(FormatNode beginNode, uint beginTextPosition, FormatNode endNode, uint endTextPosition)
        {
            Restart(rootNode);

            var node = beginNode;
            var countStartLevels = 0;

            

            while (node != rootNode)
            {
                countStartLevels ++;
                node = node.Parent;
            }

            if (outputStack == null)
            {
                InternalDebug.Assert(outputStackTop == 0 && formatStore != null);

                outputStack = new OutputStackEntry[Math.Max(32, countStartLevels)];
            }
            else if (outputStack.Length < countStartLevels)
            {
                if (outputStackTop >= HtmlSupport.HtmlNestingLimit)
                {
                    throw new TextConvertersException(Strings.InputDocumentTooComplex);
                }

                outputStack = new OutputStackEntry[Math.Max(outputStack.Length * 2, countStartLevels)];
            }

            

            node = beginNode;
            var level = countStartLevels - 1;
            while (node != rootNode)
            {
                outputStack[level--].node = node;
                node = node.Parent;
            }
            InternalDebug.Assert(level == -1);

            

            for (level = 0; level < countStartLevels; level++)
            {
                if (!StartCurrentLevel())
                {
                    PopPushNextSibling();
                    break;
                }

                currentOutputLevel.state = OutputState.Started;

                Push(outputStack[level].node);
            }

            var endReached = false;

            while (currentOutputLevel.state != OutputState.Ended)
            {
                if (currentOutputLevel.state == OutputState.NotStarted)
                {
                    if (StartCurrentLevel())
                    {
                        PushFirstChild();
                    }
                    else
                    {
                        
                        PopPushNextSibling();
                    }
                }
                else if (currentOutputLevel.state == OutputState.Started)
                {
                    InternalDebug.Assert(currentOutputLevel.node.NodeType == FormatContainerType.Text);

                    var nodeBeginTextPosition = currentOutputLevel.node == beginNode ? beginTextPosition : currentOutputLevel.node.BeginTextPosition;
                    var nodeEndTextPosition = currentOutputLevel.node == endNode ? endTextPosition : currentOutputLevel.node.EndTextPosition;

                    if (nodeBeginTextPosition <= nodeEndTextPosition)
                    {
                        ContinueText(nodeBeginTextPosition, nodeEndTextPosition);
                    }

                    currentOutputLevel.state = OutputState.EndPending;
                }
                else 
                {
                    InternalDebug.Assert(currentOutputLevel.state == OutputState.EndPending);

                    EndCurrentLevel();

                    currentOutputLevel.state = OutputState.Ended;

                    if (outputStackTop != 0)
                    {
                        if (!endReached &&
                            currentOutputLevel.node != endNode &&
                            (currentOutputLevel.node.NextSibling.IsNull ||
                            currentOutputLevel.node.NextSibling != endNode ||
                            (currentOutputLevel.node.NextSibling.NodeType == FormatContainerType.Text &&
                                currentOutputLevel.node.NextSibling.BeginTextPosition < endTextPosition)))
                        {
                            
                            PopPushNextSibling();
                        }
                        else
                        {
                            Pop();

                            

                            currentOutputLevel.state = OutputState.EndPending;

                            endReached = true;
                        }
                    }
                }
            }

            InternalDebug.Assert(outputStackTop == 0);
            InternalDebug.Assert(propertyState.UndoStackTop == 0);
        }

        

        private void FlushFragment()
        {
            
            
            

            while (currentOutputLevel.state != OutputState.Ended)
            {
                if (currentOutputLevel.state == OutputState.NotStarted)
                {
                    if (StartCurrentLevel())
                    {
                        PushFirstChild();
                    }
                    else
                    {
                        
                        PopPushNextSibling();
                    }
                }
                else if (currentOutputLevel.state == OutputState.Started)
                {
                    if (ContinueCurrentLevel())
                    {
                        currentOutputLevel.state = OutputState.EndPending;
                    }
                }
                else 
                {
                    InternalDebug.Assert(currentOutputLevel.state == OutputState.EndPending);

                    EndCurrentLevel();

                    currentOutputLevel.state = OutputState.Ended;

                    if (outputStackTop != 0)
                    {
                        
                        PopPushNextSibling();
                    }
                }
            }

            InternalDebug.Assert(outputStackTop == 0);
            InternalDebug.Assert(propertyState.UndoStackTop == 0);
        }

        

        private bool StartCurrentLevel()
        {
            switch (currentOutputLevel.node.NodeType)
            {
                case FormatContainerType.Root:

                        return StartRoot();

                case FormatContainerType.Document:

                        return StartDocument();

                case FormatContainerType.Fragment:

                        return StartFragment();

                case FormatContainerType.BaseFont:

                        StartEndBaseFont();
                        
                        return false;

                case FormatContainerType.Block:

                        return StartBlock();

                case FormatContainerType.BlockQuote:

                        return StartBlockQuote();

                case FormatContainerType.TableContainer:

                        return StartTableContainer();

                case FormatContainerType.TableDefinition:

                        return StartTableDefinition();

                case FormatContainerType.TableColumnGroup:

                        return StartTableColumnGroup();

                case FormatContainerType.TableColumn:

                        StartEndTableColumn();
                        
                        return false;

                case FormatContainerType.TableCaption:

                        return StartTableCaption();

                case FormatContainerType.TableExtraContent:

                        return StartTableExtraContent();

                case FormatContainerType.Table:

                        return StartTable();

                case FormatContainerType.TableRow:

                        return StartTableRow();

                case FormatContainerType.TableCell:

                        return StartTableCell();

                case FormatContainerType.List:

                        return StartList();

                case FormatContainerType.ListItem:

                        return StartListItem();

                case FormatContainerType.HyperLink:

                        return StartHyperLink();

                case FormatContainerType.Bookmark:

                        return StartBookmark();

                case FormatContainerType.Image:

                        StartEndImage();
                        
                        return false;

                case FormatContainerType.HorizontalLine:

                        StartEndHorizontalLine();
                        
                        return false;

                case FormatContainerType.Inline:

                        return StartInline();

                case FormatContainerType.Map:

                        return StartMap();

                case FormatContainerType.Area:

                        StartEndArea();
                        
                        return false;

                case FormatContainerType.Form:

                        return StartForm();

                case FormatContainerType.FieldSet:

                        return StartFieldSet();

                case FormatContainerType.Label:

                        return StartLabel();

                case FormatContainerType.Input:

                        return StartInput();

                case FormatContainerType.Button:

                        return StartButton();

                case FormatContainerType.Legend:

                        return StartLegend();

                case FormatContainerType.TextArea:

                        return StartTextArea();

                case FormatContainerType.Select:

                        return StartSelect();

                case FormatContainerType.OptionGroup:

                        return StartOptionGroup();

                case FormatContainerType.Option:

                        return StartOption();

                case FormatContainerType.Text:

                        
                        
                        return StartText();
            }

            
            InternalDebug.Assert(false);
            return true;
        }

        

        private bool ContinueCurrentLevel()
        {
            InternalDebug.Assert(currentOutputLevel.node.NodeType == FormatContainerType.Text);

            

            return ContinueText(currentOutputLevel.node.BeginTextPosition, currentOutputLevel.node.EndTextPosition);
        }

        

        private void EndCurrentLevel()
        {
            switch (currentOutputLevel.node.NodeType)
            {
#if DEBUG
                default:
                case FormatContainerType.BaseFont:
                case FormatContainerType.Image:
                case FormatContainerType.HorizontalLine:
                case FormatContainerType.Area:
                case FormatContainerType.TableColumn:

                        InternalDebug.Assert(false);
                        break;
#endif
                case FormatContainerType.Root:

                        EndRoot();
                        break;

                case FormatContainerType.Document:

                        EndDocument();
                        break;

                case FormatContainerType.Fragment:

                        EndFragment();
                        break;

                case FormatContainerType.Block:

                        EndBlock();
                        break;

                case FormatContainerType.BlockQuote:

                        EndBlockQuote();
                        break;

                case FormatContainerType.TableContainer:

                        EndTableContainer();
                        break;

                case FormatContainerType.TableDefinition:

                        EndTableDefinition();
                        break;

                case FormatContainerType.TableColumnGroup:

                        EndTableColumnGroup();
                        break;

                case FormatContainerType.TableCaption:

                        EndTableCaption();
                        break;

                case FormatContainerType.TableExtraContent:

                        EndTableExtraContent();
                        break;

                case FormatContainerType.Table:

                        EndTable();
                        break;

                case FormatContainerType.TableRow:

                        EndTableRow();
                        break;

                case FormatContainerType.TableCell:

                        EndTableCell();
                        break;

                case FormatContainerType.List:

                        EndList();
                        break;

                case FormatContainerType.ListItem:

                        EndListItem();
                        break;

                case FormatContainerType.HyperLink:

                        EndHyperLink();
                        break;

                case FormatContainerType.Bookmark:

                        EndBookmark();
                        break;

                case FormatContainerType.Inline:

                        EndInline();
                        break;

                case FormatContainerType.Map:

                        EndMap();
                        break;

                case FormatContainerType.Form:

                        EndForm();
                        break;

                case FormatContainerType.FieldSet:

                        EndFieldSet();
                        break;

                case FormatContainerType.Label:

                        EndLabel();
                        break;

                case FormatContainerType.Input:

                        EndInput();
                        break;

                case FormatContainerType.Button:

                        EndButton();
                        break;

                case FormatContainerType.Legend:

                        EndLegend();
                        break;

                case FormatContainerType.TextArea:

                        EndTextArea();
                        break;

                case FormatContainerType.Select:

                        EndSelect();
                        break;

                case FormatContainerType.OptionGroup:

                        EndOptionGroup();
                        break;

                case FormatContainerType.Option:

                        EndOption();
                        break;

                case FormatContainerType.Text:

                        EndText();
                        break;
            }
        }

        

        private void PushFirstChild()
        {
            var firstChild = currentOutputLevel.node.FirstChild;

            if (!firstChild.IsNull)
            {
                currentOutputLevel.state = OutputState.Started;

                Push(firstChild);
            }
            else if (currentOutputLevel.node.IsText)
            {
                
                
                currentOutputLevel.state = OutputState.Started;
            }
            else
            {
                currentOutputLevel.state = OutputState.EndPending;
            }
        }

        

        private void PopPushNextSibling()
        {
            var nextSibling = currentOutputLevel.node.NextSibling;

            Pop();

            currentOutputLevel.childIndex++;

            if (!nextSibling.IsNull)
            {
                InternalDebug.Assert(currentOutputLevel.state == OutputState.Started);

                Push(nextSibling);
            }
            else
            {
                currentOutputLevel.state = OutputState.EndPending;
            }
        }

        

        private void Push(FormatNode node)
        {
            if (outputStack == null)
            {
                InternalDebug.Assert(outputStackTop == 0 && formatStore != null);

                outputStack = new OutputStackEntry[32];
            }
            else if (outputStackTop == outputStack.Length)
            {
                if (outputStackTop >= HtmlSupport.HtmlNestingLimit)
                {
                    throw new TextConvertersException(Strings.InputDocumentTooComplex);
                }

                var newOutputStack = new OutputStackEntry[outputStack.Length * 2];
                Array.Copy(outputStack, 0, newOutputStack, 0, outputStackTop);
                outputStack = newOutputStack;
            }

#if DEBUG
            if (formatOutputTraceWriter != null)
            {
                formatOutputTraceWriter.WriteLine("{0}{1} Push {2}", Indent(outputStackTop), outputStackTop, node.NodeType.ToString());
                
                formatOutputTraceWriter.Flush();
            }
#endif
            outputStack[outputStackTop++] = currentOutputLevel;

            currentOutputLevel.node = node;
            currentOutputLevel.state = OutputState.NotStarted;
            currentOutputLevel.index = currentOutputLevel.childIndex;
            currentOutputLevel.childIndex = 0;
            currentOutputLevel.propertyUndoLevel = propertyState.ApplyProperties(node.FlagProperties, node.Properties, FormatStoreData.GlobalInheritanceMasks[node.InheritanceMaskIndex].flagProperties, FormatStoreData.GlobalInheritanceMasks[node.InheritanceMaskIndex].propertyMask);
#if DEBUG
            if (formatOutputTraceWriter != null)
            {
                formatOutputTraceWriter.WriteLine("{0}{1} Props After {2}", Indent(outputStackTop), outputStackTop, propertyState.ToString());
                formatOutputTraceWriter.Flush();
            }
#endif
            
            node.SetOnLeftEdge();
        }

        

        private void Pop()
        {
            InternalDebug.Assert(outputStackTop != 0);

            if (outputStackTop != 0)
            {
#if DEBUG
                if (formatOutputTraceWriter != null)
                {
                    formatOutputTraceWriter.WriteLine("{0}{1} Pop {2}", Indent(outputStackTop), outputStackTop, currentOutputLevel.node.NodeType.ToString());
                    
                    formatOutputTraceWriter.Flush();
                }
#endif
                
                currentOutputLevel.node.ResetOnLeftEdge();

                
                propertyState.UndoProperties(currentOutputLevel.propertyUndoLevel);

                

                currentOutputLevel = outputStack[--outputStackTop];
#if DEBUG
                if (formatOutputTraceWriter != null)
                {
                    
                    formatOutputTraceWriter.Flush();
                }
#endif
            }
        }

        

        

        protected virtual bool StartRoot()
        {
            return StartBlockContainer();
        }

        protected virtual void EndRoot()
        {
            EndBlockContainer();
        }

        protected virtual bool StartDocument()
        {
            return StartBlockContainer();
        }

        protected virtual void EndDocument()
        {
            EndBlockContainer();
        }

        protected virtual bool StartFragment()
        {
            return StartBlockContainer();
        }

        protected virtual void EndFragment()
        {
            EndBlockContainer();
        }

        protected virtual void StartEndBaseFont()
        {
        }

        protected virtual bool StartBlock()
        {
            return StartBlockContainer();
        }

        protected virtual void EndBlock()
        {
            EndBlockContainer();
        }

        protected virtual bool StartBlockQuote()
        {
            return StartBlockContainer();
        }

        protected virtual void EndBlockQuote()
        {
            EndBlockContainer();
        }

        protected virtual bool StartTableContainer()
        {
            return true;
        }

        protected virtual void EndTableContainer()
        {
        }

        protected virtual bool StartTableDefinition()
        {
            return true;
        }

        protected virtual void EndTableDefinition()
        {
        }

        protected virtual bool StartTableColumnGroup()
        {
            return true;
        }

        protected virtual void EndTableColumnGroup()
        {
        }

        protected virtual void StartEndTableColumn()
        {
        }

        protected virtual bool StartTableCaption()
        {
            return StartBlockContainer();
        }

        protected virtual void EndTableCaption()
        {
            EndBlockContainer();
        }

        protected virtual bool StartTableExtraContent()
        {
            return StartBlockContainer();
        }

        protected virtual void EndTableExtraContent()
        {
            EndBlockContainer();
        }

        protected virtual bool StartTable()
        {
            return StartBlockContainer();
        }

        protected virtual void EndTable()
        {
            EndBlockContainer();
        }

        protected virtual bool StartTableRow()
        {
            return StartBlockContainer();
        }

        protected virtual void EndTableRow()
        {
            EndBlockContainer();
        }

        protected virtual bool StartTableCell()
        {
            return StartBlockContainer();
        }

        protected virtual void EndTableCell()
        {
            EndBlockContainer();
        }

        protected virtual bool StartList()
        {
            return StartBlockContainer();
        }

        protected virtual void EndList()
        {
            EndBlockContainer();
        }

        protected virtual bool StartListItem()
        {
            return StartBlockContainer();
        }

        protected virtual void EndListItem()
        {
            EndBlockContainer();
        }

        protected virtual bool StartHyperLink()
        {
            return StartInlineContainer();
        }

        protected virtual void EndHyperLink()
        {
            EndInlineContainer();
        }

        protected virtual bool StartBookmark()
        {
            return true;
        }

        protected virtual void EndBookmark()
        {
        }

        protected virtual void StartEndImage()
        {
        }

        protected virtual void StartEndHorizontalLine()
        {
        }

        protected virtual bool StartInline()
        {
            return StartInlineContainer();
        }

        protected virtual void EndInline()
        {
            EndInlineContainer();
        }

        protected virtual bool StartMap()
        {
            return StartBlockContainer();
        }

        protected virtual void EndMap()
        {
            EndBlockContainer();
        }

        protected virtual void StartEndArea()
        {
        }

        protected virtual bool StartForm()
        {
            return StartInlineContainer();
        }

        protected virtual void EndForm()
        {
            EndInlineContainer();
        }

        protected virtual bool StartFieldSet()
        {
            return StartBlockContainer();
        }

        protected virtual void EndFieldSet()
        {
            EndBlockContainer();
        }

        protected virtual bool StartLabel()
        {
            return StartInlineContainer();
        }

        protected virtual void EndLabel()
        {
            EndInlineContainer();
        }

        protected virtual bool StartInput()
        {
            return StartInlineContainer();
        }

        protected virtual void EndInput()
        {
            EndInlineContainer();
        }

        protected virtual bool StartButton()
        {
            return StartInlineContainer();
        }

        protected virtual void EndButton()
        {
            EndInlineContainer();
        }

        protected virtual bool StartLegend()
        {
            return StartInlineContainer();
        }

        protected virtual void EndLegend()
        {
            EndInlineContainer();
        }

        protected virtual bool StartTextArea()
        {
            return StartInlineContainer();
        }

        protected virtual void EndTextArea()
        {
            EndInlineContainer();
        }

        protected virtual bool StartSelect()
        {
            return StartInlineContainer();
        }

        protected virtual void EndSelect()
        {
            EndInlineContainer();
        }

        protected virtual bool StartOptionGroup()
        {
            return true;
        }

        protected virtual void EndOptionGroup()
        {
        }

        protected virtual bool StartOption()
        {
            return true;
        }

        protected virtual void EndOption()
        {
        }


        protected virtual bool StartText()
        {
            return StartInlineContainer();
        }

        protected virtual bool ContinueText(uint beginTextPosition, uint endTextPosition)
        {
            return true;
        }

        protected virtual void EndText()
        {
            EndInlineContainer();
        }

        private static string Indent(int level)
        {
            const string spaces = "                                                  ";
            return spaces.Substring(0, Math.Min(spaces.Length, level * 2));
        }

        protected virtual bool StartBlockContainer()
        {
            return true;
        }

        protected virtual void EndBlockContainer()
        {
        }

        protected virtual bool StartInlineContainer()
        {
            return true;
        }

        protected virtual void EndInlineContainer()
        {
        }

        

        protected virtual void Dispose(bool disposing)
        {
            currentOutputLevel.node = FormatNode.Null;
            outputStack = null;
            formatStore = null;
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

