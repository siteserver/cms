using System;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls
{
    /**
     * HMAC implementation based on original internet draft for HMAC (RFC 2104)
     * 
     * The difference is that padding is concatentated versus XORed with the key
     * 
     * H(K + opad, H(K + ipad, text))
     */
    public class Ssl3Mac
        : IMac
    {
        private const byte IPAD_BYTE = 0x36;
        private const byte OPAD_BYTE = 0x5C;

        internal static readonly byte[] IPAD = GenPad(IPAD_BYTE, 48);
        internal static readonly byte[] OPAD = GenPad(OPAD_BYTE, 48);

        private readonly IDigest digest;
        private readonly int padLength;

        private byte[] secret;

        /**
         * Base constructor for one of the standard digest algorithms that the byteLength of
         * the algorithm is know for. Behaviour is undefined for digests other than MD5 or SHA1.
         * 
         * @param digest the digest.
         */
        public Ssl3Mac(IDigest digest)
        {
            this.digest = digest;

            if (digest.GetDigestSize() == 20)
            {
                this.padLength = 40;
            }
            else
            {
                this.padLength = 48;
            }
        }

        public virtual string AlgorithmName
        {
            get { return digest.AlgorithmName + "/SSL3MAC"; }
        }

        public virtual void Init(ICipherParameters parameters)
        {
            secret = Arrays.Clone(((KeyParameter)parameters).GetKey());

            Reset();
        }

        public virtual int GetMacSize()
        {
            return digest.GetDigestSize();
        }

        public virtual void Update(byte input)
        {
            digest.Update(input);
        }

        public virtual void BlockUpdate(byte[] input, int inOff, int len)
        {
            digest.BlockUpdate(input, inOff, len);
        }

        public virtual int DoFinal(byte[] output, int outOff)
        {
            byte[] tmp = new byte[digest.GetDigestSize()];
            digest.DoFinal(tmp, 0);

            digest.BlockUpdate(secret, 0, secret.Length);
            digest.BlockUpdate(OPAD, 0, padLength);
            digest.BlockUpdate(tmp, 0, tmp.Length);

            int len = digest.DoFinal(output, outOff);

            Reset();

            return len;
        }

        /**
         * Reset the mac generator.
         */
        public virtual void Reset()
        {
            digest.Reset();
            digest.BlockUpdate(secret, 0, secret.Length);
            digest.BlockUpdate(IPAD, 0, padLength);
        }

        private static byte[] GenPad(byte b, int count)
        {
            byte[] padding = new byte[count];
            Arrays.Fill(padding, b);
            return padding;
        }
    }
}
