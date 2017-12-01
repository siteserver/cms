using System;
using System.Collections;

using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls
{
    class DtlsReassembler
    {
        private readonly byte mMsgType;
        private readonly byte[] mBody;

        private readonly IList mMissing = Platform.CreateArrayList();

        internal DtlsReassembler(byte msg_type, int length)
        {
            this.mMsgType = msg_type;
            this.mBody = new byte[length];
            this.mMissing.Add(new Range(0, length));
        }

        internal byte MsgType
        {
            get { return mMsgType; }
        }

        internal byte[] GetBodyIfComplete()
        {
            return mMissing.Count == 0 ? mBody : null;
        }

        internal void ContributeFragment(byte msg_type, int length, byte[] buf, int off, int fragment_offset,
            int fragment_length)
        {
            int fragment_end = fragment_offset + fragment_length;

            if (this.mMsgType != msg_type || this.mBody.Length != length || fragment_end > length)
            {
                return;
            }

            if (fragment_length == 0)
            {
                // NOTE: Empty messages still require an empty fragment to complete it
                if (fragment_offset == 0 && mMissing.Count > 0)
                {
                    Range firstRange = (Range)mMissing[0];
                    if (firstRange.End == 0)
                    {
                        mMissing.RemoveAt(0);
                    }
                }
                return;
            }

            for (int i = 0; i < mMissing.Count; ++i)
            {
                Range range = (Range)mMissing[i];
                if (range.Start >= fragment_end)
                {
                    break;
                }
                if (range.End > fragment_offset)
                {

                    int copyStart = System.Math.Max(range.Start, fragment_offset);
                    int copyEnd = System.Math.Min(range.End, fragment_end);
                    int copyLength = copyEnd - copyStart;

                    Array.Copy(buf, off + copyStart - fragment_offset, mBody, copyStart,
                        copyLength);

                    if (copyStart == range.Start)
                    {
                        if (copyEnd == range.End)
                        {
                            mMissing.RemoveAt(i--);
                        }
                        else
                        {
                            range.Start = copyEnd;
                        }
                    }
                    else
                    {
                        if (copyEnd != range.End)
                        {
                            mMissing.Insert(++i, new Range(copyEnd, range.End));
                        }
                        range.End = copyStart;
                    }
                }
            }
        }

        internal void Reset()
        {
            this.mMissing.Clear();
            this.mMissing.Add(new Range(0, mBody.Length));
        }

        private class Range
        {
            private int mStart, mEnd;

            internal Range(int start, int end)
            {
                this.mStart = start;
                this.mEnd = end;
            }

            public int Start
            {
                get { return mStart; }
                set { this.mStart = value; }
            }

            public int End
            {
                get { return mEnd; }
                set { this.mEnd = value; }
            }
        }
    }
}
