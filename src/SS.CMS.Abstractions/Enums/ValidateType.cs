using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SS.CMS.Abstractions.Enums
{
    /// <summary>
    /// 表单输入的验证规则类型。
    /// </summary>
    [JsonConverter(typeof(ValidateTypeConverter))]
    public class ValidateType : IEquatable<ValidateType>, IComparable<ValidateType>
    {
        /// <summary>
        /// 无验证。
        /// </summary>
        public static readonly ValidateType None = new ValidateType(nameof(None));

        /// <summary>
        /// 中文验证。
        /// </summary>
        public static readonly ValidateType Chinese = new ValidateType(nameof(Chinese));

        /// <summary>
        /// 英文验证。
        /// </summary>
        public static readonly ValidateType English = new ValidateType(nameof(English));

        /// <summary>
        /// Email格式验证。
        /// </summary>
        public static readonly ValidateType Email = new ValidateType(nameof(Email));

        /// <summary>
        /// 网址格式验证。
        /// </summary>
        public static readonly ValidateType Url = new ValidateType(nameof(Url));

        /// <summary>
        /// 电话号码验证。
        /// </summary>
        public static readonly ValidateType Phone = new ValidateType(nameof(Phone));

        /// <summary>
        /// 手机号码验证。
        /// </summary>
        public static readonly ValidateType Mobile = new ValidateType(nameof(Mobile));

        /// <summary>
        /// 整数验证。
        /// </summary>
        public static readonly ValidateType Integer = new ValidateType(nameof(Integer));

        /// <summary>
        /// 货币格式验证。
        /// </summary>
        public static readonly ValidateType Currency = new ValidateType(nameof(Currency));

        /// <summary>
        /// 邮政编码验证。
        /// </summary>
        public static readonly ValidateType Zip = new ValidateType(nameof(Zip));

        /// <summary>
        /// 身份证号码验证。
        /// </summary>
        public static readonly ValidateType IdCard = new ValidateType(nameof(IdCard));

        /// <summary>
        /// 正则表达式验证。
        /// </summary>
        public static readonly ValidateType RegExp = new ValidateType(nameof(RegExp));

        public static string GetText(ValidateType type)
        {
            if (type == ValidateType.None)
            {
                return "无";
            }
            if (type == ValidateType.Chinese)
            {
                return "中文";
            }
            if (type == ValidateType.English)
            {
                return "英文";
            }
            if (type == ValidateType.Email)
            {
                return "Email格式";
            }
            if (type == ValidateType.Url)
            {
                return "网址格式";
            }
            if (type == ValidateType.Phone)
            {
                return "电话号码";
            }
            if (type == ValidateType.Mobile)
            {
                return "手机号码";
            }
            if (type == ValidateType.Integer)
            {
                return "整数";
            }
            if (type == ValidateType.Currency)
            {
                return "货币格式";
            }
            if (type == ValidateType.Zip)
            {
                return "邮政编码";
            }
            if (type == ValidateType.IdCard)
            {
                return "身份证号码";
            }
            if (type == ValidateType.RegExp)
            {
                return "正则表达式验证";
            }
            throw new Exception();
        }

        public static ValidateType GetValidateType(string typeStr)
        {
            var retVal = ValidateType.None;

            if (Equals(ValidateType.None, typeStr))
            {
                retVal = ValidateType.None;
            }
            else if (Equals(ValidateType.Chinese, typeStr))
            {
                retVal = ValidateType.Chinese;
            }
            else if (Equals(ValidateType.Currency, typeStr))
            {
                retVal = ValidateType.Currency;
            }
            else if (Equals(ValidateType.RegExp, typeStr))
            {
                retVal = ValidateType.RegExp;
            }
            else if (Equals(ValidateType.Email, typeStr))
            {
                retVal = ValidateType.Email;
            }
            else if (Equals(ValidateType.English, typeStr))
            {
                retVal = ValidateType.English;
            }
            else if (Equals(ValidateType.IdCard, typeStr))
            {
                retVal = ValidateType.IdCard;
            }
            else if (Equals(ValidateType.Integer, typeStr))
            {
                retVal = ValidateType.Integer;
            }
            else if (Equals(ValidateType.Mobile, typeStr))
            {
                retVal = ValidateType.Mobile;
            }
            else if (Equals(ValidateType.Phone, typeStr))
            {
                retVal = ValidateType.Phone;
            }
            else if (Equals(ValidateType.Url, typeStr))
            {
                retVal = ValidateType.Url;
            }
            else if (Equals(ValidateType.Zip, typeStr))
            {
                retVal = ValidateType.Zip;
            }

            return retVal;
        }

        internal ValidateType(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            Value = value;
        }

        /// <summary>
        /// 验证规则的值。
        /// </summary>
        public string Value { get; }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as ValidateType);
        }

        /// <summary>
        /// 比较两个验证规则是否一致。
        /// </summary>
        /// <param name="a">需要比较的验证规则。</param>
        /// <param name="b">需要比较的验证规则。</param>
        /// <returns>如果一致，则为true；否则为false。</returns>
        public static bool operator ==(ValidateType a, ValidateType b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if ((object)a == null || (object)b == null)
            {
                return false;
            }

            return a.Equals(b);
        }
        /// <summary>
        /// 比较两个验证规则是否不一致。
        /// </summary>
        /// <param name="a">需要比较的验证规则。</param>
        /// <param name="b">需要比较的验证规则。</param>
        /// <returns>如果不一致，则为true；否则为false。</returns>
        public static bool operator !=(ValidateType a, ValidateType b)
        {
            return !(a == b);
        }
        /// <summary>
        /// 比较两个验证规则是否一致。
        /// </summary>
        /// <param name="other">需要比较的验证规则。</param>
        /// <returns>如果一致，则为true；否则为false。</returns>
        public bool Equals(ValidateType other)
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
        /// 比较两个验证规则是否一致。
        /// </summary>
        /// <param name="other">需要比较的验证规则。</param>
        /// <returns>如果一致，则为0；否则为1。</returns>
        public int CompareTo(ValidateType other)
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
    /// 字符串与ValidateType转换类。
    /// </summary>
    public class ValidateTypeConverter : JsonConverter
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
            return objectType == typeof(ValidateType);
        }

        /// <summary>
        /// 编写对象的JSON表示。
        /// </summary>
        /// <param name="writer">JsonWriter</param>
        /// <param name="value">值</param>
        /// <param name="serializer">序列化类</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var validateType = value as ValidateType;
            serializer.Serialize(writer, validateType != null ? validateType.Value : null);
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
            return string.IsNullOrEmpty(value) ? null : new ValidateType(value);
        }
    }
}