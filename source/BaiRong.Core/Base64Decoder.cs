namespace BaiRong.Core
{
    public class Base64Decoder
    {
        // Fields
        private int blockCount;
        public static Base64Decoder Decoder = new Base64Decoder();
        private int length;
        private int length2;
        private int length3;
        private int paddingCount;
        private char[] source;

        // Methods
        private byte char2sixbit(char c)
        {
            var lookupTable = new char[] { 
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 
            'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 
            'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 
            'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+', '/'
         };
            if (c != '=')
            {
                for (var x = 0; x < 0x40; x++)
                {
                    if (lookupTable[x] == c)
                    {
                        return (byte)x;
                    }
                }
            }
            return 0;
        }

        public byte[] GetDecoded(string strInput)
        {
            int x;
            init(strInput.ToCharArray());
            var buffer = new byte[length];
            var buffer2 = new byte[length2];
            for (x = 0; x < length; x++)
            {
                buffer[x] = char2sixbit(source[x]);
            }
            for (x = 0; x < blockCount; x++)
            {
                var temp1 = buffer[x * 4];
                var temp2 = buffer[(x * 4) + 1];
                var temp3 = buffer[(x * 4) + 2];
                var temp4 = buffer[(x * 4) + 3];
                var b = (byte)(temp1 << 2);
                var b1 = (byte)((temp2 & 0x30) >> 4);
                b1 = (byte)(b1 + b);
                b = (byte)((temp2 & 15) << 4);
                var b2 = (byte)((temp3 & 60) >> 2);
                b2 = (byte)(b2 + b);
                b = (byte)((temp3 & 3) << 6);
                var b3 = temp4;
                b3 = (byte)(b3 + b);
                buffer2[x * 3] = b1;
                buffer2[(x * 3) + 1] = b2;
                buffer2[(x * 3) + 2] = b3;
            }
            length3 = length2 - paddingCount;
            var result = new byte[length3];
            for (x = 0; x < length3; x++)
            {
                result[x] = buffer2[x];
            }
            return result;
        }

        private void init(char[] input)
        {
            var temp = 0;
            source = input;
            length = input.Length;
            for (var x = 0; x < 2; x++)
            {
                if (input[(length - x) - 1] == '=')
                {
                    temp++;
                }
            }
            paddingCount = temp;
            blockCount = length / 4;
            length2 = blockCount * 3;
        }
    }


}
