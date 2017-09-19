using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// BizOrderQueryResponse Data Structure.
    /// </summary>
    [Serializable]
    public class BizOrderQueryResponse : AopObject
    {
        /// <summary>
        /// 操作动作。  CREATE_SHOP-创建门店，  MODIFY_SHOP-修改门店，  CREATE_ITEM-创建商品，  MODIFY_ITEM-修改商品，  EFFECTIVE_ITEM-上架商品，  INVALID_ITEM-下架商品，  RESUME_ITEM-暂停售卖商品，  PAUSE_ITEM-恢复售卖商品
        /// </summary>
        [XmlElement("action")]
        public string Action { get; set; }

        /// <summary>
        /// 操作模式：NORMAL-普通开店
        /// </summary>
        [XmlElement("action_mode")]
        public string ActionMode { get; set; }

        /// <summary>
        /// 支付宝流水ID
        /// </summary>
        [XmlElement("apply_id")]
        public string ApplyId { get; set; }

        /// <summary>
        /// 流水上下文信息，JSON格式。根据action不同对应的结构也不同，JSON字段与含义可参考各个接口的请求参数。
        /// </summary>
        [XmlElement("biz_context_info")]
        public string BizContextInfo { get; set; }

        /// <summary>
        /// 业务主体ID。根据biz_type不同可能对应shop_id或item_id。  特别注意对于门店创建，当流水status=SUCCESS时，此字段才为shop_id，其他状态时为0或空。
        /// </summary>
        [XmlElement("biz_id")]
        public string BizId { get; set; }

        /// <summary>
        /// 业务类型：SHOP-店铺，ITEM-商品
        /// </summary>
        [XmlElement("biz_type")]
        public string BizType { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [XmlElement("create_time")]
        public string CreateTime { get; set; }

        /// <summary>
        /// 操作用户的支付账号id
        /// </summary>
        [XmlElement("op_id")]
        public string OpId { get; set; }

        /// <summary>
        /// 注意：此字段并非外部商户请求时传入的request_id，暂时代表支付宝内部字段，请勿用。
        /// </summary>
        [XmlElement("request_id")]
        public string RequestId { get; set; }

        /// <summary>
        /// 流水处理结果码  <a href="https://doc.open.alipay.com/doc2/detail.htm?spm=a219a.7629140.0.0.lL9hGI&treeId=78&articleId=103834&docType=1#s2">点此查看</a>
        /// </summary>
        [XmlElement("result_code")]
        public string ResultCode { get; set; }

        /// <summary>
        /// 流水处理结果描述
        /// </summary>
        [XmlElement("result_desc")]
        public string ResultDesc { get; set; }

        /// <summary>
        /// 流水状态：INIT-初始，PROCESS-处理中，SUCCESS-成功，FAIL-失败。
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }

        /// <summary>
        /// 流水子状态：WAIT_CERTIFY-等待认证，LICENSE_AUDITING-证照审核中，RISK_AUDITING-风控审核中，WAIT_SIGN-等待签约，FINISH-终结。
        /// </summary>
        [XmlElement("sub_status")]
        public string SubStatus { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [XmlElement("update_time")]
        public string UpdateTime { get; set; }
    }
}
