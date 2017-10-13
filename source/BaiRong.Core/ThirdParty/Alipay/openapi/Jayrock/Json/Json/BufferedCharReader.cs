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
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Text;

    #endregion
    
    internal sealed class BufferedCharReader
    {
        private TextReader _reader;
        private char[] _buffer;
        private int _index;
        private int _end;
        private bool _backed;
        private char _backup;
        private int _bufferSize;

        public const char EOF = (char) 0;

        public BufferedCharReader(TextReader reader) :
            this(reader, 0) {}
        
        public BufferedCharReader(TextReader reader, int bufferSize)
        {
            Debug.Assert(reader != null);
            
            _reader = reader;
            _bufferSize = Math.Max(256, bufferSize);
        }

        /// <summary>
        /// Back up one character. This provides a sort of lookahead capability,
        /// so that one can test for a digit or letter before attempting to,
        /// for example, parse the next number or identifier.
        /// </summary>
        /// <remarks>
        /// This implementation currently does not support backing up more
        /// than a single character (the last read).
        /// </remarks>
        
        public void Back()
        {
            Debug.Assert(!_backed);

            if (_index > 0)
            {
                _backup = _buffer[_index - 1];
                _backed = true;
            }
        }

        /// <summary>
        /// Determine if the source string still contains characters that Next()
        /// can consume.
        /// </summary>
        /// <returns>true if not yet at the end of the source.</returns>
        
        public bool More()
        {
            if (_index == _end)
            {
                if (_buffer == null)
                    _buffer = new char[_bufferSize];
                
                _index = 0;
                _end = _reader.Read(_buffer, 0, _buffer.Length);
                
                if (_end == 0)
                    return false;
            }
            
            return true;
        }

        /// <summary>
        /// Get the next character in the source string.
        /// </summary>
        /// <returns>The next character, or 0 if past the end of the source string.</returns>
        
        public char Next()
        {
            if (_backed)
            {
                _backed = false;
                return _backup;    
            }
            
            if (!More())
                return EOF;

            char ch = _buffer[_index++];
            return ch;
        }
    }
}
