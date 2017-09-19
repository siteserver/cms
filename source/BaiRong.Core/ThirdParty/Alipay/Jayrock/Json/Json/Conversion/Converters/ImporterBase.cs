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
    using System.Diagnostics;
    using Jayrock.Json.Conversion;

    #endregion

    public abstract class ImporterBase : IImporter
    {
        private readonly Type _outputType;

        protected ImporterBase(Type outputType)
        {
            if (outputType == null)
                throw new ArgumentNullException("outputType");
            
            _outputType = outputType;
        }

        public Type OutputType
        {
            get { return _outputType; }
        }

        public virtual object Import(ImportContext context, JsonReader reader)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            
            if (reader == null)
                throw new ArgumentNullException("reader");
            
            if (!reader.MoveToContent())
                throw new JsonException("Unexpected EOF.");
            
            if (reader.TokenClass == JsonTokenClass.Null)
            {
                return ImportNull(context, reader);
            }
            else if (reader.TokenClass == JsonTokenClass.String)
            {
                return ImportFromString(context, reader);
            }
            else if (reader.TokenClass == JsonTokenClass.Number)
            {
                return ImportFromNumber(context, reader);
            }
            else if (reader.TokenClass == JsonTokenClass.Boolean)
            {
                return ImportFromBoolean(context, reader);
            }
            else if (reader.TokenClass == JsonTokenClass.Array)
            {
                return ImportFromArray(context, reader);
            }
            else if (reader.TokenClass == JsonTokenClass.Object)
            {
                return ImportFromObject(context, reader);
            }
            else 
            {
                throw new JsonException(string.Format("{0} not expected.", reader.TokenClass));
            }
        }

        protected virtual object ImportNull(ImportContext context, JsonReader reader) 
        {
            reader.Read();
            return null;
        }

        protected virtual object ImportFromBoolean(ImportContext context, JsonReader reader) { return ThrowNotSupported(JsonTokenClass.Boolean); }
        protected virtual object ImportFromNumber(ImportContext context, JsonReader reader) { return ThrowNotSupported(JsonTokenClass.Number); }
        protected virtual object ImportFromString(ImportContext context, JsonReader reader) { return ThrowNotSupported(JsonTokenClass.String); }
        protected virtual object ImportFromArray(ImportContext context, JsonReader reader) { return ThrowNotSupported(JsonTokenClass.Array); }
        protected virtual object ImportFromObject(ImportContext context, JsonReader reader) { return ThrowNotSupported(JsonTokenClass.Object); }

        internal static object ReadReturning(JsonReader reader, object result) 
        {
            Debug.Assert(reader != null);
            reader.Read();
            return result;
        }

        protected virtual JsonException GetImportException(string jsonValueType) 
        {
            return new JsonException(string.Format("Cannot import {0} from a JSON {1} value.", OutputType, jsonValueType));
        }

        private object ThrowNotSupported(JsonTokenClass clazz)
        {
            Debug.Assert(clazz != null);
            throw GetImportException(clazz.Name);
        }
    }
}