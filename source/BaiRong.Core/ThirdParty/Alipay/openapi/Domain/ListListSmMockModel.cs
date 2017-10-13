using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ListListSmMockModel Data Structure.
    /// </summary>
    [Serializable]
    public class ListListSmMockModel : AopObject
    {
        /// <summary>
        /// 简单模型列表
        /// </summary>
        [XmlArray("list_simple_model_list")]
        [XmlArrayItem("simple_mock_model")]
        public List<SimpleMockModel> ListSimpleModelList { get; set; }
    }
}
