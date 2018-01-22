using System;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;


namespace Org.BouncyCastle.Crypto.Paddings
{

    /**
    * A padder that adds ISO10126-2 padding to a block.
    */
    public class ISO10126d2Padding: IBlockCipherPadding
    {
        private SecureRandom random;

        /**
        * Initialise the padder.
        *
        * @param random a SecureRandom if available.
        */
        public void Init(
			SecureRandom random)
            //throws ArgumentException
        {
			this.random = (random != null) ? random : new SecureRandom();
        }

		/**
        * Return the name of the algorithm the cipher implements.
        *
        * @return the name of the algorithm the cipher implements.
        */
        public string PaddingName
        {
            get { return "ISO10126-2"; }
        }

		/**
        * add the pad bytes to the passed in block, returning the
        * number of bytes added.
        */
        public int AddPadding(
            byte[]	input,
            int		inOff)
        {
            byte code = (byte)(input.Length - inOff);

            while (inOff < (input.Length - 1))
            {
                input[inOff] = (byte)random.NextInt();
                inOff++;
            }

            input[inOff] = code;

            return code;
        }

        /**
        * return the number of pad bytes present in the block.
        */
        public int PadCount(byte[] input)
            //throws InvalidCipherTextException
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
