namespace Org.BouncyCastle.Bcpg.OpenPgp
{
	/// <remarks>
	/// A PGP marker packet - in general these should be ignored other than where
	/// the idea is to preserve the original input stream.
	/// </remarks>
    public class PgpMarker
		: PgpObject
    {
        private readonly MarkerPacket p;

		public PgpMarker(
            BcpgInputStream bcpgIn)
        {
            p = (MarkerPacket) bcpgIn.ReadPacket();
        }
	}
}
