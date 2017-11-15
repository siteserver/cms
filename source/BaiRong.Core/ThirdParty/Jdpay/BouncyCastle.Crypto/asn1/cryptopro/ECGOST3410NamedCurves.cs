using System;
using System.Collections;

using Org.BouncyCastle.Asn1.Rosstandart;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;

namespace Org.BouncyCastle.Asn1.CryptoPro
{
    /// <summary>
    /// Table of the available named parameters for GOST 3410-2001 / 2012.
    /// </summary>
    public sealed class ECGost3410NamedCurves
    {
        private ECGost3410NamedCurves()
        {
        }

        internal static readonly IDictionary objIds = Platform.CreateHashtable();
        internal static readonly IDictionary parameters = Platform.CreateHashtable();
        internal static readonly IDictionary names = Platform.CreateHashtable();

        static ECGost3410NamedCurves()
        {
            BigInteger mod_p = new BigInteger("115792089237316195423570985008687907853269984665640564039457584007913129639319");
            BigInteger mod_q = new BigInteger("115792089237316195423570985008687907853073762908499243225378155805079068850323");

            FpCurve curve = new FpCurve(
                mod_p, // p
                new BigInteger("115792089237316195423570985008687907853269984665640564039457584007913129639316"), // a
                new BigInteger("166"), // b
                mod_q,
                BigInteger.One);

            ECDomainParameters ecParams = new ECDomainParameters(
                curve,
                curve.CreatePoint(
                    new BigInteger("1"), // x
                    new BigInteger("64033881142927202683649881450433473985931760268884941288852745803908878638612")), // y
                mod_q);

            parameters[CryptoProObjectIdentifiers.GostR3410x2001CryptoProA] = ecParams;

            mod_p = new BigInteger("115792089237316195423570985008687907853269984665640564039457584007913129639319");
            mod_q = new BigInteger("115792089237316195423570985008687907853073762908499243225378155805079068850323");

            curve = new FpCurve(
                mod_p, // p
                new BigInteger("115792089237316195423570985008687907853269984665640564039457584007913129639316"),
                new BigInteger("166"),
                mod_q,
                BigInteger.One);

            ecParams = new ECDomainParameters(
                curve,
                curve.CreatePoint(
                    new BigInteger("1"), // x
                    new BigInteger("64033881142927202683649881450433473985931760268884941288852745803908878638612")), // y
                mod_q);

            parameters[CryptoProObjectIdentifiers.GostR3410x2001CryptoProXchA] = ecParams;

            mod_p = new BigInteger("57896044618658097711785492504343953926634992332820282019728792003956564823193"); //p
            mod_q = new BigInteger("57896044618658097711785492504343953927102133160255826820068844496087732066703"); //q

            curve = new FpCurve(
                mod_p, // p
                new BigInteger("57896044618658097711785492504343953926634992332820282019728792003956564823190"), // a
                new BigInteger("28091019353058090096996979000309560759124368558014865957655842872397301267595"), // b
                mod_q,
                BigInteger.One);

            ecParams = new ECDomainParameters(
                curve,
                curve.CreatePoint(
                    new BigInteger("1"), // x
                    new BigInteger("28792665814854611296992347458380284135028636778229113005756334730996303888124")), // y
                mod_q); // q

            parameters[CryptoProObjectIdentifiers.GostR3410x2001CryptoProB] = ecParams;

            mod_p = new BigInteger("70390085352083305199547718019018437841079516630045180471284346843705633502619");
            mod_q = new BigInteger("70390085352083305199547718019018437840920882647164081035322601458352298396601");

            curve = new FpCurve(
                mod_p, // p
                new BigInteger("70390085352083305199547718019018437841079516630045180471284346843705633502616"),
                new BigInteger("32858"),
                mod_q,
                BigInteger.One);

            ecParams = new ECDomainParameters(
                curve,
                curve.CreatePoint(
                    new BigInteger("0"),
                    new BigInteger("29818893917731240733471273240314769927240550812383695689146495261604565990247")),
                mod_q);

            parameters[CryptoProObjectIdentifiers.GostR3410x2001CryptoProXchB] = ecParams;

            mod_p = new BigInteger("70390085352083305199547718019018437841079516630045180471284346843705633502619"); //p
            mod_q = new BigInteger("70390085352083305199547718019018437840920882647164081035322601458352298396601"); //q
            curve = new FpCurve(
                mod_p, // p
                new BigInteger("70390085352083305199547718019018437841079516630045180471284346843705633502616"), // a
                new BigInteger("32858"), // b
                mod_q,
                BigInteger.One);

            ecParams = new ECDomainParameters(
                curve,
                curve.CreatePoint(
                    new BigInteger("0"), // x
                    new BigInteger("29818893917731240733471273240314769927240550812383695689146495261604565990247")), // y
                mod_q); // q

            parameters[CryptoProObjectIdentifiers.GostR3410x2001CryptoProC] = ecParams;

            //GOST34.10 2012
            mod_p = new BigInteger("115792089237316195423570985008687907853269984665640564039457584007913129639319"); //p
            mod_q = new BigInteger("115792089237316195423570985008687907853073762908499243225378155805079068850323"); //q
            curve = new FpCurve(
                mod_p, // p
                new BigInteger("115792089237316195423570985008687907853269984665640564039457584007913129639316"), // a
                new BigInteger("166"), // b
                mod_q,
                BigInteger.One);

            ecParams = new ECDomainParameters(
                curve,
                curve.CreatePoint(
                    new BigInteger("1"), // x
                    new BigInteger("64033881142927202683649881450433473985931760268884941288852745803908878638612")), // y
                mod_q); // q

            parameters[RosstandartObjectIdentifiers.id_tc26_gost_3410_12_256_paramSetA] = ecParams;

            mod_p = new BigInteger("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFDC7",16); //p
            mod_q = new BigInteger("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF27E69532F48D89116FF22B8D4E0560609B4B38ABFAD2B85DCACDB1411F10B275",16); //q
            curve = new FpCurve(
                mod_p, // p
                new BigInteger("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFDC4",16), // a
                new BigInteger("E8C2505DEDFC86DDC1BD0B2B6667F1DA34B82574761CB0E879BD081CFD0B6265EE3CB090F30D27614CB4574010DA90DD862EF9D4EBEE4761503190785A71C760",16), // b
                mod_q,
                BigInteger.One);

            ecParams = new ECDomainParameters(
                curve,
                curve.CreatePoint(
                    new BigInteger("00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000003"), // x
                    new BigInteger("7503CFE87A836AE3A61B8816E25450E6CE5E1C93ACF1ABC1778064FDCBEFA921DF1626BE4FD036E93D75E6A50E3A41E98028FE5FC235F5B889A589CB5215F2A4",16)), // y
                mod_q); // q

            parameters[RosstandartObjectIdentifiers.id_tc26_gost_3410_12_512_paramSetA] = ecParams;

            mod_p = new BigInteger("8000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000006F",16); //p
            mod_q = new BigInteger("800000000000000000000000000000000000000000000000000000000000000149A1EC142565A545ACFDB77BD9D40CFA8B996712101BEA0EC6346C54374F25BD",16); //q
            curve = new FpCurve(
                mod_p, // p
                new BigInteger("8000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000006C",16), // a
                new BigInteger("687D1B459DC841457E3E06CF6F5E2517B97C7D614AF138BCBF85DC806C4B289F3E965D2DB1416D217F8B276FAD1AB69C50F78BEE1FA3106EFB8CCBC7C5140116",16), // b
                mod_q,
                BigInteger.One);

            ecParams = new ECDomainParameters(
                curve,
                curve.CreatePoint(
                    new BigInteger("00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000002"), // x
                    new BigInteger("1A8F7EDA389B094C2C071E3647A8940F3C123B697578C213BE6DD9E6C8EC7335DCB228FD1EDF4A39152CBCAAF8C0398828041055F94CEEEC7E21340780FE41BD",16)), // y
                mod_q); // q

            parameters[RosstandartObjectIdentifiers.id_tc26_gost_3410_12_512_paramSetB] = ecParams;

            mod_p = new BigInteger("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFDC7",16); //p
            mod_q = new BigInteger("3FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFC98CDBA46506AB004C33A9FF5147502CC8EDA9E7A769A12694623CEF47F023ED",16); //q
            curve = new FpCurve(
                mod_p, // p
                new BigInteger("DC9203E514A721875485A529D2C722FB187BC8980EB866644DE41C68E143064546E861C0E2C9EDD92ADE71F46FCF50FF2AD97F951FDA9F2A2EB6546F39689BD3",16), // a
                new BigInteger("B4C4EE28CEBC6C2C8AC12952CF37F16AC7EFB6A9F69F4B57FFDA2E4F0DE5ADE038CBC2FFF719D2C18DE0284B8BFEF3B52B8CC7A5F5BF0A3C8D2319A5312557E1",16), // b
                mod_q,
                BigInteger.One);

            ecParams = new ECDomainParameters(
                curve,
                curve.CreatePoint(
                    new BigInteger("E2E31EDFC23DE7BDEBE241CE593EF5DE2295B7A9CBAEF021D385F7074CEA043AA27272A7AE602BF2A7B9033DB9ED3610C6FB85487EAE97AAC5BC7928C1950148", 16), // x
                    new BigInteger("F5CE40D95B5EB899ABBCCFF5911CB8577939804D6527378B8C108C3D2090FF9BE18E2D33E3021ED2EF32D85822423B6304F726AA854BAE07D0396E9A9ADDC40F",16)), // y
                mod_q); // q

            parameters[RosstandartObjectIdentifiers.id_tc26_gost_3410_12_512_paramSetC] = ecParams;

            objIds["GostR3410-2001-CryptoPro-A"] = CryptoProObjectIdentifiers.GostR3410x2001CryptoProA;
            objIds["GostR3410-2001-CryptoPro-B"] = CryptoProObjectIdentifiers.GostR3410x2001CryptoProB;
            objIds["GostR3410-2001-CryptoPro-C"] = CryptoProObjectIdentifiers.GostR3410x2001CryptoProC;
            objIds["GostR3410-2001-CryptoPro-XchA"] = CryptoProObjectIdentifiers.GostR3410x2001CryptoProXchA;
            objIds["GostR3410-2001-CryptoPro-XchB"] = CryptoProObjectIdentifiers.GostR3410x2001CryptoProXchB;
            objIds["Tc26-Gost-3410-12-256-paramSetA"] = RosstandartObjectIdentifiers.id_tc26_gost_3410_12_256_paramSetA;
            objIds["Tc26-Gost-3410-12-512-paramSetA"] = RosstandartObjectIdentifiers.id_tc26_gost_3410_12_512_paramSetA;
            objIds["Tc26-Gost-3410-12-512-paramSetB"] = RosstandartObjectIdentifiers.id_tc26_gost_3410_12_512_paramSetB;
            objIds["Tc26-Gost-3410-12-512-paramSetC"] = RosstandartObjectIdentifiers.id_tc26_gost_3410_12_512_paramSetC;

            names[CryptoProObjectIdentifiers.GostR3410x2001CryptoProA] = "GostR3410-2001-CryptoPro-A";
            names[CryptoProObjectIdentifiers.GostR3410x2001CryptoProB] = "GostR3410-2001-CryptoPro-B";
            names[CryptoProObjectIdentifiers.GostR3410x2001CryptoProC] = "GostR3410-2001-CryptoPro-C";
            names[CryptoProObjectIdentifiers.GostR3410x2001CryptoProXchA] = "GostR3410-2001-CryptoPro-XchA";
            names[CryptoProObjectIdentifiers.GostR3410x2001CryptoProXchB] = "GostR3410-2001-CryptoPro-XchB";
            names[RosstandartObjectIdentifiers.id_tc26_gost_3410_12_256_paramSetA] = "Tc26-Gost-3410-12-256-paramSetA";
            names[RosstandartObjectIdentifiers.id_tc26_gost_3410_12_512_paramSetA] = "Tc26-Gost-3410-12-512-paramSetA";
            names[RosstandartObjectIdentifiers.id_tc26_gost_3410_12_512_paramSetB] = "Tc26-Gost-3410-12-512-paramSetB";
            names[RosstandartObjectIdentifiers.id_tc26_gost_3410_12_512_paramSetC] = "Tc26-Gost-3410-12-512-paramSetC";
        }

        /**
        * return the ECDomainParameters object for the given OID, null if it
        * isn't present.
        *
        * @param oid an object identifier representing a named parameters, if present.
        */
        public static ECDomainParameters GetByOid(
            DerObjectIdentifier oid)
        {
            return (ECDomainParameters) parameters[oid];
        }

        /**
         * returns an enumeration containing the name strings for curves
         * contained in this structure.
         */
        public static IEnumerable Names
        {
            get { return new EnumerableProxy(names.Values); }
        }

        public static ECDomainParameters GetByName(
            string name)
        {
            DerObjectIdentifier oid = (DerObjectIdentifier) objIds[name];

            if (oid != null)
            {
                return (ECDomainParameters) parameters[oid];
            }

            return null;
        }

        /**
        * return the named curve name represented by the given object identifier.
        */
        public static string GetName(
            DerObjectIdentifier oid)
        {
            return (string) names[oid];
        }

        public static DerObjectIdentifier GetOid(
            string name)
        {
            return (DerObjectIdentifier) objIds[name];
        }
    }
}
