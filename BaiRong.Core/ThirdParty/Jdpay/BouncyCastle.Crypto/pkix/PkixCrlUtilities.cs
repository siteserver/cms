using System;
using System.Collections;

using Org.BouncyCastle.Utilities.Collections;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Store;

namespace Org.BouncyCastle.Pkix
{
	public class PkixCrlUtilities
	{
		public virtual ISet FindCrls(X509CrlStoreSelector crlselect, PkixParameters paramsPkix, DateTime currentDate)
		{
			ISet initialSet = new HashSet();

			// get complete CRL(s)
			try
			{
				initialSet.AddAll(FindCrls(crlselect, paramsPkix.GetAdditionalStores()));
				initialSet.AddAll(FindCrls(crlselect, paramsPkix.GetStores()));
			}
			catch (Exception e)
			{
				throw new Exception("Exception obtaining complete CRLs.", e);
			}

			ISet finalSet = new HashSet();
			DateTime validityDate = currentDate;

			if (paramsPkix.Date != null)
			{
				validityDate = paramsPkix.Date.Value;
			}

			// based on RFC 5280 6.3.3
			foreach (X509Crl crl in initialSet)
			{
				if (crl.NextUpdate.Value.CompareTo(validityDate) > 0)
				{
					X509Certificate cert = crlselect.CertificateChecking;

					if (cert != null)
					{
						if (crl.ThisUpdate.CompareTo(cert.NotAfter) < 0)
						{
							finalSet.Add(crl);
						}
					}
					else
					{
						finalSet.Add(crl);
					}
				}
			}

			return finalSet;
		}

		public virtual ISet FindCrls(X509CrlStoreSelector crlselect, PkixParameters paramsPkix)
		{
			ISet completeSet = new HashSet();

			// get complete CRL(s)
			try
			{
				completeSet.AddAll(FindCrls(crlselect, paramsPkix.GetStores()));
			}
			catch (Exception e)
			{
				throw new Exception("Exception obtaining complete CRLs.", e);
			}

			return completeSet;
		}

		/// <summary>
		/// crl checking
		/// Return a Collection of all CRLs found in the X509Store's that are
		/// matching the crlSelect criteriums.
		/// </summary>
		/// <param name="crlSelect">a {@link X509CRLStoreSelector} object that will be used
		/// to select the CRLs</param>
		/// <param name="crlStores">a List containing only {@link org.bouncycastle.x509.X509Store
		/// X509Store} objects. These are used to search for CRLs</param>
		/// <returns>a Collection of all found {@link X509CRL X509CRL} objects. May be
		/// empty but never <code>null</code>.
		/// </returns>
		private ICollection FindCrls(X509CrlStoreSelector crlSelect, IList crlStores)
		{
			ISet crls = new HashSet();

			Exception lastException = null;
			bool foundValidStore = false;

			foreach (IX509Store store in crlStores)
			{
				try
				{
					crls.AddAll(store.GetMatches(crlSelect));
					foundValidStore = true;
				}
				catch (X509StoreException e)
				{
					lastException = new Exception("Exception searching in X.509 CRL store.", e);
				}
			}

	        if (!foundValidStore && lastException != null)
	            throw lastException;

			return crls;
		}
	}
}
