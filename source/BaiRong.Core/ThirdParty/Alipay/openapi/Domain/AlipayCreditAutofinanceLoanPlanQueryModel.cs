using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayCreditAutofinanceLoanPlanQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayCreditAutofinanceLoanPlanQueryModel : AopObject
    {
        /// <summary>
        /// 扩展参数,针对不同的平台特殊业务场景，将需要的参数填入改字段，目前针对大搜车业务有以下参数：itemprice 车辆价格，lastprop 车辆残值率,extintamt 基础服务包+增值服务包，loantenor 贷款期数，creditamtprop 授信额度比例调整值；
        /// </summary>
        [XmlElement("extparam")]
        public string Extparam { get; set; }

        /// <summary>
        /// 机构编码，机构接入汽车金融平台时分配，固定值
        /// </summary>
        [XmlElement("orgcode")]
        public string Orgcode { get; set; }

        /// <summary>
        /// 产品编码，汽车金融平台给机构提供的产品编码
        /// </summary>
        [XmlElement("productcode")]
        public string Productcode { get; set; }

        /// <summary>
        /// 本次请求流水号，全局唯一
        /// </summary>
        [XmlElement("seqno")]
        public string Seqno { get; set; }

        /// <summary>
        /// 支付宝账号数字ID
        /// </summary>
        [XmlElement("uid")]
        public string Uid { get; set; }
    }
}
