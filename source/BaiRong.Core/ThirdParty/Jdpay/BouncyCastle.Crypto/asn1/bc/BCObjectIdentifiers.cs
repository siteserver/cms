using System;

namespace Org.BouncyCastle.Asn1.BC
{
	public abstract class BCObjectIdentifiers
	{
		// iso.org.dod.internet.private.enterprise.legion-of-the-bouncy-castle
		// 1.3.6.1.4.1.22554
		public static readonly DerObjectIdentifier bc = new DerObjectIdentifier("1.3.6.1.4.1.22554");

		// pbe(1) algorithms
		public static readonly DerObjectIdentifier bc_pbe = new DerObjectIdentifier(bc + ".1");

		// SHA-1(1)
		public static readonly DerObjectIdentifier bc_pbe_sha1 = new DerObjectIdentifier(bc_pbe + ".1");

		// SHA-2(2) . (SHA-256(1)|SHA-384(2)|SHA-512(3)|SHA-224(4))
		public static readonly DerObjectIdentifier bc_pbe_sha256 = new DerObjectIdentifier(bc_pbe + ".2.1");
		public static readonly DerObjectIdentifier bc_pbe_sha384 = new DerObjectIdentifier(bc_pbe + ".2.2");
		public static readonly DerObjectIdentifier bc_pbe_sha512 = new DerObjectIdentifier(bc_pbe + ".2.3");
		public static readonly DerObjectIdentifier bc_pbe_sha224 = new DerObjectIdentifier(bc_pbe + ".2.4");
		
		// PKCS-5(1)|PKCS-12(2)
		public static readonly DerObjectIdentifier bc_pbe_sha1_pkcs5 = new DerObjectIdentifier(bc_pbe_sha1 + ".1");
		public static readonly DerObjectIdentifier bc_pbe_sha1_pkcs12 = new DerObjectIdentifier(bc_pbe_sha1 + ".2");
		
		public static readonly DerObjectIdentifier bc_pbe_sha256_pkcs5 = new DerObjectIdentifier(bc_pbe_sha256 + ".1");
		public static readonly DerObjectIdentifier bc_pbe_sha256_pkcs12 = new DerObjectIdentifier(bc_pbe_sha256 + ".2");

		// AES(1) . (CBC-128(2)|CBC-192(22)|CBC-256(42))
		public static readonly DerObjectIdentifier bc_pbe_sha1_pkcs12_aes128_cbc = new DerObjectIdentifier(bc_pbe_sha1_pkcs12 + ".1.2");
		public static readonly DerObjectIdentifier bc_pbe_sha1_pkcs12_aes192_cbc = new DerObjectIdentifier(bc_pbe_sha1_pkcs12 + ".1.22");
		public static readonly DerObjectIdentifier bc_pbe_sha1_pkcs12_aes256_cbc = new DerObjectIdentifier(bc_pbe_sha1_pkcs12 + ".1.42");

		public static readonly DerObjectIdentifier bc_pbe_sha256_pkcs12_aes128_cbc = new DerObjectIdentifier(bc_pbe_sha256_pkcs12 + ".1.2");
		public static readonly DerObjectIdentifier bc_pbe_sha256_pkcs12_aes192_cbc = new DerObjectIdentifier(bc_pbe_sha256_pkcs12 + ".1.22");
		public static readonly DerObjectIdentifier bc_pbe_sha256_pkcs12_aes256_cbc = new DerObjectIdentifier(bc_pbe_sha256_pkcs12 + ".1.42");
	}
}