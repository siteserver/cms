using System;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Paddings
{

    /// <summary> A padder that adds Trailing-Bit-Compliment padding to a block.
    /// <p>
    /// This padding pads the block out compliment of the last bit
    /// of the plain text.
    /// </p>
    /// </summary>
    public class TbcPadding
		: IBlockCipherPadding
    {
        /// <summary> Return the name of the algorithm the cipher implements.</summary>
        /// <returns> the name of the algorithm the cipher implements.
        /// </returns>
        public string PaddingName
        {
            get { return "TBC"; }
        }

		/// <summary> Initialise the padder.</summary>
        /// <param name="random">- a SecureRandom if available.
        /// </param>
        public virtual void Init(SecureRandom random)
        {
            // nothing to do.
        }

        /// <summary> add the pad bytes to the passed in block, returning the
        /// number of bytes added.
        /// <p>
        /// Note: this assumes that the last block of plain text is always
        /// passed to it inside in. i.e. if inOff is zero, indicating the
        /// entire block is to be overwritten with padding the value of in
        /// should be the same as the last block of plain text.
        /// </p>
        /// </summary>
        public virtual int AddPadding(byte[] input, int inOff)
        {
            int count = input.Length - inOff;
            byte code;

            if (inOff > 0)
            {
                code = (byte)((input[inOff - 1] & 0x01) == 0?0xff:0x00);
            }
            else
            {
                code = (byte)((input[input.Length - 1] & 0x01) == 0?0xff:0x00);
            }

            while (inOff < input.Length)
            {
                input[inOff] = code;
                inOff++;
            }

            return count;
        }

        /// <summary> return the number of pad bytes present in the block.</summary>
        public virtual int PadCount(byte[] input)
        {
            byte code = input[input.Length - 1];

            int index = input.Length - 1;
            while (index > 0 && input[index - 1] == code)
            {
                index--;
            }

            return input.Length - index;
        }
    }
}
