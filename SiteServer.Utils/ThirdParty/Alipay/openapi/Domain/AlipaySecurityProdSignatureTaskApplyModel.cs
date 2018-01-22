using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipaySecurityProdSignatureTaskApplyModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipaySecurityProdSignatureTaskApplyModel : AopObject
    {
        /// <summary>
        /// 外部应用名称，由支付宝统一分配，无法自助获取。
        /// </summary>
        [XmlElement("biz_app")]
        public string BizApp { get; set; }

        /// <summary>
        /// 业务流水号，保证唯一性
        /// </summary>
        [XmlElement("biz_id")]
        public string BizId { get; set; }

        /// <summary>
        /// 业务扩展参数 {"key1":"value2"}
        /// </summary>
        [XmlElement("biz_info")]
        public string BizInfo { get; set; }

        /// <summary>
        /// 业务唯一标识，由支付宝统一分配，无法自助获取
        /// </summary>
        [XmlElement("biz_product")]
        public string BizProduct { get; set; }

        /// <summary>
        /// 电子签约类型，目前只支持一种类型电子合同，取值1
        /// </summary>
        [XmlElement("order_type")]
        public long OrderType { get; set; }

        /// <summary>
        /// 接口版本信息，目前默认3，由服务提供方指定。
        /// </summary>
        [XmlElement("service_version")]
        public string ServiceVersion { get; set; }

        /// <summary>
        /// 签约文件列表。具体见SignDataInfo中定义。
        /// </summary>
        [XmlArray("sign_data_list")]
        [XmlArrayItem("sign_data_info")]
        public List<SignDataInfo> SignDataList { get; set; }

        /// <summary>
        /// 签约子任务，每个任务对应一个签约主体。
        /// </summary>
        [XmlArray("sign_task_list")]
        [XmlArrayItem("sign_task")]
        public List<SignTask> SignTaskList { get; set; }

        /// <summary>
        /// 制定签约主体执行签约顺序，例如甲乙双方签约，“顺序签约”模式下，甲签约完成后乙才能开始签约；“并行签约”模式下，甲乙可同时进行认证，按照时序顺序在文档上签约。  1 : 顺序签约  2 : 并行签约
        /// </summary>
        [XmlElement("sign_task_type")]
        public long SignTaskType { get; set; }
    }
}
