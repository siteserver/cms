using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiAdvertCommissionAdvertQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiAdvertCommissionAdvertQueryModel : AopObject
    {
        /// <summary>
        /// 查询主键列表  identify_type=advert所有值都必须是推广ID  identify_type=mission所有值都必须是分佣任务ID  identify_type=voucher所有值都必须是券ID
        /// </summary>
        [XmlArray("identifies")]
        [XmlArrayItem("string")]
        public List<string> Identifies { get; set; }

        /// <summary>
        /// 查询主键类型(枚举值key对应于请求对象中查询主键列表的key)  advert-推广  mission-分佣任务  voucher-券
        /// </summary>
        [XmlElement("identify_type")]
        public string IdentifyType { get; set; }
    }
}
