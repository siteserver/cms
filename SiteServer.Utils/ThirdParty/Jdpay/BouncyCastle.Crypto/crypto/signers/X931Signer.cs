using System;
using System.Collections;

using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Signers
{
    /**
     * X9.31-1998 - signing using a hash.
     * <p>
     * The message digest hash, H, is encapsulated to form a byte string as follows
     * </p>
     * <pre>
     * EB = 06 || PS || 0xBA || H || TRAILER
     * </pre>
     * where PS is a string of bytes all of value 0xBB of length such that |EB|=|n|, and TRAILER is the ISO/IEC 10118 part numberâ€  for the digest. The byte string, EB, is converted to an integer value, the message representative, f.
     */
    public class X931Signer
        :   ISigner
    {
        [Obsolete("Use 'IsoTrailers' instead")]
        public const int TRAILER_IMPLICIT = 0xBC;
        [Obsolete("Use 'IsoTrailers' instead")]
        public const int TRAILER_RIPEMD160 = 0x31CC;
        [Obsolete("Use 'IsoTrailers' instead")]
        public const int TRAILER_RIPEMD128 = 0x32CC;
        [Obsolete("Use 'IsoTrailers' instead")]
        public const int TRAILER_SHA1 = 0x33CC;
        [Obsolete("Use 'IsoTrailers' instead")]
        public const int TRAILER_SHA256 = 0x34CC;
        [Obsolete("Use 'IsoTrailers' instead")]
        public const int TRAILER_SHA512 = 0x35CC;
        [Obsolete("Use 'IsoTrailers' instead")]
        public const int TRAILER_SHA384 = 0x36CC;
        [Obsolete("Use 'IsoTrailers' instead")]
        public const int TRAILER_WHIRLPOOL = 0x37CC;
        [Obsolete("Use 'IsoTrailers' instead")]
        public const int TRAILER_SHA224 = 0x38CC;

        private IDigest                     digest;
        private IAsymmetricBlockCipher      cipher;
        private RsaKeyParameters            kParam;

        private int         trailer;
        private int         keyBits;
        private byte[]      block;

        /**
         * Generate a signer with either implicit or explicit trailers for X9.31.
         *
         * @param cipher base cipher to use for signature creation/verification
         * @param digest digest to use.
         * @param implicit whether or not the trailer is implicit or gives the hash.
         */
        public X931Signer(IAsymmetricBlockCipher cipher, IDigest digest, bool isImplicit)
        {
            this.cipher = cipher;
            this.digest = digest;

            if (isImplicit)
            {
                trailer = IsoTrailers.TRAILER_IMPLICIT;
            }
            else if (IsoTrailers.NoTrailerAvailable(digest))
            {
                throw new ArgumentException("no valid trailer", "digest");
            }
            else
            {
                trailer = IsoTrailers.GetTrailer(digest);
            }
        }

        public virtual string AlgorithmName
        {
            get { return digest.AlgorithmName + "with" + cipher.AlgorithmName + "/X9.31"; }
        }

        /**
         * Constructor for a signer with an explicit digest trailer.
         *
         * @param cipher cipher to use.
         * @param digest digest to sign with.
         */
        public X931Signer(IAsymmetricBlockCipher cipher, IDigest digest)
            :   this(cipher, digest, false)
        {
        }

        public virtual void Init(bool forSigning, ICipherParameters parameters)
        {
            kParam = (RsaKeyParameters)parameters;

            cipher.Init(forSigning, kParam);

            keyBits = kParam.Modulus.BitLength;

            block = new byte[(keyBits + 7) / 8];

            Reset();
        }

        /// <summary> clear possible sensitive data</summary>
        private void ClearBlock(byte[] block)
        {
            Array.Clear(block, 0, block.Length);
        }

        /**
         * update the internal digest with the byte b
         */
        public virtual void Update(byte b)
        {
            digest.Update(b);
        }

        /**
         * update the internal digest with the byte array in
         */
        public virtual void BlockUpdate(byte[] input, int off, int len)
        {
            digest.BlockUpdate(input, off, len);
        }

        /**
         * reset the internal state
         */
        public virtual void Reset()
        {
            digest.Reset();
        }

        /**
         * generate a signature for the loaded message using the key we were
         * initialised with.
         */
        public virtual byte[] GenerateSignature()
        {
            CreateSignatureBlock();

            BigInteger t = new BigInteger(1, cipher.ProcessBlock(block, 0, block.Length));
            ClearBlock(block);

            t = t.Min(kParam.Modulus.Subtract(t));

            return BigIntegers.AsUnsignedByteArray((kParam.Modulus.BitLength + 7) / 8, t);
        }

        private void CreateSignatureBlock()
        {
            int digSize = digest.GetDigestSize();

            int delta;
            if (trailer == IsoTrailers.TRAILER_IMPLICIT)
            {
                delta = block.Length - digSize - 1;
                digest.DoFinal(block, delta);
                block[block.Length - 1] = (byte)IsoTrailers.TRAILER_IMPLICIT;
            }
            else
            {
                delta = block.Length - digSize - 2;
                digest.DoFinal(block, delta);
                block[block.Length - 2] = (byte)(trailer >> 8);
                block[block.Length - 1] = (byte)trailer;
            }

            block[0] = 0x6b;
            for (int i = delta - 2; i != 0; i--)
            {
                block[i] = (byte)0xbb;
            }
            block[delta - 1] = (byte)0xba;
        }

        /**
         * return true if the signature represents a ISO9796-2 signature
         * for the passed in message.
         */
        public virtual bool VerifySignature(byte[] signature)
        {
            try
            {
                block = cipher.ProcessBlock(signature, 0, signature.Length);
            }
            catch (Exception)
            {
                return false;
            }

            BigInteger t = new BigInteger(1, block);
            BigInteger f;

            if ((t.IntValue & 15) == 12)
            {
                 f = t;
            }
            else
            {
                t = kParam.Modulus.Subtract(t);
                if ((t.IntValue & 15) == 12)
                {
                     f = t;
                }
                else
                {
                    return false;
                }
            }

            CreateSignatureBlock();

            byte[] fBlock = BigIntegers.AsUnsignedByteArray(block.Length, f);

            bool rv = Arrays.ConstantTimeAreEqual(block, fBlock);

            ClearBlock(block);
            ClearBlock(fBlock);

            return rv;
        }
    }
}
