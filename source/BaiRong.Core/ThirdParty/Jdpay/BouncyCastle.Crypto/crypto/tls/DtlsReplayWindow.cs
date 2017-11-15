using System;

namespace Org.BouncyCastle.Crypto.Tls
{
    /**
     * RFC 4347 4.1.2.5 Anti-replay
     * <p/>
     * Support fast rejection of duplicate records by maintaining a sliding receive window
     */
    internal class DtlsReplayWindow
    {
        private const long VALID_SEQ_MASK = 0x0000FFFFFFFFFFFFL;

        private const long WINDOW_SIZE = 64L;

        private long mLatestConfirmedSeq = -1;
        private long mBitmap = 0;

        /**
         * Check whether a received record with the given sequence number should be rejected as a duplicate.
         *
         * @param seq the 48-bit DTLSPlainText.sequence_number field of a received record.
         * @return true if the record should be discarded without further processing.
         */
        internal bool ShouldDiscard(long seq)
        {
            if ((seq & VALID_SEQ_MASK) != seq)
                return true;

            if (seq <= mLatestConfirmedSeq)
            {
                long diff = mLatestConfirmedSeq - seq;
                if (diff >= WINDOW_SIZE)
                    return true;
                if ((mBitmap & (1L << (int)diff)) != 0)
                    return true;
            }

            return false;
        }

        /**
         * Report that a received record with the given sequence number passed authentication checks.
         *
         * @param seq the 48-bit DTLSPlainText.sequence_number field of an authenticated record.
         */
        internal void ReportAuthenticated(long seq)
        {
            if ((seq & VALID_SEQ_MASK) != seq)
                throw new ArgumentException("out of range", "seq");

            if (seq <= mLatestConfirmedSeq)
            {
                long diff = mLatestConfirmedSeq - seq;
                if (diff < WINDOW_SIZE)
                {
                    mBitmap |= (1L << (int)diff);
                }
            }
            else
            {
                long diff = seq - mLatestConfirmedSeq;
                if (diff >= WINDOW_SIZE)
                {
                    mBitmap = 1;
                }
                else
                {
                    mBitmap <<= (int)diff;
                    mBitmap |= 1;
                }
                mLatestConfirmedSeq = seq;
            }
        }

        /**
         * When a new epoch begins, sequence numbers begin again at 0
         */
        internal void Reset()
        {
            mLatestConfirmedSeq = -1;
            mBitmap = 0;
        }
    }
}
