using System;

namespace Org.BouncyCastle.Crypto.Tls
{
    public class TlsFatalAlertReceived
        : TlsException
    {
        private readonly byte alertDescription;

        public TlsFatalAlertReceived(byte alertDescription)
            : base(Tls.AlertDescription.GetText(alertDescription), null)
        {
            this.alertDescription = alertDescription;
        }

        public virtual byte AlertDescription
        {
            get { return alertDescription; }
        }
    }
}
