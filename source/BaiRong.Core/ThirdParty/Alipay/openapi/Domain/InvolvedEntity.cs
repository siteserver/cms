using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// InvolvedEntity Data Structure.
    /// </summary>
    [Serializable]
    public class InvolvedEntity : AopObject
    {
        /// <summary>
        /// 实体身份编码-个人是身份证号码、企业是工商注册号、会员是会员编号-具体的数字编号
        /// </summary>
        [XmlElement("code")]
        public string Code { get; set; }

        /// <summary>
        /// 实体编码的类型。例如若实体为个人，编码可能为身份证，则code_type为“RESIDENT”；可能为户口簿，则code_type为“HOUSEHOLD”；若实体为ALIPAY，编码可能为支付宝ID,则code_type为“USER_ID”
        /// </summary>
        [XmlElement("code_type")]
        public string CodeType { get; set; }

        /// <summary>
        /// 实体的标识-个人是姓名、企业是公司名称等，会员是会员名称，如支付宝的手机号或者邮箱号
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// 对象的类型。  类型说明：值  个人:PERSON  企业:COMPANY
        /// </summary>
        [XmlElement("type")]
        public string Type { get; set; }
    }
}
