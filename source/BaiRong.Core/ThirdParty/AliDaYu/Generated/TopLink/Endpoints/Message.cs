using System.Collections.Generic;

namespace Taobao.Top.Link.Endpoints
{
    /// <summary>for endpoints talking
    /// </summary>
    public class Message
    {
        public int ProtocolVersion { get; set; }
        public short MessageType { get; set; }

        public int StatusCode { get; set; }
        public string StatusPhase { get; set; }
        public int Flag { get; set; }
        public string Token { get; set; }

        public IDictionary<string, object> Content { get; set; }

        public Message()
        {
            this.ProtocolVersion = 2;
        }
    }
}