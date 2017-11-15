using System;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
    public class DtlsTransport
        :   DatagramTransport
    {
        private readonly DtlsRecordLayer mRecordLayer;

        internal DtlsTransport(DtlsRecordLayer recordLayer)
        {
            this.mRecordLayer = recordLayer;
        }

        public virtual int GetReceiveLimit()
        {
            return mRecordLayer.GetReceiveLimit();
        }

        public virtual int GetSendLimit()
        {
            return mRecordLayer.GetSendLimit();
        }

        public virtual int Receive(byte[] buf, int off, int len, int waitMillis)
        {
            try
            {
                return mRecordLayer.Receive(buf, off, len, waitMillis);
            }
            catch (TlsFatalAlert fatalAlert)
            {
                mRecordLayer.Fail(fatalAlert.AlertDescription);
                throw fatalAlert;
            }
            catch (IOException e)
            {
                mRecordLayer.Fail(AlertDescription.internal_error);
                throw e;
            }
            catch (Exception e)
            {
                mRecordLayer.Fail(AlertDescription.internal_error);
                throw new TlsFatalAlert(AlertDescription.internal_error, e);
            }
        }

        public virtual void Send(byte[] buf, int off, int len)
        {
            try
            {
                mRecordLayer.Send(buf, off, len);
            }
            catch (TlsFatalAlert fatalAlert)
            {
                mRecordLayer.Fail(fatalAlert.AlertDescription);
                throw fatalAlert;
            }
            catch (IOException e)
            {
                mRecordLayer.Fail(AlertDescription.internal_error);
                throw e;
            }
            catch (Exception e)
            {
                mRecordLayer.Fail(AlertDescription.internal_error);
                throw new TlsFatalAlert(AlertDescription.internal_error, e);
            }
        }

        public virtual void Close()
        {
            mRecordLayer.Close();
        }
    }
}
