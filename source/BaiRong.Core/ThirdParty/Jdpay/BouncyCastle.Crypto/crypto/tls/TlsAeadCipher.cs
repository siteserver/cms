using System;
using System.IO;

using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls
{
    public class TlsAeadCipher
        :   TlsCipher
    {
        // TODO[draft-zauner-tls-aes-ocb-04] Apply data volume limit described in section 8.4

        public const int NONCE_RFC5288 = 1;
    
        /*
         * draft-zauner-tls-aes-ocb-04 specifies the nonce construction from draft-ietf-tls-chacha20-poly1305-04
         */
        internal const int NONCE_DRAFT_CHACHA20_POLY1305 = 2;

        protected readonly TlsContext context;
        protected readonly int macSize;
        // TODO SecurityParameters.record_iv_length
        protected readonly int record_iv_length;

        protected readonly IAeadBlockCipher encryptCipher;
        protected readonly IAeadBlockCipher decryptCipher;

        protected readonly byte[] encryptImplicitNonce, decryptImplicitNonce;

        protected readonly int nonceMode;

        /// <exception cref="IOException"></exception>
        public TlsAeadCipher(TlsContext context, IAeadBlockCipher clientWriteCipher, IAeadBlockCipher serverWriteCipher,
            int cipherKeySize, int macSize)
            : this(context, clientWriteCipher, serverWriteCipher, cipherKeySize, macSize, NONCE_RFC5288)
        {
        }

        /// <exception cref="IOException"></exception>
        internal TlsAeadCipher(TlsContext context, IAeadBlockCipher clientWriteCipher, IAeadBlockCipher serverWriteCipher,
            int cipherKeySize, int macSize, int nonceMode)
        {
            if (!TlsUtilities.IsTlsV12(context))
                throw new TlsFatalAlert(AlertDescription.internal_error);

            this.nonceMode = nonceMode;

            // TODO SecurityParameters.fixed_iv_length
            int fixed_iv_length;

            switch (nonceMode)
            {
                case NONCE_RFC5288:
                    fixed_iv_length = 4;
                    this.record_iv_length = 8;
                    break;
                case NONCE_DRAFT_CHACHA20_POLY1305:
                    fixed_iv_length = 12;
                    this.record_iv_length = 0;
                    break;
                default:
                    throw new TlsFatalAlert(AlertDescription.internal_error);
            }

            this.context = context;
            this.macSize = macSize;

            int key_block_size = (2 * cipherKeySize) + (2 * fixed_iv_length);

            byte[] key_block = TlsUtilities.CalculateKeyBlock(context, key_block_size);

            int offset = 0;

            KeyParameter client_write_key = new KeyParameter(key_block, offset, cipherKeySize);
            offset += cipherKeySize;
            KeyParameter server_write_key = new KeyParameter(key_block, offset, cipherKeySize);
            offset += cipherKeySize;
            byte[] client_write_IV = Arrays.CopyOfRange(key_block, offset, offset + fixed_iv_length);
            offset += fixed_iv_length;
            byte[] server_write_IV = Arrays.CopyOfRange(key_block, offset, offset + fixed_iv_length);
            offset += fixed_iv_length;

            if (offset != key_block_size)
                throw new TlsFatalAlert(AlertDescription.internal_error);

            KeyParameter encryptKey, decryptKey;
            if (context.IsServer)
            {
                this.encryptCipher = serverWriteCipher;
                this.decryptCipher = clientWriteCipher;
                this.encryptImplicitNonce = server_write_IV;
                this.decryptImplicitNonce = client_write_IV;
                encryptKey = server_write_key;
                decryptKey = client_write_key;
            }
            else
            {
                this.encryptCipher = clientWriteCipher;
                this.decryptCipher = serverWriteCipher;
                this.encryptImplicitNonce = client_write_IV;
                this.decryptImplicitNonce = server_write_IV;
                encryptKey = client_write_key;
                decryptKey = server_write_key;
            }

            byte[] dummyNonce = new byte[fixed_iv_length + record_iv_length];

            this.encryptCipher.Init(true, new AeadParameters(encryptKey, 8 * macSize, dummyNonce));
            this.decryptCipher.Init(false, new AeadParameters(decryptKey, 8 * macSize, dummyNonce));
        }

        public virtual int GetPlaintextLimit(int ciphertextLimit)
        {
            // TODO We ought to be able to ask the decryptCipher (independently of it's current state!)
            return ciphertextLimit - macSize - record_iv_length;
        }

        /// <exception cref="IOException"></exception>
        public virtual byte[] EncodePlaintext(long seqNo, byte type, byte[] plaintext, int offset, int len)
        {
            byte[] nonce = new byte[encryptImplicitNonce.Length + record_iv_length];

            switch (nonceMode)
            {
                case NONCE_RFC5288:
                    Array.Copy(encryptImplicitNonce, 0, nonce, 0, encryptImplicitNonce.Length);
                    // RFC 5288/6655: The nonce_explicit MAY be the 64-bit sequence number.
                    TlsUtilities.WriteUint64(seqNo, nonce, encryptImplicitNonce.Length);
                    break;
                case NONCE_DRAFT_CHACHA20_POLY1305:
                    TlsUtilities.WriteUint64(seqNo, nonce, nonce.Length - 8);
                    for (int i = 0; i < encryptImplicitNonce.Length; ++i)
                    {
                        nonce[i] ^= encryptImplicitNonce[i];
                    }
                    break;
                default:
                    throw new TlsFatalAlert(AlertDescription.internal_error);
            }

            int plaintextOffset = offset;
            int plaintextLength = len;
            int ciphertextLength = encryptCipher.GetOutputSize(plaintextLength);

            byte[] output = new byte[record_iv_length + ciphertextLength];
            if (record_iv_length != 0)
            {
                Array.Copy(nonce, nonce.Length - record_iv_length, output, 0, record_iv_length);
            }
            int outputPos = record_iv_length;

            byte[] additionalData = GetAdditionalData(seqNo, type, plaintextLength);
            AeadParameters parameters = new AeadParameters(null, 8 * macSize, nonce, additionalData);

            try
            {
                encryptCipher.Init(true, parameters);
                outputPos += encryptCipher.ProcessBytes(plaintext, plaintextOffset, plaintextLength, output, outputPos);
                outputPos += encryptCipher.DoFinal(output, outputPos);
            }
            catch (Exception e)
            {
                throw new TlsFatalAlert(AlertDescription.internal_error, e);
            }

            if (outputPos != output.Length)
            {
                // NOTE: Existing AEAD cipher implementations all give exact output lengths
                throw new TlsFatalAlert(AlertDescription.internal_error);
            }

            return output;
        }

        /// <exception cref="IOException"></exception>
        public virtual byte[] DecodeCiphertext(long seqNo, byte type, byte[] ciphertext, int offset, int len)
        {
            if (GetPlaintextLimit(len) < 0)
                throw new TlsFatalAlert(AlertDescription.decode_error);

            byte[] nonce = new byte[decryptImplicitNonce.Length + record_iv_length];

            switch (nonceMode)
            {
                case NONCE_RFC5288:
                    Array.Copy(decryptImplicitNonce, 0, nonce, 0, decryptImplicitNonce.Length);
                    Array.Copy(ciphertext, offset, nonce, nonce.Length - record_iv_length, record_iv_length);
                    break;
                case NONCE_DRAFT_CHACHA20_POLY1305:
                    TlsUtilities.WriteUint64(seqNo, nonce, nonce.Length - 8);
                    for (int i = 0; i < decryptImplicitNonce.Length; ++i)
                    {
                        nonce[i] ^= decryptImplicitNonce[i];
                    }
                    break;
                default:
                    throw new TlsFatalAlert(AlertDescription.internal_error);
            }

            int ciphertextOffset = offset + record_iv_length;
            int ciphertextLength = len - record_iv_length;
            int plaintextLength = decryptCipher.GetOutputSize(ciphertextLength);

            byte[] output = new byte[plaintextLength];
            int outputPos = 0;

            byte[] additionalData = GetAdditionalData(seqNo, type, plaintextLength);
            AeadParameters parameters = new AeadParameters(null, 8 * macSize, nonce, additionalData);

            try
            {
                decryptCipher.Init(false, parameters);
                outputPos += decryptCipher.ProcessBytes(ciphertext, ciphertextOffset, ciphertextLength, output, outputPos);
                outputPos += decryptCipher.DoFinal(output, outputPos);
            }
            catch (Exception e)
            {
                throw new TlsFatalAlert(AlertDescription.bad_record_mac, e);
            }

            if (outputPos != output.Length)
            {
                // NOTE: Existing AEAD cipher implementations all give exact output lengths
                throw new TlsFatalAlert(AlertDescription.internal_error);
            }

            return output;
        }

        /// <exception cref="IOException"></exception>
        protected virtual byte[] GetAdditionalData(long seqNo, byte type, int len)
        {
            /*
             * additional_data = seq_num + TLSCompressed.type + TLSCompressed.version +
             * TLSCompressed.length
             */

            byte[] additional_data = new byte[13];
            TlsUtilities.WriteUint64(seqNo, additional_data, 0);
            TlsUtilities.WriteUint8(type, additional_data, 8);
            TlsUtilities.WriteVersion(context.ServerVersion, additional_data, 9);
            TlsUtilities.WriteUint16(len, additional_data, 11);

            return additional_data;
        }
    }
}
