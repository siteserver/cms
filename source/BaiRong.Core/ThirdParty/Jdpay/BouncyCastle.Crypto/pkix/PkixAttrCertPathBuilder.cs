using System;
using System.Collections;

using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Store;

namespace Org.BouncyCastle.Pkix
{
	public class PkixAttrCertPathBuilder
	{
		/**
		* Build and validate a CertPath using the given parameter.
		*
		* @param params PKIXBuilderParameters object containing all information to
		*            build the CertPath
		*/
		public virtual PkixCertPathBuilderResult Build(
			PkixBuilderParameters pkixParams)
		{
			// search target certificates

			IX509Selector certSelect = pkixParams.GetTargetConstraints();
			if (!(certSelect is X509AttrCertStoreSelector))
			{
				throw new PkixCertPathBuilderException(
					"TargetConstraints must be an instance of "
					+ typeof(X509AttrCertStoreSelector).FullName
					+ " for "
					+ typeof(PkixAttrCertPathBuilder).FullName + " class.");
			}

			ICollection targets;
			try
			{
				targets = PkixCertPathValidatorUtilities.FindCertificates(
					(X509AttrCertStoreSelector)certSelect, pkixParams.GetStores());
			}
			catch (Exception e)
			{
				throw new PkixCertPathBuilderException("Error finding target attribute certificate.", e);
			}

			if (targets.Count == 0)
			{
				throw new PkixCertPathBuilderException(
					"No attribute certificate found matching targetContraints.");
			}

			PkixCertPathBuilderResult result = null;

			// check all potential target certificates
			foreach (IX509AttributeCertificate cert in targets)
			{
				X509CertStoreSelector selector = new X509CertStoreSelector();
				X509Name[] principals = cert.Issuer.GetPrincipals();
				ISet issuers = new HashSet();
				for (int i = 0; i < principals.Length; i++)
				{
					try
					{
						selector.Subject = principals[i];

						issuers.AddAll(PkixCertPathValidatorUtilities.FindCertificates(selector, pkixParams.GetStores()));
					}
					catch (Exception e)
					{
						throw new PkixCertPathBuilderException(
							"Public key certificate for attribute certificate cannot be searched.",
							e);
					}
				}

				if (issuers.IsEmpty)
					throw new PkixCertPathBuilderException("Public key certificate for attribute certificate cannot be found.");

                IList certPathList = Platform.CreateArrayList();

				foreach (X509Certificate issuer in issuers)
				{
					result = Build(cert, issuer, pkixParams, certPathList);

					if (result != null)
						break;
				}

				if (result != null)
					break;
			}

			if (result == null && certPathException != null)
			{
				throw new PkixCertPathBuilderException(
					"Possible certificate chain could not be validated.",
					certPathException);
			}

			if (result == null && certPathException == null)
			{
				throw new PkixCertPathBuilderException(
					"Unable to find certificate chain.");
			}

			return result;
		}

		private Exception certPathException;

		private PkixCertPathBuilderResult Build(
			IX509AttributeCertificate	attrCert,
			X509Certificate				tbvCert,
			PkixBuilderParameters		pkixParams,
			IList						tbvPath)
		{
			// If tbvCert is readily present in tbvPath, it indicates having run
			// into a cycle in the
			// PKI graph.
			if (tbvPath.Contains(tbvCert))
				return null;

			// step out, the certificate is not allowed to appear in a certification
			// chain
			if (pkixParams.GetExcludedCerts().Contains(tbvCert))
				return null;

			// test if certificate path exceeds maximum length
			if (pkixParams.MaxPathLength != -1)
			{
				if (tbvPath.Count - 1 > pkixParams.MaxPathLength)
					return null;
			}

			tbvPath.Add(tbvCert);

			PkixCertPathBuilderResult builderResult = null;

//			X509CertificateParser certParser = new X509CertificateParser();
			PkixAttrCertPathValidator validator = new PkixAttrCertPathValidator();

			try
			{
				// check whether the issuer of <tbvCert> is a TrustAnchor
				if (PkixCertPathValidatorUtilities.FindTrustAnchor(tbvCert, pkixParams.GetTrustAnchors()) != null)
				{
					PkixCertPath certPath = new PkixCertPath(tbvPath);
					PkixCertPathValidatorResult result;

					try
					{
						result = validator.Validate(certPath, pkixParams);
					}
					catch (Exception e)
					{
						throw new Exception("Certification path could not be validated.", e);
					}

					return new PkixCertPathBuilderResult(certPath, result.TrustAnchor,
						result.PolicyTree, result.SubjectPublicKey);
				}
				else
				{
					// add additional X.509 stores from locations in certificate
					try
					{
						PkixCertPathValidatorUtilities.AddAdditionalStoresFromAltNames(tbvCert, pkixParams);
					}
					catch (CertificateParsingException e)
					{
						throw new Exception("No additional X.509 stores can be added from certificate locations.", e);
					}

					// try to get the issuer certificate from one of the stores
					ISet issuers = new HashSet();
					try
					{
						issuers.AddAll(PkixCertPathValidatorUtilities.FindIssuerCerts(tbvCert, pkixParams));
					}
					catch (Exception e)
					{
						throw new Exception("Cannot find issuer certificate for certificate in certification path.", e);
					}

					if (issuers.IsEmpty)
						throw new Exception("No issuer certificate for certificate in certification path found.");

					foreach (X509Certificate issuer in issuers)
					{
						// if untrusted self signed certificate continue
						if (PkixCertPathValidatorUtilities.IsSelfIssued(issuer))
							continue;

						builderResult = Build(attrCert, issuer, pkixParams, tbvPath);

						if (builderResult != null)
							break;
					}
				}
			}
			catch (Exception e)
			{
				certPathException = new Exception("No valid certification path could be build.", e);
			}

			if (builderResult == null)
			{
				tbvPath.Remove(tbvCert);
			}

			return builderResult;
		}
	}
}
