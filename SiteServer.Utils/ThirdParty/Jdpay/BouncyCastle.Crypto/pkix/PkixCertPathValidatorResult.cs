using System;
using System.Text;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Pkix
{
	/// <summary>
	/// Summary description for PkixCertPathValidatorResult.
	/// </summary>
	public class PkixCertPathValidatorResult
		//: ICertPathValidatorResult
	{
		private TrustAnchor trustAnchor;
		private PkixPolicyNode policyTree;
		private AsymmetricKeyParameter subjectPublicKey;

		public PkixPolicyNode PolicyTree
		{
			get { return this.policyTree; }
		}

		public TrustAnchor TrustAnchor
		{
			get { return this.trustAnchor; }
		}

		public AsymmetricKeyParameter SubjectPublicKey
		{
			get { return this.subjectPublicKey; }
		}

		public PkixCertPathValidatorResult(
			TrustAnchor				trustAnchor,
			PkixPolicyNode			policyTree,
			AsymmetricKeyParameter	subjectPublicKey)
		{
			if (subjectPublicKey == null)
			{
				throw new NullReferenceException("subjectPublicKey must be non-null");
			}
			if (trustAnchor == null)
			{
				throw new NullReferenceException("trustAnchor must be non-null");
			}
			
			this.trustAnchor = trustAnchor;
			this.policyTree = policyTree;
			this.subjectPublicKey = subjectPublicKey;
		}

		public object Clone()
		{
			return new PkixCertPathValidatorResult(this.TrustAnchor, this.PolicyTree, this.SubjectPublicKey);
		}

		public override String ToString() 
		{
			StringBuilder sB = new StringBuilder();
			sB.Append("PKIXCertPathValidatorResult: [ \n");
			sB.Append("  Trust Anchor: ").Append(this.TrustAnchor).Append('\n');
			sB.Append("  Policy Tree: ").Append(this.PolicyTree).Append('\n');
			sB.Append("  Subject Public Key: ").Append(this.SubjectPublicKey).Append("\n]");
			return sB.ToString();
		}

	}
}
