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
    using System.Collections;
    using System.Configuration;
    using System.Diagnostics;
    using System.Threading;
    using Jayrock.Json.Conversion.Converters;

    #endregion

    [ Serializable ]
    public class ImportContext
    {
        private ImporterCollection _importers;
        private IDictionary _items;

        private static ImporterCollection _stockImporters;
        
        public virtual object Import(JsonReader reader)
        {
            return Import(AnyType.Value, reader);
        }

        public virtual object Import(Type type, JsonReader reader)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            
            if (reader == null)
                throw new ArgumentNullException("reader");

            IImporter importer = FindImporter(type);

            if (importer == null)
                throw new JsonException(string.Format("Don't know how to import {0} from JSON.", type.FullName));

            reader.MoveToContent();
            return importer.Import(this, reader);
        }

#if NET_2_0

        public virtual T Import<T>(JsonReader reader)
        {
            return (T) Import(typeof(T), reader);
        }

#endif

        public virtual void Register(IImporter importer)
        {
            if (importer == null)
                throw new ArgumentNullException("importer");
            
            Importers.Put(importer);
        }

        public virtual IImporter FindImporter(Type type) 
        {
            if (type == null)
                throw new ArgumentNullException("type");

            IImporter importer = Importers[type];
            
            if (importer != null)
                return importer;
            
            importer = StockImporters[type];

            if (importer == null)
               importer = FindCompatibleImporter(type);

            if (importer != null)
            {
                Register(importer);
                return importer;
            }

            return null;
        }

        public IDictionary Items
        {
            get
            {
                if (_items == null)
                    _items = new Hashtable();
                
                return _items;
            }
        }

        private static IImporter FindCompatibleImporter(Type type) 
        {
            Debug.Assert(type != null);

            if (typeof(IJsonImportable).IsAssignableFrom(type))
                return new ImportAwareImporter(type);
            
            if (type.IsArray && type.GetArrayRank() == 1)
                return new ArrayImporter(type);

            if (type.IsEnum)
                return new EnumImporter(type);
            
            if ((type.IsPublic || type.IsNestedPublic) && 
                !type.IsPrimitive && type.GetConstructor(Type.EmptyTypes) != null)
            {
                return new ComponentImporter(type);
            }

            return null;
        }

        private ImporterCollection Importers
        {
            get
            {
                if (_importers == null)
                    _importers = new ImporterCollection();
                
                return _importers;
            }
        }

        private static ImporterCollection StockImporters
        {
            get
            {
                if (_stockImporters == null)
                {
                    ImporterCollection importers = new ImporterCollection();

                    importers.Add(new ByteImporter());
                    importers.Add(new Int16Importer());
                    importers.Add(new Int32Importer());
                    importers.Add(new Int64Importer());
                    importers.Add(new SingleImporter());
                    importers.Add(new DoubleImporter());
                    importers.Add(new DecimalImporter());
                    importers.Add(new StringImporter());
                    importers.Add(new BooleanImporter());
                    importers.Add(new DateTimeImporter());
                    importers.Add(new GuidImporter());
                    importers.Add(new ByteArrayImporter());
                    importers.Add(new AnyImporter());
                    importers.Add(new DictionaryImporter());
                    importers.Add(new ListImporter());
                    
                    IList typeList = (IList) ConfigurationManager.GetSection("jayrock/json.conversion.importers");

                    if (typeList != null && typeList.Count > 0)
                    {
                        foreach (Type type in typeList)
                            importers.Add((IImporter) Activator.CreateInstance(type));
                    }

                    _stockImporters = importers;
                }
                
                return _stockImporters;
            }
        }
    }
}