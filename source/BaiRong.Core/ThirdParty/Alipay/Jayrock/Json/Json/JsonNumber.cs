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

namespace Jayrock.Json
{
    #region Imports

    using System;
    using System.Globalization;
    using Jayrock.Json.Conversion;

    #endregion
    
    /// <summary> 
    /// Represents a JSON Number.  This class models a number as a string 
    /// and only converts to a native numerical representation when needed 
    /// and therefore told.  
    /// </summary>
    /// <remarks>
    /// This class cannot be used to compare two numbers or perform
    /// mathematical operations like addition and substraction without 
    /// first converting to an actual native numerical data type.
    /// Use <see cref="LogicallyEquals"/> to test for equality.
    /// </remarks>

    [ Serializable ]
    public struct JsonNumber : IConvertible, IJsonExportable
    {
        private readonly string _value;

        public JsonNumber(string value)
        {
            if (value != null)
            {
                try
                {
                    double.Parse(value, NumberStyles.Float, CultureInfo.InvariantCulture);
                }
                catch (FormatException e)
                {
                    throw new ArgumentException(e.Message, "value");
                }
            }
            
            _value = value;
        }
        
        private string Value
        {
            get { return Mask.EmptyString(_value, "0"); }
        }
        
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Value.Equals(obj);
        }

        public override string ToString()
        {
            return Value;
        }
        
        public bool LogicallyEquals(object o)
        {
            if (o == null) 
                return false;
            
            return Convert.ChangeType(this, o.GetType(), CultureInfo.InvariantCulture).Equals(o);
        }

        //
        // IMPORTANT! The following ToXXX methods will throw 
        // OverflowException in case the JsonNumber instance contains a value
        // that is too big or small to be represented as the request type.
        //

        public bool ToBoolean()
        {
            return ToInt64() != 0;
        }

        public char ToChar()
        {
            return (char) Convert.ToUInt16(Value, CultureInfo.InvariantCulture);
        }

        public byte ToByte()
        {
            return Convert.ToByte(Value, CultureInfo.InvariantCulture);
        }

        public short ToInt16()
        {
            return Convert.ToInt16(Value, CultureInfo.InvariantCulture);
        }

        public int ToInt32()
        {
            return Convert.ToInt32(Value, CultureInfo.InvariantCulture);
        }

        public long ToInt64()
        {
            return Convert.ToInt64(Value, CultureInfo.InvariantCulture);
        }

        public float ToSingle()
        {
            return Convert.ToSingle(Value, CultureInfo.InvariantCulture);
        }

        public double ToDouble()
        {
            return Convert.ToDouble(Value, CultureInfo.InvariantCulture);
        }

        public decimal ToDecimal()
        {
            return Convert.ToDecimal(Value, CultureInfo.InvariantCulture);
        }

        public DateTime ToDateTime()
        {
            return UnixTime.ToDateTime(ToInt64());
        }
        
        #region IConvertible implementation

        TypeCode IConvertible.GetTypeCode()
        {
            return TypeCode.Object;
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            return ToBoolean();
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            return ToChar();
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            return Convert.ToSByte(ToInt32());
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            return ToByte();
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            return ToInt16();
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            return Convert.ToUInt16(Value, provider);
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            return ToInt32();
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            return Convert.ToUInt32(Value, provider);
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            return ToInt64();
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            return Convert.ToUInt64(Value, provider);
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            return ToSingle();
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            return ToDouble();
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            return ToDecimal();
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            return ToDateTime();
        }

        string IConvertible.ToString(IFormatProvider provider)
        {
            return ToString();
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            return Convert.ChangeType(this, conversionType, provider);
        }

        #endregion

        //
        // Explicit conversion operators.
        //
        
        public static explicit operator byte(JsonNumber number)
        {
            return number.ToByte();
        }

        public static explicit operator short(JsonNumber number)
        {
            return number.ToInt16();
        }

        public static explicit operator int(JsonNumber number)
        {
            return number.ToInt32();
        }
        
        public static explicit operator long(JsonNumber number)
        {
            return number.ToInt64();
        }
        
        public static explicit operator float(JsonNumber number)
        {
            return number.ToSingle();
        }
        
        public static explicit operator double(JsonNumber number)
        {
            return number.ToDouble();
        }
        
        public static explicit operator decimal(JsonNumber number)
        {
            return number.ToDecimal();
        }
        
        public static explicit operator DateTime(JsonNumber number)
        {
            return number.ToDateTime();
        }

        void IJsonExportable.Export(ExportContext context, JsonWriter writer)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (writer == null)
                throw new ArgumentNullException("writer");
            
            writer.WriteNumber(Value);
        }
    }
}