namespace Org.BouncyCastle.Crypto.Tls
{
    /// <summary>
    /// RFC 4492 5.1.2
    /// </summary>
    public abstract class ECPointFormat
    {
        public const byte uncompressed = 0;
        public const byte ansiX962_compressed_prime = 1;
        public const byte ansiX962_compressed_char2 = 2;

        /*
         * reserved (248..255)
         */
    }
}
