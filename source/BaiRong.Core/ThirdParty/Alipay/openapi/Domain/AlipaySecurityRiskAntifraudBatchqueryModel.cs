using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipaySecurityRiskAntifraudBatchqueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipaySecurityRiskAntifraudBatchqueryModel : AopObject
    {
        /// <summary>
        /// company_list+传入的一批待检查的企业名单+用户传入+还可以传入{"creditCode":"企业信用代码"}或者{"regNo":"企业工商注册号"}
        /// </summary>
        [XmlArray("company_list")]
        [XmlArrayItem("string")]
        public List<string> CompanyList { get; set; }

        /// <summary>
        /// partner_name+唯一+作为标识调用者身份的字段+用户填入
        /// </summary>
        [XmlElement("partner_name")]
        public string PartnerName { get; set; }

        /// <summary>
        /// staff_list+传入的一批待检查员工信息+用户传入+手机号/身份证姓名二选一+还可以传入{"name":"姓名","phone":"手机号码"}
        /// </summary>
        [XmlArray("staff_list")]
        [XmlArrayItem("string")]
        public List<string> StaffList { get; set; }
    }
}
