using System;

namespace Org.BouncyCastle.Bcpg.OpenPgp
{
    /// <remarks>Padding functions.</remarks>
    public sealed class PgpPad
    {
        private PgpPad()
        {
        }

        public static byte[] PadSessionData(byte[] sessionInfo)
        {
            byte[] result = new byte[40];

            Array.Copy(sessionInfo, 0, result, 0, sessionInfo.Length);

            byte padValue = (byte)(result.Length - sessionInfo.Length);

            for (int i = sessionInfo.Length; i != result.Length; i++)
            {
                result[i] = padValue;
            }

            return result;
        }

        public static byte[] UnpadSessionData(byte[] encoded)
        {
            byte padValue = encoded[encoded.Length - 1];

            for (int i = encoded.Length - padValue; i != encoded.Length; i++)
            {
                if (encoded[i] != padValue)
                    throw new PgpException("bad padding found in session data");
            }

            byte[] taggedKey = new byte[encoded.Length - padValue];

            Array.Copy(encoded, 0, taggedKey, 0, taggedKey.Length);

            return taggedKey;
        }
    }
}
