// ***************************************************************
// <copyright file="HtmlNormalizingParser.cs" company="Microsoft">
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
    using Data.Internal;
    
    using Strings = CtsResources.TextConvertersStrings;

    
    internal class HtmlNormalizingParser : IHtmlParser, IRestartable, IReusable, IDisposable
    {
        

        private HtmlParser parser;
        private IRestartable restartConsumer;

        private int maxElementStack;

        private Context context;                  
        private Context[] contextStack;           
        private int contextStackTop;

        private HtmlTagIndex[] elementStack;
        private int elementStackTop;

        private QueueItem[] queue;
        private int queueHead;
        private int queueTail;
        
        
        private int queueStart;

        private bool ensureHead = true;

        
        private int[] closeList;
        private HtmlTagIndex[] openList;

        private bool validRTC;
        private HtmlTagIndex tagIdRTC;

        
        private HtmlToken token;

        
        private HtmlToken inputToken;
        private bool ignoreInputTag;     

        
        private int currentRun;
        private int currentRunOffset;
        private int numRuns;

        private bool allowWspLeft;
        private bool allowWspRight;

        private SmallTokenBuilder tokenBuilder;

        
        private HtmlInjection injection;
        private DocumentState saveState;

        
        public HtmlNormalizingParser(
                    HtmlParser parser,
                    HtmlInjection injection,
                    bool ensureHead,
                    int maxNesting,
                    bool testBoundaryConditions,
                    Stream traceStream,
                    bool traceShowTokenNum,
                    int traceStopOnTokenNum)
        {
            this.parser = parser;
            this.parser.SetRestartConsumer(this);

            this.injection = injection;
            if (injection != null)
            {
                saveState = new DocumentState();
            }

            this.ensureHead = ensureHead;

            var initialStackSize = testBoundaryConditions ? 1 : 32;
            maxElementStack = testBoundaryConditions ? 30 : maxNesting;

            openList = new HtmlTagIndex[8];
            closeList = new int[8];

            elementStack = new HtmlTagIndex[initialStackSize];
            

            contextStack = new Context[testBoundaryConditions ? 1 : 4];
            

            

            elementStack[elementStackTop++] = HtmlTagIndex._ROOT;

            
            context.type = HtmlDtd.ContextType.Root;
            context.textType = HtmlDtd.ContextTextType.Full;
            
            context.reject = HtmlDtd.SetId.Empty;
            
            
            
            
            

            queue = new QueueItem[testBoundaryConditions ? 1 : initialStackSize / 4];

            tokenBuilder = new SmallTokenBuilder();
        }

        
        private void Reinitialize()
        {
            elementStackTop = 0;

            contextStackTop = 0;

            ignoreInputTag = false;

            elementStack[elementStackTop++] = HtmlTagIndex._ROOT;

            context.topElement = 0;
            context.type = HtmlDtd.ContextType.Root;
            context.textType = HtmlDtd.ContextTextType.Full;
            context.accept = HtmlDtd.SetId.Null;
            context.reject = HtmlDtd.SetId.Empty;
            context.ignoreEnd = HtmlDtd.SetId.Null;
            context.hasSpace = false;
            context.eatSpace = false;
            context.oneNL = false;
            context.lastCh = '\0';

            queueHead = 0;
            queueTail = 0;
            queueStart = 0;

            validRTC = false;
            tagIdRTC = HtmlTagIndex._NULL;

            token = null;

            if (injection != null)
            {
                if (injection.Active)
                {
                    parser = (HtmlParser) injection.Pop();
                }

                injection.Reset();
            }
        }

        
        private enum QueueItemKind : byte
        {
            Empty,
            None,
            Eof,
            BeginElement,
            EndElement,
            OverlappedClose,
            OverlappedReopen,
            PassThrough,
            Space,
            Text,
            Suspend,
            InjectionBegin,
            InjectionEnd,
            
            EndLastTag,
        }

        
        [Flags]
        private enum QueueItemFlags : byte
        {
            AllowWspLeft = 0x01,
            AllowWspRight = 0x02,
        }

        
        public HtmlToken Token => token;


        public void SetRestartConsumer(IRestartable restartConsumer)
        {
            this.restartConsumer = restartConsumer;
        }

        
        public HtmlTokenId Parse()
        {
            while (true)
            {
                if (!QueueEmpty())
                {
                    return GetTokenFromQueue();
                }

                Process(parser.Parse());
            }
        }

        
        bool IRestartable.CanRestart()
        {
            return restartConsumer != null && restartConsumer.CanRestart();
        }

        
        void IRestartable.Restart()
        {
            InternalDebug.Assert(((IRestartable)this).CanRestart());

            if (restartConsumer != null)
            {
                restartConsumer.Restart();
            }

            Reinitialize();
        }

        
        void IRestartable.DisableRestart()
        {
            if (restartConsumer != null)
            {
                restartConsumer.DisableRestart();
            }
        }

        
        void IReusable.Initialize(object newSourceOrDestination)
        {
            InternalDebug.Assert(parser is IReusable);

            ((IReusable)parser).Initialize(newSourceOrDestination);

            Reinitialize();

            parser.SetRestartConsumer(this);
        }

        
        public void Initialize(string fragment, bool preformatedText)
        {
            parser.Initialize(fragment, preformatedText);

            Reinitialize();
        }

        
        void IDisposable.Dispose()
        {
            if (parser != null /*&& this.parser is IDisposable*/)
            {
                ((IDisposable)parser).Dispose();
            }

            parser = null;
            restartConsumer = null;
            contextStack = null;   
            queue = null;
            closeList = null;
            openList = null;
            token = null;
            inputToken = null;
            tokenBuilder = null;

            GC.SuppressFinalize(this);
        }

        
        private static HtmlDtd.TagDefinition GetTagDefinition(HtmlTagIndex tagIndex)
        {
            return HtmlDtd.tags[(int)tagIndex];
        }

        
        private void Process(HtmlTokenId tokenId)
        {
            if (tokenId == HtmlTokenId.None)
            {
                InternalDebug.Assert(QueueEmpty());

                EnqueueHead(QueueItemKind.None);
                return;
            }

            InternalDebug.Assert(queueHead == queueTail || queue[queueHead].kind != QueueItemKind.Suspend || tokenId == HtmlTokenId.Tag || tokenId == HtmlTokenId.EndOfFile);

            inputToken = parser.Token;

            switch (tokenId)
            {
                case HtmlTokenId.Restart:

                    EnqueueTail(QueueItemKind.PassThrough);
                    break;

                case HtmlTokenId.EncodingChange:

                    EnqueueTail(QueueItemKind.PassThrough);
                    break;

                case HtmlTokenId.EndOfFile:

                    HandleTokenEof();
                    break;

                case HtmlTokenId.Tag:

                    InternalDebug.Assert(queue[queueHead].kind != QueueItemKind.Suspend || !inputToken.IsTagBegin);

                    if (parser.Token.NameIndex < HtmlNameIndex.Unknown)
                    {
                        HandleTokenSpecialTag(parser.Token);
                        break;
                    }

                    HandleTokenTag(parser.Token);
                    break;

                case HtmlTokenId.Text:

                    HandleTokenText(parser.Token);
                    break;

                default:

                    
                    InternalDebug.Assert(false, "unexpected HTML token");
                    break;
            }
        }

        
        private void HandleTokenEof()
        {
            
            InternalDebug.Assert(QueueEmpty());

            if (queueHead != queueTail && queue[queueHead].kind == QueueItemKind.Suspend)
            {
                
                

                var qi = DoDequeueFirst();
                EnqueueHead(
                            QueueItemKind.EndLastTag, 
                            qi.tagIndex, 
                            0 != (qi.flags & QueueItemFlags.AllowWspLeft),
                            0 != (qi.flags & QueueItemFlags.AllowWspRight));
                return;
            }

            if (injection != null && injection.Active)
            {
                

                
                CloseAllContainers(saveState.SavedStackTop);

                if (queueHead != queueTail)
                {
                    
                    return;
                }

                
                saveState.Restore(this);
                EnqueueHead(QueueItemKind.InjectionEnd, injection.InjectingHead ? 1 : 0);
                parser = (HtmlParser) injection.Pop();
                return;
            }

            

            
            
            
            
            

            if (injection != null)
            {
                var bodyLevel = FindContainer(HtmlTagIndex.Body, HtmlDtd.SetId.Empty);

                if (bodyLevel == -1)
                {
                    

                    
                    

                    
                    CloseAllProhibitedContainers(GetTagDefinition(HtmlTagIndex.Body));

                    OpenContainer(HtmlTagIndex.Body);
                    return;
                }

                
                CloseAllContainers(bodyLevel + 1);

                if (queueHead != queueTail)
                {
                    
                    return;
                }

                if (injection.HaveTail && !injection.TailDone)
                {
                    

                    parser = (HtmlParser) injection.Push(false, parser);
                    saveState.Save(this, elementStackTop);
                    EnqueueTail(QueueItemKind.InjectionBegin, 0);
                    if (injection.HeaderFooterFormat == HeaderFooterFormat.Text)
                    {
                        OpenContainer(HtmlTagIndex.TT);
                        OpenContainer(HtmlTagIndex.Pre);
                    }
                    return;
                }
            }

            

            CloseAllContainers();

            

            EnqueueTail(QueueItemKind.Eof);
        }

        
        private void HandleTokenTag(HtmlToken tag)
        {
            HtmlTagIndex tagIndex;

            InternalDebug.Assert(HtmlNameIndex.Unknown <= tag.NameIndex && (int)tag.NameIndex < HtmlNameData.names.Length - 1);

            tagIndex = HtmlNameData.names[(int)tag.NameIndex].tagIndex;

            if (tag.IsTagBegin)
            {
                StartTagProcessing(tagIndex, tag);
            }
            else
            {
                

                if (!ignoreInputTag)
                {
                    InternalDebug.Assert(QueueHeadKind() == QueueItemKind.Suspend);

                    if (tag.IsTagEnd)
                    {
                        

                        DoDequeueFirst();
                    }

                    if (inputToken.TagIndex != HtmlTagIndex.Unknown)
                    {
                        var emptyScope = (GetTagDefinition(tagIndex).scope == HtmlDtd.TagScope.EMPTY);

                        
                        inputToken.Flags = emptyScope ? inputToken.Flags | HtmlToken.TagFlags.EmptyScope : inputToken.Flags & ~HtmlToken.TagFlags.EmptyScope;
                    }

                    EnqueueHead(QueueItemKind.PassThrough);
                }
                else
                {
                    if (tag.IsTagEnd)
                    {
                        ignoreInputTag = false;
                    }
                }
            }
        }

        
        private void HandleTokenSpecialTag(HtmlToken tag)
        {
            HtmlTagIndex tagIndex;

            InternalDebug.Assert(HtmlNameIndex._NOTANAME < tag.NameIndex && tag.NameIndex < HtmlNameIndex.Unknown);

            
            tag.Flags = tag.Flags | HtmlToken.TagFlags.EmptyScope;

            tagIndex = HtmlNameData.names[(int)tag.NameIndex].tagIndex;

            if (tag.IsTagBegin)
            {
                StartSpecialTagProcessing(tagIndex, tag);
            }
            else
            {
                

                if (!ignoreInputTag)
                {
                    InternalDebug.Assert(QueueHeadKind() == QueueItemKind.Suspend);

                    if (tag.IsTagEnd)
                    {
                        

                        DoDequeueFirst();
                    }

                    EnqueueHead(QueueItemKind.PassThrough);
                }
                else
                {
                    if (tag.IsTagEnd)
                    {
                        ignoreInputTag = false;
                    }
                }
            }
        }

        
        private void HandleTokenText(HtmlToken token)
        {
            var tagIdRTC = validRTC ? this.tagIdRTC : RequiredTextContainer();

            var cntWhitespaces = 0;

            var runs = inputToken.Runs;

            if (HtmlTagIndex._NULL != tagIdRTC)
            {
                
                while (runs.MoveNext(true) && (runs.Current.TextType <= RunTextType.LastWhitespace))
                {
                }

                if (!runs.IsValidPosition)
                {
                    
                    return;
                }

                CloseAllProhibitedContainers(GetTagDefinition(tagIdRTC));
                OpenContainer(tagIdRTC);
            }
            else if (context.textType != HtmlDtd.ContextTextType.Literal)
            {
                
                

                while (runs.MoveNext(true) && (runs.Current.TextType <= RunTextType.LastWhitespace))
                {
                    
                    
                    cntWhitespaces += runs.Current.TextType == RunTextType.NewLine ? 1 : 2;
                }
            }

            if (context.textType == HtmlDtd.ContextTextType.Literal)
            {
                

                EnqueueTail(QueueItemKind.PassThrough);
            }
            else if (context.textType == HtmlDtd.ContextTextType.Full)
            {
                if (cntWhitespaces != 0)
                {
                    
                    
                    AddSpace(cntWhitespaces == 1);
                }

                currentRun = runs.CurrentIndex;
                currentRunOffset = runs.CurrentOffset;

                if (runs.IsValidPosition)
                {
                    var firstChar = runs.Current.FirstChar;
                    char lastChar;
                    do
                    {
                        lastChar = runs.Current.LastChar;
                    }
                    while (runs.MoveNext(true) && !(runs.Current.TextType <= RunTextType.LastWhitespace));

                    
                    AddNonspace(firstChar, lastChar);
                }

                numRuns = runs.CurrentIndex - currentRun;
            }
        }

        
        private void StartTagProcessing(HtmlTagIndex tagIndex, HtmlToken tag)
        {
            InternalDebug.Assert(!ignoreInputTag);

            if ((HtmlDtd.SetId.Null != context.reject && !HtmlDtd.IsTagInSet(tagIndex, context.reject)) ||
                (HtmlDtd.SetId.Null != context.accept && HtmlDtd.IsTagInSet(tagIndex, context.accept)))
            {
                if (!tag.IsEndTag)
                {
                    if (!ProcessOpenTag(tagIndex, GetTagDefinition(tagIndex)))
                    {
                        ProcessIgnoredTag(tagIndex, tag);
                    }
                }
                else
                {
                    if (HtmlDtd.SetId.Null == context.ignoreEnd || !HtmlDtd.IsTagInSet(tagIndex, context.ignoreEnd)) 
                    {
                        if (!ProcessEndTag(tagIndex, GetTagDefinition(tagIndex)))
                        {
                            ProcessIgnoredTag(tagIndex, tag);
                        }
                    }
                    else
                    {
                        ProcessIgnoredTag(tagIndex, tag);
                    }
                }
            }
            else if (context.type == HtmlDtd.ContextType.Select && tagIndex == HtmlTagIndex.Select)
            {
                if (!ProcessEndTag(tagIndex, GetTagDefinition(tagIndex)))
                {
                    ProcessIgnoredTag(tagIndex, tag);
                }
            }
            else
            {
                ProcessIgnoredTag(tagIndex, tag);
            }
        }

        
        private void StartSpecialTagProcessing(HtmlTagIndex tagIndex, HtmlToken tag)
        {
            InternalDebug.Assert(!ignoreInputTag);

            EnqueueTail(QueueItemKind.PassThrough);

            if (!tag.IsTagEnd)
            {
                EnqueueTail(QueueItemKind.Suspend, tagIndex, allowWspLeft, allowWspRight);
            }
        }

        
        private void ProcessIgnoredTag(HtmlTagIndex tagIndex, HtmlToken tag)
        {
            

            if (!tag.IsTagEnd)
            {
                ignoreInputTag = true;
            }
        }

        
        private bool ProcessOpenTag(HtmlTagIndex tagIndex, HtmlDtd.TagDefinition tagDef)
        {
            if (!PrepareContainer(tagIndex, tagDef))
            {
                return false;
            }

            PushElement(tagIndex, true, tagDef);
            return true;
        }

        
        private bool ProcessEndTag(HtmlTagIndex tagIndex, HtmlDtd.TagDefinition tagDef)
        {
            var elementStackPos = -1;

            
            if (tagIndex == HtmlTagIndex.Unknown)
            {
                
                
                
                
                
                
                
                

                PushElement(tagIndex, true, tagDef);
                return true;
            }

            
            var useInputTag = true;
            var inputTagUsed = false;

            if (HtmlDtd.SetId.Null != tagDef.match)
            {
                elementStackPos = FindContainer(tagDef.match, tagDef.endContainers);

                if (elementStackPos >= 0 && elementStack[elementStackPos] != tagIndex)
                {
                    
                    
                    useInputTag = false;
                }
            }
            else
            {
                elementStackPos = FindContainer(tagIndex, tagDef.endContainers);
            }

            
            
            

            if (elementStackPos < 0)
            {
                

                var tagId2 = tagDef.unmatchedSubstitute;
                if (tagId2 == HtmlTagIndex._NULL)
                {
                    
                    return false;
                }

                if (tagId2 == HtmlTagIndex._IMPLICIT_BEGIN)
                {
                    

                    if (!PrepareContainer(tagIndex, tagDef))
                    {
                        return false;
                    }

                    
                    
                    
                    

                    
                    
                    inputToken.Flags &= ~HtmlToken.TagFlags.EndTag;

                    elementStackPos = PushElement(tagIndex, useInputTag, tagDef);

                    
                    inputTagUsed |= useInputTag;

                    
                    useInputTag = false;

                    
                    
                }
                else
                {
                    

                    elementStackPos = FindContainer(tagId2, GetTagDefinition(tagId2).endContainers);
                    if (elementStackPos < 0)
                    {
                        return false;
                    }

                    
                    
                    useInputTag = false;
                }
            }

            

            if (elementStackPos >= 0 && elementStackPos < elementStackTop)
            {
                
                useInputTag &= inputToken.IsEndTag;

                inputTagUsed |= useInputTag;

                CloseContainer(elementStackPos, useInputTag);
            }

            
            
            return inputTagUsed;
        }

        
        private bool PrepareContainer(HtmlTagIndex tagIndex, HtmlDtd.TagDefinition tagDef)
        {
            

            if (tagIndex == HtmlTagIndex.Unknown)
            {
                
                
                
                
                
                
                return true;
            }

            if (HtmlDtd.SetId.Null != tagDef.maskingContainers)
            {
                var elementStackPos = FindContainer(tagDef.maskingContainers, tagDef.beginContainers);
                if (elementStackPos >= 0)
                {
                    
                    return false;
                }
            }

            

            CloseAllProhibitedContainers(tagDef);

            

            var tagId2 = HtmlTagIndex._NULL;

            if (tagDef.textType == HtmlDtd.TagTextType.ALWAYS ||
                (tagDef.textType == HtmlDtd.TagTextType.QUERY && QueryTextlike(tagIndex)))
            {
                tagId2 = validRTC ? tagIdRTC : RequiredTextContainer();

                if (HtmlTagIndex._NULL != tagId2)
                {
                    CloseAllProhibitedContainers(GetTagDefinition(tagId2));
                    if (-1 == OpenContainer(tagId2))
                    {
                        return false;
                    }
                }
            }

            

            if (tagId2 == HtmlTagIndex._NULL)
            {
                if (HtmlDtd.SetId.Null != tagDef.requiredContainers)
                {
                    var elementStackPos = FindContainer(tagDef.requiredContainers, tagDef.beginContainers);
                    if (elementStackPos < 0)
                    {
                        
                        
                        
                        

                        CloseAllProhibitedContainers(GetTagDefinition(tagDef.defaultContainer));
                        if (-1 == OpenContainer(tagDef.defaultContainer))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        
        private int OpenContainer(HtmlTagIndex tagIndex)
        {
            var numTags = 0;

            while (HtmlTagIndex._NULL != tagIndex)
            {
                InternalDebug.Assert(numTags < openList.Length);
                openList[numTags++] = tagIndex;

                var tagDef = GetTagDefinition(tagIndex);

                if (HtmlDtd.SetId.Null == tagDef.requiredContainers)
                {
                    break;
                }

                var elementStackPos = FindContainer(tagDef.requiredContainers, tagDef.beginContainers);
                if (elementStackPos >= 0)
                {
                    break;
                }

                tagIndex = tagDef.defaultContainer;
            }

            
            if (HtmlTagIndex._NULL == tagIndex)
            {
                
                return -1;
            }

            var stackPos = -1;

            for (var i = numTags - 1; i >= 0; i--)
            {
                

                
                
                
                tagIndex = openList[i];

                stackPos = PushElement(tagIndex, false, GetTagDefinition(tagIndex));
            }

            return stackPos;
        }

        
        private void CloseContainer(int stackPos, bool useInputTag)
        {
            if (stackPos != elementStackTop - 1)
            {
                var closeNested = false;

                var numClose = 0;

                closeList[numClose++] = stackPos;

                if (GetTagDefinition(elementStack[stackPos]).scope == HtmlDtd.TagScope.NESTED)
                {
                    closeNested = true;
                }

                for (var i = stackPos + 1; i < elementStackTop; i++)
                {
                    var tagDef = GetTagDefinition(elementStack[i]);

                    if (numClose == closeList.Length)
                    {
                        
                        
                        var newCloseList = new int[closeList.Length * 2];
                        Array.Copy(closeList, 0, newCloseList, 0, numClose);
                        closeList = newCloseList;
                    }

                    if (closeNested && tagDef.scope == HtmlDtd.TagScope.NESTED)
                    {
                        closeList[numClose++] = i;
                    }
                    else
                    {
                        for (var j = 0; j < numClose; j++)
                        {
                            if (HtmlDtd.IsTagInSet(elementStack[closeList[j]], tagDef.endContainers))
                            {
                                closeList[numClose++] = i;

                                closeNested = closeNested || (tagDef.scope == HtmlDtd.TagScope.NESTED);
                                break;
                            }
                        }
                    }
                }

                

                for (var j = numClose - 1; j > 0; j--)
                {
                    

                    PopElement(closeList[j], false);
                }
            }

            

            PopElement(stackPos, useInputTag);
        }

        
        private void CloseAllProhibitedContainers(HtmlDtd.TagDefinition tagDef)
        {
            var close = tagDef.prohibitedContainers;

            if (HtmlDtd.SetId.Null != close)
            {
                while (true)
                {
                    var elementStackPos = FindContainer(close, tagDef.beginContainers);
                    if (elementStackPos < 0)
                    {
                        break;
                    }

                    CloseContainer(elementStackPos, false);
                }
            }
        }

        
        private void CloseAllContainers()
        {
            for (var i = elementStackTop - 1; i > 0; i--)
            {
                CloseContainer(i, false);
            }
        }

        
        private void CloseAllContainers(int level)
        {
            for (var i = elementStackTop - 1; i >= level; i--)
            {
                CloseContainer(i, false);
            }
        }

        
        private int FindContainer(HtmlDtd.SetId matchSet, HtmlDtd.SetId stopSet)
        {
            
            

            int i;

            for (i = elementStackTop - 1; i >= 0 && !HtmlDtd.IsTagInSet(elementStack[i], matchSet); i--)
            {
                if (HtmlDtd.IsTagInSet(elementStack[i], stopSet))
                {
                    return -1;
                }
            }

            return i;
        }

        
        private int FindContainer(HtmlTagIndex match, HtmlDtd.SetId stopSet)
        {
            int i;

            for (i = elementStackTop - 1; i >= 0 && elementStack[i] != match; i--)
            {
                if (HtmlDtd.IsTagInSet(elementStack[i], stopSet))
                {
                    return -1;
                }
            }

            return i;
        }

        
        private HtmlTagIndex RequiredTextContainer()
        {
            InternalDebug.Assert(!validRTC);

            validRTC = true;

            for (var i = elementStackTop - 1; i >= 0; i--)
            {
                var tagDef = GetTagDefinition(elementStack[i]);

                if (tagDef.textScope == HtmlDtd.TagTextScope.INCLUDE)
                {
                    tagIdRTC = HtmlTagIndex._NULL;
                    return tagIdRTC;
                }

                if (tagDef.textScope == HtmlDtd.TagTextScope.EXCLUDE)
                {
                    tagIdRTC = tagDef.textSubcontainer;
                    return tagIdRTC;
                }
            }

            InternalDebug.Assert(false);

            tagIdRTC = HtmlTagIndex._NULL;
            return tagIdRTC;
        }

        
        private int PushElement(HtmlTagIndex tagIndex, bool useInputTag, HtmlDtd.TagDefinition tagDef)
        {
            InternalDebug.Assert(!useInputTag || inputToken.IsTagBegin);

            int stackPos;

            if (ensureHead)
            {
                if (tagIndex == HtmlTagIndex.Body)
                {
                    
                    stackPos = PushElement(HtmlTagIndex.Head, false, HtmlDtd.tags[(int)HtmlTagIndex.Head]);
                    PopElement(stackPos, false);
                }
                else if (tagIndex == HtmlTagIndex.Head)
                {
                    
                    ensureHead = false;
                }
            }

            if (tagDef.textScope != HtmlDtd.TagTextScope.NEUTRAL)
            {
                validRTC = false;
            }

            if (elementStackTop == elementStack.Length && !EnsureElementStackSpace())
            {
                
                

                throw new TextConvertersException(Strings.HtmlNestingTooDeep);
            }

            var emptyScope = (tagDef.scope == HtmlDtd.TagScope.EMPTY);

            if (useInputTag)
            {
                if (inputToken.TagIndex != HtmlTagIndex.Unknown)
                {
                    
                    inputToken.Flags = emptyScope ? inputToken.Flags | HtmlToken.TagFlags.EmptyScope : inputToken.Flags & ~HtmlToken.TagFlags.EmptyScope;
                }
                else
                {
                    
                    
                    
                    
                    emptyScope = true; 
                }
            }

            stackPos = elementStackTop ++;

            elementStack[stackPos] = tagIndex;

            LFillTagB(tagDef);

            EnqueueTail(
                        useInputTag ? QueueItemKind.PassThrough : QueueItemKind.BeginElement, 
                        tagIndex, 
                        allowWspLeft, 
                        allowWspRight);

            if (useInputTag && !inputToken.IsTagEnd)
            {
                
                EnqueueTail(QueueItemKind.Suspend, tagIndex, allowWspLeft, allowWspRight);
            }

            RFillTagB(tagDef);

            

            

            if (!emptyScope)
            {
                

                if (tagDef.contextType != HtmlDtd.ContextType.None)
                {
                    

                    if (contextStackTop == contextStack.Length)
                    {
                        

                        EnsureContextStackSpace();
                    }

                    contextStack[contextStackTop++] = context;

                    

                    context.topElement = stackPos;
                    context.type = tagDef.contextType;
                    context.textType = tagDef.contextTextType;
                    context.accept = tagDef.accept;
                    context.reject = tagDef.reject;
                    context.ignoreEnd = tagDef.ignoreEnd;
                    context.hasSpace = false;
                    context.eatSpace = false;
                    context.oneNL = false;
                    context.lastCh = '\0';

                    if (context.textType != HtmlDtd.ContextTextType.Full)
                    {
                        allowWspLeft = false;
                        allowWspRight = false;
                    }

                    RFillTagB(tagDef);
                }
            }
            else
            {
                
                
                
                

                

                

                

                elementStackTop --;
            }

            return stackPos;
        }

        
        private void PopElement(int stackPos, bool useInputTag)
        {
            InternalDebug.Assert(!useInputTag || inputToken.IsTagBegin);

            var tagIndex = elementStack[stackPos];
            var tagDef = GetTagDefinition(tagIndex);

            if (tagDef.textScope != HtmlDtd.TagTextScope.NEUTRAL)
            {
                validRTC = false;
            }

            if (stackPos == context.topElement)
            {
                if (context.textType == HtmlDtd.ContextTextType.Full)
                {
                    LFillTagE(tagDef);
                }

                
                context = contextStack[--contextStackTop];
            }

            LFillTagE(tagDef);

            if (stackPos != elementStackTop - 1)
            {
                
                
                

                InternalDebug.Assert(stackPos < elementStackTop - 1);

                EnqueueTail(QueueItemKind.OverlappedClose, elementStackTop - stackPos - 1);
            }

            EnqueueTail(
                        useInputTag ? QueueItemKind.PassThrough : QueueItemKind.EndElement,
                        tagIndex, 
                        allowWspLeft, 
                        allowWspRight);

            if (useInputTag && !inputToken.IsTagEnd)
            {
                
                EnqueueTail(QueueItemKind.Suspend, tagIndex, allowWspLeft, allowWspRight);
            }

            RFillTagE(tagDef);

            

            if (stackPos != elementStackTop - 1)
            {
                
                

                InternalDebug.Assert(stackPos < elementStackTop - 1);

                EnqueueTail(QueueItemKind.OverlappedReopen, elementStackTop - stackPos - 1);

                
                Array.Copy(elementStack, stackPos + 1, elementStack, stackPos, elementStackTop - stackPos - 1);

                if (context.topElement > stackPos)
                {
                    context.topElement --;

                    for (var i = contextStackTop - 1; i > 0; i--)
                    {
                        InternalDebug.Assert(contextStack[i].topElement != stackPos);

                        if (contextStack[i].topElement < stackPos)
                        {
                            break;
                        }

                        contextStack[i].topElement --;
                    }
                }
            }

            elementStackTop --;
        }

        
        private void AddNonspace(char firstChar, char lastChar)
        {
            if (context.hasSpace)
            {
                context.hasSpace = false;

                

                if ('\0' == context.lastCh || !context.oneNL || !ParseSupport.TwoFarEastNonHanguelChars(context.lastCh, firstChar))
                {
                    EnqueueTail(QueueItemKind.Space);
                }
            }

            EnqueueTail(QueueItemKind.Text);

            context.eatSpace = false;
            context.lastCh = lastChar;
            context.oneNL = false;
        }

        
        private void AddSpace(bool oneNL)
        {
            InternalDebug.Assert(context.textType == HtmlDtd.ContextTextType.Full);

            if (!context.eatSpace)
            {
                context.hasSpace = true;
            }

            if (context.lastCh != '\0')
            {
                if (oneNL && !context.oneNL)
                {
                    context.oneNL = true;
                }
                else
                {
                    context.lastCh = '\0';
                }
            }
        }

        
        private bool QueryTextlike(HtmlTagIndex tagIndex)
        {
            
            
            
            var contextType = context.type;
            var i = contextStackTop;

            while (i != 0)
            {
                switch (contextType)
                {
                    case HtmlDtd.ContextType.Head:

                        if (tagIndex == HtmlTagIndex.Object)
                        {
                            return false;
                        }

                        
                        break;

                    case HtmlDtd.ContextType.Body:

                        switch (tagIndex)
                        {
                            case HtmlTagIndex.Input:
                            case HtmlTagIndex.Object:
                            case HtmlTagIndex.Applet:
                            case HtmlTagIndex.A:
                            case HtmlTagIndex.Div:
                            case HtmlTagIndex.Span:
                                    return true;
                        }

                        return false;
                }

                contextType = contextStack[--i].type;
            }

            

            InternalDebug.Assert(contextType == HtmlDtd.ContextType.Root);

            if (tagIndex == HtmlTagIndex.Object || tagIndex == HtmlTagIndex.Applet)
            {
                return true;
            }

            return false;
        }

        
        
        
        private void LFillTagB(HtmlDtd.TagDefinition tagDef)
        {
            if (context.textType == HtmlDtd.ContextTextType.Full)
            {
                LFill(FillCodeFromTag(tagDef).LB, FillCodeFromTag(tagDef).RB);
            }
        }

        
        private void RFillTagB(HtmlDtd.TagDefinition tagDef)
        {
            if (context.textType == HtmlDtd.ContextTextType.Full)
            {
                RFill(FillCodeFromTag(tagDef).RB);
            }
        }

        
        private void LFillTagE(HtmlDtd.TagDefinition tagDef)
        {
            if (context.textType == HtmlDtd.ContextTextType.Full)
            {
                LFill(FillCodeFromTag(tagDef).LE, FillCodeFromTag(tagDef).RE);
            }
        }

        
        private void RFillTagE(HtmlDtd.TagDefinition tagDef)
        {
            if (context.textType == HtmlDtd.ContextTextType.Full)
            {
                RFill(FillCodeFromTag(tagDef).RE);
            }
        }

        
        private void LFill(HtmlDtd.FillCode codeLeft, HtmlDtd.FillCode codeRight)
        {
            InternalDebug.Assert(context.textType == HtmlDtd.ContextTextType.Full);

            
            
            allowWspLeft = context.hasSpace || codeLeft == HtmlDtd.FillCode.EAT;

            context.lastCh = '\0';

            if (context.hasSpace)
            {
                if (codeLeft == HtmlDtd.FillCode.PUT)
                {
                    EnqueueTail(QueueItemKind.Space);
                    context.eatSpace = true;
                }

                context.hasSpace = (codeLeft == HtmlDtd.FillCode.NUL);
            }

            allowWspRight = context.hasSpace || codeRight == HtmlDtd.FillCode.EAT;
        }

        
        private void RFill(HtmlDtd.FillCode code)
        {
            InternalDebug.Assert(context.textType == HtmlDtd.ContextTextType.Full);

            if (code == HtmlDtd.FillCode.EAT)
            {
                context.hasSpace = false;
                context.eatSpace = true;
            }
            else if (code == HtmlDtd.FillCode.PUT)
            {
                context.eatSpace = false;
            }
        }

        
        private bool QueueEmpty()
        {
            return (queueHead == queueTail || queue[queueHead].kind == QueueItemKind.Suspend);
        }

        
        private QueueItemKind QueueHeadKind()
        {
            if (queueHead == queueTail)
            {
                return QueueItemKind.Empty;
            }

            return queue[queueHead].kind;
        }

        
        private void EnqueueTail(QueueItemKind kind, HtmlTagIndex tagIndex, bool allowWspLeft, bool allowWspRight)
        {
            if (queueTail == queue.Length)
            {
                ExpandQueue();
            }

            queue[queueTail].kind = kind;
            queue[queueTail].tagIndex = tagIndex;
            queue[queueTail].flags = (allowWspLeft ? QueueItemFlags.AllowWspLeft : 0) | 
                                        (allowWspRight ? QueueItemFlags.AllowWspRight : 0);
            queue[queueTail].argument = 0;

            queueTail ++;
        }

        
        private void EnqueueTail(QueueItemKind kind, int argument)
        {
            if (queueTail == queue.Length)
            {
                ExpandQueue();
            }

            queue[queueTail].kind = kind;
            queue[queueTail].tagIndex = HtmlTagIndex._NULL;
            queue[queueTail].flags = 0; 
            queue[queueTail].argument = argument;

            queueTail ++;
        }

        
        private void EnqueueTail(QueueItemKind kind)
        {
            if (queueTail == queue.Length)
            {
                ExpandQueue();
            }

            queue[queueTail].kind = kind;
            queue[queueTail].tagIndex = HtmlTagIndex._NULL;
            queue[queueTail].flags = 0; 
            queue[queueTail].argument = 0;

            queueTail ++;
        }

        
        private void EnqueueHead(QueueItemKind kind, HtmlTagIndex tagIndex, bool allowWspLeft, bool allowWspRight)
        {
            

            if (queueHead != queueStart)
            {
                queueHead --;
            }
            else
            {
                
                InternalDebug.Assert(queueHead == queueTail);
                queueTail ++;
            }

            queue[queueHead].kind = kind;
            queue[queueHead].tagIndex = tagIndex;
            queue[queueHead].flags = (allowWspLeft ? QueueItemFlags.AllowWspLeft : 0) | 
                                        (allowWspRight ? QueueItemFlags.AllowWspRight : 0);
            queue[queueHead].argument = 0;
        }

        
        private void EnqueueHead(QueueItemKind kind)
        {
            EnqueueHead(kind, 0);
        }

        
        private void EnqueueHead(QueueItemKind kind, int argument)
        {
            

            if (queueHead != queueStart)
            {
                queueHead --;
            }
            else
            {
                
                InternalDebug.Assert(queueHead == queueTail);
                queueTail ++;
            }

            queue[queueHead].kind = kind;
            queue[queueHead].tagIndex = HtmlTagIndex._NULL;
            queue[queueHead].flags = 0; 
            queue[queueHead].argument = argument;
        }

        
        private HtmlTokenId GetTokenFromQueue()
        {
            QueueItem qi;

            switch (QueueHeadKind())
            {
                case QueueItemKind.None:

                        

                        DoDequeueFirst();

                        token = null;

                        return HtmlTokenId.None;

                case QueueItemKind.PassThrough:

                        

                        qi = DoDequeueFirst();

                        token = inputToken;

                        if (token.TokenId == HtmlTokenId.Tag)
                        {
                            

                            token.Flags |= (((qi.flags & QueueItemFlags.AllowWspLeft) == QueueItemFlags.AllowWspLeft) ? 
                                                        HtmlToken.TagFlags.AllowWspLeft : 0) |
                                                (((qi.flags & QueueItemFlags.AllowWspRight) == QueueItemFlags.AllowWspRight) ?
                                                        HtmlToken.TagFlags.AllowWspRight : 0);

                            if (token.OriginalTagId == HtmlTagIndex.Body &&
                                token.IsTagEnd &&
                                injection != null && 
                                injection.HaveHead && 
                                !injection.HeadDone)
                            {
                                
                                InternalDebug.Assert(token.IsEndTag == false);

                                
                                

                                var bodyLevel = FindContainer(HtmlTagIndex.Body, HtmlDtd.SetId.Empty);

                                parser = (HtmlParser) injection.Push(true, parser);
                                saveState.Save(this, bodyLevel + 1);
                                EnqueueTail(QueueItemKind.InjectionBegin, 1);
                                if (injection.HeaderFooterFormat == HeaderFooterFormat.Text)
                                {
                                    OpenContainer(HtmlTagIndex.TT);
                                    OpenContainer(HtmlTagIndex.Pre);
                                }
                            }
                        }

                        return token.TokenId;

                case QueueItemKind.BeginElement:
                case QueueItemKind.EndElement:

                        

                        qi = DoDequeueFirst();

                        tokenBuilder.BuildTagToken(
                                            qi.tagIndex, 
                                            qi.kind == QueueItemKind.EndElement,
                                            (qi.flags & QueueItemFlags.AllowWspLeft) == QueueItemFlags.AllowWspLeft,
                                            (qi.flags & QueueItemFlags.AllowWspRight) == QueueItemFlags.AllowWspRight,
                                            false);

                        token = tokenBuilder;

                        if (qi.kind == QueueItemKind.BeginElement &&
                            token.OriginalTagId == HtmlTagIndex.Body &&
                            injection != null && 
                            injection.HaveHead && 
                            !injection.HeadDone)
                        {
                            
                            InternalDebug.Assert(token.IsEndTag == false);

                            
                            

                            var bodyLevel = FindContainer(HtmlTagIndex.Body, HtmlDtd.SetId.Empty);

                            parser = (HtmlParser) injection.Push(true, parser);
                            saveState.Save(this, bodyLevel + 1);
                            EnqueueTail(QueueItemKind.InjectionBegin, 1);
                            if (injection.HeaderFooterFormat == HeaderFooterFormat.Text)
                            {
                                OpenContainer(HtmlTagIndex.TT);
                                OpenContainer(HtmlTagIndex.Pre);
                            }
                        }

                        
                        return token.TokenId;

                case QueueItemKind.EndLastTag:

                        qi = DoDequeueFirst();

                        tokenBuilder.BuildTagToken(
                                            qi.tagIndex, 
                                            false,      
                                            (qi.flags & QueueItemFlags.AllowWspLeft) == QueueItemFlags.AllowWspLeft,
                                            (qi.flags & QueueItemFlags.AllowWspRight) == QueueItemFlags.AllowWspRight,
                                            true);

                        token = tokenBuilder;

                        if (qi.kind == QueueItemKind.BeginElement &&
                            token.OriginalTagId == HtmlTagIndex.Body &&
                            injection != null && 
                            injection.HaveHead && 
                            !injection.HeadDone)
                        {
                            
                            InternalDebug.Assert(token.IsEndTag == false);

                            
                            

                            var bodyLevel = FindContainer(HtmlTagIndex.Body, HtmlDtd.SetId.Empty);

                            parser = (HtmlParser) injection.Push(true, parser);
                            saveState.Save(this, bodyLevel + 1);
                            EnqueueTail(QueueItemKind.InjectionBegin, 1);
                            if (injection.HeaderFooterFormat == HeaderFooterFormat.Text)
                            {
                                OpenContainer(HtmlTagIndex.TT);
                                OpenContainer(HtmlTagIndex.Pre);
                            }
                        }

                        
                        return token.TokenId;

                case QueueItemKind.OverlappedClose:
                case QueueItemKind.OverlappedReopen:

                        

                        qi = DoDequeueFirst();

                        tokenBuilder.BuildOverlappedToken(qi.kind == QueueItemKind.OverlappedClose, qi.argument);

                        token = tokenBuilder;

                        
                        return token.TokenId;

                case QueueItemKind.Space:

                        

                        qi = DoDequeueFirst();

                        tokenBuilder.BuildSpaceToken();

                        token = tokenBuilder;

                        
                        return token.TokenId;

                case QueueItemKind.Text:

                        
                        

                        var requeueInjectionEnd = false;
                        var requeueInjectionArgument = 0;

                        InternalDebug.Assert(context.textType == HtmlDtd.ContextTextType.Full);
                        
                        
                        qi = DoDequeueFirst();

                        if (queueHead != queueTail)
                        {
                            InternalDebug.Assert(queueHead == queueTail - 1 && QueueHeadKind() == QueueItemKind.InjectionEnd);
                            requeueInjectionEnd = true;
                            requeueInjectionArgument = queue[queueHead].argument;
                            DoDequeueFirst();
                        }

                        
                        

                        tokenBuilder.BuildTextSliceToken(inputToken, currentRun, currentRunOffset, numRuns);

                        token = tokenBuilder;

                        var runs = inputToken.Runs;

                        if (runs.IsValidPosition)
                        {
                            var cnt = 0;

                            InternalDebug.Assert(runs.Current.TextType <= RunTextType.LastWhitespace);

                            

                            do
                            {
                                
                                
                                cnt += runs.Current.TextType == RunTextType.NewLine ? 1 : 2;
                            }
                            while (runs.MoveNext(true) && (runs.Current.TextType <= RunTextType.LastWhitespace));

                            if (cnt != 0)
                            {
                                
                                
                                AddSpace(cnt == 1);
                            }

                            currentRun = runs.CurrentIndex;
                            currentRunOffset = runs.CurrentOffset;

                            if (runs.IsValidPosition)
                            {
                                var firstChar = runs.Current.FirstChar;
                                var lastChar = firstChar;
                                do
                                {
                                    lastChar = runs.Current.LastChar;
                                }
                                while (runs.MoveNext(true) && !(runs.Current.TextType <= RunTextType.LastWhitespace));

                                
                                AddNonspace(firstChar, lastChar);
                            }

                            numRuns = runs.CurrentIndex - currentRun;
                        }
                        else
                        {
                            currentRun = runs.CurrentIndex;
                            currentRunOffset = runs.CurrentOffset;
                            numRuns = 0;
                        }

                        if (requeueInjectionEnd)
                        {
                            EnqueueTail(QueueItemKind.InjectionEnd, requeueInjectionArgument);
                        }

                        
                        return token.TokenId;

                case QueueItemKind.Eof:

                        
                        

                        InternalDebug.Assert(queueHead + 1 == queueTail);     

                        tokenBuilder.BuildEofToken();

                        token = tokenBuilder;

                        break;

                case QueueItemKind.InjectionBegin:
                case QueueItemKind.InjectionEnd:

                        

                        qi = DoDequeueFirst();

                        tokenBuilder.BuildInjectionToken(qi.kind == QueueItemKind.InjectionBegin, qi.argument != 0);

                        token = tokenBuilder;

                        break;

                default:

                        InternalDebug.Assert(false);
                        break;
            }

            
            return token.TokenId;
        }

        
        private void ExpandQueue()
        {
            
            

            var newQueue = new QueueItem[queue.Length * 2];
            Array.Copy(queue, queueHead, newQueue, queueHead, queueTail - queueHead);
            if (queueStart != 0)
            {
                Array.Copy(queue, 0, newQueue, 0, queueStart);
            }
            queue = newQueue;
            newQueue = null;
        }

        
        private QueueItem DoDequeueFirst()
        {
            var head = queueHead;

            queueHead ++;          

            if (queueHead == queueTail)
            {
                queueHead = queueTail = queueStart;
            }

            return queue[head];
        }

        
        private HtmlDtd.TagFill FillCodeFromTag(HtmlDtd.TagDefinition tagDef)
        {
            if (context.type == HtmlDtd.ContextType.Select && tagDef.tagIndex != HtmlTagIndex.Option)
            {
                return HtmlDtd.TagFill.PUT_PUT_PUT_PUT;
            }
            else if (context.type == HtmlDtd.ContextType.Title)
            {
                InternalDebug.Assert(tagDef.tagIndex == HtmlTagIndex.Title);
                return HtmlDtd.TagFill.NUL_EAT_EAT_NUL;
            }

            return tagDef.fill;
        }

        
        private bool EnsureElementStackSpace()
        {
            if (elementStackTop == elementStack.Length)
            {
                if (elementStack.Length >= maxElementStack)
                {
                    return false;
                }

                var newSize = (maxElementStack / 2 > elementStack.Length) ? elementStack.Length * 2 : maxElementStack;

                var newElementStack = new HtmlTagIndex[newSize];
                Array.Copy(elementStack, 0, newElementStack, 0, elementStackTop);
                elementStack = newElementStack;
                newElementStack = null;
            }

            return true;
        }
        
        
        private void EnsureContextStackSpace()
        {
            if (contextStackTop + 1 > contextStack.Length)
            {
                

                var newContextStack = new Context[contextStack.Length * 2];
                Array.Copy(contextStack, 0, newContextStack, 0, contextStackTop);
                contextStack = newContextStack;
                newContextStack = null;
            }
        }
        
        
        private struct Context
        {
            public int topElement;                              

            
            public HtmlDtd.ContextType type;                    
            public HtmlDtd.ContextTextType textType;            
            public HtmlDtd.SetId accept;
            public HtmlDtd.SetId reject;
            public HtmlDtd.SetId ignoreEnd;

            
            public char lastCh;
            public bool oneNL;
            public bool hasSpace;
            public bool eatSpace;
        }

        
        private struct QueueItem
        {
            public QueueItemKind kind;
            public HtmlTagIndex tagIndex;
            public QueueItemFlags flags;
            public int argument;
        }

        
        
        
        private class DocumentState
        {
            private int queueHead;
            private int queueTail;
            private HtmlToken inputToken;
            private int elementStackTop;
            private int currentRun;
            private int currentRunOffset;
            private int numRuns;
            private HtmlTagIndex[] savedElementStackEntries = new HtmlTagIndex[5];
            private int savedElementStackEntriesCount;
            private bool hasSpace;
            private bool eatSpace;
            private bool validRTC;
            private HtmlTagIndex tagIdRTC;

            public DocumentState()
            {
            }

            public int SavedStackTop => elementStackTop;

            public void Save(HtmlNormalizingParser document, int stackLevel)
            {
                if (stackLevel != document.elementStackTop)
                {
                    
                    InternalDebug.Assert(stackLevel < document.elementStackTop && document.elementStackTop - stackLevel < 5);

                    Array.Copy(document.elementStack, stackLevel, savedElementStackEntries, 0, document.elementStackTop - stackLevel);
                    savedElementStackEntriesCount = document.elementStackTop - stackLevel;
                    document.elementStackTop = stackLevel;
                }
                else
                {
                    savedElementStackEntriesCount = 0;
                }

                elementStackTop = document.elementStackTop;
                queueHead = document.queueHead;
                queueTail = document.queueTail;
                inputToken = document.inputToken;
                currentRun = document.currentRun;
                currentRunOffset = document.currentRunOffset;
                numRuns = document.numRuns;
                hasSpace = document.context.hasSpace;
                eatSpace = document.context.eatSpace;
                validRTC = document.validRTC;
                tagIdRTC = document.tagIdRTC;

                
                document.queueStart = document.queueTail;
                document.queueHead = document.queueTail = document.queueStart;
            }

            public void Restore(HtmlNormalizingParser document)
            {
                InternalDebug.Assert(document.elementStackTop == elementStackTop);

                if (savedElementStackEntriesCount != 0)
                {
                    Array.Copy(savedElementStackEntries, 0, document.elementStack, document.elementStackTop, savedElementStackEntriesCount);
                    document.elementStackTop += savedElementStackEntriesCount;
                }

                document.queueStart = 0;
                document.queueHead = queueHead;
                document.queueTail = queueTail;

                document.inputToken = inputToken;
                document.currentRun = currentRun;
                document.currentRunOffset = currentRunOffset;
                document.numRuns = numRuns;
                document.context.hasSpace = hasSpace;
                document.context.eatSpace = eatSpace;
                document.validRTC = validRTC;
                document.tagIdRTC = tagIdRTC;
            }
        }

        
        private class SmallTokenBuilder : HtmlToken
        {
            private char[] spareBuffer = new char[1];
            private RunEntry[] spareRuns = new RunEntry[1];

            public SmallTokenBuilder()
            {
            }

            public void BuildTagToken(HtmlTagIndex tagIndex, bool closingTag, bool allowWspLeft, bool allowWspRight, bool endOnly)
            {
                tokenId = (TokenId)HtmlTokenId.Tag;
                argument = 1;

                buffer = spareBuffer;
                runList = spareRuns;
                whole.Reset();
                wholePosition.Rewind(whole);

                this.tagIndex = originalTagIndex = tagIndex;
                nameIndex = HtmlDtd.tags[(int)tagIndex].nameIndex;

                if (!endOnly)
                {
                    partMajor = TagPartMajor.Complete;
                    partMinor = TagPartMinor.CompleteName;
                }
                else
                {
                    partMajor = TagPartMajor.End;
                    partMinor = TagPartMinor.Empty;
                }

                flags = (closingTag ? TagFlags.EndTag : 0) |
                            (allowWspLeft ? TagFlags.AllowWspLeft : 0) | 
                            (allowWspRight ? TagFlags.AllowWspRight : 0);
            }

            public void BuildOverlappedToken(bool close, int argument)
            {
                tokenId = (TokenId)(close ? HtmlTokenId.OverlappedClose : HtmlTokenId.OverlappedReopen);
                this.argument = argument;

                buffer = spareBuffer;
                runList = spareRuns;
                whole.Reset();
                wholePosition.Rewind(whole);
            }

            public void BuildInjectionToken(bool begin, bool head)
            {
                tokenId = (TokenId)(begin ? HtmlTokenId.InjectionBegin : HtmlTokenId.InjectionEnd);
                argument = head ? 1 : 0;

                buffer = spareBuffer;
                runList = spareRuns;
                whole.Reset();
                wholePosition.Rewind(whole);
            }

            public void BuildSpaceToken()
            {
                tokenId = (TokenId)HtmlTokenId.Text;
                argument = 1;

                buffer = spareBuffer;
                runList = spareRuns;

                buffer[0] = ' ';
                runList[0].Initialize(RunType.Normal, RunTextType.Space, (uint)HtmlRunKind.Text, 1, 0);

                whole.Reset();
                whole.tail = 1;
                wholePosition.Rewind(whole);
            }

            public void BuildTextSliceToken(Token source, int startRun, int startRunOffset, int numRuns)
            {
                InternalDebug.Assert(numRuns > 0);
                InternalDebug.Assert(startRun >= source.whole.head && startRun < source.whole.tail);
                InternalDebug.Assert(startRun + numRuns <= source.whole.tail);
                InternalDebug.Assert(startRunOffset >= source.whole.headOffset && startRunOffset < source.buffer.Length);

                tokenId = (TokenId)HtmlTokenId.Text;
                argument = 0;

                buffer = source.buffer;
                runList = source.runList;

                whole.Initialize(startRun, startRunOffset);
                whole.tail = whole.head + numRuns;
                wholePosition.Rewind(whole);
            }

            public void BuildEofToken()
            {
                tokenId = (TokenId)HtmlTokenId.EndOfFile;
                argument = 0;

                buffer = spareBuffer;
                runList = spareRuns;
                whole.Reset();
                wholePosition.Rewind(whole);
            }
        }
    }
}

