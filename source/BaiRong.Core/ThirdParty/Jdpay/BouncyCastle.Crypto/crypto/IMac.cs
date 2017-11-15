using System;

namespace Org.BouncyCastle.Crypto
{
    /**
     * The base interface for implementations of message authentication codes (MACs).
     */
    public interface IMac
    {
        /**
         * Initialise the MAC.
         *
         * @param param the key and other data required by the MAC.
         * @exception ArgumentException if the parameters argument is
         * inappropriate.
         */
        void Init(ICipherParameters parameters);

        /**
         * Return the name of the algorithm the MAC implements.
         *
         * @return the name of the algorithm the MAC implements.
         */
        string AlgorithmName { get; }

		/**
		 * Return the block size for this MAC (in bytes).
		 *
		 * @return the block size for this MAC in bytes.
		 */
		int GetMacSize();

        /**
         * add a single byte to the mac for processing.
         *
         * @param in the byte to be processed.
         * @exception InvalidOperationException if the MAC is not initialised.
         */
        void Update(byte input);

		/**
         * @param in the array containing the input.
         * @param inOff the index in the array the data begins at.
         * @param len the length of the input starting at inOff.
         * @exception InvalidOperationException if the MAC is not initialised.
         * @exception DataLengthException if there isn't enough data in in.
         */
        void BlockUpdate(byte[] input, int inOff, int len);

		/**
         * Compute the final stage of the MAC writing the output to the out
         * parameter.
         * <p>
         * doFinal leaves the MAC in the same state it was after the last init.
         * </p>
         * @param out the array the MAC is to be output to.
         * @param outOff the offset into the out buffer the output is to start at.
         * @exception DataLengthException if there isn't enough space in out.
         * @exception InvalidOperationException if the MAC is not initialised.
         */
        int DoFinal(byte[] output, int outOff);

		/**
         * Reset the MAC. At the end of resetting the MAC should be in the
         * in the same state it was after the last init (if there was one).
         */
        void Reset();
    }
}
