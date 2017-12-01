using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls
{
    public sealed class SessionParameters
    {
        public sealed class Builder
        {
            private int mCipherSuite = -1;
            private short mCompressionAlgorithm = -1;
            private byte[] mMasterSecret = null;
            private Certificate mPeerCertificate = null;
            private byte[] mPskIdentity = null;
            private byte[] mSrpIdentity = null;
            private byte[] mEncodedServerExtensions = null;

            public Builder()
            {
            }

            public SessionParameters Build()
            {
                Validate(this.mCipherSuite >= 0, "cipherSuite");
                Validate(this.mCompressionAlgorithm >= 0, "compressionAlgorithm");
                Validate(this.mMasterSecret != null, "masterSecret");
                return new SessionParameters(mCipherSuite, (byte)mCompressionAlgorithm, mMasterSecret, mPeerCertificate,
                    mPskIdentity, mSrpIdentity, mEncodedServerExtensions);
            }

            public Builder SetCipherSuite(int cipherSuite)
            {
                this.mCipherSuite = cipherSuite;
                return this;
            }

            public Builder SetCompressionAlgorithm(byte compressionAlgorithm)
            {
                this.mCompressionAlgorithm = compressionAlgorithm;
                return this;
            }

            public Builder SetMasterSecret(byte[] masterSecret)
            {
                this.mMasterSecret = masterSecret;
                return this;
            }

            public Builder SetPeerCertificate(Certificate peerCertificate)
            {
                this.mPeerCertificate = peerCertificate;
                return this;
            }

            public Builder SetPskIdentity(byte[] pskIdentity)
            {
                this.mPskIdentity = pskIdentity;
                return this;
            }

            public Builder SetSrpIdentity(byte[] srpIdentity)
            {
                this.mSrpIdentity = srpIdentity;
                return this;
            }

            public Builder SetServerExtensions(IDictionary serverExtensions)
            {
                if (serverExtensions == null)
                {
                    mEncodedServerExtensions = null;
                }
                else
                {
                    MemoryStream buf = new MemoryStream();
                    TlsProtocol.WriteExtensions(buf, serverExtensions);
                    mEncodedServerExtensions = buf.ToArray();
                }
                return this;
            }

            private void Validate(bool condition, string parameter)
            {
                if (!condition)
                    throw new InvalidOperationException("Required session parameter '" + parameter + "' not configured");
            }
        }

        private int mCipherSuite;
        private byte mCompressionAlgorithm;
        private byte[] mMasterSecret;
        private Certificate mPeerCertificate;
        private byte[] mPskIdentity;
        private byte[] mSrpIdentity;
        private byte[] mEncodedServerExtensions;

        private SessionParameters(int cipherSuite, byte compressionAlgorithm, byte[] masterSecret,
            Certificate peerCertificate, byte[] pskIdentity, byte[] srpIdentity, byte[] encodedServerExtensions)
        {
            this.mCipherSuite = cipherSuite;
            this.mCompressionAlgorithm = compressionAlgorithm;
            this.mMasterSecret = Arrays.Clone(masterSecret);
            this.mPeerCertificate = peerCertificate;
            this.mPskIdentity = Arrays.Clone(pskIdentity);
            this.mSrpIdentity = Arrays.Clone(srpIdentity);
            this.mEncodedServerExtensions = encodedServerExtensions;
        }

        public void Clear()
        {
            if (this.mMasterSecret != null)
            {
                Arrays.Fill(this.mMasterSecret, (byte)0);
            }
        }

        public SessionParameters Copy()
        {
            return new SessionParameters(mCipherSuite, mCompressionAlgorithm, mMasterSecret, mPeerCertificate,
                mPskIdentity, mSrpIdentity, mEncodedServerExtensions);
        }

        public int CipherSuite
        {
            get { return mCipherSuite; }
        }

        public byte CompressionAlgorithm
        {
            get { return mCompressionAlgorithm; }
        }

        public byte[] MasterSecret
        {
            get { return mMasterSecret; }
        }

        public Certificate PeerCertificate
        {
            get { return mPeerCertificate; }
        }

        public byte[] PskIdentity
        {
            get { return mPskIdentity; }
        }

        public byte[] SrpIdentity
        {
            get { return mSrpIdentity; }
        }

        public IDictionary ReadServerExtensions()
        {
            if (mEncodedServerExtensions == null)
                return null;

            MemoryStream buf = new MemoryStream(mEncodedServerExtensions, false);
            return TlsProtocol.ReadExtensions(buf);
        }
    }
}
