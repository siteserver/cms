using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayEbppFacepayBillPayModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayEbppFacepayBillPayModel : AopObject
    {
        /// <summary>
        /// 账期
        /// </summary>
        [XmlElement("bill_date")]
        public string BillDate { get; set; }

        /// <summary>
        /// 户号（缴税业务：纳税人识别号，对于三证合一的企业来说，采用社会信用代码；对于个人来说，采用身份证号）
        /// </summary>
        [XmlElement("bill_key")]
        public string BillKey { get; set; }

        /// <summary>
        /// 业务类型英文名称，JF-缴费、TAX-缴税
        /// </summary>
        [XmlElement("biz_type")]
        public string BizType { get; set; }

        /// <summary>
        /// 出账机构代码（缴税业务：指征收机关英文代码，例如南京玄武国税（NJXWGS））
        /// </summary>
        [XmlElement("charge_inst")]
        public string ChargeInst { get; set; }

        /// <summary>
        /// 扩展字段（缴税业务：taxpayerName -纳税人名称，taxOrgCode - 征收机关代码；缴费业务如需支付宝销账：billCacheKey -欠费单缓存Key，billUniqId - 欠费单唯一ID）
        /// </summary>
        [XmlElement("extend_field")]
        public string ExtendField { get; set; }

        /// <summary>
        /// 滞纳金额，单位：元
        /// </summary>
        [XmlElement("fine_amount")]
        public string FineAmount { get; set; }

        /// <summary>
        /// 机构账单ID（缴税业务：用外部申报号）
        /// </summary>
        [XmlElement("inst_no")]
        public string InstNo { get; set; }

        /// <summary>
        /// ISV流水号，用于控制幂等，须确保全局唯一（缴税业务：可采用{征收机关代码}-{外部申报号}的形式）
        /// </summary>
        [XmlElement("out_order_no")]
        public string OutOrderNo { get; set; }

        /// <summary>
        /// 支付金额（包含滞纳金），单位：元
        /// </summary>
        [XmlElement("pay_amount")]
        public string PayAmount { get; set; }

        /// <summary>
        /// 商户partnerId
        /// </summary>
        [XmlElement("pid")]
        public string Pid { get; set; }

        /// <summary>
        /// 子业务类型英文名称，ELECTRIC-电费，WATER-水费，GAS-燃气费，TAX-缴税
        /// </summary>
        [XmlElement("sub_biz_type")]
        public string SubBizType { get; set; }

        /// <summary>
        /// 用户支付宝付款码
        /// </summary>
        [XmlElement("user_identity_code")]
        public string UserIdentityCode { get; set; }
    }
}
