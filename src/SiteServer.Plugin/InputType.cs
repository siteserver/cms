using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SiteServer.Plugin
{
    /// <summary>
    /// 表单的输入类型。
    /// </summary>
    [JsonConverter(typeof(InputTypeConverter))]
    public class InputType : IEquatable<InputType>, IComparable<InputType>
    {
        /// <summary>
        /// 文本输入框。
        /// </summary>
        public static readonly InputType Text = new InputType(nameof(Text));

        /// <summary>
        /// 多行文本输入框。
        /// </summary>
        public static readonly InputType TextArea = new InputType(nameof(TextArea));

        /// <summary>
        /// 富文本编辑器。
        /// </summary>
        public static readonly InputType TextEditor = new InputType(nameof(TextEditor));

        /// <summary>
        /// 多选项。
        /// </summary>
        public static readonly InputType CheckBox = new InputType(nameof(CheckBox));

        /// <summary>
        /// 单选项。
        /// </summary>
        public static readonly InputType Radio = new InputType(nameof(Radio));

        /// <summary>
        /// 单选下拉框。
        /// </summary>
        public static readonly InputType SelectOne = new InputType(nameof(SelectOne));

        /// <summary>
        /// 多选下拉框。
        /// </summary>
        public static readonly InputType SelectMultiple = new InputType(nameof(SelectMultiple));

        /// <summary>
        /// 级联选择下拉框。
        /// </summary>
        public static readonly InputType SelectCascading = new InputType(nameof(SelectCascading));

        /// <summary>
        /// 日期选择框。
        /// </summary>
        public static readonly InputType Date = new InputType(nameof(Date));

        /// <summary>
        /// 日期及时间选择框。
        /// </summary>
        public static readonly InputType DateTime = new InputType(nameof(DateTime));

        /// <summary>
        /// 图片上传控件。
        /// </summary>
        public static readonly InputType Image = new InputType(nameof(Image));

        /// <summary>
        /// 音视频上传控件。
        /// </summary>
        public static readonly InputType Video = new InputType(nameof(Video));

        /// <summary>
        /// 文件上传控件。
        /// </summary>
        public static readonly InputType File = new InputType(nameof(File));

        /// <summary>
        /// 自定义输入控件。
        /// </summary>
        public static readonly InputType Customize = new InputType(nameof(Customize));

        /// <summary>
        /// 隐藏项。
        /// </summary>
        public static readonly InputType Hidden = new InputType(nameof(Hidden));

        internal InputType(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            Value = value;
        }

        /// <summary>
        /// 输入类型的值。
        /// </summary>
        public string Value { get; }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as InputType);
        }

        /// <summary>
        /// 比较两个输入类型是否一致。
        /// </summary>
        /// <param name="a">需要比较的输入类型。</param>
        /// <param name="b">需要比较的输入类型。</param>
        /// <returns>如果一致，则为true；否则为false。</returns>
        public static bool operator ==(InputType a, InputType b)
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
        /// 比较两个输入类型是否不一致。
        /// </summary>
        /// <param name="a">需要比较的输入类型。</param>
        /// <param name="b">需要比较的输入类型。</param>
        /// <returns>如果不一致，则为true；否则为false。</returns>
        public static bool operator !=(InputType a, InputType b)
        {
            return !(a == b);
        }

        /// <summary>
        /// 比较两个输入类型是否一致。
        /// </summary>
        /// <param name="other">需要比较的输入类型。</param>
        /// <returns>如果一致，则为true；否则为false。</returns>
        public bool Equals(InputType other)
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
        /// 比较两个输入类型是否一致。
        /// </summary>
        /// <param name="other">需要比较的输入类型。</param>
        /// <returns>如果一致，则为0；否则为1。</returns>
        public int CompareTo(InputType other)
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
    /// 字符串与InputType转换类。
    /// </summary>
    public class InputTypeConverter : JsonConverter
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
            return objectType == typeof(InputType);
        }

        /// <summary>
        /// 编写对象的JSON表示。
        /// </summary>
        /// <param name="writer">JsonWriter</param>
        /// <param name="value">值</param>
        /// <param name="serializer">序列化类</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var inputType = value as InputType;
            serializer.Serialize(writer, inputType != null ? inputType.Value : null);
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
            return string.IsNullOrEmpty(value) ? null : new InputType(value);
        }
    }
}