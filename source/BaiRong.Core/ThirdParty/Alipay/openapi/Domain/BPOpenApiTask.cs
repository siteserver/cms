using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// BPOpenApiTask Data Structure.
    /// </summary>
    [Serializable]
    public class BPOpenApiTask : AopObject
    {
        /// <summary>
        /// 处理地址
        /// </summary>
        [XmlElement("deal_url")]
        public string DealUrl { get; set; }

        /// <summary>
        /// 详情展示地址
        /// </summary>
        [XmlElement("detail_url")]
        public string DetailUrl { get; set; }

        /// <summary>
        /// 审批节点中文显示名称
        /// </summary>
        [XmlElement("display_name")]
        public string DisplayName { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        [XmlElement("gmt_operate")]
        public string GmtOperate { get; set; }

        /// <summary>
        /// 处理备注信息
        /// </summary>
        [XmlElement("memo")]
        public string Memo { get; set; }

        /// <summary>
        /// 审批节点code
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// 点击的操作按钮
        /// </summary>
        [XmlElement("operate")]
        public string Operate { get; set; }

        /// <summary>
        /// 可点击的操作
        /// </summary>
        [XmlElement("operate_transition")]
        public string OperateTransition { get; set; }

        /// <summary>
        /// 处理人域账号
        /// </summary>
        [XmlElement("operator")]
        public string Operator { get; set; }

        /// <summary>
        /// 处理人花名
        /// </summary>
        [XmlElement("operator_name")]
        public string OperatorName { get; set; }

        /// <summary>
        /// 加签类型
        /// </summary>
        [XmlElement("sign_type")]
        public string SignType { get; set; }

        /// <summary>
        /// 状态:CREATED,TAKEN,TEMP_SAVE,COMPLETED,CANCELED
        /// </summary>
        [XmlElement("state")]
        public string State { get; set; }

        /// <summary>
        /// 节点类型：UserTask，SystemTask
        /// </summary>
        [XmlElement("type")]
        public string Type { get; set; }
    }
}
