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

namespace Jayrock.Collections
{
    #region Imports

    using System;
    using System.Collections;

    #endregion

    [ Serializable ]
    public abstract class KeyedCollection : CollectionBase
    {
        private Hashtable _valueByKey; // TODO: Mark [ NonSerializable ] and implement IDeserializationCallback

        protected KeyedCollection()
        {
            _valueByKey = new Hashtable();
        }
        
        private Hashtable ValueByKey
        {
            get { return _valueByKey; }
        }

        protected void Add(object value)
        {
            List.Add(value);
        }

        protected object GetByKey(object key)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            
            return ValueByKey[key];
        }

        protected bool Contains(object key)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            
            return ValueByKey.ContainsKey(key);
        }

        protected bool Remove(object key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            object value = GetByKey(key);
            
            if (value == null)
                return false;
            
            List.Remove(value);
            return true;
        }

        protected override void OnValidate(object value)
        {
            base.OnValidate(value);
            
            if (KeyFromValue(value) == null)
                throw new ArgumentException("value");
        }

        protected override void OnInsertComplete(int index, object value)
        {
            ValueByKey.Add(KeyFromValue(value), value);
            base.OnInsertComplete(index, value);
        }

        protected override void OnRemoveComplete(int index, object value)
        {
            ValueByKey.Remove(KeyFromValue(value));
            base.OnRemoveComplete(index, value);
        }

        protected override void OnSetComplete(int index, object oldValue, object newValue)
        {
            ValueByKey.Remove(KeyFromValue(oldValue));
            ValueByKey.Add(KeyFromValue(newValue), newValue);
            base.OnSetComplete(index, oldValue, newValue);
        }

        protected override void OnClearComplete()
        {
            ValueByKey.Clear();
            base.OnClearComplete();
        }

        protected void ListKeysByIndex(Array keys)
        {
            if (keys == null)
                throw new ArgumentNullException("keys");
            
            if (keys.Rank != 1)
                throw new ArgumentException("keys");

            for (int i = 0; i < Math.Min(Count, keys.Length); i++)
                keys.SetValue(KeyFromValue(InnerList[i]), i);
        }

        protected abstract object KeyFromValue(object value);
    }
}
