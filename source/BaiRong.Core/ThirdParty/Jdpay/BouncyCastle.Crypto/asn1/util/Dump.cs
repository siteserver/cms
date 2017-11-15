#if !PORTABLE
using System;
using System.IO;

using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1.Utilities
{
    public sealed class Dump
    {
        private Dump()
        {
        }

        public static void Main(string[] args)
        {
            FileStream fIn = File.OpenRead(args[0]);
            Asn1InputStream bIn = new Asn1InputStream(fIn);

			Asn1Object obj;
			while ((obj = bIn.ReadObject()) != null)
            {
                Console.WriteLine(Asn1Dump.DumpAsString(obj));
            }

            Platform.Dispose(bIn);
        }
    }
}
#endif
