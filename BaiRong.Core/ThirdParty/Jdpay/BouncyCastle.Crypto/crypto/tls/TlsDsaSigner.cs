using System;

using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Tls
{
    public abstract class TlsDsaSigner
        :	AbstractTlsSigner
    {
        public override byte[] GenerateRawSignature(SignatureAndHashAlgorithm algorithm,
            AsymmetricKeyParameter privateKey, byte[] hash)
        {
            ISigner signer = MakeSigner(algorithm, true, true,
                new ParametersWithRandom(privateKey, this.mContext.SecureRandom));
            if (algorithm == null)
            {
                // Note: Only use the SHA1 part of the (MD5/SHA1) hash
                signer.BlockUpdate(hash, 16, 20);
            }
            else
            {
                signer.BlockUpdate(hash, 0, hash.Length);
            }
            return signer.GenerateSignature();
        }

        public override bool VerifyRawSignature(SignatureAndHashAlgorithm algorithm, byte[] sigBytes,
            AsymmetricKeyParameter publicKey, byte[] hash)
        {
            ISigner signer = MakeSigner(algorithm, true, false, publicKey);
            if (algorithm == null)
            {
                // Note: Only use the SHA1 part of the (MD5/SHA1) hash
                signer.BlockUpdate(hash, 16, 20);
            }
            else
            {
                signer.BlockUpdate(hash, 0, hash.Length);
            }
            return signer.VerifySignature(sigBytes);
        }

        public override ISigner CreateSigner(SignatureAndHashAlgorithm algorithm, AsymmetricKeyParameter privateKey)
        {
            return MakeSigner(algorithm, false, true, privateKey);
        }

        public override ISigner CreateVerifyer(SignatureAndHashAlgorithm algorithm, AsymmetricKeyParameter publicKey)
        {
            return MakeSigner(algorithm, false, false, publicKey);
        }

        protected virtual ICipherParameters MakeInitParameters(bool forSigning, ICipherParameters cp)
        {
            return cp;
        }

        protected virtual ISigner MakeSigner(SignatureAndHashAlgorithm algorithm, bool raw, bool forSigning,
            ICipherParameters cp)
        {
            if ((algorithm != null) != TlsUtilities.IsTlsV12(mContext))
                throw new InvalidOperationException();

            if (algorithm != null && algorithm.Signature != SignatureAlgorithm)
                throw new InvalidOperationException();

            byte hashAlgorithm = algorithm == null ? HashAlgorithm.sha1 : algorithm.Hash;
            IDigest d = raw ? new NullDigest() : TlsUtilities.CreateHash(hashAlgorithm);

            ISigner s = new DsaDigestSigner(CreateDsaImpl(hashAlgorithm), d);
            s.Init(forSigning, MakeInitParameters(forSigning, cp));
            return s;
        }

        protected abstract byte SignatureAlgorithm { get; }

        protected abstract IDsa CreateDsaImpl(byte hashAlgorithm);
    }
}
