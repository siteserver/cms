using System;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Paddings
{

    /// <summary> A padder that adds Null byte padding to a block.</summary>
    public class ZeroBytePadding : IBlockCipherPadding
    {
        /// <summary> Return the name of the algorithm the cipher implements.
        ///
        /// </summary>
        /// <returns> the name of the algorithm the cipher implements.
        /// </returns>
        public string PaddingName
        {
            get { return "ZeroBytePadding"; }
        }

		/// <summary> Initialise the padder.
        ///
        /// </summary>
        /// <param name="random">- a SecureRandom if available.
        /// </param>
        public void Init(SecureRandom random)
        {
            // nothing to do.
        }

        /// <summary> add the pad bytes to the passed in block, returning the
        /// number of bytes added.
        /// </summary>
        public int AddPadding(
			byte[]	input,
			int		inOff)
        {
            int added = (input.Length - inOff);

            while (inOff < input.Length)
            {
                input[inOff] = (byte) 0;
                inOff++;
            }

            return added;
        }

		/// <summary> return the number of pad bytes present in the block.</summary>
        public int PadCount(
			byte[] input)
        {
            int count = input.Length;

            while (count > 0)
            {
                if (input[count - 1] != 0)
                {
                    break;
                }

                count--;
            }

            return input.Length - count;
        }
    }
}
