using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayInsSceneApplicationApplyModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayInsSceneApplicationApplyModel : AopObject
    {
        /// <summary>
        /// 投保人
        /// </summary>
        [XmlElement("applicant")]
        public InsPerson Applicant { get; set; }

        /// <summary>
        /// 支付账单流水的标题
        /// </summary>
        [XmlElement("bill_title")]
        public string BillTitle { get; set; }

        /// <summary>
        /// 投保参数 ,每个产品特有的投保参数,如航空险的航班信息;标准json格式
        /// </summary>
        [XmlElement("biz_data")]
        public string BizData { get; set; }

        /// <summary>
        /// 生效时间
        /// </summary>
        [XmlElement("effect_start_time")]
        public string EffectStartTime { get; set; }

        /// <summary>
        /// 被保险人
        /// </summary>
        [XmlArray("insureds")]
        [XmlArrayItem("ins_person")]
        public List<InsPerson> Insureds { get; set; }

        /// <summary>
        /// 商户生成的外部投保业务号,必须保证唯一
        /// </summary>
        [XmlElement("out_biz_no")]
        public string OutBizNo { get; set; }

        /// <summary>
        /// 险种保障期限,数字+"Y/M/D"结尾,非固定期限险种或多固定期限险种必填
        /// </summary>
        [XmlElement("period")]
        public string Period { get; set; }

        /// <summary>
        /// 实际保费，询价接口获取的保费通过投保接口传递进来。投保接口会对传入的保费进行验证。传入的保费和核价的值不一样投保失败
        /// </summary>
        [XmlElement("premium")]
        public long Premium { get; set; }

        /// <summary>
        /// 产品编码;由蚂蚁保险平台分配,商户通过该产品编码投保特定的保险产品（如饿了么外卖延误险）
        /// </summary>
        [XmlElement("prod_code")]
        public string ProdCode { get; set; }

        /// <summary>
        /// 渠道来源
        /// </summary>
        [XmlElement("source")]
        public string Source { get; set; }

        /// <summary>
        /// 保额值,保额类型为枚举的时候是一个枚举值,当为金额类型时单位为分
        /// </summary>
        [XmlElement("sum_insured")]
        public long SumInsured { get; set; }
    }
}
