using System;

using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1.Tsp
{
	public class MessageImprint
		: Asn1Encodable
	{
		private readonly AlgorithmIdentifier	hashAlgorithm;
		private readonly byte[]					hashedMessage;

		/**
		 * @param o
		 * @return a MessageImprint object.
		 */
		public static MessageImprint GetInstance(
			object o)
		{
			if (o == null || o is MessageImprint)
			{
				return (MessageImprint) o;
			}

			if (o is Asn1Sequence)
			{
				return new MessageImprint((Asn1Sequence) o);
			}

			throw new ArgumentException(
				"Unknown object in 'MessageImprint' factory: " + Platform.GetTypeName(o));
		}

		private MessageImprint(
			Asn1Sequence seq)
		{
			if (seq.Count != 2)
				throw new ArgumentException("Wrong number of elements in sequence", "seq");

			this.hashAlgorithm = AlgorithmIdentifier.GetInstance(seq[0]);
			this.hashedMessage = Asn1OctetString.GetInstance(seq[1]).GetOctets();
		}

		public MessageImprint(
			AlgorithmIdentifier	hashAlgorithm,
			byte[]				hashedMessage)
		{
			this.hashAlgorithm = hashAlgorithm;
			this.hashedMessage = hashedMessage;
		}

		public AlgorithmIdentifier HashAlgorithm
		{
			get { return hashAlgorithm; }
		}

		public byte[] GetHashedMessage()
		{
			return hashedMessage;
		}

		/**
		 * <pre>
		 *    MessageImprint ::= SEQUENCE  {
		 *       hashAlgorithm                AlgorithmIdentifier,
		 *       hashedMessage                OCTET STRING  }
		 * </pre>
		 */
		public override Asn1Object ToAsn1Object()
		{
			return new DerSequence(hashAlgorithm, new DerOctetString(hashedMessage));
		}
	}
}
