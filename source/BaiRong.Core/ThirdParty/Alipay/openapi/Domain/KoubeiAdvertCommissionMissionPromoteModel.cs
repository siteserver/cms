using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiAdvertCommissionMissionPromoteModel Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiAdvertCommissionMissionPromoteModel : AopObject
    {
        /// <summary>
        /// 推广主键  identify_type=mission-值必须是任务ID  identify_type=voucher-值必须是券ID
        /// </summary>
        [XmlElement("identify")]
        public string Identify { get; set; }

        /// <summary>
        /// 推广主键类型  mission-任务  voucher-券
        /// </summary>
        [XmlElement("identify_type")]
        public string IdentifyType { get; set; }

        /// <summary>
        /// 指定推广活动的名称
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }
    }
}
