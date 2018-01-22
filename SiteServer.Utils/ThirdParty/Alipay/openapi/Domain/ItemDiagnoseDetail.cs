using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ItemDiagnoseDetail Data Structure.
    /// </summary>
    [Serializable]
    public class ItemDiagnoseDetail : AopObject
    {
        /// <summary>
        /// 菜品的热度等级 菜品热度等级（0/0.5/1/1.5/2/2.5/3/3.5/4/4.5/5）该字段是对热度值做离散化，方便用户用图像化表达热度
        /// </summary>
        [XmlElement("hot_grade")]
        public long HotGrade { get; set; }

        /// <summary>
        /// 菜品的热度值 保留两位小数，热度值在0~100分之间
        /// </summary>
        [XmlElement("hot_value")]
        public long HotValue { get; set; }

        /// <summary>
        /// 菜品诊断：001-明星菜品；002潜力菜品；003其他菜品。
        /// </summary>
        [XmlElement("item_diagnose")]
        public string ItemDiagnose { get; set; }

        /// <summary>
        /// 诊断描述  明星菜品：销量和复购多指标表现强劲，可力推该菜品；潜力菜品：高复购销量适中，可适当增加此类菜品推荐；其他菜品：除明星菜品和潜力菜品外的其他菜品。
        /// </summary>
        [XmlElement("item_diagnose_desc")]
        public string ItemDiagnoseDesc { get; set; }

        /// <summary>
        /// 外部商品ID
        /// </summary>
        [XmlElement("item_id")]
        public string ItemId { get; set; }

        /// <summary>
        /// 菜品名称
        /// </summary>
        [XmlElement("item_name")]
        public string ItemName { get; set; }

        /// <summary>
        /// 单位分
        /// </summary>
        [XmlElement("item_price")]
        public long ItemPrice { get; set; }

        /// <summary>
        /// 近90天消费的支付宝用户数
        /// </summary>
        [XmlElement("rec_ninety_consume_uid_cnt")]
        public long RecNinetyConsumeUidCnt { get; set; }

        /// <summary>
        /// 近90天购买2次及以上的支付宝用户数
        /// </summary>
        [XmlElement("rec_ninety_rebuy_uid_cnt")]
        public long RecNinetyRebuyUidCnt { get; set; }

        /// <summary>
        /// 近7天的销售金额
        /// </summary>
        [XmlElement("rec_seven_sale_amt")]
        public long RecSevenSaleAmt { get; set; }

        /// <summary>
        /// 近7天销售个数
        /// </summary>
        [XmlElement("rec_seven_sale_cnt")]
        public long RecSevenSaleCnt { get; set; }

        /// <summary>
        /// 近60天消费的支付
        /// </summary>
        [XmlElement("rec_sixty_consume_uid_cnt")]
        public long RecSixtyConsumeUidCnt { get; set; }

        /// <summary>
        /// 近60天购买2次及以上的支付宝用户数
        /// </summary>
        [XmlElement("rec_sixty_rebuy_uid_cnt")]
        public long RecSixtyRebuyUidCnt { get; set; }

        /// <summary>
        /// 近30天消费的支付宝用户数
        /// </summary>
        [XmlElement("rec_thirty_consume_uid_cnt")]
        public string RecThirtyConsumeUidCnt { get; set; }

        /// <summary>
        /// 近30天购买2次及以上的支付宝用户数
        /// </summary>
        [XmlElement("rec_thirty_rebuy_uid_cnt")]
        public long RecThirtyRebuyUidCnt { get; set; }

        /// <summary>
        /// 近30天销售金额，单位分
        /// </summary>
        [XmlElement("rec_thirty_sale_amt")]
        public long RecThirtySaleAmt { get; set; }

        /// <summary>
        /// 近30天销售个数
        /// </summary>
        [XmlElement("rec_thirty_sale_cnt")]
        public long RecThirtySaleCnt { get; set; }

        /// <summary>
        /// 报表数据生成日期 yyyyMMdd格式 保留最近30天数据
        /// </summary>
        [XmlElement("report_date")]
        public string ReportDate { get; set; }
    }
}
