using System;

using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls
{
    public class BasicTlsPskIdentity
        : TlsPskIdentity
    {
        protected byte[] mIdentity;
        protected byte[] mPsk;

        public BasicTlsPskIdentity(byte[] identity, byte[] psk)
        {
            this.mIdentity = Arrays.Clone(identity);
            this.mPsk = Arrays.Clone(psk);
        }

        public BasicTlsPskIdentity(string identity, byte[] psk)
        {
            this.mIdentity = Strings.ToUtf8ByteArray(identity);
            this.mPsk = Arrays.Clone(psk);
        }

        public virtual void SkipIdentityHint()
        {
        }

        public virtual void NotifyIdentityHint(byte[] psk_identity_hint)
        {
        }

        public virtual byte[] GetPskIdentity()
        {
            return mIdentity;
        }

        public virtual byte[] GetPsk()
        {
            return mPsk;
        }
    }
}
