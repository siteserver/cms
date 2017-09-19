using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Aop.Api.Domain;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayMobilePublicStdMockListsmlistApiResponse.
    /// </summary>
    public class AlipayMobilePublicStdMockListsmlistApiResponse : AopResponse
    {
        /// <summary>
        /// 简单对象嵌套List
        /// </summary>
        [XmlArray("list_sm_model_list")]
        [XmlArrayItem("list_list_sm_mock_model")]
        public List<ListListSmMockModel> ListSmModelList { get; set; }
    }
}
