using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1.Tsp
{
	public class TstInfo
		: Asn1Encodable
	{
		private readonly DerInteger				version;
		private readonly DerObjectIdentifier	tsaPolicyId;
		private readonly MessageImprint			messageImprint;
		private readonly DerInteger				serialNumber;
		private readonly DerGeneralizedTime		genTime;
		private readonly Accuracy				accuracy;
		private readonly DerBoolean				ordering;
		private readonly DerInteger				nonce;
		private readonly GeneralName			tsa;
		private readonly X509Extensions			extensions;

		public static TstInfo GetInstance(
			object o)
		{
			if (o == null || o is TstInfo)
			{
				return (TstInfo) o;
			}

			if (o is Asn1Sequence)
			{
				return new TstInfo((Asn1Sequence) o);
			}

			if (o is Asn1OctetString)
			{
				try
				{
					byte[] octets = ((Asn1OctetString)o).GetOctets();
					return GetInstance(Asn1Object.FromByteArray(octets));
				}
				catch (IOException)
				{
					throw new ArgumentException(
						"Bad object format in 'TstInfo' factory.");
				}
			}

			throw new ArgumentException(
				"Unknown object in 'TstInfo' factory: " + Platform.GetTypeName(o));
		}

		private TstInfo(
			Asn1Sequence seq)
		{
			IEnumerator e = seq.GetEnumerator();

			// version
			e.MoveNext();
			version = DerInteger.GetInstance(e.Current);

			// tsaPolicy
			e.MoveNext();
			tsaPolicyId = DerObjectIdentifier.GetInstance(e.Current);

			// messageImprint
			e.MoveNext();
			messageImprint = MessageImprint.GetInstance(e.Current);

			// serialNumber
			e.MoveNext();
			serialNumber = DerInteger.GetInstance(e.Current);

			// genTime
			e.MoveNext();
			genTime = DerGeneralizedTime.GetInstance(e.Current);

			// default for ordering
			ordering = DerBoolean.False;

			while (e.MoveNext())
			{
				Asn1Object o = (Asn1Object) e.Current;

				if (o is Asn1TaggedObject)
				{
					DerTaggedObject tagged = (DerTaggedObject) o;

					switch (tagged.TagNo)
					{
						case 0:
							tsa = GeneralName.GetInstance(tagged, true);
							break;
						case 1:
							extensions = X509Extensions.GetInstance(tagged, false);
							break;
						default:
							throw new ArgumentException("Unknown tag value " + tagged.TagNo);
					}
				}

				if (o is DerSequence)
				{
					accuracy = Accuracy.GetInstance(o);
				}

				if (o is DerBoolean)
				{
					ordering = DerBoolean.GetInstance(o);
				}

				if (o is DerInteger)
				{
					nonce = DerInteger.GetInstance(o);
				}
			}
		}

		public TstInfo(
			DerObjectIdentifier	tsaPolicyId,
			MessageImprint		messageImprint,
			DerInteger			serialNumber,
			DerGeneralizedTime	genTime,
			Accuracy			accuracy,
			DerBoolean			ordering,
			DerInteger			nonce,
			GeneralName			tsa,
			X509Extensions		extensions)
		{
			this.version = new DerInteger(1);
			this.tsaPolicyId = tsaPolicyId;
			this.messageImprint = messageImprint;
			this.serialNumber = serialNumber;
			this.genTime = genTime;
			this.accuracy = accuracy;
			this.ordering = ordering;
			this.nonce = nonce;
			this.tsa = tsa;
			this.extensions = extensions;
		}

        public DerInteger Version
        {
            get { return version; }
        }

		public MessageImprint MessageImprint
		{
			get { return messageImprint; }
		}

		public DerObjectIdentifier Policy
		{
			get { return tsaPolicyId; }
		}

		public DerInteger SerialNumber
		{
			get { return serialNumber; }
		}

		public Accuracy Accuracy
		{
			get { return accuracy; }
		}

		public DerGeneralizedTime GenTime
		{
			get { return genTime; }
		}

		public DerBoolean Ordering
		{
			get { return ordering; }
		}

		public DerInteger Nonce
		{
			get { return nonce; }
		}

		public GeneralName Tsa
		{
			get { return tsa; }
		}

		public X509Extensions Extensions
		{
			get { return extensions; }
		}

		/**
		 * <pre>
		 *
		 *     TstInfo ::= SEQUENCE  {
		 *        version                      INTEGER  { v1(1) },
		 *        policy                       TSAPolicyId,
		 *        messageImprint               MessageImprint,
		 *          -- MUST have the same value as the similar field in
		 *          -- TimeStampReq
		 *        serialNumber                 INTEGER,
		 *         -- Time-Stamping users MUST be ready to accommodate integers
		 *         -- up to 160 bits.
		 *        genTime                      GeneralizedTime,
		 *        accuracy                     Accuracy                 OPTIONAL,
		 *        ordering                     BOOLEAN             DEFAULT FALSE,
		 *        nonce                        INTEGER                  OPTIONAL,
		 *          -- MUST be present if the similar field was present
		 *          -- in TimeStampReq.  In that case it MUST have the same value.
		 *        tsa                          [0] GeneralName          OPTIONAL,
		 *        extensions                   [1] IMPLICIT Extensions   OPTIONAL  }
		 *
		 * </pre>
		 */
		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector v = new Asn1EncodableVector(
				version, tsaPolicyId, messageImprint, serialNumber, genTime);

			if (accuracy != null)
			{
				v.Add(accuracy);
			}

			if (ordering != null && ordering.IsTrue)
			{
				v.Add(ordering);
			}

			if (nonce != null)
			{
				v.Add(nonce);
			}

			if (tsa != null)
			{
				v.Add(new DerTaggedObject(true, 0, tsa));
			}

			if (extensions != null)
			{
				v.Add(new DerTaggedObject(false, 1, extensions));
			}

			return new DerSequence(v);
		}
	}
}
