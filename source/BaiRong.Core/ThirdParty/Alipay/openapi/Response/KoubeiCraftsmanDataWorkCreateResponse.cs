using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Aop.Api.Domain;

namespace Aop.Api.Response
{
    /// <summary>
    /// KoubeiCraftsmanDataWorkCreateResponse.
    /// </summary>
    public class KoubeiCraftsmanDataWorkCreateResponse : AopResponse
    {
        /// <summary>
        /// 作品id
        /// </summary>
        [XmlArray("works")]
        [XmlArrayItem("craftsman_work_out_id_open_model")]
        public List<CraftsmanWorkOutIdOpenModel> Works { get; set; }
    }
}
