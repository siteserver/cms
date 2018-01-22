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

    #endregion

    public abstract class ExporterBase : IExporter
    {
        private readonly Type _inputType;

        protected ExporterBase(Type inputType)
        {
            if (inputType == null)
                throw new ArgumentNullException("inputType");
            
            _inputType = inputType;
        }

        public Type InputType
        {
            get { return _inputType; }
        }

        public virtual void Export(ExportContext context, object value, JsonWriter writer)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (JsonNull.LogicallyEquals(value))
                writer.WriteNull();
            else
                ExportValue(context, value, writer);
        }

        protected abstract void ExportValue(ExportContext context, object value, JsonWriter writer);
    }
}