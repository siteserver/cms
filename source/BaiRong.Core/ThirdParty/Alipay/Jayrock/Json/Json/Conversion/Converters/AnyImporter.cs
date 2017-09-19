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
    using System.Globalization;

    #endregion

    public sealed class AnyImporter : ImporterBase
    {
        public AnyImporter() : 
            base(AnyType.Value) {}

        protected override object ImportFromBoolean(ImportContext context, JsonReader reader)
        {
            return BooleanObject.Box(reader.ReadBoolean());
        }

        protected override object ImportFromNumber(ImportContext context, JsonReader reader)
        {
            return reader.ReadNumber();
        }

        protected override object ImportFromString(ImportContext context, JsonReader reader)
        {
            return reader.ReadString();
        }

        protected override object ImportFromArray(ImportContext context, JsonReader reader)
        {
            JsonArray items = new JsonArray();
            ((IJsonImportable) items).Import(context, reader);
            return items;
        }

        protected override object ImportFromObject(ImportContext context, JsonReader reader)
        {
            JsonObject o = new JsonObject();
            ((IJsonImportable) o).Import(context, reader);
            return o;
        }
    }
}