using System;

namespace Org.BouncyCastle.Crypto.Tls
{
    public class SupplementalDataEntry
    {
        protected readonly int mDataType;
        protected readonly byte[] mData;

        public SupplementalDataEntry(int dataType, byte[] data)
        {
            this.mDataType = dataType;
            this.mData = data;
        }

        public virtual int DataType
        {
            get { return mDataType; }
        }

        public virtual byte[] Data
        {
            get { return mData; }
        }
    }
}
