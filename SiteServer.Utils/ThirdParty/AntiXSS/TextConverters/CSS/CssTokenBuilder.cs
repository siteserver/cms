// ***************************************************************
// <copyright file="CssTokenBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters.Internal.Css
{
    using System;
    using Data.Internal;
    using Html;

    
    

    internal class CssTokenBuilder : TokenBuilder
    {
        
        protected const byte BuildStateEndedCss = BuildStateEnded + 1; 
        protected const byte BuildStatePropertyListStarted = 20;

        protected const byte BuildStateBeforeSelector = 23;
        protected const byte BuildStateSelectorName = 24;
        protected const byte BuildStateEndSelectorName = 25;
        protected const byte BuildStateSelectorClass = 26;
        protected const byte BuildStateEndSelectorClass = 27;

        protected const byte BuildStateBeforeProperty = 43;
        
        protected const byte BuildStatePropertyName = 44;
        protected const byte BuildStateEndPropertyName = 45;
        protected const byte BuildStatePropertyValue = 46;
        protected const byte BuildStateEndPropertyValue = 47;

        protected CssToken cssToken;
        protected int maxProperties;
        protected int maxSelectors;

        
        
        
        
        
        
        

        public CssTokenBuilder(char[] buffer, int maxProperties, int maxSelectors, int maxRuns, bool testBoundaryConditions) :
            base(new CssToken(), buffer, maxRuns, testBoundaryConditions)
        {
            cssToken = (CssToken) base.Token;

            var initialProperties = 16;
            var initialSelectors = 16;

            if (!testBoundaryConditions)
            {
                this.maxProperties = maxProperties;
                this.maxSelectors = maxSelectors;
            }
            else
            {
                initialProperties = 1;
                initialSelectors = 1;
                this.maxProperties = 5;
                this.maxSelectors = 5;
            }

            

            cssToken.propertyList = new CssToken.PropertyEntry[initialProperties];
            cssToken.selectorList = new CssToken.SelectorEntry[initialSelectors];
        }

        
        

        public new CssToken Token => cssToken;


        public bool Incomplete => state >= FirstStarted && state != BuildStateText;


        public override void Reset()
        {
            if (state >= BuildStateEndedCss)
            {
                cssToken.Reset();
            }

            base.Reset();
        }

        

        public void StartRuleSet(int baseOffset, CssTokenId id)
        {
            state = BuildStateBeforeSelector;

            cssToken.tokenId = (TokenId)id;
            cssToken.whole.headOffset = baseOffset;
            tailOffset = baseOffset;
        }

        

        public void EndRuleSet()
        {
            if (state >= BuildStateBeforeProperty)
            {
                EndDeclarations();
            }

            tokenValid = true;
            state = BuildStateEndedCss;
            token.wholePosition.Rewind(token.whole);
        }

        

        public void BuildUniversalSelector()
        {
            StartSelectorName();
            EndSelectorName(0);
        }

        

        public bool CanAddSelector()
        {
            InternalDebug.Assert(state == BuildStateBeforeSelector ||
                state == BuildStateEndSelectorName ||
                state == BuildStateEndSelectorClass );

            return cssToken.selectorTail - cssToken.selectorHead < maxSelectors;
        }

        

        public void StartSelectorName()
        {
            if (cssToken.selectorTail == cssToken.selectorList.Length)
            {
                

                
                InternalDebug.Assert(cssToken.selectorList.Length < maxSelectors);

                int newSize;

                if (maxSelectors / 2 > cssToken.selectorList.Length)
                {
                    newSize = cssToken.selectorList.Length * 2;
                }
                else
                {
                    newSize = maxSelectors;
                }

                var newSelectors = new CssToken.SelectorEntry[newSize];

                Array.Copy(cssToken.selectorList, 0, newSelectors, 0, cssToken.selectorTail);

                cssToken.selectorList = newSelectors;
            }

            InternalDebug.Assert(state == BuildStateBeforeSelector ||
                state == BuildStateEndSelectorName ||
                state == BuildStateEndSelectorClass );

            cssToken.selectorList[cssToken.selectorTail].nameId = HtmlNameIndex.Unknown;
            cssToken.selectorList[cssToken.selectorTail].name.Initialize(cssToken.whole.tail, tailOffset);
            cssToken.selectorList[cssToken.selectorTail].className.Reset();

            state = BuildStateSelectorName;
        }

        

        public void EndSelectorName(int nameLength)
        {
            InternalDebug.Assert(state == BuildStateSelectorName);

            cssToken.selectorList[cssToken.selectorTail].name.tail = cssToken.whole.tail;

            
            cssToken.selectorList[cssToken.selectorTail].nameId = LookupTagName(nameLength, cssToken.selectorList[cssToken.selectorTail].name);

            state = BuildStateEndSelectorName;
        }

        

        public void StartSelectorClass(CssSelectorClassType classType)
        {
            InternalDebug.Assert(state == BuildStateEndSelectorName);

            cssToken.selectorList[cssToken.selectorTail].className.Initialize(cssToken.whole.tail, tailOffset);
            cssToken.selectorList[cssToken.selectorTail].classType = classType;

            state = BuildStateSelectorClass;
        }

        

        public void EndSelectorClass()
        {
            InternalDebug.Assert(state == BuildStateSelectorClass);

            cssToken.selectorList[cssToken.selectorTail].className.tail = cssToken.whole.tail;

            state = BuildStateEndSelectorClass;
        }

        

        public void SetSelectorCombinator(CssSelectorCombinator combinator, bool previous)
        {
            InternalDebug.Assert(state == BuildStateEndSelectorName || state == BuildStateEndSelectorClass);

            var index = cssToken.selectorTail;
            if (previous)
            {
                
                index--;
            }

            InternalDebug.Assert(index >= 0);

            cssToken.selectorList[index].combinator = combinator;
        }

        

        public void EndSimpleSelector()
        {
            InternalDebug.Assert(
                state == BuildStateEndSelectorName ||
                state == BuildStateSelectorClass ||
                state == BuildStateEndSelectorClass);

            cssToken.selectorTail ++;
        }

        

        public void StartDeclarations(int baseOffset)
        {
            InternalDebug.Assert((state == BuildStateInitialized && cssToken.IsEmpty) ||
                state == BuildStateEndSelectorName ||
                state == BuildStateSelectorClass ||
                state == BuildStateEndSelectorClass);

            state = BuildStateBeforeProperty;

            if (cssToken.tokenId == (TokenId)CssTokenId.None)
            {
                cssToken.tokenId = (TokenId)CssTokenId.Declarations;
            }
            cssToken.partMajor = CssToken.PropertyListPartMajor.Begin;
            cssToken.partMinor = CssToken.PropertyListPartMinor.Empty;

            cssToken.whole.headOffset = baseOffset;
            tailOffset = baseOffset;
        }

        

        public bool CanAddProperty()
        {
            InternalDebug.Assert(state == BuildStateBeforeProperty);

            return cssToken.propertyTail - cssToken.propertyHead < maxProperties;
        }

        

        public void StartPropertyName()
        {
            if (cssToken.propertyTail == cssToken.propertyList.Length)
            {
                

                
                InternalDebug.Assert(cssToken.propertyList.Length < maxProperties);

                int newSize;

                if (maxProperties / 2 > cssToken.propertyList.Length)
                {
                    newSize = cssToken.propertyList.Length * 2;
                }
                else
                {
                    newSize = maxProperties;
                }

                var newProperties = new CssToken.PropertyEntry[newSize];

                Array.Copy(cssToken.propertyList, 0, newProperties, 0, cssToken.propertyTail);

                cssToken.propertyList = newProperties;
            }

            InternalDebug.Assert(cssToken.propertyTail < cssToken.propertyList.Length);
            InternalDebug.Assert(state == BuildStateBeforeProperty);

            InternalDebug.Assert(cssToken.partMinor == CssToken.PropertyListPartMinor.Empty ||
                                cssToken.partMinor == CssToken.PropertyListPartMinor.EndProperty ||
                                cssToken.partMinor == CssToken.PropertyListPartMinor.EndPropertyWithOtherProperties ||
                                cssToken.partMinor == CssToken.PropertyListPartMinor.Properties);

            if (cssToken.partMinor == CssToken.PropertyListPartMinor.Empty)
            {
                cssToken.partMinor = CssToken.PropertyListPartMinor.BeginProperty;
            }

            cssToken.propertyList[cssToken.propertyTail].nameId = CssNameIndex.Unknown;
            cssToken.propertyList[cssToken.propertyTail].partMajor = CssToken.PropertyPartMajor.Begin;
            cssToken.propertyList[cssToken.propertyTail].partMinor = CssToken.PropertyPartMinor.BeginName;
            cssToken.propertyList[cssToken.propertyTail].quoteChar = 0;
            cssToken.propertyList[cssToken.propertyTail].name.Initialize(cssToken.whole.tail, tailOffset);
            cssToken.propertyList[cssToken.propertyTail].value.Reset();

            state = BuildStatePropertyName;
        }

        

        public void EndPropertyName(int nameLength)
        {
            InternalDebug.Assert(state == BuildStatePropertyName);

            cssToken.propertyList[cssToken.propertyTail].name.tail = cssToken.whole.tail;

            cssToken.propertyList[cssToken.propertyTail].partMinor |= CssToken.PropertyPartMinor.EndName;

            if (cssToken.propertyList[cssToken.propertyTail].IsPropertyBegin)
            {
                cssToken.propertyList[cssToken.propertyTail].nameId = LookupName(nameLength, cssToken.propertyList[cssToken.propertyTail].name);
            }

            state = BuildStateEndPropertyName;
        }

        

        public void StartPropertyValue()
        {
            InternalDebug.Assert(state == BuildStateEndPropertyName);

            cssToken.propertyList[cssToken.propertyTail].value.Initialize(cssToken.whole.tail, tailOffset);

            cssToken.propertyList[cssToken.propertyTail].partMinor |= CssToken.PropertyPartMinor.BeginValue;

            state = BuildStatePropertyValue;
        }

        

        public void SetPropertyValueQuote(char ch)
        {
            InternalDebug.Assert(state == BuildStatePropertyValue);
            InternalDebug.Assert(ParseSupport.QuoteCharacter(ParseSupport.GetCharClass(ch)));

            cssToken.propertyList[cssToken.propertyTail].IsPropertyValueQuoted = true;
            cssToken.propertyList[cssToken.propertyTail].quoteChar = (byte) ch;
        }

        

        public void EndPropertyValue()
        {
            InternalDebug.Assert(state == BuildStatePropertyValue);

            cssToken.propertyList[cssToken.propertyTail].value.tail = cssToken.whole.tail;

            cssToken.propertyList[cssToken.propertyTail].partMinor |= CssToken.PropertyPartMinor.EndValue;

            state = BuildStateEndPropertyValue;
        }

        

        public void EndProperty()
        {
            InternalDebug.Assert(state == BuildStateEndPropertyName || state == BuildStateEndPropertyValue);

            cssToken.propertyList[cssToken.propertyTail].partMajor |= CssToken.PropertyPartMajor.End;

            cssToken.propertyTail ++;

            if (cssToken.propertyTail < cssToken.propertyList.Length)
            {
                

                cssToken.propertyList[cssToken.propertyTail].partMajor = CssToken.PropertyPartMajor.None;
                cssToken.propertyList[cssToken.propertyTail].partMinor = CssToken.PropertyPartMinor.Empty;
            }

            if (cssToken.partMinor == CssToken.PropertyListPartMinor.BeginProperty)
            {
                cssToken.partMinor = CssToken.PropertyListPartMinor.Properties;
            }
            else if (cssToken.partMinor == CssToken.PropertyListPartMinor.ContinueProperty)
            {
                cssToken.partMinor = CssToken.PropertyListPartMinor.EndProperty;
            }
            else
            {
                InternalDebug.Assert(cssToken.partMinor == CssToken.PropertyListPartMinor.EndProperty ||
                                    cssToken.partMinor == CssToken.PropertyListPartMinor.EndPropertyWithOtherProperties ||
                                    cssToken.partMinor == CssToken.PropertyListPartMinor.Properties);

                cssToken.partMinor |= CssToken.PropertyListPartMinor.Properties;
            }

            state = BuildStateBeforeProperty;
        }

        

        public void EndDeclarations()
        {
            if (state != BuildStatePropertyListStarted)
            {
                

                if (state == BuildStatePropertyName)
                {
                    cssToken.propertyList[cssToken.propertyTail].name.tail = cssToken.whole.tail;
                }
                else if (state == BuildStatePropertyValue)
                {
                    cssToken.propertyList[cssToken.propertyTail].value.tail = cssToken.whole.tail;
                }

            }

            if (state == BuildStatePropertyName)
            {
                EndPropertyName(0);
            }
            else if (state == BuildStatePropertyValue)
            {
                EndPropertyValue();
            }

            if (state == BuildStateEndPropertyName || state == BuildStateEndPropertyValue)
            {
                EndProperty();
            }

            InternalDebug.Assert(state == BuildStateBeforeProperty || state == BuildStatePropertyListStarted);

            state = BuildStateBeforeProperty;
            cssToken.partMajor |= CssToken.PropertyListPartMajor.End;

            tokenValid = true;
        }

        

        public bool PrepareAndAddRun(CssRunKind cssRunKind, int start, int end)
        {
            if (end != start)
            {
                if (!PrepareToAddMoreRuns(1))
                {
                    return false;
                }

                AddRun(
                    cssRunKind == CssRunKind.Invalid ? RunType.Invalid : RunType.Normal,
                    cssRunKind == CssRunKind.Space ? RunTextType.Space : RunTextType.NonSpace,
                    (uint)cssRunKind,
                    start,
                    end,
                    0);
            }

            return true;
        }

        

        public bool PrepareAndAddInvalidRun(CssRunKind cssRunKind, int end)
        {
            if (!PrepareToAddMoreRuns(1))
            {
                return false;
            }

            AddInvalidRun(end, (uint)cssRunKind);

            return true;
        }

        

        public bool PrepareAndAddLiteralRun(CssRunKind cssRunKind, int start, int end, int value)
        {
            if (end != start)
            {
                if (!PrepareToAddMoreRuns(1))
                {
                    return false;
                }

                AddRun(
                    RunType.Literal,
                    RunTextType.NonSpace,
                    (uint)cssRunKind,
                    start,
                    end,
                    value);
            }

            return true;
        }

        

        public void InvalidateLastValidRun(CssRunKind kind)
        {
            var tail = token.whole.tail;
            InternalDebug.Assert(tail > 0);

            do
            {
                tail--;

                var tailEntry = token.runList[tail];
                if (tailEntry.Type != RunType.Invalid)
                {
                    if ((uint)kind == (uint)tailEntry.Kind)
                    {
                        token.runList[tail].Initialize(RunType.Invalid, tailEntry.TextType, tailEntry.Kind, tailEntry.Length, tailEntry.Value);
                    }
                    break;
                }
            }
            while (tail > 0);
        }

        

        public void MarkPropertyAsDeleted()
        {
            cssToken.propertyList[cssToken.propertyTail].IsPropertyDeleted = true;
        }

        

        public CssTokenId MakeEmptyToken(CssTokenId tokenId)
        {
            return (CssTokenId)base.MakeEmptyToken((TokenId)tokenId);
        }

        

        public CssNameIndex LookupName(int nameLength, Token.Fragment fragment)
        {
            if (nameLength > CssData.MAX_NAME)
            {
                
                return CssNameIndex.Unknown;
            }

            var nameHashValue = (short)(((uint)token.CalculateHashLowerCase(fragment) ^ CssData.NAME_HASH_MODIFIER) % CssData.NAME_HASH_SIZE);

            

            var nameIndex = (int) CssData.nameHashTable[nameHashValue];

            if (nameIndex > 0)
            {
                do
                {
                    

                    var name = CssData.names[nameIndex].name;

                    if (name.Length == nameLength)
                    {
                        if (fragment.tail == fragment.head + 1)
                        {
                            if (name[0] == ParseSupport.ToLowerCase(token.buffer[fragment.headOffset]))
                            {
                                if (nameLength == 1 || token.CaseInsensitiveCompareRunEqual(fragment.headOffset + 1, name, 1))
                                {
                                    return (CssNameIndex)nameIndex;
                                }
                            }
                        }
                        else if (token.CaseInsensitiveCompareEqual(ref fragment, name))
                        {
                            return (CssNameIndex)nameIndex;
                        }
                    }

                    nameIndex ++;

                    
                    
                }
                while (CssData.names[nameIndex].hash == nameHashValue);
            }

            return CssNameIndex.Unknown;
        }

        

        public HtmlNameIndex LookupTagName(int nameLength, Token.Fragment fragment)
        {
            if (nameLength > HtmlNameData.MAX_NAME)
            {
                
                return HtmlNameIndex.Unknown;
            }

            var nameHashValue = (short)(((uint)token.CalculateHashLowerCase(fragment) ^ HtmlNameData.NAME_HASH_MODIFIER) % HtmlNameData.NAME_HASH_SIZE);

            

            var nameIndex = (int)HtmlNameData.nameHashTable[nameHashValue];

            if (nameIndex > 0)
            {
                do
                {
                    

                    var name = HtmlNameData.names[nameIndex].name;

                    if (name.Length == nameLength)
                    {
                        if (fragment.tail == fragment.head + 1)
                        {
                            if (name[0] == ParseSupport.ToLowerCase(token.buffer[fragment.headOffset]))
                            {
                                if (nameLength == 1 || token.CaseInsensitiveCompareRunEqual(fragment.headOffset + 1, name, 1))
                                {
                                    return (HtmlNameIndex)nameIndex;
                                }
                            }
                        }
                        else if (token.CaseInsensitiveCompareEqual(ref fragment, name))
                        {
                            return (HtmlNameIndex)nameIndex;
                        }
                    }

                    nameIndex ++;

                    
                    
                }
                while (HtmlNameData.names[nameIndex].hash == nameHashValue);
            }

            return HtmlNameIndex.Unknown;
        }
    }
}

