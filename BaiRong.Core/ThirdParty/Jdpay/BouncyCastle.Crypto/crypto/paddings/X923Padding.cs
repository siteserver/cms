using System;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Paddings
{
    /**
    * A padder that adds X9.23 padding to a block - if a SecureRandom is
    * passed in random padding is assumed, otherwise padding with zeros is used.
    */
    public class X923Padding
		: IBlockCipherPadding
    {
        private SecureRandom random;

		/**
        * Initialise the padder.
        *
        * @param random a SecureRandom if one is available.
        */
        public void Init(
			SecureRandom random)
        {
            this.random = random;
        }

		/**
        * Return the name of the algorithm the cipher implements.
        *
        * @return the name of the algorithm the cipher implements.
        */
        public string PaddingName
        {
            get { return "X9.23"; }
        }

		/**
        * add the pad bytes to the passed in block, returning the
        * number of bytes added.
        */
        public int AddPadding(
            byte[]  input,
            int     inOff)
        {
            byte code = (byte)(input.Length - inOff);

            while (inOff < input.Length - 1)
            {
                if (random == null)
                {
                    input[inOff] = 0;
                }
                else
                {
                    input[inOff] = (byte)random.NextInt();
                }
                inOff++;
            }

            input[inOff] = code;

            return code;
        }

        /**
        * return the number of pad bytes present in the block.
        */
        public int PadCount(
			byte[] input)
        {
            int count = input[input.Length - 1] & 0xff;

            if (count > input.Length)
            {
                throw new InvalidCipherTextException("pad block corrupted");
            }

            return count;
        }
    }
}
