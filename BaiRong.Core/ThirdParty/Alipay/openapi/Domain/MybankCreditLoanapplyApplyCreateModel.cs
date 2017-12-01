using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// MybankCreditLoanapplyApplyCreateModel Data Structure.
    /// </summary>
    [Serializable]
    public class MybankCreditLoanapplyApplyCreateModel : AopObject
    {
        /// <summary>
        /// 客户的支付宝ID
        /// </summary>
        [XmlElement("alipay_id")]
        public string AlipayId { get; set; }

        /// <summary>
        /// 申请金额，默认以元为单位，支持精确到小数点后2位，为了便于传输，调用方需要把double类型的金额转换为string类型
        /// </summary>
        [XmlElement("apply_lmt")]
        public string ApplyLmt { get; set; }

        /// <summary>
        /// 关联实体，申贷主体可以关联一些其他实体比方说一个企业，一个会员账号用来作为申贷材料辅助申贷
        /// </summary>
        [XmlArray("associate_entitys")]
        [XmlArrayItem("involved_entity")]
        public List<InvolvedEntity> AssociateEntitys { get; set; }

        /// <summary>
        /// 业务编号，比如当使用一笔订单创建授信申请时，业务编号就是订单号
        /// </summary>
        [XmlElement("biz_no")]
        public string BizNo { get; set; }

        /// <summary>
        /// 授信申请的场景码，1表示普通客户申请，2表示订单申请，可扩展场景
        /// </summary>
        [XmlElement("biz_scene_no")]
        public string BizSceneNo { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        [XmlElement("biz_tag")]
        public string BizTag { get; set; }

        /// <summary>
        /// 客户的身份证号码
        /// </summary>
        [XmlElement("cert_no")]
        public string CertNo { get; set; }

        /// <summary>
        /// 证件类型，01:身份证
        /// </summary>
        [XmlElement("cert_type")]
        public string CertType { get; set; }

        /// <summary>
        /// 联系邮箱
        /// </summary>
        [XmlElement("email")]
        public string Email { get; set; }

        /// <summary>
        /// 扩展数据（map转换为json字符串）
        /// </summary>
        [XmlElement("ext_par")]
        public string ExtPar { get; set; }

        /// <summary>
        /// 银行参与者id，是在网商银行创建会员后生成的id，网商银行会员的唯一标识
        /// </summary>
        [XmlElement("ip_id")]
        public string IpId { get; set; }

        /// <summary>
        /// 银行参与者角色id，是在网商银行创建会员后生成的角色id，网商银行会员角色的唯一标识
        /// </summary>
        [XmlElement("ip_role_id")]
        public string IpRoleId { get; set; }

        /// <summary>
        /// 会员的登录账号，比如支付宝登录账号，一般为手机号或邮箱
        /// </summary>
        [XmlElement("login_id")]
        public string LoginId { get; set; }

        /// <summary>
        /// 联系手机号
        /// </summary>
        [XmlElement("mobile")]
        public string Mobile { get; set; }

        /// <summary>
        /// 客户姓名
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// 运营产品码
        /// </summary>
        [XmlElement("op_pd_code")]
        public string OpPdCode { get; set; }

        /// <summary>
        /// 外部机构定义的客户编号
        /// </summary>
        [XmlElement("out_mem_id")]
        public string OutMemId { get; set; }

        /// <summary>
        /// 外部流水号,唯一标识客户的一笔贷款申请。格式：日期(8位)+序列号(8位）,序列号是数字，如00000001（必须是16位且符合该格式）。
        /// </summary>
        [XmlElement("request_id")]
        public string RequestId { get; set; }

        /// <summary>
        /// ALIPAY:支付宝  MYBANK:网商银行  TMALL：天猫  TAOBAO：淘宝
        /// </summary>
        [XmlElement("site")]
        public string Site { get; set; }

        /// <summary>
        /// 外部站点用户ID，跟SITE对应，若SITE是ALIPAY则为支付宝ID，若SITE是ICBU则为ICBU 用户ID
        /// </summary>
        [XmlElement("site_user_id")]
        public string SiteUserId { get; set; }
    }
}
