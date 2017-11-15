using System;

using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls
{
    internal class TlsSessionImpl
        :   TlsSession
    {
        internal readonly byte[] mSessionID;
        internal SessionParameters mSessionParameters;

        internal TlsSessionImpl(byte[] sessionID, SessionParameters sessionParameters)
        {
            if (sessionID == null)
                throw new ArgumentNullException("sessionID");
            if (sessionID.Length < 1 || sessionID.Length > 32)
                throw new ArgumentException("must have length between 1 and 32 bytes, inclusive", "sessionID");

            this.mSessionID = Arrays.Clone(sessionID);
            this.mSessionParameters = sessionParameters;
        }

        public virtual SessionParameters ExportSessionParameters()
        {
            lock (this)
            {
                return this.mSessionParameters == null ? null : this.mSessionParameters.Copy();
            }
        }

        public virtual byte[] SessionID
        {
            get { lock (this) return mSessionID; }
        }

        public virtual void Invalidate()
        {
            lock (this)
            {
                if (this.mSessionParameters != null)
                {
                    this.mSessionParameters.Clear();
                    this.mSessionParameters = null;
                }
            }
        }

        public virtual bool IsResumable
        {
            get { lock (this) return this.mSessionParameters != null; }
        }
    }
}
