using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ListListComplexMockModel Data Structure.
    /// </summary>
    [Serializable]
    public class ListListComplexMockModel : AopObject
    {
        /// <summary>
        /// 复杂对象list
        /// </summary>
        [XmlArray("cm_list")]
        [XmlArrayItem("complext_mock_model")]
        public List<ComplextMockModel> CmList { get; set; }
    }
}
