using System;

namespace Org.BouncyCastle.Bcpg.OpenPgp
{
	/// <remarks>Holder for a list of PgpOnePassSignature objects.</remarks>
    public class PgpOnePassSignatureList
		: PgpObject
    {
        private readonly PgpOnePassSignature[] sigs;

		public PgpOnePassSignatureList(
            PgpOnePassSignature[] sigs)
        {
			this.sigs = (PgpOnePassSignature[]) sigs.Clone();
        }

		public PgpOnePassSignatureList(
            PgpOnePassSignature sig)
        {
			this.sigs = new PgpOnePassSignature[]{ sig };
        }

		public PgpOnePassSignature this[int index]
		{
			get { return sigs[index]; }
		}

		[Obsolete("Use 'object[index]' syntax instead")]
		public PgpOnePassSignature Get(
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
