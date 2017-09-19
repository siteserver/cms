using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayOfflineMarketApplyorderBatchqueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayOfflineMarketApplyorderBatchqueryModel : AopObject
    {
        /// <summary>
        /// 操作动作
        /// </summary>
        [XmlElement("action")]
        public string Action { get; set; }

        /// <summary>
        /// 支付宝流水ID列表，最大不超过100个
        /// </summary>
        [XmlArray("apply_ids")]
        [XmlArrayItem("string")]
        public List<string> ApplyIds { get; set; }

        /// <summary>
        /// 业务主体ID。根据biz_type不同可能对应shop_id或item_id。
        /// </summary>
        [XmlElement("biz_id")]
        public string BizId { get; set; }

        /// <summary>
        /// 业务类型：SHOP-店铺，ITEM-商品。
        /// </summary>
        [XmlElement("biz_type")]
        public string BizType { get; set; }

        /// <summary>
        /// 查询的流水创建时间最后值。格式：yyyy-MM-dd HH:mm:ss
        /// </summary>
        [XmlElement("end_time")]
        public string EndTime { get; set; }

        /// <summary>
        /// 操作用户的支付账号id
        /// </summary>
        [XmlElement("op_id")]
        public string OpId { get; set; }

        /// <summary>
        /// 系统集成商统一传入ISV
        /// </summary>
        [XmlElement("op_role")]
        public string OpRole { get; set; }

        /// <summary>
        /// 页码，留空标示第一页，默认20个结果为一页
        /// </summary>
        [XmlElement("page_no")]
        public long PageNo { get; set; }

        /// <summary>
        /// 每页记录数。默认20，最大100。
        /// </summary>
        [XmlElement("page_size")]
        public long PageSize { get; set; }

        /// <summary>
        /// 请求ID列表，最大不超过100个。  注意：暂时不支持此字段查询。
        /// </summary>
        [XmlArray("request_ids")]
        [XmlArrayItem("string")]
        public List<string> RequestIds { get; set; }

        /// <summary>
        /// 查询的流水创建时间起始值，只能查询近3个月数据。格式：yyyy-MM-dd HH:mm:ss
        /// </summary>
        [XmlElement("start_time")]
        public string StartTime { get; set; }

        /// <summary>
        /// 流水状态：INIT-初始，PROCESS-处理中，SUCCESS-成功，FAIL-失败。
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }
    }
}
