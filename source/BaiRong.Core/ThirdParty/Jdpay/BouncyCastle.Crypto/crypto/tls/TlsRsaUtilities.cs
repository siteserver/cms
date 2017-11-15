using System;
using System.IO;

using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls
{
    public abstract class TlsRsaUtilities
    {
        /// <exception cref="IOException"></exception>
        public static byte[] GenerateEncryptedPreMasterSecret(TlsContext context, RsaKeyParameters rsaServerPublicKey,
            Stream output)
        {
            /*
             * Choose a PremasterSecret and send it encrypted to the server
             */
            byte[] premasterSecret = new byte[48];
            context.SecureRandom.NextBytes(premasterSecret);
            TlsUtilities.WriteVersion(context.ClientVersion, premasterSecret, 0);

            Pkcs1Encoding encoding = new Pkcs1Encoding(new RsaBlindedEngine());
            encoding.Init(true, new ParametersWithRandom(rsaServerPublicKey, context.SecureRandom));

            try
            {
                byte[] encryptedPreMasterSecret = encoding.ProcessBlock(premasterSecret, 0, premasterSecret.Length);

                if (TlsUtilities.IsSsl(context))
                {
                    // TODO Do any SSLv3 servers actually expect the length?
                    output.Write(encryptedPreMasterSecret, 0, encryptedPreMasterSecret.Length);
                }
                else
                {
                    TlsUtilities.WriteOpaque16(encryptedPreMasterSecret, output);
                }
            }
            catch (InvalidCipherTextException e)
            {
                /*
                 * This should never happen, only during decryption.
                 */
                throw new TlsFatalAlert(AlertDescription.internal_error, e);
            }

            return premasterSecret;
        }

        public static byte[] SafeDecryptPreMasterSecret(TlsContext context, RsaKeyParameters rsaServerPrivateKey,
            byte[] encryptedPreMasterSecret)
        {
            /*
             * RFC 5246 7.4.7.1.
             */
            ProtocolVersion clientVersion = context.ClientVersion;

            // TODO Provide as configuration option?
            bool versionNumberCheckDisabled = false;

            /*
             * Generate 48 random bytes we can use as a Pre-Master-Secret, if the
             * PKCS1 padding check should fail.
             */
            byte[] fallback = new byte[48];
            context.SecureRandom.NextBytes(fallback);

            byte[] M = Arrays.Clone(fallback);
            try
            {
                Pkcs1Encoding encoding = new Pkcs1Encoding(new RsaBlindedEngine(), fallback);
                encoding.Init(false,
                    new ParametersWithRandom(rsaServerPrivateKey, context.SecureRandom));

                M = encoding.ProcessBlock(encryptedPreMasterSecret, 0, encryptedPreMasterSecret.Length);
            }
            catch (Exception)
            {
                /*
                 * This should never happen since the decryption should never throw an exception
                 * and return a random value instead.
                 *
                 * In any case, a TLS server MUST NOT generate an alert if processing an
                 * RSA-encrypted premaster secret message fails, or the version number is not as
                 * expected. Instead, it MUST continue the handshake with a randomly generated
                 * premaster secret.
                 */
            }

            /*
             * If ClientHello.client_version is TLS 1.1 or higher, server implementations MUST
             * check the version number [..].
             */
            if (versionNumberCheckDisabled && clientVersion.IsEqualOrEarlierVersionOf(ProtocolVersion.TLSv10))
            {
                /*
                 * If the version number is TLS 1.0 or earlier, server
                 * implementations SHOULD check the version number, but MAY have a
                 * configuration option to disable the check.
                 *
                 * So there is nothing to do here.
                 */
            }
            else
            {
                /*
                 * OK, we need to compare the version number in the decrypted Pre-Master-Secret with the
                 * clientVersion received during the handshake. If they don't match, we replace the
                 * decrypted Pre-Master-Secret with a random one.
                 */
                int correct = (clientVersion.MajorVersion ^ (M[0] & 0xff))
                    | (clientVersion.MinorVersion ^ (M[1] & 0xff));
                correct |= correct >> 1;
                correct |= correct >> 2;
                correct |= correct >> 4;
                int mask = ~((correct & 1) - 1);

                /*
                 * mask will be all bits set to 0xff if the version number differed.
                 */
                for (int i = 0; i < 48; i++)
                {
                    M[i] = (byte)((M[i] & (~mask)) | (fallback[i] & mask));
                }
            }
            return M;
        }
    }
}
