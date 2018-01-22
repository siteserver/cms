using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayOpenPublicMessageGroupSendResponse.
    /// </summary>
    public class AlipayOpenPublicMessageGroupSendResponse : AopResponse
    {
        /// <summary>
        /// 2013121100055554f1000b64-3f05-4581-a5e1-16e29d242950
        /// </summary>
        [XmlElement("message_id")]
        public string MessageId { get; set; }
    }
}
