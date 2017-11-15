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
    using System.Globalization;
    using System.Xml;

    #endregion

    public class DictionarySectionHandler : IConfigurationSectionHandler
    {
        public virtual object Create(object parent, object configContext, XmlNode section)
        {
            if (section == null)
                throw new ArgumentNullException("section");

            IDictionary dictionary = CreateDictionary(parent);

            string keyName = KeyName;

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

                string nodeName = childNode.Name;

                if (nodeName == "clear")
                {
                    OnClear(dictionary);
                }
                else
                {
                    XmlAttribute keyAttribute = childNode.Attributes[keyName];
                    string key = keyAttribute == null ? null : keyAttribute.Value;

                    if (key == null || key.Length == 0)
                        throw new ConfigurationErrorsException("Missing entry key.", childNode);

                    if (nodeName == "add")
                    {
                        OnAdd(dictionary, key, childNode);
                    }
                    else if (nodeName == "remove")
                    {
                        OnRemove(dictionary, key);
                    }
                    else
                    {
                        throw new ConfigurationErrorsException(string.Format("'{0}' is not a valid dictionary node. Use add, remove or clear.", nodeName), childNode);
                    }
                }
            }

            return dictionary;
        }

        protected virtual IDictionary CreateDictionary(object parent)
        {
            return parent != null ?
                new Hashtable((IDictionary)parent, StringComparer.InvariantCultureIgnoreCase) :
                new Hashtable(StringComparer.InvariantCultureIgnoreCase);
        }

        protected virtual string KeyName
        {
            get { return "key"; }
        }

        protected virtual string ValueName
        {
            get { return "value"; }
        }

        protected virtual void OnAdd(IDictionary dictionary, string key, XmlNode node)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            if (node == null)
                throw new ArgumentNullException("node");

            XmlAttribute valueAttribute = node.Attributes[ValueName];
            dictionary.Add(key, valueAttribute != null ? valueAttribute.Value : null);
        }

        protected virtual void OnRemove(IDictionary dictionary, string key)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            dictionary.Remove(key);
        }

        protected virtual void OnClear(IDictionary dictionary)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            dictionary.Clear();
        }
    }
}