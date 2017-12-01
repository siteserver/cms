using System;
using System.IO;

using Org.BouncyCastle.Utilities.IO;

namespace Org.BouncyCastle.Asn1
{
	public class BerOctetStringGenerator
		: BerGenerator
	{
		public BerOctetStringGenerator(Stream outStream)
			: base(outStream)
		{
			WriteBerHeader(Asn1Tags.Constructed | Asn1Tags.OctetString);
		}

		public BerOctetStringGenerator(
			Stream	outStream,
			int		tagNo,
			bool	isExplicit)
			: base(outStream, tagNo, isExplicit)
		{
			WriteBerHeader(Asn1Tags.Constructed | Asn1Tags.OctetString);
		}

		public Stream GetOctetOutputStream()
		{
			return GetOctetOutputStream(new byte[1000]); // limit for CER encoding.
		}

		public Stream GetOctetOutputStream(
			int bufSize)
		{
			return bufSize < 1
				?	GetOctetOutputStream()
				:	GetOctetOutputStream(new byte[bufSize]);
		}

		public Stream GetOctetOutputStream(
			byte[] buf)
		{
			return new BufferedBerOctetStream(this, buf);
		}

		private class BufferedBerOctetStream
			: BaseOutputStream
		{
			private byte[] _buf;
			private int    _off;
			private readonly BerOctetStringGenerator _gen;
			private readonly DerOutputStream _derOut;

			internal BufferedBerOctetStream(
				BerOctetStringGenerator	gen,
				byte[]					buf)
			{
				_gen = gen;
				_buf = buf;
				_off = 0;
				_derOut = new DerOutputStream(_gen.Out);
			}

			public override void WriteByte(
				byte b)
			{
				_buf[_off++] = b;

				if (_off == _buf.Length)
				{
					DerOctetString.Encode(_derOut, _buf, 0, _off);
					_off = 0;
				}
			}

			public override void Write(
				byte[] buf,
				int    offset,
				int    len)
			{
				while (len > 0)
				{
					int numToCopy = System.Math.Min(len, _buf.Length - _off);

					if (numToCopy == _buf.Length)
					{
						DerOctetString.Encode(_derOut, buf, offset, numToCopy);
					}
					else
					{
						Array.Copy(buf, offset, _buf, _off, numToCopy);

						_off += numToCopy;
						if (_off < _buf.Length)
							break;

						DerOctetString.Encode(_derOut, _buf, 0, _off);
						_off = 0;
					}

					offset += numToCopy;
					len -= numToCopy;
				}
			}

#if PORTABLE
            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
				    if (_off != 0)
				    {
					    DerOctetString.Encode(_derOut, _buf, 0, _off);
				    }

				    _gen.WriteBerEnd();
                }
                base.Dispose(disposing);
            }
#else
            public override void Close()
			{
				if (_off != 0)
				{
					DerOctetString.Encode(_derOut, _buf, 0, _off);
				}

				_gen.WriteBerEnd();
				base.Close();
			}
#endif
		}
	}
}
