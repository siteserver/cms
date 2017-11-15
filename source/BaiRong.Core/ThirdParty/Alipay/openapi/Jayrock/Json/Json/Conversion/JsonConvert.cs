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

namespace Jayrock.Json.Conversion
{
    #region Imports

    using System;
    using System.IO;
    using System.Text;

    #endregion

    /// <summary>
    /// Provides methods for converting between Common Language Runtime 
    /// (CLR) types and JSON types.
    /// </summary>
    
    public sealed class JsonConvert
    {
        public static void Export(object value, JsonWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");
            
            ExportContext context = new ExportContext();
            context.Export(value, writer);
        }
        
        public static void Export(object value, TextWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");
            
            Export(value, new JsonTextWriter(writer));
        }

        public static void Export(object value, StringBuilder sb)
        {
            if (sb == null)
                throw new ArgumentNullException("sb");
            
            Export(value, new StringWriter(sb));
        }
        
        public static string ExportToString(object value)
        {
            StringBuilder sb = new StringBuilder();
            Export(value, sb);
            return sb.ToString();
        }

        public static object Import(string text)
        {
            return Import(new StringReader(text));
        }

        public static object Import(TextReader reader)
        {
            return Import(new JsonTextReader(reader));
        }

        public static object Import(JsonReader reader)
        {
            return Import(AnyType.Value, reader);
        }
        
        public static object Import(Type type, string text)
        {
            return Import(type, new StringReader(text));
        }

        public static object Import(Type type, TextReader reader)
        {
            return Import(type, new JsonTextReader(reader));
        }

        public static object Import(Type type, JsonReader reader)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (reader == null)
                throw new ArgumentNullException("reader");
            
            ImportContext context = new ImportContext();
            return context.Import(type, reader);
        }

#if NET_2_0

        //
        // Generic versions of Import methods.
        //

        public static T Import<T>(string text)
        {
            return (T) Import(typeof(T), text);
        }

        public static T Import<T>(TextReader reader)
        {
            return (T) Import(typeof(T), reader);
        }

        public static T Import<T>(JsonReader reader)
        {
            return (T) Import(typeof(T), reader);
        }

#endif

        private JsonConvert()
        {
            throw new NotSupportedException();
        }
    }
}
