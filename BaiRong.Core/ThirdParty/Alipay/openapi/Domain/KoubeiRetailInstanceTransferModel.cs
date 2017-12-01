using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiRetailInstanceTransferModel Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiRetailInstanceTransferModel : AopObject
    {
        /// <summary>
        /// json格式的置顶的券id列表信息，id的顺序指定置顶的券的顺序
        /// </summary>
        [XmlElement("instance_id_list")]
        public string InstanceIdList { get; set; }
    }
}
