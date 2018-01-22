using System;
using System.Collections;

using Org.BouncyCastle.Asn1.Anssi;
using Org.BouncyCastle.Asn1.CryptoPro;
using Org.BouncyCastle.Asn1.GM;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.TeleTrust;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;

namespace Org.BouncyCastle.Asn1.X9
{
    /**
     * A general class that reads all X9.62 style EC curve tables.
     */
    public class ECNamedCurveTable
    {
        /**
         * return a X9ECParameters object representing the passed in named
         * curve. The routine returns null if the curve is not present.
         *
         * @param name the name of the curve requested
         * @return an X9ECParameters object or null if the curve is not available.
         */
        public static X9ECParameters GetByName(string name)
        {
            X9ECParameters ecP = X962NamedCurves.GetByName(name);
            if (ecP == null)
            {
                ecP = SecNamedCurves.GetByName(name);
            }
            if (ecP == null)
            {
                ecP = NistNamedCurves.GetByName(name);
            }
            if (ecP == null)
            {
                ecP = TeleTrusTNamedCurves.GetByName(name);
            }
            if (ecP == null)
            {
                ecP = AnssiNamedCurves.GetByName(name);
            }
            if (ecP == null)
            {
                ecP = FromDomainParameters(ECGost3410NamedCurves.GetByName(name));
            }
            if (ecP == null)
            {
                ecP = GMNamedCurves.GetByName(name);
            }
            return ecP;
        }

        public static string GetName(DerObjectIdentifier oid)
        {
            string name = X962NamedCurves.GetName(oid);
            if (name == null)
            {
                name = SecNamedCurves.GetName(oid);
            }
            if (name == null)
            {
                name = NistNamedCurves.GetName(oid);
            }
            if (name == null)
            {
                name = TeleTrusTNamedCurves.GetName(oid);
            }
            if (name == null)
            {
                name = AnssiNamedCurves.GetName(oid);
            }
            if (name == null)
            {
                name = ECGost3410NamedCurves.GetName(oid);
            }
            if (name == null)
            {
                name = GMNamedCurves.GetName(oid);
            }
            return name;
        }

        /**
         * return the object identifier signified by the passed in name. Null
         * if there is no object identifier associated with name.
         *
         * @return the object identifier associated with name, if present.
         */
        public static DerObjectIdentifier GetOid(string name)
        {
            DerObjectIdentifier oid = X962NamedCurves.GetOid(name);
            if (oid == null)
            {
                oid = SecNamedCurves.GetOid(name);
            }
            if (oid == null)
            {
                oid = NistNamedCurves.GetOid(name);
            }
            if (oid == null)
            {
                oid = TeleTrusTNamedCurves.GetOid(name);
            }
            if (oid == null)
            {
                oid = AnssiNamedCurves.GetOid(name);
            }
            if (oid == null)
            {
                oid = ECGost3410NamedCurves.GetOid(name);
            }
            if (oid == null)
            {
                oid = GMNamedCurves.GetOid(name);
            }
            return oid;
        }

        /**
         * return a X9ECParameters object representing the passed in named
         * curve.
         *
         * @param oid the object id of the curve requested
         * @return an X9ECParameters object or null if the curve is not available.
         */
        public static X9ECParameters GetByOid(DerObjectIdentifier oid)
        {
            X9ECParameters ecP = X962NamedCurves.GetByOid(oid);
            if (ecP == null)
            {
                ecP = SecNamedCurves.GetByOid(oid);
            }

            // NOTE: All the NIST curves are currently from SEC, so no point in redundant OID lookup

            if (ecP == null)
            {
                ecP = TeleTrusTNamedCurves.GetByOid(oid);
            }
            if (ecP == null)
            {
                ecP = AnssiNamedCurves.GetByOid(oid);
            }
            if (ecP == null)
            {
                ecP = FromDomainParameters(ECGost3410NamedCurves.GetByOid(oid));
            }
            if (ecP == null)
            {
                ecP = GMNamedCurves.GetByOid(oid);
            }
            return ecP;
        }

        /**
         * return an enumeration of the names of the available curves.
         *
         * @return an enumeration of the names of the available curves.
         */
        public static IEnumerable Names
        {
            get
            {
                IList v = Platform.CreateArrayList();
                CollectionUtilities.AddRange(v, X962NamedCurves.Names);
                CollectionUtilities.AddRange(v, SecNamedCurves.Names);
                CollectionUtilities.AddRange(v, NistNamedCurves.Names);
                CollectionUtilities.AddRange(v, TeleTrusTNamedCurves.Names);
                CollectionUtilities.AddRange(v, AnssiNamedCurves.Names);
                CollectionUtilities.AddRange(v, ECGost3410NamedCurves.Names);
                CollectionUtilities.AddRange(v, GMNamedCurves.Names);
                return v;
            }
        }

        private static X9ECParameters FromDomainParameters(ECDomainParameters dp)
        {
            return dp == null ? null : new X9ECParameters(dp.Curve, dp.G, dp.N, dp.H, dp.GetSeed());
        }
    }
}
