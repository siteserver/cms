using System;
using System.Collections;
using System.IO;
using System.Text;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.TeleTrust;
using Org.BouncyCastle.Asn1.Utilities;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Signers
{
    public class RsaDigestSigner
        : ISigner
    {
        private readonly IAsymmetricBlockCipher rsaEngine = new Pkcs1Encoding(new RsaBlindedEngine());
        private readonly AlgorithmIdentifier algId;
        private readonly IDigest digest;
        private bool forSigning;

        private static readonly IDictionary oidMap = Platform.CreateHashtable();

        /// <summary>
        /// Load oid table.
        /// </summary>
        static RsaDigestSigner()
        {
            oidMap["RIPEMD128"] = TeleTrusTObjectIdentifiers.RipeMD128;
            oidMap["RIPEMD160"] = TeleTrusTObjectIdentifiers.RipeMD160;
            oidMap["RIPEMD256"] = TeleTrusTObjectIdentifiers.RipeMD256;

            oidMap["SHA-1"] = X509ObjectIdentifiers.IdSha1;
            oidMap["SHA-224"] = NistObjectIdentifiers.IdSha224;
            oidMap["SHA-256"] = NistObjectIdentifiers.IdSha256;
            oidMap["SHA-384"] = NistObjectIdentifiers.IdSha384;
            oidMap["SHA-512"] = NistObjectIdentifiers.IdSha512;

            oidMap["MD2"] = PkcsObjectIdentifiers.MD2;
            oidMap["MD4"] = PkcsObjectIdentifiers.MD4;
            oidMap["MD5"] = PkcsObjectIdentifiers.MD5;
        }

        public RsaDigestSigner(IDigest digest)
            :   this(digest, (DerObjectIdentifier)oidMap[digest.AlgorithmName])
        {
        }

        public RsaDigestSigner(IDigest digest, DerObjectIdentifier digestOid)
            :   this(digest, new AlgorithmIdentifier(digestOid, DerNull.Instance))
        {
        }

        public RsaDigestSigner(IDigest digest, AlgorithmIdentifier algId)
        {
            this.digest = digest;
            this.algId = algId;
        }

        public virtual string AlgorithmName
        {
            get { return digest.AlgorithmName + "withRSA"; }
        }

        /**
         * Initialise the signer for signing or verification.
         *
         * @param forSigning true if for signing, false otherwise
         * @param param necessary parameters.
         */
        public virtual void Init(
            bool				forSigning,
            ICipherParameters	parameters)
        {
            this.forSigning = forSigning;
            AsymmetricKeyParameter k;

            if (parameters is ParametersWithRandom)
            {
                k = (AsymmetricKeyParameter)((ParametersWithRandom)parameters).Parameters;
            }
            else
            {
                k = (AsymmetricKeyParameter)parameters;
            }

            if (forSigning && !k.IsPrivate)
                throw new InvalidKeyException("Signing requires private key.");

            if (!forSigning && k.IsPrivate)
                throw new InvalidKeyException("Verification requires public key.");

            Reset();

            rsaEngine.Init(forSigning, parameters);
        }

        /**
         * update the internal digest with the byte b
         */
        public virtual void Update(
            byte input)
        {
            digest.Update(input);
        }

        /**
         * update the internal digest with the byte array in
         */
        public virtual void BlockUpdate(
            byte[]	input,
            int		inOff,
            int		length)
        {
            digest.BlockUpdate(input, inOff, length);
        }

        /**
         * Generate a signature for the message we've been loaded with using
         * the key we were initialised with.
         */
        public virtual byte[] GenerateSignature()
        {
            if (!forSigning)
                throw new InvalidOperationException("RsaDigestSigner not initialised for signature generation.");

            byte[] hash = new byte[digest.GetDigestSize()];
            digest.DoFinal(hash, 0);

            byte[] data = DerEncode(hash);
            return rsaEngine.ProcessBlock(data, 0, data.Length);
        }

        /**
         * return true if the internal state represents the signature described
         * in the passed in array.
         */
        public virtual bool VerifySignature(
            byte[] signature)
        {
            if (forSigning)
                throw new InvalidOperationException("RsaDigestSigner not initialised for verification");

            byte[] hash = new byte[digest.GetDigestSize()];
            digest.DoFinal(hash, 0);

            byte[] sig;
            byte[] expected;

            try
            {
                sig = rsaEngine.ProcessBlock(signature, 0, signature.Length);
                expected = DerEncode(hash);
            }
            catch (Exception)
            {
                return false;
            }

            if (sig.Length == expected.Length)
            {
                return Arrays.ConstantTimeAreEqual(sig, expected);
            }
            else if (sig.Length == expected.Length - 2)  // NULL left out
            {
                int sigOffset = sig.Length - hash.Length - 2;
                int expectedOffset = expected.Length - hash.Length - 2;

                expected[1] -= 2;      // adjust lengths
                expected[3] -= 2;

                int nonEqual = 0;

                for (int i = 0; i < hash.Length; i++)
                {
                    nonEqual |= (sig[sigOffset + i] ^ expected[expectedOffset + i]);
                }

                for (int i = 0; i < sigOffset; i++)
                {
                    nonEqual |= (sig[i] ^ expected[i]);  // check header less NULL
                }

                return nonEqual == 0;
            }
            else
            {
                return false;
            }
        }

        public virtual void Reset()
        {
            digest.Reset();
        }

        private byte[] DerEncode(byte[] hash)
        {
            if (algId == null)
            {
                // For raw RSA, the DigestInfo must be prepared externally
                return hash;
            }

            DigestInfo dInfo = new DigestInfo(algId, hash);

            return dInfo.GetDerEncoded();
        }
    }
}
