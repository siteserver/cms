using System;
using System.Xml.Serialization;

namespace Qimen.Api
{
    [Serializable]
    public abstract class QimenResponse
    {
        [XmlElement("flag")]
        public string Flag { get; set; }

        [XmlElement("code")]
        public string Code { get; set; }

        [XmlElement("message")]
        public string Message { get; set; }

        public string Body { get; set; }

        public bool IsError => !"success".Equals(Flag);
    }
}
