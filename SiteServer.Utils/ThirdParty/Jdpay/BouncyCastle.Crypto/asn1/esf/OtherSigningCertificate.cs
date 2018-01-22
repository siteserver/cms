using System;
using System.Collections;

using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;

namespace Org.BouncyCastle.Asn1.Esf
{
	/// <remarks>
	/// <code>
	/// OtherSigningCertificate ::= SEQUENCE {
	/// 	certs		SEQUENCE OF OtherCertID,
	/// 	policies	SEQUENCE OF PolicyInformation OPTIONAL
	/// }
	/// </code>
	/// </remarks>
	public class OtherSigningCertificate
		: Asn1Encodable
	{
		private readonly Asn1Sequence	certs;
		private readonly Asn1Sequence	policies;

		public static OtherSigningCertificate GetInstance(
			object obj)
		{
			if (obj == null || obj is OtherSigningCertificate)
				return (OtherSigningCertificate) obj;

			if (obj is Asn1Sequence)
				return new OtherSigningCertificate((Asn1Sequence) obj);

			throw new ArgumentException(
				"Unknown object in 'OtherSigningCertificate' factory: "
                    + Platform.GetTypeName(obj),
				"obj");
		}

		private OtherSigningCertificate(
			Asn1Sequence seq)
		{
			if (seq == null)
				throw new ArgumentNullException("seq");
			if (seq.Count < 1 || seq.Count > 2)
				throw new ArgumentException("Bad sequence size: " + seq.Count, "seq");

			this.certs = Asn1Sequence.GetInstance(seq[0].ToAsn1Object());

			if (seq.Count > 1)
			{
				this.policies = Asn1Sequence.GetInstance(seq[1].ToAsn1Object());
			}
		}

		public OtherSigningCertificate(
			params OtherCertID[] certs)
			: this(certs, null)
		{
		}

		public OtherSigningCertificate(
			OtherCertID[]				certs,
			params PolicyInformation[]	policies)
		{
			if (certs == null)
				throw new ArgumentNullException("certs");

			this.certs = new DerSequence(certs);

			if (policies != null)
			{
				this.policies = new DerSequence(policies);
			}
		}

		public OtherSigningCertificate(
			IEnumerable certs)
			: this(certs, null)
		{
		}

		public OtherSigningCertificate(
			IEnumerable	certs,
			IEnumerable	policies)
		{
			if (certs == null)
				throw new ArgumentNullException("certs");
			if (!CollectionUtilities.CheckElementsAreOfType(certs, typeof(OtherCertID)))
				throw new ArgumentException("Must contain only 'OtherCertID' objects", "certs");

			this.certs = new DerSequence(
				Asn1EncodableVector.FromEnumerable(certs));

			if (policies != null)
			{
				if (!CollectionUtilities.CheckElementsAreOfType(policies, typeof(PolicyInformation)))
					throw new ArgumentException("Must contain only 'PolicyInformation' objects", "policies");

				this.policies = new DerSequence(
					Asn1EncodableVector.FromEnumerable(policies));
			}
		}

		public OtherCertID[] GetCerts()
		{
			OtherCertID[] cs = new OtherCertID[certs.Count];
			for (int i = 0; i < certs.Count; ++i)
			{
				cs[i] = OtherCertID.GetInstance(certs[i].ToAsn1Object());
			}
			return cs;
		}

		public PolicyInformation[] GetPolicies()
		{
			if (policies == null)
				return null;

			PolicyInformation[] ps = new PolicyInformation[policies.Count];
			for (int i = 0; i < policies.Count; ++i)
			{
				ps[i] = PolicyInformation.GetInstance(policies[i].ToAsn1Object());
			}
			return ps;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector v = new Asn1EncodableVector(certs);

			if (policies != null)
			{
				v.Add(policies);
			}

			return new DerSequence(v);
		}
	}
}
