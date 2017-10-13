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
    using System.Diagnostics;
    using System.Globalization;
    using System.Reflection;
    
    //
    // Types from System.ComponentModel must be imported explicitly because
    // .NET Framework 2.0 also contains a CustomTypeDescriptor in 
    // System.ComponentModel.
    //
    
    using ICustomTypeDescriptor = System.ComponentModel.ICustomTypeDescriptor;
    using PropertyDescriptorCollection = System.ComponentModel.PropertyDescriptorCollection;
    using PropertyDescriptor = System.ComponentModel.PropertyDescriptor;
    using AttributeCollection= System.ComponentModel.AttributeCollection;
    using TypeConverter = System.ComponentModel.TypeConverter;
    using EventDescriptor = System.ComponentModel.EventDescriptor;
    using EventDescriptorCollection = System.ComponentModel.EventDescriptorCollection;
    
    #endregion
    
    /// <summary>
    /// Provides an <see cref="ICustomTypeDescriptor"/> implementation on top of the
    /// public read/write fields and properties of a given type.
    /// </summary>

    public sealed class CustomTypeDescriptor : ICustomTypeDescriptor
    {
        private readonly PropertyDescriptorCollection _properties;

        public CustomTypeDescriptor(Type type) : 
            this(type, null) {}

        public CustomTypeDescriptor(Type type, MemberInfo[] members) :
            this(type, members, null) {}

        public CustomTypeDescriptor(Type type, MemberInfo[] members, string[] names)
        {
            if (type == null) 
                throw new ArgumentNullException("type");
            
            //
            // No members supplied? Get all public, instance-level fields and 
            // properties of the type that are not marked with the JsonIgnore
            // attribute.
            //
            
            if (members == null)
            {
                const BindingFlags bindings = BindingFlags.Instance | BindingFlags.Public;
                FieldInfo[] fields = type.GetFields(bindings);
                PropertyInfo[] properties = type.GetProperties(bindings);
                
                //
                // Filter out members marked with JsonIgnore attribute.
                //
                
                ArrayList memberList = new ArrayList(fields.Length + properties.Length);
                memberList.AddRange(fields);
                memberList.AddRange(properties);

                for (int i = 0; i < memberList.Count; i++)
                {
                    MemberInfo member = (MemberInfo) memberList[i];
                    
                    if (!member.IsDefined(typeof(JsonIgnoreAttribute), true))
                        continue;
                    
                    memberList.RemoveAt(i--);
                }
                
                members = (MemberInfo[]) memberList.ToArray(typeof(MemberInfo));
            }
                        
            PropertyDescriptorCollection logicalProperties = new PropertyDescriptorCollection(null);
            
            int index = 0;
            
            foreach (MemberInfo member in members)
            {
                FieldInfo field = member as FieldInfo;
                string name = names != null && index < names.Length ? names[index] : null;

                if (field != null)
                {
                    //
                    // Add public fields that are not read-only and not 
                    // constant literals.
                    //
            
                    if (field.DeclaringType != type && field.ReflectedType != type)
                        throw new ArgumentException("fields");
                
                    if (!field.IsInitOnly && !field.IsLiteral)
                        logicalProperties.Add(new TypeFieldDescriptor(field, name));
                }
                else
                {
                    PropertyInfo property = member as PropertyInfo;
                    
                    if (property != null)
                    {
                        //
                        // Add public properties that can be read and modified.
                        //

                        if (property.DeclaringType != type && property.ReflectedType != type)
                            throw new ArgumentException("properties");

                        if (property.CanRead && property.CanWrite)
                            logicalProperties.Add(new TypePropertyDescriptor(property, name));
                    }
                    else
                    {
                        throw new ArgumentException("members");
                    }
                }
                
                index++;
            }
                
            _properties = logicalProperties;
        }

        public PropertyDescriptorCollection GetProperties()
        {
            return _properties;
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            throw new NotImplementedException(); // FIXME: Incomplete implementation!
        }

        #region Uninteresting implementations of ICustomTypeDescriptor members

        public AttributeCollection GetAttributes()
        {
            return AttributeCollection.Empty;
        }

        public string GetClassName()
        {
            return null;
        }

        public string GetComponentName()
        {
            return null;
        }

        public TypeConverter GetConverter()
        {
            return new TypeConverter();
        }

        public EventDescriptor GetDefaultEvent()
        {
            return null;
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return null;
        }

        public object GetEditor(Type editorBaseType)
        {
            return null;
        }

        public EventDescriptorCollection GetEvents()
        {
            return EventDescriptorCollection.Empty;
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return EventDescriptorCollection.Empty;
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return null;
        }

        #endregion

        /// <summary>
        /// A base <see cref="PropertyDescriptor"/> implementation for
        /// a type member (<see cref="MemberInfo"/>).
        /// </summary>

        private abstract class TypeMemberDescriptor : PropertyDescriptor
        {
            protected TypeMemberDescriptor(MemberInfo member, string name) : 
                base(ChooseName(name, member.Name), null) {}

            protected abstract MemberInfo Member { get; }
                
            public override bool Equals(object obj)
            {
                TypeMemberDescriptor other = obj as TypeMemberDescriptor;
                return other != null && other.Member.Equals(Member);
            }

            public override int GetHashCode() { return Member.GetHashCode(); }
            public override bool IsReadOnly { get { return false; } }
            public override void ResetValue(object component) {}
            public override bool CanResetValue(object component) { return false; }
            public override bool ShouldSerializeValue(object component) { return true; }
            public override Type ComponentType { get { return Member.DeclaringType; } }
                
            public abstract override Type PropertyType { get; }
            public abstract override object GetValue(object component);
            public abstract override void SetValue(object component, object value);

            private static string ChooseName(string propsedName, string baseName)
            {
                if (Mask.NullString(propsedName).Length > 0)
                    return propsedName;
                
                return ToCamelCase(baseName);
            }

            private static string ToCamelCase(string s)
            {
                if (s == null || s.Length == 0)
                    return s;
                
                return char.ToLower(s[0], CultureInfo.InvariantCulture) + s.Substring(1);
            }
        }

        /// <summary>
        /// A <see cref="PropertyDescriptor"/> implementation around
        /// <see cref="FieldInfo"/>.
        /// </summary>

        private sealed class TypeFieldDescriptor : TypeMemberDescriptor
        {
            private readonly FieldInfo _field;

            public TypeFieldDescriptor(FieldInfo field, string name) : 
                base(field, name)
            {
                _field = field;
            }

            protected override MemberInfo Member
            {
                get { return _field; }
            }
                
            public override Type PropertyType
            {
                get { return _field.FieldType; }
            }

            public override object GetValue(object component)
            {
                return _field.GetValue(component);
            }

            public override void SetValue(object component, object value) 
            {
                _field.SetValue(component, value); 
                OnValueChanged(component, EventArgs.Empty);
            }
        }
            
        /// <summary>
        /// A <see cref="PropertyDescriptor"/> implementation around
        /// <see cref="PropertyInfo"/>.
        /// </summary>

        private sealed class TypePropertyDescriptor : TypeMemberDescriptor
        {
            private readonly PropertyInfo _property;

            public TypePropertyDescriptor(PropertyInfo property, string name) : 
                base(property, name)
            {
                _property = property;
            }

            protected override MemberInfo Member
            {
                get { return _property; }
            }

            public override Type PropertyType
            {
                get { return _property.PropertyType; }
            }

            public override object GetValue(object component)
            {
                return _property.GetValue(component, null);
            }

            public override void SetValue(object component, object value) 
            {
                _property.SetValue(component, value, null); 
                OnValueChanged(component, EventArgs.Empty);
            }
        }
    }
}