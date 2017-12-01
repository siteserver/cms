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

namespace Jayrock.Configuration
{
    #region Imports

    using System;
    using System.Collections;
    using System.Configuration;
    using System.Xml;

    #endregion

    public class TypeListSectionHandler : ListSectionHandler
    {
        private readonly Type _expectedType;
        
        public TypeListSectionHandler(string elementName) : 
            this(elementName, null) {}

        public TypeListSectionHandler(string elementName, Type expectedType) : 
            base(elementName)
        {
            _expectedType = expectedType;
        }

        protected Type ExpectedType
        {
            get { return _expectedType; }
        }

        protected override object GetItem(XmlElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            string typeName = element.GetAttribute("type");
            
            if (typeName.Length == 0)
            {
                throw new ConfigurationErrorsException(string.Format("Missing type name specification on <{0}> element.", ElementName), element);
            }

            Type type = GetType(typeName);
            ValidateType(type, element);
            return type;
        }
 
        protected virtual void ValidateType(Type type, XmlElement element)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (element == null)
                throw new ArgumentNullException("element");
            
            if (ExpectedType == null)
                return;
            
            if (!ExpectedType.IsAssignableFrom(type))
                throw new ConfigurationErrorsException(string.Format("The type {0} is not valid for the <{2}> configuration element. It must be compatible with the type {1}.", type.FullName, ExpectedType.FullName, element.Name), element);
        }

        protected virtual Type GetType(string typeName) 
        {
            if (typeName == null)
                throw new ArgumentNullException("typeName");
            
            return Compat.GetType(typeName);
        }
    }
}