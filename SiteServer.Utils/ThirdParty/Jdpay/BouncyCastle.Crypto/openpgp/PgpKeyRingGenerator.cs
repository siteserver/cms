using System;
using System.Collections;

using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Bcpg.OpenPgp
{
	/// <remarks>
	/// Generator for a PGP master and subkey ring.
	/// This class will generate both the secret and public key rings
	/// </remarks>
    public class PgpKeyRingGenerator
    {
        private IList					    keys = Platform.CreateArrayList();
        private string                      id;
        private SymmetricKeyAlgorithmTag	encAlgorithm;
        private HashAlgorithmTag            hashAlgorithm;
        private int                         certificationLevel;
        private byte[]                      rawPassPhrase;
		private bool						useSha1;
		private PgpKeyPair                  masterKey;
        private PgpSignatureSubpacketVector hashedPacketVector;
        private PgpSignatureSubpacketVector unhashedPacketVector;
        private SecureRandom				rand;

		/// <summary>
		/// Create a new key ring generator using old style checksumming. It is recommended to use
		/// SHA1 checksumming where possible.
		/// </summary>
        /// <remarks>
        /// Conversion of the passphrase characters to bytes is performed using Convert.ToByte(), which is
        /// the historical behaviour of the library (1.7 and earlier).
        /// </remarks>
        /// <param name="certificationLevel">The certification level for keys on this ring.</param>
		/// <param name="masterKey">The master key pair.</param>
		/// <param name="id">The id to be associated with the ring.</param>
		/// <param name="encAlgorithm">The algorithm to be used to protect secret keys.</param>
		/// <param name="passPhrase">The passPhrase to be used to protect secret keys.</param>
		/// <param name="hashedPackets">Packets to be included in the certification hash.</param>
		/// <param name="unhashedPackets">Packets to be attached unhashed to the certification.</param>
		/// <param name="rand">input secured random.</param>
        [Obsolete("Use version taking an explicit 'useSha1' parameter instead")]
		public PgpKeyRingGenerator(
			int							certificationLevel,
			PgpKeyPair					masterKey,
			string						id,
			SymmetricKeyAlgorithmTag	encAlgorithm,
			char[]						passPhrase,
			PgpSignatureSubpacketVector	hashedPackets,
			PgpSignatureSubpacketVector	unhashedPackets,
			SecureRandom				rand)
			: this(certificationLevel, masterKey, id, encAlgorithm, passPhrase, false, hashedPackets, unhashedPackets, rand)
		{
		}

		/// <summary>
		/// Create a new key ring generator.
		/// </summary>
        /// <remarks>
        /// Conversion of the passphrase characters to bytes is performed using Convert.ToByte(), which is
        /// the historical behaviour of the library (1.7 and earlier).
        /// </remarks>
        /// <param name="certificationLevel">The certification level for keys on this ring.</param>
		/// <param name="masterKey">The master key pair.</param>
		/// <param name="id">The id to be associated with the ring.</param>
		/// <param name="encAlgorithm">The algorithm to be used to protect secret keys.</param>
		/// <param name="passPhrase">The passPhrase to be used to protect secret keys.</param>
		/// <param name="useSha1">Checksum the secret keys with SHA1 rather than the older 16 bit checksum.</param>
		/// <param name="hashedPackets">Packets to be included in the certification hash.</param>
		/// <param name="unhashedPackets">Packets to be attached unhashed to the certification.</param>
		/// <param name="rand">input secured random.</param>
        public PgpKeyRingGenerator(
            int							certificationLevel,
            PgpKeyPair					masterKey,
            string						id,
            SymmetricKeyAlgorithmTag	encAlgorithm,
            char[]						passPhrase,
			bool						useSha1,
			PgpSignatureSubpacketVector	hashedPackets,
            PgpSignatureSubpacketVector	unhashedPackets,
            SecureRandom				rand)
            : this(certificationLevel, masterKey, id, encAlgorithm, false, passPhrase, useSha1, hashedPackets, unhashedPackets, rand)
        {
        }

		/// <summary>
		/// Create a new key ring generator.
		/// </summary>
		/// <param name="certificationLevel">The certification level for keys on this ring.</param>
		/// <param name="masterKey">The master key pair.</param>
		/// <param name="id">The id to be associated with the ring.</param>
		/// <param name="encAlgorithm">The algorithm to be used to protect secret keys.</param>
        /// <param name="utf8PassPhrase">
        /// If true, conversion of the passphrase to bytes uses Encoding.UTF8.GetBytes(), otherwise the conversion
        /// is performed using Convert.ToByte(), which is the historical behaviour of the library (1.7 and earlier).
        /// </param>
        /// <param name="passPhrase">The passPhrase to be used to protect secret keys.</param>
		/// <param name="useSha1">Checksum the secret keys with SHA1 rather than the older 16 bit checksum.</param>
		/// <param name="hashedPackets">Packets to be included in the certification hash.</param>
		/// <param name="unhashedPackets">Packets to be attached unhashed to the certification.</param>
		/// <param name="rand">input secured random.</param>
        public PgpKeyRingGenerator(
            int							certificationLevel,
            PgpKeyPair					masterKey,
            string						id,
            SymmetricKeyAlgorithmTag	encAlgorithm,
            bool                        utf8PassPhrase,
            char[]						passPhrase,
			bool						useSha1,
			PgpSignatureSubpacketVector	hashedPackets,
            PgpSignatureSubpacketVector	unhashedPackets,
            SecureRandom				rand)
            : this(certificationLevel, masterKey, id, encAlgorithm,
                PgpUtilities.EncodePassPhrase(passPhrase, utf8PassPhrase),
                useSha1, hashedPackets, unhashedPackets, rand)
        {
        }

        /// <summary>
		/// Create a new key ring generator.
		/// </summary>
		/// <param name="certificationLevel">The certification level for keys on this ring.</param>
		/// <param name="masterKey">The master key pair.</param>
		/// <param name="id">The id to be associated with the ring.</param>
		/// <param name="encAlgorithm">The algorithm to be used to protect secret keys.</param>
		/// <param name="rawPassPhrase">The passPhrase to be used to protect secret keys.</param>
		/// <param name="useSha1">Checksum the secret keys with SHA1 rather than the older 16 bit checksum.</param>
		/// <param name="hashedPackets">Packets to be included in the certification hash.</param>
		/// <param name="unhashedPackets">Packets to be attached unhashed to the certification.</param>
		/// <param name="rand">input secured random.</param>
        public PgpKeyRingGenerator(
            int							certificationLevel,
            PgpKeyPair					masterKey,
            string						id,
            SymmetricKeyAlgorithmTag	encAlgorithm,
            byte[]						rawPassPhrase,
			bool						useSha1,
			PgpSignatureSubpacketVector	hashedPackets,
            PgpSignatureSubpacketVector	unhashedPackets,
            SecureRandom				rand)
        {
            this.certificationLevel = certificationLevel;
            this.masterKey = masterKey;
            this.id = id;
            this.encAlgorithm = encAlgorithm;
            this.rawPassPhrase = rawPassPhrase;
			this.useSha1 = useSha1;
			this.hashedPacketVector = hashedPackets;
            this.unhashedPacketVector = unhashedPackets;
            this.rand = rand;

			keys.Add(new PgpSecretKey(certificationLevel, masterKey, id, encAlgorithm, rawPassPhrase, false, useSha1, hashedPackets, unhashedPackets, rand));
        }

        /// <summary>
        /// Create a new key ring generator.
        /// </summary>
        /// <remarks>
        /// Conversion of the passphrase characters to bytes is performed using Convert.ToByte(), which is
        /// the historical behaviour of the library (1.7 and earlier).
        /// </remarks>
        /// <param name="certificationLevel">The certification level for keys on this ring.</param>
        /// <param name="masterKey">The master key pair.</param>
        /// <param name="id">The id to be associated with the ring.</param>
        /// <param name="encAlgorithm">The algorithm to be used to protect secret keys.</param>
        /// <param name="hashAlgorithm">The hash algorithm.</param>
        /// <param name="passPhrase">The passPhrase to be used to protect secret keys.</param>
        /// <param name="useSha1">Checksum the secret keys with SHA1 rather than the older 16 bit checksum.</param>
        /// <param name="hashedPackets">Packets to be included in the certification hash.</param>
        /// <param name="unhashedPackets">Packets to be attached unhashed to the certification.</param>
        /// <param name="rand">input secured random.</param>
        public PgpKeyRingGenerator(
            int                         certificationLevel,
            PgpKeyPair                  masterKey,
            string                      id,
            SymmetricKeyAlgorithmTag    encAlgorithm,
            HashAlgorithmTag            hashAlgorithm,
            char[]                      passPhrase,
            bool                        useSha1,
            PgpSignatureSubpacketVector hashedPackets,
            PgpSignatureSubpacketVector unhashedPackets,
            SecureRandom                rand)
            : this(certificationLevel, masterKey, id, encAlgorithm, hashAlgorithm, false, passPhrase, useSha1, hashedPackets, unhashedPackets, rand)
        {
        }

        /// <summary>
        /// Create a new key ring generator.
        /// </summary>
        /// <param name="certificationLevel">The certification level for keys on this ring.</param>
        /// <param name="masterKey">The master key pair.</param>
        /// <param name="id">The id to be associated with the ring.</param>
        /// <param name="encAlgorithm">The algorithm to be used to protect secret keys.</param>
        /// <param name="hashAlgorithm">The hash algorithm.</param>
        /// <param name="utf8PassPhrase">
        /// If true, conversion of the passphrase to bytes uses Encoding.UTF8.GetBytes(), otherwise the conversion
        /// is performed using Convert.ToByte(), which is the historical behaviour of the library (1.7 and earlier).
        /// </param>
        /// <param name="passPhrase">The passPhrase to be used to protect secret keys.</param>
        /// <param name="useSha1">Checksum the secret keys with SHA1 rather than the older 16 bit checksum.</param>
        /// <param name="hashedPackets">Packets to be included in the certification hash.</param>
        /// <param name="unhashedPackets">Packets to be attached unhashed to the certification.</param>
        /// <param name="rand">input secured random.</param>
        public PgpKeyRingGenerator(
            int                         certificationLevel,
            PgpKeyPair                  masterKey,
            string                      id,
            SymmetricKeyAlgorithmTag    encAlgorithm,
            HashAlgorithmTag            hashAlgorithm,
            bool                        utf8PassPhrase,
            char[]                      passPhrase,
            bool                        useSha1,
            PgpSignatureSubpacketVector hashedPackets,
            PgpSignatureSubpacketVector unhashedPackets,
            SecureRandom                rand)
            : this(certificationLevel, masterKey, id, encAlgorithm, hashAlgorithm,
                PgpUtilities.EncodePassPhrase(passPhrase, utf8PassPhrase),
                useSha1, hashedPackets, unhashedPackets, rand)
        {
        }

        /// <summary>
        /// Create a new key ring generator.
        /// </summary>
        /// <remarks>
        /// Allows the caller to handle the encoding of the passphrase to bytes.
        /// </remarks>
        /// <param name="certificationLevel">The certification level for keys on this ring.</param>
        /// <param name="masterKey">The master key pair.</param>
        /// <param name="id">The id to be associated with the ring.</param>
        /// <param name="encAlgorithm">The algorithm to be used to protect secret keys.</param>
        /// <param name="hashAlgorithm">The hash algorithm.</param>
        /// <param name="rawPassPhrase">The passPhrase to be used to protect secret keys.</param>
        /// <param name="useSha1">Checksum the secret keys with SHA1 rather than the older 16 bit checksum.</param>
        /// <param name="hashedPackets">Packets to be included in the certification hash.</param>
        /// <param name="unhashedPackets">Packets to be attached unhashed to the certification.</param>
        /// <param name="rand">input secured random.</param>
        public PgpKeyRingGenerator(
            int                         certificationLevel,
            PgpKeyPair                  masterKey,
            string                      id,
            SymmetricKeyAlgorithmTag    encAlgorithm,
            HashAlgorithmTag            hashAlgorithm,
            byte[]                      rawPassPhrase,
            bool                        useSha1,
            PgpSignatureSubpacketVector hashedPackets,
            PgpSignatureSubpacketVector unhashedPackets,
            SecureRandom                rand)
        {
            this.certificationLevel = certificationLevel;
            this.masterKey = masterKey;
            this.id = id;
            this.encAlgorithm = encAlgorithm;
            this.rawPassPhrase = rawPassPhrase;
            this.useSha1 = useSha1;
            this.hashedPacketVector = hashedPackets;
            this.unhashedPacketVector = unhashedPackets;
            this.rand = rand;
            this.hashAlgorithm = hashAlgorithm;

            keys.Add(new PgpSecretKey(certificationLevel, masterKey, id, encAlgorithm, hashAlgorithm, rawPassPhrase, false, useSha1, hashedPackets, unhashedPackets, rand));
        }

		/// <summary>Add a subkey to the key ring to be generated with default certification.</summary>
        public void AddSubKey(
            PgpKeyPair keyPair)
        {
			AddSubKey(keyPair, this.hashedPacketVector, this.unhashedPacketVector);
		}


        /// <summary>
        /// Add a subkey to the key ring to be generated with default certification.
        /// </summary>
        /// <param name="keyPair">The key pair.</param>
        /// <param name="hashAlgorithm">The hash algorithm.</param>
        public void AddSubKey(PgpKeyPair keyPair, HashAlgorithmTag hashAlgorithm)
        {
            this.AddSubKey(keyPair, this.hashedPacketVector, this.unhashedPacketVector, hashAlgorithm);
        }

		/// <summary>
		/// Add a subkey with specific hashed and unhashed packets associated with it and
		/// default certification.
		/// </summary>
		/// <param name="keyPair">Public/private key pair.</param>
		/// <param name="hashedPackets">Hashed packet values to be included in certification.</param>
		/// <param name="unhashedPackets">Unhashed packets values to be included in certification.</param>
		/// <exception cref="PgpException"></exception>
		public void AddSubKey(
			PgpKeyPair					keyPair,
			PgpSignatureSubpacketVector	hashedPackets,
			PgpSignatureSubpacketVector	unhashedPackets)
		{
			try
            {
                PgpSignatureGenerator sGen = new PgpSignatureGenerator(
					masterKey.PublicKey.Algorithm, HashAlgorithmTag.Sha1);

				//
                // Generate the certification
                //
                sGen.InitSign(PgpSignature.SubkeyBinding, masterKey.PrivateKey);

				sGen.SetHashedSubpackets(hashedPackets);
                sGen.SetUnhashedSubpackets(unhashedPackets);

				IList subSigs = Platform.CreateArrayList();

				subSigs.Add(sGen.GenerateCertification(masterKey.PublicKey, keyPair.PublicKey));

				keys.Add(new PgpSecretKey(keyPair.PrivateKey, new PgpPublicKey(keyPair.PublicKey, null, subSigs), encAlgorithm,
                    rawPassPhrase, false, useSha1, rand, false));
			}
            catch (PgpException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new PgpException("exception adding subkey: ", e);
            }
        }

        /// <summary>
        /// Add a subkey with specific hashed and unhashed packets associated with it and
        /// default certification.
        /// </summary>
        /// <param name="keyPair">Public/private key pair.</param>
        /// <param name="hashedPackets">Hashed packet values to be included in certification.</param>
        /// <param name="unhashedPackets">Unhashed packets values to be included in certification.</param>
        /// <param name="hashAlgorithm">The hash algorithm.</param>
        /// <exception cref="Org.BouncyCastle.Bcpg.OpenPgp.PgpException">exception adding subkey: </exception>
        /// <exception cref="PgpException"></exception>
        public void AddSubKey(
            PgpKeyPair keyPair,
            PgpSignatureSubpacketVector hashedPackets,
            PgpSignatureSubpacketVector unhashedPackets,
            HashAlgorithmTag hashAlgorithm)
        {
            try
            {
                PgpSignatureGenerator sGen = new PgpSignatureGenerator(masterKey.PublicKey.Algorithm, hashAlgorithm);

                //
                // Generate the certification
                //
                sGen.InitSign(PgpSignature.SubkeyBinding, masterKey.PrivateKey);

                sGen.SetHashedSubpackets(hashedPackets);
                sGen.SetUnhashedSubpackets(unhashedPackets);

                IList subSigs = Platform.CreateArrayList();
                subSigs.Add(sGen.GenerateCertification(masterKey.PublicKey, keyPair.PublicKey));

                keys.Add(new PgpSecretKey(keyPair.PrivateKey, new PgpPublicKey(keyPair.PublicKey, null, subSigs), encAlgorithm,
                    rawPassPhrase, false, useSha1, rand, false));
            }
            catch (PgpException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new PgpException("exception adding subkey: ", e);
            }
        }


		/// <summary>Return the secret key ring.</summary>
        public PgpSecretKeyRing GenerateSecretKeyRing()
        {
            return new PgpSecretKeyRing(keys);
        }

		/// <summary>Return the public key ring that corresponds to the secret key ring.</summary>
        public PgpPublicKeyRing GeneratePublicKeyRing()
        {
            IList pubKeys = Platform.CreateArrayList();

            IEnumerator enumerator = keys.GetEnumerator();
            enumerator.MoveNext();

			PgpSecretKey pgpSecretKey = (PgpSecretKey) enumerator.Current;
			pubKeys.Add(pgpSecretKey.PublicKey);

			while (enumerator.MoveNext())
            {
                pgpSecretKey = (PgpSecretKey) enumerator.Current;

				PgpPublicKey k = new PgpPublicKey(pgpSecretKey.PublicKey);
				k.publicPk = new PublicSubkeyPacket(
					k.Algorithm, k.CreationTime, k.publicPk.Key);

				pubKeys.Add(k);
			}

			return new PgpPublicKeyRing(pubKeys);
        }
    }
}
