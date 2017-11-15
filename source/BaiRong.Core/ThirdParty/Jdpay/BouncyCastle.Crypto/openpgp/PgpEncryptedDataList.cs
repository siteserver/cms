using System;
using System.Collections;

using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;

namespace Org.BouncyCastle.Bcpg.OpenPgp
{
	/// <remarks>A holder for a list of PGP encryption method packets.</remarks>
    public class PgpEncryptedDataList
		: PgpObject
    {
        private IList list = Platform.CreateArrayList();
        private InputStreamPacket data;

		public PgpEncryptedDataList(
            BcpgInputStream bcpgInput)
        {
            while (bcpgInput.NextPacketTag() == PacketTag.PublicKeyEncryptedSession
                || bcpgInput.NextPacketTag() == PacketTag.SymmetricKeyEncryptedSessionKey)
            {
                list.Add(bcpgInput.ReadPacket());
            }

			data = (InputStreamPacket)bcpgInput.ReadPacket();

			for (int i = 0; i != list.Count; i++)
            {
                if (list[i] is SymmetricKeyEncSessionPacket)
                {
                    list[i] = new PgpPbeEncryptedData((SymmetricKeyEncSessionPacket) list[i], data);
                }
                else
                {
                    list[i] = new PgpPublicKeyEncryptedData((PublicKeyEncSessionPacket) list[i], data);
                }
            }
        }

		public PgpEncryptedData this[int index]
		{
			get { return (PgpEncryptedData) list[index]; }
		}

		[Obsolete("Use 'object[index]' syntax instead")]
		public object Get(int index)
        {
            return this[index];
        }

		[Obsolete("Use 'Count' property instead")]
		public int Size
        {
			get { return list.Count; }
        }

		public int Count
		{
			get { return list.Count; }
		}

		public bool IsEmpty
        {
			get { return list.Count == 0; }
        }

		public IEnumerable GetEncryptedDataObjects()
        {
            return new EnumerableProxy(list);
        }
    }
}
