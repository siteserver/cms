using System;
using System.Collections.Generic;

namespace Taobao.Top.Link.Endpoints
{
    public class AckMessageArgs : EventArgs
    {
        /// <summary>get ack message
        /// </summary>
        public IDictionary<string, object> Message { get; private set; }
        /// <summary>get where the ack message come from
        /// </summary>
        public Identity MessageFrom { get; private set; }

        public AckMessageArgs(IDictionary<string, object> message, Identity messageFrom)
        {
            this.Message = message;
            this.MessageFrom = messageFrom;
        }
    }
}