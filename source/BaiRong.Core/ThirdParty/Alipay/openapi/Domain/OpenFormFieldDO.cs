using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// OpenFormFieldDO Data Structure.
    /// </summary>
    [Serializable]
    public class OpenFormFieldDO : AopObject
    {
        /// <summary>
        /// 表单可选字段配置，common_fields属性定义一个表单字段数组，表单字段有效值列表与required字段有效值列表相同。  可选字段配置中不能含有必须字段配置的有效值。
        /// </summary>
        [XmlElement("optional")]
        public string Optional { get; set; }

        /// <summary>
        /// 表单必填字段配置，common_fields属性定义一个表单字段数组，字段有效值如下列表所示：  OPEN_FORM_FIELD_MOBILE -- 手机号  OPEN_FORM_FIELD_GENDER -- 性别  OPEN_FORM_FIELD_NAME -- 姓名  OPEN_FORM_FIELD_BIRTHDAY -- 生日  OPEN_FORM_FIELD_IDCARD -- 身份证  OPEN_FORM_FIELD_EMAIL -- 邮箱  OPEN_FORM_FIELD_ADDRESS -- 地址  OPEN_FORM_FIELD_CITY -- 城市  OPEN_FORM_FIELD_IS_STUDENT -- 是否学生认证  OPEN_FORM_FIELD_MEMBER_GRADE -- 会员等级
        /// </summary>
        [XmlElement("required")]
        public string Required { get; set; }
    }
}
