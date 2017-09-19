using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiMarketingDataIndicatorQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiMarketingDataIndicatorQueryModel : AopObject
    {
        /// <summary>
        /// 开始日期,格式:yyyyMMdd
        /// </summary>
        [XmlElement("begin_date")]
        public string BeginDate { get; set; }

        /// <summary>
        /// 业务类型，可选值有六个  1，MemberQuery 商户会员数据查询  2，MemberQueryByStore 门店会员数据查询  3，TradeQuery 商户交易数据查询  4，TradeQueryByStore 门店交易数据查询  5，CampaignQuery 商户活动数据查询  6，CampaignQueryByStore 门店活动数据查询
        /// </summary>
        [XmlElement("biz_type")]
        public string BizType { get; set; }

        /// <summary>
        /// 结束日期 格式:yyyyMMdd
        /// </summary>
        [XmlElement("end_date")]
        public string EndDate { get; set; }

        /// <summary>
        /// camp_id：为活动ID  sort_field：为排序指标KEY  sort_type：ASC表示升序,DESC表示降序  store_Ids：为门店ID，多个门店使用逗号分隔
        /// </summary>
        [XmlElement("ext_info")]
        public string ExtInfo { get; set; }

        /// <summary>
        /// 当前页数，默认为1
        /// </summary>
        [XmlElement("page_num")]
        public string PageNum { get; set; }

        /// <summary>
        /// 每页记录数,不能超过50，默认为20
        /// </summary>
        [XmlElement("page_size")]
        public string PageSize { get; set; }
    }
}
