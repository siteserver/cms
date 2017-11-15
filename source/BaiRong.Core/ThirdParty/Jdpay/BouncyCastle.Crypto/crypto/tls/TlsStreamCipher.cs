using System;
using System.IO;

using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Tls;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls
{
    public class TlsStreamCipher
        :   TlsCipher
    {
        protected readonly TlsContext context;

        protected readonly IStreamCipher encryptCipher;
        protected readonly IStreamCipher decryptCipher;

        protected readonly TlsMac writeMac;
        protected readonly TlsMac readMac;

        protected readonly bool usesNonce;

        /// <exception cref="IOException"></exception>
        public TlsStreamCipher(TlsContext context, IStreamCipher clientWriteCipher,
            IStreamCipher serverWriteCipher, IDigest clientWriteDigest, IDigest serverWriteDigest,
            int cipherKeySize, bool usesNonce)
        {
            bool isServer = context.IsServer;

            this.context = context;
            this.usesNonce = usesNonce;

            this.encryptCipher = clientWriteCipher;
            this.decryptCipher = serverWriteCipher;

            int key_block_size = (2 * cipherKeySize) + clientWriteDigest.GetDigestSize()
                + serverWriteDigest.GetDigestSize();

            byte[] key_block = TlsUtilities.CalculateKeyBlock(context, key_block_size);

            int offset = 0;

            // Init MACs
            TlsMac clientWriteMac = new TlsMac(context, clientWriteDigest, key_block, offset,
                clientWriteDigest.GetDigestSize());
            offset += clientWriteDigest.GetDigestSize();
            TlsMac serverWriteMac = new TlsMac(context, serverWriteDigest, key_block, offset,
                serverWriteDigest.GetDigestSize());
            offset += serverWriteDigest.GetDigestSize();

            // Build keys
            KeyParameter clientWriteKey = new KeyParameter(key_block, offset, cipherKeySize);
            offset += cipherKeySize;
            KeyParameter serverWriteKey = new KeyParameter(key_block, offset, cipherKeySize);
            offset += cipherKeySize;

            if (offset != key_block_size)
            {
                throw new TlsFatalAlert(AlertDescription.internal_error);
            }

            ICipherParameters encryptParams, decryptParams;
            if (isServer)
            {
                this.writeMac = serverWriteMac;
                this.readMac = clientWriteMac;
                this.encryptCipher = serverWriteCipher;
                this.decryptCipher = clientWriteCipher;
                encryptParams = serverWriteKey;
                decryptParams = clientWriteKey;
            }
            else
            {
                this.writeMac = clientWriteMac;
                this.readMac = serverWriteMac;
                this.encryptCipher = clientWriteCipher;
                this.decryptCipher = serverWriteCipher;
                encryptParams = clientWriteKey;
                decryptParams = serverWriteKey;
            }

            if (usesNonce)
            {
                byte[] dummyNonce = new byte[8];
                encryptParams = new ParametersWithIV(encryptParams, dummyNonce);
                decryptParams = new ParametersWithIV(decryptParams, dummyNonce);
            }

            this.encryptCipher.Init(true, encryptParams);
            this.decryptCipher.Init(false, decryptParams);
        }

        public virtual int GetPlaintextLimit(int ciphertextLimit)
        {
            return ciphertextLimit - writeMac.Size;
        }

        public virtual byte[] EncodePlaintext(long seqNo, byte type, byte[] plaintext, int offset, int len)
        {
            if (usesNonce)
            {
                UpdateIV(encryptCipher, true, seqNo);
            }

            byte[] outBuf = new byte[len + writeMac.Size];

            encryptCipher.ProcessBytes(plaintext, offset, len, outBuf, 0);

            byte[] mac = writeMac.CalculateMac(seqNo, type, plaintext, offset, len);
            encryptCipher.ProcessBytes(mac, 0, mac.Length, outBuf, len);

            return outBuf;
        }

        /// <exception cref="IOException"></exception>
        public virtual byte[] DecodeCiphertext(long seqNo, byte type, byte[] ciphertext, int offset, int len)
        {
            if (usesNonce)
            {
                UpdateIV(decryptCipher, false, seqNo);
            }

            int macSize = readMac.Size;
            if (len < macSize)
                throw new TlsFatalAlert(AlertDescription.decode_error);

            int plaintextLength = len - macSize;

            byte[] deciphered = new byte[len];
            decryptCipher.ProcessBytes(ciphertext, offset, len, deciphered, 0);
            CheckMac(seqNo, type, deciphered, plaintextLength, len, deciphered, 0, plaintextLength);
            return Arrays.CopyOfRange(deciphered, 0, plaintextLength);
        }

        /// <exception cref="IOException"></exception>
        protected virtual void CheckMac(long seqNo, byte type, byte[] recBuf, int recStart, int recEnd, byte[] calcBuf, int calcOff, int calcLen)
        {
            byte[] receivedMac = Arrays.CopyOfRange(recBuf, recStart, recEnd);
            byte[] computedMac = readMac.CalculateMac(seqNo, type, calcBuf, calcOff, calcLen);

            if (!Arrays.ConstantTimeAreEqual(receivedMac, computedMac))
                throw new TlsFatalAlert(AlertDescription.bad_record_mac);
        }

        protected virtual void UpdateIV(IStreamCipher cipher, bool forEncryption, long seqNo)
        {
            byte[] nonce = new byte[8];
            TlsUtilities.WriteUint64(seqNo, nonce, 0);
            cipher.Init(forEncryption, new ParametersWithIV(null, nonce));
        }
    }
}
