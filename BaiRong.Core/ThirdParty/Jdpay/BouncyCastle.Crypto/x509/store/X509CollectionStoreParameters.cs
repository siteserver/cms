using System;
using System.Collections;
using System.Text;

using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.X509.Store
{
	/// <remarks>This class contains a collection for collection based <code>X509Store</code>s.</remarks>
	public class X509CollectionStoreParameters
		: IX509StoreParameters
	{
		private readonly IList collection;

		/// <summary>
		/// Constructor.
		/// <p>
		/// The collection is copied.
		/// </p>
		/// </summary>
		/// <param name="collection">The collection containing X.509 object types.</param>
		/// <exception cref="ArgumentNullException">If collection is null.</exception>
		public X509CollectionStoreParameters(
			ICollection collection)
		{
			if (collection == null)
				throw new ArgumentNullException("collection");

			this.collection = Platform.CreateArrayList(collection);
		}

		// TODO Do we need to be able to Clone() these, and should it really be shallow?
//		/**
//		* Returns a shallow clone. The returned contents are not copied, so adding
//		* or removing objects will effect this.
//		*
//		* @return a shallow clone.
//		*/
//		public object Clone()
//		{
//			return new X509CollectionStoreParameters(collection);
//		}

		/// <summary>Returns a copy of the <code>ICollection</code>.</summary>
		public ICollection GetCollection()
		{
			return Platform.CreateArrayList(collection);
		}

		/// <summary>Returns a formatted string describing the parameters.</summary>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("X509CollectionStoreParameters: [\n");
			sb.Append("  collection: " + collection + "\n");
			sb.Append("]");
			return sb.ToString();
		}
	}
}
