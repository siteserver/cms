namespace RemotingProtocolParser.TCP
{
    //MS .Net Remoting Core Protocol
    //http://msdn.microsoft.com/en-us/library/cc237297(v=prot.20).aspx

    public class TcpHeaders
    {
        public const ushort EndOfHeaders = 0;
        public const ushort Custom = 1;
        public const ushort StatusCode = 2;
        public const ushort StatusPhrase = 3;
        public const ushort RequestUri = 4;
        public const ushort CloseConnection = 5;
        public const ushort ContentType = 6;
    }
    
    public class TcpHeaderFormat
    {
        public const byte Void = 0;
        public const byte CountedString = 1;
        public const byte Byte = 2;
        public const byte UInt16 = 3;
        public const byte Int32 = 4;
    }

    public class TcpStringFormat
    {
        public const byte Unicode = 0;
        public const byte UTF8 = 1;
    }
    
    public class TcpOperations
    {
        public const ushort Request = 0;
        public const ushort OneWayRequest = 1;
        public const ushort Reply = 2;
    }
    
    public class TcpContentDelimiter
    {
        public const ushort ContentLength = 0;
        public const ushort Chunked = 1;
    }
}
