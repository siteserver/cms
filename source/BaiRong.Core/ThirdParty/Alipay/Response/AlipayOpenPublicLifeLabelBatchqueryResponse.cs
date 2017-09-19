using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Aop.Api.Domain;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayOpenPublicLifeLabelBatchqueryResponse.
    /// </summary>
    public class AlipayOpenPublicLifeLabelBatchqueryResponse : AopResponse
    {
        /// <summary>
        /// 标签列表
        /// </summary>
        [XmlArray("labels")]
        [XmlArrayItem("life_label")]
        public List<LifeLabel> Labels { get; set; }
    }
}
