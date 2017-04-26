namespace BaiRong.Core
{
    public class Base64Encoder
    {
        // Fields
        private int blockCount;
        private int length;
        private int length2;
        private int paddingCount;
        private byte[] source;

        // Methods
        public Base64Encoder()
        {
        }

        public Base64Encoder(byte[] input)
        {
            source = input;
            length = input.Length;
            if ((length % 3) == 0)
            {
                paddingCount = 0;
                blockCount = length / 3;
            }
            else
            {
                paddingCount = 3 - (length % 3);
                blockCount = (length + paddingCount) / 3;
            }
            length2 = length + paddingCount;
        }

        public char[] GetEncoded()
        {
            int x;
            var source2 = new byte[length2];
            for (x = 0; x < length2; x++)
            {
                if (x < length)
                {
                    source2[x] = source[x];
                }
                else
                {
                    source2[x] = 0;
                }
            }
            var buffer = new byte[blockCount * 4];
            var result = new char[blockCount * 4];
            for (x = 0; x < blockCount; x++)
            {
                var b1 = source2[x * 3];
                var b2 = source2[(x * 3) + 1];
                var b3 = source2[(x * 3) + 2];
                var temp1 = (byte)((b1 & 0xfc) >> 2);
                var temp = (byte)((b1 & 3) << 4);
                var temp2 = (byte)((b2 & 240) >> 4);
                temp2 = (byte)(temp2 + temp);
                temp = (byte)((b2 & 15) << 2);
                var temp3 = (byte)((b3 & 0xc0) >> 6);
                temp3 = (byte)(temp3 + temp);
                var temp4 = (byte)(b3 & 0x3f);
                buffer[x * 4] = temp1;
                buffer[(x * 4) + 1] = temp2;
                buffer[(x * 4) + 2] = temp3;
                buffer[(x * 4) + 3] = temp4;
            }
            for (x = 0; x < (blockCount * 4); x++)
            {
                result[x] = sixbit2char(buffer[x]);
            }
            switch (paddingCount)
            {
                case 0:
                    return result;

                case 1:
                    result[(blockCount * 4) - 1] = '=';
                    return result;

                case 2:
                    result[(blockCount * 4) - 1] = '=';
                    result[(blockCount * 4) - 2] = '=';
                    return result;
            }
            return result;
        }

        private char sixbit2char(byte b)
        {
            var lookupTable = new char[] { 
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 
            'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 
            'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 
            'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+', '/'
         };
            if ((b >= 0) && (b <= 0x3f))
            {
                return lookupTable[b];
            }
            return ' ';
        }
    }

 

}
