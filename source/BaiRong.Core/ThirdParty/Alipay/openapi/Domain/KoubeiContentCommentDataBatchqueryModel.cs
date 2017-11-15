using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiContentCommentDataBatchqueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiContentCommentDataBatchqueryModel : AopObject
    {
        /// <summary>
        /// 口碑评价id，如果传入评价id则会忽略其他参数。 （口碑评价id、口碑门店id、口碑手艺人id不能同时为空）
        /// </summary>
        [XmlElement("comment_id")]
        public string CommentId { get; set; }

        /// <summary>
        /// 口碑手艺人id，可通过 koubei.craftsman.data.provider.batchquery 批量查询手艺人信息接口查询。（口碑评价id、口碑门店id、口碑手艺人id不能同时为空）
        /// </summary>
        [XmlElement("craftsman_id")]
        public string CraftsmanId { get; set; }

        /// <summary>
        /// 前次查询的最后一条评价id，用于做分页查询的游标。查询时，需要指定从哪一条评价开始往后取，如果上一次该值传空，则从第一页从头取。
        /// </summary>
        [XmlElement("last_comment_id")]
        public string LastCommentId { get; set; }

        /// <summary>
        /// 每页的条数，最大20条，不传会报错
        /// </summary>
        [XmlElement("page_size")]
        public long PageSize { get; set; }

        /// <summary>
        /// 口碑门店id，如果传入口碑门店id则会忽略手艺人id。（口碑评价id、口碑门店id、口碑手艺人id不能同时为空）
        /// </summary>
        [XmlElement("shop_id")]
        public string ShopId { get; set; }
    }
}
