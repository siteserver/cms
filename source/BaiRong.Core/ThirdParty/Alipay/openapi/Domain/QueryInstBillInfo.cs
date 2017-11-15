using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// QueryInstBillInfo Data Structure.
    /// </summary>
    [Serializable]
    public class QueryInstBillInfo : AopObject
    {
        /// <summary>
        /// 账单金额
        /// </summary>
        [XmlElement("amount")]
        public string Amount { get; set; }

        /// <summary>
        /// 余额
        /// </summary>
        [XmlElement("balance")]
        public string Balance { get; set; }

        /// <summary>
        /// 账单日期
        /// </summary>
        [XmlElement("bill_date")]
        public string BillDate { get; set; }

        /// <summary>
        /// 明细说明
        /// </summary>
        [XmlArray("bill_detail")]
        [XmlArrayItem("query_inst_bill_detail")]
        public List<QueryInstBillDetail> BillDetail { get; set; }

        /// <summary>
        /// 滞纳金
        /// </summary>
        [XmlElement("bill_fines")]
        public string BillFines { get; set; }

        /// <summary>
        /// 账单流水
        /// </summary>
        [XmlElement("bill_key")]
        public string BillKey { get; set; }

        /// <summary>
        /// 户名
        /// </summary>
        [XmlElement("bill_user_name")]
        public string BillUserName { get; set; }

        /// <summary>
        /// JDBXINHUI
        /// </summary>
        [XmlElement("charge_inst")]
        public string ChargeInst { get; set; }

        /// <summary>
        /// 查询欠费单的惟一标识
        /// </summary>
        [XmlElement("charge_uniq_id")]
        public string ChargeUniqId { get; set; }

        /// <summary>
        /// 销账机构
        /// </summary>
        [XmlElement("chargeoff_inst")]
        public string ChargeoffInst { get; set; }

        /// <summary>
        /// 销账机构给出账机构分配的ID
        /// </summary>
        [XmlElement("company_id")]
        public string CompanyId { get; set; }

        /// <summary>
        /// 扩展属性
        /// </summary>
        [XmlElement("extend")]
        public string Extend { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        [XmlElement("order_type")]
        public string OrderType { get; set; }

        /// <summary>
        /// 外部流水号
        /// </summary>
        [XmlElement("out_id")]
        public string OutId { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        [XmlElement("return_message")]
        public string ReturnMessage { get; set; }

        /// <summary>
        /// 子业务类型
        /// </summary>
        [XmlElement("sub_order_type")]
        public string SubOrderType { get; set; }
    }
}
