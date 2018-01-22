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
    using Jayrock.Json.Conversion;

    #endregion

    /// <summary>
    /// An ordered sequence of values. This class also provides a number of
    /// methods that can be found on a JavaScript Array for sake of parity.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Public Domain 2002 JSON.org, ported to C# by Are Bjolseth (teleplan.no)
    /// and re-adapted by Atif Aziz (www.raboof.com)</para>
    /// </remarks>

    [ Serializable ]
    public class JsonArray : CollectionBase, IJsonImportable, IJsonExportable
    {
        public JsonArray() {}

        public JsonArray(IEnumerable collection)
        {
            foreach (object item in collection)
                InnerList.Add(item);
        }

        public virtual object this[int index]
        {
            get { return InnerList[index]; }
            set { InnerList[index] = value; }
        }

        public int Length
        {
            get { return Count; }
        }

        public JsonArray Put(object value)
        {
            Add(value);
            return this;
        }

        public virtual void Add(object value)
        {
            InnerList.Add(value);
        }

        public virtual void Remove(object value)
        {
            InnerList.Remove(value);
        }

        public virtual bool Contains(object value)
        {
            return InnerList.Contains(value);
        }

        public virtual int IndexOf(object value)
        {
            return InnerList.IndexOf(value);
        }

        public virtual bool HasValueAt(int index)
        {
            return this[index] != null;
        }

        public virtual object GetValue(int index)
        {
            return GetValue(index, null);
        }

        public virtual object GetValue(int index, object defaultValue)
        {
            object value = this[index];
            return value != null ? value : defaultValue;
        }

        public virtual bool GetBoolean(int index)
        {
            return GetBoolean(index, false);
        }

        public virtual bool GetBoolean(int index, bool defaultValue)
        {
            object value = GetValue(index);
            if (value == null) return defaultValue;
            return Convert.ToBoolean(value, CultureInfo.InvariantCulture);
        }

        public virtual double GetDouble(int index)
        {
            return GetDouble(index, float.NaN);
        }

        public virtual double GetDouble(int index, float defaultValue)
        {
            object value = GetValue(index);
            if (value == null) return defaultValue;
            return Convert.ToDouble(value, CultureInfo.InvariantCulture);
        }

        public virtual int GetInt32(int index)
        {
            return GetInt32(index, 0);
        }

        public virtual int GetInt32(int index, int defaultValue)
        {
            object value = GetValue(index);
            if (value == null) return defaultValue;
            return Convert.ToInt32(value, CultureInfo.InvariantCulture);
        }

        public virtual string GetString(int index)
        {
            return GetString(index, string.Empty);
        }

        public virtual string GetString(int index, string defaultValue)
        {
            object value = GetValue(index);
            if (value == null) return defaultValue;
            return value.ToString();
        }

        public virtual JsonArray GetArray(int index)
        {
            return (JsonArray) GetValue(index);
        }

        public virtual JsonObject GetObject(int index)
        {
            return (JsonObject) GetValue(index);
        }

        /// <summary>
        /// Make an JSON external form string of this JsonArray. For
        /// compactness, no unnecessary whitespace is added.
        /// </summary>
        /// <remarks>
        /// This method assumes that the data structure is acyclical.
        /// </remarks>

        public override string ToString()
        {
            JsonTextWriter writer = new JsonTextWriter();
            Export(writer);
            return writer.ToString();
        }
        
        /// <summary>
        /// Make an JSON external form string of this JsonArray. For
        /// compactness, no unnecessary whitespace is added.
        /// </summary>
        /// <remarks>
        /// This method assumes that the data structure is acyclical.
        /// </remarks>

        public virtual void Export(JsonWriter writer)
        {
            Export(new ExportContext(), writer);
        }

        void IJsonExportable.Export(ExportContext context, JsonWriter writer)
        {
            Export(context, writer);
        }

        protected virtual void Export(ExportContext context, JsonWriter writer)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (writer == null)
                throw new ArgumentNullException("writer");
            
            writer.WriteStartArray();

            foreach (object value in this)
                context.Export(value, writer);
            
            writer.WriteEndArray();
        }
        
        public virtual void Import(JsonReader reader)
        {
            Import(new ImportContext(), reader);
        }

        void IJsonImportable.Import(ImportContext context, JsonReader reader)
        {
            Import(context, reader);
        }

        protected virtual void Import(ImportContext context, JsonReader reader)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            
            if (reader == null)
                throw new ArgumentNullException("reader");
            
            //
            // IMPORTANT! A new list is created and then committed to make
            // sure that this method is exception-safe. If something goes
            // wrong during the import of elements then this instance 
            // will remain largely untouched.
            //
            
            ArrayList list = new ArrayList();
            
            reader.ReadToken(JsonTokenClass.Array);
            
            while (reader.TokenClass != JsonTokenClass.EndArray)
                list.Add(context.Import(reader));
            
            reader.Read();
            
            InnerList.Clear();
            InnerList.AddRange(list);
        }

        /// <summary>
        /// Copies the elements to a new object array.
        /// </summary>

        public virtual object[] ToArray()
        {
            return (object[]) ToArray(typeof(object));
        }

        /// <summary>
        /// Copies the elements to a new array of the specified type.
        /// </summary>

        public virtual Array ToArray(Type elementType)
        {
            return InnerList.ToArray(elementType);
        }

        public virtual void Reverse()
        {
            InnerList.Reverse();
        }

        //
        // Methods that imitate the JavaScript array methods.
        //

        /// <summary>
        /// Appends new elements to an array.
        /// </summary>
        /// <returns>
        /// The new length of the array.
        /// </returns>
        /// <remarks>
        /// This method appends elements in the order in which they appear. If
        /// one of the arguments is an array, it is added as a single element.
        /// Use the <see cref="Concat"/> method to join the elements from two or
        /// more arrays.
        /// </remarks>
        
        public virtual int Push(object value)
        {
            Add(value);
            return Count;
        }

        /// <summary>
        /// Appends new elements to an array.
        /// </summary>
        /// <returns>
        /// The new length of the array.
        /// </returns>
        /// <remarks>
        /// This method appends elements in the order in which they appear. If
        /// one of the arguments is an array, it is added as a single element.
        /// Use the <see cref="Concat"/> method to join the elements from two or
        /// more arrays.
        /// </remarks>

        public virtual int Push(params object[] values)
        {
            if (values != null)
            {
                foreach (object value in values)
                    Push(value);
            }

            return Count;
        }

        /// <summary>
        /// Removes the last element from an array and returns it.
        /// </summary>
        /// <remarks>
        /// If the array is empty, null is returned.
        /// </remarks>

        public virtual object Pop()
        {
            if (Count == 0)
                return null;

            object lastValue = InnerList[Count - 1];
            RemoveAt(Count - 1);
            return lastValue;
        }

        /// <summary>
        /// Returns a new array consisting of a combination of two or more
        /// arrays.
        /// </summary>

        public virtual JsonArray Concat(params object[] values)
        {
            JsonArray newArray = new JsonArray(this);

            if (values != null)
            {
                foreach (object value in values)
                {
                    JsonArray arrayValue = value as JsonArray;
                    
                    if (arrayValue != null)
                    {
                        foreach (object arrayValueValue in arrayValue)
                            newArray.Push(arrayValueValue);
                    }
                    else
                    {
                        newArray.Push(value);
                    }
                }
            }

            return newArray;
        }

        /// <summary>
        /// Removes the first element from an array and returns it.
        /// </summary>

        public virtual object Shift()
        {
            if (Count == 0)
                return null;

            object firstValue = InnerList[0];
            RemoveAt(0);
            return firstValue;
        }

        /// <summary>
        /// Returns an array with specified elements inserted at the beginning.
        /// </summary>
        /// <remarks>
        /// The unshift method inserts elements into the start of an array, so
        /// they appear in the same order in which they appear in the argument
        /// list.
        /// </remarks>

        public virtual JsonArray Unshift(object value)
        {
            InnerList.Insert(0, value);
            return this;
        }

        /// <summary>
        /// Returns an array with specified elements inserted at the beginning.
        /// </summary>
        /// <remarks>
        /// The unshift method inserts elements into the start of an array, so
        /// they appear in the same order in which they appear in the argument
        /// list.
        /// </remarks>

        public virtual JsonArray Unshift(params object[] values)
        {
            if (values != null)
            {
                foreach (object value in values)
                    Unshift(value);
            }

            return this;
        }
    }
}
