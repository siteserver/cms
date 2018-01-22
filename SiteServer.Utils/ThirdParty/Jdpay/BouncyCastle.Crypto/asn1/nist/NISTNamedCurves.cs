using System;
using System.Collections;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;

namespace Org.BouncyCastle.Asn1.Nist
{
    /**
    * Utility class for fetching curves using their NIST names as published in FIPS-PUB 186-3
    */
    public sealed class NistNamedCurves
    {
        private NistNamedCurves()
        {
        }

        private static readonly IDictionary objIds = Platform.CreateHashtable();
        private static readonly IDictionary names = Platform.CreateHashtable();

        private static void DefineCurveAlias(
            string				name,
            DerObjectIdentifier	oid)
        {
            objIds.Add(Platform.ToUpperInvariant(name), oid);
            names.Add(oid, name);
        }

        static NistNamedCurves()
        {
            DefineCurveAlias("B-163", SecObjectIdentifiers.SecT163r2);
            DefineCurveAlias("B-233", SecObjectIdentifiers.SecT233r1);
            DefineCurveAlias("B-283", SecObjectIdentifiers.SecT283r1);
            DefineCurveAlias("B-409", SecObjectIdentifiers.SecT409r1);
            DefineCurveAlias("B-571", SecObjectIdentifiers.SecT571r1);

            DefineCurveAlias("K-163", SecObjectIdentifiers.SecT163k1);
            DefineCurveAlias("K-233", SecObjectIdentifiers.SecT233k1);
            DefineCurveAlias("K-283", SecObjectIdentifiers.SecT283k1);
            DefineCurveAlias("K-409", SecObjectIdentifiers.SecT409k1);
            DefineCurveAlias("K-571", SecObjectIdentifiers.SecT571k1);

            DefineCurveAlias("P-192", SecObjectIdentifiers.SecP192r1);
            DefineCurveAlias("P-224", SecObjectIdentifiers.SecP224r1);
            DefineCurveAlias("P-256", SecObjectIdentifiers.SecP256r1);
            DefineCurveAlias("P-384", SecObjectIdentifiers.SecP384r1);
            DefineCurveAlias("P-521", SecObjectIdentifiers.SecP521r1);
        }

        public static X9ECParameters GetByName(
            string name)
        {
            DerObjectIdentifier oid = GetOid(name);
            return oid == null ? null : GetByOid(oid);
        }

        /**
        * return the X9ECParameters object for the named curve represented by
        * the passed in object identifier. Null if the curve isn't present.
        *
        * @param oid an object identifier representing a named curve, if present.
        */
        public static X9ECParameters GetByOid(
            DerObjectIdentifier oid)
        {
            return SecNamedCurves.GetByOid(oid);
        }

        /**
        * return the object identifier signified by the passed in name. Null
        * if there is no object identifier associated with name.
        *
        * @return the object identifier associated with name, if present.
        */
        public static DerObjectIdentifier GetOid(
            string name)
        {
            return (DerObjectIdentifier) objIds[Platform.ToUpperInvariant(name)];
        }

        /**
        * return the named curve name represented by the given object identifier.
        */
        public static string GetName(
            DerObjectIdentifier  oid)
        {
            return (string) names[oid];
        }

        /**
        * returns an enumeration containing the name strings for curves
        * contained in this structure.
        */
        public static IEnumerable Names
        {
            get { return new EnumerableProxy(names.Values); }
        }
    }
}
