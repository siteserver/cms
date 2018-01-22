using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// InputFieldModel Data Structure.
    /// </summary>
    [Serializable]
    public class InputFieldModel : AopObject
    {
        /// <summary>
        /// 默认提示值，例如歌华宽带的金额为100的倍数
        /// </summary>
        [XmlElement("default_value")]
        public string DefaultValue { get; set; }

        /// <summary>
        /// 域英文名称 例如：billkey
        /// </summary>
        [XmlElement("field_name")]
        public string FieldName { get; set; }

        /// <summary>
        /// 输入框下方文字提示，例如： 户号为10位数字
        /// </summary>
        [XmlElement("field_tips")]
        public string FieldTips { get; set; }

        /// <summary>
        /// 页面显示提示 例如：手机号码
        /// </summary>
        [XmlElement("field_title")]
        public string FieldTitle { get; set; }

        /// <summary>
        /// 控件类型 输入框类型 例如：inputText (文本输入框)
        /// </summary>
        [XmlElement("field_type")]
        public string FieldType { get; set; }

        /// <summary>
        /// 控件展示顺序 例如：优选级 1
        /// </summary>
        [XmlElement("priority")]
        public string Priority { get; set; }

        /// <summary>
        /// 输入域的校验规则模型
        /// </summary>
        [XmlArray("regexp_rule_list")]
        [XmlArrayItem("validation_rule")]
        public List<ValidationRule> RegexpRuleList { get; set; }
    }
}
