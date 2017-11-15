
using System;
using System.Text;

namespace Org.BouncyCastle.Crypto
{
    public interface ISigner
    {
        /**
         * Return the name of the algorithm the signer implements.
         *
         * @return the name of the algorithm the signer implements.
         */
        string AlgorithmName { get; }

		/**
         * Initialise the signer for signing or verification.
         *
         * @param forSigning true if for signing, false otherwise
         * @param param necessary parameters.
         */
         void Init(bool forSigning, ICipherParameters parameters);

        /**
         * update the internal digest with the byte b
         */
        void Update(byte input);

        /**
         * update the internal digest with the byte array in
         */
        void BlockUpdate(byte[] input, int inOff, int length);

        /**
         * Generate a signature for the message we've been loaded with using
         * the key we were initialised with.
         */
        byte[] GenerateSignature();
        /**
         * return true if the internal state represents the signature described
         * in the passed in array.
         */
        bool VerifySignature(byte[] signature);

        /**
         * reset the internal state
         */
        void Reset();
    }
}
