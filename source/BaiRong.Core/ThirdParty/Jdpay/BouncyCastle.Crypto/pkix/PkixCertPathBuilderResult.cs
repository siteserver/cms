using System;
using System.Text;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkix;

namespace Org.BouncyCastle.Pkix
{
	/// <summary>
	/// Summary description for PkixCertPathBuilderResult.
	/// </summary>
	public class PkixCertPathBuilderResult
		: PkixCertPathValidatorResult//, ICertPathBuilderResult
	{
		private PkixCertPath certPath;
		
		public PkixCertPathBuilderResult(
			PkixCertPath			certPath,
			TrustAnchor				trustAnchor,
			PkixPolicyNode			policyTree,
			AsymmetricKeyParameter	subjectPublicKey)
			: base(trustAnchor, policyTree, subjectPublicKey)
		{			
			if (certPath == null)
				throw new ArgumentNullException("certPath");

			this.certPath = certPath;
		}

		public PkixCertPath CertPath
		{
            get { return certPath; }
		}

		public override string ToString()
		{
			StringBuilder s = new StringBuilder();
			s.Append("SimplePKIXCertPathBuilderResult: [\n");
			s.Append("  Certification Path: ").Append(CertPath).Append('\n');
			s.Append("  Trust Anchor: ").Append(this.TrustAnchor.TrustedCert.IssuerDN.ToString()).Append('\n');
			s.Append("  Subject Public Key: ").Append(this.SubjectPublicKey).Append("\n]");
			return s.ToString();
		}
	}
}
