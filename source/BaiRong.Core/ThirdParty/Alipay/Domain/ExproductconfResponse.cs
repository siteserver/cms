using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ExproductconfResponse Data Structure.
    /// </summary>
    [Serializable]
    public class ExproductconfResponse : AopObject
    {
        /// <summary>
        /// 出账机构
        /// </summary>
        [XmlElement("charge_inst")]
        public string ChargeInst { get; set; }

        /// <summary>
        /// 出账机构中文名称
        /// </summary>
        [XmlElement("chargeinst_name")]
        public string ChargeinstName { get; set; }

        /// <summary>
        /// 销账机构
        /// </summary>
        [XmlElement("chargeoff_inst")]
        public string ChargeoffInst { get; set; }

        /// <summary>
        /// 销账机构中文名称
        /// </summary>
        [XmlElement("chargeoffinst_name")]
        public string ChargeoffinstName { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        [XmlElement("city")]
        public string City { get; set; }

        /// <summary>
        /// 扩展字段
        /// </summary>
        [XmlElement("extend")]
        public string Extend { get; set; }

        /// <summary>
        /// 省份
        /// </summary>
        [XmlElement("province")]
        public string Province { get; set; }

        /// <summary>
        /// 产品状态
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }
    }
}
