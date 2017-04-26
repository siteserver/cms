// ***************************************************************
// <copyright file="HtmlToHtmlConverter.cs" company="Microsoft">
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


    internal class HtmlToHtmlConverter : IProducerConsumer, IRestartable, IDisposable
    {
        private bool convertFragment;
        private bool outputFragment;
        private bool filterForFragment;

        private bool filterHtml;
        private bool truncateForCallback;

        private int smallCssBlockThreshold;     
        private bool preserveDisplayNoneStyle;  

        private bool hasTailInjection;

        private IHtmlParser parser;
        private bool endOfFile;

        private bool normalizedInput;

        internal HtmlWriter writer;

        private HtmlTagCallback callback;
        private HtmlToHtmlTagContext callbackContext;

        internal HtmlToken token;

        private bool headDivUnterminated;

        private int currentLevel;
        private int currentLevelDelta;          

        private bool insideCSS;

        private int dropLevel = Int32.MaxValue;

        private EndTagActionEntry[] endTagActionStack;
        private int endTagActionStackTop;

        private bool tagDropped;
        private bool justTruncated;
        private bool tagCallbackRequested;
        private bool attributeTriggeredCallback;
        private bool endTagCallbackRequested;
        private bool ignoreAttrCallback;
        private bool styleIsCSS;

        private HtmlFilterData.FilterAction attrContinuationAction;

        private CopyPendingState copyPendingState;

        private HtmlTagIndex tagIndex;

        private int attributeCount;
        private int attributeSkipCount;

        private bool attributeIndirect;
        private AttributeIndirectEntry[] attributeIndirectIndex;

        private AttributeVirtualEntry[] attributeVirtualList;
        private int attributeVirtualCount;
        private ScratchBuffer attributeVirtualScratch;

        private ScratchBuffer attributeActionScratch;
        private bool attributeLeadingSpaces;

        private bool metaInjected;
        private bool insideHtml;
        private bool insideHead;
        private bool insideBody;

        private bool tagHasFilteredStyleAttribute;

        private CssParser cssParser;
        private ConverterBufferInput cssParserInput;

        private VirtualScratchSink virtualScratchSink;

        private IProgressMonitor progressMonitor;

        private static readonly string NamePrefix = "x_";

        
        internal enum CopyPendingState : byte
        {
            NotPending,
            TagCopyPending,                     
            TagContentCopyPending,              
            TagNameCopyPending,                 
            AttributeCopyPending,               
            AttributeNameCopyPending,           
            AttributeValueCopyPending,          
        }

        
        [Flags]
        private enum AvailableTagParts : byte
        {
            None = 0,
            TagBegin = 0x01,
            TagEnd = 0x02,
            TagName = 0x04,
            Attributes = 0x08,
            UnstructuredContent = 0x10,
        }

        
        private enum AttributeIndirectKind
        {
            PassThrough,                
            EmptyValue,                 
            FilteredStyle,              
            Virtual,                    
            VirtualFilteredStyle,       
            NameOnlyFragment,           
        }

        
        private struct AttributeIndirectEntry
        {
            public AttributeIndirectKind kind;
            public short index;         
        }

        
        private struct AttributeVirtualEntry
        {
            public short index;         
            public int offset;
            public int length;
            public int position;
        }

        
        private struct EndTagActionEntry
        {
            public int tagLevel;
            public bool drop;
            public bool callback;
        }

        
        public HtmlToHtmlConverter(
                    IHtmlParser parser,
                    HtmlWriter writer,
                    bool convertFragment,
                    bool outputFragment,
                    bool filterHtml,
                    HtmlTagCallback callback,
                    bool truncateForCallback,
                    bool hasTailInjection,
                    Stream traceStream,
                    bool traceShowTokenNum,
                    int traceStopOnTokenNum,
                    int smallCssBlockThreshold,
                    bool preserveDisplayNoneStyle,
                    IProgressMonitor progressMonitor)
        {

            this.writer = writer;

            normalizedInput = parser is HtmlNormalizingParser;

            InternalDebug.Assert(progressMonitor != null);
            this.progressMonitor = progressMonitor;

            
            
            
            InternalDebug.Assert(!(outputFragment && !normalizedInput));

            this.convertFragment = convertFragment;
            this.outputFragment = outputFragment;

            filterForFragment = outputFragment || convertFragment;

            this.filterHtml = filterHtml || filterForFragment;
            this.callback = callback;

            this.parser = parser;
            if (!convertFragment)
            {
                this.parser.SetRestartConsumer(this);
            }

            this.truncateForCallback = truncateForCallback;

            this.hasTailInjection = hasTailInjection;

            this.smallCssBlockThreshold = smallCssBlockThreshold;
            this.preserveDisplayNoneStyle = preserveDisplayNoneStyle;
        }

        
        void IDisposable.Dispose()
        {
            if (parser != null && parser is IDisposable)
            {
                ((IDisposable)parser).Dispose();
            }

            if (!convertFragment && writer != null && writer is IDisposable)
            {
                ((IDisposable)writer).Dispose();
            }

            if (token != null && token is IDisposable)
            {
                ((IDisposable)token).Dispose();
            }

            parser = null;
            writer = null;

            token = null;

            GC.SuppressFinalize(this);
        }

        private CopyPendingState CopyPendingStateFlag
        {
            get
            {
                return copyPendingState;
            }
            set
            {
                writer.SetCopyPending(value != CopyPendingState.NotPending);
                copyPendingState = value;
            }
        }


        
        public void Run()
        {
            
            if (!endOfFile)
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
            if (!endOfFile)
            {
                Run();
            }

            return endOfFile;
        }

        
        public void Initialize(string fragment, bool preformatedText)
        {
            InternalDebug.Assert(convertFragment);

            if (parser is HtmlNormalizingParser)
            {
                ((HtmlNormalizingParser)parser).Initialize(fragment, preformatedText);
            }
            else
            {
                ((HtmlParser)parser).Initialize(fragment, preformatedText);
            }

            ((IRestartable)this).Restart();
        }

        
        bool IRestartable.CanRestart()
        {
            if (writer is IRestartable)
            {
                return ((IRestartable)writer).CanRestart();
            }

            return false;
        }

        
        void IRestartable.Restart()
        {
            if (writer is IRestartable && !convertFragment)
            {
                ((IRestartable)writer).Restart();
            }

            

            endOfFile = false;

            token = null;

            styleIsCSS = true;
            insideCSS = false;

            headDivUnterminated = false;

            tagDropped = false;
            justTruncated = false;
            tagCallbackRequested = false;
            endTagCallbackRequested = false;
            ignoreAttrCallback = false;
            attrContinuationAction = HtmlFilterData.FilterAction.Unknown;
            currentLevel = 0;
            currentLevelDelta = 0;
            dropLevel = Int32.MaxValue;
            endTagActionStackTop = 0;

            copyPendingState = CopyPendingState.NotPending;

            metaInjected = false;
            insideHtml = false;
            insideHead = false;
            insideBody = false;
        }

        
        void IRestartable.DisableRestart()
        {
            if (writer is IRestartable)
            {
                ((IRestartable)writer).DisableRestart();
            }
        }

        
        private void Process(HtmlTokenId tokenId)
        {
            token = parser.Token;

            if (!metaInjected)
            {
                if (!InjectMetaTagIfNecessary())
                {
                    
                    return;
                }
            }

            switch (tokenId)
            {
                case HtmlTokenId.Tag:

                    if (!token.IsEndTag)
                    {
                        ProcessStartTag();
                    }
                    else
                    {
                        ProcessEndTag();
                    }
                    break;

                case HtmlTokenId.OverlappedClose:

                    ProcessOverlappedClose();
                    break;

                case HtmlTokenId.OverlappedReopen:

                    ProcessOverlappedReopen();
                    break;

                case HtmlTokenId.Text:

                    ProcessText();
                    break;

                case HtmlTokenId.InjectionBegin:

                    ProcessInjectionBegin();
                    break;

                case HtmlTokenId.InjectionEnd:

                    ProcessInjectionEnd();
                    break;

                case HtmlTokenId.Restart:

                    break;

                case HtmlTokenId.EncodingChange:

                    

                    if (writer.HasEncoding && writer.CodePageSameAsInput)
                    {
                        var codePage = token.Argument;

#if DEBUG
                        Encoding newOutputEncoding;
                        
                        InternalDebug.Assert(Charset.TryGetEncoding(codePage, out newOutputEncoding));
#endif

                        
                        
                        writer.Encoding = Charset.GetEncoding(codePage);
                    }
                    break;

                case HtmlTokenId.EndOfFile:

                    ProcessEof();
                    break;
            }
        }

        
        private void ProcessStartTag()
        {
            AvailableTagParts availableParts = 0;

            if (insideCSS &&
                token.TagIndex == HtmlTagIndex._COMMENT &&
                filterHtml)
            {
                
                AppendCssFromTokenText();

                
                return;
            }

            

            if (token.IsTagBegin)
            {
                InternalDebug.Assert(CopyPendingStateFlag == CopyPendingState.NotPending);

                
                currentLevel++;

                tagIndex = token.TagIndex;

                
                
                tagDropped = false;
                justTruncated = false;
                endTagCallbackRequested = false;

                

                
                PreProcessStartTag();

                if (currentLevel >= dropLevel)
                {
                    

                    tagDropped = true;
                }
                else if (!tagDropped)
                {
                    tagCallbackRequested = false;
                    ignoreAttrCallback = false;

                    if (filterHtml || callback != null)
                    {
                        InternalDebug.Assert(normalizedInput);

                        var tagAction = filterForFragment ?
                                HtmlFilterData.filterInstructions[(int)token.NameIndex].tagFragmentAction :
                                HtmlFilterData.filterInstructions[(int)token.NameIndex].tagAction;

                        if (callback != null && 0 != (tagAction & HtmlFilterData.FilterAction.Callback))
                        {
                            
                            

                            tagCallbackRequested = true;
                        }
                        else if (filterHtml)
                        {
                            

                            ignoreAttrCallback = (0 != (tagAction & HtmlFilterData.FilterAction.IgnoreAttrCallbacks));

                            switch (tagAction & HtmlFilterData.FilterAction.ActionMask)
                            {
                                case HtmlFilterData.FilterAction.Drop:
                                    
                                    tagDropped = true;
                                    dropLevel = currentLevel;
                                    break;

                                case HtmlFilterData.FilterAction.DropKeepContent:
                                    
                                    tagDropped = true;
                                    break;

                                case HtmlFilterData.FilterAction.KeepDropContent:
                                    
                                    dropLevel = currentLevel + 1;
                                    break;

                                case HtmlFilterData.FilterAction.Keep:
                                    
                                    break;
                            }
                        }
                    }

                    if (!tagDropped)
                    {
                        

                        attributeTriggeredCallback = false;
                        tagHasFilteredStyleAttribute = false;

                        availableParts = AvailableTagParts.TagBegin;
                    }
                }
            }

            if (!tagDropped)
            {
                

                var tagMinorPart = token.MinorPart;

                if (token.IsTagEnd)
                {
                    availableParts |= AvailableTagParts.TagEnd;
                }

                if (tagIndex < HtmlTagIndex.Unknown)
                {
                    InternalDebug.Assert(CopyPendingStateFlag == CopyPendingState.NotPending || CopyPendingStateFlag == CopyPendingState.TagCopyPending);

                    availableParts |= AvailableTagParts.UnstructuredContent;
                    attributeCount = 0;
                }
                else
                {
                    if (token.HasNameFragment || token.IsTagNameEnd)
                    {
                        availableParts |= AvailableTagParts.TagName;
                    }

                    

                    ProcessTagAttributes();

                    if (attributeCount != 0)
                    {
                        availableParts |= AvailableTagParts.Attributes;
                    }
                }

                

                if (availableParts != 0)
                {
                    

                    if (CopyPendingStateFlag != CopyPendingState.NotPending)
                    {
                        
                        InternalDebug.Assert(!token.IsTagBegin);

                        switch (CopyPendingStateFlag)
                        {
                            case CopyPendingState.TagCopyPending:

                                CopyInputTag(true);

                                if (tagCallbackRequested && 0 != (availableParts & AvailableTagParts.TagEnd))
                                {
                                    
                                    
                                    

                                    
                                    
                                    attributeCount = 0;
                                    token.Name.MakeEmpty();

                                    
                                    availableParts &= ~AvailableTagParts.TagEnd;
                                    tagMinorPart = 0;
                                }
                                else
                                {
                                    
                                    
                                    availableParts = AvailableTagParts.None;

                                    
                                }
                                break;

                            case CopyPendingState.TagNameCopyPending:

                                InternalDebug.Assert(tagIndex == HtmlTagIndex.Unknown && !token.IsTagNameBegin);

                                token.Name.WriteTo(writer.WriteTagName());

                                if (token.IsTagNameEnd)
                                {
                                    CopyPendingStateFlag = CopyPendingState.NotPending;
                                }

                                token.Name.MakeEmpty();

                                availableParts &= ~AvailableTagParts.TagName;
                                tagMinorPart &= ~HtmlToken.TagPartMinor.CompleteName;
                                break;

                            case CopyPendingState.AttributeCopyPending:

                                InternalDebug.Assert(GetAttribute(0).Index == 0);
                                InternalDebug.Assert(!token.Attributes[0].IsAttrBegin && attributeCount != 0);

                                CopyInputAttribute(0);

                                attributeSkipCount = 1;
                                attributeCount--;

                                if (0 == attributeCount)
                                {
                                    availableParts &= ~AvailableTagParts.Attributes;
                                }

                                tagMinorPart &= ~HtmlToken.TagPartMinor.AttributePartMask;
                                break;

                            case CopyPendingState.AttributeNameCopyPending:

                                InternalDebug.Assert(GetAttribute(0).Index == 0);
                                InternalDebug.Assert(!token.Attributes[0].IsAttrBegin &&
                                                    0 != (token.Attributes[0].MinorPart & HtmlToken.AttrPartMinor.ContinueName) &&
                                                    attributeCount != 0);

                                CopyInputAttributeName(0);

                                if (1 == attributeCount && 0 == (token.Attributes[0].MinorPart & HtmlToken.AttrPartMinor.ContinueValue))
                                {
                                    attributeSkipCount = 1;
                                    attributeCount--;

                                    availableParts &= ~AvailableTagParts.Attributes;
                                    tagMinorPart &= ~HtmlToken.TagPartMinor.AttributePartMask;

                                    InternalDebug.Assert(availableParts == 0 || availableParts == AvailableTagParts.TagEnd);
                                }
                                else
                                {
                                    token.Attributes[0].Name.MakeEmpty();
                                    
                                    token.Attributes[0].SetMinorPart(token.Attributes[0].MinorPart & ~HtmlToken.AttrPartMinor.CompleteName);
                                }

                                break;

                            case CopyPendingState.AttributeValueCopyPending:

                                InternalDebug.Assert(GetAttribute(0).Index == 0);
                                InternalDebug.Assert(!token.Attributes[0].IsAttrBegin &&
                                                    (0 != (token.Attributes[0].MinorPart & HtmlToken.AttrPartMinor.ContinueValue) ||
                                                     token.Attributes[0].MinorPart == HtmlToken.AttrPartMinor.Empty ||
                                                     token.Attributes[0].IsAttrEnd) &&
                                                    attributeCount != 0);

                                CopyInputAttributeValue(0);

                                attributeSkipCount = 1;
                                attributeCount--;

                                if (0 == attributeCount)
                                {
                                    availableParts &= ~AvailableTagParts.Attributes;
                                }

                                tagMinorPart &= ~HtmlToken.TagPartMinor.AttributePartMask;
                                break;

                            default:

                                InternalDebug.Assert(false);
                                break;
                        }
                    }

                    
                    
                    
                    

                    if (availableParts != 0)
                    {
                        

                        if (tagCallbackRequested)
                        {
                            InternalDebug.Assert(!truncateForCallback || token.IsTagBegin);

                            InternalDebug.Assert(callback != null);

                            if (callbackContext == null)
                            {
                                
                                callbackContext = new HtmlToHtmlTagContext(this);
                            }

                            InternalDebug.Assert(CopyPendingStateFlag == CopyPendingState.NotPending);

                            if (token.IsTagBegin || attributeTriggeredCallback)
                            {
                                
                                callbackContext.InitializeTag(false/*this.token.IsEndTag*/, HtmlDtd.tags[(int)tagIndex].nameIndex, false);

                                attributeTriggeredCallback = false;
                            }

                            callbackContext.InitializeFragment(token.IsEmptyScope, attributeCount, new HtmlTagParts(token.MajorPart, token.MinorPart));

                            
                            callback(callbackContext, writer);

                            
                            callbackContext.UninitializeFragment();

                            if (token.IsTagEnd || truncateForCallback)
                            {
                                if (callbackContext.IsInvokeCallbackForEndTag)
                                {
                                    
                                    

                                    endTagCallbackRequested = true;
                                }

                                if (callbackContext.IsDeleteInnerContent)
                                {
                                    dropLevel = currentLevel + 1;
                                }

                                if (token.IsTagBegin && callbackContext.IsDeleteEndTag)
                                {
                                    tagDropped = true;
                                }

                                if (!tagDropped && !token.IsTagEnd)
                                {
                                    InternalDebug.Assert(truncateForCallback);

                                    tagDropped = true;

                                    
                                    justTruncated = true;

                                    CopyPendingStateFlag = CopyPendingState.NotPending;
                                }
                            }
                        }
                        else
                        {
                            

                            if (token.IsTagBegin)
                            {
                                CopyInputTag(false);
                            }

                            if (attributeCount != 0)
                            {
                                CopyInputTagAttributes();
                            }

                            if (token.IsTagEnd)
                            {
                                if (tagIndex == HtmlTagIndex.Unknown)
                                {
                                    writer.WriteTagEnd(token.IsEmptyScope);
                                }
                            }
                        }
                    }
                }
            }

            

            if (token.IsTagEnd)
            {
                InternalDebug.Assert(CopyPendingStateFlag == CopyPendingState.NotPending);

                
                

                if (writer.IsTagOpen)
                {
                    writer.WriteTagEnd();
                }

                

                if (!token.IsEmptyScope && tagIndex > HtmlTagIndex.Unknown)
                {
                    
                    

                    if (normalizedInput && currentLevel < dropLevel &&
                        ((tagDropped && !justTruncated) || endTagCallbackRequested))
                    {
                        if (endTagActionStack == null)
                        {
                            endTagActionStack = new EndTagActionEntry[4];
                        }
                        else if (endTagActionStack.Length == endTagActionStackTop)
                        {
                            var newEndTagActionStack = new EndTagActionEntry[endTagActionStack.Length * 2];
                            Array.Copy(endTagActionStack, 0, newEndTagActionStack, 0, endTagActionStackTop);
                            endTagActionStack = newEndTagActionStack;
                        }

                        endTagActionStack[endTagActionStackTop].tagLevel = currentLevel;
                        endTagActionStack[endTagActionStackTop].drop = tagDropped && !justTruncated;
                        endTagActionStack[endTagActionStackTop].callback = endTagCallbackRequested;

                        endTagActionStackTop++;
                    }

                    

                    currentLevel++;

                    PostProcessStartTag();
                }
                else
                {
                    

                    InternalDebug.Assert(!normalizedInput || currentLevel > 0);

                    currentLevel--;

                    if (dropLevel != Int32.MaxValue && currentLevel < dropLevel)
                    {
                        dropLevel = Int32.MaxValue;
                    }
                }
            }
        }

        
        private void ProcessEndTag()
        {
            AvailableTagParts availableParts = 0;

            InternalDebug.Assert(token.TagIndex >= HtmlTagIndex.Unknown);

            if (token.IsTagBegin)
            {
                InternalDebug.Assert(CopyPendingStateFlag == CopyPendingState.NotPending);
                InternalDebug.Assert(!normalizedInput || currentLevel > 0 || token.TagIndex == HtmlTagIndex.Unknown);

                
                if (currentLevel > 0)
                {
                    currentLevel--;
                }

                tagIndex = token.TagIndex;

                
                
                tagDropped = false;
                tagCallbackRequested = false;
                tagHasFilteredStyleAttribute = false;

                availableParts = AvailableTagParts.TagBegin;

                
                PreProcessEndTag();

                
                

                
                

                InternalDebug.Assert(endTagActionStackTop == 0 || endTagActionStack[endTagActionStackTop - 1].tagLevel <= currentLevel + currentLevelDelta);

                if (currentLevel >= dropLevel)
                {
                    
                    tagDropped = true;
                }
                else
                {
                    if (endTagActionStackTop != 0 && tagIndex > HtmlTagIndex.Unknown)
                    {
                        
                        
                        

                        if (endTagActionStack[endTagActionStackTop - 1].tagLevel >= currentLevel)
                        {
                            

                            if (endTagActionStack[endTagActionStackTop - 1].tagLevel == currentLevel)
                            {
                                

                                endTagActionStackTop--;

                                tagDropped = endTagActionStack[endTagActionStackTop].drop;
                                tagCallbackRequested = endTagActionStack[endTagActionStackTop].callback;
                            }
                            else
                            {
                                
                                InternalDebug.Assert(currentLevelDelta != 0);

                                int stackPos;

                                

                                for (stackPos = endTagActionStackTop; stackPos > 0 && endTagActionStack[stackPos - 1].tagLevel > currentLevel; stackPos--)
                                {
                                }

                                
                                
                                
                                
                                
                                

                                for (var j = stackPos; j < endTagActionStackTop; j++)
                                {
                                    
                                    
                                    endTagActionStack[j].tagLevel -= 2;
                                }

                                

                                if (stackPos > 0 && endTagActionStack[stackPos - 1].tagLevel == currentLevel)
                                {
                                    
                                    tagDropped = endTagActionStack[stackPos - 1].drop;
                                    tagCallbackRequested = endTagActionStack[stackPos - 1].callback;

                                    

                                    for (; stackPos < endTagActionStackTop; stackPos++)
                                    {
                                        
                                        
                                        endTagActionStack[stackPos - 1] = endTagActionStack[stackPos];
                                    }

                                    endTagActionStackTop--;
                                }
                            }
                        }
                    }

                    if (token.Argument == 1 && tagIndex == HtmlTagIndex.Unknown)
                    {
                        
                        
                        

                        tagDropped = true;
                    }
                }
            }

            HtmlToken.TagPartMinor tagMinorPart;

            if (!tagDropped)
            {
                tagMinorPart = token.MinorPart & ~(HtmlToken.TagPartMinor.AttributePartMask | HtmlToken.TagPartMinor.Attributes);

                if (token.IsTagEnd)
                {
                    availableParts |= AvailableTagParts.TagEnd;
                }

                if (token.HasNameFragment)
                {
                    availableParts |= AvailableTagParts.TagName;
                }

                if (CopyPendingStateFlag == CopyPendingState.TagNameCopyPending)
                {
                    InternalDebug.Assert(!token.IsTagBegin);

                    

                    token.Name.WriteTo(writer.WriteTagName());

                    if (token.IsTagNameEnd)
                    {
                        CopyPendingStateFlag = CopyPendingState.NotPending;
                    }

                    token.Name.MakeEmpty();

                    availableParts &= ~AvailableTagParts.TagName;
                    tagMinorPart &= ~HtmlToken.TagPartMinor.CompleteName;
                }

                if (availableParts != 0)
                {
                    if (tagCallbackRequested)
                    {
                        InternalDebug.Assert(callback != null && callbackContext != null);

                        InternalDebug.Assert(CopyPendingStateFlag == CopyPendingState.NotPending);

                        if (token.IsTagBegin)
                        {
                            
                            callbackContext.InitializeTag(true/*this.token.IsEndTag*/, HtmlDtd.tags[(int)tagIndex].nameIndex, false);
                        }

                        
                        callbackContext.InitializeFragment(false, 0, new HtmlTagParts(token.MajorPart, tagMinorPart));

                        
                        callback(callbackContext, writer);

                        
                        callbackContext.UninitializeFragment();
                    }
                    else
                    {
                        

                        if (token.IsTagBegin)
                        {
                            CopyInputTag(false);
                        }
                    }
                }
            }
            else if (tagCallbackRequested)
            {
                

                tagMinorPart = token.MinorPart & ~(HtmlToken.TagPartMinor.AttributePartMask | HtmlToken.TagPartMinor.Attributes);

                InternalDebug.Assert(callback != null && callbackContext != null);

                InternalDebug.Assert(CopyPendingStateFlag == CopyPendingState.NotPending);

                if (token.IsTagBegin)
                {
                    
                    callbackContext.InitializeTag(true/*this.token.IsEndTag*/, HtmlDtd.tags[(int)tagIndex].nameIndex, true/*droppedEndTag*/);
                }

                callbackContext.InitializeFragment(false, 0, new HtmlTagParts(token.MajorPart, tagMinorPart));

                
                callback(callbackContext, writer);

                
                callbackContext.UninitializeFragment();
            }

            if (token.IsTagEnd)
            {
                
                

                if (writer.IsTagOpen)
                {
                    writer.WriteTagEnd();
                }

                if (tagIndex > HtmlTagIndex.Unknown)
                {
                    InternalDebug.Assert(!normalizedInput || currentLevel > 0);

                    if (currentLevel > 0)
                    {
                        currentLevel--;
                    }

                    if (dropLevel != Int32.MaxValue && currentLevel < dropLevel)
                    {
                        dropLevel = Int32.MaxValue;
                    }
                }
                else
                {
                    

                    if (currentLevel > 0)
                    {
                        currentLevel++;
                    }
                }
            }
        }

        
        private void ProcessOverlappedClose()
        {
            InternalDebug.Assert(currentLevelDelta == 0);

            
            currentLevelDelta = token.Argument * 2;
            currentLevel -= currentLevelDelta;
        }

        
        private void ProcessOverlappedReopen()
        {
            InternalDebug.Assert(currentLevelDelta == token.Argument * 2);

            
            currentLevel += token.Argument * 2;
            currentLevelDelta = 0;
        }

        
        private void ProcessText()
        {
            if (currentLevel >= dropLevel)
            {
                return;
            }

            if (insideCSS && filterHtml)
            {
                
                AppendCssFromTokenText();

                
            }
            else if (token.Argument == 1)
            {
                

                InternalDebug.Assert(token.Text.Length == 1/* todo: && this.token.Text[0].IsWhitespace*/);

                writer.WriteCollapsedWhitespace();
            }
            else if (token.Runs.MoveNext(true))
            {
                
                token.Text.WriteTo(writer.WriteText());
            }
        }

        
        private void ProcessInjectionBegin()
        {
            

            if (token.Argument == 0)
            {
                

                if (headDivUnterminated)
                {
                    writer.WriteEndTag(HtmlNameIndex.Div);
                    writer.WriteAutoNewLine(true);

                    headDivUnterminated = false;
                }
            }
        }

        
        private void ProcessInjectionEnd()
        {
            

            if (token.Argument != 0)
            {
                

                writer.WriteAutoNewLine(true);
                writer.WriteStartTag(HtmlNameIndex.Div);

                
                

                headDivUnterminated = true;
            }
        }

        
        private void ProcessEof()
        {
            writer.SetCopyPending(false);

            if (headDivUnterminated && dropLevel != 0)
            {
                

                writer.WriteEndTag(HtmlNameIndex.Div);
                writer.WriteAutoNewLine(true);

                headDivUnterminated = false;
            }

            

            if (outputFragment && !insideBody)
            {
                
                writer.WriteStartTag(HtmlNameIndex.Div);
                writer.WriteEndTag(HtmlNameIndex.Div);
                writer.WriteAutoNewLine(true);
            }

            if (!convertFragment)
            {
                writer.Flush();
            }

            endOfFile = true;
        }

        
        private void PreProcessStartTag()
        {
            InternalDebug.Assert(token.IsTagBegin && !token.IsEndTag);

            if (tagIndex > HtmlTagIndex.Unknown)
            {
                

                if (tagIndex == HtmlTagIndex.Body)
                {
                    if (outputFragment)
                    {
                        insideBody = true;

                        
                        tagIndex = HtmlTagIndex.Div;
                    }
                }
                else if (tagIndex == HtmlTagIndex.Meta)
                {
                    
                    if (!filterHtml)
                    {
                        
                        

                        
                        
                        
                        
                        
                        

                        
                        
                        
                        

                        token.Attributes.Rewind();

                        foreach (var attribute in token.Attributes)
                        {
                            if (attribute.NameIndex == HtmlNameIndex.HttpEquiv)
                            {
                                if (attribute.Value.CaseInsensitiveCompareEqual("content-type") ||
                                    attribute.Value.CaseInsensitiveCompareEqual("charset"))
                                {
                                    tagDropped = true;
                                    break;
                                }
                            }
                            else if (attribute.NameIndex == HtmlNameIndex.Charset)
                            {
                                tagDropped = true;
                                break;
                            }
                        }
                    }
                }
                else if (tagIndex == HtmlTagIndex.Style)
                {
#if false
                    
                    

                    if (!this.outputFragment)
                    {
#endif

                    
                    styleIsCSS = true;

                    if (token.Attributes.Find(HtmlNameIndex.Type))
                    {
                        var attribute = token.Attributes.Current;

                        if (!attribute.Value.CaseInsensitiveCompareEqual("text/css"))
                        {
                            
                            styleIsCSS = false;
                        }
                    }

#if false
                    
                    }
                    else
                    {
                        
                        
                        this.tagDropped = true;
                        this.dropLevel = this.currentLevel;
                    }
#endif
                }
                else if (tagIndex == HtmlTagIndex.TC)
                {
                    
                    tagDropped = true;
                }
                else if (tagIndex == HtmlTagIndex.PlainText || tagIndex == HtmlTagIndex.Xmp)
                {
                    if (filterHtml || (hasTailInjection && tagIndex == HtmlTagIndex.PlainText))
                    {
                        
                        

                        tagDropped = true;

                        writer.WriteAutoNewLine(true);
                        writer.WriteStartTag(HtmlNameIndex.TT);
                        writer.WriteStartTag(HtmlNameIndex.Pre);
                        writer.WriteAutoNewLine();
                    }
                }
                else if (tagIndex == HtmlTagIndex.Image)
                {
                    if (filterHtml)
                    {
                        
                        tagIndex = HtmlTagIndex.Img;
                    }
                }
            }
        }

        
        private void ProcessTagAttributes()
        {
            attributeSkipCount = 0;

            var attributes = token.Attributes;
            HtmlAttribute attribute;
            HtmlFilterData.FilterAction attrAction;

            if (filterHtml)
            {
                attributeCount = 0;
                attributeIndirect = true;
                attributeVirtualCount = 0;
                attributeVirtualScratch.Reset();

                if (attributeIndirectIndex == null)
                {
                    attributeIndirectIndex = new AttributeIndirectEntry[Math.Max(attributes.Count + 1, 32)];
                }
                else if (attributeIndirectIndex.Length <= attributes.Count)
                {
                    attributeIndirectIndex = new AttributeIndirectEntry[Math.Max(attributeIndirectIndex.Length * 2, attributes.Count + 1)];
                }

                for (var i = 0; i < attributes.Count; i++)
                {
                    attribute = attributes[i];

                    if (attribute.IsAttrBegin)
                    {
                        

                        attrAction = filterForFragment ?
                                HtmlFilterData.filterInstructions[(int)attribute.NameIndex].attrFragmentAction :
                                HtmlFilterData.filterInstructions[(int)attribute.NameIndex].attrAction;

                        if (0 != (attrAction & HtmlFilterData.FilterAction.HasExceptions) &&
                            0 != (HtmlFilterData.filterInstructions[(int)token.NameIndex].tagAction & HtmlFilterData.FilterAction.HasExceptions))
                        {
                            

                            for (var j = 0; j < HtmlFilterData.filterExceptions.Length; j++)
                            {
                                if (HtmlFilterData.filterExceptions[j].tagNameIndex == token.NameIndex &&
                                    HtmlFilterData.filterExceptions[j].attrNameIndex == attribute.NameIndex)
                                {
                                    attrAction = filterForFragment ?
                                            HtmlFilterData.filterExceptions[j].fragmentAction :
                                            HtmlFilterData.filterExceptions[j].action;
                                    break;
                                }
                            }
                        }

                        if (!outputFragment &&
                            (attrAction == HtmlFilterData.FilterAction.PrefixName || attrAction == HtmlFilterData.FilterAction.PrefixNameList))
                        {
                            
                            
                            attrAction = HtmlFilterData.FilterAction.Keep;
                        }

                        if (callback != null && !ignoreAttrCallback && 0 != (attrAction & HtmlFilterData.FilterAction.Callback))
                        {
                            

                            if (token.IsTagBegin || !truncateForCallback)
                            {
                                

                                attributeTriggeredCallback = attributeTriggeredCallback || !tagCallbackRequested;
                                tagCallbackRequested = true;
                            }
                            else
                            {
                                
                                
                                

                                attrAction = HtmlFilterData.FilterAction.KeepDropContent;
                            }

                            
                            
                            
                            
                            
                            
                        }

                        
                        attrAction &= HtmlFilterData.FilterAction.ActionMask;

                        if (!attribute.IsAttrEnd)
                        {
                            
                            InternalDebug.Assert(attribute.Index == attributes.Count - 1 && !token.IsTagEnd);

                            
                            

                            attrContinuationAction = attrAction;
                        }
                    }
                    else
                    {
                        
                        InternalDebug.Assert(attribute.Index == 0 && !token.IsTagBegin);
                        attrAction = attrContinuationAction;
                    }

                    

                    if (attrAction != HtmlFilterData.FilterAction.Drop)
                    {
                        if (attrAction == HtmlFilterData.FilterAction.Keep)
                        {
                            attributeIndirectIndex[attributeCount].index = (short)i;
                            attributeIndirectIndex[attributeCount].kind = AttributeIndirectKind.PassThrough;
                            attributeCount++;
                        }
                        else
                        {
                            if (attrAction == HtmlFilterData.FilterAction.KeepDropContent)
                            {
                                
                                InternalDebug.Assert(attribute.IsAttrBegin);

                                
                                attrContinuationAction = HtmlFilterData.FilterAction.Drop;

                                
                                attributeIndirectIndex[attributeCount].index = (short)i;
                                attributeIndirectIndex[attributeCount].kind = AttributeIndirectKind.EmptyValue;
                                attributeCount++;
                            }
                            else if (attrAction == HtmlFilterData.FilterAction.FilterStyleAttribute)
                            {
                                if (attribute.IsAttrBegin)
                                {
                                    if (tagHasFilteredStyleAttribute)
                                    {
                                        
                                        
                                        

                                        
                                        
                                        
                                        
                                        
                                        

                                        AppendCss(";");
                                    }

                                    tagHasFilteredStyleAttribute = true;
                                }

                                AppendCssFromAttribute(attribute);

                                
                                continue;
                            }
                            else if (attrAction == HtmlFilterData.FilterAction.ConvertBgcolorIntoStyle)
                            {
                                if (attribute.IsAttrBegin)
                                {
                                    if (tagHasFilteredStyleAttribute)
                                    {
                                        

                                        AppendCss(";");
                                    }

                                    tagHasFilteredStyleAttribute = true;
                                }

                                
                                
                                
                                
                                
                                
                                
                                
                                

                                AppendCss("background-color:");
                                AppendCssFromAttribute(attribute);

                                
                                continue;
                            }
                            else
                            {
                                int vi;
                                int offset, length;

                                
                                InternalDebug.Assert(attrAction == HtmlFilterData.FilterAction.SanitizeUrl ||
                                                    attrAction == HtmlFilterData.FilterAction.PrefixName ||
                                                    attrAction == HtmlFilterData.FilterAction.PrefixNameList);

                                if (attribute.IsAttrBegin)
                                {
                                    
                                    attributeLeadingSpaces = true;
                                }

                                if (attributeLeadingSpaces)
                                {
                                    if (!attribute.Value.SkipLeadingWhitespace() && !attribute.IsAttrEnd)
                                    {
                                        

                                        if (!attribute.IsAttrBegin && !attribute.HasNameFragment)
                                        {
                                            
                                            continue;
                                        }

                                        
                                        attributeIndirectIndex[attributeCount].index = (short)i;
                                        attributeIndirectIndex[attributeCount].kind = AttributeIndirectKind.NameOnlyFragment;
                                        attributeCount++;
                                        continue;
                                    }

                                    attributeLeadingSpaces = false;
                                    attributeActionScratch.Reset();
                                }

                                var truncate = false;

                                
                                
                                
                                
                                if (!attributeActionScratch.AppendHtmlAttributeValue(attribute, 4 * 1024))
                                {
                                    
                                    truncate = true;
                                }

                                if (!attribute.IsAttrEnd && !truncate)
                                {
                                    

                                    if (!attribute.IsAttrBegin && !attribute.HasNameFragment)
                                    {
                                        
                                        continue;
                                    }

                                    
                                    attributeIndirectIndex[attributeCount].index = (short)i;
                                    attributeIndirectIndex[attributeCount].kind = AttributeIndirectKind.NameOnlyFragment;
                                    attributeCount++;
                                    continue;
                                }

                                
                                
                                attrContinuationAction = HtmlFilterData.FilterAction.Drop;

                                
                                

                                if (attrAction == HtmlFilterData.FilterAction.SanitizeUrl)
                                {
                                    switch (CheckUrl(attributeActionScratch.Buffer, attributeActionScratch.Length, tagCallbackRequested))
                                    {
                                        case CheckUrlResult.LocalHyperlink:

                                            InternalDebug.Assert(attributeActionScratch[0] == '#');

                                            if (!outputFragment)
                                            {
                                                goto case CheckUrlResult.Safe;
                                            }

                                            var nameLength = NonWhitespaceLength(attributeActionScratch.Buffer, 1, attributeActionScratch.Length - 1);

                                            if (nameLength == 0)
                                            {
                                                
                                                goto default;
                                            }

                                            
                                            offset = attributeVirtualScratch.Length;
                                            length = 0;
                                            length += attributeVirtualScratch.Append('#', int.MaxValue);
                                            length += attributeVirtualScratch.Append(NamePrefix, int.MaxValue);
                                            length += attributeVirtualScratch.Append(attributeActionScratch.Buffer, 1, nameLength, int.MaxValue);

                                            
                                            vi = AllocateVirtualEntry(i, offset, length);

                                            
                                            attributeIndirectIndex[attributeCount].index = (short)vi;
                                            attributeIndirectIndex[attributeCount].kind = AttributeIndirectKind.Virtual;
                                            attributeCount++;
                                            break;

                                        case CheckUrlResult.Safe:

                                            if (attribute.IsCompleteAttr)
                                            {
                                                
                                                attributeIndirectIndex[attributeCount].index = (short)i;
                                                attributeIndirectIndex[attributeCount].kind = AttributeIndirectKind.PassThrough;
                                                attributeCount++;
                                                break;
                                            }

                                            
                                            

                                            
                                            offset = attributeVirtualScratch.Length;
                                            length = attributeVirtualScratch.Append(attributeActionScratch.Buffer, 0, attributeActionScratch.Length, int.MaxValue);

                                            
                                            vi = AllocateVirtualEntry(i, offset, length);

                                            
                                            attributeIndirectIndex[attributeCount].index = (short)vi;
                                            attributeIndirectIndex[attributeCount].kind = AttributeIndirectKind.Virtual;
                                            attributeCount++;
                                            break;

                                        case CheckUrlResult.Inconclusive:

                                            if (attributeActionScratch.Length <= 256 && attribute.IsAttrEnd)
                                            {
                                                InternalDebug.Assert(attribute.IsAttrEnd);

                                                
                                                goto case CheckUrlResult.Safe;
                                            }

                                            
                                            goto default;

                                        default:    

                                            
                                            attrContinuationAction = HtmlFilterData.FilterAction.Drop;

                                            
                                            attributeIndirectIndex[attributeCount].index = (short)i;
                                            attributeIndirectIndex[attributeCount].kind = AttributeIndirectKind.EmptyValue;
                                            attributeCount++;
                                            break;
                                    }
                                }
                                else if (attrAction == HtmlFilterData.FilterAction.PrefixName)
                                {
                                    InternalDebug.Assert(outputFragment);

                                    offset = attributeVirtualScratch.Length;
                                    length = 0;

                                    var nameLength = NonWhitespaceLength(attributeActionScratch.Buffer, 0, attributeActionScratch.Length);

                                    if (nameLength != 0)
                                    {
                                        
                                        length += attributeVirtualScratch.Append(NamePrefix, int.MaxValue);
                                        length += attributeVirtualScratch.Append(attributeActionScratch.Buffer, 0, nameLength, int.MaxValue);
                                    }

                                    
                                    vi = AllocateVirtualEntry(i, offset, length);

                                    
                                    attributeIndirectIndex[attributeCount].index = (short)vi;
                                    attributeIndirectIndex[attributeCount].kind = AttributeIndirectKind.Virtual;
                                    attributeCount++;
                                }
                                else if (attrAction == HtmlFilterData.FilterAction.PrefixNameList)
                                {
                                    InternalDebug.Assert(outputFragment);

                                    
                                    offset = attributeVirtualScratch.Length;
                                    length = 0;

                                    var nameOffset = 0;
                                    var nameLength = NonWhitespaceLength(attributeActionScratch.Buffer, nameOffset, attributeActionScratch.Length - nameOffset);

                                    if (nameLength != 0)
                                    {
                                        do
                                        {
                                            length += attributeVirtualScratch.Append(NamePrefix, int.MaxValue);
                                            length += attributeVirtualScratch.Append(attributeActionScratch.Buffer, nameOffset, nameLength, int.MaxValue);

                                            nameOffset += nameLength;
                                            nameOffset += WhitespaceLength(attributeActionScratch.Buffer, nameOffset, attributeActionScratch.Length - nameOffset);

                                            nameLength = NonWhitespaceLength(attributeActionScratch.Buffer, nameOffset, attributeActionScratch.Length - nameOffset);

                                            if (nameLength != 0)
                                            {
                                                length += attributeVirtualScratch.Append(' ', int.MaxValue);
                                            }
                                        }
                                        while (nameLength != 0);
                                    }

                                    
                                    vi = AllocateVirtualEntry(i, offset, length);

                                    
                                    attributeIndirectIndex[attributeCount].index = (short)vi;
                                    attributeIndirectIndex[attributeCount].kind = AttributeIndirectKind.Virtual;
                                    attributeCount++;
                                }
                                else
                                {
                                    
                                    InternalDebug.Assert(false);

                                    
                                    InternalDebug.Assert((attribute.MinorPart & HtmlToken.AttrPartMinor.BeginValue) == HtmlToken.AttrPartMinor.BeginValue);

                                    
                                    attrContinuationAction = HtmlFilterData.FilterAction.Drop;

                                    
                                    attributeIndirectIndex[attributeCount].index = (short)i;
                                    attributeIndirectIndex[attributeCount].kind = AttributeIndirectKind.EmptyValue;
                                    attributeCount++;
                                }
                            }
                        }
                    }
                }

                if (tagHasFilteredStyleAttribute && (token.IsTagEnd || (tagCallbackRequested && truncateForCallback)))
                {
                    
                    attributeIndirectIndex[attributeCount].index = -1;
                    attributeIndirectIndex[attributeCount].kind = AttributeIndirectKind.FilteredStyle;
                    attributeCount++;
                }
            }
            else
            {
                attributeCount = attributes.Count;
                attributeIndirect = false;

                if (callback != null && !tagCallbackRequested && !ignoreAttrCallback)
                {
                    
                    

                    for (var i = 0; i < attributes.Count; i++)
                    {
                        attribute = attributes[i];

                        if (attribute.IsAttrBegin)
                        {
                            

                            attrAction = HtmlFilterData.filterInstructions[(int)attribute.NameIndex].attrAction;

                            if (0 != (attrAction & HtmlFilterData.FilterAction.HasExceptions) &&
                                0 != (HtmlFilterData.filterInstructions[(int)token.NameIndex].tagAction & HtmlFilterData.FilterAction.HasExceptions))
                            {
                                

                                for (var j = 0; j < HtmlFilterData.filterExceptions.Length; j++)
                                {
                                    if (HtmlFilterData.filterExceptions[j].tagNameIndex == token.NameIndex &&
                                        HtmlFilterData.filterExceptions[j].attrNameIndex == attribute.NameIndex)
                                    {
                                        attrAction = HtmlFilterData.filterExceptions[j].action;
                                        break;
                                    }
                                }
                            }

                            if (0 != (attrAction & HtmlFilterData.FilterAction.Callback))
                            {
                                

                                if (token.IsTagBegin || !truncateForCallback)
                                {
                                    

                                    attributeTriggeredCallback = attributeTriggeredCallback || !tagCallbackRequested;
                                    tagCallbackRequested = true;

                                    
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private static int WhitespaceLength(char[] buffer, int offset, int remainingLength)
        {
            var length = 0;

            while (remainingLength != 0)
            {
                if (!ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(buffer[offset++])))
                {
                    break;
                }

                length++;
                remainingLength--;
            }

            return length;
        }

        private static int NonWhitespaceLength(char[] buffer, int offset, int remainingLength)
        {
            var length = 0;

            while (remainingLength != 0)
            {
                if (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(buffer[offset++])))
                {
                    break;
                }

                length++;
                remainingLength--;
            }

            return length;
        }

        
        private void PostProcessStartTag()
        {
            if (tagIndex == HtmlTagIndex.Style && styleIsCSS)
            {
                insideCSS = true;
            }
        }

        
        private void PreProcessEndTag()
        {
            InternalDebug.Assert(token.IsTagBegin && token.IsEndTag);

            if (tagIndex > HtmlTagIndex.Unknown)
            {
                if (0 != (HtmlDtd.tags[(int)tagIndex].literal & HtmlDtd.Literal.Entities))
                {
                    

                    if (tagIndex == HtmlTagIndex.Style)
                    {
                        if (insideCSS && filterHtml)
                        {
                            
                            FlushCssInStyleTag();
                        }
                    }

                    insideCSS = false;
                    styleIsCSS = true;     
                }

                if (tagIndex == HtmlTagIndex.PlainText || tagIndex == HtmlTagIndex.Xmp)
                {
                    
                    
                    
                    
                    

                    if (filterHtml || (hasTailInjection && tagIndex == HtmlTagIndex.PlainText))
                    {
                        
                        

                        tagDropped = true;
                        writer.WriteEndTag(HtmlNameIndex.Pre);
                        writer.WriteEndTag(HtmlNameIndex.TT);
                    }
                    else if (tagIndex == HtmlTagIndex.PlainText)
                    {
                        if (normalizedInput)
                        {
                            
                            
                            

                            tagDropped = true;
                            dropLevel = 0;                 
                            endTagActionStackTop = 0;
                        }
                    }
                }
                else if (tagIndex == HtmlTagIndex.Body)
                {
                    if (headDivUnterminated && dropLevel != 0)
                    {
                        

                        writer.WriteEndTag(HtmlNameIndex.Div);
                        writer.WriteAutoNewLine(true);

                        headDivUnterminated = false;
                    }

                    if (outputFragment)
                    {
                        tagIndex = HtmlTagIndex.Div;
                    }
                }
                else if (tagIndex == HtmlTagIndex.TC)
                {
                    
                    tagDropped = true;
                }
                else if (tagIndex == HtmlTagIndex.Image)
                {
                    if (filterHtml)
                    {
                        
                        tagIndex = HtmlTagIndex.Img;
                    }
                }
            }
            else if (tagIndex == HtmlTagIndex.Unknown && filterHtml)
            {
                
                tagDropped = true;
            }
        }

        
        internal void CopyInputTag(bool copyTagAttributes)
        {
            if (token.IsTagBegin)
            {
                writer.WriteTagBegin(HtmlDtd.tags[(int)tagIndex].nameIndex, null, token.IsEndTag, token.IsAllowWspLeft, token.IsAllowWspRight);
            }

            if (tagIndex <= HtmlTagIndex.Unknown)
            {
                if (tagIndex < HtmlTagIndex.Unknown)
                {
                    InternalDebug.Assert(!token.HasNameFragment && attributeCount == 0);

                    token.UnstructuredContent.WriteTo(writer.WriteUnstructuredTagContent());

                    if (token.IsTagEnd)
                    {
                        CopyPendingStateFlag = CopyPendingState.NotPending;
                    }
                    else
                    {
                        CopyPendingStateFlag = CopyPendingState.TagCopyPending;
                    }
                    return;
                }
                else
                {
                    if (token.HasNameFragment)
                    {
                        token.Name.WriteTo(writer.WriteTagName());

                        if (!token.IsTagNameEnd)
                        {
                            InternalDebug.Assert(token.Attributes.Count == 0 && !token.IsTagEnd);

                            if (!copyTagAttributes)
                            {
                                
                                CopyPendingStateFlag = CopyPendingState.TagNameCopyPending;
                                return;
                            }
                        }
                    }
                }
            }

            if (!copyTagAttributes)
            {
                CopyPendingStateFlag = CopyPendingState.NotPending;
                return;
            }

            if (attributeCount != 0)
            {
                CopyInputTagAttributes();
            }

            if (token.IsTagEnd)
            {
                CopyPendingStateFlag = CopyPendingState.NotPending;
            }
            else
            {
                CopyPendingStateFlag = CopyPendingState.TagCopyPending;
            }
        }

        
        private void CopyInputTagAttributes()
        {
            

            InternalDebug.Assert(!token.IsEndTag);

            for (var i = 0; i < attributeCount; i++)
            {
                CopyInputAttribute(i);
            }
        }

        
        internal void CopyInputAttribute(int index)
        {
            var kind = GetAttributeIndirectKind(index);

            if (kind == AttributeIndirectKind.FilteredStyle)
            {
                if (!tagCallbackRequested)
                {
                    

                    writer.WriteAttributeName(HtmlNameIndex.Style);

                    InternalDebug.Assert(cssParserInput != null);
                    if (!cssParserInput.IsEmpty)
                    {
                        FlushCssInStyleAttribute(writer);
                    }
                    else
                    {
                        writer.WriteAttributeValueInternal(String.Empty);
                    }
                    return;
                }

                
                VirtualizeFilteredStyle(index);
                kind = AttributeIndirectKind.VirtualFilteredStyle;
            }

            var endAttr = true;

            if (kind == AttributeIndirectKind.VirtualFilteredStyle)
            {
                
                writer.WriteAttributeName(HtmlNameIndex.Style);

                
                var vi = GetAttributeVirtualEntryIndex(index);

                if (attributeVirtualList[vi].length != 0)
                {
                    writer.WriteAttributeValueInternal(attributeVirtualScratch.Buffer, attributeVirtualList[vi].offset, attributeVirtualList[vi].length);
                }
                else
                {
                    writer.WriteAttributeValueInternal(String.Empty);
                }
                
            }
            else
            {
                var attribute = GetAttribute(index);

                InternalDebug.Assert(!attribute.IsDeleted);

                if (attribute.IsAttrBegin)
                {
                    if (attribute.NameIndex != HtmlNameIndex.Unknown)
                    {
                        writer.WriteAttributeName(attribute.NameIndex);
                    }
                }

                if (attribute.NameIndex == HtmlNameIndex.Unknown && (attribute.HasNameFragment || attribute.IsAttrBegin))
                {
                    attribute.Name.WriteTo(writer.WriteAttributeName());
                }

                if (kind == AttributeIndirectKind.NameOnlyFragment)
                {
                    InternalDebug.Assert(!attribute.IsAttrEnd);
                    endAttr = false;
                }
                else if (kind == AttributeIndirectKind.EmptyValue)
                {
                    writer.WriteAttributeValueInternal(String.Empty);
                    
                }
                else if (kind == AttributeIndirectKind.Virtual)
                {
                    
                    var vi = GetAttributeVirtualEntryIndex(index);

                    if (attributeVirtualList[vi].length != 0)
                    {
                        writer.WriteAttributeValueInternal(attributeVirtualScratch.Buffer, attributeVirtualList[vi].offset, attributeVirtualList[vi].length);
                    }
                    else
                    {
                        writer.WriteAttributeValueInternal(String.Empty);
                    }
                    
                }
                else
                {
                    if (attribute.HasValueFragment)
                    {
                        attribute.Value.WriteTo(writer.WriteAttributeValue());
                    }

                    endAttr = attribute.IsAttrEnd;
                }
            }

            if (endAttr)
            {
                CopyPendingStateFlag = CopyPendingState.NotPending;
            }
            else
            {
                CopyPendingStateFlag = CopyPendingState.AttributeCopyPending;
            }
        }

        
        internal void CopyInputAttributeName(int index)
        {
            var kind = GetAttributeIndirectKind(index);

            if (kind == AttributeIndirectKind.FilteredStyle || kind == AttributeIndirectKind.VirtualFilteredStyle)
            {
                writer.WriteAttributeName(HtmlNameIndex.Style);
                return;
            }

            

            var attribute = GetAttribute(index);

            InternalDebug.Assert(!attribute.IsDeleted);

            if (attribute.IsAttrBegin)
            {
                if (attribute.NameIndex != HtmlNameIndex.Unknown)
                {
                    writer.WriteAttributeName(attribute.NameIndex);
                }
            }

            if (attribute.NameIndex == HtmlNameIndex.Unknown && (attribute.HasNameFragment || attribute.IsAttrBegin))
            {
                attribute.Name.WriteTo(writer.WriteAttributeName());
            }

            if (attribute.IsAttrNameEnd)
            {
                CopyPendingStateFlag = CopyPendingState.NotPending;
            }
            else
            {
                CopyPendingStateFlag = CopyPendingState.AttributeNameCopyPending;
            }
        }

        
        internal void CopyInputAttributeValue(int index)
        {
            var kind = GetAttributeIndirectKind(index);

            var endAttr = true;

            if (kind != AttributeIndirectKind.PassThrough)
            {
                if (kind == AttributeIndirectKind.FilteredStyle)
                {
                    if (!tagCallbackRequested)
                    {
                        

                        InternalDebug.Assert(cssParserInput != null);
                        if (!cssParserInput.IsEmpty)
                        {
                            FlushCssInStyleAttribute(writer);
                        }
                        else
                        {
                            writer.WriteAttributeValueInternal(String.Empty);
                        }
                        return;
                    }

                    
                    VirtualizeFilteredStyle(index);
                    kind = AttributeIndirectKind.VirtualFilteredStyle;
                }

                if (kind == AttributeIndirectKind.Virtual || kind == AttributeIndirectKind.VirtualFilteredStyle)
                {
                    
                    var vi = GetAttributeVirtualEntryIndex(index);

                    if (attributeVirtualList[vi].length != 0)
                    {
                        writer.WriteAttributeValueInternal(attributeVirtualScratch.Buffer, attributeVirtualList[vi].offset, attributeVirtualList[vi].length);
                    }
                    else
                    {
                        writer.WriteAttributeValueInternal(String.Empty);
                    }
                    
                }
                else if (kind == AttributeIndirectKind.NameOnlyFragment)
                {
                    InternalDebug.Assert(!GetAttribute(index).IsAttrEnd);
                    endAttr = false;
                }
                else if (kind == AttributeIndirectKind.EmptyValue)
                {
                    writer.WriteAttributeValueInternal(String.Empty);
                    
                }
            }
            else
            {
                var attribute = GetAttribute(index);

                InternalDebug.Assert(!attribute.IsDeleted);

                if (attribute.HasValueFragment)
                {
                    attribute.Value.WriteTo(writer.WriteAttributeValue());
                }

                endAttr = attribute.IsAttrEnd;
            }

            if (endAttr)
            {
                CopyPendingStateFlag = CopyPendingState.NotPending;
            }
            else
            {
                CopyPendingStateFlag = CopyPendingState.AttributeValueCopyPending;
            }
        }

        
        private static object lockObject = new object();
        private static bool textConvertersConfigured;

        private static Dictionary<string, string> safeUrlDictionary;

        internal static void RefreshConfiguration()
        {
            textConvertersConfigured = false;
        }

        internal static bool TestSafeUrlSchema(string schema)
        {
            if (schema.Length < 2 || schema.Length > 20)
            {
                return false;
            }

            if (!textConvertersConfigured)
            {
                ConfigureTextConverters();
            }

            

            if (safeUrlDictionary.ContainsKey(schema))
            {
                return true;
            }

            return false;
        }

        private static void ConfigureTextConverters()
        {
            lock (lockObject)
            {
                if (!textConvertersConfigured)
                {
                    /* example of the configuration:

                    <TextConverters>

                        <SafeUrlScheme Add="foo bar buz"/>
                        -- or --
                        <SafeUrlScheme Override="foo bar buz"/>

                    </TextConverters>
                    */

                    safeUrlDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                    var safeUrlSchemeListSpecified = false;
                    var safeUrlSchemeListAdd = true;

                    if (!safeUrlSchemeListSpecified || safeUrlSchemeListAdd)
                    {
                        

                        
                        

                        safeUrlDictionary["http"] = null;
                        safeUrlDictionary["https"] = null;
                        safeUrlDictionary["ftp"] = null;
                        safeUrlDictionary["file"] = null;
                        safeUrlDictionary["mailto"] = null;
                        safeUrlDictionary["news"] = null;
                        safeUrlDictionary["gopher"] = null;
                        safeUrlDictionary["about"] = null;
                        safeUrlDictionary["wais"] = null;
                        safeUrlDictionary["cid"] = null;
                        safeUrlDictionary["mhtml"] = null;
                        safeUrlDictionary["ipp"] = null;
                        safeUrlDictionary["msdaipp"] = null;
                        safeUrlDictionary["meet"] = null;
                        safeUrlDictionary["tel"] = null;
                        safeUrlDictionary["sip"] = null;
                        safeUrlDictionary["conf"] = null;
                        safeUrlDictionary["im"] = null;
                        safeUrlDictionary["callto"] = null;
                        safeUrlDictionary["notes"] = null;
                        safeUrlDictionary["onenote"] = null;
                        safeUrlDictionary["groove"] = null;
                        safeUrlDictionary["mms"] = null;
                    }

                    textConvertersConfigured = true;
                }
            }
        }

        
        private static bool SafeUrlSchema(char[] urlBuffer, int schemaLength)
        {
            if (schemaLength < 2 || schemaLength > 20)
            {
                return false;
            }

            if (!textConvertersConfigured)
            {
                ConfigureTextConverters();
            }

            

            if (safeUrlDictionary.ContainsKey(new string(urlBuffer, 0, schemaLength)))
            {
                return true;
            }

            return false;
        }

        
        private enum CheckUrlResult
        {
            Inconclusive,
            Unsafe,
            Safe,
            LocalHyperlink,
        }

        
        private static CheckUrlResult CheckUrl(char[] urlBuffer, int urlLength, bool callbackRequested)
        {
            
            

            int i;

            if (urlLength > 0 && urlBuffer[0] == '#')
            {
                
                return CheckUrlResult.LocalHyperlink;
            }

            for (i = 0; i < urlLength; i++)
            {
                if (urlBuffer[i] == '/' || urlBuffer[i] == '\\')
                {
                    if (i == 0 && urlLength > 1 && (urlBuffer[1] == '/' || urlBuffer[1] == '\\'))
                    {
                        
                        return callbackRequested ? CheckUrlResult.Safe : CheckUrlResult.Unsafe;
                    }

                    
                    return CheckUrlResult.Safe;
                }
                if (urlBuffer[i] == '?' || urlBuffer[i] == '#' || urlBuffer[i] == ';')
                {
                    
                    return CheckUrlResult.Safe;
                }
                if (urlBuffer[i] == ':')
                {
                    if (SafeUrlSchema(urlBuffer, i))
                    {
                        return CheckUrlResult.Safe;
                    }

                    if (callbackRequested)
                    {
                        if (i == 1 &&
                            urlLength > 2 &&
                            ParseSupport.AlphaCharacter(ParseSupport.GetCharClass(urlBuffer[0])) &&
                            (urlBuffer[2] == '/' || urlBuffer[2] == '\\'))
                        {
                            
                            return CheckUrlResult.Safe;
                        }
                        else
                        {
                            var url = new BufferString(urlBuffer, 0, urlLength);

                            if (url.EqualsToLowerCaseStringIgnoreCase("objattph://") ||
                                url.EqualsToLowerCaseStringIgnoreCase("rtfimage://"))
                            {
                                return CheckUrlResult.Safe;
                            }
                        }
                    }

                    
                    return CheckUrlResult.Unsafe;
                }
            }

            InternalDebug.Assert(i == urlLength);

            
            
            return CheckUrlResult.Inconclusive;
        }

        
        internal static bool IsUrlSafe(string url, bool callbackRequested)
        {
            
            var urlBuffer = url.ToCharArray();

            switch (CheckUrl(urlBuffer, urlBuffer.Length, callbackRequested))
            {
                case CheckUrlResult.Safe:
                case CheckUrlResult.Inconclusive:
                case CheckUrlResult.LocalHyperlink:

                    return true;
            }

            return false;
        }

        private int AllocateVirtualEntry(int index, int offset, int length)
        {
            if (attributeVirtualList == null)
            {
                attributeVirtualList = new AttributeVirtualEntry[4];
            }
            else if (attributeVirtualList.Length == attributeVirtualCount)
            {
                var newVirtualList = new AttributeVirtualEntry[attributeVirtualList.Length * 2];
                Array.Copy(attributeVirtualList, 0, newVirtualList, 0, attributeVirtualCount);
                attributeVirtualList = newVirtualList;
            }

            var vi = attributeVirtualCount++;

            
            attributeVirtualList[vi].index = (short)index;
            attributeVirtualList[vi].offset = offset;
            attributeVirtualList[vi].length = length;
            attributeVirtualList[vi].position = 0;

            return vi;
        }

        
        private void VirtualizeFilteredStyle(int index)
        {
            
            var offset = attributeVirtualScratch.Length;
            FlushCssInStyleAttributeToVirtualScratch();
            var length = attributeVirtualScratch.Length - offset;

            
            var vi = AllocateVirtualEntry(attributeIndirectIndex[index + attributeSkipCount].index, offset, length);

            
            attributeIndirectIndex[index + attributeSkipCount].index = (short)vi;
            attributeIndirectIndex[index + attributeSkipCount].kind = AttributeIndirectKind.VirtualFilteredStyle;
        }

        
        private bool InjectMetaTagIfNecessary()
        {
            

            if (filterForFragment || !writer.HasEncoding)
            {
                
                metaInjected = true;
            }
            else
            {
                if (token.TokenId != HtmlTokenId.Restart && token.TokenId != HtmlTokenId.EncodingChange)
                {
                    if (writer.Encoding.CodePage == 65000)
                    {
                        

                        OutputMetaTag();
                        metaInjected = true;
                    }
                    else if (token.TokenId == HtmlTokenId.Tag)
                    {
                        if (!insideHtml && token.TagIndex == HtmlTagIndex.Html)
                        {
                            if (token.IsTagEnd)
                            {
                                insideHtml = true;
                            }
                        }
                        else if (!insideHead && token.TagIndex == HtmlTagIndex.Head)
                        {
                            if (token.IsTagEnd)
                            {
                                insideHead = true;
                            }
                        }
                        else if (token.TagIndex <= HtmlTagIndex._ASP)
                        {
                            
                        }
                        else
                        {
                            
                            InternalDebug.Assert(token.IsTagBegin);

                            if (insideHtml && !insideHead)
                            {
                                writer.WriteNewLine(true);
                                writer.WriteStartTag(HtmlNameIndex.Head);
                                writer.WriteNewLine(true);

                                OutputMetaTag();

                                writer.WriteEndTag(HtmlNameIndex.Head);
                                writer.WriteNewLine(true);
                            }
                            else
                            {
                                if (insideHead)
                                {
                                    writer.WriteNewLine(true);
                                }
                                OutputMetaTag();
                            }

                            metaInjected = true;
                        }
                    }
                    else if (token.TokenId == HtmlTokenId.Text)
                    {
                        if (token.IsWhitespaceOnly)
                        {
                            
                            return false;
                        }

                        

                        
                        token.Text.StripLeadingWhitespace();

                        if (insideHtml && !insideHead)
                        {
                            writer.WriteNewLine(true);
                            writer.WriteStartTag(HtmlNameIndex.Head);
                            writer.WriteNewLine(true);

                            OutputMetaTag();

                            writer.WriteEndTag(HtmlNameIndex.Head);
                            writer.WriteNewLine(true);
                        }
                        else
                        {
                            if (insideHead)
                            {
                                writer.WriteNewLine(true);
                            }

                            OutputMetaTag();
                        }

                        metaInjected = true;
                    }
                }
            }

            return true;
        }

        
        private void OutputMetaTag()
        {
            InternalDebug.Assert(CopyPendingStateFlag == CopyPendingState.NotPending);
            InternalDebug.Assert(!filterForFragment);

            
            
            

            
            InternalDebug.Assert(writer.HasEncoding);

            var encoding = writer.Encoding;

            if (encoding.CodePage == 65000)
            {
                

                writer.Encoding = Encoding.ASCII;
            }

            writer.WriteStartTag(HtmlNameIndex.Meta);
            writer.WriteAttribute(HtmlNameIndex.HttpEquiv, "Content-Type");
            writer.WriteAttributeName(HtmlNameIndex.Content);
            writer.WriteAttributeValueInternal("text/html; charset=");
            writer.WriteAttributeValueInternal(Charset.GetCharset(encoding.CodePage).Name);

            if (encoding.CodePage == 65000)
            {
                writer.WriteTagEnd();
                writer.Encoding = encoding;
            }

            
        }

        
        private AttributeIndirectKind GetAttributeIndirectKind(int index)
        {
            InternalDebug.Assert(index >= 0 && index < attributeCount);

            return attributeIndirect ? attributeIndirectIndex[index + attributeSkipCount].kind : AttributeIndirectKind.PassThrough;
        }

        
        private int GetAttributeVirtualEntryIndex(int index)
        {
            InternalDebug.Assert(index >= 0 && index < attributeCount);
            InternalDebug.Assert(attributeIndirect);
            InternalDebug.Assert(GetAttributeIndirectKind(index) == AttributeIndirectKind.Virtual ||
                                GetAttributeIndirectKind(index) == AttributeIndirectKind.VirtualFilteredStyle);

            return attributeIndirectIndex[index + attributeSkipCount].index;
        }

        
        private HtmlAttribute GetAttribute(int index)
        {
            InternalDebug.Assert(index >= 0 && index < attributeCount);
            
            InternalDebug.Assert(GetAttributeIndirectKind(index) != AttributeIndirectKind.FilteredStyle &&
                                GetAttributeIndirectKind(index) != AttributeIndirectKind.VirtualFilteredStyle);

            if (!attributeIndirect)
            {
                
                return token.Attributes[index + attributeSkipCount];
            }
            else if (attributeIndirectIndex[index + attributeSkipCount].kind != AttributeIndirectKind.Virtual)
            {
                return token.Attributes[attributeIndirectIndex[index + attributeSkipCount].index];
            }
            else
            {
                
                return token.Attributes[attributeVirtualList[attributeIndirectIndex[index + attributeSkipCount].index].index];
            }
        }

        
        internal HtmlAttributeId GetAttributeNameId(int index)
        {
            var kind = GetAttributeIndirectKind(index);

            if (kind == AttributeIndirectKind.FilteredStyle || kind == AttributeIndirectKind.VirtualFilteredStyle)
            {
                
                return HtmlAttributeId.Style;
            }

            

            var attribute = GetAttribute(index);
            return HtmlNameData.names[(int)attribute.NameIndex].publicAttributeId;
        }

        private static readonly HtmlAttributeParts CompleteAttributeParts = new HtmlAttributeParts(HtmlToken.AttrPartMajor.Complete, HtmlToken.AttrPartMinor.CompleteNameWithCompleteValue);

        
        internal HtmlAttributeParts GetAttributeParts(int index)
        {
            var kind = GetAttributeIndirectKind(index);

            if (kind == AttributeIndirectKind.FilteredStyle || kind == AttributeIndirectKind.VirtualFilteredStyle)
            {
                
                return CompleteAttributeParts;
            }

            var attribute = GetAttribute(index);

            if (kind == AttributeIndirectKind.NameOnlyFragment)
            {
                

                
                InternalDebug.Assert((attribute.MajorPart & HtmlToken.AttrPartMajor.End) != HtmlToken.AttrPartMajor.End);

                
                InternalDebug.Assert((attribute.MinorPart & HtmlToken.AttrPartMinor.ContinueName) != 0);

                return new HtmlAttributeParts(attribute.MajorPart, attribute.MinorPart & ~HtmlToken.AttrPartMinor.CompleteValue);
            }
            else if (kind == AttributeIndirectKind.EmptyValue || kind == AttributeIndirectKind.Virtual)
            {
                
                return new HtmlAttributeParts(attribute.MajorPart | HtmlToken.AttrPartMajor.End, attribute.MinorPart | HtmlToken.AttrPartMinor.CompleteValue);
            }

            return new HtmlAttributeParts(attribute.MajorPart, attribute.MinorPart);
        }

        
        internal string GetAttributeName(int index)
        {
            var kind = GetAttributeIndirectKind(index);

            if (kind == AttributeIndirectKind.FilteredStyle || kind == AttributeIndirectKind.VirtualFilteredStyle)
            {
                
                InternalDebug.Assert(tagHasFilteredStyleAttribute);

                return HtmlNameData.names[(int)HtmlNameIndex.Style].name;
            }

            

            var attribute = GetAttribute(index);

            if (attribute.NameIndex > HtmlNameIndex.Unknown)
            {
                return attribute.IsAttrBegin ? HtmlNameData.names[(int)attribute.NameIndex].name : String.Empty;
            }
            else if (attribute.HasNameFragment)
            {
                return attribute.Name.GetString(int.MaxValue);
            }
            else
            {
                return attribute.IsAttrBegin ? "?" : String.Empty;
            }
        }

        

        
        internal string GetAttributeValue(int index)
        {
            var kind = GetAttributeIndirectKind(index);

            if (kind != AttributeIndirectKind.PassThrough)
            {
                if (kind == AttributeIndirectKind.FilteredStyle)
                {
                    
                    VirtualizeFilteredStyle(index);
                    kind = AttributeIndirectKind.VirtualFilteredStyle;
                }

                if (kind == AttributeIndirectKind.Virtual || kind == AttributeIndirectKind.VirtualFilteredStyle)
                {
                    
                    var vi = GetAttributeVirtualEntryIndex(index);
                    if (attributeVirtualList[vi].length != 0)
                    {
                        return new string(attributeVirtualScratch.Buffer, attributeVirtualList[vi].offset, attributeVirtualList[vi].length);
                    }

                    return String.Empty;
                }

                InternalDebug.Assert(kind == AttributeIndirectKind.EmptyValue || kind == AttributeIndirectKind.NameOnlyFragment);
                return String.Empty;
            }

            var attribute = GetAttribute(index);

            if (!attribute.HasValueFragment)
            {
                return String.Empty;
            }

            return attribute.Value.GetString(int.MaxValue);
        }

        
        internal int ReadAttributeValue(int index, char[] buffer, int offset, int count)
        {
            var kind = GetAttributeIndirectKind(index);

            if (kind != AttributeIndirectKind.PassThrough)
            {
                if (kind == AttributeIndirectKind.FilteredStyle)
                {
                    
                    VirtualizeFilteredStyle(index);
                    kind = AttributeIndirectKind.VirtualFilteredStyle;
                }

                if (kind == AttributeIndirectKind.Virtual || kind == AttributeIndirectKind.VirtualFilteredStyle)
                {
                    
                    var vi = GetAttributeVirtualEntryIndex(index);

                    InternalDebug.Assert(attributeVirtualList[vi].position < attributeVirtualList[vi].length);

                    var countRead = Math.Min(attributeVirtualList[vi].length - attributeVirtualList[vi].position, count);
                    if (countRead != 0)
                    {
                        Buffer.BlockCopy(attributeVirtualScratch.Buffer, 2 * (attributeVirtualList[vi].offset + attributeVirtualList[vi].position), buffer, offset, 2 * countRead);
                        attributeVirtualList[vi].position += countRead;
                    }

                    return countRead;
                }

                InternalDebug.Assert(kind == AttributeIndirectKind.EmptyValue || kind == AttributeIndirectKind.NameOnlyFragment);
                return 0;
            }

            var attribute = GetAttribute(index);

            if (!attribute.HasValueFragment)
            {
                return 0;
            }

            return attribute.Value.Read(buffer, offset, count);
        }

        
        internal void WriteTag(bool copyTagAttributes)
        {
            CopyInputTag(copyTagAttributes);
        }

        
        internal void WriteAttribute(int index, bool writeName, bool writeValue)
        {
            if (writeName)
            {
                if (writeValue)
                {
                    CopyInputAttribute(index);
                }
                else
                {
                    CopyInputAttributeName(index);
                }
            }
            else if (writeValue)
            {
                CopyInputAttributeValue(index);
            }
        }

        
        private void AppendCssFromTokenText()
        {
            if (null == cssParserInput)
            {
                cssParserInput = new ConverterBufferInput(CssParser.MaxCssLength, progressMonitor);
                cssParser = new CssParser(cssParserInput, 4 * 1024, false);
            }

            token.Text.WriteTo(cssParserInput);
        }

        
        private void AppendCss(string css)
        {
            if (null == cssParserInput)
            {
                cssParserInput = new ConverterBufferInput(CssParser.MaxCssLength, progressMonitor);
                cssParser = new CssParser(cssParserInput, 4 * 1024, false);
            }

            cssParserInput.Write(css);
        }

        
        private void AppendCssFromAttribute(HtmlAttribute attribute)
        {
            if (null == cssParserInput)
            {
                cssParserInput = new ConverterBufferInput(CssParser.MaxCssLength, progressMonitor);
                cssParser = new CssParser(cssParserInput, 4 * 1024, false);
            }

            attribute.Value.Rewind();
            attribute.Value.WriteTo(cssParserInput);
        }

        
        private void FlushCssInStyleTag()
        {
            if (null != cssParserInput)
            {
                writer.WriteNewLine();
                writer.WriteMarkupText("<!--");
                writer.WriteNewLine();

                var agressiveFiltering = false;

                if (smallCssBlockThreshold != -1 && cssParserInput.MaxTokenSize > smallCssBlockThreshold)
                {
                    agressiveFiltering = true;
                }

                
                cssParser.SetParseMode(CssParseMode.StyleTag);

                CssTokenId tokenId;
                var firstProperty = true;

                var sink = writer.WriteText();

                do
                {
                    tokenId = cssParser.Parse();

                    if ((CssTokenId.RuleSet == tokenId || CssTokenId.AtRule == tokenId) &&
                        cssParser.Token.Selectors.ValidCount != 0 &&
                        cssParser.Token.Properties.ValidCount != 0)
                    {
                        var selectorOk = CopyInputCssSelectors(cssParser.Token.Selectors, sink, agressiveFiltering);

                        if (selectorOk)
                        {
                            if (cssParser.Token.IsPropertyListBegin)
                            {
                                sink.Write("\r\n\t{");
                            }

                            CopyInputCssProperties(true, cssParser.Token.Properties, sink, ref firstProperty);

                            if (cssParser.Token.IsPropertyListEnd)
                            {
                                sink.Write("}\r\n");
                                firstProperty = true;
                            }
                        }
                    }
                }
                while (CssTokenId.EndOfFile != tokenId);

                cssParserInput.Reset();
                cssParser.Reset();

                writer.WriteMarkupText("-->");
                writer.WriteNewLine();
            }
        }

        
        private void FlushCssInStyleAttributeToVirtualScratch()
        {
            
            cssParser.SetParseMode(CssParseMode.StyleAttribute);

            if (virtualScratchSink == null)
            {
                virtualScratchSink = new VirtualScratchSink(this, int.MaxValue);
            }

            CssTokenId tokenId;
            var firstProperty = true;
            do
            {
                tokenId = cssParser.Parse();

                if (CssTokenId.Declarations == tokenId && cssParser.Token.Properties.ValidCount != 0)
                {
                    CopyInputCssProperties(false, cssParser.Token.Properties, virtualScratchSink, ref firstProperty);
                }
            }
            while (CssTokenId.EndOfFile != tokenId);

            cssParserInput.Reset();
            cssParser.Reset();
        }

        
        private void FlushCssInStyleAttribute(HtmlWriter writer)
        {
            
            cssParser.SetParseMode(CssParseMode.StyleAttribute);

            var sink = writer.WriteAttributeValue();

            CssTokenId tokenId;
            var firstProperty = true;
            do
            {
                tokenId = cssParser.Parse();

                if (CssTokenId.Declarations == tokenId && cssParser.Token.Properties.ValidCount != 0)
                {
                    CopyInputCssProperties(false, cssParser.Token.Properties, sink, ref firstProperty);
                }
            }
            while (CssTokenId.EndOfFile != tokenId);

            cssParserInput.Reset();
            cssParser.Reset();
        }

        
        private bool CopyInputCssSelectors(CssToken.SelectorEnumerator selectors, ITextSinkEx sink, bool agressiveFiltering)
        {
            var atLeastOneOk = false;
            var lastOk = false;

            
            selectors.Rewind();

            foreach (var selector in selectors)
            {
                if (!selector.IsDeleted)
                {
                    if (lastOk)
                    {
                        if (selector.Combinator == CssSelectorCombinator.None)
                        {
                            sink.Write(", ");
                        }
                        else if (selector.Combinator == CssSelectorCombinator.Descendant)
                        {
                            InternalDebug.Assert(!selector.IsSimple);
                            sink.Write(' ');
                        }
                        else if (selector.Combinator == CssSelectorCombinator.Adjacent)
                        {
                            InternalDebug.Assert(!selector.IsSimple);
                            sink.Write(" + ");
                        }
                        else
                        {
                            InternalDebug.Assert(!selector.IsSimple);
                            InternalDebug.Assert(selector.Combinator == CssSelectorCombinator.Child);
                            sink.Write(" > ");
                        }
                    }

                    lastOk = CopyInputCssSelector(selector, sink, agressiveFiltering);
                    atLeastOneOk = atLeastOneOk || lastOk;
                }
            }

            return atLeastOneOk;
        }

        
        private bool CopyInputCssSelector(CssSelector selector, ITextSinkEx sink, bool agressiveFiltering)
        {
            InternalDebug.Assert(!selector.IsDeleted);

            if (filterForFragment)
            {
                if (!selector.HasClassFragment ||
                    (selector.ClassType != CssSelectorClassType.Regular &&
                     selector.ClassType != CssSelectorClassType.Hash))
                {
                    
                    return false;
                }
#if false
                string className = selector.ClassName.GetString(256);

                if (!className.StartsWith("Mso", StringComparison.Ordinal) &&
                    !className.StartsWith("NL", StringComparison.Ordinal) &&
                    !className.StartsWith("email", StringComparison.OrdinalIgnoreCase) &&
                    !className.StartsWith("outlook", StringComparison.OrdinalIgnoreCase))
                {
                    
                    return false;
                }
#endif
            }

            if (agressiveFiltering)
            {
                

                if (!selector.HasClassFragment || selector.ClassType != CssSelectorClassType.Regular)
                {
                    
                    return false;
                }

                
                var className = selector.ClassName.GetString(256);

                
                InternalDebug.Assert(className.Length > 0);

                if (!className.Equals("MsoNormal", StringComparison.Ordinal))
                {
                    return false;
                }
            }

            if (selector.NameId != HtmlNameIndex.Unknown && selector.NameId != HtmlNameIndex._NOTANAME)
            {
                sink.Write(HtmlNameData.names[(int)selector.NameId].name);
            }
            else if (selector.HasNameFragment)
            {
                
                selector.Name.WriteOriginalTo(sink);
            }

            if (selector.HasClassFragment)
            {
                if (selector.ClassType == CssSelectorClassType.Regular)
                {
                    sink.Write(".");
                }
                else if (selector.ClassType == CssSelectorClassType.Hash)
                {
                    sink.Write("#");
                }
                else if (selector.ClassType == CssSelectorClassType.Pseudo)
                {
                    sink.Write(":");
                }
                else
                {
                    
                    InternalDebug.Assert(selector.ClassType == CssSelectorClassType.Attrib);
                }

                if (outputFragment)
                {
                    InternalDebug.Assert(selector.ClassType == CssSelectorClassType.Hash || selector.ClassType == CssSelectorClassType.Regular);

                    sink.Write(NamePrefix);
                }

                
                selector.ClassName.WriteOriginalTo(sink);
            }

            return true;
        }

        
        private void CopyInputCssProperties(bool inTag, CssToken.PropertyEnumerator properties, ITextSinkEx sink, ref bool firstProperty)
        {
            

            properties.Rewind();

            foreach (var property in properties)
            {
                if (property.IsPropertyBegin && !property.IsDeleted)
                {
                    var action = CssData.filterInstructions[(int)property.NameId].propertyAction;

                    if (CssData.FilterAction.CheckContent == action)
                    {
                        if (property.NameId == CssNameIndex.Display &&
                            property.HasValueFragment &&
                            property.Value.CaseInsensitiveContainsSubstring("none") &&
                            !preserveDisplayNoneStyle)
                        {
                            action = CssData.FilterAction.Drop;
                        }
                        else if (property.NameId == CssNameIndex.Position &&
                            property.HasValueFragment &&
                            (property.Value.CaseInsensitiveContainsSubstring("absolute") ||
                              property.Value.CaseInsensitiveContainsSubstring("relative")) &&
                            outputFragment)
                        {
                            action = CssData.FilterAction.Drop;
                        }
                        else
                        {
                            action = CssData.FilterAction.Keep;
                        }
                    }

                    if (CssData.FilterAction.Keep == action)
                    {
                        if (firstProperty)
                        {
                            firstProperty = false;
                        }
                        else
                        {
                            sink.Write(inTag ? ";\r\n\t" : "; ");
                        }

                        CopyInputCssProperty(property, sink);
                    }
                }
            }
        }

        
        private static void CopyInputCssProperty(CssProperty property, ITextSinkEx sink)
        {
            InternalDebug.Assert(!property.IsDeleted);

            if (property.IsPropertyBegin)
            {
                if (property.NameId != CssNameIndex.Unknown)
                {
                    sink.Write(CssData.names[(int)property.NameId].name);
                }
            }

            if (property.NameId == CssNameIndex.Unknown && property.HasNameFragment)
            {
                
                property.Name.WriteOriginalTo(sink);
            }

            if (property.IsPropertyNameEnd)
            {
                sink.Write(":");
            }

            if (property.HasValueFragment)
            {
                
                property.Value.WriteEscapedOriginalTo(sink);
            }
        }

        

        internal class VirtualScratchSink : ITextSinkEx
        {
            private HtmlToHtmlConverter converter;
            private int maxLength;

            public VirtualScratchSink(HtmlToHtmlConverter converter, int maxLength)
            {
                this.converter = converter;
                this.maxLength = maxLength;
            }

            public bool IsEnough => converter.attributeVirtualScratch.Length >= maxLength;

            public void Write(char[] buffer, int offset, int count)
            {
                InternalDebug.Assert(!IsEnough);
                converter.attributeVirtualScratch.Append(buffer, offset, count, maxLength);
            }

            public void Write(int ucs32Char)
            {
                InternalDebug.Assert(!IsEnough);

                if (Token.LiteralLength(ucs32Char) == 1)
                {
                    converter.attributeVirtualScratch.Append((char)ucs32Char, maxLength);
                }
                else
                {
                    converter.attributeVirtualScratch.Append(Token.LiteralFirstChar(ucs32Char), maxLength);
                    if (!IsEnough)
                    {
                        converter.attributeVirtualScratch.Append(Token.LiteralLastChar(ucs32Char), maxLength);
                    }
                }
            }

            public void Write(string value)
            {
                InternalDebug.Assert(!IsEnough);

                
                converter.attributeVirtualScratch.Append(value, maxLength);
            }

            public void WriteNewLine()
            {
                InternalDebug.Assert(!IsEnough);

                converter.attributeVirtualScratch.Append('\r', maxLength);

                if (!IsEnough)
                {
                    converter.attributeVirtualScratch.Append('\n', maxLength);
                }
            }
        }
    }

    
    
    

    internal class HtmlToHtmlTagContext : HtmlTagContext
    {
        private HtmlToHtmlConverter converter;

        public HtmlToHtmlTagContext(HtmlToHtmlConverter converter)
        {
            this.converter = converter;
        }

        
        

        internal override string GetTagNameImpl()
        {
            if (TagNameIndex > HtmlNameIndex.Unknown)
            {
                return TagParts.Begin ? HtmlNameData.names[(int)TagNameIndex].name : String.Empty;
            }
            else if (TagParts.Name)
            {
                return converter.token.Name.GetString(int.MaxValue);
            }
            else
            {
                return String.Empty;
            }
        }

        internal override HtmlAttributeId GetAttributeNameIdImpl(int attributeIndex)
        {
            return converter.GetAttributeNameId(attributeIndex);
        }

        internal override HtmlAttributeParts GetAttributePartsImpl(int attributeIndex)
        {
            return converter.GetAttributeParts(attributeIndex);
        }

        internal override string GetAttributeNameImpl(int attributeIndex)
        {
            return converter.GetAttributeName(attributeIndex);
        }

        

        internal override string GetAttributeValueImpl(int attributeIndex)
        {
            return converter.GetAttributeValue(attributeIndex);
        }

        internal override int ReadAttributeValueImpl(int attributeIndex, char[] buffer, int offset, int count)
        {
            return converter.ReadAttributeValue(attributeIndex, buffer, offset, count);
        }

        internal override void WriteTagImpl(bool copyTagAttributes)
        {
            converter.WriteTag(copyTagAttributes);
        }

        internal override void WriteAttributeImpl(int attributeIndex, bool writeName, bool writeValue)
        {
            converter.WriteAttribute(attributeIndex, writeName, writeValue);
        }
    }
}
