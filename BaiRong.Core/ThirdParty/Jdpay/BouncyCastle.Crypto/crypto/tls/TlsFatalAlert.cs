using System;

namespace Org.BouncyCastle.Crypto.Tls
{
    public class TlsFatalAlert
        : TlsException
    {
        private readonly byte alertDescription;

        public TlsFatalAlert(byte alertDescription)
            : this(alertDescription, null)
        {
        }

        public TlsFatalAlert(byte alertDescription, Exception alertCause)
            : base(Tls.AlertDescription.GetText(alertDescription), alertCause)
        {
            this.alertDescription = alertDescription;
        }

        public virtual byte AlertDescription
        {
            get { return alertDescription; }
        }
    }
}
