using System;
using System.Collections;
using System.Globalization;
using System.IO;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;
using Org.BouncyCastle.Utilities.Date;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Store;

namespace Org.BouncyCastle.Pkix
{
	public class Rfc3280CertPathUtilities
	{
		private static readonly PkixCrlUtilities CrlUtilities = new PkixCrlUtilities();

		internal static readonly string ANY_POLICY = "2.5.29.32.0";

		// key usage bits
		internal static readonly int KEY_CERT_SIGN = 5;
		internal static readonly int CRL_SIGN = 6;

		/**
		* If the complete CRL includes an issuing distribution point (IDP) CRL
		* extension check the following:
		* <p>
		* (i) If the distribution point name is present in the IDP CRL extension
		* and the distribution field is present in the DP, then verify that one of
		* the names in the IDP matches one of the names in the DP. If the
		* distribution point name is present in the IDP CRL extension and the
		* distribution field is omitted from the DP, then verify that one of the
		* names in the IDP matches one of the names in the cRLIssuer field of the
		* DP.
		* </p>
		* <p>
		* (ii) If the onlyContainsUserCerts boolean is asserted in the IDP CRL
		* extension, verify that the certificate does not include the basic
		* constraints extension with the cA boolean asserted.
		* </p>
		* <p>
		* (iii) If the onlyContainsCACerts boolean is asserted in the IDP CRL
		* extension, verify that the certificate includes the basic constraints
		* extension with the cA boolean asserted.
		* </p>
		* <p>
		* (iv) Verify that the onlyContainsAttributeCerts boolean is not asserted.
		* </p>
		*
		* @param dp   The distribution point.
		* @param cert The certificate.
		* @param crl  The CRL.
		* @throws AnnotatedException if one of the conditions is not met or an error occurs.
		*/
		internal static void ProcessCrlB2(
			DistributionPoint	dp,
			object				cert,
			X509Crl				crl)
		{
			IssuingDistributionPoint idp = null;
			try
			{
				idp = IssuingDistributionPoint.GetInstance(PkixCertPathValidatorUtilities.GetExtensionValue(crl, X509Extensions.IssuingDistributionPoint));
			}
			catch (Exception e)
			{
				throw new Exception("0 Issuing distribution point extension could not be decoded.", e);
			}
			// (b) (2) (i)
			// distribution point name is present
			if (idp != null)
			{
				if (idp.DistributionPoint != null)
				{
					// make list of names
					DistributionPointName dpName = IssuingDistributionPoint.GetInstance(idp).DistributionPoint;
					IList names = Platform.CreateArrayList();

					if (dpName.PointType == DistributionPointName.FullName)
					{
						GeneralName[] genNames = GeneralNames.GetInstance(dpName.Name).GetNames();
						for (int j = 0; j < genNames.Length; j++)
						{
							names.Add(genNames[j]);
						}
					}
					if (dpName.PointType == DistributionPointName.NameRelativeToCrlIssuer)
					{
						Asn1EncodableVector vec = new Asn1EncodableVector();
						try
						{
							IEnumerator e = Asn1Sequence.GetInstance(
								Asn1Sequence.FromByteArray(crl.IssuerDN.GetEncoded())).GetEnumerator();
							while (e.MoveNext())
							{
								vec.Add((Asn1Encodable)e.Current);
							}
						}
						catch (IOException e)
						{
							throw new Exception("Could not read CRL issuer.", e);
						}
						vec.Add(dpName.Name);
						names.Add(new GeneralName(X509Name.GetInstance(new DerSequence(vec))));
					}
					bool matches = false;
					// verify that one of the names in the IDP matches one
					// of the names in the DP.
					if (dp.DistributionPointName != null)
					{
						dpName = dp.DistributionPointName;
						GeneralName[] genNames = null;
						if (dpName.PointType == DistributionPointName.FullName)
						{
							genNames = GeneralNames.GetInstance(dpName.Name).GetNames();
						}
						if (dpName.PointType == DistributionPointName.NameRelativeToCrlIssuer)
						{
							if (dp.CrlIssuer != null)
							{
								genNames = dp.CrlIssuer.GetNames();
							}
							else
							{
								genNames = new GeneralName[1];
								try
								{
									genNames[0] = new GeneralName(
										PkixCertPathValidatorUtilities.GetIssuerPrincipal(cert));
								}
								catch (IOException e)
								{
									throw new Exception("Could not read certificate issuer.", e);
								}
							}
							for (int j = 0; j < genNames.Length; j++)
							{
								IEnumerator e = Asn1Sequence.GetInstance(genNames[j].Name.ToAsn1Object()).GetEnumerator();
								Asn1EncodableVector vec = new Asn1EncodableVector();
								while (e.MoveNext())
								{
									vec.Add((Asn1Encodable)e.Current);
								}
								vec.Add(dpName.Name);
								genNames[j] = new GeneralName(X509Name.GetInstance(new DerSequence(vec)));
							}
						}
						if (genNames != null)
						{
							for (int j = 0; j < genNames.Length; j++)
							{
								if (names.Contains(genNames[j]))
								{
									matches = true;
									break;
								}
							}
						}
						if (!matches)
						{
							throw new Exception(
								"No match for certificate CRL issuing distribution point name to cRLIssuer CRL distribution point.");
						}
					}
						// verify that one of the names in
						// the IDP matches one of the names in the cRLIssuer field of
						// the DP
					else
					{
						if (dp.CrlIssuer == null)
						{
							throw new Exception("Either the cRLIssuer or the distributionPoint field must "
								+ "be contained in DistributionPoint.");
						}
						GeneralName[] genNames = dp.CrlIssuer.GetNames();
						for (int j = 0; j < genNames.Length; j++)
						{
							if (names.Contains(genNames[j]))
							{
								matches = true;
								break;
							}
						}
						if (!matches)
						{
							throw new Exception(
								"No match for certificate CRL issuing distribution point name to cRLIssuer CRL distribution point.");
						}
					}
				}
				BasicConstraints bc = null;
				try
				{
					bc = BasicConstraints.GetInstance(PkixCertPathValidatorUtilities.GetExtensionValue(
						(IX509Extension)cert, X509Extensions.BasicConstraints));
				}
				catch (Exception e)
				{
					throw new Exception("Basic constraints extension could not be decoded.", e);
				}

				//if (cert is X509Certificate)
				{
					// (b) (2) (ii)
					if (idp.OnlyContainsUserCerts && ((bc != null) && bc.IsCA()))
					{
						throw new Exception("CA Cert CRL only contains user certificates.");
					}

					// (b) (2) (iii)
					if (idp.OnlyContainsCACerts && (bc == null || !bc.IsCA()))
					{
						throw new Exception("End CRL only contains CA certificates.");
					}
				}

				// (b) (2) (iv)
				if (idp.OnlyContainsAttributeCerts)
				{
					throw new Exception("onlyContainsAttributeCerts boolean is asserted.");
				}
			}
		}

		internal static void ProcessCertBC(
			PkixCertPath				certPath,
			int							index,
			PkixNameConstraintValidator	nameConstraintValidator)
			//throws CertPathValidatorException
		{
			IList certs = certPath.Certificates;
			X509Certificate cert = (X509Certificate)certs[index];
			int n = certs.Count;
			// i as defined in the algorithm description
			int i = n - index;
			//
			// (b), (c) permitted and excluded subtree checking.
			//
			if (!(PkixCertPathValidatorUtilities.IsSelfIssued(cert) && (i < n)))
			{
				X509Name principal = cert.SubjectDN;
				Asn1InputStream aIn = new Asn1InputStream(principal.GetEncoded());
				Asn1Sequence dns;

				try
				{
					dns = DerSequence.GetInstance(aIn.ReadObject());
				}
				catch (Exception e)
				{
					throw new PkixCertPathValidatorException(
						"Exception extracting subject name when checking subtrees.", e, certPath, index);
				}

				try
				{
					nameConstraintValidator.CheckPermittedDN(dns);
					nameConstraintValidator.CheckExcludedDN(dns);
				}
				catch (PkixNameConstraintValidatorException e)
				{
					throw new PkixCertPathValidatorException(
						"Subtree check for certificate subject failed.", e, certPath, index);
				}

				GeneralNames altName = null;
				try
				{
					altName = GeneralNames.GetInstance(
						PkixCertPathValidatorUtilities.GetExtensionValue(cert, X509Extensions.SubjectAlternativeName));
				}
				catch (Exception e)
				{
					throw new PkixCertPathValidatorException(
						"Subject alternative name extension could not be decoded.", e, certPath, index);
				}

				IList emails = X509Name.GetInstance(dns).GetValueList(X509Name.EmailAddress);
				foreach (string email in emails)
				{
					GeneralName emailAsGeneralName = new GeneralName(GeneralName.Rfc822Name, email);
					try
					{
						nameConstraintValidator.checkPermitted(emailAsGeneralName);
						nameConstraintValidator.checkExcluded(emailAsGeneralName);
					}
					catch (PkixNameConstraintValidatorException ex)
					{
						throw new PkixCertPathValidatorException(
							"Subtree check for certificate subject alternative email failed.", ex, certPath, index);
					}
				}
				if (altName != null)
				{
					GeneralName[] genNames = null;
					try
					{
						genNames = altName.GetNames();
					}
					catch (Exception e)
					{
						throw new PkixCertPathValidatorException(
							"Subject alternative name contents could not be decoded.", e, certPath, index);
					}
					foreach (GeneralName genName in genNames)
					{
						try
						{
							nameConstraintValidator.checkPermitted(genName);
							nameConstraintValidator.checkExcluded(genName);
						}
						catch (PkixNameConstraintValidatorException e)
						{
							throw new PkixCertPathValidatorException(
								"Subtree check for certificate subject alternative name failed.", e, certPath, index);
						}
					}
				}
			}
		}

		internal static void PrepareNextCertA(
			PkixCertPath	certPath,
			int				index)
			//throws CertPathValidatorException
		{
			IList certs = certPath.Certificates;
			X509Certificate cert = (X509Certificate)certs[index];
			//
			//
			// (a) check the policy mappings
			//
			Asn1Sequence pm = null;
			try
			{
				pm = Asn1Sequence.GetInstance(
					PkixCertPathValidatorUtilities.GetExtensionValue(cert, X509Extensions.PolicyMappings));
			}
			catch (Exception ex)
			{
				throw new PkixCertPathValidatorException(
					"Policy mappings extension could not be decoded.", ex, certPath, index);
			}
			if (pm != null)
			{
				Asn1Sequence mappings = pm;

				for (int j = 0; j < mappings.Count; j++)
				{
					DerObjectIdentifier issuerDomainPolicy = null;
					DerObjectIdentifier subjectDomainPolicy = null;
					try
					{
						Asn1Sequence mapping = DerSequence.GetInstance(mappings[j]);

						issuerDomainPolicy = DerObjectIdentifier.GetInstance(mapping[0]);
						subjectDomainPolicy = DerObjectIdentifier.GetInstance(mapping[1]);
					}
					catch (Exception e)
					{
						throw new PkixCertPathValidatorException(
							"Policy mappings extension contents could not be decoded.", e, certPath, index);
					}

					if (Rfc3280CertPathUtilities.ANY_POLICY.Equals(issuerDomainPolicy.Id))
						throw new PkixCertPathValidatorException(
							"IssuerDomainPolicy is anyPolicy", null, certPath, index);

					if (Rfc3280CertPathUtilities.ANY_POLICY.Equals(subjectDomainPolicy.Id))
						throw new PkixCertPathValidatorException(
							"SubjectDomainPolicy is anyPolicy,", null, certPath, index);
				}
			}
		}

		internal static PkixPolicyNode ProcessCertD(
			PkixCertPath	certPath,
			int				index,
			ISet			acceptablePolicies,
			PkixPolicyNode	validPolicyTree,
			IList[]			policyNodes,
			int				inhibitAnyPolicy)
			//throws CertPathValidatorException
		{
			IList certs = certPath.Certificates;
			X509Certificate cert = (X509Certificate)certs[index];
			int n = certs.Count;
			// i as defined in the algorithm description
			int i = n - index;
			//
			// (d) policy Information checking against initial policy and
			// policy mapping
			//
			Asn1Sequence certPolicies = null;
			try
			{
				certPolicies = DerSequence.GetInstance(
					PkixCertPathValidatorUtilities.GetExtensionValue(cert, X509Extensions.CertificatePolicies));
			}
			catch (Exception e)
			{
				throw new PkixCertPathValidatorException(
					"Could not read certificate policies extension from certificate.", e, certPath, index);
			}
			if (certPolicies != null && validPolicyTree != null)
			{
				//
				// (d) (1)
				//
				ISet pols = new HashSet();

				foreach (Asn1Encodable ae in certPolicies)
				{
					PolicyInformation pInfo = PolicyInformation.GetInstance(ae.ToAsn1Object());
					DerObjectIdentifier pOid = pInfo.PolicyIdentifier;

					pols.Add(pOid.Id);

					if (!Rfc3280CertPathUtilities.ANY_POLICY.Equals(pOid.Id))
					{
						ISet pq = null;
						try
						{
							pq = PkixCertPathValidatorUtilities.GetQualifierSet(pInfo.PolicyQualifiers);
						}
						catch (PkixCertPathValidatorException ex)
						{
							throw new PkixCertPathValidatorException(
								"Policy qualifier info set could not be build.", ex, certPath, index);
						}

						bool match = PkixCertPathValidatorUtilities.ProcessCertD1i(i, policyNodes, pOid, pq);

						if (!match)
						{
							PkixCertPathValidatorUtilities.ProcessCertD1ii(i, policyNodes, pOid, pq);
						}
					}
				}

				if (acceptablePolicies.IsEmpty || acceptablePolicies.Contains(Rfc3280CertPathUtilities.ANY_POLICY))
				{
					acceptablePolicies.Clear();
					acceptablePolicies.AddAll(pols);
				}
				else
				{
					ISet t1 = new HashSet();

					foreach (object o in acceptablePolicies)
					{
						if (pols.Contains(o))
						{
							t1.Add(o);
						}
					}
					acceptablePolicies.Clear();
					acceptablePolicies.AddAll(t1);
				}

				//
				// (d) (2)
				//
				if ((inhibitAnyPolicy > 0) || ((i < n) && PkixCertPathValidatorUtilities.IsSelfIssued(cert)))
				{
					foreach (Asn1Encodable ae in certPolicies)
					{
						PolicyInformation pInfo = PolicyInformation.GetInstance(ae.ToAsn1Object());
						if (Rfc3280CertPathUtilities.ANY_POLICY.Equals(pInfo.PolicyIdentifier.Id))
						{
							ISet _apq = PkixCertPathValidatorUtilities.GetQualifierSet(pInfo.PolicyQualifiers);
							IList _nodes = policyNodes[i - 1];

							for (int k = 0; k < _nodes.Count; k++)
							{
								PkixPolicyNode _node = (PkixPolicyNode)_nodes[k];

								IEnumerator  _policySetIter = _node.ExpectedPolicies.GetEnumerator();
								while (_policySetIter.MoveNext())
								{
									object _tmp = _policySetIter.Current;

									string _policy;
									if (_tmp is string)
									{
										_policy = (string)_tmp;
									}
									else if (_tmp is DerObjectIdentifier)
									{
										_policy = ((DerObjectIdentifier)_tmp).Id;
									}
									else
									{
										continue;
									}

									bool _found = false;

									foreach (PkixPolicyNode _child in _node.Children)
									{
										if (_policy.Equals(_child.ValidPolicy))
										{
											_found = true;
										}
									}

									if (!_found)
									{
										ISet _newChildExpectedPolicies = new HashSet();
										_newChildExpectedPolicies.Add(_policy);

										PkixPolicyNode _newChild = new PkixPolicyNode(Platform.CreateArrayList(), i,
											_newChildExpectedPolicies, _node, _apq, _policy, false);
										_node.AddChild(_newChild);
										policyNodes[i].Add(_newChild);
									}
								}
							}
							break;
						}
					}
				}

				PkixPolicyNode _validPolicyTree = validPolicyTree;
				//
				// (d) (3)
				//
				for (int j = (i - 1); j >= 0; j--)
				{
					IList nodes = policyNodes[j];

					for (int k = 0; k < nodes.Count; k++)
					{
						PkixPolicyNode node = (PkixPolicyNode)nodes[k];
						if (!node.HasChildren)
						{
							_validPolicyTree = PkixCertPathValidatorUtilities.RemovePolicyNode(_validPolicyTree, policyNodes,
								node);
							if (_validPolicyTree == null)
							{
								break;
							}
						}
					}
				}

				//
				// d (4)
				//
				ISet criticalExtensionOids = cert.GetCriticalExtensionOids();

				if (criticalExtensionOids != null)
				{
					bool critical = criticalExtensionOids.Contains(X509Extensions.CertificatePolicies.Id);

					IList nodes = policyNodes[i];
					for (int j = 0; j < nodes.Count; j++)
					{
						PkixPolicyNode node = (PkixPolicyNode)nodes[j];
						node.IsCritical = critical;
					}
				}
				return _validPolicyTree;
			}
			return null;
		}

		/**
		* If the DP includes cRLIssuer, then verify that the issuer field in the
		* complete CRL matches cRLIssuer in the DP and that the complete CRL
		* contains an
		*      g distribution point extension with the indirectCRL
		* boolean asserted. Otherwise, verify that the CRL issuer matches the
		* certificate issuer.
		*
		* @param dp   The distribution point.
		* @param cert The certificate ot attribute certificate.
		* @param crl  The CRL for <code>cert</code>.
		* @throws AnnotatedException if one of the above conditions does not apply or an error
		*                            occurs.
		*/
		internal static void ProcessCrlB1(
			DistributionPoint	dp,
			object				cert,
			X509Crl				crl)
		{
			Asn1Object idp = PkixCertPathValidatorUtilities.GetExtensionValue(
				crl, X509Extensions.IssuingDistributionPoint);

			bool isIndirect = false;
			if (idp != null)
			{
				if (IssuingDistributionPoint.GetInstance(idp).IsIndirectCrl)
				{
					isIndirect = true;
				}
			}
			byte[] issuerBytes = crl.IssuerDN.GetEncoded();

			bool matchIssuer = false;
			if (dp.CrlIssuer != null)
			{
				GeneralName[] genNames = dp.CrlIssuer.GetNames();
				for (int j = 0; j < genNames.Length; j++)
				{
					if (genNames[j].TagNo == GeneralName.DirectoryName)
					{
						try
						{
							if (Org.BouncyCastle.Utilities.Arrays.AreEqual(genNames[j].Name.ToAsn1Object().GetEncoded(), issuerBytes))
							{
								matchIssuer = true;
							}
						}
						catch (IOException e)
						{
							throw new Exception(
								"CRL issuer information from distribution point cannot be decoded.", e);
						}
					}
				}
				if (matchIssuer && !isIndirect)
				{
					throw new Exception("Distribution point contains cRLIssuer field but CRL is not indirect.");
				}
				if (!matchIssuer)
				{
					throw new Exception("CRL issuer of CRL does not match CRL issuer of distribution point.");
				}
			}
			else
			{
				if (crl.IssuerDN.Equivalent(PkixCertPathValidatorUtilities.GetIssuerPrincipal(cert), true))
				{
					matchIssuer = true;
				}
			}
			if (!matchIssuer)
			{
				throw new Exception("Cannot find matching CRL issuer for certificate.");
			}
		}

		internal static ReasonsMask ProcessCrlD(
			X509Crl				crl,
			DistributionPoint	dp)
			//throws AnnotatedException
		{
			IssuingDistributionPoint idp = null;
			try
			{
				idp = IssuingDistributionPoint.GetInstance(PkixCertPathValidatorUtilities.GetExtensionValue(crl, X509Extensions.IssuingDistributionPoint));
			}
			catch (Exception e)
			{
				throw new Exception("issuing distribution point extension could not be decoded.", e);
			}

			// (d) (1)
			if (idp != null && idp.OnlySomeReasons != null && dp.Reasons != null)
			{
				return new ReasonsMask(dp.Reasons.IntValue).Intersect(new ReasonsMask(idp.OnlySomeReasons
					.IntValue));
			}
			// (d) (4)
			if ((idp == null || idp.OnlySomeReasons == null) && dp.Reasons == null)
			{
				return ReasonsMask.AllReasons;
			}

			// (d) (2) and (d)(3)

			ReasonsMask dpReasons = null;

			if (dp.Reasons == null)
			{
				dpReasons = ReasonsMask.AllReasons;
			}
			else
			{
				dpReasons = new ReasonsMask(dp.Reasons.IntValue);
			}

			ReasonsMask idpReasons = null;

			if (idp == null)
			{
				idpReasons = ReasonsMask.AllReasons;
			}
			else
			{
				idpReasons = new ReasonsMask(idp.OnlySomeReasons.IntValue);
			}

			return dpReasons.Intersect(idpReasons);
		}

		/**
		* Obtain and validate the certification path for the complete CRL issuer.
		* If a key usage extension is present in the CRL issuer's certificate,
		* verify that the cRLSign bit is set.
		*
		* @param crl                CRL which contains revocation information for the certificate
		*                           <code>cert</code>.
		* @param cert               The attribute certificate or certificate to check if it is
		*                           revoked.
		* @param defaultCRLSignCert The issuer certificate of the certificate <code>cert</code>.
		* @param defaultCRLSignKey  The public key of the issuer certificate
		*                           <code>defaultCRLSignCert</code>.
		* @param paramsPKIX         paramsPKIX PKIX parameters.
		* @param certPathCerts      The certificates on the certification path.
		* @return A <code>Set</code> with all keys of possible CRL issuer
		*         certificates.
		* @throws AnnotatedException if the CRL is not valid or the status cannot be checked or
		*                            some error occurs.
		*/
		internal static ISet ProcessCrlF(
			X509Crl					crl,
			object					cert,
			X509Certificate			defaultCRLSignCert,
			AsymmetricKeyParameter	defaultCRLSignKey,
			PkixParameters			paramsPKIX,
			IList					certPathCerts)
		{
			// (f)

			// get issuer from CRL
			X509CertStoreSelector selector = new X509CertStoreSelector();
			try
			{
				selector.Subject = crl.IssuerDN;
			}
			catch (IOException e)
			{
				throw new Exception(
					"Subject criteria for certificate selector to find issuer certificate for CRL could not be set.", e);
			}

			// get CRL signing certs
			IList coll = Platform.CreateArrayList();

			try
			{
                CollectionUtilities.AddRange(coll, PkixCertPathValidatorUtilities.FindCertificates(selector, paramsPKIX.GetStores()));
                CollectionUtilities.AddRange(coll, PkixCertPathValidatorUtilities.FindCertificates(selector, paramsPKIX.GetAdditionalStores()));
			}
			catch (Exception e)
			{
				throw new Exception("Issuer certificate for CRL cannot be searched.", e);
			}

			coll.Add(defaultCRLSignCert);

			IEnumerator cert_it = coll.GetEnumerator();

            IList validCerts = Platform.CreateArrayList();
            IList validKeys = Platform.CreateArrayList();

			while (cert_it.MoveNext())
			{
				X509Certificate signingCert = (X509Certificate)cert_it.Current;

				/*
				 * CA of the certificate, for which this CRL is checked, has also
				 * signed CRL, so skip the path validation, because is already done
				 */
				if (signingCert.Equals(defaultCRLSignCert))
				{
					validCerts.Add(signingCert);
					validKeys.Add(defaultCRLSignKey);
					continue;
				}
				try
				{
//					CertPathBuilder builder = CertPathBuilder.GetInstance("PKIX");
					PkixCertPathBuilder builder = new PkixCertPathBuilder();
					selector = new X509CertStoreSelector();
					selector.Certificate = signingCert;

					PkixParameters temp = (PkixParameters)paramsPKIX.Clone();
					temp.SetTargetCertConstraints(selector);

					PkixBuilderParameters parameters = (PkixBuilderParameters)
						PkixBuilderParameters.GetInstance(temp);

					/*
					 * if signingCert is placed not higher on the cert path a
					 * dependency loop results. CRL for cert is checked, but
					 * signingCert is needed for checking the CRL which is dependent
					 * on checking cert because it is higher in the cert path and so
					 * signing signingCert transitively. so, revocation is disabled,
					 * forgery attacks of the CRL are detected in this outer loop
					 * for all other it must be enabled to prevent forgery attacks
					 */
					if (certPathCerts.Contains(signingCert))
					{
						parameters.IsRevocationEnabled = false;
					}
					else
					{
						parameters.IsRevocationEnabled = true;
					}
					IList certs = builder.Build(parameters).CertPath.Certificates;
					validCerts.Add(signingCert);
					validKeys.Add(PkixCertPathValidatorUtilities.GetNextWorkingKey(certs, 0));
				}
				catch (PkixCertPathBuilderException e)
				{
					throw new Exception("Internal error.", e);
				}
				catch (PkixCertPathValidatorException e)
				{
					throw new Exception("Public key of issuer certificate of CRL could not be retrieved.", e);
				}
				//catch (Exception e)
				//{
				//    throw new Exception(e.Message);
				//}
			}

			ISet checkKeys = new HashSet();

			Exception lastException = null;
			for (int i = 0; i < validCerts.Count; i++)
			{
				X509Certificate signCert = (X509Certificate)validCerts[i];
				bool[] keyusage = signCert.GetKeyUsage();

				if (keyusage != null && (keyusage.Length < 7 || !keyusage[CRL_SIGN]))
				{
					lastException = new Exception(
						"Issuer certificate key usage extension does not permit CRL signing.");
				}
				else
				{
					checkKeys.Add(validKeys[i]);
				}
			}

			if ((checkKeys.Count == 0) && lastException == null)
			{
				throw new Exception("Cannot find a valid issuer certificate.");
			}
			if ((checkKeys.Count == 0) && lastException != null)
			{
				throw lastException;
			}

			return checkKeys;
		}

		internal static AsymmetricKeyParameter ProcessCrlG(
			X509Crl	crl,
			ISet	keys)
		{
			Exception lastException = null;
			foreach (AsymmetricKeyParameter key in keys)
			{
				try
				{
					crl.Verify(key);
					return key;
				}
				catch (Exception e)
				{
					lastException = e;
				}
			}
			throw new Exception("Cannot verify CRL.", lastException);
		}

		internal static X509Crl ProcessCrlH(
			ISet					deltaCrls,
			AsymmetricKeyParameter	key)
		{
			Exception lastException = null;
			foreach (X509Crl crl in deltaCrls)
			{
				try
				{
					crl.Verify(key);
					return crl;
				}
				catch (Exception e)
				{
					lastException = e;
				}
			}
			if (lastException != null)
			{
				throw new Exception("Cannot verify delta CRL.", lastException);
			}
			return null;
		}

		/**
		* Checks a distribution point for revocation information for the
		* certificate <code>cert</code>.
		*
		* @param dp                 The distribution point to consider.
		* @param paramsPKIX         PKIX parameters.
		* @param cert               Certificate to check if it is revoked.
		* @param validDate          The date when the certificate revocation status should be
		*                           checked.
		* @param defaultCRLSignCert The issuer certificate of the certificate <code>cert</code>.
		* @param defaultCRLSignKey  The public key of the issuer certificate
		*                           <code>defaultCRLSignCert</code>.
		* @param certStatus         The current certificate revocation status.
		* @param reasonMask         The reasons mask which is already checked.
		* @param certPathCerts      The certificates of the certification path.
		* @throws AnnotatedException if the certificate is revoked or the status cannot be checked
		*                            or some error occurs.
		*/
		private static void CheckCrl(
			DistributionPoint dp,
			PkixParameters paramsPKIX,
			X509Certificate cert,
			DateTime validDate,
			X509Certificate defaultCRLSignCert,
			AsymmetricKeyParameter defaultCRLSignKey,
			CertStatus certStatus,
			ReasonsMask reasonMask,
			IList certPathCerts)
			//throws AnnotatedException
		{
			DateTime currentDate = DateTime.UtcNow;

			if (validDate.Ticks > currentDate.Ticks)
			{
				throw new Exception("Validation time is in future.");
			}

			// (a)
			/*
			 * We always get timely valid CRLs, so there is no step (a) (1).
			 * "locally cached" CRLs are assumed to be in getStore(), additional
			 * CRLs must be enabled in the ExtendedPKIXParameters and are in
			 * getAdditionalStore()
			 */

			ISet crls = PkixCertPathValidatorUtilities.GetCompleteCrls(dp, cert, currentDate, paramsPKIX);
			bool validCrlFound = false;
			Exception lastException = null;

			IEnumerator crl_iter = crls.GetEnumerator();

			while (crl_iter.MoveNext() && certStatus.Status == CertStatus.Unrevoked && !reasonMask.IsAllReasons)
			{
				try
				{
					X509Crl crl = (X509Crl)crl_iter.Current;

					// (d)
					ReasonsMask interimReasonsMask = Rfc3280CertPathUtilities.ProcessCrlD(crl, dp);

					// (e)
					/*
					 * The reasons mask is updated at the end, so only valid CRLs
					 * can update it. If this CRL does not contain new reasons it
					 * must be ignored.
					 */
					if (!interimReasonsMask.HasNewReasons(reasonMask))
					{
						continue;
					}

					// (f)
					ISet keys = Rfc3280CertPathUtilities.ProcessCrlF(crl, cert, defaultCRLSignCert, defaultCRLSignKey,
						paramsPKIX, certPathCerts);
					// (g)
					AsymmetricKeyParameter key = Rfc3280CertPathUtilities.ProcessCrlG(crl, keys);

					X509Crl deltaCRL = null;

					if (paramsPKIX.IsUseDeltasEnabled)
					{
						// get delta CRLs
						ISet deltaCRLs = PkixCertPathValidatorUtilities.GetDeltaCrls(currentDate, paramsPKIX, crl);
						// we only want one valid delta CRL
						// (h)
						deltaCRL = Rfc3280CertPathUtilities.ProcessCrlH(deltaCRLs, key);
					}

					/*
					 * CRL must be be valid at the current time, not the validation
					 * time. If a certificate is revoked with reason keyCompromise,
					 * cACompromise, it can be used for forgery, also for the past.
					 * This reason may not be contained in older CRLs.
					 */

					/*
					 * in the chain model signatures stay valid also after the
					 * certificate has been expired, so they do not have to be in
					 * the CRL validity time
					 */

					if (paramsPKIX.ValidityModel != PkixParameters.ChainValidityModel)
					{
						/*
						 * if a certificate has expired, but was revoked, it is not
						 * more in the CRL, so it would be regarded as valid if the
						 * first check is not done
						 */
						if (cert.NotAfter.Ticks < crl.ThisUpdate.Ticks)
						{
							throw new Exception("No valid CRL for current time found.");
						}
					}

					Rfc3280CertPathUtilities.ProcessCrlB1(dp, cert, crl);

					// (b) (2)
					Rfc3280CertPathUtilities.ProcessCrlB2(dp, cert, crl);

					// (c)
					Rfc3280CertPathUtilities.ProcessCrlC(deltaCRL, crl, paramsPKIX);

					// (i)
					Rfc3280CertPathUtilities.ProcessCrlI(validDate, deltaCRL, cert, certStatus, paramsPKIX);

					// (j)
					Rfc3280CertPathUtilities.ProcessCrlJ(validDate, crl, cert, certStatus);

					// (k)
					if (certStatus.Status == CrlReason.RemoveFromCrl)
					{
						certStatus.Status = CertStatus.Unrevoked;
					}

					// update reasons mask
					reasonMask.AddReasons(interimReasonsMask);

					ISet criticalExtensions = crl.GetCriticalExtensionOids();

					if (criticalExtensions != null)
					{
						criticalExtensions = new HashSet(criticalExtensions);
						criticalExtensions.Remove(X509Extensions.IssuingDistributionPoint.Id);
						criticalExtensions.Remove(X509Extensions.DeltaCrlIndicator.Id);

						if (!criticalExtensions.IsEmpty)
							throw new Exception("CRL contains unsupported critical extensions.");
					}

					if (deltaCRL != null)
					{
						criticalExtensions = deltaCRL.GetCriticalExtensionOids();
						if (criticalExtensions != null)
						{
							criticalExtensions = new HashSet(criticalExtensions);
							criticalExtensions.Remove(X509Extensions.IssuingDistributionPoint.Id);
							criticalExtensions.Remove(X509Extensions.DeltaCrlIndicator.Id);

							if (!criticalExtensions.IsEmpty)
								throw new Exception("Delta CRL contains unsupported critical extension.");
						}
					}

					validCrlFound = true;
				}
				catch (Exception e)
				{
					lastException = e;
				}
			}
			if (!validCrlFound)
			{
				throw lastException;
			}
		}

		/**
		 * Checks a certificate if it is revoked.
		 *
		 * @param paramsPKIX       PKIX parameters.
		 * @param cert             Certificate to check if it is revoked.
		 * @param validDate        The date when the certificate revocation status should be
		 *                         checked.
		 * @param sign             The issuer certificate of the certificate <code>cert</code>.
		 * @param workingPublicKey The public key of the issuer certificate <code>sign</code>.
		 * @param certPathCerts    The certificates of the certification path.
		 * @throws AnnotatedException if the certificate is revoked or the status cannot be checked
		 *                            or some error occurs.
		 */
		protected static void CheckCrls(
			PkixParameters			paramsPKIX,
			X509Certificate			cert,
			DateTime				validDate,
			X509Certificate			sign,
			AsymmetricKeyParameter	workingPublicKey,
			IList					certPathCerts)
		{
			Exception lastException = null;
			CrlDistPoint crldp = null;

			try
			{
				crldp = CrlDistPoint.GetInstance(PkixCertPathValidatorUtilities.GetExtensionValue(cert, X509Extensions.CrlDistributionPoints));
			}
			catch (Exception e)
			{
				throw new Exception("CRL distribution point extension could not be read.", e);
			}

			try
			{
				PkixCertPathValidatorUtilities.AddAdditionalStoresFromCrlDistributionPoint(crldp, paramsPKIX);
			}
			catch (Exception e)
			{
				throw new Exception(
					"No additional CRL locations could be decoded from CRL distribution point extension.", e);
			}
			CertStatus certStatus = new CertStatus();
			ReasonsMask reasonsMask = new ReasonsMask();

			bool validCrlFound = false;

			// for each distribution point
			if (crldp != null)
			{
				DistributionPoint[] dps = null;
				try
				{
					dps = crldp.GetDistributionPoints();
				}
				catch (Exception e)
				{
					throw new Exception("Distribution points could not be read.", e);
				}
				if (dps != null)
				{
					for (int i = 0; i < dps.Length && certStatus.Status == CertStatus.Unrevoked && !reasonsMask.IsAllReasons; i++)
					{
						PkixParameters paramsPKIXClone = (PkixParameters)paramsPKIX.Clone();
						try
						{
							CheckCrl(dps[i], paramsPKIXClone, cert, validDate, sign, workingPublicKey, certStatus, reasonsMask, certPathCerts);
							validCrlFound = true;
						}
						catch (Exception e)
						{
							lastException = e;
						}
					}
				}
			}

			/*
			 * If the revocation status has not been determined, repeat the process
			 * above with any available CRLs not specified in a distribution point
			 * but issued by the certificate issuer.
			 */

			if (certStatus.Status == CertStatus.Unrevoked && !reasonsMask.IsAllReasons)
			{
				try
				{
					/*
					 * assume a DP with both the reasons and the cRLIssuer fields
					 * omitted and a distribution point name of the certificate
					 * issuer.
					 */
					Asn1Object issuer = null;
					try
					{
						issuer = new Asn1InputStream(cert.IssuerDN.GetEncoded()).ReadObject();
					}
					catch (Exception e)
					{
						throw new Exception("Issuer from certificate for CRL could not be reencoded.", e);
					}
					DistributionPoint dp = new DistributionPoint(new DistributionPointName(0, new GeneralNames(
						new GeneralName(GeneralName.DirectoryName, issuer))), null, null);
					PkixParameters paramsPKIXClone = (PkixParameters)paramsPKIX.Clone();

					CheckCrl(dp, paramsPKIXClone, cert, validDate, sign, workingPublicKey, certStatus, reasonsMask,
						certPathCerts);

					validCrlFound = true;
				}
				catch (Exception e)
				{
					lastException = e;
				}
			}

			if (!validCrlFound)
			{
				throw lastException;
			}
			if (certStatus.Status != CertStatus.Unrevoked)
			{
				// This format is enforced by the NistCertPath tests
                string formattedDate = certStatus.RevocationDate.Value.ToString(
                    "ddd MMM dd HH:mm:ss K yyyy");
                string message = "Certificate revocation after " + formattedDate;
				message += ", reason: " + CrlReasons[certStatus.Status];
				throw new Exception(message);
			}

			if (!reasonsMask.IsAllReasons && certStatus.Status == CertStatus.Unrevoked)
			{
				certStatus.Status = CertStatus.Undetermined;
			}

			if (certStatus.Status == CertStatus.Undetermined)
			{
				throw new Exception("Certificate status could not be determined.");
			}
		}

		internal static PkixPolicyNode PrepareCertB(
			PkixCertPath	certPath,
			int				index,
			IList[]			policyNodes,
			PkixPolicyNode	validPolicyTree,
			int				policyMapping)
			//throws CertPathValidatorException
		{
			IList certs = certPath.Certificates;
			X509Certificate cert = (X509Certificate)certs[index];
			int n = certs.Count;
			// i as defined in the algorithm description
			int i = n - index;
			// (b)
			//
			Asn1Sequence pm = null;
			try
			{
				pm = (Asn1Sequence)Asn1Sequence.GetInstance(PkixCertPathValidatorUtilities.GetExtensionValue(cert, X509Extensions.PolicyMappings));
			}
			catch (Exception ex)
			{
				throw new PkixCertPathValidatorException(
					"Policy mappings extension could not be decoded.", ex, certPath, index);
			}
			PkixPolicyNode _validPolicyTree = validPolicyTree;
			if (pm != null)
			{
				Asn1Sequence mappings = (Asn1Sequence)pm;
				IDictionary m_idp = Platform.CreateHashtable();
				ISet s_idp = new HashSet();

				for (int j = 0; j < mappings.Count; j++)
				{
					Asn1Sequence mapping = (Asn1Sequence) mappings[j];
					string id_p = ((DerObjectIdentifier) mapping[0]).Id;
					string sd_p = ((DerObjectIdentifier) mapping[1]).Id;
					ISet tmp;

					if (!m_idp.Contains(id_p))
					{
						tmp = new HashSet();
						tmp.Add(sd_p);
						m_idp[id_p] = tmp;
						s_idp.Add(id_p);
					}
					else
					{
						tmp = (ISet)m_idp[id_p];
						tmp.Add(sd_p);
					}
				}

				IEnumerator it_idp = s_idp.GetEnumerator();
				while (it_idp.MoveNext())
				{
					string id_p = (string)it_idp.Current;

					//
					// (1)
					//
					if (policyMapping > 0)
					{
						bool idp_found = false;
						IEnumerator nodes_i = policyNodes[i].GetEnumerator();

						while (nodes_i.MoveNext())
						{
							PkixPolicyNode node = (PkixPolicyNode)nodes_i.Current;
							if (node.ValidPolicy.Equals(id_p))
							{
								idp_found = true;
								node.ExpectedPolicies = (ISet)m_idp[id_p];
								break;
							}
						}

						if (!idp_found)
						{
							nodes_i = policyNodes[i].GetEnumerator();
							while (nodes_i.MoveNext())
							{
								PkixPolicyNode node = (PkixPolicyNode)nodes_i.Current;
								if (Rfc3280CertPathUtilities.ANY_POLICY.Equals(node.ValidPolicy))
								{
									ISet pq = null;
									Asn1Sequence policies = null;
									try
									{
										policies = (Asn1Sequence)PkixCertPathValidatorUtilities.GetExtensionValue(cert,
											X509Extensions.CertificatePolicies);
									}
									catch (Exception e)
									{
										throw new PkixCertPathValidatorException(
											"Certificate policies extension could not be decoded.", e, certPath, index);
									}

									foreach (Asn1Encodable ae in policies)
									{
										PolicyInformation pinfo = null;
										try
										{
											pinfo = PolicyInformation.GetInstance(ae.ToAsn1Object());
										}
										catch (Exception ex)
										{
											throw new PkixCertPathValidatorException(
												"Policy information could not be decoded.", ex, certPath, index);
										}
										if (Rfc3280CertPathUtilities.ANY_POLICY.Equals(pinfo.PolicyIdentifier.Id))
										{
											try
											{
												pq = PkixCertPathValidatorUtilities
													.GetQualifierSet(pinfo.PolicyQualifiers);
											}
											catch (PkixCertPathValidatorException ex)
											{
												throw new PkixCertPathValidatorException(
													"Policy qualifier info set could not be decoded.", ex, certPath,
													index);
											}
											break;
										}
									}
									bool ci = false;
									ISet critExtOids = cert.GetCriticalExtensionOids();
									if (critExtOids != null)
									{
										ci = critExtOids.Contains(X509Extensions.CertificatePolicies.Id);
									}

									PkixPolicyNode p_node = (PkixPolicyNode)node.Parent;
									if (Rfc3280CertPathUtilities.ANY_POLICY.Equals(p_node.ValidPolicy))
									{
										PkixPolicyNode c_node = new PkixPolicyNode(Platform.CreateArrayList(), i,
											(ISet)m_idp[id_p], p_node, pq, id_p, ci);
										p_node.AddChild(c_node);
										policyNodes[i].Add(c_node);
									}
									break;
								}
							}
						}

						//
						// (2)
						//
					}
					else if (policyMapping <= 0)
					{
                        foreach (PkixPolicyNode node in Platform.CreateArrayList(policyNodes[i]))
                        {
							if (node.ValidPolicy.Equals(id_p))
							{
								node.Parent.RemoveChild(node);

                                for (int k = i - 1; k >= 0; k--)
								{
                                    foreach (PkixPolicyNode node2 in Platform.CreateArrayList(policyNodes[k]))
									{
										if (!node2.HasChildren)
										{
											_validPolicyTree = PkixCertPathValidatorUtilities.RemovePolicyNode(
												_validPolicyTree, policyNodes, node2);

                                            if (_validPolicyTree == null)
												break;
										}
									}
								}
							}
						}
					}
				}
			}
			return _validPolicyTree;
		}

		internal static ISet[] ProcessCrlA1ii(
			DateTime		currentDate,
			PkixParameters	paramsPKIX,
			X509Certificate	cert,
			X509Crl			crl)
		{
			ISet deltaSet = new HashSet();
			X509CrlStoreSelector crlselect = new X509CrlStoreSelector();
			crlselect.CertificateChecking = cert;

			try
			{
				IList issuer = Platform.CreateArrayList();
				issuer.Add(crl.IssuerDN);
				crlselect.Issuers = issuer;
			}
			catch (IOException e)
			{
				throw new Exception("Cannot extract issuer from CRL." + e, e);
			}

			crlselect.CompleteCrlEnabled = true;
			ISet completeSet = CrlUtilities.FindCrls(crlselect, paramsPKIX, currentDate);

			if (paramsPKIX.IsUseDeltasEnabled)
			{
				// get delta CRL(s)
				try
				{
					deltaSet.AddAll(PkixCertPathValidatorUtilities.GetDeltaCrls(currentDate, paramsPKIX, crl));
				}
				catch (Exception e)
				{
					throw new Exception("Exception obtaining delta CRLs.", e);
				}
			}

			return new ISet[]{ completeSet, deltaSet };
		}

		internal static ISet ProcessCrlA1i(
			DateTime		currentDate,
			PkixParameters	paramsPKIX,
			X509Certificate	cert,
			X509Crl			crl)
		{
			ISet deltaSet = new HashSet();
			if (paramsPKIX.IsUseDeltasEnabled)
			{
				CrlDistPoint freshestCRL = null;
				try
				{
					freshestCRL = CrlDistPoint.GetInstance(
						PkixCertPathValidatorUtilities.GetExtensionValue(cert, X509Extensions.FreshestCrl));
				}
				catch (Exception e)
				{
					throw new Exception("Freshest CRL extension could not be decoded from certificate.", e);
				}

				if (freshestCRL == null)
				{
					try
					{
						freshestCRL = CrlDistPoint.GetInstance(PkixCertPathValidatorUtilities.GetExtensionValue(crl, X509Extensions.FreshestCrl));
					}
					catch (Exception e)
					{
						throw new Exception("Freshest CRL extension could not be decoded from CRL.", e);
					}
				}
				if (freshestCRL != null)
				{
					try
					{
						PkixCertPathValidatorUtilities.AddAdditionalStoresFromCrlDistributionPoint(freshestCRL, paramsPKIX);
					}
					catch (Exception e)
					{
						throw new Exception(
							"No new delta CRL locations could be added from Freshest CRL extension.", e);
					}
					// get delta CRL(s)
					try
					{
						deltaSet.AddAll(PkixCertPathValidatorUtilities.GetDeltaCrls(currentDate, paramsPKIX, crl));
					}
					catch (Exception e)
					{
						throw new Exception("Exception obtaining delta CRLs.", e);
					}
				}
			}
			return deltaSet;
		}

		internal static void ProcessCertF(
			PkixCertPath	certPath,
			int				index,
			PkixPolicyNode	validPolicyTree,
			int				explicitPolicy)
		{
			//
			// (f)
			//
			if (explicitPolicy <= 0 && validPolicyTree == null)
			{
				throw new PkixCertPathValidatorException(
					"No valid policy tree found when one expected.", null, certPath, index);
			}
		}

		internal static void ProcessCertA(
			PkixCertPath			certPath,
			PkixParameters			paramsPKIX,
			int						index,
			AsymmetricKeyParameter	workingPublicKey,
			X509Name				workingIssuerName,
			X509Certificate			sign)
		{
			IList certs = certPath.Certificates;
			X509Certificate cert = (X509Certificate)certs[index];
			//
			// (a) verify
			//
			try
			{
				// (a) (1)
				//
				cert.Verify(workingPublicKey);
			}
			catch (GeneralSecurityException e)
			{
				throw new PkixCertPathValidatorException("Could not validate certificate signature.", e, certPath, index);
			}

			try
			{
				// (a) (2)
				//
				cert.CheckValidity(PkixCertPathValidatorUtilities
					.GetValidCertDateFromValidityModel(paramsPKIX, certPath, index));
			}
			catch (CertificateExpiredException e)
			{
				throw new PkixCertPathValidatorException("Could not validate certificate: " + e.Message, e, certPath, index);
			}
			catch (CertificateNotYetValidException e)
			{
				throw new PkixCertPathValidatorException("Could not validate certificate: " + e.Message, e, certPath, index);
			}
			catch (Exception e)
			{
				throw new PkixCertPathValidatorException("Could not validate time of certificate.", e, certPath, index);
			}

			//
			// (a) (3)
			//
			if (paramsPKIX.IsRevocationEnabled)
			{
				try
				{
					CheckCrls(paramsPKIX, cert, PkixCertPathValidatorUtilities.GetValidCertDateFromValidityModel(paramsPKIX,
						certPath, index), sign, workingPublicKey, certs);
				}
				catch (Exception e)
				{
					Exception cause = e.InnerException;
					if (cause == null)
					{
						cause = e;
					}
					throw new PkixCertPathValidatorException(e.Message, cause, certPath, index);
				}
			}

			//
			// (a) (4) name chaining
			//
			X509Name issuer = PkixCertPathValidatorUtilities.GetIssuerPrincipal(cert);
			if (!issuer.Equivalent(workingIssuerName, true))
			{
				throw new PkixCertPathValidatorException("IssuerName(" + issuer
					+ ") does not match SubjectName(" + workingIssuerName + ") of signing certificate.", null,
					certPath, index);
			}
		}

		internal static int PrepareNextCertI1(
			PkixCertPath	certPath,
			int				index,
			int				explicitPolicy)
		{
			IList certs = certPath.Certificates;
			X509Certificate cert = (X509Certificate)certs[index];
			//
			// (i)
			//
			Asn1Sequence pc = null;
			try
			{
				pc = DerSequence.GetInstance(
					PkixCertPathValidatorUtilities.GetExtensionValue(cert, X509Extensions.PolicyConstraints));
			}
			catch (Exception e)
			{
				throw new PkixCertPathValidatorException(
					"Policy constraints extension cannot be decoded.", e, certPath, index);
			}

			int tmpInt;

			if (pc != null)
			{
				IEnumerator policyConstraints = pc.GetEnumerator();

				while (policyConstraints.MoveNext())
				{
					try
					{
						Asn1TaggedObject constraint = Asn1TaggedObject.GetInstance(policyConstraints.Current);
						if (constraint.TagNo == 0)
						{
							tmpInt = DerInteger.GetInstance(constraint, false).Value.IntValue;
							if (tmpInt < explicitPolicy)
							{
								return tmpInt;
							}
							break;
						}
					}
					catch (ArgumentException e)
					{
						throw new PkixCertPathValidatorException(
							"Policy constraints extension contents cannot be decoded.", e, certPath, index);
					}
				}
			}
			return explicitPolicy;
		}

		internal static int PrepareNextCertI2(
			PkixCertPath	certPath,
			int				index,
			int				policyMapping)
			//throws CertPathValidatorException
		{
			IList certs = certPath.Certificates;
			X509Certificate cert = (X509Certificate)certs[index];

			//
			// (i)
			//
			Asn1Sequence pc = null;
			try
			{
				pc = DerSequence.GetInstance(
					PkixCertPathValidatorUtilities.GetExtensionValue(cert, X509Extensions.PolicyConstraints));
			}
			catch (Exception e)
			{
				throw new PkixCertPathValidatorException(
					"Policy constraints extension cannot be decoded.", e, certPath, index);
			}

			int tmpInt;

			if (pc != null)
			{
				IEnumerator policyConstraints = pc.GetEnumerator();

				while (policyConstraints.MoveNext())
				{
					try
					{
						Asn1TaggedObject constraint = Asn1TaggedObject.GetInstance(policyConstraints.Current);
						if (constraint.TagNo == 1)
						{
							tmpInt = DerInteger.GetInstance(constraint, false).Value.IntValue;
							if (tmpInt < policyMapping)
							{
								return tmpInt;
							}
							break;
						}
					}
					catch (ArgumentException e)
					{
						throw new PkixCertPathValidatorException(
							"Policy constraints extension contents cannot be decoded.", e, certPath, index);
					}
				}
			}
			return policyMapping;
		}

		internal static void PrepareNextCertG(
			PkixCertPath				certPath,
			int							index,
			PkixNameConstraintValidator	nameConstraintValidator)
			//throws CertPathValidatorException
		{
			IList certs = certPath.Certificates;
			X509Certificate cert = (X509Certificate)certs[index];

			//
			// (g) handle the name constraints extension
			//
			NameConstraints nc = null;
			try
			{
				Asn1Sequence ncSeq = DerSequence.GetInstance(
					PkixCertPathValidatorUtilities.GetExtensionValue(cert, X509Extensions.NameConstraints));
				if (ncSeq != null)
				{
					nc = new NameConstraints(ncSeq);
				}
			}
			catch (Exception e)
			{
				throw new PkixCertPathValidatorException(
					"Name constraints extension could not be decoded.", e, certPath, index);
			}
			if (nc != null)
			{
				//
				// (g) (1) permitted subtrees
				//
				Asn1Sequence permitted = nc.PermittedSubtrees;
				if (permitted != null)
				{
					try
					{
						nameConstraintValidator.IntersectPermittedSubtree(permitted);
					}
					catch (Exception ex)
					{
						throw new PkixCertPathValidatorException(
							"Permitted subtrees cannot be build from name constraints extension.", ex, certPath, index);
					}
				}

				//
				// (g) (2) excluded subtrees
				//
				Asn1Sequence excluded = nc.ExcludedSubtrees;
				if (excluded != null)
				{
					IEnumerator e = excluded.GetEnumerator();
					try
					{
						while (e.MoveNext())
						{
							GeneralSubtree subtree = GeneralSubtree.GetInstance(e.Current);
							nameConstraintValidator.AddExcludedSubtree(subtree);
						}
					}
					catch (Exception ex)
					{
						throw new PkixCertPathValidatorException(
							"Excluded subtrees cannot be build from name constraints extension.", ex, certPath, index);
					}
				}
			}
		}

		internal static int PrepareNextCertJ(
			PkixCertPath	certPath,
			int				index,
			int				inhibitAnyPolicy)
			//throws CertPathValidatorException
		{
			IList certs = certPath.Certificates;
			X509Certificate cert = (X509Certificate)certs[index];

			//
			// (j)
			//
			DerInteger iap = null;
			try
			{
				iap = DerInteger.GetInstance(
					PkixCertPathValidatorUtilities.GetExtensionValue(cert, X509Extensions.InhibitAnyPolicy));
			}
			catch (Exception e)
			{
				throw new PkixCertPathValidatorException(
					"Inhibit any-policy extension cannot be decoded.", e, certPath, index);
			}

			if (iap != null)
			{
				int _inhibitAnyPolicy = iap.Value.IntValue;

				if (_inhibitAnyPolicy < inhibitAnyPolicy)
					return _inhibitAnyPolicy;
			}
			return inhibitAnyPolicy;
		}

		internal static void PrepareNextCertK(
			PkixCertPath	certPath,
			int				index)
			//throws CertPathValidatorException
		{
			IList certs = certPath.Certificates;
			X509Certificate cert = (X509Certificate)certs[index];
			//
			// (k)
			//
			BasicConstraints bc = null;
			try
			{
				bc = BasicConstraints.GetInstance(
					PkixCertPathValidatorUtilities.GetExtensionValue(cert, X509Extensions.BasicConstraints));
			}
			catch (Exception e)
			{
				throw new PkixCertPathValidatorException("Basic constraints extension cannot be decoded.", e, certPath,
					index);
			}
			if (bc != null)
			{
				if (!(bc.IsCA()))
					throw new PkixCertPathValidatorException("Not a CA certificate");
			}
			else
			{
				throw new PkixCertPathValidatorException("Intermediate certificate lacks BasicConstraints");
			}
		}

		internal static int PrepareNextCertL(
			PkixCertPath	certPath,
			int				index,
			int				maxPathLength)
			//throws CertPathValidatorException
		{
			IList certs = certPath.Certificates;
			X509Certificate cert = (X509Certificate)certs[index];
			//
			// (l)
			//
			if (!PkixCertPathValidatorUtilities.IsSelfIssued(cert))
			{
				if (maxPathLength <= 0)
				{
					throw new PkixCertPathValidatorException("Max path length not greater than zero", null, certPath, index);
				}

				return maxPathLength - 1;
			}
			return maxPathLength;
		}

		internal static int PrepareNextCertM(
			PkixCertPath	certPath,
			int				index,
			int				maxPathLength)
			//throws CertPathValidatorException
		{
			IList certs = certPath.Certificates;
			X509Certificate cert = (X509Certificate)certs[index];

			//
			// (m)
			//
			BasicConstraints bc = null;
			try
			{
				bc = BasicConstraints.GetInstance(
					PkixCertPathValidatorUtilities.GetExtensionValue(cert, X509Extensions.BasicConstraints));
			}
			catch (Exception e)
			{
				throw new PkixCertPathValidatorException("Basic constraints extension cannot be decoded.", e, certPath,
					index);
			}
			if (bc != null)
			{
				BigInteger _pathLengthConstraint = bc.PathLenConstraint;

				if (_pathLengthConstraint != null)
				{
					int _plc = _pathLengthConstraint.IntValue;

					if (_plc < maxPathLength)
					{
						return _plc;
					}
				}
			}
			return maxPathLength;
		}

		internal static void PrepareNextCertN(
			PkixCertPath	certPath,
			int				index)
			//throws CertPathValidatorException
		{
			IList certs = certPath.Certificates;
			X509Certificate cert = (X509Certificate)certs[index];

			//
			// (n)
			//
			bool[] _usage = cert.GetKeyUsage();

			if ((_usage != null) && !_usage[Rfc3280CertPathUtilities.KEY_CERT_SIGN])
			{
				throw new PkixCertPathValidatorException(
					"Issuer certificate keyusage extension is critical and does not permit key signing.", null,
					certPath, index);
			}
		}

		internal static void PrepareNextCertO(
			PkixCertPath	certPath,
			int				index,
			ISet			criticalExtensions,
			IList			pathCheckers)
			//throws CertPathValidatorException
		{
			IList certs = certPath.Certificates;
			X509Certificate cert = (X509Certificate)certs[index];

			//
			// (o)
			//
			IEnumerator tmpIter = pathCheckers.GetEnumerator();
			while (tmpIter.MoveNext())
			{
				try
				{
					((PkixCertPathChecker)tmpIter.Current).Check(cert, criticalExtensions);
				}
				catch (PkixCertPathValidatorException e)
				{
					throw new PkixCertPathValidatorException(e.Message, e.InnerException, certPath, index);
				}
			}
			if (!criticalExtensions.IsEmpty)
			{
				throw new PkixCertPathValidatorException("Certificate has unsupported critical extension.", null, certPath,
					index);
			}
		}

		internal static int PrepareNextCertH1(
			PkixCertPath	certPath,
			int				index,
			int				explicitPolicy)
		{
			IList certs = certPath.Certificates;
			X509Certificate cert = (X509Certificate)certs[index];

			//
			// (h)
			//
			if (!PkixCertPathValidatorUtilities.IsSelfIssued(cert))
			{
				//
				// (1)
				//
				if (explicitPolicy != 0)
					return explicitPolicy - 1;
			}
			return explicitPolicy;
		}

		internal static int PrepareNextCertH2(
			PkixCertPath	certPath,
			int				index,
			int				policyMapping)
		{
			IList certs = certPath.Certificates;
			X509Certificate cert = (X509Certificate)certs[index];

			//
			// (h)
			//
			if (!PkixCertPathValidatorUtilities.IsSelfIssued(cert))
			{
				//
				// (2)
				//
				if (policyMapping != 0)
					return policyMapping - 1;
			}
			return policyMapping;
		}


		internal static int PrepareNextCertH3(
			PkixCertPath	certPath,
			int				index,
			int				inhibitAnyPolicy)
		{
			IList certs = certPath.Certificates;
			X509Certificate cert = (X509Certificate)certs[index];

			//
			// (h)
			//
			if (!PkixCertPathValidatorUtilities.IsSelfIssued(cert))
			{
				//
				// (3)
				//
				if (inhibitAnyPolicy != 0)
					return inhibitAnyPolicy - 1;
			}
			return inhibitAnyPolicy;
		}

		internal static int WrapupCertA(
			int				explicitPolicy,
			X509Certificate	cert)
		{
			//
			// (a)
			//
			if (!PkixCertPathValidatorUtilities.IsSelfIssued(cert) && (explicitPolicy != 0))
			{
				explicitPolicy--;
			}
			return explicitPolicy;
		}

		internal static int WrapupCertB(
			PkixCertPath	certPath,
			int				index,
			int				explicitPolicy)
			//throws CertPathValidatorException
		{
			IList certs = certPath.Certificates;
			X509Certificate cert = (X509Certificate)certs[index];

			//
			// (b)
			//
			int tmpInt;
			Asn1Sequence pc = null;
			try
			{
				pc = DerSequence.GetInstance(
					PkixCertPathValidatorUtilities.GetExtensionValue(cert, X509Extensions.PolicyConstraints));
			}
			catch (Exception e)
			{
				throw new PkixCertPathValidatorException("Policy constraints could not be decoded.", e, certPath, index);
			}

			if (pc != null)
			{
				IEnumerator policyConstraints = pc.GetEnumerator();

				while (policyConstraints.MoveNext())
				{
					Asn1TaggedObject constraint = (Asn1TaggedObject)policyConstraints.Current;
					switch (constraint.TagNo)
					{
						case 0:
							try
							{
								tmpInt = DerInteger.GetInstance(constraint, false).Value.IntValue;
							}
							catch (Exception e)
							{
								throw new PkixCertPathValidatorException(
									"Policy constraints requireExplicitPolicy field could not be decoded.", e, certPath,
									index);
							}
							if (tmpInt == 0)
							{
								return 0;
							}
							break;
					}
				}
			}
			return explicitPolicy;
		}

		internal static void WrapupCertF(
			PkixCertPath	certPath,
			int				index,
			IList			pathCheckers,
			ISet			criticalExtensions)
			//throws CertPathValidatorException
		{
			IList certs = certPath.Certificates;
			X509Certificate cert = (X509Certificate)certs[index];
			IEnumerator tmpIter = pathCheckers.GetEnumerator();

			while (tmpIter.MoveNext())
			{
				try
				{
					((PkixCertPathChecker)tmpIter.Current).Check(cert, criticalExtensions);
				}
				catch (PkixCertPathValidatorException e)
				{
					throw new PkixCertPathValidatorException("Additional certificate path checker failed.", e, certPath,
						index);
				}
			}

			if (!criticalExtensions.IsEmpty)
			{
				throw new PkixCertPathValidatorException("Certificate has unsupported critical extension",
					null, certPath, index);
			}
		}

		internal static PkixPolicyNode WrapupCertG(
			PkixCertPath	certPath,
			PkixParameters	paramsPKIX,
			ISet			userInitialPolicySet,
			int				index,
			IList[]			policyNodes,
			PkixPolicyNode	validPolicyTree,
			ISet			acceptablePolicies)
		{
			int n = certPath.Certificates.Count;

			//
			// (g)
			//
			PkixPolicyNode intersection;

			//
			// (g) (i)
			//
			if (validPolicyTree == null)
			{
				if (paramsPKIX.IsExplicitPolicyRequired)
				{
					throw new PkixCertPathValidatorException(
						"Explicit policy requested but none available.", null, certPath, index);
				}
				intersection = null;
			}
			else if (PkixCertPathValidatorUtilities.IsAnyPolicy(userInitialPolicySet)) // (g)
				// (ii)
			{
				if (paramsPKIX.IsExplicitPolicyRequired)
				{
					if (acceptablePolicies.IsEmpty)
					{
						throw new PkixCertPathValidatorException(
							"Explicit policy requested but none available.", null, certPath, index);
					}
					else
					{
						ISet _validPolicyNodeSet = new HashSet();

						for (int j = 0; j < policyNodes.Length; j++)
						{
							IList _nodeDepth = policyNodes[j];

							for (int k = 0; k < _nodeDepth.Count; k++)
							{
								PkixPolicyNode _node = (PkixPolicyNode)_nodeDepth[k];

								if (Rfc3280CertPathUtilities.ANY_POLICY.Equals(_node.ValidPolicy))
								{
									foreach (object o in _node.Children)
									{
										_validPolicyNodeSet.Add(o);
									}
								}
							}
						}

						foreach (PkixPolicyNode _node in _validPolicyNodeSet)
						{
							string _validPolicy = _node.ValidPolicy;

							if (!acceptablePolicies.Contains(_validPolicy))
							{
								// TODO?
								// validPolicyTree =
								// removePolicyNode(validPolicyTree, policyNodes,
								// _node);
							}
						}
						if (validPolicyTree != null)
						{
							for (int j = (n - 1); j >= 0; j--)
							{
								IList nodes = policyNodes[j];

								for (int k = 0; k < nodes.Count; k++)
								{
									PkixPolicyNode node = (PkixPolicyNode)nodes[k];
									if (!node.HasChildren)
									{
										validPolicyTree = PkixCertPathValidatorUtilities.RemovePolicyNode(validPolicyTree,
											policyNodes, node);
									}
								}
							}
						}
					}
				}

				intersection = validPolicyTree;
			}
			else
			{
				//
				// (g) (iii)
				//
				// This implementation is not exactly same as the one described in
				// RFC3280.
				// However, as far as the validation result is concerned, both
				// produce
				// adequate result. The only difference is whether AnyPolicy is
				// remain
				// in the policy tree or not.
				//
				// (g) (iii) 1
				//
				ISet _validPolicyNodeSet = new HashSet();

				for (int j = 0; j < policyNodes.Length; j++)
				{
					IList _nodeDepth = policyNodes[j];

					for (int k = 0; k < _nodeDepth.Count; k++)
					{
						PkixPolicyNode _node = (PkixPolicyNode)_nodeDepth[k];

						if (Rfc3280CertPathUtilities.ANY_POLICY.Equals(_node.ValidPolicy))
						{
							foreach (PkixPolicyNode _c_node in _node.Children)
							{
								if (!Rfc3280CertPathUtilities.ANY_POLICY.Equals(_c_node.ValidPolicy))
								{
									_validPolicyNodeSet.Add(_c_node);
								}
							}
						}
					}
				}

				//
				// (g) (iii) 2
				//
				IEnumerator _vpnsIter = _validPolicyNodeSet.GetEnumerator();
				while (_vpnsIter.MoveNext())
				{
					PkixPolicyNode _node = (PkixPolicyNode)_vpnsIter.Current;
					string _validPolicy = _node.ValidPolicy;

					if (!userInitialPolicySet.Contains(_validPolicy))
					{
						validPolicyTree = PkixCertPathValidatorUtilities.RemovePolicyNode(validPolicyTree, policyNodes, _node);
					}
				}

				//
				// (g) (iii) 4
				//
				if (validPolicyTree != null)
				{
					for (int j = (n - 1); j >= 0; j--)
					{
						IList nodes = policyNodes[j];

						for (int k = 0; k < nodes.Count; k++)
						{
							PkixPolicyNode node = (PkixPolicyNode)nodes[k];
							if (!node.HasChildren)
							{
								validPolicyTree = PkixCertPathValidatorUtilities.RemovePolicyNode(validPolicyTree, policyNodes,
									node);
							}
						}
					}
				}

				intersection = validPolicyTree;
			}
			return intersection;
		}

		/**
		* If use-deltas is set, verify the issuer and scope of the delta CRL.
		*
		* @param deltaCRL    The delta CRL.
		* @param completeCRL The complete CRL.
		* @param pkixParams  The PKIX paramaters.
		* @throws AnnotatedException if an exception occurs.
		*/
		internal static void ProcessCrlC(
			X509Crl			deltaCRL,
			X509Crl			completeCRL,
			PkixParameters	pkixParams)
		{
			if (deltaCRL == null)
				return;

			IssuingDistributionPoint completeidp = null;
			try
			{
				completeidp = IssuingDistributionPoint.GetInstance(
					PkixCertPathValidatorUtilities.GetExtensionValue(completeCRL, X509Extensions.IssuingDistributionPoint));
			}
			catch (Exception e)
			{
				throw new Exception("000 Issuing distribution point extension could not be decoded.", e);
			}

			if (pkixParams.IsUseDeltasEnabled)
			{
				// (c) (1)
				if (!deltaCRL.IssuerDN.Equivalent(completeCRL.IssuerDN, true))
					throw new Exception("Complete CRL issuer does not match delta CRL issuer.");

				// (c) (2)
				IssuingDistributionPoint deltaidp = null;
				try
				{
					deltaidp = IssuingDistributionPoint.GetInstance(
						PkixCertPathValidatorUtilities.GetExtensionValue(deltaCRL, X509Extensions.IssuingDistributionPoint));
				}
				catch (Exception e)
				{
					throw new Exception(
						"Issuing distribution point extension from delta CRL could not be decoded.", e);
				}

				if (!Platform.Equals(completeidp, deltaidp))
				{
					throw new Exception(
						"Issuing distribution point extension from delta CRL and complete CRL does not match.");
				}

				// (c) (3)
				Asn1Object completeKeyIdentifier = null;
				try
				{
					completeKeyIdentifier = PkixCertPathValidatorUtilities.GetExtensionValue(
						completeCRL, X509Extensions.AuthorityKeyIdentifier);
				}
				catch (Exception e)
				{
					throw new Exception(
						"Authority key identifier extension could not be extracted from complete CRL.", e);
				}

				Asn1Object deltaKeyIdentifier = null;
				try
				{
					deltaKeyIdentifier = PkixCertPathValidatorUtilities.GetExtensionValue(
						deltaCRL, X509Extensions.AuthorityKeyIdentifier);
				}
				catch (Exception e)
				{
					throw new Exception(
						"Authority key identifier extension could not be extracted from delta CRL.", e);
				}

				if (completeKeyIdentifier == null)
					throw new Exception("CRL authority key identifier is null.");

				if (deltaKeyIdentifier == null)
					throw new Exception("Delta CRL authority key identifier is null.");

				if (!completeKeyIdentifier.Equals(deltaKeyIdentifier))
				{
					throw new Exception(
						"Delta CRL authority key identifier does not match complete CRL authority key identifier.");
				}
			}
		}

		internal static void ProcessCrlI(
			DateTime		validDate,
			X509Crl			deltacrl,
			object			cert,
			CertStatus		certStatus,
			PkixParameters	pkixParams)
		{
			if (pkixParams.IsUseDeltasEnabled && deltacrl != null)
			{
				PkixCertPathValidatorUtilities.GetCertStatus(validDate, deltacrl, cert, certStatus);
			}
		}

		internal static void ProcessCrlJ(
			DateTime	validDate,
			X509Crl		completecrl,
			object		cert,
			CertStatus	certStatus)
		{
			if (certStatus.Status == CertStatus.Unrevoked)
			{
				PkixCertPathValidatorUtilities.GetCertStatus(validDate, completecrl, cert, certStatus);
			}
		}

		internal static PkixPolicyNode ProcessCertE(
			PkixCertPath	certPath,
			int				index,
			PkixPolicyNode	validPolicyTree)
		{
			IList certs = certPath.Certificates;
			X509Certificate cert = (X509Certificate)certs[index];

			//
			// (e)
			//
			Asn1Sequence certPolicies = null;
			try
			{
				certPolicies = DerSequence.GetInstance(
					PkixCertPathValidatorUtilities.GetExtensionValue(cert, X509Extensions.CertificatePolicies));
			}
			catch (Exception e)
			{
				throw new PkixCertPathValidatorException("Could not read certificate policies extension from certificate.",
					e, certPath, index);
			}
			if (certPolicies == null)
			{
				validPolicyTree = null;
			}
			return validPolicyTree;
		}

		internal static readonly string[] CrlReasons = new string[]
		{
			"unspecified",
			"keyCompromise",
			"cACompromise",
			"affiliationChanged",
			"superseded",
			"cessationOfOperation",
			"certificateHold",
			"unknown",
			"removeFromCRL",
			"privilegeWithdrawn",
			"aACompromise"
		};
	}
}
