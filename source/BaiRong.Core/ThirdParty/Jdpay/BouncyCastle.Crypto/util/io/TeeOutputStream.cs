using System;
using System.Diagnostics;
using System.IO;

namespace Org.BouncyCastle.Utilities.IO
{
    public class TeeOutputStream
		: BaseOutputStream
	{
		private readonly Stream output, tee;

		public TeeOutputStream(Stream output, Stream tee)
		{
			Debug.Assert(output.CanWrite);
			Debug.Assert(tee.CanWrite);

			this.output = output;
			this.tee = tee;
		}

#if PORTABLE
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Platform.Dispose(output);
                Platform.Dispose(tee);
            }
            base.Dispose(disposing);
        }
#else
        public override void Close()
		{
            Platform.Dispose(output);
            Platform.Dispose(tee);
            base.Close();
		}
#endif

        public override void Write(byte[] buffer, int offset, int count)
		{
			output.Write(buffer, offset, count);
			tee.Write(buffer, offset, count);
		}

		public override void WriteByte(byte b)
		{
			output.WriteByte(b);
			tee.WriteByte(b);
		}
	}
}
