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

    #endregion

    /// <summary>
    /// Base implementation of <see cref="JsonWriter"/> that can be used
    /// as a starting point for sub-classes of <see cref="JsonWriter"/>.
    /// </summary>

    public abstract class JsonWriterBase : JsonWriter
    {
        private WriterStateStack _stateStack;
        private WriterState _state;
        
        public JsonWriterBase()
        {
            _state = new WriterState(JsonWriterBracket.Pending);
        }
        
        public sealed override int Depth
        {
            get { return HasStates ? States.Count : 0; }
        }

        public sealed override int Index
        {
            get { return Depth == 0 ? -1 : _state.Index; }
        }
        
        public sealed override JsonWriterBracket Bracket
        {
            get { return _state.Bracket; }
        }

        public sealed override void WriteStartObject()
        {
            EnsureNotEnded();

            if (_state.Bracket != JsonWriterBracket.Pending)
                EnsureMemberOnObjectBracket();
            
            WriteStartObjectImpl();
            EnterBracket(JsonWriterBracket.Object);
        }

        public sealed override void WriteEndObject()
        {
            if (_state.Bracket != JsonWriterBracket.Object)
                throw new JsonException("JSON Object tail not expected at this time.");
            
            WriteEndObjectImpl();
            ExitBracket();
        }

        public sealed override void WriteMember(string name)
        {
            if (_state.Bracket != JsonWriterBracket.Object)
                throw new JsonException("A JSON Object member is not valid inside a JSON Array.");

            WriteMemberImpl(name);
            _state.Bracket = JsonWriterBracket.Member;
        }

        public sealed override void WriteStartArray()
        {
            EnsureNotEnded();

            if (_state.Bracket != JsonWriterBracket.Pending)
                EnsureMemberOnObjectBracket();
            
            WriteStartArrayImpl();
            EnterBracket(JsonWriterBracket.Array);
        }

        public sealed override void WriteEndArray()
        {
            if (_state.Bracket != JsonWriterBracket.Array)
                throw new JsonException("JSON Array tail not expected at this time.");
            
            WriteEndArrayImpl();
            ExitBracket();
        }

        public sealed override void WriteString(string value)
        {
            if (Depth == 0)
            {
                WriteStartArray(); WriteString(value); WriteEndArray();
            }
            else
            {
                EnsureMemberOnObjectBracket();
                WriteStringImpl(value);
                OnValueWritten();
            }
        }

        public sealed override void WriteNumber(string value)
        {
            if (Depth == 0)
            {
                WriteStartArray(); WriteNumber(value); WriteEndArray();
            }
            else
            {
                EnsureMemberOnObjectBracket();
                WriteNumberImpl(value);
                OnValueWritten();
            }
        }

        public sealed override void WriteBoolean(bool value)
        {
            if (Depth == 0)
            {
                WriteStartArray(); WriteBoolean(value); WriteEndArray();
            }
            else
            {
                EnsureMemberOnObjectBracket();
                WriteBooleanImpl(value);
                OnValueWritten();
            }
        }

        public sealed override void WriteNull()
        {
            if (Depth == 0)
            {
                WriteStartArray(); WriteNull(); WriteEndArray();
            }
            else 
            {
                EnsureMemberOnObjectBracket();
                WriteNullImpl();
                OnValueWritten();
            }
        }
        
        //
        // Actual methods that need to be implemented by the subclass.
        // These methods do not need to check for the structural 
        // integrity since this is checked by this base implementation.
        //
        
        protected abstract void WriteStartObjectImpl();
        protected abstract void WriteEndObjectImpl();
        protected abstract void WriteMemberImpl(string name);
        protected abstract void WriteStartArrayImpl();
        protected abstract void WriteEndArrayImpl();
        protected abstract void WriteStringImpl(string value);
        protected abstract void WriteNumberImpl(string value);
        protected abstract void WriteBooleanImpl(bool value);
        protected abstract void WriteNullImpl();

        private bool HasStates
        {
            get { return _stateStack != null && _stateStack.Count > 0; }
        }

        private WriterStateStack States
        {
            get
            {
                if (_stateStack == null)
                    _stateStack = new WriterStateStack();
                
                return _stateStack;
            }
        }

        private void EnterBracket(JsonWriterBracket newBracket)
        {
            Debug.Assert(newBracket == JsonWriterBracket.Array || newBracket == JsonWriterBracket.Object);
            
            States.Push(_state);
            _state = new WriterState(newBracket);
        }
        
        private void ExitBracket()
        {
            _state = States.Pop();

            if (_state.Bracket == JsonWriterBracket.Pending)
                _state.Bracket = JsonWriterBracket.Closed;
            else            
                OnValueWritten();
        }

        private void OnValueWritten()
        {
            if (_state.Bracket == JsonWriterBracket.Member) 
                _state.Bracket = JsonWriterBracket.Object;
            
            _state.Index++;
        }

        private void EnsureMemberOnObjectBracket() 
        {
            if (_state.Bracket == JsonWriterBracket.Object)
                throw new JsonException("A JSON member value inside a JSON object must be preceded by its member name.");
        }

        private void EnsureNotEnded()
        {
            if (_state.Bracket == JsonWriterBracket.Closed)
                throw new JsonException("JSON text has already been ended.");
        }

        [ Serializable ]
        private struct WriterState
        {
            public JsonWriterBracket Bracket;
            public int Index;

            public WriterState(JsonWriterBracket bracket)
            {
                Bracket = bracket;
                Index = 0;
            }
        }
        
        [ Serializable ]
        private sealed class WriterStateStack
        {
            private WriterState[] _states;
            private int _count;

            public int Count
            {
                get { return _count; }
            }

            public void Push(WriterState state)
            {
                if (_states == null)
                {
                    _states = new WriterState[6];
                }
                else if (_count == _states.Length)
                {
                    WriterState[] items = new WriterState[_states.Length * 2];
                    _states.CopyTo(items, 0);
                    _states = items;
                }
                
                _states[_count++] = state;
            }
            
            public WriterState Pop()
            {
                if (_count == 0)
                    throw new InvalidOperationException();
                
                WriterState state = _states[--_count];
                
                if (_count == 0)
                    _states = null;
                
                return state;
            }
        }
    }
}