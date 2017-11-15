using System;
using System.Text;

using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509.Store;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;

namespace Org.BouncyCastle.Pkix
{
	/// <summary>
    /// Summary description for PkixBuilderParameters.
	/// </summary>
	public class PkixBuilderParameters
		: PkixParameters
	{
		private int maxPathLength = 5;

		private ISet excludedCerts = new HashSet();

		/**
		* Returns an instance of <code>PkixBuilderParameters</code>.
		* <p>
		* This method can be used to get a copy from other
		* <code>PKIXBuilderParameters</code>, <code>PKIXParameters</code>,
		* and <code>ExtendedPKIXParameters</code> instances.
		* </p>
		*
		* @param pkixParams The PKIX parameters to create a copy of.
		* @return An <code>PkixBuilderParameters</code> instance.
		*/
		public static PkixBuilderParameters GetInstance(
			PkixParameters pkixParams)
		{
			PkixBuilderParameters parameters = new PkixBuilderParameters(
				pkixParams.GetTrustAnchors(),
				new X509CertStoreSelector(pkixParams.GetTargetCertConstraints()));
			parameters.SetParams(pkixParams);
			return parameters;
		}

		public PkixBuilderParameters(
			ISet			trustAnchors,
			IX509Selector	targetConstraints)
			: base(trustAnchors)
		{
			SetTargetCertConstraints(targetConstraints);
		}

		public virtual int MaxPathLength
		{
			get { return maxPathLength; }
			set
			{
				if (value < -1)
				{
					throw new InvalidParameterException(
						"The maximum path length parameter can not be less than -1.");
				}
				this.maxPathLength = value;
			}
		}

		/// <summary>
		/// Excluded certificates are not used for building a certification path.
		/// </summary>
		/// <returns>the excluded certificates.</returns>
		public virtual ISet GetExcludedCerts()
		{
			return new HashSet(excludedCerts);
		}

		/// <summary>
		/// Sets the excluded certificates which are not used for building a
		/// certification path. If the <code>ISet</code> is <code>null</code> an
		/// empty set is assumed.
		/// </summary>
		/// <remarks>
		/// The given set is cloned to protect it against subsequent modifications.
		/// </remarks>
		/// <param name="excludedCerts">The excluded certificates to set.</param>
		public virtual void SetExcludedCerts(
			ISet excludedCerts)
		{
			if (excludedCerts == null)
			{
				excludedCerts = new HashSet();
			}
			else
			{
				this.excludedCerts = new HashSet(excludedCerts);
			}
		}

		/**
		* Can alse handle <code>ExtendedPKIXBuilderParameters</code> and
		* <code>PKIXBuilderParameters</code>.
		* 
		* @param params Parameters to set.
		* @see org.bouncycastle.x509.ExtendedPKIXParameters#setParams(java.security.cert.PKIXParameters)
		*/
		protected override void SetParams(
			PkixParameters parameters)
		{
			base.SetParams(parameters);
			if (parameters is PkixBuilderParameters)
			{
				PkixBuilderParameters _params = (PkixBuilderParameters) parameters;
				maxPathLength = _params.maxPathLength;
				excludedCerts = new HashSet(_params.excludedCerts);
			}
		}

		/**
		* Makes a copy of this <code>PKIXParameters</code> object. Changes to the
		* copy will not affect the original and vice versa.
		*
		* @return a copy of this <code>PKIXParameters</code> object
		*/
		public override object Clone()
		{
			PkixBuilderParameters parameters = new PkixBuilderParameters(
				GetTrustAnchors(), GetTargetCertConstraints());
			parameters.SetParams(this);
			return parameters;
		}

		public override string ToString()
		{
			string nl = Platform.NewLine;
			StringBuilder s = new StringBuilder();
			s.Append("PkixBuilderParameters [" + nl);
			s.Append(base.ToString());
			s.Append("  Maximum Path Length: ");
			s.Append(MaxPathLength);
			s.Append(nl + "]" + nl);
			return s.ToString();
		}
	}
}
