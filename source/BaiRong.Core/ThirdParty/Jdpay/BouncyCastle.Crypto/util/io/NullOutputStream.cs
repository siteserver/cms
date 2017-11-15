using System;

namespace Org.BouncyCastle.Utilities.IO
{
	internal class NullOutputStream
		: BaseOutputStream
	{
		public override void WriteByte(byte b)
		{
			// do nothing
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			// do nothing
		}
	}
}
