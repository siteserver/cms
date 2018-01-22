using System;
using System.Collections;

using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Math.EC.Endo;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;
using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Asn1.GM
{
    public sealed class GMNamedCurves
    {
        private GMNamedCurves()
        {
        }

        private static ECCurve ConfigureCurve(ECCurve curve)
        {
            return curve;
        }

        private static BigInteger FromHex(string hex)
        {
            return new BigInteger(1, Hex.Decode(hex));
        }

        /*
         * sm2p256v1
         */
        internal class SM2P256V1Holder
            : X9ECParametersHolder
        {
            private SM2P256V1Holder() {}

            internal static readonly X9ECParametersHolder Instance = new SM2P256V1Holder();

            protected override X9ECParameters CreateParameters()
            {
                BigInteger p = FromHex("FFFFFFFEFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF00000000FFFFFFFFFFFFFFFF");
                BigInteger a = FromHex("FFFFFFFEFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF00000000FFFFFFFFFFFFFFFC");
                BigInteger b = FromHex("28E9FA9E9D9F5E344D5A9E4BCF6509A7F39789F515AB8F92DDBCBD414D940E93");
                byte[] S = null;
                BigInteger n = FromHex("FFFFFFFEFFFFFFFFFFFFFFFFFFFFFFFF7203DF6B21C6052B53BBF40939D54123");
                BigInteger h = BigInteger.One;

                ECCurve curve = ConfigureCurve(new FpCurve(p, a, b, n, h));
                X9ECPoint G = new X9ECPoint(curve, Hex.Decode("04"
                    + "32C4AE2C1F1981195F9904466A39C9948FE30BBFF2660BE1715A4589334C74C7"
                    + "BC3736A2F4F6779C59BDCEE36B692153D0A9877CC62A474002DF32E52139F0A0"));

                return new X9ECParameters(curve, G, n, h, S);
            }
        }

        /*
         * wapip192v1
         */
        internal class WapiP192V1Holder
            : X9ECParametersHolder
        {
            private WapiP192V1Holder() { }

            internal static readonly X9ECParametersHolder Instance = new WapiP192V1Holder();

            protected override X9ECParameters CreateParameters()
            {
                BigInteger p = FromHex("BDB6F4FE3E8B1D9E0DA8C0D46F4C318CEFE4AFE3B6B8551F");
                BigInteger a = FromHex("BB8E5E8FBC115E139FE6A814FE48AAA6F0ADA1AA5DF91985");
                BigInteger b = FromHex("1854BEBDC31B21B7AEFC80AB0ECD10D5B1B3308E6DBF11C1");
                byte[] S = null;
                BigInteger n = FromHex("BDB6F4FE3E8B1D9E0DA8C0D40FC962195DFAE76F56564677");
                BigInteger h = BigInteger.One;

                ECCurve curve = ConfigureCurve(new FpCurve(p, a, b, n, h));
                X9ECPoint G = new X9ECPoint(curve, Hex.Decode("04"
                    + "4AD5F7048DE709AD51236DE6" + "5E4D4B482C836DC6E4106640"
                    + "02BB3A02D4AAADACAE24817A" + "4CA3A1B014B5270432DB27D2"));

                return new X9ECParameters(curve, G, n, h, S);
            }
        }


        private static readonly IDictionary objIds = Platform.CreateHashtable();
        private static readonly IDictionary curves = Platform.CreateHashtable();
        private static readonly IDictionary names = Platform.CreateHashtable();

        private static void DefineCurve(
            string					name,
            DerObjectIdentifier		oid,
            X9ECParametersHolder	holder)
        {
            objIds.Add(Platform.ToUpperInvariant(name), oid);
            names.Add(oid, name);
            curves.Add(oid, holder);
        }

        static GMNamedCurves()
        {
            DefineCurve("wapip192v1", GMObjectIdentifiers.wapip192v1, WapiP192V1Holder.Instance);
            DefineCurve("sm2p256v1", GMObjectIdentifiers.sm2p256v1, SM2P256V1Holder.Instance);
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
            X9ECParametersHolder holder = (X9ECParametersHolder)curves[oid];
            return holder == null ? null : holder.Parameters;
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
            return (DerObjectIdentifier)objIds[Platform.ToUpperInvariant(name)];
        }

        /**
         * return the named curve name represented by the given object identifier.
         */
        public static string GetName(
            DerObjectIdentifier oid)
        {
            return (string)names[oid];
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
