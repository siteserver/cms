using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiCateringItemlistQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiCateringItemlistQueryModel : AopObject
    {
        /// <summary>
        /// 服务商、服务商员工、商户、商户员工等口碑角色操作时必填，对应为《koubei.member.data.oauth.query》中的auth_code，默认有效期24小时；isv自身角色操作的时候，无需传该参数
        /// </summary>
        [XmlElement("auth_code")]
        public string AuthCode { get; set; }

        /// <summary>
        /// 查询特定状态的商品。状态枚举值为：INIT表示未上架，EFFECTIVE表示已上架，PAUSE表示已暂停, FREEZE表示已冻结, INVALID表示已下架。如果为空则默认查询所有状态商品
        /// </summary>
        [XmlElement("item_status")]
        public string ItemStatus { get; set; }

        /// <summary>
        /// 操作人员身份类型。如果是isv代操作，请传入ISV；如果是商户操作请传入MERCHANT；如果是商户员工则传入M_STAFF
        /// </summary>
        [XmlElement("operator_type")]
        public string OperatorType { get; set; }

        /// <summary>
        /// 页码数，整数，表示需要查询第几页数据。
        /// </summary>
        [XmlElement("page_no")]
        public long PageNo { get; set; }

        /// <summary>
        /// 列表每页显示商品的条目数，整数
        /// </summary>
        [XmlElement("page_size")]
        public long PageSize { get; set; }

        /// <summary>
        /// 请求id。支持英文字母和数字，由开发者自行定义（不允许重复）
        /// </summary>
        [XmlElement("request_id")]
        public string RequestId { get; set; }
    }
}
