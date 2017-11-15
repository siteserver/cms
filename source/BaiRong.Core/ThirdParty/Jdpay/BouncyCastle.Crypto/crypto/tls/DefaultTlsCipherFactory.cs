using System;
using System.IO;

using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;

namespace Org.BouncyCastle.Crypto.Tls
{
    public class DefaultTlsCipherFactory
        :   AbstractTlsCipherFactory
    {
        /// <exception cref="IOException"></exception>
        public override TlsCipher CreateCipher(TlsContext context, int encryptionAlgorithm, int macAlgorithm)
        {
            switch (encryptionAlgorithm)
            {
            case EncryptionAlgorithm.cls_3DES_EDE_CBC:
                return CreateDesEdeCipher(context, macAlgorithm);
            case EncryptionAlgorithm.AES_128_CBC:
                return CreateAESCipher(context, 16, macAlgorithm);
            case EncryptionAlgorithm.AES_128_CCM:
                // NOTE: Ignores macAlgorithm
                return CreateCipher_Aes_Ccm(context, 16, 16);
            case EncryptionAlgorithm.AES_128_CCM_8:
                // NOTE: Ignores macAlgorithm
                return CreateCipher_Aes_Ccm(context, 16, 8);
            case EncryptionAlgorithm.AES_128_GCM:
                // NOTE: Ignores macAlgorithm
                return CreateCipher_Aes_Gcm(context, 16, 16);
            case EncryptionAlgorithm.AES_128_OCB_TAGLEN96:
                // NOTE: Ignores macAlgorithm
                return CreateCipher_Aes_Ocb(context, 16, 12);
            case EncryptionAlgorithm.AES_256_CBC:
                return CreateAESCipher(context, 32, macAlgorithm);
            case EncryptionAlgorithm.AES_256_CCM:
                // NOTE: Ignores macAlgorithm
                return CreateCipher_Aes_Ccm(context, 32, 16);
            case EncryptionAlgorithm.AES_256_CCM_8:
                // NOTE: Ignores macAlgorithm
                return CreateCipher_Aes_Ccm(context, 32, 8);
            case EncryptionAlgorithm.AES_256_GCM:
                // NOTE: Ignores macAlgorithm
                return CreateCipher_Aes_Gcm(context, 32, 16);
            case EncryptionAlgorithm.AES_256_OCB_TAGLEN96:
                // NOTE: Ignores macAlgorithm
                return CreateCipher_Aes_Ocb(context, 32, 12);
            case EncryptionAlgorithm.CAMELLIA_128_CBC:
                return CreateCamelliaCipher(context, 16, macAlgorithm);
            case EncryptionAlgorithm.CAMELLIA_128_GCM:
                // NOTE: Ignores macAlgorithm
                return CreateCipher_Camellia_Gcm(context, 16, 16);
            case EncryptionAlgorithm.CAMELLIA_256_CBC:
                return CreateCamelliaCipher(context, 32, macAlgorithm);
            case EncryptionAlgorithm.CAMELLIA_256_GCM:
                // NOTE: Ignores macAlgorithm
                return CreateCipher_Camellia_Gcm(context, 32, 16);
            case EncryptionAlgorithm.CHACHA20_POLY1305:
                // NOTE: Ignores macAlgorithm
                return CreateChaCha20Poly1305(context);
            case EncryptionAlgorithm.NULL:
                return CreateNullCipher(context, macAlgorithm);
            case EncryptionAlgorithm.RC4_128:
                return CreateRC4Cipher(context, 16, macAlgorithm);
            case EncryptionAlgorithm.SEED_CBC:
                return CreateSeedCipher(context, macAlgorithm);
            default:
                throw new TlsFatalAlert(AlertDescription.internal_error);
            }
        }

        /// <exception cref="IOException"></exception>
        protected virtual TlsBlockCipher CreateAESCipher(TlsContext context, int cipherKeySize, int macAlgorithm)
        {
            return new TlsBlockCipher(context, CreateAesBlockCipher(), CreateAesBlockCipher(),
                CreateHMacDigest(macAlgorithm), CreateHMacDigest(macAlgorithm), cipherKeySize);
        }

        /// <exception cref="IOException"></exception>
        protected virtual TlsBlockCipher CreateCamelliaCipher(TlsContext context, int cipherKeySize, int macAlgorithm)
        {
            return new TlsBlockCipher(context, CreateCamelliaBlockCipher(),
                CreateCamelliaBlockCipher(), CreateHMacDigest(macAlgorithm),
                CreateHMacDigest(macAlgorithm), cipherKeySize);
        }

        /// <exception cref="IOException"></exception>
        protected virtual TlsCipher CreateChaCha20Poly1305(TlsContext context)
        {
            return new Chacha20Poly1305(context);
        }

        /// <exception cref="IOException"></exception>
        protected virtual TlsAeadCipher CreateCipher_Aes_Ccm(TlsContext context, int cipherKeySize, int macSize)
        {
            return new TlsAeadCipher(context, CreateAeadBlockCipher_Aes_Ccm(),
                CreateAeadBlockCipher_Aes_Ccm(), cipherKeySize, macSize);
        }

        /// <exception cref="IOException"></exception>
        protected virtual TlsAeadCipher CreateCipher_Aes_Gcm(TlsContext context, int cipherKeySize, int macSize)
        {
            return new TlsAeadCipher(context, CreateAeadBlockCipher_Aes_Gcm(),
                CreateAeadBlockCipher_Aes_Gcm(), cipherKeySize, macSize);
        }

        /// <exception cref="IOException"></exception>
        protected virtual TlsAeadCipher CreateCipher_Aes_Ocb(TlsContext context, int cipherKeySize, int macSize)
        {
            return new TlsAeadCipher(context, CreateAeadBlockCipher_Aes_Ocb(),
                CreateAeadBlockCipher_Aes_Ocb(), cipherKeySize, macSize, TlsAeadCipher.NONCE_DRAFT_CHACHA20_POLY1305);
        }

        /// <exception cref="IOException"></exception>
        protected virtual TlsAeadCipher CreateCipher_Camellia_Gcm(TlsContext context, int cipherKeySize, int macSize)
        {
            return new TlsAeadCipher(context, CreateAeadBlockCipher_Camellia_Gcm(),
                CreateAeadBlockCipher_Camellia_Gcm(), cipherKeySize, macSize);
        }

        /// <exception cref="IOException"></exception>
        protected virtual TlsBlockCipher CreateDesEdeCipher(TlsContext context, int macAlgorithm)
        {
            return new TlsBlockCipher(context, CreateDesEdeBlockCipher(), CreateDesEdeBlockCipher(),
                CreateHMacDigest(macAlgorithm), CreateHMacDigest(macAlgorithm), 24);
        }

        /// <exception cref="IOException"></exception>
        protected virtual TlsNullCipher CreateNullCipher(TlsContext context, int macAlgorithm)
        {
            return new TlsNullCipher(context, CreateHMacDigest(macAlgorithm),
                CreateHMacDigest(macAlgorithm));
        }

        /// <exception cref="IOException"></exception>
        protected virtual TlsStreamCipher CreateRC4Cipher(TlsContext context, int cipherKeySize, int macAlgorithm)
        {
            return new TlsStreamCipher(context, CreateRC4StreamCipher(), CreateRC4StreamCipher(),
                CreateHMacDigest(macAlgorithm), CreateHMacDigest(macAlgorithm), cipherKeySize, false);
        }

        /// <exception cref="IOException"></exception>
        protected virtual TlsBlockCipher CreateSeedCipher(TlsContext context, int macAlgorithm)
        {
            return new TlsBlockCipher(context, CreateSeedBlockCipher(), CreateSeedBlockCipher(),
                CreateHMacDigest(macAlgorithm), CreateHMacDigest(macAlgorithm), 16);
        }

        protected virtual IBlockCipher CreateAesEngine()
        {
            return new AesEngine();
        }

        protected virtual IBlockCipher CreateCamelliaEngine()
        {
            return new CamelliaEngine();
        }

        protected virtual IBlockCipher CreateAesBlockCipher()
        {
            return new CbcBlockCipher(CreateAesEngine());
        }

        protected virtual IAeadBlockCipher CreateAeadBlockCipher_Aes_Ccm()
        {
            return new CcmBlockCipher(CreateAesEngine());
        }

        protected virtual IAeadBlockCipher CreateAeadBlockCipher_Aes_Gcm()
        {
            // TODO Consider allowing custom configuration of multiplier
            return new GcmBlockCipher(CreateAesEngine());
        }

        protected virtual IAeadBlockCipher CreateAeadBlockCipher_Aes_Ocb()
        {
            return new OcbBlockCipher(CreateAesEngine(), CreateAesEngine());
        }

        protected virtual IAeadBlockCipher CreateAeadBlockCipher_Camellia_Gcm()
        {
            // TODO Consider allowing custom configuration of multiplier
            return new GcmBlockCipher(CreateCamelliaEngine());
        }

        protected virtual IBlockCipher CreateCamelliaBlockCipher()
        {
            return new CbcBlockCipher(CreateCamelliaEngine());
        }

        protected virtual IBlockCipher CreateDesEdeBlockCipher()
        {
            return new CbcBlockCipher(new DesEdeEngine());
        }

        protected virtual IStreamCipher CreateRC4StreamCipher()
        {
            return new RC4Engine();
        }

        protected virtual IBlockCipher CreateSeedBlockCipher()
        {
            return new CbcBlockCipher(new SeedEngine());
        }

        /// <exception cref="IOException"></exception>
        protected virtual IDigest CreateHMacDigest(int macAlgorithm)
        {
            switch (macAlgorithm)
            {
            case MacAlgorithm.cls_null:
                return null;
            case MacAlgorithm.hmac_md5:
                return TlsUtilities.CreateHash(HashAlgorithm.md5);
            case MacAlgorithm.hmac_sha1:
                return TlsUtilities.CreateHash(HashAlgorithm.sha1);
            case MacAlgorithm.hmac_sha256:
                return TlsUtilities.CreateHash(HashAlgorithm.sha256);
            case MacAlgorithm.hmac_sha384:
                return TlsUtilities.CreateHash(HashAlgorithm.sha384);
            case MacAlgorithm.hmac_sha512:
                return TlsUtilities.CreateHash(HashAlgorithm.sha512);
            default:
                throw new TlsFatalAlert(AlertDescription.internal_error);
            }
        }
    }
}
