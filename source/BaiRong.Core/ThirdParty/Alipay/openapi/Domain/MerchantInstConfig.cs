using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// MerchantInstConfig Data Structure.
    /// </summary>
    [Serializable]
    public class MerchantInstConfig : AopObject
    {
        /// <summary>
        /// 机构短名称，既是合作机构的英文简称，用来标识该机构的唯一性；
        /// </summary>
        [XmlElement("en_name")]
        public string EnName { get; set; }

        /// <summary>
        /// 支付宝定义的业务类型，JF表示公共事业缴费，HK表示信用卡还款具体见附录描述
        /// </summary>
        [XmlElement("order_type")]
        public string OrderType { get; set; }

        /// <summary>
        /// 业务场景；分为销帐(chargeoff)和输出(EXTERNAL)
        /// </summary>
        [XmlElement("scene")]
        public string Scene { get; set; }

        /// <summary>
        /// 合作机构中文名称，如HZCB，中文名称：杭州银行
        /// </summary>
        [XmlElement("zh_name")]
        public string ZhName { get; set; }
    }
}
