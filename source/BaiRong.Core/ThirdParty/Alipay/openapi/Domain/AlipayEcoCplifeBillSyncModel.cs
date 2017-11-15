using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayEcoCplifeBillSyncModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayEcoCplifeBillSyncModel : AopObject
    {
        /// <summary>
        /// 待同步的物业费账单应收明细条目ID
        /// </summary>
        [XmlElement("bill_entry_id")]
        public string BillEntryId { get; set; }

        /// <summary>
        /// 支付宝社区物业平台小区ID，用户通过支付宝社区物业平台物业查询获取。
        /// </summary>
        [XmlElement("community_id")]
        public string CommunityId { get; set; }

        /// <summary>
        /// 指定条目待更新的缴费截止日期  若operate_type为update，该参数选填；  若operate_type为delete，该参数不用填。
        /// </summary>
        [XmlElement("new_deadline")]
        public string NewDeadline { get; set; }

        /// <summary>
        /// 指定条目待更新的应收金额，单位为元，精确到小数点后两位，取值范围[0.01,100000000]  若operate_type为update，该参数选填；  若operate_type为delete，该参数不用填。
        /// </summary>
        [XmlElement("new_entry_amount")]
        public string NewEntryAmount { get; set; }

        /// <summary>
        /// 同步类型：  delete：删除  update：更改
        /// </summary>
        [XmlElement("operate_type")]
        public string OperateType { get; set; }
    }
}
