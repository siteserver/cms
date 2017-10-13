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
    using Jayrock.Collections;

    #endregion
    
    [ Serializable ]
    internal sealed class ImporterCollection : KeyedCollection
    {
        public IImporter this[Type type]
        {
            get { return (IImporter) GetByKey(type); }
        }
       
        public void Put(IImporter importer)
        {
            if (importer == null)
                throw new ArgumentNullException("importer");
            
            Remove(importer.OutputType);
            Add(importer);
        }

        public void Add(IImporter importer)
        {
            if (importer == null)
                throw new ArgumentNullException("importer");
            
            base.Add(importer);
        }
        
        protected override object KeyFromValue(object value)
        {
            return ((IImporter) value).OutputType;
        }
    }
}