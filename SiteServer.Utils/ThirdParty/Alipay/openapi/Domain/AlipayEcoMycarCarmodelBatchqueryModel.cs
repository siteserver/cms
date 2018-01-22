using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayEcoMycarCarmodelBatchqueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayEcoMycarCarmodelBatchqueryModel : AopObject
    {
        /// <summary>
        /// 支付宝车型库品牌编号（系统唯一）
        /// </summary>
        [XmlElement("brand_id")]
        public string BrandId { get; set; }

        /// <summary>
        /// 支付宝车型库厂商编号（系统唯一）
        /// </summary>
        [XmlElement("company_id")]
        public string CompanyId { get; set; }

        /// <summary>
        /// 查询类型，接口通过此参数判断本次请求是查询品牌信息还是车型信息等，brands（查询品牌），series（查询车系），company(厂商)，models（查询车型），当该字段值为brands时，则其他参数不需要填值，当该字段为series时，则字段brand_id为必填，当该字段为company时，则字段brand_id为必填，当该字段为models时，则字段serie_id或者company_id不能同时为空，
        /// </summary>
        [XmlElement("query_type")]
        public string QueryType { get; set; }

        /// <summary>
        /// 支付宝车型库车系编号（系统唯一）
        /// </summary>
        [XmlElement("serie_id")]
        public string SerieId { get; set; }
    }
}
