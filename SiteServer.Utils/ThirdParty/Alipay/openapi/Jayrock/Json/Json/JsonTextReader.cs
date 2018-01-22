#region License, Terms and Conditions
//
// Jayrock - JSON and JSON-RPC for Microsoft .NET Framework and Mono
// Written by Atif Aziz (atif.aziz@skybow.com)
// Copyright (c) 2005 Atif Aziz. All rights reserved.
//
// This library is free software; you can redistribute it and/or modify it under
// the terms of the GNU Lesser General Public License as published by the Free
// Software Foundation; either version 2.1 of the License, or (at your option)
// any later version.
//
// This library is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
// details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this library; if not, write to the Free Software Foundation, Inc.,
// 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 
//
#endregion

namespace Jayrock.Json
{
    #region Imports

    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Text;

    #endregion

    /// <summary>
    /// Represents a reader that provides fast, non-cached, forward-only 
    /// access to JSON data over JSON text (RFC 4627). 
    /// </summary>

    public sealed class JsonTextReader : JsonReaderBase
    {
        private BufferedCharReader _reader;
        private Stack _stack;

        private delegate JsonToken Continuation();

        private Continuation _methodParse;
        private Continuation _methodParseArrayFirst;
        private Continuation _methodParseArrayNext;
        private Continuation _methodParseObjectMember;
        private Continuation _methodParseObjectMemberValue;
        private Continuation _methodParseNextMember;

        public JsonTextReader(TextReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            _reader = new BufferedCharReader(reader);
            Push(ParseMethod);
        }

        /// <summary>
        /// Reads the next token and returns it.
        /// </summary>

        protected override JsonToken ReadTokenImpl()
        {
            if (_stack == null)
            {
                return JsonToken.EOF();
            }
            else if (_stack.Count == 0)
            {
                _stack = null;
                _reader = null;
                return JsonToken.EOF();
            }
            else
            {
                return Pop()();
            }
        }

        /// <summary>
        /// Parses the next token from the input and returns it.
        /// </summary>

        private JsonToken Parse()
        {
            char ch = NextClean();

            //
            // String
            //

            if (ch == '"' || ch == '\'')
            {
                return Yield(JsonToken.String(NextString(ch)));
            }

            //
            // Object
            //

            if (ch == '{')
            {
                _reader.Back();
                return ParseObject();
            }

            //
            // Array
            //

            if (ch == '[')
            {
                _reader.Back();
                return ParseArray();
            }

            //
            // Handle unquoted text. This could be the values true, false, or
            // null, or it can be a number. An implementation (such as this one)
            // is allowed to also accept non-standard forms.
            //
            // Accumulate characters until we reach the end of the text or a
            // formatting character.
            // 

            StringBuilder sb = new StringBuilder();
            char b = ch;

            while (ch >= ' ' && ",:]}/\\\"[{;=#".IndexOf(ch) < 0) 
            {
                sb.Append(ch);
                ch = _reader.Next();
            }

            _reader.Back();

            string s = sb.ToString().Trim();

            if (s.Length == 0)
                throw new JsonException("Missing value.");
            
            
            //
            // Boolean
            //

            if (s == JsonBoolean.TrueText || s == JsonBoolean.FalseText)
                return Yield(JsonToken.Boolean(s == JsonBoolean.TrueText));
            
            //
            // Null
            //

            if (s == JsonNull.Text)
                return Yield(JsonToken.Null());
            
            //
            // Number
            //

            if ((b >= '0' && b <= '9') || b == '.' || b == '-' || b == '+')
            {
                double unused;
                if (!double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out unused))
                    throw new JsonException(string.Format("The text '{0}' has the incorrect syntax for a number.", s));

                return Yield(JsonToken.Number(s));
            }
            
            //
            // Treat as String in all other cases, e.g. when unquoted.
            //

            return Yield(JsonToken.String(s));
        }

        private Continuation ParseMethod
        {
            get
            {
                if (_methodParse == null) _methodParse = new Continuation(Parse);
                return _methodParse;
            }
        }

        /// <summary>
        /// Parses expecting an array in the source.
        /// </summary>

        private JsonToken ParseArray()
        {
            if (NextClean() != '[')
                throw new JsonException("An array must start with '['.");

            return Yield(JsonToken.Array(), ParseArrayFirstMethod);
        }

        /// <summary>
        /// Parses the first element of an array or the end of the array if
        /// it is empty.
        /// </summary>

        private JsonToken ParseArrayFirst()
        {
            if (NextClean() == ']')
                return Yield(JsonToken.EndArray());

            _reader.Back();

            Push(ParseArrayNextMethod);
            return Parse();
        }

        private Continuation ParseArrayFirstMethod
        {
            get
            {
                if (_methodParseArrayFirst == null) _methodParseArrayFirst = new Continuation(ParseArrayFirst);
                return _methodParseArrayFirst;
            }
        }

        /// <summary>
        /// Parses the next element in the array.
        /// </summary>

        private JsonToken ParseArrayNext()
        {
            switch (NextClean())
            {
                case ';':
                case ',':
                {
                    if (NextClean() == ']')
                        return Yield(JsonToken.EndArray());
                    else
                        _reader.Back();

                    break;
                }

                case ']':
                {
                    return Yield(JsonToken.EndArray());
                }

                default:
                    throw new JsonException("Expected a ',' or ']'.");
            }

            Push(ParseArrayNextMethod);
            return Parse();
        }

        private Continuation ParseArrayNextMethod
        {
            get
            {
                if (_methodParseArrayNext == null) _methodParseArrayNext = new Continuation(ParseArrayNext);
                return _methodParseArrayNext;
            }
        }

        /// <summary>
        /// Parses expecting an object in the source.
        /// </summary>

        private JsonToken ParseObject()
        {
            if (NextClean() != '{')
                throw new JsonException("An object must begin with '{'.");

            return Yield(JsonToken.Object(), ParseObjectMemberMethod);
        }

        /// <summary>
        /// Parses the first member name of the object or the end of the array
        /// in case of an empty object.
        /// </summary>

        private JsonToken ParseObjectMember()
        {
            char ch = NextClean();

            if (ch == '}')
                return Yield(JsonToken.EndObject());

            if (ch == BufferedCharReader.EOF)
                throw new JsonException("An object must end with '}'.");

            _reader.Back();
            string name = Parse().Text;
            return Yield(JsonToken.Member(name), ParseObjectMemberValueMethod);
        }

        private Continuation ParseObjectMemberMethod
        {
            get
            {
                if (_methodParseObjectMember == null) _methodParseObjectMember = new Continuation(ParseObjectMember);
                return _methodParseObjectMember;
            }
        }

        private JsonToken ParseObjectMemberValue()
        {
            char ch = NextClean();

            if (ch == '=')
            {
                if (_reader.Next() != '>')
                    _reader.Back();
            }
            else if (ch != ':')
                throw new JsonException("Expected a ':' after a key.");

            Push(ParseNextMemberMethod);
            return Parse();
        }

        private Continuation ParseObjectMemberValueMethod
        {
            get
            {
                if (_methodParseObjectMemberValue == null) _methodParseObjectMemberValue = new Continuation(ParseObjectMemberValue);
                return _methodParseObjectMemberValue;
            }
        }

        private JsonToken ParseNextMember()
        {
            switch (NextClean())
            {
                case ';':
                case ',':
                    {
                        if (NextClean() == '}')
                            return Yield(JsonToken.EndObject());
                        break;
                    }

                case '}':
                    return Yield(JsonToken.EndObject());

                default:
                    throw new JsonException("Expected a ',' or '}'.");
            }

            _reader.Back();
            string name = Parse().Text;
            return Yield(JsonToken.Member(name), ParseObjectMemberValueMethod);
        }

        private Continuation ParseNextMemberMethod
        {
            get
            {
                if (_methodParseNextMember == null) _methodParseNextMember = new Continuation(ParseNextMember);
                return _methodParseNextMember;
            }
        }

        /// <summary> 
        /// Yields control back to the reader's user while updating the
        /// reader with the new found token and its text.
        /// </summary>

        private JsonToken Yield(JsonToken token)
        {
            return Yield(token, null);
        }

        /// <summary> 
        /// Yields control back to the reader's user while updating the
        /// reader with the new found token, its text and the next 
        /// continuation point into the reader.
        /// </summary>
        /// <remarks>
        /// By itself, this method cannot affect the stack such tha control 
        /// is returned back to the reader's user. This must be done by 
        /// Yield's caller by way of explicit return.
        /// </remarks>

        private JsonToken Yield(JsonToken token, Continuation continuation)
        {
            if (continuation != null)
                Push(continuation);
            
            return token;
        }
 
        /// <summary>
        /// Get the next char in the string, skipping whitespace
        /// and comments (slashslash and slashstar).
        /// </summary>
        /// <returns>A character, or 0 if there are no more characters.</returns>
        
        private char NextClean()
        {
            Debug.Assert(_reader != null);

            while (true)
            {
                char ch = _reader.Next();

                if (ch == '/')
                {
                    switch (_reader.Next())
                    {
                        case '/':
                        {
                            do
                            {
                                ch = _reader.Next();
                            } while (ch != '\n' && ch != '\r' && ch != BufferedCharReader.EOF);
                            break;
                        }
                        case '*':
                        {
                            while (true)
                            {
                                ch = _reader.Next();

                                if (ch == BufferedCharReader.EOF)
                                    throw new JsonException("Unclosed comment.");

                                if (ch == '*')
                                {
                                    if (_reader.Next() == '/')
                                        break;

                                    _reader.Back();
                                }
                            }
                            break;
                        }
                        default:
                        {
                            _reader.Back();
                            return '/';
                        }
                    }
                }
                else if (ch == '#') 
                {
                    do 
                    {
                        ch = _reader.Next();
                    } 
                    while (ch != '\n' && ch != '\r' && ch != BufferedCharReader.EOF);
                }
                else if (ch == BufferedCharReader.EOF || ch > ' ')
                {
                    return ch;
                }
            }
        }

        private string NextString(char quote)
        {
            try
            {
                return JsonString.Dequote(_reader, quote);
            }
            catch (FormatException e)
            {
                throw new JsonException(e.Message, e);
            }
        }
        
        private void Push(Continuation continuation)
        {
            Debug.Assert(continuation != null);
            
            if (_stack == null)
                _stack = new Stack(6);
            
            _stack.Push(continuation);
        }
        
        private Continuation Pop()
        {
            Debug.Assert(_stack != null);
            return (Continuation) _stack.Pop();
        }
    }
}