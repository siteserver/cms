using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Pingpp.Utils
{
    public class RsaUtils
    {
        public static string RsaSign(string data, byte[] privateKey)
        {
            if (privateKey == null)
            {
                return "";
            }
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);

            var rsa = DecodeRsaPrivateKey(privateKey);
            return Convert.ToBase64String(rsa.SignData(dataBytes, "SHA256"));
        }

        private static RSACryptoServiceProvider DecodeRsaPrivateKey(byte[] privkey)
        {
            var mem = new MemoryStream(privkey);
            var binr = new BinaryReader(mem);
            try
            {
                var twobytes = binr.ReadUInt16();
                switch (twobytes)
                {
                    case 0x8130:
                        binr.ReadByte();
                        break;
                    case 0x8230:
                        binr.ReadInt16();
                        break;
                    default:
                        return null;
                }

                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102)
                    return null;
                var bt = binr.ReadByte();
                if (bt != 0x00)
                    return null;

                var elems = GetIntegerSize(binr);
                var modulus = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                var e = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                var d = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                var p = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                var q = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                var dp = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                var dq = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                var iq = binr.ReadBytes(elems);

                var rsa = new RSACryptoServiceProvider();
                var rsaParam = new RSAParameters
                {
                    Modulus = modulus,
                    Exponent = e,
                    D = d,
                    P = p,
                    Q = q,
                    DP = dp,
                    DQ = dq,
                    InverseQ = iq
                };
                rsa.ImportParameters(rsaParam);
                return rsa;
            }
            finally
            {
                binr.Close();
            }
        }

        private static int GetIntegerSize(BinaryReader binr)
        {
            int count;
            var bt = binr.ReadByte();
            if (bt != 0x02)
                return 0;
            bt = binr.ReadByte();

            switch (bt)
            {
                case 0x81:
                    count = binr.ReadByte();
                    break;
                case 0x82:
                    var highbyte = binr.ReadByte();
                    var lowbyte = binr.ReadByte();
                    byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                    count = BitConverter.ToInt32(modint, 0);
                    break;
                default:
                    count = bt;
                    break;
            }
            while (binr.ReadByte() == 0x00)
            {
                count -= 1;
            }
            binr.BaseStream.Seek(-1, SeekOrigin.Current);

            return count;
        }
    }
}
