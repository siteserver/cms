using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipaySecurityRiskContentResultGetModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipaySecurityRiskContentResultGetModel : AopObject
    {
        /// <summary>
        /// 应用场景
        /// </summary>
        [XmlElement("app_scene")]
        public string AppScene { get; set; }

        /// <summary>
        /// alipay.security.risk.content.analyze （内容风险识别接口服务）中的内容业务ID，用于进行异步识别结果的索引查询
        /// </summary>
        [XmlElement("app_scene_data_id")]
        public string AppSceneDataId { get; set; }
    }
}
