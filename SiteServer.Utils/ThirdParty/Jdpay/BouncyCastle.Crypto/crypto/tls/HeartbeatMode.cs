using System;

namespace Org.BouncyCastle.Crypto.Tls
{
    /*
     * RFC 6520
     */
    public abstract class HeartbeatMode
    {
        public const byte peer_allowed_to_send = 1;
        public const byte peer_not_allowed_to_send = 2;

        public static bool IsValid(byte heartbeatMode)
        {
            return heartbeatMode >= peer_allowed_to_send && heartbeatMode <= peer_not_allowed_to_send;
        }
    }
}
