using System;
using System.Collections;
using System.IO;
using System.Text;

using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Utilities.IO.Pem
{
	public class PemReader
	{
		private const string BeginString = "-----BEGIN ";
		private const string EndString = "-----END ";

		private readonly TextReader reader;

		public PemReader(TextReader reader)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");

			this.reader = reader;
		}

		public TextReader Reader
		{
			get { return reader; }
		}

		/// <returns>
		/// A <see cref="PemObject"/>
		/// </returns>
		/// <exception cref="IOException"></exception>
		public PemObject ReadPemObject()
		{
			string line = reader.ReadLine();

            if (line != null && Platform.StartsWith(line, BeginString))
			{
				line = line.Substring(BeginString.Length);
				int index = line.IndexOf('-');
				string type = line.Substring(0, index);

				if (index > 0)
					return LoadObject(type);
			}

			return null;
		}

		private PemObject LoadObject(string type)
		{
			string endMarker = EndString + type;
			IList headers = Platform.CreateArrayList();
			StringBuilder buf = new StringBuilder();

			string line;
			while ((line = reader.ReadLine()) != null
                && Platform.IndexOf(line, endMarker) == -1)
			{
				int colonPos = line.IndexOf(':');

				if (colonPos == -1)
				{
					buf.Append(line.Trim());
				}
				else
				{
					// Process field
					string fieldName = line.Substring(0, colonPos).Trim();

                    if (Platform.StartsWith(fieldName, "X-"))
                    {
                        fieldName = fieldName.Substring(2);
                    }

					string fieldValue = line.Substring(colonPos + 1).Trim();

					headers.Add(new PemHeader(fieldName, fieldValue));
				}
			}

			if (line == null)
			{
				throw new IOException(endMarker + " not found");
			}

			if (buf.Length % 4 != 0)
			{
				throw new IOException("base64 data appears to be truncated");
			}

			return new PemObject(type, headers, Base64.Decode(buf.ToString()));
		}
	}
}
