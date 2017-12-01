using System;
using System.IO;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Math.EC.Multiplier;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Crypto.Signers
{
    /// <summary>The SM2 Digital Signature algorithm.</summary>
    public class SM2Signer
        : ISigner
    {
        private readonly IDsaKCalculator kCalculator = new RandomDsaKCalculator();
        private readonly SM3Digest digest = new SM3Digest();

        private ECDomainParameters ecParams;
        private ECPoint pubPoint;
        private ECKeyParameters ecKey;
        private byte[] z;

        public virtual string AlgorithmName
        {
            get { return "SM2Sign"; }
        }

        public virtual void Init(bool forSigning, ICipherParameters parameters)
        {
            ICipherParameters baseParam;
            byte[] userID;

            if (parameters is ParametersWithID)
            {
                baseParam = ((ParametersWithID)parameters).Parameters;
                userID = ((ParametersWithID)parameters).GetID();
            }
            else
            {
                baseParam = parameters;
                userID = Hex.Decode("31323334353637383132333435363738"); // the default value (ASCII "1234567812345678")
            }

            if (forSigning)
            {
                if (baseParam is ParametersWithRandom)
                {
                    ParametersWithRandom rParam = (ParametersWithRandom)baseParam;

                    ecKey = (ECKeyParameters)rParam.Parameters;
                    ecParams = ecKey.Parameters;
                    kCalculator.Init(ecParams.N, rParam.Random);
                }
                else
                {
                    ecKey = (ECKeyParameters)baseParam;
                    ecParams = ecKey.Parameters;
                    kCalculator.Init(ecParams.N, new SecureRandom());
                }
                pubPoint = CreateBasePointMultiplier().Multiply(ecParams.G, ((ECPrivateKeyParameters)ecKey).D).Normalize();
            }
            else
            {
                ecKey = (ECKeyParameters)baseParam;
                ecParams = ecKey.Parameters;
                pubPoint = ((ECPublicKeyParameters)ecKey).Q;
            }

            digest.Reset();
            z = GetZ(userID);

            digest.BlockUpdate(z, 0, z.Length);
        }

        public virtual void Update(byte b)
        {
            digest.Update(b);
        }

        public virtual void BlockUpdate(byte[] buf, int off, int len)
        {
            digest.BlockUpdate(buf, off, len);
        }

        public virtual bool VerifySignature(byte[] signature)
        {
            try
            {
                BigInteger[] rs = DerDecode(signature);
                if (rs != null)
                {
                    return VerifySignature(rs[0], rs[1]);
                }
            }
            catch (IOException e)
            {
            }

            return false;
        }

        public virtual void Reset()
        {
            if (z != null)
            {
                digest.Reset();
                digest.BlockUpdate(z, 0, z.Length);
            }
        }

        public virtual byte[] GenerateSignature()
        {
            byte[] eHash = DigestUtilities.DoFinal(digest);

            BigInteger n = ecParams.N;
            BigInteger e = CalculateE(eHash);
            BigInteger d = ((ECPrivateKeyParameters)ecKey).D;

            BigInteger r, s;

            ECMultiplier basePointMultiplier = CreateBasePointMultiplier();

            // 5.2.1 Draft RFC:  SM2 Public Key Algorithms
            do // generate s
            {
                BigInteger k;
                do // generate r
                {
                    // A3
                    k = kCalculator.NextK();

                    // A4
                    ECPoint p = basePointMultiplier.Multiply(ecParams.G, k).Normalize();

                    // A5
                    r = e.Add(p.AffineXCoord.ToBigInteger()).Mod(n);
                }
                while (r.SignValue == 0 || r.Add(k).Equals(n));

                // A6
                BigInteger dPlus1ModN = d.Add(BigInteger.One).ModInverse(n);

                s = k.Subtract(r.Multiply(d)).Mod(n);
                s = dPlus1ModN.Multiply(s).Mod(n);
            }
            while (s.SignValue == 0);

            // A7
            try
            {
                return DerEncode(r, s);
            }
            catch (IOException ex)
            {
                throw new CryptoException("unable to encode signature: " + ex.Message, ex);
            }
        }

        private bool VerifySignature(BigInteger r, BigInteger s)
        {
            BigInteger n = ecParams.N;

            // 5.3.1 Draft RFC:  SM2 Public Key Algorithms
            // B1
            if (r.CompareTo(BigInteger.One) < 0 || r.CompareTo(n) >= 0)
                return false;

            // B2
            if (s.CompareTo(BigInteger.One) < 0 || s.CompareTo(n) >= 0)
                return false;

            // B3
            byte[] eHash = DigestUtilities.DoFinal(digest);

            // B4
            BigInteger e = CalculateE(eHash);

            // B5
            BigInteger t = r.Add(s).Mod(n);
            if (t.SignValue == 0)
                return false;

            // B6
            ECPoint q = ((ECPublicKeyParameters)ecKey).Q;
            ECPoint x1y1 = ECAlgorithms.SumOfTwoMultiplies(ecParams.G, s, q, t).Normalize();
            if (x1y1.IsInfinity)
                return false;

            // B7
            return r.Equals(e.Add(x1y1.AffineXCoord.ToBigInteger()).Mod(n));
        }

        private byte[] GetZ(byte[] userID)
        {
            AddUserID(digest, userID);

            AddFieldElement(digest, ecParams.Curve.A);
            AddFieldElement(digest, ecParams.Curve.B);
            AddFieldElement(digest, ecParams.G.AffineXCoord);
            AddFieldElement(digest, ecParams.G.AffineYCoord);
            AddFieldElement(digest, pubPoint.AffineXCoord);
            AddFieldElement(digest, pubPoint.AffineYCoord);

            return DigestUtilities.DoFinal(digest);
        }

        private void AddUserID(IDigest digest, byte[] userID)
        {
            int len = userID.Length * 8;
            digest.Update((byte)(len >> 8));
            digest.Update((byte)len);
            digest.BlockUpdate(userID, 0, userID.Length);
        }

        private void AddFieldElement(IDigest digest, ECFieldElement v)
        {
            byte[] p = v.GetEncoded();
            digest.BlockUpdate(p, 0, p.Length);
        }

        protected virtual BigInteger CalculateE(byte[] message)
        {
            return new BigInteger(1, message);
        }

        protected virtual ECMultiplier CreateBasePointMultiplier()
        {
            return new FixedPointCombMultiplier();
        }

        protected virtual BigInteger[] DerDecode(byte[] encoding)
        {
            Asn1Sequence seq = Asn1Sequence.GetInstance(Asn1Object.FromByteArray(encoding));
            if (seq.Count != 2)
                return null;

            BigInteger r = DerInteger.GetInstance(seq[0]).Value;
            BigInteger s = DerInteger.GetInstance(seq[1]).Value;

            byte[] expectedEncoding = DerEncode(r, s);
            if (!Arrays.ConstantTimeAreEqual(expectedEncoding, encoding))
                return null;

            return new BigInteger[]{ r, s };
        }

        protected virtual byte[] DerEncode(BigInteger r, BigInteger s)
        {
            return new DerSequence(new DerInteger(r), new DerInteger(s)).GetEncoded(Asn1Encodable.Der);
        }
    }
}
