using System;
using System.Collections;
using System.IO;
using System.Text;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.Utilities;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.X509;

namespace Org.BouncyCastle.Pkcs
{
    public class Pkcs12Store
    {
        private readonly IgnoresCaseHashtable	keys = new IgnoresCaseHashtable();
        private readonly IDictionary            localIds = Platform.CreateHashtable();
        private readonly IgnoresCaseHashtable	certs = new IgnoresCaseHashtable();
        private readonly IDictionary            chainCerts = Platform.CreateHashtable();
        private readonly IDictionary            keyCerts = Platform.CreateHashtable();
        private readonly DerObjectIdentifier	keyAlgorithm;
        private readonly DerObjectIdentifier	certAlgorithm;
        private readonly bool					useDerEncoding;

        private AsymmetricKeyEntry unmarkedKeyEntry = null;

        private const int MinIterations = 1024;
        private const int SaltSize = 20;

        private static SubjectKeyIdentifier CreateSubjectKeyID(
            AsymmetricKeyParameter pubKey)
        {
            return new SubjectKeyIdentifier(
                SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(pubKey));
        }

        internal class CertId
        {
            private readonly byte[] id;

            internal CertId(
                AsymmetricKeyParameter pubKey)
            {
                this.id = CreateSubjectKeyID(pubKey).GetKeyIdentifier();
            }

            internal CertId(
                byte[] id)
            {
                this.id = id;
            }

            internal byte[] Id
            {
                get { return id; }
            }

            public override int GetHashCode()
            {
                return Arrays.GetHashCode(id);
            }

            public override bool Equals(
                object obj)
            {
                if (obj == this)
                    return true;

                CertId other = obj as CertId;

                if (other == null)
                    return false;

                return Arrays.AreEqual(id, other.id);
            }
        }

        internal Pkcs12Store(
            DerObjectIdentifier	keyAlgorithm,
            DerObjectIdentifier	certAlgorithm,
            bool				useDerEncoding)
        {
            this.keyAlgorithm = keyAlgorithm;
            this.certAlgorithm = certAlgorithm;
            this.useDerEncoding = useDerEncoding;
        }

        // TODO Consider making obsolete
//		[Obsolete("Use 'Pkcs12StoreBuilder' instead")]
        public Pkcs12Store()
            : this(PkcsObjectIdentifiers.PbeWithShaAnd3KeyTripleDesCbc,
                PkcsObjectIdentifiers.PbewithShaAnd40BitRC2Cbc, false)
        {
        }

        // TODO Consider making obsolete
//		[Obsolete("Use 'Pkcs12StoreBuilder' and 'Load' method instead")]
        public Pkcs12Store(
            Stream	input,
            char[]	password)
            : this()
        {
            Load(input, password);
        }

        protected virtual void LoadKeyBag(PrivateKeyInfo privKeyInfo, Asn1Set bagAttributes)
        {
            AsymmetricKeyParameter privKey = PrivateKeyFactory.CreateKey(privKeyInfo);

            IDictionary attributes = Platform.CreateHashtable();
            AsymmetricKeyEntry keyEntry = new AsymmetricKeyEntry(privKey, attributes);

            string alias = null;
            Asn1OctetString localId = null;

            if (bagAttributes != null)
            {
                foreach (Asn1Sequence sq in bagAttributes)
                {
                    DerObjectIdentifier aOid = DerObjectIdentifier.GetInstance(sq[0]);
                    Asn1Set attrSet = Asn1Set.GetInstance(sq[1]);
                    Asn1Encodable attr = null;

                    if (attrSet.Count > 0)
                    {
                        // TODO We should be adding all attributes in the set
                        attr = attrSet[0];

                        // TODO We might want to "merge" attribute sets with
                        // the same OID - currently, differing values give an error
                        if (attributes.Contains(aOid.Id))
                        {
                            // OK, but the value has to be the same
                            if (!attributes[aOid.Id].Equals(attr))
                                throw new IOException("attempt to add existing attribute with different value");
                        }
                        else
                        {
                            attributes.Add(aOid.Id, attr);
                        }

                        if (aOid.Equals(PkcsObjectIdentifiers.Pkcs9AtFriendlyName))
                        {
                            alias = ((DerBmpString)attr).GetString();
                            // TODO Do these in a separate loop, just collect aliases here
                            keys[alias] = keyEntry;
                        }
                        else if (aOid.Equals(PkcsObjectIdentifiers.Pkcs9AtLocalKeyID))
                        {
                            localId = (Asn1OctetString)attr;
                        }
                    }
                }
            }

            if (localId != null)
            {
                string name = Hex.ToHexString(localId.GetOctets());

                if (alias == null)
                {
                    keys[name] = keyEntry;
                }
                else
                {
                    // TODO There may have been more than one alias
                    localIds[alias] = name;
                }
            }
            else
            {
                unmarkedKeyEntry = keyEntry;
            }
        }

        protected virtual void LoadPkcs8ShroudedKeyBag(EncryptedPrivateKeyInfo encPrivKeyInfo, Asn1Set bagAttributes,
            char[] password, bool wrongPkcs12Zero)
        {
            if (password != null)
            {
                PrivateKeyInfo privInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(
                    password, wrongPkcs12Zero, encPrivKeyInfo);

                LoadKeyBag(privInfo, bagAttributes);
            }
        }

        public void Load(
            Stream	input,
            char[]	password)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            Asn1Sequence obj = (Asn1Sequence) Asn1Object.FromStream(input);
            Pfx bag = new Pfx(obj);
            ContentInfo info = bag.AuthSafe;
            bool wrongPkcs12Zero = false;

            if (password != null && bag.MacData != null) // check the mac code
            {
                MacData mData = bag.MacData;
                DigestInfo dInfo = mData.Mac;
                AlgorithmIdentifier algId = dInfo.AlgorithmID;
                byte[] salt = mData.GetSalt();
                int itCount = mData.IterationCount.IntValue;

                byte[] data = ((Asn1OctetString) info.Content).GetOctets();

                byte[] mac = CalculatePbeMac(algId.Algorithm, salt, itCount, password, false, data);
                byte[] dig = dInfo.GetDigest();

                if (!Arrays.ConstantTimeAreEqual(mac, dig))
                {
                    if (password.Length > 0)
                        throw new IOException("PKCS12 key store MAC invalid - wrong password or corrupted file.");

                    // Try with incorrect zero length password
                    mac = CalculatePbeMac(algId.Algorithm, salt, itCount, password, true, data);

                    if (!Arrays.ConstantTimeAreEqual(mac, dig))
                        throw new IOException("PKCS12 key store MAC invalid - wrong password or corrupted file.");

                    wrongPkcs12Zero = true;
                }
            }

            keys.Clear();
            localIds.Clear();
            unmarkedKeyEntry = null;

            IList certBags = Platform.CreateArrayList();

            if (info.ContentType.Equals(PkcsObjectIdentifiers.Data))
            {
                byte[] octs = ((Asn1OctetString)info.Content).GetOctets();
                AuthenticatedSafe authSafe = new AuthenticatedSafe(
                    (Asn1Sequence) Asn1OctetString.FromByteArray(octs));
                ContentInfo[] cis = authSafe.GetContentInfo();

                foreach (ContentInfo ci in cis)
                {
                    DerObjectIdentifier oid = ci.ContentType;

                    byte[] octets = null;
                    if (oid.Equals(PkcsObjectIdentifiers.Data))
                    {
                        octets = ((Asn1OctetString)ci.Content).GetOctets();
                    }
                    else if (oid.Equals(PkcsObjectIdentifiers.EncryptedData))
                    {
                        if (password != null)
                        {
                            EncryptedData d = EncryptedData.GetInstance(ci.Content);
                            octets = CryptPbeData(false, d.EncryptionAlgorithm,
                                password, wrongPkcs12Zero, d.Content.GetOctets());
                        }
                    }
                    else
                    {
                        // TODO Other data types
                    }

                    if (octets != null)
                    {
                        Asn1Sequence seq = (Asn1Sequence)Asn1Object.FromByteArray(octets);

                        foreach (Asn1Sequence subSeq in seq)
                        {
                            SafeBag b = new SafeBag(subSeq);

                            if (b.BagID.Equals(PkcsObjectIdentifiers.CertBag))
                            {
                                certBags.Add(b);
                            }
                            else if (b.BagID.Equals(PkcsObjectIdentifiers.Pkcs8ShroudedKeyBag))
                            {
                                LoadPkcs8ShroudedKeyBag(EncryptedPrivateKeyInfo.GetInstance(b.BagValue),
                                    b.BagAttributes, password, wrongPkcs12Zero);
                            }
                            else if (b.BagID.Equals(PkcsObjectIdentifiers.KeyBag))
                            {
                                LoadKeyBag(PrivateKeyInfo.GetInstance(b.BagValue), b.BagAttributes);
                            }
                            else
                            {
                                // TODO Other bag types
                            }
                        }
                    }
                }
            }

            certs.Clear();
            chainCerts.Clear();
            keyCerts.Clear();

            foreach (SafeBag b in certBags)
            {
                CertBag certBag = new CertBag((Asn1Sequence)b.BagValue);
                byte[] octets = ((Asn1OctetString)certBag.CertValue).GetOctets();
                X509Certificate cert = new X509CertificateParser().ReadCertificate(octets);

                //
                // set the attributes
                //
                IDictionary attributes = Platform.CreateHashtable();
                Asn1OctetString localId = null;
                string alias = null;

                if (b.BagAttributes != null)
                {
                    foreach (Asn1Sequence sq in b.BagAttributes)
                    {
                        DerObjectIdentifier aOid = DerObjectIdentifier.GetInstance(sq[0]);
                        Asn1Set attrSet = Asn1Set.GetInstance(sq[1]);

                        if (attrSet.Count > 0)
                        {
                            // TODO We should be adding all attributes in the set
                            Asn1Encodable attr = attrSet[0];

                            // TODO We might want to "merge" attribute sets with
                            // the same OID - currently, differing values give an error
                            if (attributes.Contains(aOid.Id))
                            {
                                // OK, but the value has to be the same
                                if (!attributes[aOid.Id].Equals(attr))
                                {
                                    throw new IOException("attempt to add existing attribute with different value");
                                }
                            }
                            else
                            {
                                attributes.Add(aOid.Id, attr);
                            }

                            if (aOid.Equals(PkcsObjectIdentifiers.Pkcs9AtFriendlyName))
                            {
                                alias = ((DerBmpString)attr).GetString();
                            }
                            else if (aOid.Equals(PkcsObjectIdentifiers.Pkcs9AtLocalKeyID))
                            {
                                localId = (Asn1OctetString)attr;
                            }
                        }
                    }
                }

                CertId certId = new CertId(cert.GetPublicKey());
                X509CertificateEntry certEntry = new X509CertificateEntry(cert, attributes);

                chainCerts[certId] = certEntry;

                if (unmarkedKeyEntry != null)
                {
                    if (keyCerts.Count == 0)
                    {
                        string name = Hex.ToHexString(certId.Id);

                        keyCerts[name] = certEntry;
                        keys[name] = unmarkedKeyEntry;
                    }
                }
                else
                {
                    if (localId != null)
                    {
                        string name = Hex.ToHexString(localId.GetOctets());

                        keyCerts[name] = certEntry;
                    }

                    if (alias != null)
                    {
                        // TODO There may have been more than one alias
                        certs[alias] = certEntry;
                    }
                }
            }
        }

        public AsymmetricKeyEntry GetKey(
            string alias)
        {
            if (alias == null)
                throw new ArgumentNullException("alias");

            return (AsymmetricKeyEntry)keys[alias];
        }

        public bool IsCertificateEntry(
            string alias)
        {
            if (alias == null)
                throw new ArgumentNullException("alias");

            return (certs[alias] != null && keys[alias] == null);
        }

        public bool IsKeyEntry(
            string alias)
        {
            if (alias == null)
                throw new ArgumentNullException("alias");

            return (keys[alias] != null);
        }

        private IDictionary GetAliasesTable()
        {
            IDictionary tab = Platform.CreateHashtable();

            foreach (string key in certs.Keys)
            {
                tab[key] = "cert";
            }

            foreach (string a in keys.Keys)
            {
                if (tab[a] == null)
                {
                    tab[a] = "key";
                }
            }

            return tab;
        }

        public IEnumerable Aliases
        {
            get { return new EnumerableProxy(GetAliasesTable().Keys); }
        }

        public bool ContainsAlias(
            string alias)
        {
            return certs[alias] != null || keys[alias] != null;
        }

        /**
         * simply return the cert entry for the private key
         */
        public X509CertificateEntry GetCertificate(
            string alias)
        {
            if (alias == null)
                throw new ArgumentNullException("alias");

            X509CertificateEntry c = (X509CertificateEntry) certs[alias];

            //
            // look up the key table - and try the local key id
            //
            if (c == null)
            {
                string id = (string)localIds[alias];
                if (id != null)
                {
                    c = (X509CertificateEntry)keyCerts[id];
                }
                else
                {
                    c = (X509CertificateEntry)keyCerts[alias];
                }
            }

            return c;
        }

        public string GetCertificateAlias(
            X509Certificate cert)
        {
            if (cert == null)
                throw new ArgumentNullException("cert");

            foreach (DictionaryEntry entry in certs)
            {
                X509CertificateEntry entryValue = (X509CertificateEntry) entry.Value;
                if (entryValue.Certificate.Equals(cert))
                {
                    return (string) entry.Key;
                }
            }

            foreach (DictionaryEntry entry in keyCerts)
            {
                X509CertificateEntry entryValue = (X509CertificateEntry) entry.Value;
                if (entryValue.Certificate.Equals(cert))
                {
                    return (string) entry.Key;
                }
            }

            return null;
        }

        public X509CertificateEntry[] GetCertificateChain(
            string alias)
        {
            if (alias == null)
                throw new ArgumentNullException("alias");

            if (!IsKeyEntry(alias))
            {
                return null;
            }

            X509CertificateEntry c = GetCertificate(alias);

            if (c != null)
            {
                IList cs = Platform.CreateArrayList();

                while (c != null)
                {
                    X509Certificate x509c = c.Certificate;
                    X509CertificateEntry nextC = null;

                    Asn1OctetString ext = x509c.GetExtensionValue(X509Extensions.AuthorityKeyIdentifier);
                    if (ext != null)
                    {
                        AuthorityKeyIdentifier id = AuthorityKeyIdentifier.GetInstance(
                            Asn1Object.FromByteArray(ext.GetOctets()));

                        if (id.GetKeyIdentifier() != null)
                        {
                            nextC = (X509CertificateEntry) chainCerts[new CertId(id.GetKeyIdentifier())];
                        }
                    }

                    if (nextC == null)
                    {
                        //
                        // no authority key id, try the Issuer DN
                        //
                        X509Name i = x509c.IssuerDN;
                        X509Name s = x509c.SubjectDN;

                        if (!i.Equivalent(s))
                        {
                            foreach (CertId certId in chainCerts.Keys)
                            {
                                X509CertificateEntry x509CertEntry = (X509CertificateEntry) chainCerts[certId];

                                X509Certificate crt = x509CertEntry.Certificate;

                                X509Name sub = crt.SubjectDN;
                                if (sub.Equivalent(i))
                                {
                                    try
                                    {
                                        x509c.Verify(crt.GetPublicKey());

                                        nextC = x509CertEntry;
                                        break;
                                    }
                                    catch (InvalidKeyException)
                                    {
                                        // TODO What if it doesn't verify?
                                    }
                                }
                            }
                        }
                    }

                    cs.Add(c);
                    if (nextC != c) // self signed - end of the chain
                    {
                        c = nextC;
                    }
                    else
                    {
                        c = null;
                    }
                }

                X509CertificateEntry[] result = new X509CertificateEntry[cs.Count];
                for (int i = 0; i < cs.Count; ++i)
                {
                    result[i] = (X509CertificateEntry)cs[i];
                }
                return result;
            }

            return null;
        }

        public void SetCertificateEntry(
            string					alias,
            X509CertificateEntry	certEntry)
        {
            if (alias == null)
                throw new ArgumentNullException("alias");
            if (certEntry == null)
                throw new ArgumentNullException("certEntry");
            if (keys[alias] != null)
                throw new ArgumentException("There is a key entry with the name " + alias + ".");

            certs[alias] = certEntry;
            chainCerts[new CertId(certEntry.Certificate.GetPublicKey())] = certEntry;
        }

        public void SetKeyEntry(
            string					alias,
            AsymmetricKeyEntry		keyEntry,
            X509CertificateEntry[]	chain)
        {
            if (alias == null)
                throw new ArgumentNullException("alias");
            if (keyEntry == null)
                throw new ArgumentNullException("keyEntry");
            if (keyEntry.Key.IsPrivate && (chain == null))
                throw new ArgumentException("No certificate chain for private key");

            if (keys[alias] != null)
            {
                DeleteEntry(alias);
            }

            keys[alias] = keyEntry;
            certs[alias] = chain[0];

            for (int i = 0; i != chain.Length; i++)
            {
                chainCerts[new CertId(chain[i].Certificate.GetPublicKey())] = chain[i];
            }
        }

        public void DeleteEntry(
            string alias)
        {
            if (alias == null)
                throw new ArgumentNullException("alias");

            AsymmetricKeyEntry k = (AsymmetricKeyEntry)keys[alias];
            if (k != null)
            {
                keys.Remove(alias);
            }

            X509CertificateEntry c = (X509CertificateEntry)certs[alias];

            if (c != null)
            {
                certs.Remove(alias);
                chainCerts.Remove(new CertId(c.Certificate.GetPublicKey()));
            }

            if (k != null)
            {
                string id = (string)localIds[alias];
                if (id != null)
                {
                    localIds.Remove(alias);
                    c = (X509CertificateEntry)keyCerts[id];
                }
                if (c != null)
                {
                    keyCerts.Remove(id);
                    chainCerts.Remove(new CertId(c.Certificate.GetPublicKey()));
                }
            }

            if (c == null && k == null)
            {
                throw new ArgumentException("no such entry as " + alias);
            }
        }

        public bool IsEntryOfType(
            string	alias,
            Type	entryType)
        {
            if (entryType == typeof(X509CertificateEntry))
                return IsCertificateEntry(alias);

            if (entryType == typeof(AsymmetricKeyEntry))
                return IsKeyEntry(alias) && GetCertificate(alias) != null;

            return false;
        }

        [Obsolete("Use 'Count' property instead")]
        public int Size()
        {
            return Count;
        }

        public int Count
        {
            // TODO Seems a little inefficient
            get { return GetAliasesTable().Count; }
        }

        public void Save(
            Stream			stream,
            char[]			password,
            SecureRandom	random)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (random == null)
                throw new ArgumentNullException("random");

            //
            // handle the keys
            //
            Asn1EncodableVector keyBags = new Asn1EncodableVector();
            foreach (string name in keys.Keys)
            {
                byte[] kSalt = new byte[SaltSize];
                random.NextBytes(kSalt);

                AsymmetricKeyEntry privKey = (AsymmetricKeyEntry)keys[name];

                DerObjectIdentifier bagOid;
                Asn1Encodable bagData;

                if (password == null)
                {
                    bagOid = PkcsObjectIdentifiers.KeyBag;
                    bagData = PrivateKeyInfoFactory.CreatePrivateKeyInfo(privKey.Key);
                }
                else
                {
                    bagOid = PkcsObjectIdentifiers.Pkcs8ShroudedKeyBag;
                    bagData = EncryptedPrivateKeyInfoFactory.CreateEncryptedPrivateKeyInfo(
                        keyAlgorithm, password, kSalt, MinIterations, privKey.Key);
                }

                Asn1EncodableVector kName = new Asn1EncodableVector();

                foreach (string oid in privKey.BagAttributeKeys)
                {
                    Asn1Encodable entry = privKey[oid];

                    // NB: Ignore any existing FriendlyName
                    if (oid.Equals(PkcsObjectIdentifiers.Pkcs9AtFriendlyName.Id))
                        continue;

                    kName.Add(
                        new DerSequence(
                            new DerObjectIdentifier(oid),
                            new DerSet(entry)));
                }

                //
                // make sure we are using the local alias on store
                //
                // NB: We always set the FriendlyName based on 'name'
                //if (privKey[PkcsObjectIdentifiers.Pkcs9AtFriendlyName] == null)
                {
                    kName.Add(
                        new DerSequence(
                            PkcsObjectIdentifiers.Pkcs9AtFriendlyName,
                            new DerSet(new DerBmpString(name))));
                }

                //
                // make sure we have a local key-id
                //
                if (privKey[PkcsObjectIdentifiers.Pkcs9AtLocalKeyID] == null)
                {
                    X509CertificateEntry ct = GetCertificate(name);
                    AsymmetricKeyParameter pubKey = ct.Certificate.GetPublicKey();
                    SubjectKeyIdentifier subjectKeyID = CreateSubjectKeyID(pubKey);

                    kName.Add(
                        new DerSequence(
                            PkcsObjectIdentifiers.Pkcs9AtLocalKeyID,
                            new DerSet(subjectKeyID)));
                }

                keyBags.Add(new SafeBag(bagOid, bagData.ToAsn1Object(), new DerSet(kName)));
            }

            byte[] keyBagsEncoding = new DerSequence(keyBags).GetDerEncoded();
            ContentInfo keysInfo = new ContentInfo(PkcsObjectIdentifiers.Data, new BerOctetString(keyBagsEncoding));

            //
            // certificate processing
            //
            byte[] cSalt = new byte[SaltSize];

            random.NextBytes(cSalt);

            Asn1EncodableVector	certBags = new Asn1EncodableVector();
            Pkcs12PbeParams		cParams = new Pkcs12PbeParams(cSalt, MinIterations);
            AlgorithmIdentifier	cAlgId = new AlgorithmIdentifier(certAlgorithm, cParams.ToAsn1Object());
            ISet				doneCerts = new HashSet();

            foreach (string name in keys.Keys)
            {
                X509CertificateEntry certEntry = GetCertificate(name);
                CertBag cBag = new CertBag(
                    PkcsObjectIdentifiers.X509Certificate,
                    new DerOctetString(certEntry.Certificate.GetEncoded()));

                Asn1EncodableVector fName = new Asn1EncodableVector();

                foreach (string oid in certEntry.BagAttributeKeys)
                {
                    Asn1Encodable entry = certEntry[oid];

                    // NB: Ignore any existing FriendlyName
                    if (oid.Equals(PkcsObjectIdentifiers.Pkcs9AtFriendlyName.Id))
                        continue;

                    fName.Add(
                        new DerSequence(
                            new DerObjectIdentifier(oid),
                            new DerSet(entry)));
                }

                //
                // make sure we are using the local alias on store
                //
                // NB: We always set the FriendlyName based on 'name'
                //if (certEntry[PkcsObjectIdentifiers.Pkcs9AtFriendlyName] == null)
                {
                    fName.Add(
                        new DerSequence(
                            PkcsObjectIdentifiers.Pkcs9AtFriendlyName,
                            new DerSet(new DerBmpString(name))));
                }

                //
                // make sure we have a local key-id
                //
                if (certEntry[PkcsObjectIdentifiers.Pkcs9AtLocalKeyID] == null)
                {
                    AsymmetricKeyParameter pubKey = certEntry.Certificate.GetPublicKey();
                    SubjectKeyIdentifier subjectKeyID = CreateSubjectKeyID(pubKey);

                    fName.Add(
                        new DerSequence(
                            PkcsObjectIdentifiers.Pkcs9AtLocalKeyID,
                            new DerSet(subjectKeyID)));
                }

                certBags.Add(new SafeBag(PkcsObjectIdentifiers.CertBag, cBag.ToAsn1Object(), new DerSet(fName)));

                doneCerts.Add(certEntry.Certificate);
            }

            foreach (string certId in certs.Keys)
            {
                X509CertificateEntry cert = (X509CertificateEntry)certs[certId];

                if (keys[certId] != null)
                    continue;

                CertBag cBag = new CertBag(
                    PkcsObjectIdentifiers.X509Certificate,
                    new DerOctetString(cert.Certificate.GetEncoded()));

                Asn1EncodableVector fName = new Asn1EncodableVector();

                foreach (string oid in cert.BagAttributeKeys)
                {
                    // a certificate not immediately linked to a key doesn't require
                    // a localKeyID and will confuse some PKCS12 implementations.
                    //
                    // If we find one, we'll prune it out.
                    if (oid.Equals(PkcsObjectIdentifiers.Pkcs9AtLocalKeyID.Id))
                        continue;

                    Asn1Encodable entry = cert[oid];

                    // NB: Ignore any existing FriendlyName
                    if (oid.Equals(PkcsObjectIdentifiers.Pkcs9AtFriendlyName.Id))
                        continue;

                    fName.Add(
                        new DerSequence(
                            new DerObjectIdentifier(oid),
                            new DerSet(entry)));
                }

                //
                // make sure we are using the local alias on store
                //
                // NB: We always set the FriendlyName based on 'certId'
                //if (cert[PkcsObjectIdentifiers.Pkcs9AtFriendlyName] == null)
                {
                    fName.Add(
                        new DerSequence(
                            PkcsObjectIdentifiers.Pkcs9AtFriendlyName,
                            new DerSet(new DerBmpString(certId))));
                }

                certBags.Add(new SafeBag(PkcsObjectIdentifiers.CertBag, cBag.ToAsn1Object(), new DerSet(fName)));

                doneCerts.Add(cert.Certificate);
            }

            foreach (CertId certId in chainCerts.Keys)
            {
                X509CertificateEntry cert = (X509CertificateEntry)chainCerts[certId];

                if (doneCerts.Contains(cert.Certificate))
                    continue;

                CertBag cBag = new CertBag(
                    PkcsObjectIdentifiers.X509Certificate,
                    new DerOctetString(cert.Certificate.GetEncoded()));

                Asn1EncodableVector fName = new Asn1EncodableVector();

                foreach (string oid in cert.BagAttributeKeys)
                {
                    // a certificate not immediately linked to a key doesn't require
                    // a localKeyID and will confuse some PKCS12 implementations.
                    //
                    // If we find one, we'll prune it out.
                    if (oid.Equals(PkcsObjectIdentifiers.Pkcs9AtLocalKeyID.Id))
                        continue;

                    fName.Add(
                        new DerSequence(
                            new DerObjectIdentifier(oid),
                            new DerSet(cert[oid])));
                }

                certBags.Add(new SafeBag(PkcsObjectIdentifiers.CertBag, cBag.ToAsn1Object(), new DerSet(fName)));
            }

            byte[] certBagsEncoding = new DerSequence(certBags).GetDerEncoded();

            ContentInfo certsInfo;
            if (password == null)
            {
                certsInfo = new ContentInfo(PkcsObjectIdentifiers.Data, new BerOctetString(certBagsEncoding));
            }
            else
            {
                byte[] certBytes = CryptPbeData(true, cAlgId, password, false, certBagsEncoding);
                EncryptedData cInfo = new EncryptedData(PkcsObjectIdentifiers.Data, cAlgId, new BerOctetString(certBytes));
                certsInfo = new ContentInfo(PkcsObjectIdentifiers.EncryptedData, cInfo.ToAsn1Object());
            }

            ContentInfo[] info = new ContentInfo[]{ keysInfo, certsInfo };

            byte[] data = new AuthenticatedSafe(info).GetEncoded(
                useDerEncoding ? Asn1Encodable.Der : Asn1Encodable.Ber);

            ContentInfo mainInfo = new ContentInfo(PkcsObjectIdentifiers.Data, new BerOctetString(data));

            //
            // create the mac
            //
            MacData macData = null;
            if (password != null)
            {
                byte[] mSalt = new byte[20];
                random.NextBytes(mSalt);

                byte[] mac = CalculatePbeMac(OiwObjectIdentifiers.IdSha1,
                    mSalt, MinIterations, password, false, data);

                AlgorithmIdentifier algId = new AlgorithmIdentifier(
                    OiwObjectIdentifiers.IdSha1, DerNull.Instance);
                DigestInfo dInfo = new DigestInfo(algId, mac);

                macData = new MacData(dInfo, mSalt, MinIterations);
            }

            //
            // output the Pfx
            //
            Pfx pfx = new Pfx(mainInfo, macData);

            DerOutputStream derOut;
            if (useDerEncoding)
            {
                derOut = new DerOutputStream(stream);
            }
            else
            {
                derOut = new BerOutputStream(stream);
            }

            derOut.WriteObject(pfx);
        }

        internal static byte[] CalculatePbeMac(
            DerObjectIdentifier	oid,
            byte[]				salt,
            int					itCount,
            char[]				password,
            bool				wrongPkcs12Zero,
            byte[]				data)
        {
            Asn1Encodable asn1Params = PbeUtilities.GenerateAlgorithmParameters(
                oid, salt, itCount);
            ICipherParameters cipherParams = PbeUtilities.GenerateCipherParameters(
                oid, password, wrongPkcs12Zero, asn1Params);

            IMac mac = (IMac) PbeUtilities.CreateEngine(oid);
            mac.Init(cipherParams);
            return MacUtilities.DoFinal(mac, data);
        }

        private static byte[] CryptPbeData(
            bool				forEncryption,
            AlgorithmIdentifier	algId,
            char[]				password,
            bool				wrongPkcs12Zero,
            byte[]				data)
        {
            IBufferedCipher cipher = PbeUtilities.CreateEngine(algId.Algorithm) as IBufferedCipher;

            if (cipher == null)
                throw new Exception("Unknown encryption algorithm: " + algId.Algorithm);

            Pkcs12PbeParams pbeParameters = Pkcs12PbeParams.GetInstance(algId.Parameters);
            ICipherParameters cipherParams = PbeUtilities.GenerateCipherParameters(
                algId.Algorithm, password, wrongPkcs12Zero, pbeParameters);
            cipher.Init(forEncryption, cipherParams);
            return cipher.DoFinal(data);
        }

        private class IgnoresCaseHashtable
            : IEnumerable
        {
            private readonly IDictionary orig = Platform.CreateHashtable();
            private readonly IDictionary keys = Platform.CreateHashtable();

            public void Clear()
            {
                orig.Clear();
                keys.Clear();
            }

            public IEnumerator GetEnumerator()
            {
                return orig.GetEnumerator();
            }

            public ICollection Keys
            {
                get { return orig.Keys; }
            }

            public object Remove(
                string alias)
            {
                string upper = Platform.ToUpperInvariant(alias);
                string k = (string)keys[upper];

                if (k == null)
                    return null;

                keys.Remove(upper);

                object o = orig[k];
                orig.Remove(k);
                return o;
            }

            public object this[
                string alias]
            {
                get
                {
                    string upper = Platform.ToUpperInvariant(alias);
                    string k = (string)keys[upper];

                    if (k == null)
                        return null;

                    return orig[k];
                }
                set
                {
                    string upper = Platform.ToUpperInvariant(alias);
                    string k = (string)keys[upper];
                    if (k != null)
                    {
                        orig.Remove(k);
                    }
                    keys[upper] = alias;
                    orig[alias] = value;
                }
            }

            public ICollection Values
            {
                get { return orig.Values; }
            }
        }
    }
}
