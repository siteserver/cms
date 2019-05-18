namespace SiteServer.Utils
{
    public class Base64Encoder
    {
        // Fields
        private readonly int _blockCount;
        private readonly int _length;
        private readonly int _length2;
        private readonly int _paddingCount;
        private readonly byte[] _source;

        // Methods
        public Base64Encoder()
        {
        }

        public Base64Encoder(byte[] input)
        {
            _source = input;
            _length = input.Length;
            if (_length % 3 == 0)
            {
                _paddingCount = 0;
                _blockCount = _length / 3;
            }
            else
            {
                _paddingCount = 3 - _length % 3;
                _blockCount = (_length + _paddingCount) / 3;
            }
            _length2 = _length + _paddingCount;
        }

        public char[] GetEncoded()
        {
            int x;
            var source2 = new byte[_length2];
            for (x = 0; x < _length2; x++)
            {
                if (x < _length)
                {
                    source2[x] = _source[x];
                }
                else
                {
                    source2[x] = 0;
                }
            }
            var buffer = new byte[_blockCount * 4];
            var result = new char[_blockCount * 4];
            for (x = 0; x < _blockCount; x++)
            {
                var b1 = source2[x * 3];
                var b2 = source2[x * 3 + 1];
                var b3 = source2[x * 3 + 2];
                var temp1 = (byte)((b1 & 0xfc) >> 2);
                var temp = (byte)((b1 & 3) << 4);
                var temp2 = (byte)((b2 & 240) >> 4);
                temp2 = (byte)(temp2 + temp);
                temp = (byte)((b2 & 15) << 2);
                var temp3 = (byte)((b3 & 0xc0) >> 6);
                temp3 = (byte)(temp3 + temp);
                var temp4 = (byte)(b3 & 0x3f);
                buffer[x * 4] = temp1;
                buffer[x * 4 + 1] = temp2;
                buffer[x * 4 + 2] = temp3;
                buffer[x * 4 + 3] = temp4;
            }
            for (x = 0; x < _blockCount * 4; x++)
            {
                result[x] = Sixbit2Char(buffer[x]);
            }
            switch (_paddingCount)
            {
                case 0:
                    return result;

                case 1:
                    result[_blockCount * 4 - 1] = '=';
                    return result;

                case 2:
                    result[_blockCount * 4 - 1] = '=';
                    result[_blockCount * 4 - 2] = '=';
                    return result;
            }
            return result;
        }

        private static char Sixbit2Char(byte b)
        {
            var lookupTable = new[] { 
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 
            'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 
            'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 
            'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+', '/'
         };
            if (b <= 0x3f)
            {
                return lookupTable[b];
            }
            return ' ';
        }
    }

 

}
