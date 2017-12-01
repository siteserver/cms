using System;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities.Collections;

namespace Org.BouncyCastle.X509
{
	public abstract class X509ExtensionBase
		: IX509Extension
	{
		protected abstract X509Extensions GetX509Extensions();

		protected virtual ISet GetExtensionOids(
			bool critical)
		{
			X509Extensions extensions = GetX509Extensions();
			if (extensions != null)
			{
				HashSet set = new HashSet();
				foreach (DerObjectIdentifier oid in extensions.ExtensionOids)
				{
					X509Extension ext = extensions.GetExtension(oid);
					if (ext.IsCritical == critical)
					{
						set.Add(oid.Id);
					}
				}

				return set;
			}

			return null;
		}

		/// <summary>
		/// Get non critical extensions.
		/// </summary>
		/// <returns>A set of non critical extension oids.</returns>
		public virtual ISet GetNonCriticalExtensionOids()
		{
			return GetExtensionOids(false);
		}

		/// <summary>
		/// Get any critical extensions.
		/// </summary>
		/// <returns>A sorted list of critical entension.</returns>
		public virtual ISet GetCriticalExtensionOids()
		{
			return GetExtensionOids(true);
		}

		/// <summary>
		/// Get the value of a given extension.
		/// </summary>
		/// <param name="oid">The object ID of the extension. </param>
		/// <returns>An Asn1OctetString object if that extension is found or null if not.</returns>
		[Obsolete("Use version taking a DerObjectIdentifier instead")]
		public Asn1OctetString GetExtensionValue(
			string oid)
		{
			return GetExtensionValue(new DerObjectIdentifier(oid));
		}

		public virtual Asn1OctetString GetExtensionValue(
			DerObjectIdentifier oid)
		{
			X509Extensions exts = GetX509Extensions();
			if (exts != null)
			{
				X509Extension ext = exts.GetExtension(oid);
				if (ext != null)
				{
					return ext.Value;
				}
			}

			return null;
		}
	}
}
