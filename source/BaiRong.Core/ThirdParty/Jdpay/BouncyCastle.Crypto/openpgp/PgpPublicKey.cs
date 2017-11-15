using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;

namespace Org.BouncyCastle.Bcpg.OpenPgp
{
    /// <remarks>General class to handle a PGP public key object.</remarks>
    public class PgpPublicKey
    {
        public static byte[] CalculateFingerprint(PublicKeyPacket publicPk)
        {
            IBcpgKey key = publicPk.Key;
            IDigest digest;

            if (publicPk.Version <= 3)
            {
                RsaPublicBcpgKey rK = (RsaPublicBcpgKey)key;

                try
                {
                    digest = DigestUtilities.GetDigest("MD5");
                    UpdateDigest(digest, rK.Modulus);
                    UpdateDigest(digest, rK.PublicExponent);
                }
                catch (Exception e)
                {
                    throw new PgpException("can't encode key components: " + e.Message, e);
                }
            }
            else
            {
                try
                {
                    byte[] kBytes = publicPk.GetEncodedContents();

                    digest = DigestUtilities.GetDigest("SHA1");

                    digest.Update(0x99);
                    digest.Update((byte)(kBytes.Length >> 8));
                    digest.Update((byte)kBytes.Length);
                    digest.BlockUpdate(kBytes, 0, kBytes.Length);
                }
                catch (Exception e)
                {
                    throw new PgpException("can't encode key components: " + e.Message, e);
                }
            }

            return DigestUtilities.DoFinal(digest);
        }

        private static void UpdateDigest(IDigest d, BigInteger b)
        {
            byte[] bytes = b.ToByteArrayUnsigned();
            d.BlockUpdate(bytes, 0, bytes.Length);
        }

        private static readonly int[] MasterKeyCertificationTypes = new int[]
        {
            PgpSignature.PositiveCertification,
            PgpSignature.CasualCertification,
            PgpSignature.NoCertification,
            PgpSignature.DefaultCertification
        };

        private long				keyId;
        private byte[]				fingerprint;
        private int					keyStrength;

        internal PublicKeyPacket	publicPk;
        internal TrustPacket		trustPk;
        internal IList			    keySigs = Platform.CreateArrayList();
        internal IList			    ids = Platform.CreateArrayList();
        internal IList              idTrusts = Platform.CreateArrayList();
        internal IList              idSigs = Platform.CreateArrayList();
        internal IList			    subSigs;

        private void Init()
        {
            IBcpgKey key = publicPk.Key;

            this.fingerprint = CalculateFingerprint(publicPk);

            if (publicPk.Version <= 3)
            {
                RsaPublicBcpgKey rK = (RsaPublicBcpgKey) key;

                this.keyId = rK.Modulus.LongValue;
                this.keyStrength = rK.Modulus.BitLength;
            }
            else
            {
                this.keyId = (long)(((ulong)fingerprint[fingerprint.Length - 8] << 56)
                    | ((ulong)fingerprint[fingerprint.Length - 7] << 48)
                    | ((ulong)fingerprint[fingerprint.Length - 6] << 40)
                    | ((ulong)fingerprint[fingerprint.Length - 5] << 32)
                    | ((ulong)fingerprint[fingerprint.Length - 4] << 24)
                    | ((ulong)fingerprint[fingerprint.Length - 3] << 16)
                    | ((ulong)fingerprint[fingerprint.Length - 2] << 8)
                    | (ulong)fingerprint[fingerprint.Length - 1]);

                if (key is RsaPublicBcpgKey)
                {
                    this.keyStrength = ((RsaPublicBcpgKey)key).Modulus.BitLength;
                }
                else if (key is DsaPublicBcpgKey)
                {
                    this.keyStrength = ((DsaPublicBcpgKey)key).P.BitLength;
                }
                else if (key is ElGamalPublicBcpgKey)
                {
                    this.keyStrength = ((ElGamalPublicBcpgKey)key).P.BitLength;
                }
                else if (key is ECPublicBcpgKey)
                {
                    this.keyStrength = ECKeyPairGenerator.FindECCurveByOid(((ECPublicBcpgKey)key).CurveOid).Curve.FieldSize;
                }
            }
        }

        /// <summary>
        /// Create a PgpPublicKey from the passed in lightweight one.
        /// </summary>
        /// <remarks>
        /// Note: the time passed in affects the value of the key's keyId, so you probably only want
        /// to do this once for a lightweight key, or make sure you keep track of the time you used.
        /// </remarks>
        /// <param name="algorithm">Asymmetric algorithm type representing the public key.</param>
        /// <param name="pubKey">Actual public key to associate.</param>
        /// <param name="time">Date of creation.</param>
        /// <exception cref="ArgumentException">If <c>pubKey</c> is not public.</exception>
        /// <exception cref="PgpException">On key creation problem.</exception>
        public PgpPublicKey(
            PublicKeyAlgorithmTag	algorithm,
            AsymmetricKeyParameter	pubKey,
            DateTime				time)
        {
            if (pubKey.IsPrivate)
                throw new ArgumentException("Expected a public key", "pubKey");

            IBcpgKey bcpgKey;
            if (pubKey is RsaKeyParameters)
            {
                RsaKeyParameters rK = (RsaKeyParameters) pubKey;

                bcpgKey = new RsaPublicBcpgKey(rK.Modulus, rK.Exponent);
            }
            else if (pubKey is DsaPublicKeyParameters)
            {
                DsaPublicKeyParameters dK = (DsaPublicKeyParameters) pubKey;
                DsaParameters dP = dK.Parameters;

                bcpgKey = new DsaPublicBcpgKey(dP.P, dP.Q, dP.G, dK.Y);
            }
            else if (pubKey is ECPublicKeyParameters)
            {
                ECPublicKeyParameters ecK = (ECPublicKeyParameters)pubKey;

                if (algorithm == PublicKeyAlgorithmTag.ECDH)
                {
                    bcpgKey = new ECDHPublicBcpgKey(ecK.PublicKeyParamSet, ecK.Q, HashAlgorithmTag.Sha256, SymmetricKeyAlgorithmTag.Aes128);
                }
                else if (algorithm == PublicKeyAlgorithmTag.ECDsa)
                {
                    bcpgKey = new ECDsaPublicBcpgKey(ecK.PublicKeyParamSet, ecK.Q);
                }
                else
                {
                    throw new PgpException("unknown EC algorithm");
                }
            }
            else if (pubKey is ElGamalPublicKeyParameters)
            {
                ElGamalPublicKeyParameters eK = (ElGamalPublicKeyParameters) pubKey;
                ElGamalParameters eS = eK.Parameters;

                bcpgKey = new ElGamalPublicBcpgKey(eS.P, eS.G, eK.Y);
            }
            else
            {
                throw new PgpException("unknown key class");
            }

            this.publicPk = new PublicKeyPacket(algorithm, time, bcpgKey);
            this.ids = Platform.CreateArrayList();
            this.idSigs = Platform.CreateArrayList();

            try
            {
                Init();
            }
            catch (IOException e)
            {
                throw new PgpException("exception calculating keyId", e);
            }
        }

        public PgpPublicKey(PublicKeyPacket publicPk)
            : this(publicPk, Platform.CreateArrayList(), Platform.CreateArrayList())
        {
        }

        /// <summary>Constructor for a sub-key.</summary>
        internal PgpPublicKey(
            PublicKeyPacket	publicPk,
            TrustPacket		trustPk,
            IList           sigs)
        {
            this.publicPk = publicPk;
            this.trustPk = trustPk;
            this.subSigs = sigs;

            Init();
        }

        internal PgpPublicKey(
            PgpPublicKey	key,
            TrustPacket		trust,
            IList           subSigs)
        {
            this.publicPk = key.publicPk;
            this.trustPk = trust;
            this.subSigs = subSigs;

            this.fingerprint = key.fingerprint;
            this.keyId = key.keyId;
            this.keyStrength = key.keyStrength;
        }

        /// <summary>Copy constructor.</summary>
        /// <param name="pubKey">The public key to copy.</param>
        internal PgpPublicKey(
            PgpPublicKey pubKey)
        {
            this.publicPk = pubKey.publicPk;

            this.keySigs = Platform.CreateArrayList(pubKey.keySigs);
            this.ids = Platform.CreateArrayList(pubKey.ids);
            this.idTrusts = Platform.CreateArrayList(pubKey.idTrusts);
            this.idSigs = Platform.CreateArrayList(pubKey.idSigs.Count);
            for (int i = 0; i != pubKey.idSigs.Count; i++)
            {
                this.idSigs.Add(Platform.CreateArrayList((IList)pubKey.idSigs[i]));
            }

            if (pubKey.subSigs != null)
            {
                this.subSigs = Platform.CreateArrayList(pubKey.subSigs.Count);
                for (int i = 0; i != pubKey.subSigs.Count; i++)
                {
                    this.subSigs.Add(pubKey.subSigs[i]);
                }
            }

            this.fingerprint = pubKey.fingerprint;
            this.keyId = pubKey.keyId;
            this.keyStrength = pubKey.keyStrength;
        }

        internal PgpPublicKey(
            PublicKeyPacket	publicPk,
            TrustPacket		trustPk,
            IList		    keySigs,
            IList		    ids,
            IList           idTrusts,
            IList           idSigs)
        {
            this.publicPk = publicPk;
            this.trustPk = trustPk;
            this.keySigs = keySigs;
            this.ids = ids;
            this.idTrusts = idTrusts;
            this.idSigs = idSigs;

            Init();
        }

        internal PgpPublicKey(
            PublicKeyPacket	publicPk,
            IList           ids,
            IList           idSigs)
        {
            this.publicPk = publicPk;
            this.ids = ids;
            this.idSigs = idSigs;
            Init();
        }

        /// <summary>The version of this key.</summary>
        public int Version
        {
            get { return publicPk.Version; }
        }

        /// <summary>The creation time of this key.</summary>
        public DateTime CreationTime
        {
            get { return publicPk.GetTime(); }
        }

        /// <summary>The number of valid days from creation time - zero means no expiry.</summary>
        /// <remarks>WARNING: This method will return 1 for keys with version > 3 that expire in less than 1 day</remarks>
        [Obsolete("Use 'GetValidSeconds' instead")]
        public int ValidDays
        {
            get
            {
                if (publicPk.Version <= 3)
                {
                    return publicPk.ValidDays;
                }

                long expSecs = GetValidSeconds();
                if (expSecs <= 0)
                    return 0;

                int days = (int)(expSecs / (24 * 60 * 60));
                return System.Math.Max(1, days);
            }
        }

        /// <summary>Return the trust data associated with the public key, if present.</summary>
        /// <returns>A byte array with trust data, null otherwise.</returns>
        public byte[] GetTrustData()
        {
            if (trustPk == null)
            {
                return null;
            }

            return Arrays.Clone(trustPk.GetLevelAndTrustAmount());
        }

        /// <summary>The number of valid seconds from creation time - zero means no expiry.</summary>
        public long GetValidSeconds()
        {
            if (publicPk.Version <= 3)
            {
                return (long)publicPk.ValidDays * (24 * 60 * 60);
            }

            if (IsMasterKey)
            {
                for (int i = 0; i != MasterKeyCertificationTypes.Length; i++)
                {
                    long seconds = GetExpirationTimeFromSig(true, MasterKeyCertificationTypes[i]);
                    if (seconds >= 0)
                    {
                        return seconds;
                    }
                }
            }
            else
            {
                long seconds = GetExpirationTimeFromSig(false, PgpSignature.SubkeyBinding);
                if (seconds >= 0)
                {
                    return seconds;
                }
            }

            return 0;
        }

        private long GetExpirationTimeFromSig(bool selfSigned, int signatureType)
        {
            long expiryTime = -1;
            long lastDate = -1;

            foreach (PgpSignature sig in GetSignaturesOfType(signatureType))
            {
                if (selfSigned && sig.KeyId != this.KeyId)
                    continue;

                PgpSignatureSubpacketVector hashed = sig.GetHashedSubPackets();
                if (hashed == null)
                    continue;

                long current = hashed.GetKeyExpirationTime();

                if (sig.KeyId == this.KeyId)
                {
                    if (sig.CreationTime.Ticks > lastDate)
                    {
                        lastDate = sig.CreationTime.Ticks;
                        expiryTime = current;
                    }
                }
                else if (current == 0 || current > expiryTime)
                {
                    expiryTime = current;
                }
            }

            return expiryTime;
        }

        /// <summary>The keyId associated with the public key.</summary>
        public long KeyId
        {
            get { return keyId; }
        }

        /// <summary>The fingerprint of the key</summary>
        public byte[] GetFingerprint()
        {
            return (byte[]) fingerprint.Clone();
        }

        /// <summary>
        /// Check if this key has an algorithm type that makes it suitable to use for encryption.
        /// </summary>
        /// <remarks>
        /// Note: with version 4 keys KeyFlags subpackets should also be considered when present for
        /// determining the preferred use of the key.
        /// </remarks>
        /// <returns>
        /// <c>true</c> if this key algorithm is suitable for encryption.
        /// </returns>
        public bool IsEncryptionKey
        {
            get
            {
                switch (publicPk.Algorithm)
                {
                    case PublicKeyAlgorithmTag.ECDH:
                    case PublicKeyAlgorithmTag.ElGamalEncrypt:
                    case PublicKeyAlgorithmTag.ElGamalGeneral:
                    case PublicKeyAlgorithmTag.RsaEncrypt:
                    case PublicKeyAlgorithmTag.RsaGeneral:
                        return true;
                    default:
                        return false;
                }
            }
        }

        /// <summary>True, if this is a master key.</summary>
        public bool IsMasterKey
        {
            get { return subSigs == null; }
        }

        /// <summary>The algorithm code associated with the public key.</summary>
        public PublicKeyAlgorithmTag Algorithm
        {
            get { return publicPk.Algorithm; }
        }

        /// <summary>The strength of the key in bits.</summary>
        public int BitStrength
        {
            get { return keyStrength; }
        }

        /// <summary>The public key contained in the object.</summary>
        /// <returns>A lightweight public key.</returns>
        /// <exception cref="PgpException">If the key algorithm is not recognised.</exception>
        public AsymmetricKeyParameter GetKey()
        {
            try
            {
                switch (publicPk.Algorithm)
                {
                    case PublicKeyAlgorithmTag.RsaEncrypt:
                    case PublicKeyAlgorithmTag.RsaGeneral:
                    case PublicKeyAlgorithmTag.RsaSign:
                        RsaPublicBcpgKey rsaK = (RsaPublicBcpgKey)publicPk.Key;
                        return new RsaKeyParameters(false, rsaK.Modulus, rsaK.PublicExponent);
                    case PublicKeyAlgorithmTag.Dsa:
                        DsaPublicBcpgKey dsaK = (DsaPublicBcpgKey)publicPk.Key;
                        return new DsaPublicKeyParameters(dsaK.Y, new DsaParameters(dsaK.P, dsaK.Q, dsaK.G));
                    case PublicKeyAlgorithmTag.ECDsa:
                        return GetECKey("ECDSA");
                    case PublicKeyAlgorithmTag.ECDH:
                        return GetECKey("ECDH");
                    case PublicKeyAlgorithmTag.ElGamalEncrypt:
                    case PublicKeyAlgorithmTag.ElGamalGeneral:
                        ElGamalPublicBcpgKey elK = (ElGamalPublicBcpgKey)publicPk.Key;
                        return new ElGamalPublicKeyParameters(elK.Y, new ElGamalParameters(elK.P, elK.G));
                    default:
                        throw new PgpException("unknown public key algorithm encountered");
                }
            }
            catch (PgpException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new PgpException("exception constructing public key", e);
            }
        }

        private ECPublicKeyParameters GetECKey(string algorithm)
        {
            ECPublicBcpgKey ecK = (ECPublicBcpgKey)publicPk.Key;
            X9ECParameters x9 = ECKeyPairGenerator.FindECCurveByOid(ecK.CurveOid);
            ECPoint q = x9.Curve.DecodePoint(BigIntegers.AsUnsignedByteArray(ecK.EncodedPoint));
            return new ECPublicKeyParameters(algorithm, q, ecK.CurveOid);
        }

        /// <summary>Allows enumeration of any user IDs associated with the key.</summary>
        /// <returns>An <c>IEnumerable</c> of <c>string</c> objects.</returns>
        public IEnumerable GetUserIds()
        {
            IList temp = Platform.CreateArrayList();

            foreach (object o in ids)
            {
                if (o is string)
                {
                    temp.Add(o);
                }
            }

            return new EnumerableProxy(temp);
        }

        /// <summary>Allows enumeration of any user attribute vectors associated with the key.</summary>
        /// <returns>An <c>IEnumerable</c> of <c>PgpUserAttributeSubpacketVector</c> objects.</returns>
        public IEnumerable GetUserAttributes()
        {
            IList temp = Platform.CreateArrayList();

            foreach (object o in ids)
            {
                if (o is PgpUserAttributeSubpacketVector)
                {
                    temp.Add(o);
                }
            }

            return new EnumerableProxy(temp);
        }

        /// <summary>Allows enumeration of any signatures associated with the passed in id.</summary>
        /// <param name="id">The ID to be matched.</param>
        /// <returns>An <c>IEnumerable</c> of <c>PgpSignature</c> objects.</returns>
        public IEnumerable GetSignaturesForId(
            string id)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            for (int i = 0; i != ids.Count; i++)
            {
                if (id.Equals(ids[i]))
                {
                    return new EnumerableProxy((IList)idSigs[i]);
                }
            }

            return null;
        }

        /// <summary>Allows enumeration of signatures associated with the passed in user attributes.</summary>
        /// <param name="userAttributes">The vector of user attributes to be matched.</param>
        /// <returns>An <c>IEnumerable</c> of <c>PgpSignature</c> objects.</returns>
        public IEnumerable GetSignaturesForUserAttribute(
            PgpUserAttributeSubpacketVector userAttributes)
        {
            for (int i = 0; i != ids.Count; i++)
            {
                if (userAttributes.Equals(ids[i]))
                {
                    return new EnumerableProxy((IList) idSigs[i]);
                }
            }

            return null;
        }

        /// <summary>Allows enumeration of signatures of the passed in type that are on this key.</summary>
        /// <param name="signatureType">The type of the signature to be returned.</param>
        /// <returns>An <c>IEnumerable</c> of <c>PgpSignature</c> objects.</returns>
        public IEnumerable GetSignaturesOfType(
            int signatureType)
        {
            IList temp = Platform.CreateArrayList();

            foreach (PgpSignature sig in GetSignatures())
            {
                if (sig.SignatureType == signatureType)
                {
                    temp.Add(sig);
                }
            }

            return new EnumerableProxy(temp);
        }

        /// <summary>Allows enumeration of all signatures/certifications associated with this key.</summary>
        /// <returns>An <c>IEnumerable</c> with all signatures/certifications.</returns>
        public IEnumerable GetSignatures()
        {
            IList sigs = subSigs;
            if (sigs == null)
            {
                sigs = Platform.CreateArrayList(keySigs);

                foreach (ICollection extraSigs in idSigs)
                {
                    CollectionUtilities.AddRange(sigs, extraSigs);
                }
            }

            return new EnumerableProxy(sigs);
        }

        /**
         * Return all signatures/certifications directly associated with this key (ie, not to a user id).
         *
         * @return an iterator (possibly empty) with all signatures/certifications.
         */
        public IEnumerable GetKeySignatures()
        {
            IList sigs = subSigs;
            if (sigs == null)
            {
                sigs = Platform.CreateArrayList(keySigs);
            }
            return new EnumerableProxy(sigs);
        }

        public PublicKeyPacket PublicKeyPacket
        {
            get { return publicPk; }
        }

        public byte[] GetEncoded()
        {
            MemoryStream bOut = new MemoryStream();
            Encode(bOut);
            return bOut.ToArray();
        }

        public void Encode(
            Stream outStr)
        {
            BcpgOutputStream bcpgOut = BcpgOutputStream.Wrap(outStr);

            bcpgOut.WritePacket(publicPk);
            if (trustPk != null)
            {
                bcpgOut.WritePacket(trustPk);
            }

            if (subSigs == null)    // not a sub-key
            {
                foreach (PgpSignature keySig in keySigs)
                {
                    keySig.Encode(bcpgOut);
                }

                for (int i = 0; i != ids.Count; i++)
                {
                    if (ids[i] is string)
                    {
                        string id = (string) ids[i];

                        bcpgOut.WritePacket(new UserIdPacket(id));
                    }
                    else
                    {
                        PgpUserAttributeSubpacketVector v = (PgpUserAttributeSubpacketVector)ids[i];
                        bcpgOut.WritePacket(new UserAttributePacket(v.ToSubpacketArray()));
                    }

                    if (idTrusts[i] != null)
                    {
                        bcpgOut.WritePacket((ContainedPacket)idTrusts[i]);
                    }

                    foreach (PgpSignature sig in (IList) idSigs[i])
                    {
                        sig.Encode(bcpgOut);
                    }
                }
            }
            else
            {
                foreach (PgpSignature subSig in subSigs)
                {
                    subSig.Encode(bcpgOut);
                }
            }
        }

        /// <summary>Check whether this (sub)key has a revocation signature on it.</summary>
        /// <returns>True, if this (sub)key has been revoked.</returns>
        public bool IsRevoked()
        {
            int ns = 0;
            bool revoked = false;
            if (IsMasterKey)	// Master key
            {
                while (!revoked && (ns < keySigs.Count))
                {
                    if (((PgpSignature)keySigs[ns++]).SignatureType == PgpSignature.KeyRevocation)
                    {
                        revoked = true;
                    }
                }
            }
            else	// Sub-key
            {
                while (!revoked && (ns < subSigs.Count))
                {
                    if (((PgpSignature)subSigs[ns++]).SignatureType == PgpSignature.SubkeyRevocation)
                    {
                        revoked = true;
                    }
                }
            }
            return revoked;
        }

        /// <summary>Add a certification for an id to the given public key.</summary>
        /// <param name="key">The key the certification is to be added to.</param>
        /// <param name="id">The ID the certification is associated with.</param>
        /// <param name="certification">The new certification.</param>
        /// <returns>The re-certified key.</returns>
        public static PgpPublicKey AddCertification(
            PgpPublicKey	key,
            string			id,
            PgpSignature	certification)
        {
            return AddCert(key, id, certification);
        }

        /// <summary>Add a certification for the given UserAttributeSubpackets to the given public key.</summary>
        /// <param name="key">The key the certification is to be added to.</param>
        /// <param name="userAttributes">The attributes the certification is associated with.</param>
        /// <param name="certification">The new certification.</param>
        /// <returns>The re-certified key.</returns>
        public static PgpPublicKey AddCertification(
            PgpPublicKey					key,
            PgpUserAttributeSubpacketVector	userAttributes,
            PgpSignature					certification)
        {
            return AddCert(key, userAttributes, certification);
        }

        private static PgpPublicKey AddCert(
            PgpPublicKey	key,
            object			id,
            PgpSignature	certification)
        {
            PgpPublicKey returnKey = new PgpPublicKey(key);
            IList sigList = null;

            for (int i = 0; i != returnKey.ids.Count; i++)
            {
                if (id.Equals(returnKey.ids[i]))
                {
                    sigList = (IList) returnKey.idSigs[i];
                }
            }

            if (sigList != null)
            {
                sigList.Add(certification);
            }
            else
            {
                sigList = Platform.CreateArrayList();
                sigList.Add(certification);
                returnKey.ids.Add(id);
                returnKey.idTrusts.Add(null);
                returnKey.idSigs.Add(sigList);
            }

            return returnKey;
        }

        /// <summary>
        /// Remove any certifications associated with a user attribute subpacket on a key.
        /// </summary>
        /// <param name="key">The key the certifications are to be removed from.</param>
        /// <param name="userAttributes">The attributes to be removed.</param>
        /// <returns>
        /// The re-certified key, or null if the user attribute subpacket was not found on the key.
        /// </returns>
        public static PgpPublicKey RemoveCertification(
            PgpPublicKey					key,
            PgpUserAttributeSubpacketVector	userAttributes)
        {
            return RemoveCert(key, userAttributes);
        }

        /// <summary>Remove any certifications associated with a given ID on a key.</summary>
        /// <param name="key">The key the certifications are to be removed from.</param>
        /// <param name="id">The ID that is to be removed.</param>
        /// <returns>The re-certified key, or null if the ID was not found on the key.</returns>
        public static PgpPublicKey RemoveCertification(
            PgpPublicKey	key,
            string			id)
        {
            return RemoveCert(key, id);
        }

        private static PgpPublicKey RemoveCert(
            PgpPublicKey	key,
            object			id)
        {
            PgpPublicKey returnKey = new PgpPublicKey(key);
            bool found = false;

            for (int i = 0; i < returnKey.ids.Count; i++)
            {
                if (id.Equals(returnKey.ids[i]))
                {
                    found = true;
                    returnKey.ids.RemoveAt(i);
                    returnKey.idTrusts.RemoveAt(i);
                    returnKey.idSigs.RemoveAt(i);
                }
            }

            return found ? returnKey : null;
        }

        /// <summary>Remove a certification associated with a given ID on a key.</summary>
        /// <param name="key">The key the certifications are to be removed from.</param>
        /// <param name="id">The ID that the certfication is to be removed from.</param>
        /// <param name="certification">The certfication to be removed.</param>
        /// <returns>The re-certified key, or null if the certification was not found.</returns>
        public static PgpPublicKey RemoveCertification(
            PgpPublicKey	key,
            string			id,
            PgpSignature	certification)
        {
            return RemoveCert(key, id, certification);
        }

        /// <summary>Remove a certification associated with a given user attributes on a key.</summary>
        /// <param name="key">The key the certifications are to be removed from.</param>
        /// <param name="userAttributes">The user attributes that the certfication is to be removed from.</param>
        /// <param name="certification">The certification to be removed.</param>
        /// <returns>The re-certified key, or null if the certification was not found.</returns>
        public static PgpPublicKey RemoveCertification(
            PgpPublicKey					key,
            PgpUserAttributeSubpacketVector	userAttributes,
            PgpSignature					certification)
        {
            return RemoveCert(key, userAttributes, certification);
        }

        private static PgpPublicKey RemoveCert(
            PgpPublicKey	key,
            object			id,
            PgpSignature	certification)
        {
            PgpPublicKey returnKey = new PgpPublicKey(key);
            bool found = false;

            for (int i = 0; i < returnKey.ids.Count; i++)
            {
                if (id.Equals(returnKey.ids[i]))
                {
                    IList certs = (IList) returnKey.idSigs[i];
                    found = certs.Contains(certification);

                    if (found)
                    {
                        certs.Remove(certification);
                    }
                }
            }

            return found ? returnKey : null;
        }

        /// <summary>Add a revocation or some other key certification to a key.</summary>
        /// <param name="key">The key the revocation is to be added to.</param>
        /// <param name="certification">The key signature to be added.</param>
        /// <returns>The new changed public key object.</returns>
        public static PgpPublicKey AddCertification(
            PgpPublicKey	key,
            PgpSignature	certification)
        {
            if (key.IsMasterKey)
            {
                if (certification.SignatureType == PgpSignature.SubkeyRevocation)
                {
                    throw new ArgumentException("signature type incorrect for master key revocation.");
                }
            }
            else
            {
                if (certification.SignatureType == PgpSignature.KeyRevocation)
                {
                    throw new ArgumentException("signature type incorrect for sub-key revocation.");
                }
            }

            PgpPublicKey returnKey = new PgpPublicKey(key);

            if (returnKey.subSigs != null)
            {
                returnKey.subSigs.Add(certification);
            }
            else
            {
                returnKey.keySigs.Add(certification);
            }

            return returnKey;
        }

        /// <summary>Remove a certification from the key.</summary>
        /// <param name="key">The key the certifications are to be removed from.</param>
        /// <param name="certification">The certfication to be removed.</param>
        /// <returns>The modified key, null if the certification was not found.</returns>
        public static PgpPublicKey RemoveCertification(
            PgpPublicKey	key,
            PgpSignature	certification)
        {
            PgpPublicKey returnKey = new PgpPublicKey(key);
            IList sigs = returnKey.subSigs != null
                ?	returnKey.subSigs
                :	returnKey.keySigs;

//			bool found = sigs.Remove(certification);
            int pos = sigs.IndexOf(certification);
            bool found = pos >= 0;

            if (found)
            {
                sigs.RemoveAt(pos);
            }
            else
            {
                foreach (String id in key.GetUserIds())
                {
                    foreach (object sig in key.GetSignaturesForId(id))
                    {
                        // TODO Is this the right type of equality test?
                        if (certification == sig)
                        {
                            found = true;
                            returnKey = PgpPublicKey.RemoveCertification(returnKey, id, certification);
                        }
                    }
                }

                if (!found)
                {
                    foreach (PgpUserAttributeSubpacketVector id in key.GetUserAttributes())
                    {
                        foreach (object sig in key.GetSignaturesForUserAttribute(id))
                        {
                            // TODO Is this the right type of equality test?
                            if (certification == sig)
                            {
                                found = true;
                                returnKey = PgpPublicKey.RemoveCertification(returnKey, id, certification);
                            }
                        }
                    }
                }
            }

            return returnKey;
        }
    }
}
