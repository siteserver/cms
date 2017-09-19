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

    #endregion
    
    /// <summary>
    /// Base implementation of <see cref="JsonReader"/> that can be used
    /// as a starting point for sub-classes of <see cref="JsonReader"/>.
    /// </summary>

    public abstract class JsonReaderBase : JsonReader
    {
        private JsonToken _token;
        private int _depth;

        public JsonReaderBase()
        {
            _token = JsonToken.BOF();
        }

        /// <summary>
        /// Gets the current token.
        /// </summary>

        public sealed override JsonToken Token
        {
            get { return _token; }
        }

        /// <summary>
        /// Return the current level of nesting as the reader encounters
        /// nested objects and arrays.
        /// </summary>

        public sealed override int Depth
        {
            get { return _depth; }
        }

        /// <summary>
        /// Reads the next token and returns true if one was found.
        /// </summary>

        public sealed override bool Read()
        {
            if (!EOF)
            {
                if (TokenClass == JsonTokenClass.EndObject || TokenClass == JsonTokenClass.EndArray)
                    _depth--;

                _token = ReadTokenImpl();

                if (TokenClass == JsonTokenClass.Object || TokenClass == JsonTokenClass.Array)
                    _depth++;
            }
            
            return !EOF;
        }

        /// <summary>
        /// Reads the next token and returns it.
        /// </summary>
        
        protected abstract JsonToken ReadTokenImpl();
    }
}