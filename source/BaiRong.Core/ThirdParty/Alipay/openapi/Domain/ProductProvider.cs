using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ProductProvider Data Structure.
    /// </summary>
    [Serializable]
    public class ProductProvider : AopObject
    {
        /// <summary>
        /// 卡代英文名称 备注:淘宝交易为空
        /// </summary>
        [XmlElement("agency")]
        public string Agency { get; set; }

        /// <summary>
        /// 卡代中文名称 备注：淘宝交易为空
        /// </summary>
        [XmlElement("agency_name")]
        public string AgencyName { get; set; }

        /// <summary>
        /// 业务 类型
        /// </summary>
        [XmlElement("biz_type")]
        public string BizType { get; set; }

        /// <summary>
        /// 充值产品主表ID
        /// </summary>
        [XmlElement("id")]
        public string Id { get; set; }

        /// <summary>
        /// 子业务类型
        /// </summary>
        [XmlElement("sub_biz_type")]
        public string SubBizType { get; set; }

        /// <summary>
        /// 子运营商英文名称 备注:淘宝交易为空
        /// </summary>
        [XmlElement("sub_operator")]
        public string SubOperator { get; set; }

        /// <summary>
        /// 子运营商中文名称 ,如浙江移动
        /// </summary>
        [XmlElement("sub_operator_name")]
        public string SubOperatorName { get; set; }
    }
}
