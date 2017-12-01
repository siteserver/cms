using System;

namespace Org.BouncyCastle.Crypto.Tls
{
    /*
     * RFC 6520 3.
     */
    public abstract class HeartbeatMessageType
    {
        public const byte heartbeat_request = 1;
        public const byte heartbeat_response = 2;

        public static bool IsValid(byte heartbeatMessageType)
        {
            return heartbeatMessageType >= heartbeat_request && heartbeatMessageType <= heartbeat_response;
        }
    }
}
