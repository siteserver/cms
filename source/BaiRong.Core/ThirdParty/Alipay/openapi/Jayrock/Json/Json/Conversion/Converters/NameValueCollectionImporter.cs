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
    using System.Collections.Specialized;

    #endregion

    public class NameValueCollectionImporter : ImporterBase
    {
        public NameValueCollectionImporter() : 
            base(typeof(NameValueCollection)) { }

        protected override object ImportFromObject(ImportContext context, JsonReader reader)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (reader == null)
                throw new ArgumentNullException("reader");

            //
            // Reader must be sitting on an object.
            //

            if (reader.TokenClass != JsonTokenClass.Object)
                throw new JsonException("Expecting object.");
            
            reader.Read();
            
            //
            // Create the NameValueCollection object being deserialized.
            // If a hint was supplied, then that's what we will create
            // here because it could be that the caller wants to 
            // return a subtype of NameValueCollection.
            //
            
            NameValueCollection collection = CreateCollection();
            
            //
            // Loop through all members of the object.
            //

            while (reader.TokenClass != JsonTokenClass.EndObject)
            {
                string name = reader.ReadMember();
                
                //
                // If the value is an array, then it's a multi-value 
                // entry otherwise a single-value one.
                //

                if (reader.TokenClass == JsonTokenClass.Array)
                {
                    reader.Read();
                    
                    while (reader.TokenClass != JsonTokenClass.EndArray)
                    {
                        collection.Add(name, GetValueAsString(reader));
                        reader.Read();
                    }
                }
                else
                {
                    collection.Add(name, GetValueAsString(reader));    
                }
                
                reader.Read(); // EndArray/String
            }
            
            return collection;
        }

        protected virtual string GetValueAsString(JsonReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");
           
            if (reader.TokenClass == JsonTokenClass.String ||
                reader.TokenClass == JsonTokenClass.Boolean ||
                reader.TokenClass == JsonTokenClass.Number)
            {
                return reader.Text;
            }
            else if (reader.TokenClass == JsonTokenClass.Null)
            {
                return null;
            }
            else
            {
                throw new JsonException(string.Format("Cannot put a JSON {0} value in a NameValueCollection instance.", reader.TokenClass));
            }
        }        

        protected virtual NameValueCollection CreateCollection()
        {
            return new NameValueCollection();
        }
    }
}