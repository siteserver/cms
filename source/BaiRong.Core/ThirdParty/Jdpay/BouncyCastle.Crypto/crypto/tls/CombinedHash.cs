using System;

using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Tls
{
    /**
     * A combined hash, which implements md5(m) || sha1(m).
     */
    internal class CombinedHash
        :   TlsHandshakeHash
    {
        protected TlsContext mContext;
        protected IDigest mMd5;
        protected IDigest mSha1;

        internal CombinedHash()
        {
            this.mMd5 = TlsUtilities.CreateHash(HashAlgorithm.md5);
            this.mSha1 = TlsUtilities.CreateHash(HashAlgorithm.sha1);
        }

        internal CombinedHash(CombinedHash t)
        {
            this.mContext = t.mContext;
            this.mMd5 = TlsUtilities.CloneHash(HashAlgorithm.md5, t.mMd5);
            this.mSha1 = TlsUtilities.CloneHash(HashAlgorithm.sha1, t.mSha1);
        }

        public virtual void Init(TlsContext context)
        {
            this.mContext = context;
        }

        public virtual TlsHandshakeHash NotifyPrfDetermined()
        {
            return this;
        }

        public virtual void TrackHashAlgorithm(byte hashAlgorithm)
        {
            throw new InvalidOperationException("CombinedHash only supports calculating the legacy PRF for handshake hash");
        }

        public virtual void SealHashAlgorithms()
        {
        }

        public virtual TlsHandshakeHash StopTracking()
        {
            return new CombinedHash(this);
        }

        public virtual IDigest ForkPrfHash()
        {
            return new CombinedHash(this);
        }

        public virtual byte[] GetFinalHash(byte hashAlgorithm)
        {
            throw new InvalidOperationException("CombinedHash doesn't support multiple hashes");
        }

        public virtual string AlgorithmName
        {
            get { return mMd5.AlgorithmName + " and " + mSha1.AlgorithmName; }
        }

        public virtual int GetByteLength()
        {
            return System.Math.Max(mMd5.GetByteLength(), mSha1.GetByteLength());
        }

        public virtual int GetDigestSize()
        {
            return mMd5.GetDigestSize() + mSha1.GetDigestSize();
        }

        public virtual void Update(byte input)
        {
            mMd5.Update(input);
            mSha1.Update(input);
        }

        /**
         * @see org.bouncycastle.crypto.Digest#update(byte[], int, int)
         */
        public virtual void BlockUpdate(byte[] input, int inOff, int len)
        {
            mMd5.BlockUpdate(input, inOff, len);
            mSha1.BlockUpdate(input, inOff, len);
        }

        /**
         * @see org.bouncycastle.crypto.Digest#doFinal(byte[], int)
         */
        public virtual int DoFinal(byte[] output, int outOff)
        {
            if (mContext != null && TlsUtilities.IsSsl(mContext))
            {
                Ssl3Complete(mMd5, Ssl3Mac.IPAD, Ssl3Mac.OPAD, 48);
                Ssl3Complete(mSha1, Ssl3Mac.IPAD, Ssl3Mac.OPAD, 40);
            }

            int i1 = mMd5.DoFinal(output, outOff);
            int i2 = mSha1.DoFinal(output, outOff + i1);
            return i1 + i2;
        }

        /**
         * @see org.bouncycastle.crypto.Digest#reset()
         */
        public virtual void Reset()
        {
            mMd5.Reset();
            mSha1.Reset();
        }

        protected virtual void Ssl3Complete(IDigest d, byte[] ipad, byte[] opad, int padLength)
        {
            byte[] master_secret = mContext.SecurityParameters.masterSecret;

            d.BlockUpdate(master_secret, 0, master_secret.Length);
            d.BlockUpdate(ipad, 0, padLength);

            byte[] tmp = DigestUtilities.DoFinal(d);

            d.BlockUpdate(master_secret, 0, master_secret.Length);
            d.BlockUpdate(opad, 0, padLength);
            d.BlockUpdate(tmp, 0, tmp.Length);
        }
    }
}
