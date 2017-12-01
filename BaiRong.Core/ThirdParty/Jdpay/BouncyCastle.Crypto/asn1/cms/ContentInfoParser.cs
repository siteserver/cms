using System;

namespace Org.BouncyCastle.Asn1.Cms
{
	/**
	* Produce an object suitable for an Asn1OutputStream.
	* <pre>
	* ContentInfo ::= SEQUENCE {
	*          contentType ContentType,
	*          content
	*          [0] EXPLICIT ANY DEFINED BY contentType OPTIONAL }
	* </pre>
	*/
	public class ContentInfoParser
	{
		private DerObjectIdentifier		contentType;
		private Asn1TaggedObjectParser	content;

		public ContentInfoParser(
			Asn1SequenceParser seq)
		{
			contentType = (DerObjectIdentifier)seq.ReadObject();
			content = (Asn1TaggedObjectParser)seq.ReadObject();
		}

		public DerObjectIdentifier ContentType
		{
			get { return contentType; }
		}

		public IAsn1Convertible GetContent(
			int tag)
		{
			if (content == null)
				return null;

			return content.GetObjectParser(tag, true);
		}
	}
}
