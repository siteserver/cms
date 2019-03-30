using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Datory
{
    /// <summary>
    /// Type of database supported by the datory
    /// </summary>
    [JsonConverter(typeof(DatabaseTypeConverter))]
    public class DatabaseType : IEquatable<DatabaseType>, IComparable<DatabaseType>
    {
        /// <summary>
        /// MySql Database Type
        /// </summary>
        public static readonly DatabaseType MySql = new DatabaseType(nameof(MySql));

        /// <summary>
        /// SqlServer Database Type
        /// </summary>
        public static readonly DatabaseType SqlServer = new DatabaseType(nameof(SqlServer));

        /// <summary>
        /// PostgreSql Database Type
        /// </summary>
        public static readonly DatabaseType PostgreSql = new DatabaseType(nameof(PostgreSql));

        /// <summary>
        /// Oracle Database Type
        /// </summary>
        public static readonly DatabaseType Oracle = new DatabaseType(nameof(Oracle));

        internal DatabaseType(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            Value = value;
        }

        /// <summary>
        /// The value of the database type
        /// </summary>
        public string Value { get; }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as DatabaseType);
        }

        /// <summary>
        /// Compares whether two types are the same.
        /// </summary>
        /// <param name="a">The type of database</param>
        /// <param name="b">The type of database</param>
        /// <returns>True if it is the same; Otherwise return false</returns>
        public static bool operator ==(DatabaseType a, DatabaseType b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if ((object) a == null || (object) b == null)
            {
                return false;
            }

            return a.Equals(b);
        }

        /// <summary>
        /// Compares whether two types are the same.
        /// </summary>
        /// <param name="a">The type of database</param>
        /// <param name="b">The type of database</param>
        /// <returns>True if it is the same; Otherwise return false</returns>
        public static bool operator !=(DatabaseType a, DatabaseType b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Compares whether two types are the same.
        /// </summary>
        /// <param name="other">The type of database</param>
        /// <returns>True if it is the same; Otherwise return false</returns>
        public bool Equals(DatabaseType other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return
                Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Compares whether two types are the same.
        /// </summary>
        /// <param name="other">The type of database</param>
        /// <returns>True if it is the same; Otherwise return false</returns>
        public int CompareTo(DatabaseType other)
        {
            if (other == null)
            {
                return 1;
            }

            if (ReferenceEquals(this, other))
            {
                return 0;
            }

            return StringComparer.OrdinalIgnoreCase.Compare(Value, other.Value);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return EqualityComparer<string>.Default.GetHashCode(Value);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Value;
        }
    }

    /// <summary>
    /// Json conversion class.
    /// </summary>
    public class DatabaseTypeConverter : JsonConverter
    {
        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Object type</param>
        /// <returns>
        /// true if the instance can convert the specified object type; Otherwise, false.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DatabaseType);
        }

        /// <summary>
        /// Write JSON of the object.
        /// </summary>
        /// <param name="writer">JsonWriter</param>
        /// <param name="value">ֵ</param>
        /// <param name="serializer">Serialization class</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var databaseType = value as DatabaseType;
            serializer.Serialize(writer, databaseType != null ? databaseType.Value : null);
        }

        /// <summary>
        /// Read JSON of the object.
        /// </summary>
        /// <param name="reader">JsonReader</param>
        /// <param name="objectType">Object type</param>
        /// <param name="existingValue">The existing value of the object being read</param>
        /// <param name="serializer">Serialization class</param>
        /// <returns>Object instance</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var value = (string)reader.Value;
            return string.IsNullOrEmpty(value) ? null : new DatabaseType(value);
        }
    }
}