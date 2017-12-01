using System;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
    public interface TlsCipherFactory
    {
        /// <exception cref="IOException"></exception>
        TlsCipher CreateCipher(TlsContext context, int encryptionAlgorithm, int macAlgorithm);
    }
}
