using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// MybankCreditLoanapplyDataUploadModel Data Structure.
    /// </summary>
    [Serializable]
    public class MybankCreditLoanapplyDataUploadModel : AopObject
    {
        /// <summary>
        /// 业务单编号。在申请场景下，就是申请单编号，唯一标识一笔贷款授信申请，获取方式：前提，和网商对接业务消息，biz_no是申请成功消息applicationEvent中的applicationCode字段，必填项。在其他场景，比如订单交易场景下，就是订单号。
        /// </summary>
        [XmlElement("biz_no")]
        public string BizNo { get; set; }

        /// <summary>
        /// 业务场景码。根据不同的合作模式，业务场景码不同，标识这笔数据是在哪个业务场景下产生的，由网商银行系统生成。目前只有两种场景，1：申请场景，2：交易订单场景。以后可根据业务发展增加场景码。默认为申请场景。
        /// </summary>
        [XmlElement("biz_type")]
        public string BizType { get; set; }

        /// <summary>
        /// 数据类型。收单行业：PARTNER_SETTLE_BILL_DATA；货运行业：PARTNER_TRANSPORT_DATA。该值网商系统会校验，请按约定填写。必填项。
        /// </summary>
        [XmlElement("category")]
        public string Category { get; set; }

        /// <summary>
        /// json格式的数据。根据合作协议约定的数据内容，把数据组装成json格式。参考isv数据接入文档，须保证数据通过校验。必填项。
        /// </summary>
        [XmlElement("data")]
        public string Data { get; set; }

        /// <summary>
        /// 数据提供方的标识，一般取合作机构的英文名或中文拼音。例如，如果合作机构是滴滴，则该值就可以填didi。该值网商系统不校验。必填项。
        /// </summary>
        [XmlElement("data_provider")]
        public string DataProvider { get; set; }

        /// <summary>
        /// 贷款申请人的唯一标识，个人客户一般为身份证号码或者支付宝ID，公司客户为工商注册号。全局唯一，用来唯一标识一个客户。如果身份证号包含字母，则字母必须大写。必填项。客户身份证号码可以从网商银行发送给机构的授信申请消息中获取，也可以是客户在机构注册时登记的信息。
        /// </summary>
        [XmlElement("entity_code")]
        public string EntityCode { get; set; }

        /// <summary>
        /// 数据所属的实体名称，一般为客户姓名或者公司名，作为客户的标识，不唯一，用来做申请单的校验。必填项。客户姓名可以从网商银行发送给机构的授信申请消息中获取，也可以是客户在机构注册时登记的信息。
        /// </summary>
        [XmlElement("entity_name")]
        public string EntityName { get; set; }

        /// <summary>
        /// 客户的身份类型，由具体的合作场景决定。当个人客户以身份证为标识时是PERSON，企业是COMPAY。如果以会员ID标识客户，则支付宝会员填ALIPAY，网商银行会员填MYBANK。非必填项。
        /// </summary>
        [XmlElement("entity_type")]
        public string EntityType { get; set; }
    }
}
