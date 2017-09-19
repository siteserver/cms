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
    using System.Text;
    using Jayrock.Json.Conversion;

    #endregion
    
    public sealed class ArrayImporter : ImporterBase
    {
        public ArrayImporter() : this(null) {}

        public ArrayImporter(Type arrayType) : 
            base(AssertArrayType(arrayType)) {}

        private static Type AssertArrayType(Type type)
        {
            if (type == null)
                return typeof(object[]);
            
            if (!type.IsArray)
                throw new ArgumentException(string.Format("{0} is not an array.", type.FullName), "arrayType");
            
            if (type.GetArrayRank() != 1)
                throw new ArgumentException(string.Format("{0} is not one-dimension array. Multi-dimensional arrays are not supported.", type.FullName), "arrayType");
            
            return type;
        }

        protected override object ImportFromArray(ImportContext context, JsonReader reader)
        {
            Debug.Assert(context != null);
            Debug.Assert(reader != null);

            reader.Read();

            ArrayList list = new ArrayList();
            Type elementType = OutputType.GetElementType();

            while (reader.TokenClass != JsonTokenClass.EndArray)
                list.Add(context.Import(elementType, reader));

            return ReadReturning(reader, list.ToArray(elementType));
        }

        protected override object ImportFromBoolean(ImportContext context, JsonReader reader)
        {
            return ImportScalarAsArray(context, reader);
        }

        protected override object ImportFromNumber(ImportContext context, JsonReader reader)
        {
            return ImportScalarAsArray(context, reader);
        }

        protected override object ImportFromString(ImportContext context, JsonReader reader)
        {
            return ImportScalarAsArray(context, reader);
        }

        private object ImportScalarAsArray(ImportContext context, JsonReader reader)
        {
            Debug.Assert(context != null);
            Debug.Assert(reader != null);

            Type elementType = OutputType.GetElementType();
            Array array = Array.CreateInstance(elementType, 1);
            array.SetValue(context.Import(elementType, reader), 0);
            return array;
        }
    }
}