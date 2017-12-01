using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// QueryDetailAopResult Data Structure.
    /// </summary>
    [Serializable]
    public class QueryDetailAopResult : AopObject
    {
        /// <summary>
        /// 批次编号；此单据对应的批次编号
        /// </summary>
        [XmlElement("batch_no")]
        public string BatchNo { get; set; }

        /// <summary>
        /// 明细编号
        /// </summary>
        [XmlElement("detail_no")]
        public string DetailNo { get; set; }

        /// <summary>
        /// 扩展属性；创建付款单时传入的数据
        /// </summary>
        [XmlElement("ext_param")]
        public string ExtParam { get; set; }

        /// <summary>
        /// 失败原因：在status为fail时此字段不为空
        /// </summary>
        [XmlElement("fail_message")]
        public string FailMessage { get; set; }

        /// <summary>
        /// 明细最后一次变更的时间
        /// </summary>
        [XmlElement("last_modified")]
        public string LastModified { get; set; }

        /// <summary>
        /// 单据号
        /// </summary>
        [XmlElement("order_no")]
        public string OrderNo { get; set; }

        /// <summary>
        /// 收／付款金额；单位为元
        /// </summary>
        [XmlElement("pay_amount")]
        public string PayAmount { get; set; }

        /// <summary>
        /// 收款人userId
        /// </summary>
        [XmlElement("payee_id")]
        public string PayeeId { get; set; }

        /// <summary>
        /// 付款人userId
        /// </summary>
        [XmlElement("payer_id")]
        public string PayerId { get; set; }

        /// <summary>
        /// 服务费，单位为元
        /// </summary>
        [XmlElement("service_charge")]
        public string ServiceCharge { get; set; }

        /// <summary>
        /// 批次明细状态；注：AVAILABLE：可付款；SUCCESS：付款成功；FAIL：失败
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }
    }
}
