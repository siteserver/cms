namespace Org.BouncyCastle.Crypto.Tls
{
    public abstract class HandshakeType
    {
        /*
         * RFC 2246 7.4
         */
        public const byte hello_request = 0;
        public const byte client_hello = 1;
        public const byte server_hello = 2;
        public const byte certificate = 11;
        public const byte server_key_exchange = 12;
        public const byte certificate_request = 13;
        public const byte server_hello_done = 14;
        public const byte certificate_verify = 15;
        public const byte client_key_exchange = 16;
        public const byte finished = 20;

        /*
         * RFC 3546 2.4
         */
        public const byte certificate_url = 21;
        public const byte certificate_status = 22;

        /*
         *  (DTLS) RFC 4347 4.3.2
         */
        public const byte hello_verify_request = 3;

        /*
         * RFC 4680 
         */
        public const byte supplemental_data = 23;

        /*
         * RFC 5077 
         */
        public const byte session_ticket = 4;
    }
}
