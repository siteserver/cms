using System;
using System.Collections;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Macs
{
    /// <summary>
    /// The GMAC specialisation of Galois/Counter mode (GCM) detailed in NIST Special Publication
    /// 800-38D.
    /// </summary>
    /// <remarks>
    /// GMac is an invocation of the GCM mode where no data is encrypted (i.e. all input data to the Mac
    /// is processed as additional authenticated data with the underlying GCM block cipher).
    /// </remarks>
    public class GMac 
        : IMac
    {
        private readonly GcmBlockCipher cipher;
        private readonly int macSizeBits;

        /// <summary>
        /// Creates a GMAC based on the operation of a block cipher in GCM mode.
        /// </summary>
        /// <remarks>
        /// This will produce an authentication code the length of the block size of the cipher.
        /// </remarks>
        /// <param name="cipher">the cipher to be used in GCM mode to generate the MAC.</param>
        public GMac(GcmBlockCipher cipher)
            : this(cipher, 128)
        {
        }

        /// <summary>
        /// Creates a GMAC based on the operation of a 128 bit block cipher in GCM mode.
        /// </summary>
        /// <remarks>
        /// This will produce an authentication code the length of the block size of the cipher.
        /// </remarks>
        /// <param name="cipher">the cipher to be used in GCM mode to generate the MAC.</param>
        /// <param name="macSizeBits">the mac size to generate, in bits. Must be a multiple of 8, between 32 and 128 (inclusive).
        /// Sizes less than 96 are not recommended, but are supported for specialized applications.</param>
        public GMac(GcmBlockCipher cipher, int macSizeBits)
        {
            this.cipher = cipher;
            this.macSizeBits = macSizeBits;
        }

        /// <summary>
        /// Initialises the GMAC - requires a <see cref="Org.BouncyCastle.Crypto.Parameters.ParametersWithIV"/> 
        /// providing a <see cref="Org.BouncyCastle.Crypto.Parameters.KeyParameter"/> and a nonce.
        /// </summary>
        public void Init(ICipherParameters parameters)
        {
            if (parameters is ParametersWithIV)
            {
                ParametersWithIV param = (ParametersWithIV)parameters;

                byte[] iv = param.GetIV();
                KeyParameter keyParam = (KeyParameter)param.Parameters;

                // GCM is always operated in encrypt mode to calculate MAC
                cipher.Init(true, new AeadParameters(keyParam, macSizeBits, iv));
            }
            else
            {
                throw new ArgumentException("GMAC requires ParametersWithIV");
            }
        }

        public string AlgorithmName
        {
            get { return cipher.GetUnderlyingCipher().AlgorithmName + "-GMAC"; }
        }

        public int GetMacSize()
        {
            return macSizeBits / 8;
        }

        public void Update(byte input) 
        {
            cipher.ProcessAadByte(input);
        }

        public void BlockUpdate(byte[] input, int inOff, int len)
        {
            cipher.ProcessAadBytes(input, inOff, len);
        }

        public int DoFinal(byte[] output, int outOff)
        {
            try
            {
                return cipher.DoFinal(output, outOff);
            }
            catch (InvalidCipherTextException e)
            {
                // Impossible in encrypt mode
                throw new InvalidOperationException(e.ToString());
            }
        }

        public void Reset()
        {
            cipher.Reset();
        }
    }
}
