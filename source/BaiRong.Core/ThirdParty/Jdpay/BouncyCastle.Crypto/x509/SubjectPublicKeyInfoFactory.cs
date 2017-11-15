using System;

using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.CryptoPro;
using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.X509
{
    /// <summary>
    /// A factory to produce Public Key Info Objects.
    /// </summary>
    public sealed class SubjectPublicKeyInfoFactory
    {
        private SubjectPublicKeyInfoFactory()
        {
        }

        /// <summary>
        /// Create a Subject Public Key Info object for a given public key.
        /// </summary>
        /// <param name="key">One of ElGammalPublicKeyParameters, DSAPublicKeyParameter, DHPublicKeyParameters, RsaKeyParameters or ECPublicKeyParameters</param>
        /// <returns>A subject public key info object.</returns>
        /// <exception cref="Exception">Throw exception if object provided is not one of the above.</exception>
        public static SubjectPublicKeyInfo CreateSubjectPublicKeyInfo(
            AsymmetricKeyParameter key)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (key.IsPrivate)
                throw new ArgumentException("Private key passed - public key expected.", "key");

            if (key is ElGamalPublicKeyParameters)
            {
                ElGamalPublicKeyParameters _key = (ElGamalPublicKeyParameters)key;
                ElGamalParameters kp = _key.Parameters;

                SubjectPublicKeyInfo info = new SubjectPublicKeyInfo(
                    new AlgorithmIdentifier(
                        OiwObjectIdentifiers.ElGamalAlgorithm,
                        new ElGamalParameter(kp.P, kp.G).ToAsn1Object()),
                        new DerInteger(_key.Y));

                return info;
            }

            if (key is DsaPublicKeyParameters)
            {
                DsaPublicKeyParameters _key = (DsaPublicKeyParameters) key;
                DsaParameters kp = _key.Parameters;
                Asn1Encodable ae = kp == null
                    ?	null
                    :	new DsaParameter(kp.P, kp.Q, kp.G).ToAsn1Object();

                return new SubjectPublicKeyInfo(
                    new AlgorithmIdentifier(X9ObjectIdentifiers.IdDsa, ae),
                    new DerInteger(_key.Y));
            }

            if (key is DHPublicKeyParameters)
            {
                DHPublicKeyParameters _key = (DHPublicKeyParameters) key;
                DHParameters kp = _key.Parameters;

                SubjectPublicKeyInfo info = new SubjectPublicKeyInfo(
                    new AlgorithmIdentifier(
                        _key.AlgorithmOid,
                        new DHParameter(kp.P, kp.G, kp.L).ToAsn1Object()),
                        new DerInteger(_key.Y));

                return info;
            } // End of DH

            if (key is RsaKeyParameters)
            {
                RsaKeyParameters _key = (RsaKeyParameters) key;

                SubjectPublicKeyInfo info = new SubjectPublicKeyInfo(
                    new AlgorithmIdentifier(PkcsObjectIdentifiers.RsaEncryption, DerNull.Instance),
                    new RsaPublicKeyStructure(_key.Modulus, _key.Exponent).ToAsn1Object());

                return info;
            } // End of RSA.

            if (key is ECPublicKeyParameters)
            {
                ECPublicKeyParameters _key = (ECPublicKeyParameters) key;

                if (_key.AlgorithmName == "ECGOST3410")
                {
                    if (_key.PublicKeyParamSet == null)
                        throw Platform.CreateNotImplementedException("Not a CryptoPro parameter set");

                    ECPoint q = _key.Q.Normalize();
                    BigInteger bX = q.AffineXCoord.ToBigInteger();
                    BigInteger bY = q.AffineYCoord.ToBigInteger();

                    byte[] encKey = new byte[64];
                    ExtractBytes(encKey, 0, bX);
                    ExtractBytes(encKey, 32, bY);

                    Gost3410PublicKeyAlgParameters gostParams = new Gost3410PublicKeyAlgParameters(
                        _key.PublicKeyParamSet, CryptoProObjectIdentifiers.GostR3411x94CryptoProParamSet);

                    AlgorithmIdentifier algID = new AlgorithmIdentifier(
                        CryptoProObjectIdentifiers.GostR3410x2001,
                        gostParams.ToAsn1Object());

                    return new SubjectPublicKeyInfo(algID, new DerOctetString(encKey));
                }
                else
                {
                    X962Parameters x962;
                    if (_key.PublicKeyParamSet == null)
                    {
                        ECDomainParameters kp = _key.Parameters;
                        X9ECParameters ecP = new X9ECParameters(kp.Curve, kp.G, kp.N, kp.H, kp.GetSeed());

                        x962 = new X962Parameters(ecP);
                    }
                    else
                    {
                        x962 = new X962Parameters(_key.PublicKeyParamSet);
                    }

                    Asn1OctetString p = (Asn1OctetString)(new X9ECPoint(_key.Q).ToAsn1Object());

                    AlgorithmIdentifier algID = new AlgorithmIdentifier(
                        X9ObjectIdentifiers.IdECPublicKey, x962.ToAsn1Object());

                    return new SubjectPublicKeyInfo(algID, p.GetOctets());
                }
            } // End of EC

            if (key is Gost3410PublicKeyParameters)
            {
                Gost3410PublicKeyParameters _key = (Gost3410PublicKeyParameters) key;

                if (_key.PublicKeyParamSet == null)
                    throw Platform.CreateNotImplementedException("Not a CryptoPro parameter set");

                byte[] keyEnc = _key.Y.ToByteArrayUnsigned();
                byte[] keyBytes = new byte[keyEnc.Length];

                for (int i = 0; i != keyBytes.Length; i++)
                {
                    keyBytes[i] = keyEnc[keyEnc.Length - 1 - i]; // must be little endian
                }

                Gost3410PublicKeyAlgParameters algParams = new Gost3410PublicKeyAlgParameters(
                    _key.PublicKeyParamSet, CryptoProObjectIdentifiers.GostR3411x94CryptoProParamSet);

                AlgorithmIdentifier algID = new AlgorithmIdentifier(
                    CryptoProObjectIdentifiers.GostR3410x94,
                    algParams.ToAsn1Object());

                return new SubjectPublicKeyInfo(algID, new DerOctetString(keyBytes));
            }

            throw new ArgumentException("Class provided no convertible: " + Platform.GetTypeName(key));
        }

        private static void ExtractBytes(
            byte[]		encKey,
            int			offset,
            BigInteger	bI)
        {
            byte[] val = bI.ToByteArray();
            int n = (bI.BitLength + 7) / 8;

            for (int i = 0; i < n; ++i)
            {
                encKey[offset + i] = val[val.Length - 1 - i];
            }
        }
    }
}
