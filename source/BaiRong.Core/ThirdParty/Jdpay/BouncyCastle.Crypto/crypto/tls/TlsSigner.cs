using System;

namespace Org.BouncyCastle.Crypto.Tls
{
    public interface TlsSigner
    {
        void Init(TlsContext context);

        byte[] GenerateRawSignature(AsymmetricKeyParameter privateKey, byte[] md5AndSha1);

        byte[] GenerateRawSignature(SignatureAndHashAlgorithm algorithm,
            AsymmetricKeyParameter privateKey, byte[] hash);

        bool VerifyRawSignature(byte[] sigBytes, AsymmetricKeyParameter publicKey, byte[] md5AndSha1);

        bool VerifyRawSignature(SignatureAndHashAlgorithm algorithm, byte[] sigBytes,
            AsymmetricKeyParameter publicKey, byte[] hash);

        ISigner CreateSigner(AsymmetricKeyParameter privateKey);

        ISigner CreateSigner(SignatureAndHashAlgorithm algorithm, AsymmetricKeyParameter privateKey);

        ISigner CreateVerifyer(AsymmetricKeyParameter publicKey);

        ISigner CreateVerifyer(SignatureAndHashAlgorithm algorithm, AsymmetricKeyParameter publicKey);

        bool IsValidPublicKey(AsymmetricKeyParameter publicKey);
    }
}
