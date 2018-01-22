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
    using System.Globalization;

    #endregion
    
    /// <summary>
    /// Represents a writer that provides a fast, non-cached, forward-only means of 
    /// emitting JSON data.
    /// </summary>

    public abstract class JsonWriter : IDisposable
    {
        /// <summary>
        /// Return the current level of nesting as the writer encounters
        /// nested objects and arrays.
        /// </summary>

        public abstract int Depth { get; }
        
        /// <summary>
        /// Return the current index within a JSON Array 
        /// (also valid for a JSON Object but indicates member count).
        /// </summary>
        
        public abstract int Index { get; }
                
        /// <summary>
        /// Determines the current bracket of the writer.
        /// </summary>

        public abstract JsonWriterBracket Bracket { get; }

        /// <summary>
        /// When overridden in a derived class, writes out the start of a 
        /// JSON object.
        /// </summary>

        public abstract void WriteStartObject();

        /// <summary>
        /// When overridden in a derived class, writes out the end of a 
        /// JSON object.
        /// </summary>

        public abstract void WriteEndObject();

        /// <summary>
        /// When overridden in a derived class, writes out an object
        /// member (but not its value).
        /// </summary>

        public abstract void WriteMember(string name);

        /// <summary>
        /// When overridden in a derived class, writes out the start of a 
        /// JSON array.
        /// </summary>

        public abstract void WriteStartArray();

        /// <summary>
        /// When overridden in a derived class, writes out the end of a 
        /// JSON array.
        /// </summary>

        public abstract void WriteEndArray();

        /// <summary>
        /// When overridden in a derived class, writes out a JSON string 
        /// value.
        /// </summary>

        public abstract void WriteString(string value);

        /// <summary>
        /// When overridden in a derived class, writes out a JSON number 
        /// value.
        /// </summary>

        public abstract void WriteNumber(string value);

        /// <summary>
        /// When overridden in a derived class, writes out a JSON boolean 
        /// value.
        /// </summary>
        
        public abstract void WriteBoolean(bool value);

        /// <summary>
        /// When overridden in a derived class, writes out the JSON null
        /// value.
        /// </summary>
        
        public abstract void WriteNull();
        
        /// <summary>
        /// When overridden in a derived class, flushes whatever is in the 
        /// buffer to the underlying streams and also flushes the 
        /// underlying stream. The default implementation does nothing.
        /// </summary>
        
        public virtual void Flush() {}

        /// <summary>
        /// Closes the writer and releases any underlying resources 
        /// associated with the writer.
        /// </summary>
        
        public virtual void Close()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        /// <summary>
        /// Writes a JSON number from a <see cref="Byte"/> value.
        /// </summary>

        public void WriteNumber(byte value)
        {
            WriteNumber(value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Writes a JSON number from an <see cref="Int16"/> value.
        /// </summary>

        public void WriteNumber(short value)
        {
            WriteNumber(value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Writes a JSON number from an <see cref="Int32"/> value.
        /// </summary>

        public void WriteNumber(int value)
        {
            WriteNumber(value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Writes a JSON number from an <see cref="Int64"/> value.
        /// </summary>

        public void WriteNumber(long value)
        {
            WriteNumber(value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Writes a JSON number from a <see cref="Decimal"/> value.
        /// </summary>

        public void WriteNumber(decimal value)
        {
            WriteNumber(value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Writes a JSON number from a <see cref="Single"/> value.
        /// </summary>

        public void WriteNumber(float value)
        {
            if (float.IsNaN(value))
                throw new ArgumentOutOfRangeException("value");

            WriteNumber(value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Writes a JSON number from a <see cref="Double"/> value.
        /// </summary>

        public void WriteNumber(double value)
        {
            if (double.IsNaN(value))
                throw new ArgumentOutOfRangeException("value");

            WriteNumber(value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Writes a JSON array of JSON strings given an enumerable source
        /// of arbitrary <see cref="Object"/> values.
        /// </summary>

        public void WriteStringArray(IEnumerable values)
        {
            if (values == null)
            {
                WriteNull();
            }
            else
            {
                WriteStartArray();
                        
                foreach (object value in values)
                {
                    if (JsonNull.LogicallyEquals(value))
                        WriteNull();
                    else
                        WriteString(value.ToString());
                }
                        
                WriteEndArray();
            }
        }

        /// <summary>
        /// Writes a JSON array of JSON strings given an array of 
        /// <see cref="String"/> values.
        /// </summary>

        public void WriteStringArray(params string[] values)
        {
            if (values == null)
            {
                WriteNull();
            }
            else
            {
                WriteStartArray();
                        
                foreach (string value in values)
                {
                    if (JsonNull.LogicallyEquals(value))
                        WriteNull();
                    else
                        WriteString(value);
                }
                        
                WriteEndArray();
            }
        }

        /// <summary>
        /// Writes the next value from the given <see cref="JsonReader"/>
        /// into this writer's output. If the reader is positioned
        /// at the root of JSON data, then the entire data will be
        /// written.
        /// </summary>

        public virtual void WriteFromReader(JsonReader reader)
        {
            if (reader == null)            
                throw new ArgumentNullException("reader");

            if (!reader.MoveToContent())
                return;

            if (reader.TokenClass == JsonTokenClass.String)
            {
                WriteString(reader.Text); 
            }
            else if (reader.TokenClass == JsonTokenClass.Number)
            {
                WriteNumber(reader.Text);
            }
            else if (reader.TokenClass == JsonTokenClass.Boolean)
            {
                WriteBoolean(reader.Text == JsonBoolean.TrueText); 
            }
            else if (reader.TokenClass == JsonTokenClass.Null)
            {
                WriteNull();
            }
            else if (reader.TokenClass == JsonTokenClass.Array)
            {
                WriteStartArray();
                reader.Read();

                while (reader.TokenClass != JsonTokenClass.EndArray)
                    WriteFromReader(reader);

                WriteEndArray();
            }
            else if (reader.TokenClass == JsonTokenClass.Object)
            {
                reader.Read();
                WriteStartObject();
                    
                while (reader.TokenClass != JsonTokenClass.EndObject)
                {
                    WriteMember(reader.ReadMember());
                    WriteFromReader(reader);
                }

                WriteEndObject();
            }
            else 
            {
                throw new JsonException(string.Format("{0} not expected.", reader.TokenClass));
            }

            reader.Read();
        }
        
        public void AutoComplete()
        {
            if (Depth == 0)
                throw new InvalidOperationException();
            
            if (Bracket == JsonWriterBracket.Member)
                WriteNull();
            
            while (Depth > 0)
            {
                if (Bracket == JsonWriterBracket.Object)
                    WriteEndObject();
                else if (Bracket == JsonWriterBracket.Array)
                    WriteEndArray();
                else 
                    throw new Exception("Implementation error.");
            }
        }

        /// <summary>
        /// Represents the method that handles the Disposed event of a reader.
        /// </summary>
        
        public virtual event EventHandler Disposed;
        
        void IDisposable.Dispose()
        {
            Close();
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                OnDisposed(EventArgs.Empty);
        }

        private void OnDisposed(EventArgs e)
        {
            EventHandler handler = Disposed;
            
            if (handler != null)
                handler(this, e);
        }
    }
}
