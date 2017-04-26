// ***************************************************************
// <copyright file="CssParser.cs" company="Microsoft">
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


    internal enum CssParseMode
    {
        StyleAttribute,
        StyleTag,
        External
    }

    
    

    internal class CssParser : IDisposable
    {
        

        private ConverterInput input;
        private bool endOfFile;

        private CssParseMode parseMode = CssParseMode.StyleAttribute;
        private bool isInvalid;

        private char[] parseBuffer;
        private int parseStart;
        private int parseCurrent;
        private int parseEnd;

        private int ruleDepth;

        
        protected CssTokenBuilder tokenBuilder;
        private CssToken token;

        private static readonly string[] SafeTermFunctions = { "rgb", "counter" };
        private static readonly string[] SafePseudoFunctions = { "lang" };

        internal const int MaxCssLength = 512 * 1024;

        
        
        public CssParser(ConverterInput input, int maxRuns, bool testBoundaryConditions)
        {
            this.input = input;

            tokenBuilder = new CssTokenBuilder(null, 256, 256, maxRuns, testBoundaryConditions);

            token = tokenBuilder.Token;
        }

        
        
        public CssToken Token => token;


        void IDisposable.Dispose()
        {
            if (input != null /*&& this.input is IDisposable*/)
            {
                ((IDisposable)input).Dispose();
            }

            input = null;
            parseBuffer = null;
            token = null;

            GC.SuppressFinalize(this);
        }

        
        
        public void Reset()
        {
            endOfFile = false;

            parseBuffer = null;
            parseStart = 0;
            parseCurrent = 0;
            parseEnd = 0;
            ruleDepth = 0;
        }

        
        
        public void SetParseMode(CssParseMode parseMode)
        {
            this.parseMode = parseMode;
        }

        
        
        
        public CssTokenId Parse()
        {
            if (endOfFile)
            {
                return CssTokenId.EndOfFile;
            }

            char ch;
            CharClass charClass;
            tokenBuilder.Reset();

            InternalDebug.Assert(this.parseCurrent >= parseStart);

            var parseBuffer = this.parseBuffer;
            var parseCurrent = this.parseCurrent;
            var parseEnd = this.parseEnd;

            if (parseCurrent >= parseEnd)
            {
                var readMore = input.ReadMore(ref this.parseBuffer, ref parseStart, ref this.parseCurrent, ref this.parseEnd);
                InternalDebug.Assert(readMore);

                if (this.parseEnd == 0)
                {
                    
                    return CssTokenId.EndOfFile;
                }

                
                

                tokenBuilder.BufferChanged(this.parseBuffer, parseStart);

                parseBuffer = this.parseBuffer;
                parseCurrent = this.parseCurrent;
                parseEnd = this.parseEnd;
            }

            InternalDebug.Assert(parseCurrent < parseEnd);

            
            ch = parseBuffer[parseCurrent];
            charClass = ParseSupport.GetCharClass(ch);

            var start = parseCurrent;

            if (parseMode == CssParseMode.StyleTag)
            {
                ScanStyleSheet(ch, ref charClass, ref parseCurrent);
                if (start >= parseCurrent)
                {
                    
                    InternalDebug.Assert(false);

                    
                    tokenBuilder.Reset();

                    return CssTokenId.EndOfFile;
                }

                if (tokenBuilder.Incomplete)
                {
                    tokenBuilder.EndRuleSet();
                }
            }
            else
            {
                InternalDebug.Assert(parseMode == CssParseMode.StyleAttribute);

                ScanDeclarations(ch, ref charClass, ref parseCurrent);
                if (parseCurrent < parseEnd)
                {
                    
                    endOfFile = true;

                    tokenBuilder.Reset();
                    return CssTokenId.EndOfFile;
                }

                if (tokenBuilder.Incomplete)
                {
                    tokenBuilder.EndDeclarations();
                }
            }

            endOfFile = (parseCurrent == parseEnd);

            this.parseCurrent = parseCurrent;
            return token.TokenId;
        }

        
        private char ScanStyleSheet(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            var parseEnd = this.parseEnd;
            var parseBuffer = this.parseBuffer;
            int start;

            do
            {
                start = parseCurrent;

                ch = ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
                if (parseCurrent == parseEnd)
                {
                    return ch;
                }

                if (IsNameStartCharacter(ch, charClass, parseCurrent) || ch == '*' ||
                    ch == '.' || ch == ':' || ch == '#' || ch == '[')
                {
                    ch = ScanRuleSet(ch, ref charClass, ref parseCurrent);
                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }

                    if (!isInvalid)
                    {
                        
                        return ch;
                    }
                }
                else if (ch == '@')
                {
                    ch = ScanAtRule(ch, ref charClass, ref parseCurrent);
                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }

                    if (!isInvalid)
                    {
                        
                        return ch;
                    }
                }
                else if (ch == '/' && parseCurrent < parseEnd && parseBuffer[parseCurrent + 1] == '*')
                {
                    ch = ScanComment(ch, ref charClass, ref parseCurrent);
                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }
                }
                else if (ch == '<')
                {
                    ch = ScanCdo(ch, ref charClass, ref parseCurrent);
                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }
                }
                else if (ch == '-')
                {
                    ch = ScanCdc(ch, ref charClass, ref parseCurrent);
                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }
                }
                else
                {
                    isInvalid = true;
                }

                if (isInvalid)
                {
                    isInvalid = false;

                    tokenBuilder.Reset();

                    ch = SkipToNextRule(ch, ref charClass, ref parseCurrent);
                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }
                }
            }
            while (start < parseCurrent);

            return ch;
        }

        
        private char ScanCdo(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            InternalDebug.Assert(ch == '<');
            parseCurrent++;

            if (parseCurrent + 3 >= parseEnd)
            {
                parseCurrent = parseEnd;
                return ch;
            }

            if (parseBuffer[parseCurrent++] != '!' ||
                parseBuffer[parseCurrent++] != '-' ||
                parseBuffer[parseCurrent++] != '-')
            {
                return SkipToNextRule(ch, ref charClass, ref parseCurrent);
            }

            ch = parseBuffer[parseCurrent];
            charClass = ParseSupport.GetCharClass(ch);
            return ch;
        }

        
        private char ScanCdc(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            InternalDebug.Assert(ch == '-');
            parseCurrent++;

            if (parseCurrent + 2 >= parseEnd)
            {
                parseCurrent = parseEnd;
                return ch;
            }

            if (parseBuffer[parseCurrent++] != '-' ||
                parseBuffer[parseCurrent++] != '>')
            {
                return SkipToNextRule(ch, ref charClass, ref parseCurrent);
            }

            ch = parseBuffer[parseCurrent];
            charClass = ParseSupport.GetCharClass(ch);
            return ch;
        }

        
        private char ScanAtRule(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            var parseEnd = this.parseEnd;
            var parseBuffer = this.parseBuffer;

            var selectorNameRunStart = parseCurrent;

            InternalDebug.Assert(ch == '@');

            ch = parseBuffer[++parseCurrent];
            charClass = ParseSupport.GetCharClass(ch);

            if (!IsNameStartCharacter(ch, charClass, parseCurrent))
            {
                isInvalid = true;
                return ch;
            }

            tokenBuilder.StartRuleSet(selectorNameRunStart, CssTokenId.AtRule);

            if (!tokenBuilder.CanAddSelector())
            {
                
                parseCurrent = parseEnd;
                return ch;
            }

            tokenBuilder.StartSelectorName();

            int nameLength;

            PrepareAndAddRun(CssRunKind.AtRuleName, selectorNameRunStart, ref parseCurrent);
            if (parseCurrent == parseEnd)
            {
                return ch;
            }

            ch = ScanName(CssRunKind.AtRuleName, ch, ref charClass, ref parseCurrent, out nameLength);

            InternalDebug.Assert(parseCurrent > selectorNameRunStart);

            tokenBuilder.EndSelectorName(nameLength);

            if (parseCurrent == parseEnd)
            {
                return ch;
            }

            if (IsNameEqual("page", selectorNameRunStart + 1, parseCurrent - selectorNameRunStart - 1))
            {
                ch = ScanPageSelector(ch, ref charClass, ref parseCurrent);
                if (parseCurrent == parseEnd)
                {
                    return ch;
                }
            }
            else if (!IsNameEqual("font-face", selectorNameRunStart + 1, parseCurrent - selectorNameRunStart - 1))
            {
                isInvalid = true;
                return ch;
            }

            tokenBuilder.EndSimpleSelector();

            ch = ScanDeclarationBlock(ch, ref charClass, ref parseCurrent);
            if (parseCurrent == parseEnd)
            {
                return ch;
            }

            return ch;
        }

        
        private char ScanPageSelector(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            ch = ScanWhitespace(ch, ref charClass, ref parseCurrent, false);
            if (parseCurrent == parseEnd)
            {
                return ch;
            }

            if (IsNameStartCharacter(ch, charClass, parseCurrent))
            {
                tokenBuilder.EndSimpleSelector();

                tokenBuilder.StartSelectorName();

                int nameLength;

                ch = ScanName(CssRunKind.PageIdent, ch, ref charClass, ref parseCurrent, out nameLength);

                tokenBuilder.EndSelectorName(nameLength);

                if (parseCurrent == parseEnd)
                {
                    return ch;
                }

                tokenBuilder.SetSelectorCombinator(CssSelectorCombinator.Descendant, false);
            }

            if (ch == ':')
            {
                ch = parseBuffer[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);

                PrepareAndAddRun(CssRunKind.PagePseudoStart, parseCurrent - 1, ref parseCurrent);
                if (parseCurrent == parseEnd)
                {
                    return ch;
                }

                if (!IsNameStartCharacter(ch, charClass, parseCurrent))
                {
                    tokenBuilder.InvalidateLastValidRun(CssRunKind.SelectorPseudoStart);
                    return ch;
                }

                tokenBuilder.StartSelectorClass(CssSelectorClassType.Pseudo);

                int nameLength;

                ch = ScanName(CssRunKind.PagePseudo, ch, ref charClass, ref parseCurrent, out nameLength);

                tokenBuilder.EndSelectorClass();

                if (parseCurrent == parseEnd)
                {
                    return ch;
                }
            }

            ch = ScanWhitespace(ch, ref charClass, ref parseCurrent, false);
            if (parseCurrent == parseEnd)
            {
                return ch;
            }

            return ch;
        }

        
        private char ScanRuleSet(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            tokenBuilder.StartRuleSet(parseCurrent, CssTokenId.RuleSet);

            
            ch = ScanSelectors(ch, ref charClass, ref parseCurrent);
            if (parseCurrent == parseEnd || isInvalid)
            {
                return ch;
            }

            ch = ScanDeclarationBlock(ch, ref charClass, ref parseCurrent);
            if (parseCurrent == parseEnd)
            {
                return ch;
            }

            return ch;
        }

        
        private char ScanDeclarationBlock(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            ch = ScanWhitespace(ch, ref charClass, ref parseCurrent, false);
            if (parseCurrent == parseEnd)
            {
                return ch;
            }

            if (ch != '{')
            {
                isInvalid = true;
                return ch;
            }

            ruleDepth++;

            ch = parseBuffer[++parseCurrent];
            charClass = ParseSupport.GetCharClass(ch);

            PrepareAndAddRun(CssRunKind.Delimiter, parseCurrent - 1, ref parseCurrent);
            if (parseCurrent == parseEnd)
            {
                return ch;
            }

            ch = ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
            if (parseCurrent == parseEnd)
            {
                return ch;
            }

            ch = ScanDeclarations(ch, ref charClass, ref parseCurrent);
            if (parseCurrent == parseEnd)
            {
                return ch;
            }

            if (ch != '}')
            {
                isInvalid = true;
                return ch;
            }

            
            InternalDebug.Assert(ruleDepth > 0);
            ruleDepth--;

            ch = parseBuffer[++parseCurrent];
            charClass = ParseSupport.GetCharClass(ch);

            PrepareAndAddRun(CssRunKind.Delimiter, parseCurrent - 1, ref parseCurrent);
            if (parseCurrent == parseEnd)
            {
                return ch;
            }

            return ch;
        }

        
        private char ScanSelectors(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            var parseEnd = this.parseEnd;
            var parseBuffer = this.parseBuffer;

            var start = parseCurrent;
            ch = ScanSimpleSelector(ch, ref charClass, ref parseCurrent);
            if (parseCurrent == parseEnd || isInvalid)
            {
                return ch;
            }

            while (start < parseCurrent)
            {
                var combinator = CssSelectorCombinator.None;

                var cleanupSpace = false;
                var cleanupCombinator = false;

                start = parseCurrent;
                ch = ScanWhitespace(ch, ref charClass, ref parseCurrent, false);
                if (parseCurrent == parseEnd)
                {
                    return ch;
                }
                if (start < parseCurrent)
                {
                    
                    cleanupSpace = true;

                    combinator = CssSelectorCombinator.Descendant;
                }

                
                if (ch == '+' || ch == '>' || ch == ',')
                {
                    combinator = (ch == '+') ? CssSelectorCombinator.Adjacent :
                        ((ch == '>') ? CssSelectorCombinator.Child : CssSelectorCombinator.None);

                    ch = parseBuffer[++parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);
                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }

                    
                    PrepareAndAddRun(CssRunKind.SelectorCombinatorOrComma, parseCurrent - 1, ref parseCurrent);
                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }

                    cleanupCombinator = true;

                    ch = ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }
                }
                else if (start == parseCurrent)
                {
                    break;
                }

                start = parseCurrent;
                ch = ScanSimpleSelector(ch, ref charClass, ref parseCurrent);
                if (start == parseCurrent)
                {
                    if (cleanupCombinator)
                    {
                        tokenBuilder.InvalidateLastValidRun(CssRunKind.SelectorCombinatorOrComma);
                    }
                    if (cleanupSpace)
                    {
                        tokenBuilder.InvalidateLastValidRun(CssRunKind.Space);
                    }
                    break;
                }
                else
                {
                    if (isInvalid)
                    {
                        return ch;
                    }
                    tokenBuilder.SetSelectorCombinator(combinator, true);
                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }
                }
            }

            return ch;
        }

        
        private char ScanSimpleSelector(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            var parseEnd = this.parseEnd;
            var parseBuffer = this.parseBuffer;

            if (ch == '.' || ch == ':' || ch == '#' || ch == '[')
            {
                if (!tokenBuilder.CanAddSelector())
                {
                    
                    parseCurrent = parseEnd;
                    return ch;
                }

                tokenBuilder.BuildUniversalSelector();
            }
            else if (IsNameStartCharacter(ch, charClass, parseCurrent) || ch == '*')
            {
                if (!tokenBuilder.CanAddSelector())
                {
                    
                    parseCurrent = parseEnd;
                    return ch;
                }

                tokenBuilder.StartSelectorName();

                int nameLength;

                if (ch == '*')
                {
                    nameLength = 1;

                    ch = parseBuffer[++parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);

                    PrepareAndAddRun(CssRunKind.SelectorName, parseCurrent - 1, ref parseCurrent);
                }
                else
                {
                    ch = ScanName(CssRunKind.SelectorName, ch, ref charClass, ref parseCurrent, out nameLength);
                }

                tokenBuilder.EndSelectorName(nameLength);
                if (parseCurrent == parseEnd)
                {
                    return ch;
                }
            }
            else
            {
                return ch;
            }

            ch = ScanSelectorSuffix(ch, ref charClass, ref parseCurrent);
            tokenBuilder.EndSimpleSelector();

            return ch;
        }

        
        private char ScanSelectorSuffix(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            if (ch == '[')
            {
                tokenBuilder.StartSelectorClass(CssSelectorClassType.Attrib);
                ch = ScanSelectorAttrib(ch, ref charClass, ref parseCurrent);
                tokenBuilder.EndSelectorClass();

                return ch;
            }

            var parseEnd = this.parseEnd;
            var parseBuffer = this.parseBuffer;

            if (ch == ':')
            {
                ch = parseBuffer[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);

                PrepareAndAddRun(CssRunKind.SelectorPseudoStart, parseCurrent - 1, ref parseCurrent);
                if (parseCurrent == parseEnd)
                {
                    return ch;
                }

                tokenBuilder.StartSelectorClass(CssSelectorClassType.Pseudo);
                ch = ScanSelectorPseudo(ch, ref charClass, ref parseCurrent);
                tokenBuilder.EndSelectorClass();

                return ch;
            }

            if (ch == '.' || ch == '#')
            {
                var isClass = ch == '.';

                ch = parseBuffer[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);

                if (IsNameCharacter(ch, charClass, parseCurrent) && (!isClass || IsNameStartCharacter(ch, charClass, parseCurrent)))
                {
                    PrepareAndAddRun(isClass ? CssRunKind.SelectorClassStart : CssRunKind.SelectorHashStart, parseCurrent - 1, ref parseCurrent);

                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }

                    tokenBuilder.StartSelectorClass(isClass ? CssSelectorClassType.Regular : CssSelectorClassType.Hash);

                    int nameLength;

                    ch = ScanName(isClass ? CssRunKind.SelectorClass : CssRunKind.SelectorHash, ch, ref charClass, ref parseCurrent, out nameLength);

                    tokenBuilder.EndSelectorClass();

                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }
                }
                else
                {
                    PrepareAndAddInvalidRun(CssRunKind.FunctionStart, ref parseCurrent);
                }
            }

            return ch;
        }

        
        private char ScanSelectorPseudo(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            var parseEnd = this.parseEnd;
            var parseBuffer = this.parseBuffer;

            if (!IsNameStartCharacter(ch, charClass, parseCurrent))
            {
                tokenBuilder.InvalidateLastValidRun(CssRunKind.SelectorPseudoStart);
                return ch;
            }

            var constructRunStart = parseCurrent;

            int nameLength;

            ch = ScanName(CssRunKind.SelectorPseudo, ch, ref charClass, ref parseCurrent, out nameLength);

            if (parseCurrent == parseEnd)
            {
                return ch;
            }

            if (ch == '(')
            {
                if (!IsSafeIdentifier(SafePseudoFunctions, constructRunStart, parseCurrent))
                {
                    return ch;
                }

                ch = parseBuffer[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
                if (parseCurrent == parseEnd)
                {
                    
                    return ch;
                }

                
                PrepareAndAddRun(CssRunKind.FunctionStart, parseCurrent - 1, ref parseCurrent);
                if (parseCurrent == parseEnd)
                {
                    
                    return ch;
                }

                ch = ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
                if (parseCurrent == parseEnd)
                {
                    
                    return ch;
                }

                if (!IsNameStartCharacter(ch, charClass, parseCurrent))
                {
                    

                    isInvalid = true;
                    return ch;
                }

                ch = ScanName(CssRunKind.SelectorPseudoArg, ch, ref charClass, ref parseCurrent, out nameLength);

                if (parseCurrent == parseEnd)
                {
                    
                    return ch;
                }

                ch = ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
                if (parseCurrent == parseEnd)
                {
                    
                    return ch;
                }

                if (ch != ')')
                {
                    isInvalid = true;
                    return ch;
                }

                ch = parseBuffer[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);

                
                PrepareAndAddRun(CssRunKind.FunctionEnd, parseCurrent - 1, ref parseCurrent);
                if (parseCurrent == parseEnd)
                {
                    return ch;
                }
            }

            return ch;
        }

        
        private char ScanSelectorAttrib(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            var parseEnd = this.parseEnd;
            var parseBuffer = this.parseBuffer;

            InternalDebug.Assert(ch == '[');

            ch = parseBuffer[++parseCurrent];
            charClass = ParseSupport.GetCharClass(ch);

            PrepareAndAddRun(CssRunKind.SelectorAttribStart, parseCurrent - 1, ref parseCurrent);
            if (parseCurrent == parseEnd)
            {
                return ch;
            }

            ch = ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
            if (parseCurrent == parseEnd)
            {
                return ch;
            }

            if (!IsNameStartCharacter(ch, charClass, parseCurrent))
            {
                isInvalid = true;
                return ch;
            }

            int nameLength;

            ch = ScanName(CssRunKind.SelectorAttribName, ch, ref charClass, ref parseCurrent, out nameLength);

            if (parseCurrent == parseEnd)
            {
                return ch;
            }

            ch = ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
            if (parseCurrent == parseEnd)
            {
                return ch;
            }

            var constructRunStart = parseCurrent;
            if (ch == '=')
            {
                ch = parseBuffer[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);

                PrepareAndAddRun(CssRunKind.SelectorAttribEquals, parseCurrent - 1, ref parseCurrent);
                if (parseCurrent == parseEnd)
                {
                    return ch;
                }
            }
            else if ((ch == '~' || ch == '|') && parseBuffer[parseCurrent + 1] == '=')
            {
                parseCurrent += 2;

                PrepareAndAddRun(
                    ch == '~' ? CssRunKind.SelectorAttribIncludes : CssRunKind.SelectorAttribDashmatch,
                    parseCurrent - 2,
                    ref parseCurrent);

                if (parseCurrent == parseEnd)
                {
                    return ch;
                }

                ch = parseBuffer[parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
            }

            if (constructRunStart < parseCurrent)
            {
                
                ch = ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
                if (parseCurrent == parseEnd)
                {
                    return ch;
                }

                if (IsNameStartCharacter(ch, charClass, parseCurrent))
                {
                    constructRunStart = parseCurrent;
                    ch = ScanName(CssRunKind.SelectorAttribIdentifier, ch, ref charClass, ref parseCurrent, out nameLength);

                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }
                }
                else if (ch == '"' || ch == '\'')
                {
                    constructRunStart = parseCurrent;
                    ch = ScanString(ch, ref charClass, ref parseCurrent, false);

                    
                    PrepareAndAddRun(CssRunKind.SelectorAttribString, constructRunStart, ref parseCurrent);

                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }
                }

                ch = ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
                if (parseCurrent == parseEnd)
                {
                    return ch;
                }
            }

            if (ch != ']')
            {
                isInvalid = true;
                return ch;
            }

            ch = parseBuffer[++parseCurrent];
            charClass = ParseSupport.GetCharClass(ch);

            PrepareAndAddRun(CssRunKind.SelectorAttribEnd, parseCurrent - 1, ref parseCurrent);
            if (parseCurrent == parseEnd)
            {
                return ch;
            }

            return ch;
        }

        
        private char ScanDeclarations(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            var parseEnd = this.parseEnd;
            var parseBuffer = this.parseBuffer;

            tokenBuilder.StartDeclarations(parseCurrent);
            int start;

            do
            {
                start = parseCurrent;

                ch = ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
                if (parseCurrent == parseEnd)
                {
                    return ch;
                }

                if (IsNameStartCharacter(ch, charClass, parseCurrent))
                {
                    if (!tokenBuilder.CanAddProperty())
                    {
                        
                        parseCurrent = parseEnd;
                        return ch;
                    }

                    tokenBuilder.StartPropertyName();

                    int nameLength;

                    ch = ScanName(CssRunKind.PropertyName, ch, ref charClass, ref parseCurrent, out nameLength);

                    tokenBuilder.EndPropertyName(nameLength);

                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }

                    ch = ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }

                    if (ch != ':')
                    {
                        tokenBuilder.MarkPropertyAsDeleted();
                        return ch;
                    }

                    ch = parseBuffer[++parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);

                    PrepareAndAddRun(CssRunKind.PropertyColon, parseCurrent - 1, ref parseCurrent);
                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }

                    ch = ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }

                    tokenBuilder.StartPropertyValue();

                    ch = ScanPropertyValue(ch, ref charClass, ref parseCurrent);
                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }

                    tokenBuilder.EndPropertyValue();
                    tokenBuilder.EndProperty();

                    ch = ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }
                }

                if (ch != ';')
                {
                    tokenBuilder.EndDeclarations();
                    return ch;
                }

                ch = parseBuffer[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);

                PrepareAndAddRun(CssRunKind.Delimiter, parseCurrent - 1, ref parseCurrent);
                if (parseCurrent == parseEnd)
                {
                    return ch;
                }
            }
            while (start < parseCurrent);

            return ch;
        }

        
        private char ScanPropertyValue(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            var parseEnd = this.parseEnd;
            var parseBuffer = this.parseBuffer;

            ch = ScanExpr(ch, ref charClass, ref parseCurrent, 0);
            if (parseCurrent == parseEnd)
            {
                return ch;
            }

            if (ch == '!')
            {
                ch = parseBuffer[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
                if (parseCurrent == parseEnd)
                {
                    tokenBuilder.MarkPropertyAsDeleted();
                    return ch;
                }

                
                PrepareAndAddRun(CssRunKind.ImportantStart, parseCurrent - 1, ref parseCurrent);
                if (parseCurrent == parseEnd)
                {
                    return ch;
                }

                ch = ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
                if (parseCurrent == parseEnd)
                {
                    tokenBuilder.MarkPropertyAsDeleted();
                    return ch;
                }

                if (IsNameStartCharacter(ch, charClass, parseCurrent))
                {
                    var identStart = parseCurrent;
                    int nameLength;

                    ch = ScanName(CssRunKind.Important, ch, ref charClass, ref parseCurrent, out nameLength);

                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }

                    if (!IsNameEqual("important", identStart, parseCurrent - identStart))
                    {
                        tokenBuilder.MarkPropertyAsDeleted();
                        return ch;
                    }
                }
                else
                {
                    tokenBuilder.MarkPropertyAsDeleted();
                    return ch;
                }
            }

            return ch;
        }

        
        private char ScanExpr(char ch, ref CharClass charClass, ref int parseCurrent, int level)
        {
            var parseEnd = this.parseEnd;
            var parseBuffer = this.parseBuffer;

            var start = parseCurrent;
            ch = ScanTerm(ch, ref charClass, ref parseCurrent, level);
            if (parseCurrent == parseEnd)
            {
                return ch;
            }

            while (start < parseCurrent)
            {
                var cleanupSpace = false;
                var cleanupOperator = false;

                start = parseCurrent;
                ch = ScanWhitespace(ch, ref charClass, ref parseCurrent, false);
                if (parseCurrent == parseEnd)
                {
                    return ch;
                }
                if (start < parseCurrent)
                {
                    
                    cleanupSpace = true;
                }

                
                if (ch == '/' || ch == ',')
                {
                    ch = parseBuffer[++parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);
                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }

                    
                    PrepareAndAddRun(CssRunKind.Operator, parseCurrent - 1, ref parseCurrent);
                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }

                    cleanupOperator = true;

                    ch = ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }
                }
                else if (start == parseCurrent)
                {
                    break;
                }

                start = parseCurrent;
                ch = ScanTerm(ch, ref charClass, ref parseCurrent, level);
                if (parseCurrent == parseEnd)
                {
                    return ch;
                }

                if (start == parseCurrent)
                {
                    if (cleanupOperator)
                    {
                        tokenBuilder.InvalidateLastValidRun(CssRunKind.Operator);
                    }
                    if (cleanupSpace)
                    {
                        tokenBuilder.InvalidateLastValidRun(CssRunKind.Space);
                    }
                    break;
                }
            }

            return ch;
        }

        
        private char ScanTerm(char ch, ref CharClass charClass, ref int parseCurrent, int level)
        {
            var parseEnd = this.parseEnd;
            var parseBuffer = this.parseBuffer;
            int start, runStart;
            var unaryOperator = false;

            
            if (ch == '-' || ch == '+')
            {
                ch = parseBuffer[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
                if (parseCurrent == parseEnd)
                {
                    tokenBuilder.MarkPropertyAsDeleted();
                    return ch;
                }

                
                PrepareAndAddRun(CssRunKind.UnaryOperator, parseCurrent - 1, ref parseCurrent);
                if (parseCurrent == parseEnd)
                {
                    return ch;
                }

                unaryOperator = true;
            }

            if (ParseSupport.NumericCharacter(charClass) || ch == '.')
            {
                ch = ScanNumeric(ch, ref charClass, ref parseCurrent);
                if (parseCurrent == parseEnd)
                {
                    return ch;
                }

                if (ch == '.')
                {
                    ch = parseBuffer[++parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);

                    
                    PrepareAndAddRun(CssRunKind.Dot, parseCurrent - 1, ref parseCurrent);

                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }

                    start = parseCurrent;
                    ch = ScanNumeric(ch, ref charClass, ref parseCurrent);
                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }

                    if (start == parseCurrent)
                    {
                        tokenBuilder.MarkPropertyAsDeleted();
                    }
                }

                if (ch == '%')
                {
                    ch = parseBuffer[++parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);

                    
                    PrepareAndAddRun(CssRunKind.Percent, parseCurrent - 1, ref parseCurrent);

                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }
                }
                else if (IsNameStartCharacter(ch, charClass, parseCurrent))
                {
                    

                    int nameLength;

                    ch = ScanName(CssRunKind.Metrics, ch, ref charClass, ref parseCurrent, out nameLength);

                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }
                }
            }
            else if (IsNameStartCharacter(ch, charClass, parseCurrent))
            {
                

                start = parseCurrent;

                int nameLength;

                ch = ScanName(CssRunKind.TermIdentifier, ch, ref charClass, ref parseCurrent, out nameLength);

                if (parseCurrent == parseEnd)
                {
                    return ch;
                }

                runStart = parseCurrent;

                if (ch == '+' && (start + 1) == parseCurrent && (parseBuffer[start] == 'u' || parseBuffer[start] == 'U'))
                {
                    ch = ScanUnicodeRange(ch, ref charClass, ref parseCurrent);

                    
                    PrepareAndAddRun(CssRunKind.UnicodeRange, runStart, ref parseCurrent);

                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }
                }
                else if (ch == '(')
                {
                    var isUrl = false;

                    if (!IsSafeIdentifier(SafeTermFunctions, start, parseCurrent))
                    {
                        tokenBuilder.MarkPropertyAsDeleted();

                        if (IsNameEqual("url", start, parseCurrent - start))
                        {
                            isUrl = true;
                        }
                    }

                    ch = parseBuffer[++parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);
                    if (parseCurrent == parseEnd)
                    {
                        
                        tokenBuilder.MarkPropertyAsDeleted();
                        return ch;
                    }

                    
                    PrepareAndAddRun(CssRunKind.FunctionStart, parseCurrent - 1, ref parseCurrent);
                    if (parseCurrent == parseEnd)
                    {
                        
                        tokenBuilder.MarkPropertyAsDeleted();
                        return ch;
                    }

                    ch = ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
                    if (parseCurrent == parseEnd)
                    {
                        
                        tokenBuilder.MarkPropertyAsDeleted();
                        return ch;
                    }

                    if (isUrl)
                    {
                        if (ch == '"' || ch == '\'')
                        {
                            start = parseCurrent;
                            ch = ScanString(ch, ref charClass, ref parseCurrent, true);

                            
                            PrepareAndAddRun(CssRunKind.String, start, ref parseCurrent);

                            if (parseCurrent == parseEnd)
                            {
                                return ch;
                            }
                        }
                        else
                        {
                            

                            start = parseCurrent;
                            ch = ScanUrl(ch, ref charClass, ref parseCurrent);

                            if (parseCurrent == parseEnd)
                            {
                                return ch;
                            }
                        }

                        ch = ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
                        if (parseCurrent == parseEnd)
                        {
                            return ch;
                        }
                    }
                    else
                    {
                        if (++level > 16)
                        {
                            
                            tokenBuilder.MarkPropertyAsDeleted();
                            return ch;
                        }
                        ch = ScanExpr(ch, ref charClass, ref parseCurrent, level);
                        if (parseCurrent == parseEnd)
                        {
                            
                            tokenBuilder.MarkPropertyAsDeleted();
                            return ch;
                        }
                    }

                    if (ch != ')')
                    {
                        tokenBuilder.MarkPropertyAsDeleted();
                    }

                    ch = parseBuffer[++parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);

                    
                    PrepareAndAddRun(CssRunKind.FunctionEnd, parseCurrent - 1, ref parseCurrent);
                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }
                }
                else if (unaryOperator)
                {
                    tokenBuilder.MarkPropertyAsDeleted();
                }
            }
            else if (unaryOperator)
            {
                tokenBuilder.MarkPropertyAsDeleted();
            }
            else if (ch == '#')
            {
                ch = parseBuffer[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);

                
                PrepareAndAddRun(CssRunKind.HexColorStart, parseCurrent - 1, ref parseCurrent);

                if (parseCurrent == parseEnd)
                {
                    return ch;
                }

                if (IsNameCharacter(ch, charClass, parseCurrent))
                {
                    

                    int nameLength;

                    ch = ScanName(CssRunKind.HexColor, ch, ref charClass, ref parseCurrent, out nameLength);

                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }
                }
                else
                {
                    tokenBuilder.MarkPropertyAsDeleted();
                }
            }
            else if (ch == '"' || ch == '\'')
            {
                start = parseCurrent;
                ch = ScanString(ch, ref charClass, ref parseCurrent, true);

                
                PrepareAndAddRun(CssRunKind.String, start, ref parseCurrent);

                if (parseCurrent == parseEnd)
                {
                    return ch;
                }
            }

            return ch;
        }

        
        private char ScanNumeric(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            var start = parseCurrent;
            var parseBuffer = this.parseBuffer;

            while (ParseSupport.NumericCharacter(charClass))
            {
                ch = parseBuffer[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
            }

            
            PrepareAndAddRun(CssRunKind.Numeric, start, ref parseCurrent);

            return ch;
        }

        
        private char ScanString(char ch, ref CharClass charClass, ref int parseCurrent, bool inProperty)
        {
            var parseEnd = this.parseEnd;
            var parseBuffer = this.parseBuffer;

            
            
            var term = ch;
            
            
            var chLast = '\0';
            var ch2ndLast = '\0';

            while (true)
            {
                ch = parseBuffer[++parseCurrent];

                if (parseCurrent == parseEnd)
                {
                    
                    
                    if (inProperty)
                    {
                        tokenBuilder.MarkPropertyAsDeleted();
                    }
                    charClass = ParseSupport.GetCharClass(ch);
                    return ch;
                }

                
                
                if (CssToken.AttemptUnescape(parseBuffer, parseEnd, ref ch, ref parseCurrent))
                {
                    if (parseCurrent == parseEnd)
                    {
                        
                        
                        if (inProperty)
                        {
                            tokenBuilder.MarkPropertyAsDeleted();
                        }
                        charClass = ParseSupport.GetCharClass(parseBuffer[parseCurrent]);
                        return parseBuffer[parseCurrent];
                    }
                    
                    
                    chLast = '\0';
                    ch2ndLast = '\0';
                }
                else
                {
                    
                    
                    if (ch == term || 
                        (ch == '\n' && chLast == '\r' && ch2ndLast != '\\') || 
                        (((ch == '\n' && chLast != '\r') || ch == '\r' || ch == '\f') && chLast != '\\')) 
                    {
                        break;
                    }
                    ch2ndLast = chLast;
                    chLast = ch;
                }
            }

            ch = parseBuffer[++parseCurrent];
            charClass = ParseSupport.GetCharClass(ch);

            return ch;
        }

        
        private char ScanName(CssRunKind runKind, char ch, ref CharClass charClass, ref int parseCurrent, out int nameLength)
        {
            nameLength = 0;

            while (true)
            {
                var runStart = parseCurrent;

                while (IsNameCharacterNoEscape(ch, ParseSupport.GetCharClass(ch)))
                {
                    nameLength++;
                    if (parseCurrent == parseEnd)
                    {
                        break;
                    }
                    ch = parseBuffer[++parseCurrent];
                }

                if (parseCurrent != runStart)
                {
                    PrepareAndAddRun(runKind, runStart, ref parseCurrent);
                }

                if (parseCurrent != parseEnd)
                {
                    runStart = parseCurrent;

                    if (ch != '\\')
                    {
                        break;
                    }

                    if (!CssToken.AttemptUnescape(parseBuffer, parseEnd, ref ch, ref parseCurrent))
                    {
                        
                        
                        ch = parseBuffer[++parseCurrent];
                        PrepareAndAddInvalidRun(runKind, ref parseCurrent);
                        break;
                    }

                    
                    
                    
                    
                    
                    
                    ++parseCurrent;
                    if (!IsNameCharacterNoEscape(ch, ParseSupport.GetCharClass(ch)))
                    {
                        if (parseCurrent != runStart)
                        {
                            PrepareAndAddLiteralRun(runKind, runStart, ref parseCurrent, ch);
                        }
                        nameLength = 0;
                        break;
                    }
                    

                    nameLength++;

                    PrepareAndAddLiteralRun(runKind, runStart, ref parseCurrent, ch);

                    if (parseCurrent == parseEnd)
                    {
                        break;
                    }
                    ch = parseBuffer[parseCurrent];
                }
                else
                {
                    break;
                }
            }

            charClass = ParseSupport.GetCharClass(ch);

            return ch;
        }

        
        private char ScanUrl(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            while (true)
            {
                var runStart = parseCurrent;

                while (IsUrlCharacter(ch, ParseSupport.GetCharClass(ch), parseCurrent) && parseCurrent != parseEnd)
                {
                    ch = parseBuffer[++parseCurrent];
                }

                if (parseCurrent != runStart)
                {
                    PrepareAndAddRun(CssRunKind.Url, runStart, ref parseCurrent);
                }

                if (parseCurrent != parseEnd)
                {
                    runStart = parseCurrent;

                    if (ch != '\\')
                    {
                        break;
                    }

                    if (!CssToken.AttemptUnescape(parseBuffer, parseEnd, ref ch, ref parseCurrent))
                    {
                        ch = parseBuffer[++parseCurrent];
                        PrepareAndAddInvalidRun(CssRunKind.Url, ref parseCurrent);
                        break;
                    }

                    ++parseCurrent;
                    PrepareAndAddLiteralRun(CssRunKind.Url, runStart, ref parseCurrent, ch);

                    if (parseCurrent == parseEnd)
                    {
                        break;
                    }
                    ch = parseBuffer[parseCurrent];
                }
                else
                {
                    break;
                }
            }

            charClass = ParseSupport.GetCharClass(ch);

            return ch;
        }

        
        private char ScanUnicodeRange(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            InternalDebug.Assert(ch == '+');

            var parseBuffer = this.parseBuffer;

            var chT = ch;
            var specStart = parseCurrent + 1;
            var i = specStart;
            var noQuestion = true;

            for (; i < specStart + 6; i++)
            {
                chT = parseBuffer[i];
                if ('?' == chT)
                {
                    noQuestion = false;
                    i++;
                    for (; i < specStart + 6 && '?' == parseBuffer[i]; i++)
                    {
                    }
                    break;
                }
                else if (!ParseSupport.HexCharacter(ParseSupport.GetCharClass(chT)))
                {
                    if (i == specStart)
                    {
                        
                        return ch;
                    }
                    break;
                }
            }

            chT = parseBuffer[i];
            if ('-' == chT && noQuestion)
            {
                i++;
                specStart = i;
                for (; i < specStart + 6; i++)
                {
                    chT = parseBuffer[i];
                    if (!ParseSupport.HexCharacter(ParseSupport.GetCharClass(chT)))
                    {
                        if (i == specStart)
                        {
                            
                            return ch;
                        }
                        break;
                    }
                }
            }

            
            chT = parseBuffer[i];
            charClass = ParseSupport.GetCharClass(chT);
            parseCurrent = i;
            return chT;
        }

        
        private char ScanWhitespace(char ch, ref CharClass charClass, ref int parseCurrent, bool ignorable)
        {
            var parseBuffer = this.parseBuffer;
            var parseEnd = this.parseEnd;

            while (ParseSupport.WhitespaceCharacter(charClass) || ch == '/')
            {
                if (ch == '/')
                {
                    if (parseCurrent < parseEnd && parseBuffer[parseCurrent + 1] == '*')
                    {
                        ch = ScanComment(ch, ref charClass, ref parseCurrent);
                        if (parseCurrent == parseEnd)
                        {
                            return ch;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    var runStart = parseCurrent;

                    do
                    {
                        if (++parseCurrent == parseEnd)
                        {
                            return ch;
                        }
                        ch = parseBuffer[parseCurrent];
                        charClass = ParseSupport.GetCharClass(ch);
                    }
                    while (ParseSupport.WhitespaceCharacter(charClass));

                    if (tokenBuilder.IsStarted)
                    {
                        
                        PrepareAndAddRun(ignorable ? CssRunKind.Invalid : CssRunKind.Space, runStart, ref parseCurrent);
                    }
                }
            }

            return ch;
        }

        
        private char ScanComment(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            var parseBuffer = this.parseBuffer;
            var parseEnd = this.parseEnd;

            var runStart = parseCurrent;

            InternalDebug.Assert(ch == '/');
            ch = parseBuffer[++parseCurrent];

            InternalDebug.Assert(ch == '*');

            while (true)
            {
                do
                {
                    if (++parseCurrent == parseEnd)
                    {
                        return ch;
                    }
                }
                while (parseBuffer[parseCurrent] != '*');

                if (parseCurrent + 1 != parseEnd && parseBuffer[parseCurrent + 1] == '/')
                {
                    parseCurrent++;
                    if (++parseCurrent == parseEnd)
                    {
                        return ch;
                    }

                    if (tokenBuilder.IsStarted)
                    {
                        
                        PrepareAndAddRun(CssRunKind.Comment, runStart, ref parseCurrent);
                    }

                    ch = parseBuffer[parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);
                    return ch;
                }
            }
        }

        
        private void PrepareAndAddRun(CssRunKind runKind, int start, ref int parseCurrent)
        {
            if (!tokenBuilder.PrepareAndAddRun(runKind, start, parseCurrent))
            {
                parseCurrent = parseEnd;
            }
        }

        
        private void PrepareAndAddInvalidRun(CssRunKind runKind, ref int parseCurrent)
        {
            if (!tokenBuilder.PrepareAndAddInvalidRun(runKind, parseCurrent))
            {
                parseCurrent = parseEnd;
            }
        }

        
        private void PrepareAndAddLiteralRun(CssRunKind runKind, int start, ref int parseCurrent, int value)
        {
            if (!tokenBuilder.PrepareAndAddLiteralRun(runKind, start, parseCurrent, value))
            {
                parseCurrent = parseEnd;
            }
        }

        
        private char SkipToNextRule(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            var parseEnd = this.parseEnd;
            var parseBuffer = this.parseBuffer;

            while (true)
            {
                if (ch == '"' || ch == '\'')
                {
                    ch = ScanString(ch, ref charClass, ref parseCurrent, false);
                    if (parseCurrent == parseEnd)
                    {
                        return ch;
                    }
                    continue;
                }
                else if (ch == '{')
                {
                    ruleDepth++;
                }
                else if (ch == '}')
                {
                    if (ruleDepth > 0)
                    {
                        ruleDepth--;
                    }

                    if (ruleDepth == 0)
                    {
                        ch = parseBuffer[++parseCurrent];
                        charClass = ParseSupport.GetCharClass(ch);

                        return ch;
                    }
                }
                else if (ch == ';' && ruleDepth == 0)
                {
                    ch = parseBuffer[++parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);

                    return ch;
                }

                if (++parseCurrent == parseEnd)
                {
                    return ch;
                }
                ch = parseBuffer[parseCurrent];
            }
        }

        
        private bool IsSafeIdentifier(string[] table, int start, int end)
        {
            var parseBuffer = this.parseBuffer;
            var length = end - start;

            for (var i = 0; i < table.Length; i++)
            {
                if (IsNameEqual(table[i], start, length))
                {
                    return true;
                }
            }

            return false;
        }

        
        private bool IsNameEqual(string name, int start, int length)
        {
            return name.Equals(new string(parseBuffer, start, length), StringComparison.OrdinalIgnoreCase);
        }

        
        private bool IsNameCharacter(char ch, CharClass charClass, int parseCurrent)
        {
            return (IsNameStartCharacter(ch, charClass, parseCurrent) || ParseSupport.NumericCharacter(charClass) || ch == '-');
        }

        
        private bool IsNameStartCharacter(char ch, CharClass charClass, int parseCurrent)
        {
            if (IsNameStartCharacterNoEscape(ch, charClass))
                return true;
            
            
            if (CssToken.AttemptUnescape(parseBuffer, parseEnd, ref ch, ref parseCurrent))
            {
                charClass = ParseSupport.GetCharClass(ch);
                return IsNameStartCharacterNoEscape(ch, charClass);
            }
            return false;
        }

        
        private static bool IsNameCharacterNoEscape(char ch, CharClass charClass)
        {
            return (IsNameStartCharacterNoEscape(ch, charClass) || ParseSupport.NumericCharacter(charClass) || ch == '-');
        }

        
        private static bool IsNameStartCharacterNoEscape(char ch, CharClass charClass)
        {
            return (ParseSupport.AlphaCharacter(charClass) || ch == '_' || ch > 127);
        }

        
        private bool IsUrlCharacter(char ch, CharClass charClass, int parseCurrent)
        {
            
            
            
            
            
            return (IsUrlCharacterNoEscape(ch, charClass) || IsEscape(ch, parseCurrent));
        }

        
        private static bool IsUrlCharacterNoEscape(char ch, CharClass charClass)
        {
            return (ch >= '*' && ch != 127) || (ch >= '#' && ch <= '&') || ch == '!';
        }

        
        private bool IsEscape(char ch, int parseCurrent)
        {
            return CssToken.AttemptUnescape(parseBuffer, parseEnd, ref ch, ref parseCurrent);
        }
    }
}

