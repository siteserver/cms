using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayEcoRenthouseKaBaseinfoSyncModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayEcoRenthouseKaBaseinfoSyncModel : AopObject
    {
        /// <summary>
        /// 返回kaCode唯一标识,如果有该值则表示更新信息(新增的时候kaCode字段不是必填的,修改的时候必填)
        /// </summary>
        [XmlElement("ka_code")]
        public string KaCode { get; set; }

        /// <summary>
        /// 公寓运营商名称-新增ka名称必填(sync的新增的时候kaName字段是必填,修改的时候不是必填)
        /// </summary>
        [XmlElement("ka_name")]
        public string KaName { get; set; }
    }
}
