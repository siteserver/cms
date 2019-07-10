using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SiteServer.Plugin
{
    /// <summary>
    /// 系统支持的模板类型。
    /// </summary>
    [JsonConverter(typeof(TemplateTypeConverter))]
    public class TemplateType : IEquatable<TemplateType>, IComparable<TemplateType>
    {
        /// <summary>
        /// 首页模板。
        /// </summary>
        public static readonly TemplateType IndexPageTemplate = new TemplateType(nameof(IndexPageTemplate));

        /// <summary>
        /// 栏目模板。
        /// </summary>
        public static readonly TemplateType ChannelTemplate = new TemplateType(nameof(ChannelTemplate));

        /// <summary>
        /// 内容模板。
        /// </summary>
        public static readonly TemplateType ContentTemplate = new TemplateType(nameof(ContentTemplate));

        /// <summary>
        /// 单页模板。
        /// </summary>
        public static readonly TemplateType FileTemplate = new TemplateType(nameof(FileTemplate));

        internal TemplateType(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            Value = value;
        }

        /// <summary>
        /// 数据类型的值。
        /// </summary>
        public string Value { get; }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as TemplateType);
        }

        /// <summary>
        /// 比较两个数据类型是否一致。
        /// </summary>
        /// <param name="a">需要比较的数据类型。</param>
        /// <param name="b">需要比较的数据类型。</param>
        /// <returns>如果一致，则为true；否则为false。</returns>
        public static bool operator ==(TemplateType a, TemplateType b)
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
        /// 比较两个数据类型是否不一致。
        /// </summary>
        /// <param name="a">需要比较的数据类型。</param>
        /// <param name="b">需要比较的数据类型。</param>
        /// <returns>如果不一致，则为true；否则为false。</returns>
        public static bool operator !=(TemplateType a, TemplateType b)
        {
            return !(a == b);
        }

        /// <summary>
        /// 比较两个数据类型是否一致。
        /// </summary>
        /// <param name="other">需要比较的数据类型。</param>
        /// <returns>如果一致，则为true；否则为false。</returns>
        public bool Equals(TemplateType other)
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
        /// 比较两个数据类型是否一致。
        /// </summary>
        /// <param name="other">需要比较的数据类型。</param>
        /// <returns>如果一致，则为0；否则为1。</returns>
        public int CompareTo(TemplateType other)
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
    /// 字符串与TemplateType转换类。
    /// </summary>
    public class TemplateTypeConverter : JsonConverter
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
            return objectType == typeof(TemplateType);
        }

        /// <summary>
        /// 编写对象的JSON表示。
        /// </summary>
        /// <param name="writer">JsonWriter</param>
        /// <param name="value">值</param>
        /// <param name="serializer">序列化类</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var templateType = value as TemplateType;
            serializer.Serialize(writer, templateType != null ? templateType.Value : null);
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
            return string.IsNullOrEmpty(value) ? null : new TemplateType(value);
        }
    }
}