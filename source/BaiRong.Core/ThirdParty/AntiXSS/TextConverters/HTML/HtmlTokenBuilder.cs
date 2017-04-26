// ***************************************************************
// <copyright file="HtmlTokenBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters.Internal.Html
{
    using System;
    using Data.Internal;

    

    internal class HtmlTokenBuilder : TokenBuilder
    {
        

        
        protected const byte BuildStateEndedHtml = BuildStateEnded + 1; 
        protected const byte BuildStateTagStarted = 20;
        protected const byte BuildStateTagText = 21;
        protected const byte BuildStateTagName = 22;
        protected const byte BuildStateTagBeforeAttr = 23;
        
        protected const byte BuildStateTagAttrName = 24;
        protected const byte BuildStateTagEndAttrName = 25;
        protected const byte BuildStateTagAttrValue = 26;
        protected const byte BuildStateTagEndAttrValue = 27;

        protected HtmlToken htmlToken;

        protected int maxAttrs;

        protected int numCarryOverRuns;        
        protected int carryOverRunsHeadOffset;
        protected int carryOverRunsLength;

        

        public HtmlTokenBuilder(
                    char[] buffer, 
                    int maxRuns, 
                    int maxAttrs, 
                    bool testBoundaryConditions) :
            base(new HtmlToken(), buffer, maxRuns, testBoundaryConditions)
        {
            htmlToken = (HtmlToken) base.Token;

            var initialAttrs = 8;

            if (maxAttrs != 0)
            {
                if (!testBoundaryConditions)
                {
                    this.maxAttrs = maxAttrs;
                }
                else
                {
                    initialAttrs = 1;
                    this.maxAttrs = 5;
                }

                

                htmlToken.attributeList = new HtmlToken.AttributeEntry[initialAttrs];
            }

            htmlToken.nameIndex = HtmlNameIndex._NOTANAME;
        }

        

        public new HtmlToken Token => htmlToken;


        public bool IncompleteTag => state >= FirstStarted && state != BuildStateText;


        public override void Reset()
        {
            if (state >= BuildStateEndedHtml)
            {
                htmlToken.Reset();

                numCarryOverRuns = 0;
            }

            base.Reset();
        }

        

        public HtmlTokenId MakeEmptyToken(HtmlTokenId tokenId)
        {
            return (HtmlTokenId)base.MakeEmptyToken((TokenId)tokenId);
        }

        

        public HtmlTokenId MakeEmptyToken(HtmlTokenId tokenId, int argument)
        {
            return (HtmlTokenId)base.MakeEmptyToken((TokenId)tokenId, argument);
        }

        

        public void StartTag(HtmlNameIndex nameIndex, int baseOffset)
        {
            InternalDebug.Assert(state == BuildStateInitialized && htmlToken.IsEmpty);

            state = BuildStateTagStarted;

            htmlToken.tokenId = (TokenId)HtmlTokenId.Tag;
            htmlToken.partMajor = HtmlToken.TagPartMajor.Begin;
            htmlToken.partMinor = HtmlToken.TagPartMinor.Empty;
            htmlToken.nameIndex = nameIndex;
            htmlToken.tagIndex = HtmlNameData.names[(int)nameIndex].tagIndex;

            htmlToken.whole.headOffset = baseOffset;
            tailOffset = baseOffset;
        }

        

        public void AbortConditional(bool comment)
        {
            InternalDebug.Assert(htmlToken.nameIndex == HtmlNameIndex._CONDITIONAL);
            InternalDebug.Assert(state == BuildStateTagStarted || state == BuildStateTagText);

            
            
            
            
            
            

            htmlToken.nameIndex = comment ? HtmlNameIndex._COMMENT : HtmlNameIndex._BANG;
        }

        

        public void SetEndTag()
        {
            htmlToken.flags |= HtmlToken.TagFlags.EndTag;
        }

        

        public void SetEmptyScope()
        {
            htmlToken.flags |= HtmlToken.TagFlags.EmptyScope;
        }

        

        public void StartTagText()
        {
            InternalDebug.Assert(state == BuildStateTagStarted);

            state = BuildStateTagText;

            htmlToken.unstructured.Initialize(htmlToken.whole.tail, tailOffset);
            htmlToken.unstructuredPosition.Rewind(htmlToken.unstructured);
        }

        

        public void EndTagText()
        {
            if (htmlToken.unstructured.head == htmlToken.whole.tail)
            {
                InternalDebug.Assert(htmlToken.unstructured.headOffset == tailOffset);

                AddNullRun(HtmlRunKind.TagText);
            }

            state = BuildStateTagStarted;
        }

        

        public void StartTagName()
        {
            InternalDebug.Assert(state == BuildStateTagStarted);

            state = BuildStateTagName;

            htmlToken.partMinor |= HtmlToken.TagPartMinor.BeginName;

            htmlToken.name.Initialize(htmlToken.whole.tail, tailOffset);
            htmlToken.localName.Initialize(htmlToken.whole.tail, tailOffset);
            htmlToken.namePosition.Rewind(htmlToken.name);
        }

        public void EndTagNamePrefix()
        {
            InternalDebug.Assert(state == BuildStateTagName);

            htmlToken.localName.Initialize(htmlToken.whole.tail, tailOffset);
        }

        

        public void EndTagName(int nameLength)
        {
            InternalDebug.Assert(state == BuildStateTagName);

            InternalDebug.Assert(htmlToken.partMinor == HtmlToken.TagPartMinor.BeginName || 
                                htmlToken.partMinor == HtmlToken.TagPartMinor.ContinueName);

            if (htmlToken.localName.head == htmlToken.whole.tail)
            {
                InternalDebug.Assert(htmlToken.localName.headOffset == tailOffset);

                AddNullRun(HtmlRunKind.Name);
                if (htmlToken.localName.head == htmlToken.name.head)
                {
                    htmlToken.flags |= HtmlToken.TagFlags.EmptyTagName;
                }
            }

            htmlToken.partMinor |= HtmlToken.TagPartMinor.EndName;

            if (htmlToken.IsTagBegin)
            {
                AddSentinelRun();
                htmlToken.nameIndex = LookupName(nameLength, htmlToken.name);
                htmlToken.tagIndex = htmlToken.originalTagIndex = HtmlNameData.names[(int)htmlToken.nameIndex].tagIndex;
            }

            state = BuildStateTagBeforeAttr;
        }

        

        public void EndTagName(HtmlNameIndex resolvedNameIndex)
        {
            InternalDebug.Assert(state == BuildStateTagName);

            InternalDebug.Assert(htmlToken.partMinor == HtmlToken.TagPartMinor.BeginName || 
                                htmlToken.partMinor == HtmlToken.TagPartMinor.ContinueName);

            if (htmlToken.localName.head == htmlToken.whole.tail)
            {
                InternalDebug.Assert(htmlToken.localName.headOffset == tailOffset);

                AddNullRun(HtmlRunKind.Name);
                if (htmlToken.localName.head == htmlToken.name.head)
                {
                    htmlToken.flags |= HtmlToken.TagFlags.EmptyTagName;
                }
            }

            htmlToken.partMinor |= HtmlToken.TagPartMinor.EndName;

            if (htmlToken.IsTagBegin)
            {
                htmlToken.nameIndex = resolvedNameIndex;
                htmlToken.tagIndex = htmlToken.originalTagIndex = HtmlNameData.names[(int)resolvedNameIndex].tagIndex;
            }

            state = BuildStateTagBeforeAttr;
        }

        

        public bool CanAddAttribute()
        {
            InternalDebug.Assert(state == BuildStateTagBeforeAttr);

            return htmlToken.attributeTail < maxAttrs;
        }

        

        public void StartAttribute()
        {
            if (htmlToken.attributeTail == htmlToken.attributeList.Length)
            {
                

                
                InternalDebug.Assert(htmlToken.attributeList.Length < maxAttrs);

                int newSize;

                if (maxAttrs / 2 > htmlToken.attributeList.Length)
                {
                    newSize = htmlToken.attributeList.Length * 2;
                }
                else
                {
                    newSize = maxAttrs;
                }

                var newAttrs = new HtmlToken.AttributeEntry[newSize];

                Array.Copy(htmlToken.attributeList, 0, newAttrs, 0, htmlToken.attributeTail);

                htmlToken.attributeList = newAttrs;
            }

            InternalDebug.Assert(htmlToken.attributeTail < htmlToken.attributeList.Length);
            InternalDebug.Assert(state == BuildStateTagBeforeAttr);

            InternalDebug.Assert(htmlToken.partMinor == HtmlToken.TagPartMinor.Empty ||
                                htmlToken.partMinor == HtmlToken.TagPartMinor.EndName ||
                                htmlToken.partMinor == HtmlToken.TagPartMinor.EndNameWithAttributes ||
                                htmlToken.partMinor == HtmlToken.TagPartMinor.CompleteName ||
                                htmlToken.partMinor == HtmlToken.TagPartMinor.CompleteNameWithAttributes ||
                                htmlToken.partMinor == HtmlToken.TagPartMinor.EndAttribute ||
                                htmlToken.partMinor == HtmlToken.TagPartMinor.EndAttributeWithOtherAttributes ||
                                htmlToken.partMinor == HtmlToken.TagPartMinor.Attributes);

            if (htmlToken.partMinor == HtmlToken.TagPartMinor.Empty)
            {
                htmlToken.partMinor = HtmlToken.TagPartMinor.BeginAttribute;
            }

            htmlToken.attributeList[htmlToken.attributeTail].nameIndex = HtmlNameIndex.Unknown;
            htmlToken.attributeList[htmlToken.attributeTail].partMajor = HtmlToken.AttrPartMajor.Begin;
            htmlToken.attributeList[htmlToken.attributeTail].partMinor = HtmlToken.AttrPartMinor.BeginName;
            htmlToken.attributeList[htmlToken.attributeTail].quoteChar = 0;
            htmlToken.attributeList[htmlToken.attributeTail].name.Initialize(htmlToken.whole.tail, tailOffset);
            htmlToken.attributeList[htmlToken.attributeTail].localName.Initialize(htmlToken.whole.tail, tailOffset);
            htmlToken.attributeList[htmlToken.attributeTail].value.Reset();

            state = BuildStateTagAttrName;
        }

        public void EndAttributeNamePrefix()
        {
            InternalDebug.Assert(state == BuildStateTagAttrName);

            htmlToken.attributeList[htmlToken.attributeTail].localName.Initialize(htmlToken.whole.tail, tailOffset);
        }

        

        public void EndAttributeName(int nameLength)
        {
            InternalDebug.Assert(state == BuildStateTagAttrName);

            htmlToken.attributeList[htmlToken.attributeTail].partMinor |= HtmlToken.AttrPartMinor.EndName;

            if (htmlToken.attributeList[htmlToken.attributeTail].localName.head == htmlToken.whole.tail)
            {
                InternalDebug.Assert(htmlToken.attributeList[htmlToken.attributeTail].localName.headOffset == tailOffset);

                AddNullRun(HtmlRunKind.Name);
                if (htmlToken.attributeList[htmlToken.attributeTail].localName.head == htmlToken.attributeList[htmlToken.attributeTail].name.head)
                {
                    htmlToken.attributeList[htmlToken.attributeTail].partMajor |= HtmlToken.AttrPartMajor.EmptyName;
                }
            }

            if (htmlToken.attributeList[htmlToken.attributeTail].IsAttrBegin)
            {
                AddSentinelRun();
                htmlToken.attributeList[htmlToken.attributeTail].nameIndex = LookupName(nameLength, htmlToken.attributeList[htmlToken.attributeTail].name);
            }

            state = BuildStateTagEndAttrName;
        }

        

        public void StartValue()
        {
            InternalDebug.Assert(state == BuildStateTagEndAttrName);

            htmlToken.attributeList[htmlToken.attributeTail].value.Initialize(htmlToken.whole.tail, tailOffset);

            htmlToken.attributeList[htmlToken.attributeTail].partMinor |= HtmlToken.AttrPartMinor.BeginValue;

            state = BuildStateTagAttrValue;
        }

        

        public void SetValueQuote(char ch)
        {
            InternalDebug.Assert(state == BuildStateTagAttrValue);
            InternalDebug.Assert(ParseSupport.QuoteCharacter(ParseSupport.GetCharClass(ch)));

            htmlToken.attributeList[htmlToken.attributeTail].IsAttrValueQuoted = true;
            htmlToken.attributeList[htmlToken.attributeTail].quoteChar = (byte) ch;
        }

        

        public void EndValue()
        {
            InternalDebug.Assert(state == BuildStateTagAttrValue);

            if (htmlToken.attributeList[htmlToken.attributeTail].value.head == htmlToken.whole.tail)
            {
                InternalDebug.Assert(htmlToken.attributeList[htmlToken.attributeTail].value.headOffset == tailOffset);

                AddNullRun(HtmlRunKind.AttrValue);
            }

            htmlToken.attributeList[htmlToken.attributeTail].partMinor |= HtmlToken.AttrPartMinor.EndValue;

            state = BuildStateTagEndAttrValue;
        }

        

        public void EndAttribute()
        {
            InternalDebug.Assert(state == BuildStateTagEndAttrName || state == BuildStateTagEndAttrValue);

            htmlToken.attributeList[htmlToken.attributeTail].partMajor |= HtmlToken.AttrPartMajor.End;

            htmlToken.attributeTail ++;

            if (htmlToken.attributeTail < htmlToken.attributeList.Length)
            {
                

                htmlToken.attributeList[htmlToken.attributeTail].partMajor = HtmlToken.AttrPartMajor.None;
                htmlToken.attributeList[htmlToken.attributeTail].partMinor = HtmlToken.AttrPartMinor.Empty;
            }

            if (htmlToken.partMinor == HtmlToken.TagPartMinor.BeginAttribute)
            {
                htmlToken.partMinor = HtmlToken.TagPartMinor.Attributes;
            }
            else if (htmlToken.partMinor == HtmlToken.TagPartMinor.ContinueAttribute)
            {
                htmlToken.partMinor = HtmlToken.TagPartMinor.EndAttribute;
            }
            else
            {
                InternalDebug.Assert(htmlToken.partMinor == HtmlToken.TagPartMinor.CompleteName ||
                                    htmlToken.partMinor == HtmlToken.TagPartMinor.CompleteNameWithAttributes ||
                                    htmlToken.partMinor == HtmlToken.TagPartMinor.EndName ||
                                    htmlToken.partMinor == HtmlToken.TagPartMinor.EndNameWithAttributes ||
                                    htmlToken.partMinor == HtmlToken.TagPartMinor.EndAttribute ||
                                    htmlToken.partMinor == HtmlToken.TagPartMinor.EndAttributeWithOtherAttributes ||
                                    htmlToken.partMinor == HtmlToken.TagPartMinor.Attributes);

                htmlToken.partMinor |= HtmlToken.TagPartMinor.Attributes;
            }

            state = BuildStateTagBeforeAttr;
        }

        

        public void EndTag(bool complete)
        {
            if (complete)
            {
                

                if (state != BuildStateTagBeforeAttr)
                {
                    if (state == BuildStateTagText)
                    {
                        EndTagText();
                    }
                    else if (state == BuildStateTagName)
                    {
                        EndTagName(0);
                    }
                    else
                    {
                        if (state == BuildStateTagAttrName)
                        {
                            EndAttributeName(0);
                        }
                        else if (state == BuildStateTagAttrValue)
                        {
                            EndValue();
                        }

                        if (state == BuildStateTagEndAttrName || state == BuildStateTagEndAttrValue)
                        {
                            EndAttribute();
                        }
                    }
                }

                
                AddSentinelRun();

                InternalDebug.Assert(state == BuildStateTagBeforeAttr || state == BuildStateTagStarted);

                state = BuildStateEndedHtml;
                htmlToken.partMajor |= HtmlToken.TagPartMajor.End;
            }
            else
            {
                if (state >= BuildStateTagAttrName)
                {
                    
                    

                    InternalDebug.Assert(!complete);

                    if (0 != htmlToken.attributeTail ||
                        htmlToken.name.head != -1 ||
                        htmlToken.attributeList[htmlToken.attributeTail].name.head > 0)
                    {
                        
                        

                        InternalDebug.Assert(htmlToken.attributeList[htmlToken.attributeTail].IsAttrBegin);

                        
                        AddSentinelRun();

                        
                        

                        numCarryOverRuns = htmlToken.whole.tail - htmlToken.attributeList[htmlToken.attributeTail].name.head;
                        carryOverRunsHeadOffset = htmlToken.attributeList[htmlToken.attributeTail].name.headOffset;
                        carryOverRunsLength = tailOffset - carryOverRunsHeadOffset;

                        htmlToken.whole.tail -= numCarryOverRuns;

                        InternalDebug.Assert(numCarryOverRuns != 0);

                        
                    }
                    
                    else
                    {
                        
                        

                         if (state == BuildStateTagAttrName)
                        {
                            if (htmlToken.attributeList[htmlToken.attributeTail].name.head == htmlToken.whole.tail)
                            {
                                AddNullRun(HtmlRunKind.Name);
                            }
                        }
                        else if (state == BuildStateTagAttrValue)
                        {
                            if (htmlToken.attributeList[htmlToken.attributeTail].value.head == htmlToken.whole.tail)
                            {
                                AddNullRun(HtmlRunKind.AttrValue);
                            }
                        }

                        
                        AddSentinelRun();

                        htmlToken.attributeTail++;       
                    }
                }
                else
                {
                    if (state == BuildStateTagName)
                    {
                        if (htmlToken.name.head == htmlToken.whole.tail)
                        {
                            
                            AddNullRun(HtmlRunKind.Name);
                        }
                    }
                    else if (state == BuildStateTagText)
                    {
                        if (htmlToken.unstructured.head == htmlToken.whole.tail)
                        {
                            AddNullRun(HtmlRunKind.TagText);
                        }
                    }

                    
                    AddSentinelRun();
                }
            }

            tokenValid = true;
        }

        

        public int RewindTag()
        {
            InternalDebug.Assert(IncompleteTag);
            InternalDebug.Assert(htmlToken.whole.head == 0);
            InternalDebug.Assert(numCarryOverRuns == 0 || carryOverRunsHeadOffset + carryOverRunsLength == tailOffset);

            
            
            
            
            
            

            if (state >= BuildStateTagAttrName)
            {
                

                if (0 == htmlToken.attributeTail ||
                    htmlToken.attributeList[htmlToken.attributeTail - 1].IsAttrEnd)
                {
                    
                    
                    

                    InternalDebug.Assert(numCarryOverRuns != 0);

                    var deltaRuns = htmlToken.whole.tail;

                    Array.Copy(htmlToken.runList, deltaRuns, htmlToken.runList, 0, numCarryOverRuns);

                    htmlToken.whole.head = 0;
                    htmlToken.whole.headOffset = carryOverRunsHeadOffset;
                    htmlToken.whole.tail = numCarryOverRuns;
                    numCarryOverRuns = 0;

                    htmlToken.attributeList[0] = htmlToken.attributeList[htmlToken.attributeTail];

                    htmlToken.partMinor = (HtmlToken.TagPartMinor)htmlToken.attributeList[0].MajorPart;

                    InternalDebug.Assert(htmlToken.attributeList[0].IsAttrBegin);

                    

                    if (htmlToken.attributeList[0].name.head != -1)
                    {
                        htmlToken.attributeList[0].name.head -= deltaRuns;
                    }

                    if (htmlToken.attributeList[0].localName.head != -1)
                    {
                        htmlToken.attributeList[0].localName.head -= deltaRuns;
                    }

                    if (htmlToken.attributeList[0].value.head != -1)
                    {
                        htmlToken.attributeList[0].value.head -= deltaRuns;
                    }
                }
                else
                {
                    

                    InternalDebug.Assert(numCarryOverRuns == 0);

                    htmlToken.whole.Initialize(0, tailOffset);

                    htmlToken.attributeList[0].nameIndex = htmlToken.attributeList[htmlToken.attributeTail - 1].nameIndex;

                    htmlToken.attributeList[0].partMajor = HtmlToken.AttrPartMajor.Continue;

                    var oldMinor = htmlToken.attributeList[htmlToken.attributeTail - 1].partMinor;

                    if (oldMinor == HtmlToken.AttrPartMinor.BeginName || oldMinor == HtmlToken.AttrPartMinor.ContinueName)
                    {
                        htmlToken.attributeList[0].partMinor = HtmlToken.AttrPartMinor.ContinueName;
                    }
                    else if (oldMinor == HtmlToken.AttrPartMinor.EndNameWithBeginValue ||
                        oldMinor == HtmlToken.AttrPartMinor.CompleteNameWithBeginValue ||
                        oldMinor == HtmlToken.AttrPartMinor.BeginValue ||
                        oldMinor == HtmlToken.AttrPartMinor.ContinueValue)
                    {
                        htmlToken.attributeList[0].partMinor = HtmlToken.AttrPartMinor.ContinueValue;
                    }
                    else
                    {
                        InternalDebug.Assert(oldMinor == HtmlToken.AttrPartMinor.EndName || oldMinor == HtmlToken.AttrPartMinor.CompleteName || oldMinor == HtmlToken.AttrPartMinor.Empty);

                        htmlToken.attributeList[0].partMinor = HtmlToken.AttrPartMinor.Empty;
                    }

                    htmlToken.attributeList[0].IsAttrDeleted = false;
                    htmlToken.attributeList[0].IsAttrValueQuoted = htmlToken.attributeList[htmlToken.attributeTail - 1].IsAttrValueQuoted;
                    htmlToken.attributeList[0].quoteChar = htmlToken.attributeList[htmlToken.attributeTail - 1].quoteChar;

                    if (state == BuildStateTagAttrName)
                    {
                        htmlToken.attributeList[0].name.Initialize(0, tailOffset);
                        htmlToken.attributeList[0].localName.Initialize(0, tailOffset);
                    }
                    else
                    {
                        htmlToken.attributeList[0].name.Reset();
                        htmlToken.attributeList[0].localName.Reset();
                    }

                    if (state == BuildStateTagAttrValue)
                    {
                        htmlToken.attributeList[0].value.Initialize(0, tailOffset);
                    }
                    else
                    {
                        htmlToken.attributeList[0].value.Reset();
                    }

                    htmlToken.partMinor = (HtmlToken.TagPartMinor)htmlToken.attributeList[0].MajorPart;
                }

                InternalDebug.Assert(!htmlToken.attributeList[0].IsAttrEnd);
            }
            else
            {
                InternalDebug.Assert(numCarryOverRuns == 0);

                htmlToken.whole.Initialize(0, tailOffset);

                if (htmlToken.partMinor == HtmlToken.TagPartMinor.BeginName || htmlToken.partMinor == HtmlToken.TagPartMinor.ContinueName)
                {
                    htmlToken.partMinor = HtmlToken.TagPartMinor.ContinueName;
                }
                else
                {
                    InternalDebug.Assert(htmlToken.partMinor == HtmlToken.TagPartMinor.CompleteName ||
                                        htmlToken.partMinor == HtmlToken.TagPartMinor.CompleteNameWithAttributes ||
                                        htmlToken.partMinor == HtmlToken.TagPartMinor.EndName ||
                                        htmlToken.partMinor == HtmlToken.TagPartMinor.EndNameWithAttributes ||
                                        htmlToken.partMinor == HtmlToken.TagPartMinor.EndAttribute ||
                                        htmlToken.partMinor == HtmlToken.TagPartMinor.EndAttributeWithOtherAttributes ||
                                        htmlToken.partMinor == HtmlToken.TagPartMinor.Attributes ||
                                        htmlToken.partMinor == HtmlToken.TagPartMinor.Empty);

                    htmlToken.partMinor = HtmlToken.TagPartMinor.Empty;
                }

                if (htmlToken.attributeList != null)
                {
                    htmlToken.attributeList[0].partMajor = HtmlToken.AttrPartMajor.None;
                    htmlToken.attributeList[0].partMinor = HtmlToken.AttrPartMinor.Empty;
                }
            }

            if (state == BuildStateTagText)
            {
                htmlToken.unstructured.Initialize(0, tailOffset);
            }
            else
            {
                htmlToken.unstructured.Reset();
            }

            if (state == BuildStateTagName)
            {
                htmlToken.name.Initialize(0, tailOffset);
                htmlToken.localName.Initialize(0, tailOffset);
            }
            else
            {
                htmlToken.name.Reset();
                htmlToken.localName.Reset();
            }

            htmlToken.attributeTail = 0;
            htmlToken.currentAttribute = -1;

            htmlToken.partMajor = HtmlToken.TagPartMajor.Continue;

            tokenValid = false;

            return htmlToken.whole.headOffset;
        }

        
#if false
        public HtmlNameIndex LookupName(int nameLength, ref HtmlToken.Fragment fragment)
        {
            if (nameLength > HtmlData.MAX_NAME || nameHashValue < 0)
            {
                
                return HtmlNameIndex.Unknown;
            }

            

            int nameIndex = (int) HtmlData.nameHashTable[nameHashValue];

            if (nameIndex > 0)
            {
                do
                {
                    

                    string name = HtmlData.names[nameIndex].name;

                    if (name.Length == nameLength)
                    {
                        if (fragment.tail == fragment.head + 1)
                        {
                            if (name[0] == ParseSupport.ToLowerCase(this.token.buffer[fragment.headOffset]))
                            {
                                if (nameLength == 1 || this.token.CaseInsensitiveCompareRunEqual(fragment.headOffset + 1, name, 1))
                                {
                                    return (HtmlNameIndex)nameIndex;
                                }
                            }
                        }
                        else if (this.token.CaseInsensitiveCompareEqual(ref fragment, name))
                        {
                            return (HtmlNameIndex)nameIndex;
                        }
                    }

                    nameIndex ++;

                    
                    
                }
                while (HtmlData.names[nameIndex].hash == nameHashValue);
            }

            return HtmlNameIndex.Unknown;
        }
#endif
        public HtmlNameIndex LookupName(int nameLength, Token.LexicalUnit unit)
        {
            InternalDebug.Assert(nameLength >= 0);

            if (nameLength != 0 && nameLength <= HtmlNameData.MAX_NAME)
            {
                var nameHashValue = (short)(((uint)token.CalculateHashLowerCase(unit) ^ HtmlNameData.NAME_HASH_MODIFIER) % HtmlNameData.NAME_HASH_SIZE);

                

                var nameIndex = (int)HtmlNameData.nameHashTable[nameHashValue];

                if (nameIndex > 0)
                {
                    do
                    {
                        

                        var name = HtmlNameData.names[nameIndex].name;

                        if (name.Length == nameLength)
                        {
                            if (token.IsContiguous(unit))
                            {
                                if (name[0] == ParseSupport.ToLowerCase(token.buffer[unit.headOffset]))
                                {
                                    if (nameLength == 1 || token.CaseInsensitiveCompareRunEqual(unit.headOffset + 1, name, 1))
                                    {
                                        return (HtmlNameIndex)nameIndex;
                                    }
                                }
                            }
                            else if (token.CaseInsensitiveCompareEqual(unit, name))
                            {
                                return (HtmlNameIndex)nameIndex;
                            }
                        }

                        nameIndex ++;

                        
                        
                    }
                    while (HtmlNameData.names[nameIndex].hash == nameHashValue);
                }
            }

            return HtmlNameIndex.Unknown;
        }

        

        public static HtmlNameIndex LookupName(char[] nameBuffer, int nameOffset, int nameLength)
        {
            InternalDebug.Assert(nameLength >= 0);

            if (nameLength != 0 && nameLength <= HtmlNameData.MAX_NAME)
            {
                

                var nameHashValue = (short)(((uint)HashCode.CalculateLowerCase(nameBuffer, nameOffset, nameLength) ^ HtmlNameData.NAME_HASH_MODIFIER) % HtmlNameData.NAME_HASH_SIZE);

                var nameIndex = (int)HtmlNameData.nameHashTable[nameHashValue];

                if (nameIndex > 0)
                {
                    do
                    {
                        

                        var name = HtmlNameData.names[nameIndex].name;

                        if (name.Length == nameLength &&
                            name[0] == ParseSupport.ToLowerCase(nameBuffer[nameOffset]))
                        {
                            var i = 0;

                            while (++i < name.Length)
                            {
                                InternalDebug.Assert(!ParseSupport.IsUpperCase(name[i]));

                                if (ParseSupport.ToLowerCase(nameBuffer[nameOffset + i]) != name[i])
                                {
                                    break;
                                }
                            }

                            if (i == name.Length)
                            {
                                return (HtmlNameIndex)nameIndex;
                            }
                        }

                        nameIndex ++;

                        
                        
                    }
                    while (HtmlNameData.names[nameIndex].hash == nameHashValue);
                }
            }

            return HtmlNameIndex.Unknown;
        }

        

        public bool PrepareToAddMoreRuns(int numRuns, int start, HtmlRunKind skippedRunKind)
        {
            return base.PrepareToAddMoreRuns(numRuns, start, (uint)skippedRunKind);
        }

        

        public void AddInvalidRun(int end, HtmlRunKind kind)
        {
            base.AddInvalidRun(end, (uint)kind);
        }

        public void AddNullRun(HtmlRunKind kind)
        {
            base.AddNullRun((uint)kind);
        }

        

        public void AddRun(RunTextType textType, HtmlRunKind kind, int start, int end)
        {
            base.AddRun(RunType.Normal, textType, (uint)kind, start, end, 0);
        }

        

        public void AddLiteralRun(RunTextType textType, HtmlRunKind kind, int start, int end, int literal)
        {
            base.AddRun(RunType.Literal, textType, (uint)kind, start, end, literal);
        }

        

        protected override void Rebase(int deltaOffset)
        {
            
            
            

            htmlToken.unstructured.headOffset += deltaOffset;
            htmlToken.unstructuredPosition.runOffset += deltaOffset;

            htmlToken.name.headOffset += deltaOffset;
            htmlToken.localName.headOffset += deltaOffset;
            htmlToken.namePosition.runOffset += deltaOffset;

            for (var i = 0; i < htmlToken.attributeTail; i++)
            {
                htmlToken.attributeList[i].name.headOffset += deltaOffset;
                htmlToken.attributeList[i].localName.headOffset += deltaOffset;
                htmlToken.attributeList[i].value.headOffset += deltaOffset;
            }

            if (state >= BuildStateTagAttrName)
            {
                htmlToken.attributeList[htmlToken.attributeTail].name.headOffset += deltaOffset;
                htmlToken.attributeList[htmlToken.attributeTail].localName.headOffset += deltaOffset;
                htmlToken.attributeList[htmlToken.attributeTail].value.headOffset += deltaOffset;
            }

            htmlToken.attrNamePosition.runOffset += deltaOffset;
            htmlToken.attrValuePosition.runOffset += deltaOffset;

            carryOverRunsHeadOffset += deltaOffset;

            base.Rebase(deltaOffset);
        }
    }
}

