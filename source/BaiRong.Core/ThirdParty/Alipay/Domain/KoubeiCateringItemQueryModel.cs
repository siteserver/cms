using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiCateringItemQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiCateringItemQueryModel : AopObject
    {
        /// <summary>
        /// 服务商、服务商员工、商户、商户员工等口碑角色操作时必填，对应为《koubei.member.data.oauth.query》中的auth_code，默认有效期24小时；isv自身角色操作的时候，无需传该参数
        /// </summary>
        [XmlElement("auth_code")]
        public string AuthCode { get; set; }

        /// <summary>
        /// 口碑体系内部商品的唯一标识
        /// </summary>
        [XmlElement("item_id")]
        public string ItemId { get; set; }

        /// <summary>
        /// 操作人员身份类型。如果是isv代操作，请传入ISV；如果是商户操作请传入MERCHANT；如果是商户员工则传入M_STAFF
        /// </summary>
        [XmlElement("operator_type")]
        public string OperatorType { get; set; }

        /// <summary>
        /// 请求id。支持英文字母和数字，由开发者自行定义（不允许重复）。比如2016102903214476899999999
        /// </summary>
        [XmlElement("request_id")]
        public string RequestId { get; set; }
    }
}
