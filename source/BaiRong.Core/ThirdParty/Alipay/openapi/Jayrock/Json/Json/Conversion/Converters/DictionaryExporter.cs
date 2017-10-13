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

namespace Jayrock.Json.Conversion.Converters
{
    #region Imports

    using System;
    using System.Collections;
    using System.Diagnostics;

    #endregion
    
    public sealed class DictionaryExporter : ExporterBase
    {
        public DictionaryExporter(Type inputType) : 
            base(inputType) {}

        protected override void ExportValue(ExportContext context, object value, JsonWriter writer)
        {
            Debug.Assert(context != null);
            Debug.Assert(value != null);
            Debug.Assert(writer != null);

            writer.WriteStartObject();
            
            IDictionary dictionary = (IDictionary) value;
            
            foreach (DictionaryEntry entry in dictionary)
            {
                writer.WriteMember(entry.Key.ToString());
                context.Export(entry.Value, writer);
            }

            /*
             FIXME: Use IDictionaryEnumerator.Entry instead and enumerate manually (faster and more robust).
             It is faster because unboxing is avoided by going over
             IDictionaryEnumerator.Entry rather than 
             IDictionaryEnumerator.Current. It is more robust because many 
             people may get the implementation of IDictionary.GetEnumerator 
             wrong, especially if they are implementing IDictionary<K, V> in 
             2.0. If they simply return the enumerator from the wrapped
             dictionary then Current will return KeyValuePair<K, V> instead
             of DictionaryEntry and therefore cause a casting exception.
             
            using (IDictionaryEnumerator e = dictionary.GetEnumerator())
            {            
                while (e.MoveNext())
                {
                    writer.WriteMember(e.Entry.Key.ToString());
                    context.Export(e.Entry.Value, writer);
                }
            }
            */

            writer.WriteEndObject();
        }
    }
}