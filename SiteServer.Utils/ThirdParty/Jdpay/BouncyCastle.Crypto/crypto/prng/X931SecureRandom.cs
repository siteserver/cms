using System;

using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Prng
{
    public class X931SecureRandom
        :   SecureRandom
    {
        private readonly bool           mPredictionResistant;
        private readonly SecureRandom   mRandomSource;
        private readonly X931Rng        mDrbg;

        internal X931SecureRandom(SecureRandom randomSource, X931Rng drbg, bool predictionResistant)
            : base((IRandomGenerator)null)
        {
            this.mRandomSource = randomSource;
            this.mDrbg = drbg;
            this.mPredictionResistant = predictionResistant;
        }

        public override void SetSeed(byte[] seed)
        {
            lock (this)
            {
                if (mRandomSource != null)
                {
                    this.mRandomSource.SetSeed(seed);
                }
            }
        }

        public override void SetSeed(long seed)
        {
            lock (this)
            {
                // this will happen when SecureRandom() is created
                if (mRandomSource != null)
                {
                    this.mRandomSource.SetSeed(seed);
                }
            }
        }

        public override void NextBytes(byte[] bytes)
        {
            lock (this)
            {
                // check if a reseed is required...
                if (mDrbg.Generate(bytes, mPredictionResistant) < 0)
                {
                    mDrbg.Reseed();
                    mDrbg.Generate(bytes, mPredictionResistant);
                }
            }
        }

        public override void NextBytes(byte[] buf, int off, int len)
        {
            byte[] bytes = new byte[len];
            NextBytes(bytes);
            Array.Copy(bytes, 0, buf, off, len);
        }

        public override byte[] GenerateSeed(int numBytes)
        {
            return EntropyUtilities.GenerateSeed(mDrbg.EntropySource, numBytes);
        }
    }
}
