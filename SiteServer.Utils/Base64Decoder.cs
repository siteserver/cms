namespace SiteServer.Utils
{
    public class Base64Decoder
    {
        // Fields
        private int _blockCount;
        public static Base64Decoder Decoder = new Base64Decoder();
        private int _length;
        private int _length2;
        private int _length3;
        private int _paddingCount;
        private char[] _source;

        // Methods
        private static byte Char2Sixbit(char c)
        {
            var lookupTable = new[]
            {
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P',
                'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f',
                'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v',
                'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+', '/'
            };
            if (c == '=') return 0;

            for (var x = 0; x < 0x40; x++)
            {
                if (lookupTable[x] == c)
                {
                    return (byte) x;
                }
            }
            return 0;
        }

        public byte[] GetDecoded(string strInput)
        {
            int x;
            Init(strInput.ToCharArray());
            var buffer = new byte[_length];
            var buffer2 = new byte[_length2];
            for (x = 0; x < _length; x++)
            {
                buffer[x] = Char2Sixbit(_source[x]);
            }
            for (x = 0; x < _blockCount; x++)
            {
                var temp1 = buffer[x * 4];
                var temp2 = buffer[x * 4 + 1];
                var temp3 = buffer[x * 4 + 2];
                var temp4 = buffer[x * 4 + 3];
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
                buffer2[x * 3 + 1] = b2;
                buffer2[x * 3 + 2] = b3;
            }
            _length3 = _length2 - _paddingCount;
            var result = new byte[_length3];
            for (x = 0; x < _length3; x++)
            {
                result[x] = buffer2[x];
            }
            return result;
        }

        private void Init(char[] input)
        {
            var temp = 0;
            _source = input;
            _length = input.Length;
            for (var x = 0; x < 2; x++)
            {
                if (input[_length - x - 1] == '=')
                {
                    temp++;
                }
            }
            _paddingCount = temp;
            _blockCount = _length / 4;
            _length2 = _blockCount * 3;
        }
    }


}
