using System;
using System.Collections;

using Org.BouncyCastle.Asn1.X500;

namespace Org.BouncyCastle.Asn1.Esf
{
	/**
	* Signer-Location attribute (RFC3126).
	*
	* <pre>
	*   SignerLocation ::= SEQUENCE {
	*       countryName        [0] DirectoryString OPTIONAL,
	*       localityName       [1] DirectoryString OPTIONAL,
	*       postalAddress      [2] PostalAddress OPTIONAL }
	*
	*   PostalAddress ::= SEQUENCE SIZE(1..6) OF DirectoryString
	* </pre>
	*/
	public class SignerLocation
		: Asn1Encodable
	{
        private DirectoryString countryName;
        private DirectoryString localityName;
        private Asn1Sequence postalAddress;

		public SignerLocation(
			Asn1Sequence seq)
		{
			foreach (Asn1TaggedObject obj in seq)
			{
				switch (obj.TagNo)
				{
					case 0:
						this.countryName = DirectoryString.GetInstance(obj, true);
						break;
					case 1:
                        this.localityName = DirectoryString.GetInstance(obj, true);
						break;
					case 2:
						bool isExplicit = obj.IsExplicit();	// handle erroneous implicitly tagged sequences
						this.postalAddress = Asn1Sequence.GetInstance(obj, isExplicit);
						if (postalAddress != null && postalAddress.Count > 6)
							throw new ArgumentException("postal address must contain less than 6 strings");
						break;
					default:
						throw new ArgumentException("illegal tag");
				}
			}
		}

        private SignerLocation(
            DirectoryString countryName,
            DirectoryString localityName,
            Asn1Sequence postalAddress)
        {
            if (postalAddress != null && postalAddress.Count > 6)
                throw new ArgumentException("postal address must contain less than 6 strings");

            this.countryName = countryName;
            this.localityName = localityName;
            this.postalAddress = postalAddress;
        }

        public SignerLocation(
            DirectoryString countryName,
            DirectoryString localityName,
            DirectoryString[] postalAddress)
            : this(countryName, localityName, new DerSequence(postalAddress))
        {
        }

        public SignerLocation(
            DerUtf8String countryName,
            DerUtf8String localityName,
            Asn1Sequence postalAddress)
            : this(DirectoryString.GetInstance(countryName), DirectoryString.GetInstance(localityName), postalAddress)
        {
        }

        public static SignerLocation GetInstance(
			object obj)
		{
			if (obj == null || obj is SignerLocation)
			{
				return (SignerLocation) obj;
			}

			return new SignerLocation(Asn1Sequence.GetInstance(obj));
		}

        public DirectoryString Country
        {
            get { return countryName; }
        }

        public DirectoryString Locality
        {
            get { return localityName; }
        }

        public DirectoryString[] GetPostal()
        {
            if (postalAddress == null)
                return null;

            DirectoryString[] dirStrings = new DirectoryString[postalAddress.Count];
            for (int i = 0; i != dirStrings.Length; i++)
            {
                dirStrings[i] = DirectoryString.GetInstance(postalAddress[i]);
            }

            return dirStrings;
        }

        [Obsolete("Use 'Country' property instead")]
		public DerUtf8String CountryName
		{
            get { return countryName == null ? null : new DerUtf8String(countryName.GetString()); }
		}

        [Obsolete("Use 'Locality' property instead")]
        public DerUtf8String LocalityName
        {
            get { return localityName == null ? null : new DerUtf8String(localityName.GetString()); }
        }

		public Asn1Sequence PostalAddress
		{
			get { return postalAddress; }
		}

		/**
		* <pre>
		*   SignerLocation ::= SEQUENCE {
		*       countryName        [0] DirectoryString OPTIONAL,
		*       localityName       [1] DirectoryString OPTIONAL,
		*       postalAddress      [2] PostalAddress OPTIONAL }
		*
		*   PostalAddress ::= SEQUENCE SIZE(1..6) OF DirectoryString
		*
		*   DirectoryString ::= CHOICE {
		*         teletexString           TeletexString (SIZE (1..MAX)),
		*         printableString         PrintableString (SIZE (1..MAX)),
		*         universalString         UniversalString (SIZE (1..MAX)),
		*         utf8String              UTF8String (SIZE (1.. MAX)),
		*         bmpString               BMPString (SIZE (1..MAX)) }
		* </pre>
		*/
		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector v = new Asn1EncodableVector();

			if (countryName != null)
			{
				v.Add(new DerTaggedObject(true, 0, countryName));
			}

			if (localityName != null)
			{
				v.Add(new DerTaggedObject(true, 1, localityName));
			}

			if (postalAddress != null)
			{
				v.Add(new DerTaggedObject(true, 2, postalAddress));
			}

			return new DerSequence(v);
		}
	}
}
