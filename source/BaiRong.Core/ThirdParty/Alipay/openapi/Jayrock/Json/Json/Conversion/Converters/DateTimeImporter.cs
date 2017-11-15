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
    using System.Xml;

    #endregion

    public sealed class DateTimeImporter : ImporterBase
    {
        public DateTimeImporter() :
            base(typeof(DateTime)) { }

        protected override object ImportFromString(ImportContext context, JsonReader reader)
        {
            Debug.Assert(context != null);
            Debug.Assert(reader != null);

            try
            {
                return ReadReturning(reader, XmlConvert.ToDateTime(reader.Text, XmlDateTimeSerializationMode.Local));
            }
            catch (FormatException e)
            {
                throw new JsonException("Error importing JSON String as System.DateTime.", e);
            }
        }

        protected override object ImportFromNumber(ImportContext context, JsonReader reader)
        {
            Debug.Assert(context != null);
            Debug.Assert(reader != null);

            string text = reader.Text;

            long time;

            try
            {
                time = Convert.ToInt64(text, CultureInfo.InvariantCulture);
            }
            catch (FormatException e)
            {
                throw NumberError(e, text);
            }
            catch (OverflowException e)
            {
                throw NumberError(e, text);
            }

            try
            {
                return ReadReturning(reader, UnixTime.ToDateTime(time));
            }
            catch (ArgumentException e)
            {
                throw NumberError(e, text);
            }
        }

        private static JsonException NumberError(Exception e, string text)
        {
            return new JsonException(string.Format("Error importing JSON Number {0} as System.DateTime.", text), e);
        }

        protected override JsonException GetImportException(string jsonValueType)
        {
            return new JsonException(string.Format("Found {0} where expecting a JSON String in ISO 8601 time format or a JSON Number expressed in Unix time.", jsonValueType));
        }
    }
}