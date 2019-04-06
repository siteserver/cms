using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SiteServer.Plugin
{
    /// <summary>
    /// 可能发生在文件或数据中的更改类型。
    /// </summary>
    [JsonConverter(typeof(ChangeTypeConverter))]
    public class ChangeType : IEquatable<ChangeType>, IComparable<ChangeType>
    {
        /// <summary>
        /// 新增
        /// </summary>
        public static readonly ChangeType Created = new ChangeType(nameof(Created));

        /// <summary>
        /// 修改
        /// </summary>
        public static readonly ChangeType Modified = new ChangeType(nameof(Modified));

        /// <summary>
        /// 删除
        /// </summary>
        public static readonly ChangeType Deleted = new ChangeType(nameof(Deleted));

        internal ChangeType(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            Value = value;
        }

        /// <summary>
        /// 变动类型的值。
        /// </summary>
        public string Value { get; }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as ChangeType);
        }

        /// <summary>
        /// 比较两个变动类型是否一致。
        /// </summary>
        /// <param name="a">需要比较的变动类型。</param>
        /// <param name="b">需要比较的变动类型。</param>
        /// <returns>如果一致，则为true；否则为false。</returns>
        public static bool operator ==(ChangeType a, ChangeType b)
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
        /// 比较两个变动类型是否不一致。
        /// </summary>
        /// <param name="a">需要比较的变动类型。</param>
        /// <param name="b">需要比较的变动类型。</param>
        /// <returns>如果不一致，则为true；否则为false。</returns>
        public static bool operator !=(ChangeType a, ChangeType b)
        {
            return !(a == b);
        }

        /// <summary>
        /// 比较两个变动类型是否一致。
        /// </summary>
        /// <param name="other">需要比较的变动类型。</param>
        /// <returns>如果一致，则为true；否则为false。</returns>
        public bool Equals(ChangeType other)
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
        /// 比较两个变动类型是否一致。
        /// </summary>
        /// <param name="other">需要比较的变动类型。</param>
        /// <returns>如果一致，则为0；否则为1。</returns>
        public int CompareTo(ChangeType other)
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
    /// 字符串与ChangeType转换类。
    /// </summary>
    public class ChangeTypeConverter : JsonConverter
    {
        /// <summary>
        /// 确定此实例是否可以转换指定的对象类型。
        /// </summary>
        /// <param name="objectType">对象实例</param>
        /// <returns>
        /// <c>true</c> 如果这个实例可以转换指定的对象类型; 否则, <c>false</c>。
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ChangeType);
        }

        /// <summary>
        /// 编写对象的JSON表示。
        /// </summary>
        /// <param name="writer">JsonWriter</param>
        /// <param name="value">值</param>
        /// <param name="serializer">序列化类</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var databaseType = value as ChangeType;
            serializer.Serialize(writer, databaseType != null ? databaseType.Value : null);
        }

        /// <summary>
        /// 读取对象的JSON表示。
        /// </summary>
        /// <param name="reader">JsonReader</param>
        /// <param name="objectType">对象类型</param>
        /// <param name="existingValue">正在读取的对象的现有值</param>
        /// <param name="serializer">序列化类</param>
        /// <returns>返回对象</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var value = (string)reader.Value;
            return string.IsNullOrEmpty(value) ? null : new ChangeType(value);
        }
    }
}