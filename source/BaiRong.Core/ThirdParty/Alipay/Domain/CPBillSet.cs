using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// CPBillSet Data Structure.
    /// </summary>
    [Serializable]
    public class CPBillSet : AopObject
    {
        /// <summary>
        /// 明细条目所归属的账期，用于归类和向用户展示，具体参数值由物业系统自行定义，除参数最大长度外支付宝不做限定。
        /// </summary>
        [XmlElement("acct_period")]
        public string AcctPeriod { get; set; }

        /// <summary>
        /// 应收金额，单位为元，精确到小数点后两位，取值范围[0.01,100000000]
        /// </summary>
        [XmlElement("bill_entry_amount")]
        public string BillEntryAmount { get; set; }

        /// <summary>
        /// 物业费账单应收明细条目ID，在同一小区内必须唯一，不同小区不做唯一性要求。
        /// </summary>
        [XmlElement("bill_entry_id")]
        public string BillEntryId { get; set; }

        /// <summary>
        /// 费用类型名称，根据物业系统定义传入，支付宝除参数最大长度外不做限定。
        /// </summary>
        [XmlElement("cost_type")]
        public string CostType { get; set; }

        /// <summary>
        /// 缴费截止日期，格式固定为YYYYMMDD。不能早于调用接口时的当前实际日期（北京时间）和出账日期。
        /// </summary>
        [XmlElement("deadline")]
        public string Deadline { get; set; }

        /// <summary>
        /// 物业系统端房屋编号，必须事先通过房屋信息上传接口上传到支付宝社区物业平台。
        /// </summary>
        [XmlElement("out_room_id")]
        public string OutRoomId { get; set; }

        /// <summary>
        /// 缴费明细条目关联ID。若物业系统业务约束上传的多条明细条目必须被一次同时支付，则对应的明细条目需传入同样的relate_id。
        /// </summary>
        [XmlElement("relate_id")]
        public string RelateId { get; set; }

        /// <summary>
        /// 出账日期，格式固定为YYYYMMDD。
        /// </summary>
        [XmlElement("release_day")]
        public string ReleaseDay { get; set; }

        /// <summary>
        /// 缴费支付确认页显示给用户的提示确认信息，除房间名外的第二重校验信息，预防用户错缴。建议传入和缴费户相关的信息例如，可传入脱敏后的物业系统中的业主姓名，或其他相关信息。可选参数，不传则不展示。账单明细合并支付时，若部分账单明细的remark_str不同，则默认取第一条有remark_str值的账单明细进行展示。
        /// </summary>
        [XmlElement("remark_str")]
        public string RemarkStr { get; set; }

        /// <summary>
        /// 对应的房屋门牌地址。若开发者之前通过上传物业小区内部房屋信息接口中的address参数已上传，可不传。
        /// </summary>
        [XmlElement("room_address")]
        public string RoomAddress { get; set; }
    }
}
