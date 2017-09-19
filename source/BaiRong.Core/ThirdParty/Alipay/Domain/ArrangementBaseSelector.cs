using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ArrangementBaseSelector Data Structure.
    /// </summary>
    [Serializable]
    public class ArrangementBaseSelector : AopObject
    {
        /// <summary>
        /// 合约状态列表.01 待生效，02 生效，03失效，04 取消
        /// </summary>
        [XmlArray("ar_statuses")]
        [XmlArrayItem("string")]
        public List<string> ArStatuses { get; set; }

        /// <summary>
        /// 产品外标类型:01：机构产品 02：借款人信息 03：主站产品 04： 标准机构产品  05：内部业务平台标准产品
        /// </summary>
        [XmlElement("mark_type")]
        public string MarkType { get; set; }

        /// <summary>
        /// 产品编码列表
        /// </summary>
        [XmlArray("pd_codes")]
        [XmlArrayItem("string")]
        public List<string> PdCodes { get; set; }

        /// <summary>
        /// 产品外标列表
        /// </summary>
        [XmlArray("pd_marks")]
        [XmlArrayItem("string")]
        public List<string> PdMarks { get; set; }

        /// <summary>
        /// 是否查询出产品外标
        /// </summary>
        [XmlElement("select_pd_mark")]
        public bool SelectPdMark { get; set; }

        /// <summary>
        /// 是否查询出产品名称
        /// </summary>
        [XmlElement("select_pd_name")]
        public bool SelectPdName { get; set; }
    }
}
