using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls
{
    public abstract class TlsSrpUtilities
    {
        public static void AddSrpExtension(IDictionary extensions, byte[] identity)
        {
            extensions[ExtensionType.srp] = CreateSrpExtension(identity);
        }

        public static byte[] GetSrpExtension(IDictionary extensions)
        {
            byte[] extensionData = TlsUtilities.GetExtensionData(extensions, ExtensionType.srp);
            return extensionData == null ? null : ReadSrpExtension(extensionData);
        }

        public static byte[] CreateSrpExtension(byte[] identity)
        {
            if (identity == null)
                throw new TlsFatalAlert(AlertDescription.internal_error);

            return TlsUtilities.EncodeOpaque8(identity);
        }

        public static byte[] ReadSrpExtension(byte[] extensionData)
        {
            if (extensionData == null)
                throw new ArgumentNullException("extensionData");

            MemoryStream buf = new MemoryStream(extensionData, false);
            byte[] identity = TlsUtilities.ReadOpaque8(buf);

            TlsProtocol.AssertEmpty(buf);

            return identity;
        }

        public static BigInteger ReadSrpParameter(Stream input)
        {
            return new BigInteger(1, TlsUtilities.ReadOpaque16(input));
        }

        public static void WriteSrpParameter(BigInteger x, Stream output)
        {
            TlsUtilities.WriteOpaque16(BigIntegers.AsUnsignedByteArray(x), output);
        }

        public static bool IsSrpCipherSuite(int cipherSuite)
        {
            switch (cipherSuite)
            {
            case CipherSuite.TLS_SRP_SHA_DSS_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_SRP_SHA_DSS_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_SRP_SHA_DSS_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_SRP_SHA_RSA_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_SRP_SHA_RSA_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_SRP_SHA_RSA_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_SRP_SHA_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_SRP_SHA_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_SRP_SHA_WITH_AES_256_CBC_SHA:
                return true;

            default:
                return false;
            }
        }
    }
}
