using System;
using System.Collections;

using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1.X509
{
	public class NameConstraints
		: Asn1Encodable
	{
		private Asn1Sequence permitted, excluded;

		public static NameConstraints GetInstance(
			object obj)
		{
			if (obj == null || obj is NameConstraints)
			{
				return (NameConstraints) obj;
			}

			if (obj is Asn1Sequence)
			{
				return new NameConstraints((Asn1Sequence) obj);
			}

            throw new ArgumentException("unknown object in factory: " + Platform.GetTypeName(obj), "obj");
		}

		public NameConstraints(
			Asn1Sequence seq)
		{
			foreach (Asn1TaggedObject o in seq)
			{
				switch (o.TagNo)
				{
					case 0:
						permitted = Asn1Sequence.GetInstance(o, false);
						break;
					case 1:
						excluded = Asn1Sequence.GetInstance(o, false);
						break;
				}
			}
		}

#if !(SILVERLIGHT || PORTABLE)
        public NameConstraints(
            ArrayList permitted,
            ArrayList excluded)
            : this((IList)permitted, (IList)excluded)
        {
        }
#endif

        /**
		 * Constructor from a given details.
		 *
		 * <p>permitted and excluded are Vectors of GeneralSubtree objects.</p>
		 *
		 * @param permitted Permitted subtrees
		 * @param excluded Excluded subtrees
		 */
		public NameConstraints(
			IList   permitted,
			IList   excluded)
		{
			if (permitted != null)
			{
				this.permitted = CreateSequence(permitted);
			}

			if (excluded != null)
			{
				this.excluded = CreateSequence(excluded);
			}
		}

		private DerSequence CreateSequence(
			IList subtrees)
		{
            GeneralSubtree[] gsts = new GeneralSubtree[subtrees.Count];
            for (int i = 0; i < subtrees.Count; ++i)
            {
                gsts[i] = (GeneralSubtree)subtrees[i];
            }
            return new DerSequence(gsts);
		}

		public Asn1Sequence PermittedSubtrees
		{
			get { return permitted; }
		}

		public Asn1Sequence ExcludedSubtrees
		{
			get { return excluded; }
		}

		/*
		 * NameConstraints ::= SEQUENCE { permittedSubtrees [0] GeneralSubtrees
		 * OPTIONAL, excludedSubtrees [1] GeneralSubtrees OPTIONAL }
		 */
		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector v = new Asn1EncodableVector();

			if (permitted != null)
			{
				v.Add(new DerTaggedObject(false, 0, permitted));
			}

			if (excluded != null)
			{
				v.Add(new DerTaggedObject(false, 1, excluded));
			}

			return new DerSequence(v);
		}
	}
}
