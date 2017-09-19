using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayUserAccountInstitutionCertifyModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayUserAccountInstitutionCertifyModel : AopObject
    {
        /// <summary>
        /// 描述机构的名称
        /// </summary>
        [XmlElement("institution_name")]
        public string InstitutionName { get; set; }

        /// <summary>
        /// 登录号，可以是手机号码或者邮箱号码
        /// </summary>
        [XmlElement("logon_id")]
        public string LogonId { get; set; }
    }
}
