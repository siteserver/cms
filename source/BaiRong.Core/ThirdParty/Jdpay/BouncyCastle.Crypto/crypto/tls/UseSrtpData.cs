using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
    /**
     * RFC 5764 4.1.1
     */
    public class UseSrtpData
    {
        protected readonly int[] mProtectionProfiles;
        protected readonly byte[] mMki;

        /**
         * @param protectionProfiles see {@link SrtpProtectionProfile} for valid constants.
         * @param mki                valid lengths from 0 to 255.
         */
        public UseSrtpData(int[] protectionProfiles, byte[] mki)
        {
            if (protectionProfiles == null || protectionProfiles.Length < 1
                || protectionProfiles.Length >= (1 << 15))
            {
                throw new ArgumentException("must have length from 1 to (2^15 - 1)", "protectionProfiles");
            }

            if (mki == null)
            {
                mki = TlsUtilities.EmptyBytes;
            }
            else if (mki.Length > 255)
            {
                throw new ArgumentException("cannot be longer than 255 bytes", "mki");
            }

            this.mProtectionProfiles = protectionProfiles;
            this.mMki = mki;
        }

        /**
         * @return see {@link SrtpProtectionProfile} for valid constants.
         */
        public virtual int[] ProtectionProfiles
        {
            get { return mProtectionProfiles; }
        }

        /**
         * @return valid lengths from 0 to 255.
         */
        public virtual byte[] Mki
        {
            get { return mMki; }
        }
    }
}
