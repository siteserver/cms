using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// LicenseInfo Data Structure.
    /// </summary>
    [Serializable]
    public class LicenseInfo : AopObject
    {
        /// <summary>
        /// 证书的认证机构
        /// </summary>
        [XmlElement("agency")]
        public string Agency { get; set; }

        /// <summary>
        /// 证照过期时间，格式：yyyy-MM-dd
        /// </summary>
        [XmlElement("gmt_expire")]
        public string GmtExpire { get; set; }

        /// <summary>
        /// 证照的起始时间：证件生效的开始时间，格式：yyyy-MM-dd
        /// </summary>
        [XmlElement("gmt_start")]
        public string GmtStart { get; set; }

        /// <summary>
        /// 证书id
        /// </summary>
        [XmlElement("license_id")]
        public string LicenseId { get; set; }

        /// <summary>
        /// 证照名称，当type为OTHER时，必填
        /// </summary>
        [XmlElement("license_name")]
        public string LicenseName { get; set; }

        /// <summary>
        /// 证书照片的url
        /// </summary>
        [XmlElement("pic_url")]
        public string PicUrl { get; set; }

        /// <summary>
        /// 技能或者能力的认证结果，如“高级”
        /// </summary>
        [XmlElement("result")]
        public string Result { get; set; }

        /// <summary>
        /// 服务者的证书编号，由证书机构颁发的证书编号
        /// </summary>
        [XmlElement("sequence")]
        public string Sequence { get; set; }

        /// <summary>
        /// 证照类型，允许以下值：  TOUR_GUIDE：导游证  LEGAL：法律职业资格证书  COUNSELOR：心理咨询师  DRIVER_TRAIN：机动车驾驶员培训许可证  CHEF：厨师证  TEACHER：教师资格证  LIFE_SAVING：救生证  FINANCIAL_PLANNER：理财规划师  FINANCIAL_MANAGEMENT：金融理财师  BANK：银行从业资格  SECURITIES：证券从业资格  INSURANCE：保险从业资格  FUTURES：期货从业资格  FUND：基金从业资格  SPECIAL：特种经营许可证  POLICE_REGISTER：公安备案登记证明  LOCKS_REPAIR：锁具修理服务卡（公安印章）  HEALTH：健康证  BEAUTY：美容相关证件  MASSAGE：按摩师职业证书  TRANSPORT：道路运输证  DRIVING：驾驶证  TRANSPORT_PERMIT：道路运输经营许可  OTHER：其他
        /// </summary>
        [XmlElement("type")]
        public string Type { get; set; }
    }
}
