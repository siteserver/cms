using System;
using System.IO;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;

namespace Org.BouncyCastle.X509
{
	/// <remarks>Interface for an X.509 Attribute Certificate.</remarks>
	public interface IX509AttributeCertificate
		: IX509Extension
	{
		/// <summary>The version number for the certificate.</summary>
		int Version { get; }

		/// <summary>The serial number for the certificate.</summary>
		BigInteger SerialNumber { get; }

		/// <summary>The UTC DateTime before which the certificate is not valid.</summary>
		DateTime NotBefore { get; }

		/// <summary>The UTC DateTime after which the certificate is not valid.</summary>
		DateTime NotAfter { get; }

		/// <summary>The holder of the certificate.</summary>
		AttributeCertificateHolder Holder { get; }

		/// <summary>The issuer details for the certificate.</summary>
		AttributeCertificateIssuer Issuer { get; }

		/// <summary>Return the attributes contained in the attribute block in the certificate.</summary>
		/// <returns>An array of attributes.</returns>
		X509Attribute[] GetAttributes();

		/// <summary>Return the attributes with the same type as the passed in oid.</summary>
		/// <param name="oid">The object identifier we wish to match.</param>
		/// <returns>An array of matched attributes, null if there is no match.</returns>
		X509Attribute[] GetAttributes(string oid);

		bool[] GetIssuerUniqueID();

		bool IsValidNow { get; }
		bool IsValid(DateTime date);

		void CheckValidity();
		void CheckValidity(DateTime date);

		byte[] GetSignature();

		void Verify(AsymmetricKeyParameter publicKey);

		/// <summary>Return an ASN.1 encoded byte array representing the attribute certificate.</summary>
		/// <returns>An ASN.1 encoded byte array.</returns>
		/// <exception cref="IOException">If the certificate cannot be encoded.</exception>
		byte[] GetEncoded();
	}
}
