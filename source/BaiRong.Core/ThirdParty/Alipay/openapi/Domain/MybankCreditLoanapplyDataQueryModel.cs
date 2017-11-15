using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// MybankCreditLoanapplyDataQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class MybankCreditLoanapplyDataQueryModel : AopObject
    {
        /// <summary>
        /// 对应业务类型相关的单号。若业务类型为1的话，则该值为申贷接口返回的sub_apply_no 子申请单号。
        /// </summary>
        [XmlElement("biz_no")]
        public string BizNo { get; set; }

        /// <summary>
        /// 什么业务场景相关的数据，比方说1为个人申贷相关的数据。
        /// </summary>
        [XmlElement("biz_type")]
        public string BizType { get; set; }

        /// <summary>
        /// 查询的数据类型。是查外访预约单还是个人征信报告还是企业征信报告还是其他。
        /// </summary>
        [XmlElement("category")]
        public string Category { get; set; }

        /// <summary>
        /// 查询数据的版本号。若biz_type为1的话，该版本号在申贷查询的接口里会返回
        /// </summary>
        [XmlElement("data_version")]
        public string DataVersion { get; set; }

        /// <summary>
        /// 查询的对象的唯一标识。个人客户一般为身份证号码或者支付宝ID，公司客户为工商注册号。全局唯一，用来唯一标识一个对象。如果身份证号包含字母，则字母必须大写。必填项。客户身份证号码可以从网商银行发送给机构的授信申请消息中获取，也可以是客户在机构注册时登记的信息。
        /// </summary>
        [XmlElement("entity_code")]
        public string EntityCode { get; set; }

        /// <summary>
        /// 查询的对象的名字，查个人征信就是这个人的名字，查企业征信就是企业名
        /// </summary>
        [XmlElement("entity_name")]
        public string EntityName { get; set; }

        /// <summary>
        /// 客户的身份类型，由具体的合作场景决定。当个人客户以身份证为标识时是PERSON，企业是COMPAY ,一笔贷款维度的为LOAN
        /// </summary>
        [XmlElement("entity_type")]
        public string EntityType { get; set; }

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
    }
}
