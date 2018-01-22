using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// CampDetailInfo Data Structure.
    /// </summary>
    [Serializable]
    public class CampDetailInfo : AopObject
    {
        /// <summary>
        /// 活动开始时间
        /// </summary>
        [XmlElement("begin_time")]
        public string BeginTime { get; set; }

        /// <summary>
        /// 业务id，与bizType 一一对应，如：biz_type为消费送，biz_id为消费送活动id
        /// </summary>
        [XmlElement("biz_id")]
        public string BizId { get; set; }

        /// <summary>
        /// 业务类型：CONSUME_SEND：消费送；MRT_DISCOUNT:商户立减；OBTAIN:通用领券
        /// </summary>
        [XmlElement("biz_type")]
        public string BizType { get; set; }

        /// <summary>
        /// 活动描述
        /// </summary>
        [XmlElement("camp_desc")]
        public string CampDesc { get; set; }

        /// <summary>
        /// 需要解析该json串，title为标题，details是描述，多个detail需要换行
        /// </summary>
        [XmlElement("camp_guide")]
        public string CampGuide { get; set; }

        /// <summary>
        /// 活动结束时间
        /// </summary>
        [XmlElement("end_time")]
        public string EndTime { get; set; }

        /// <summary>
        /// 扩展字段信息，用Map对象json串保存
        /// </summary>
        [XmlElement("ext_info")]
        public string ExtInfo { get; set; }

        /// <summary>
        /// 每人每日参与次数 -1为不限制
        /// </summary>
        [XmlElement("win_limit_daily")]
        public string WinLimitDaily { get; set; }

        /// <summary>
        /// 每人总参与次数 -1 为不限制
        /// </summary>
        [XmlElement("win_limit_life")]
        public string WinLimitLife { get; set; }
    }
}
