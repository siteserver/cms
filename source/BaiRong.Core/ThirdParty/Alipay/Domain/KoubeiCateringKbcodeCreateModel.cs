using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiCateringKbcodeCreateModel Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiCateringKbcodeCreateModel : AopObject
    {
        /// <summary>
        /// 口碑码绑定的门店或者桌号信息列表
        /// </summary>
        [XmlArray("bind_info_list")]
        [XmlArrayItem("kb_code_bind_info_v_o")]
        public List<KbCodeBindInfoVO> BindInfoList { get; set; }

        /// <summary>
        /// 请求流水ID，用于幂等控制
        /// </summary>
        [XmlElement("request_id")]
        public string RequestId { get; set; }

        /// <summary>
        /// 物料模板（可以不填，系统会根据码类型默认一个模板）
        /// </summary>
        [XmlElement("stuff_template")]
        public string StuffTemplate { get; set; }

        /// <summary>
        /// 生成码的类型（桌码：TABLE_PASTER，门店码：DOOR_PASTER）
        /// </summary>
        [XmlElement("stuff_type")]
        public string StuffType { get; set; }
    }
}
