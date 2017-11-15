using System;
using System.Collections;

using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;
using Org.BouncyCastle.Utilities.Date;
using Org.BouncyCastle.X509.Store;

namespace Org.BouncyCastle.Pkix
{
	/// <summary>
	/// Summary description for PkixParameters.
	/// </summary>
	public class PkixParameters
//		: ICertPathParameters
	{
		/**
		* This is the default PKIX validity model. Actually there are two variants
		* of this: The PKIX model and the modified PKIX model. The PKIX model
		* verifies that all involved certificates must have been valid at the
		* current time. The modified PKIX model verifies that all involved
		* certificates were valid at the signing time. Both are indirectly choosen
		* with the {@link PKIXParameters#setDate(java.util.Date)} method, so this
		* methods sets the Date when <em>all</em> certificates must have been
		* valid.
		*/
		public const int PkixValidityModel = 0;

		/**
		* This model uses the following validity model. Each certificate must have
		* been valid at the moment where is was used. That means the end
		* certificate must have been valid at the time the signature was done. The
		* CA certificate which signed the end certificate must have been valid,
		* when the end certificate was signed. The CA (or Root CA) certificate must
		* have been valid, when the CA certificate was signed and so on. So the
		* {@link PKIXParameters#setDate(java.util.Date)} method sets the time, when
		* the <em>end certificate</em> must have been valid. <p/> It is used e.g.
		* in the German signature law.
		*/
		public const int ChainValidityModel = 1;

		private ISet trustAnchors;
		private DateTimeObject date;
		private IList certPathCheckers;
		private bool revocationEnabled = true;
		private ISet initialPolicies;
		//private bool checkOnlyEECertificateCrl = false;
		private bool explicitPolicyRequired = false;
		private bool anyPolicyInhibited = false;
		private bool policyMappingInhibited = false;
		private bool policyQualifiersRejected = true;
		private IX509Selector certSelector;
		private IList stores;
		private IX509Selector selector;
		private bool additionalLocationsEnabled;
		private IList additionalStores;
		private ISet trustedACIssuers;
		private ISet necessaryACAttributes;
		private ISet prohibitedACAttributes;
		private ISet attrCertCheckers;
		private int validityModel = PkixValidityModel;
		private bool useDeltas = false;

		/**
		 * Creates an instance of PKIXParameters with the specified Set of
		 * most-trusted CAs. Each element of the set is a TrustAnchor.<br />
		 * <br />
		 * Note that the Set is copied to protect against subsequent modifications.
		 *
		 * @param trustAnchors
		 *            a Set of TrustAnchors
		 *
		 * @exception InvalidAlgorithmParameterException
		 *                if the specified Set is empty
		 *                <code>(trustAnchors.isEmpty() == true)</code>
		 * @exception NullPointerException
		 *                if the specified Set is <code>null</code>
		 * @exception ClassCastException
		 *                if any of the elements in the Set are not of type
		 *                <code>java.security.cert.TrustAnchor</code>
		 */
		public PkixParameters(
			ISet trustAnchors)
		{
			SetTrustAnchors(trustAnchors);

			this.initialPolicies = new HashSet();
			this.certPathCheckers = Platform.CreateArrayList();
            this.stores = Platform.CreateArrayList();
			this.additionalStores = Platform.CreateArrayList();
			this.trustedACIssuers = new HashSet();
			this.necessaryACAttributes = new HashSet();
			this.prohibitedACAttributes = new HashSet();
			this.attrCertCheckers = new HashSet();
		}

//		// TODO implement for other keystores (see Java build)?
//		/**
//		 * Creates an instance of <code>PKIXParameters</code> that
//		 * populates the set of most-trusted CAs from the trusted
//		 * certificate entries contained in the specified <code>KeyStore</code>.
//		 * Only keystore entries that contain trusted <code>X509Certificates</code>
//		 * are considered; all other certificate types are ignored.
//		 *
//		 * @param keystore a <code>KeyStore</code> from which the set of
//		 * most-trusted CAs will be populated
//		 * @throws KeyStoreException if the keystore has not been initialized
//		 * @throws InvalidAlgorithmParameterException if the keystore does
//		 * not contain at least one trusted certificate entry
//		 * @throws NullPointerException if the keystore is <code>null</code>
//		 */
//		public PkixParameters(
//			Pkcs12Store keystore)
////			throws KeyStoreException, InvalidAlgorithmParameterException
//		{
//			if (keystore == null)
//				throw new ArgumentNullException("keystore");
//			ISet trustAnchors = new HashSet();
//			foreach (string alias in keystore.Aliases)
//			{
//				if (keystore.IsCertificateEntry(alias))
//				{
//					X509CertificateEntry x509Entry = keystore.GetCertificate(alias);
//					trustAnchors.Add(new TrustAnchor(x509Entry.Certificate, null));
//				}
//			}
//			SetTrustAnchors(trustAnchors);
//
//			this.initialPolicies = new HashSet();
//			this.certPathCheckers = new ArrayList();
//			this.stores = new ArrayList();
//			this.additionalStores = new ArrayList();
//			this.trustedACIssuers = new HashSet();
//			this.necessaryACAttributes = new HashSet();
//			this.prohibitedACAttributes = new HashSet();
//			this.attrCertCheckers = new HashSet();
//		}

		public virtual bool IsRevocationEnabled
		{
			get { return revocationEnabled; }
			set { revocationEnabled = value; }
		}

		public virtual bool IsExplicitPolicyRequired
		{
			get { return explicitPolicyRequired; }
			set { this.explicitPolicyRequired = value; }
		}

		public virtual bool IsAnyPolicyInhibited
		{
			get { return anyPolicyInhibited; }
			set { this.anyPolicyInhibited = value; }
		}

		public virtual bool IsPolicyMappingInhibited
		{
			get { return policyMappingInhibited; }
			set { this.policyMappingInhibited = value; }
		}

		public virtual bool IsPolicyQualifiersRejected
		{
			get { return policyQualifiersRejected; }
			set { this.policyQualifiersRejected = value; }
		}

		//public bool IsCheckOnlyEECertificateCrl
		//{
		//	get { return this.checkOnlyEECertificateCrl; }
		//	set { this.checkOnlyEECertificateCrl = value; }
		//}

		public virtual DateTimeObject Date
		{
			get { return this.date; }
			set { this.date = value; }
		}

		// Returns a Set of the most-trusted CAs.
		public virtual ISet GetTrustAnchors()
		{
			return new HashSet(this.trustAnchors);
		}

		// Sets the set of most-trusted CAs.
		// Set is copied to protect against subsequent modifications.
		public virtual void SetTrustAnchors(
			ISet tas)
		{
			if (tas == null)
				throw new ArgumentNullException("value");
			if (tas.IsEmpty)
				throw new ArgumentException("non-empty set required", "value");

			// Explicit copy to enforce type-safety
			this.trustAnchors = new HashSet();
			foreach (TrustAnchor ta in tas)
			{
				if (ta != null)
				{
					trustAnchors.Add(ta);
				}
			}
		}

		/**
		* Returns the required constraints on the target certificate. The
		* constraints are returned as an instance of CertSelector. If
		* <code>null</code>, no constraints are defined.<br />
		* <br />
		* Note that the CertSelector returned is cloned to protect against
		* subsequent modifications.
		*
		* @return a CertSelector specifying the constraints on the target
		*         certificate (or <code>null</code>)
		*
		* @see #setTargetCertConstraints(CertSelector)
		*/
		public virtual X509CertStoreSelector GetTargetCertConstraints()
		{
			if (certSelector == null)
			{
				return null;
			}

			return (X509CertStoreSelector)certSelector.Clone();
		}

		/**
		 * Sets the required constraints on the target certificate. The constraints
		 * are specified as an instance of CertSelector. If null, no constraints are
		 * defined.<br />
		 * <br />
		 * Note that the CertSelector specified is cloned to protect against
		 * subsequent modifications.
		 *
		 * @param selector
		 *            a CertSelector specifying the constraints on the target
		 *            certificate (or <code>null</code>)
		 *
		 * @see #getTargetCertConstraints()
		 */
		public virtual void SetTargetCertConstraints(
			IX509Selector selector)
		{
			if (selector == null)
			{
				certSelector = null;
			}
			else
			{
				certSelector = (IX509Selector)selector.Clone();
			}
		}

		/**
		* Returns an immutable Set of initial policy identifiers (OID strings),
		* indicating that any one of these policies would be acceptable to the
		* certificate user for the purposes of certification path processing. The
		* default return value is an empty <code>Set</code>, which is
		* interpreted as meaning that any policy would be acceptable.
		*
		* @return an immutable <code>Set</code> of initial policy OIDs in String
		*         format, or an empty <code>Set</code> (implying any policy is
		*         acceptable). Never returns <code>null</code>.
		*
		* @see #setInitialPolicies(java.util.Set)
		*/
		public virtual ISet GetInitialPolicies()
		{
			ISet returnSet = initialPolicies;

			// TODO Can it really be null?
			if (initialPolicies == null)
			{
				returnSet = new HashSet();
			}

			return new HashSet(returnSet);
		}

		/**
		* Sets the <code>Set</code> of initial policy identifiers (OID strings),
		* indicating that any one of these policies would be acceptable to the
		* certificate user for the purposes of certification path processing. By
		* default, any policy is acceptable (i.e. all policies), so a user that
		* wants to allow any policy as acceptable does not need to call this
		* method, or can call it with an empty <code>Set</code> (or
		* <code>null</code>).<br />
		* <br />
		* Note that the Set is copied to protect against subsequent modifications.<br />
		* <br />
		*
		* @param initialPolicies
		*            a Set of initial policy OIDs in String format (or
		*            <code>null</code>)
		*
		* @exception ClassCastException
		*                if any of the elements in the set are not of type String
		*
		* @see #getInitialPolicies()
		*/
		public virtual void SetInitialPolicies(
			ISet initialPolicies)
		{
			this.initialPolicies = new HashSet();
			if (initialPolicies != null)
			{
				foreach (string obj in initialPolicies)
				{
					if (obj != null)
					{
						this.initialPolicies.Add(obj);
					}
				}
			}
		}

		/**
		* Sets a <code>List</code> of additional certification path checkers. If
		* the specified List contains an object that is not a PKIXCertPathChecker,
		* it is ignored.<br />
		* <br />
		* Each <code>PKIXCertPathChecker</code> specified implements additional
		* checks on a certificate. Typically, these are checks to process and
		* verify private extensions contained in certificates. Each
		* <code>PKIXCertPathChecker</code> should be instantiated with any
		* initialization parameters needed to execute the check.<br />
		* <br />
		* This method allows sophisticated applications to extend a PKIX
		* <code>CertPathValidator</code> or <code>CertPathBuilder</code>. Each
		* of the specified PKIXCertPathCheckers will be called, in turn, by a PKIX
		* <code>CertPathValidator</code> or <code>CertPathBuilder</code> for
		* each certificate processed or validated.<br />
		* <br />
		* Regardless of whether these additional PKIXCertPathCheckers are set, a
		* PKIX <code>CertPathValidator</code> or <code>CertPathBuilder</code>
		* must perform all of the required PKIX checks on each certificate. The one
		* exception to this rule is if the RevocationEnabled flag is set to false
		* (see the {@link #setRevocationEnabled(boolean) setRevocationEnabled}
		* method).<br />
		* <br />
		* Note that the List supplied here is copied and each PKIXCertPathChecker
		* in the list is cloned to protect against subsequent modifications.
		*
		* @param checkers
		*            a List of PKIXCertPathCheckers. May be null, in which case no
		*            additional checkers will be used.
		* @exception ClassCastException
		*                if any of the elements in the list are not of type
		*                <code>java.security.cert.PKIXCertPathChecker</code>
		* @see #getCertPathCheckers()
		*/
		public virtual void SetCertPathCheckers(IList checkers)
		{
            certPathCheckers = Platform.CreateArrayList();
			if (checkers != null)
			{
				foreach (PkixCertPathChecker obj in checkers)
				{
					certPathCheckers.Add(obj.Clone());
				}
			}
		}

		/**
		 * Returns the List of certification path checkers. Each PKIXCertPathChecker
		 * in the returned IList is cloned to protect against subsequent modifications.
		 *
		 * @return an immutable List of PKIXCertPathCheckers (may be empty, but not
		 *         <code>null</code>)
		 *
		 * @see #setCertPathCheckers(java.util.List)
		 */
		public virtual IList GetCertPathCheckers()
		{
			IList checkers = Platform.CreateArrayList();
			foreach (PkixCertPathChecker obj in certPathCheckers)
			{
				checkers.Add(obj.Clone());
			}
			return checkers;
		}

		/**
		 * Adds a <code>PKIXCertPathChecker</code> to the list of certification
		 * path checkers. See the {@link #setCertPathCheckers setCertPathCheckers}
		 * method for more details.
		 * <p>
		 * Note that the <code>PKIXCertPathChecker</code> is cloned to protect
		 * against subsequent modifications.</p>
		 *
		 * @param checker a <code>PKIXCertPathChecker</code> to add to the list of
		 * checks. If <code>null</code>, the checker is ignored (not added to list).
		 */
		public virtual void AddCertPathChecker(
			PkixCertPathChecker checker)
		{
			if (checker != null)
			{
				certPathCheckers.Add(checker.Clone());
			}
		}

		public virtual object Clone()
		{
			// FIXME Check this whole method against the Java implementation!

			PkixParameters parameters = new PkixParameters(GetTrustAnchors());
			parameters.SetParams(this);
			return parameters;


//			PkixParameters obj = new PkixParameters(new HashSet());
////			(PkixParameters) this.MemberwiseClone();
//			obj.x509Stores = new ArrayList(x509Stores);
//			obj.certPathCheckers = new ArrayList(certPathCheckers);
//
//			//Iterator iter = certPathCheckers.iterator();
//			//obj.certPathCheckers = new ArrayList();
//			//while (iter.hasNext())
//			//{
//			//	obj.certPathCheckers.add(((PKIXCertPathChecker)iter.next())
//			//		.clone());
//			//}
//			//if (initialPolicies != null)
//			//{
//			//	obj.initialPolicies = new HashSet(initialPolicies);
//			//}
////			if (trustAnchors != null)
////			{
////				obj.trustAnchors = new HashSet(trustAnchors);
////			}
////			if (certSelector != null)
////			{
////				obj.certSelector = (X509CertStoreSelector) certSelector.Clone();
////			}
//			return obj;
		}

		/**
		* Method to support <code>Clone()</code> under J2ME.
		* <code>super.Clone()</code> does not exist and fields are not copied.
		*
		* @param params Parameters to set. If this are
		*            <code>ExtendedPkixParameters</code> they are copied to.
		*/
		protected virtual void SetParams(
			PkixParameters parameters)
		{
			Date = parameters.Date;
			SetCertPathCheckers(parameters.GetCertPathCheckers());
			IsAnyPolicyInhibited = parameters.IsAnyPolicyInhibited;
			IsExplicitPolicyRequired = parameters.IsExplicitPolicyRequired;
			IsPolicyMappingInhibited = parameters.IsPolicyMappingInhibited;
			IsRevocationEnabled = parameters.IsRevocationEnabled;
			SetInitialPolicies(parameters.GetInitialPolicies());
			IsPolicyQualifiersRejected = parameters.IsPolicyQualifiersRejected;
			SetTargetCertConstraints(parameters.GetTargetCertConstraints());
			SetTrustAnchors(parameters.GetTrustAnchors());

			validityModel = parameters.validityModel;
			useDeltas = parameters.useDeltas;
			additionalLocationsEnabled = parameters.additionalLocationsEnabled;
			selector = parameters.selector == null ? null
				: (IX509Selector) parameters.selector.Clone();
			stores = Platform.CreateArrayList(parameters.stores);
            additionalStores = Platform.CreateArrayList(parameters.additionalStores);
			trustedACIssuers = new HashSet(parameters.trustedACIssuers);
			prohibitedACAttributes = new HashSet(parameters.prohibitedACAttributes);
			necessaryACAttributes = new HashSet(parameters.necessaryACAttributes);
			attrCertCheckers = new HashSet(parameters.attrCertCheckers);
		}

		/**
		 * Whether delta CRLs should be used for checking the revocation status.
		 * Defaults to <code>false</code>.
		 */
		public virtual bool IsUseDeltasEnabled
		{
			get { return useDeltas; }
			set { useDeltas = value; }
		}

		/**
		* The validity model.
		* @see #CHAIN_VALIDITY_MODEL
		* @see #PKIX_VALIDITY_MODEL
		*/
		public virtual int ValidityModel
		{
			get { return validityModel; }
			set { validityModel = value; }
		}

		/**
		* Sets the Bouncy Castle Stores for finding CRLs, certificates, attribute
		* certificates or cross certificates.
		* <p>
		* The <code>IList</code> is cloned.
		* </p>
		*
		* @param stores A list of stores to use.
		* @see #getStores
		* @throws ClassCastException if an element of <code>stores</code> is not
		*             a {@link Store}.
		*/
		public virtual void SetStores(
			IList stores)
		{
			if (stores == null)
			{
                this.stores = Platform.CreateArrayList();
			}
			else
			{
				foreach (object obj in stores)
				{
					if (!(obj is IX509Store))
					{
						throw new InvalidCastException(
							"All elements of list must be of type " + typeof(IX509Store).FullName);
					}
				}
                this.stores = Platform.CreateArrayList(stores);
			}
		}

		/**
		* Adds a Bouncy Castle {@link Store} to find CRLs, certificates, attribute
		* certificates or cross certificates.
		* <p>
		* This method should be used to add local stores, like collection based
		* X.509 stores, if available. Local stores should be considered first,
		* before trying to use additional (remote) locations, because they do not
		* need possible additional network traffic.
		* </p><p>
		* If <code>store</code> is <code>null</code> it is ignored.
		* </p>
		*
		* @param store The store to add.
		* @see #getStores
		*/
		public virtual void AddStore(
			IX509Store store)
		{
			if (store != null)
			{
				stores.Add(store);
			}
		}

		/**
		* Adds an additional Bouncy Castle {@link Store} to find CRLs, certificates,
		* attribute certificates or cross certificates.
		* <p>
		* You should not use this method. This method is used for adding additional
		* X.509 stores, which are used to add (remote) locations, e.g. LDAP, found
		* during X.509 object processing, e.g. in certificates or CRLs. This method
		* is used in PKIX certification path processing.
		* </p><p>
		* If <code>store</code> is <code>null</code> it is ignored.
		* </p>
		*
		* @param store The store to add.
		* @see #getStores()
		*/
		public virtual void AddAdditionalStore(
			IX509Store store)
		{
			if (store != null)
			{
				additionalStores.Add(store);
			}
		}

		/**
		* Returns an <code>IList</code> of additional Bouncy Castle
		* <code>Store</code>s used for finding CRLs, certificates, attribute
		* certificates or cross certificates.
		*
		* @return an immutable <code>IList</code> of additional Bouncy Castle
		*         <code>Store</code>s. Never <code>null</code>.
		*
		* @see #addAddionalStore(Store)
		*/
		public virtual IList GetAdditionalStores()
		{
            return Platform.CreateArrayList(additionalStores);
		}

		/**
		* Returns an <code>IList</code> of Bouncy Castle
		* <code>Store</code>s used for finding CRLs, certificates, attribute
		* certificates or cross certificates.
		*
		* @return an immutable <code>IList</code> of Bouncy Castle
		*         <code>Store</code>s. Never <code>null</code>.
		*
		* @see #setStores(IList)
		*/
		public virtual IList GetStores()
		{
            return Platform.CreateArrayList(stores);
		}

		/**
		* Returns if additional {@link X509Store}s for locations like LDAP found
		* in certificates or CRLs should be used.
		*
		* @return Returns <code>true</code> if additional stores are used.
		*/
		public virtual bool IsAdditionalLocationsEnabled
		{
			get { return additionalLocationsEnabled; }
		}

		/**
		* Sets if additional {@link X509Store}s for locations like LDAP found in
		* certificates or CRLs should be used.
		*
		* @param enabled <code>true</code> if additional stores are used.
		*/
		public virtual void SetAdditionalLocationsEnabled(
			bool enabled)
		{
			additionalLocationsEnabled = enabled;
		}

		/**
		* Returns the required constraints on the target certificate or attribute
		* certificate. The constraints are returned as an instance of
		* <code>IX509Selector</code>. If <code>null</code>, no constraints are
		* defined.
		*
		* <p>
		* The target certificate in a PKIX path may be a certificate or an
		* attribute certificate.
		* </p><p>
		* Note that the <code>IX509Selector</code> returned is cloned to protect
		* against subsequent modifications.
		* </p>
		* @return a <code>IX509Selector</code> specifying the constraints on the
		*         target certificate or attribute certificate (or <code>null</code>)
		* @see #setTargetConstraints
		* @see X509CertStoreSelector
		* @see X509AttributeCertStoreSelector
		*/
		public virtual IX509Selector GetTargetConstraints()
		{
			if (selector != null)
			{
				return (IX509Selector) selector.Clone();
			}
			else
			{
				return null;
			}
		}

		/**
		* Sets the required constraints on the target certificate or attribute
		* certificate. The constraints are specified as an instance of
		* <code>IX509Selector</code>. If <code>null</code>, no constraints are
		* defined.
		* <p>
		* The target certificate in a PKIX path may be a certificate or an
		* attribute certificate.
		* </p><p>
		* Note that the <code>IX509Selector</code> specified is cloned to protect
		* against subsequent modifications.
		* </p>
		*
		* @param selector a <code>IX509Selector</code> specifying the constraints on
		*            the target certificate or attribute certificate (or
		*            <code>null</code>)
		* @see #getTargetConstraints
		* @see X509CertStoreSelector
		* @see X509AttributeCertStoreSelector
		*/
		public virtual void SetTargetConstraints(IX509Selector selector)
		{
			if (selector != null)
			{
				this.selector = (IX509Selector) selector.Clone();
			}
			else
			{
				this.selector = null;
			}
		}

		/**
		* Returns the trusted attribute certificate issuers. If attribute
		* certificates is verified the trusted AC issuers must be set.
		* <p>
		* The returned <code>ISet</code> consists of <code>TrustAnchor</code>s.
		* </p><p>
		* The returned <code>ISet</code> is immutable. Never <code>null</code>
		* </p>
		*
		* @return Returns an immutable set of the trusted AC issuers.
		*/
		public virtual ISet GetTrustedACIssuers()
		{
			return new HashSet(trustedACIssuers);
		}

		/**
		* Sets the trusted attribute certificate issuers. If attribute certificates
		* is verified the trusted AC issuers must be set.
		* <p>
		* The <code>trustedACIssuers</code> must be a <code>ISet</code> of
		* <code>TrustAnchor</code>
		* </p><p>
		* The given set is cloned.
		* </p>
		*
		* @param trustedACIssuers The trusted AC issuers to set. Is never
		*            <code>null</code>.
		* @throws ClassCastException if an element of <code>stores</code> is not
		*             a <code>TrustAnchor</code>.
		*/
		public virtual void SetTrustedACIssuers(
			ISet trustedACIssuers)
		{
			if (trustedACIssuers == null)
			{
				this.trustedACIssuers = new HashSet();
			}
			else
			{
				foreach (object obj in trustedACIssuers)
				{
					if (!(obj is TrustAnchor))
					{
						throw new InvalidCastException("All elements of set must be "
							+ "of type " + typeof(TrustAnchor).FullName + ".");
					}
				}
				this.trustedACIssuers = new HashSet(trustedACIssuers);
			}
		}

		/**
		* Returns the necessary attributes which must be contained in an attribute
		* certificate.
		* <p>
		* The returned <code>ISet</code> is immutable and contains
		* <code>String</code>s with the OIDs.
		* </p>
		*
		* @return Returns the necessary AC attributes.
		*/
		public virtual ISet GetNecessaryACAttributes()
		{
			return new HashSet(necessaryACAttributes);
		}

		/**
		* Sets the necessary which must be contained in an attribute certificate.
		* <p>
		* The <code>ISet</code> must contain <code>String</code>s with the
		* OIDs.
		* </p><p>
		* The set is cloned.
		* </p>
		*
		* @param necessaryACAttributes The necessary AC attributes to set.
		* @throws ClassCastException if an element of
		*             <code>necessaryACAttributes</code> is not a
		*             <code>String</code>.
		*/
		public virtual void SetNecessaryACAttributes(
			ISet necessaryACAttributes)
		{
			if (necessaryACAttributes == null)
			{
				this.necessaryACAttributes = new HashSet();
			}
			else
			{
				foreach (object obj in necessaryACAttributes)
				{
					if (!(obj is string))
					{
						throw new InvalidCastException("All elements of set must be "
							+ "of type string.");
					}
				}
				this.necessaryACAttributes = new HashSet(necessaryACAttributes);
			}
		}

		/**
		* Returns the attribute certificates which are not allowed.
		* <p>
		* The returned <code>ISet</code> is immutable and contains
		* <code>String</code>s with the OIDs.
		* </p>
		*
		* @return Returns the prohibited AC attributes. Is never <code>null</code>.
		*/
		public virtual ISet GetProhibitedACAttributes()
		{
			return new HashSet(prohibitedACAttributes);
		}

		/**
		* Sets the attribute certificates which are not allowed.
		* <p>
		* The <code>ISet</code> must contain <code>String</code>s with the
		* OIDs.
		* </p><p>
		* The set is cloned.
		* </p>
		*
		* @param prohibitedACAttributes The prohibited AC attributes to set.
		* @throws ClassCastException if an element of
		*             <code>prohibitedACAttributes</code> is not a
		*             <code>String</code>.
		*/
		public virtual void SetProhibitedACAttributes(
			ISet prohibitedACAttributes)
		{
			if (prohibitedACAttributes == null)
			{
				this.prohibitedACAttributes = new HashSet();
			}
			else
			{
				foreach (object obj in prohibitedACAttributes)
				{
					if (!(obj is String))
					{
						throw new InvalidCastException("All elements of set must be "
							+ "of type string.");
					}
				}
				this.prohibitedACAttributes = new HashSet(prohibitedACAttributes);
			}
		}

		/**
		* Returns the attribute certificate checker. The returned set contains
		* {@link PKIXAttrCertChecker}s and is immutable.
		*
		* @return Returns the attribute certificate checker. Is never
		*         <code>null</code>.
		*/
		public virtual ISet GetAttrCertCheckers()
		{
			return new HashSet(attrCertCheckers);
		}

		/**
		* Sets the attribute certificate checkers.
		* <p>
		* All elements in the <code>ISet</code> must a {@link PKIXAttrCertChecker}.
		* </p>
		* <p>
		* The given set is cloned.
		* </p>
		*
		* @param attrCertCheckers The attribute certificate checkers to set. Is
		*            never <code>null</code>.
		* @throws ClassCastException if an element of <code>attrCertCheckers</code>
		*             is not a <code>PKIXAttrCertChecker</code>.
		*/
		public virtual void SetAttrCertCheckers(
			ISet attrCertCheckers)
		{
			if (attrCertCheckers == null)
			{
				this.attrCertCheckers = new HashSet();
			}
			else
			{
				foreach (object obj in attrCertCheckers)
				{
					if (!(obj is PkixAttrCertChecker))
					{
						throw new InvalidCastException("All elements of set must be "
							+ "of type " + typeof(PkixAttrCertChecker).FullName + ".");
					}
				}
				this.attrCertCheckers = new HashSet(attrCertCheckers);
			}
		}
	}
}
