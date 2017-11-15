using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Store;

namespace Org.BouncyCastle.Ocsp
{
	/// <remarks>
	/// <code>
	/// BasicOcspResponse ::= SEQUENCE {
	///		tbsResponseData		ResponseData,
	///		signatureAlgorithm	AlgorithmIdentifier,
	///		signature			BIT STRING,
	///		certs				[0] EXPLICIT SEQUENCE OF Certificate OPTIONAL
	/// }
	/// </code>
	/// </remarks>
	public class BasicOcspResp
		: X509ExtensionBase
	{
		private readonly BasicOcspResponse	resp;
		private readonly ResponseData		data;
//		private readonly X509Certificate[]	chain;

		public BasicOcspResp(
			BasicOcspResponse resp)
		{
			this.resp = resp;
			this.data = resp.TbsResponseData;
		}

		/// <returns>The DER encoding of the tbsResponseData field.</returns>
		/// <exception cref="OcspException">In the event of an encoding error.</exception>
		public byte[] GetTbsResponseData()
		{
			try
			{
				return data.GetDerEncoded();
			}
			catch (IOException e)
			{
				throw new OcspException("problem encoding tbsResponseData", e);
			}
		}

		public int Version
		{
			get { return data.Version.Value.IntValue + 1; }
		}

		public RespID ResponderId
		{
			get { return new RespID(data.ResponderID); }
		}

		public DateTime ProducedAt
		{
			get { return data.ProducedAt.ToDateTime(); }
		}

		public SingleResp[] Responses
		{
			get
			{
				Asn1Sequence s = data.Responses;
				SingleResp[] rs = new SingleResp[s.Count];

				for (int i = 0; i != rs.Length; i++)
				{
					rs[i] = new SingleResp(SingleResponse.GetInstance(s[i]));
				}

				return rs;
			}
		}

		public X509Extensions ResponseExtensions
		{
			get { return data.ResponseExtensions; }
		}

		protected override X509Extensions GetX509Extensions()
		{
			return ResponseExtensions;
		}

		public string SignatureAlgName
		{
            get { return OcspUtilities.GetAlgorithmName(resp.SignatureAlgorithm.Algorithm); }
		}

		public string SignatureAlgOid
		{
            get { return resp.SignatureAlgorithm.Algorithm.Id; }
		}

		[Obsolete("RespData class is no longer required as all functionality is available on this class")]
		public RespData GetResponseData()
		{
			return new RespData(data);
		}

		public byte[] GetSignature()
		{
			return resp.GetSignatureOctets();
		}

		private IList GetCertList()
		{
			// load the certificates and revocation lists if we have any

			IList certs = Platform.CreateArrayList();
			Asn1Sequence s = resp.Certs;

			if (s != null)
			{
				foreach (Asn1Encodable ae in s)
				{
					try
					{
						certs.Add(new X509CertificateParser().ReadCertificate(ae.GetEncoded()));
					}
					catch (IOException ex)
					{
						throw new OcspException("can't re-encode certificate!", ex);
					}
					catch (CertificateException ex)
					{
						throw new OcspException("can't re-encode certificate!", ex);
					}
				}
			}

			return certs;
		}

		public X509Certificate[] GetCerts()
		{
			IList certs = GetCertList();
            X509Certificate[] result = new X509Certificate[certs.Count];
            for (int i = 0; i < certs.Count; ++i)
            {
                result[i] = (X509Certificate)certs[i];
            }
            return result;
		}

		/// <returns>The certificates, if any, associated with the response.</returns>
		/// <exception cref="OcspException">In the event of an encoding error.</exception>
		public IX509Store GetCertificates(
			string type)
		{
			try
			{
				return X509StoreFactory.Create(
					"Certificate/" + type,
					new X509CollectionStoreParameters(this.GetCertList()));
			}
			catch (Exception e)
			{
				throw new OcspException("can't setup the CertStore", e);
			}
		}

		/// <summary>
		/// Verify the signature against the tbsResponseData object we contain.
		/// </summary>
		public bool Verify(
			AsymmetricKeyParameter publicKey)
		{
			try
			{
				ISigner signature = SignerUtilities.GetSigner(this.SignatureAlgName);
				signature.Init(false, publicKey);
				byte[] bs = data.GetDerEncoded();
				signature.BlockUpdate(bs, 0, bs.Length);

				return signature.VerifySignature(this.GetSignature());
			}
			catch (Exception e)
			{
				throw new OcspException("exception processing sig: " + e, e);
			}
		}

		/// <returns>The ASN.1 encoded representation of this object.</returns>
		public byte[] GetEncoded()
		{
			return resp.GetEncoded();
		}

		public override bool Equals(
			object obj)
		{
			if (obj == this)
				return true;

			BasicOcspResp other = obj as BasicOcspResp;

			if (other == null)
				return false;

			return resp.Equals(other.resp);
		}

		public override int GetHashCode()
		{
			return resp.GetHashCode();
		}
	}
}
