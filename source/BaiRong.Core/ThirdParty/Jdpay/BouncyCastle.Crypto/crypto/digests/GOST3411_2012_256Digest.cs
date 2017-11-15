using System;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Digests
{
    public class GOST3411_2012_256Digest : GOST3411_2012Digest
    {
        private readonly static byte[] IV = {
            0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01,
            0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01,
            0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01,
            0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01,
            0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01,
            0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01,
            0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01,
            0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01
        };

        public override string AlgorithmName
        {
            get { return "GOST3411-2012-256"; }
        }

        public GOST3411_2012_256Digest() : base(IV)
        {

        }

        public GOST3411_2012_256Digest(GOST3411_2012_256Digest other) : base(IV)
        {
            Reset(other);
        }

        public override int GetDigestSize()
        {
            return 32;
        }

        public override int DoFinal(byte[] output, int outOff)
        {
			byte[] result = new byte[64];
			base.DoFinal(result, 0);

			Array.Copy(result, 32, output, outOff, 32);

			return 32;
        }

        public override IMemoable Copy()
        {
			return new GOST3411_2012_256Digest(this);
        }
    }
}
