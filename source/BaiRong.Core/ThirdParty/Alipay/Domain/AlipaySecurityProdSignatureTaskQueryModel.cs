using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipaySecurityProdSignatureTaskQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipaySecurityProdSignatureTaskQueryModel : AopObject
    {
        /// <summary>
        /// 业务类型唯一标识。调用前联系支付宝服务提供方，由电子签名平台统一分配。
        /// </summary>
        [XmlElement("biz_product")]
        public string BizProduct { get; set; }

        /// <summary>
        /// 查询订单编号，参考alipay.security.prod.signature.task.apply接口返回的order_id。
        /// </summary>
        [XmlElement("order_id")]
        public string OrderId { get; set; }

        /// <summary>
        /// 调用前联系支付宝服务提供方，由电子签名平台统一分配。
        /// </summary>
        [XmlElement("service_version")]
        public string ServiceVersion { get; set; }

        /// <summary>
        /// 查询的签约任务编号列表，JSONArray格式。参考alipay.security.prod.signature.task.apply返回的task_list对象中的task_id属性。支持查询1到多个任务结果，如果列表为空，则默认查询所有任务结果，如果与当前订单下的任务没有匹配，则不返回任何签约任务。
        /// </summary>
        [XmlArray("task_id_list")]
        [XmlArrayItem("string")]
        public List<string> TaskIdList { get; set; }
    }
}
