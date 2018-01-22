using System;

using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1.Tsp
{
	public class Accuracy
		: Asn1Encodable
	{
		private readonly DerInteger seconds;
		private readonly DerInteger millis;
		private readonly DerInteger micros;

		// constants
		protected const int MinMillis = 1;
		protected const int MaxMillis = 999;
		protected const int MinMicros = 1;
		protected const int MaxMicros = 999;

		public Accuracy(
			DerInteger seconds,
			DerInteger millis,
			DerInteger micros)
		{
			//Verifications
			if (millis != null
				&& (millis.Value.IntValue < MinMillis
					|| millis.Value.IntValue > MaxMillis))
			{
				throw new ArgumentException(
					"Invalid millis field : not in (1..999)");
			}

			if (micros != null
				&& (micros.Value.IntValue < MinMicros
					|| micros.Value.IntValue > MaxMicros))
			{
				throw new ArgumentException(
					"Invalid micros field : not in (1..999)");
			}

			this.seconds = seconds;
			this.millis = millis;
			this.micros = micros;
		}

		private Accuracy(
			Asn1Sequence seq)
		{
			for (int i = 0; i < seq.Count; ++i)
			{
				// seconds
				if (seq[i] is DerInteger)
				{
					seconds = (DerInteger) seq[i];
				}
				else if (seq[i] is DerTaggedObject)
				{
					DerTaggedObject extra = (DerTaggedObject) seq[i];

					switch (extra.TagNo)
					{
						case 0:
							millis = DerInteger.GetInstance(extra, false);
							if (millis.Value.IntValue < MinMillis
								|| millis.Value.IntValue > MaxMillis)
							{
								throw new ArgumentException(
									"Invalid millis field : not in (1..999).");
							}
							break;
						case 1:
							micros = DerInteger.GetInstance(extra, false);
							if (micros.Value.IntValue < MinMicros
								|| micros.Value.IntValue > MaxMicros)
							{
								throw new ArgumentException(
									"Invalid micros field : not in (1..999).");
							}
							break;
						default:
							throw new ArgumentException("Invalig tag number");
					}
				}
			}
		}

		public static Accuracy GetInstance(
			object o)
		{
			if (o == null || o is Accuracy)
			{
				return (Accuracy) o;
			}

			if (o is Asn1Sequence)
			{
				return new Accuracy((Asn1Sequence) o);
			}

			throw new ArgumentException(
				"Unknown object in 'Accuracy' factory: " + Platform.GetTypeName(o));
		}

		public DerInteger Seconds
		{
			get { return seconds; }
		}

		public DerInteger Millis
		{
			get { return millis; }
		}

		public DerInteger Micros
		{
			get { return micros; }
		}

		/**
		 * <pre>
		 * Accuracy ::= SEQUENCE {
		 *             seconds        INTEGER              OPTIONAL,
		 *             millis     [0] INTEGER  (1..999)    OPTIONAL,
		 *             micros     [1] INTEGER  (1..999)    OPTIONAL
		 *             }
		 * </pre>
		 */
		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector v = new Asn1EncodableVector();

			if (seconds != null)
			{
				v.Add(seconds);
			}

			if (millis != null)
			{
				v.Add(new DerTaggedObject(false, 0, millis));
			}

			if (micros != null)
			{
				v.Add(new DerTaggedObject(false, 1, micros));
			}

			return new DerSequence(v);
		}
	}
}
