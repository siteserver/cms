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

    public abstract class ListSectionHandler : IConfigurationSectionHandler
    {
        private readonly string _elementName;

        protected ListSectionHandler(string elementName)
        {
            if (elementName == null)
                throw new ArgumentNullException("elementName");
            
            if (elementName.Length == 0)
                throw new ArgumentException("elementName");
            
            _elementName = elementName;
        }

        protected string ElementName
        {
            get { return _elementName; }
        }

        public virtual object Create(object parent, object configContext, XmlNode section)
        {
            if (section == null)
                throw new ArgumentNullException("section");

            IList list = CreateList(parent);

            string itemName = ElementName;

            foreach (XmlNode childNode in section.ChildNodes)
            {
                if (childNode.NodeType == XmlNodeType.Comment ||
                    childNode.NodeType == XmlNodeType.Whitespace)
                {
                    continue;
                }

                if (childNode.NodeType != XmlNodeType.Element)
                {
                    throw new ConfigurationErrorsException(string.Format("Unexpected type of node ({0}) in configuration.", 
                        childNode.NodeType.ToString()), childNode);
                }

                if (childNode.Name != itemName)
                {
                    throw new ConfigurationErrorsException(string.Format("Element <{0}> is not valid here in configuration. Use <{1}> elements only.", 
                        childNode.Name, itemName), childNode);
                }

                list.Add(GetItem((XmlElement) childNode));
            }
            
            return list;
        }

        protected virtual IList CreateList(object parent)
        {
            return parent != null ?
                new ArrayList((ICollection) parent) :
                new ArrayList(/* capacity */ 4);
        }
        
        protected abstract object GetItem(XmlElement element);
    }
}
