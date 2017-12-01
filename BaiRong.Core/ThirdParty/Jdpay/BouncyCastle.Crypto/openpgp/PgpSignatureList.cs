using System;

namespace Org.BouncyCastle.Bcpg.OpenPgp
{
	/// <remarks>A list of PGP signatures - normally in the signature block after literal data.</remarks>
    public class PgpSignatureList
		: PgpObject
    {
        private PgpSignature[] sigs;

		public PgpSignatureList(
            PgpSignature[] sigs)
        {
            this.sigs = (PgpSignature[]) sigs.Clone();
        }

		public PgpSignatureList(
            PgpSignature sig)
        {
			this.sigs = new PgpSignature[]{ sig };
        }

		public PgpSignature this[int index]
		{
			get { return sigs[index]; }
		}

		[Obsolete("Use 'object[index]' syntax instead")]
		public PgpSignature Get(
            int index)
        {
            return this[index];
        }

		[Obsolete("Use 'Count' property instead")]
		public int Size
        {
			get { return sigs.Length; }
        }

		public int Count
		{
			get { return sigs.Length; }
		}

		public bool IsEmpty
        {
			get { return (sigs.Length == 0); }
        }
    }
}
