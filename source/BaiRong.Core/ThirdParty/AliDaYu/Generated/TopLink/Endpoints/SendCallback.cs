using System;
using System.Collections.Generic;
using System.Threading;

namespace Taobao.Top.Link.Endpoints
{
    public class SendCallback
    {
        private EventWaitHandle _handle;
        private Exception _error;
        private IDictionary<string, object> _return;

        /// <summary>get which endpoint to send
        /// </summary>
        public EndpointProxy Target { get; private set; }
        /// <summary>get error in sending
        /// </summary>
        public Exception Error
        {
            get { return this._error; }
            internal set
            {
                this._error = value;
                this._handle.Set();
            }
        }
        /// <summary>get reply
        /// </summary>
        public IDictionary<string, object> Return
        {
            get { return this._return; }
            internal set
            {
                this._return = value;
                this._handle.Set();
            }
        }

        public SendCallback(EndpointProxy endpointProxy)
        {
            this.Target = endpointProxy;
            this._handle = new EventWaitHandle(false, EventResetMode.AutoReset);
        }
        /// <summary>wait until got return message
        /// </summary>
        /// <param name="timeout">timeout in milliseconds</param>
        public void WaitReturn(int timeout)
        {
            if (timeout > 0)
            {
                if (!this._handle.WaitOne(timeout, false))
                    throw new LinkException(Text.E_EXECUTE_TIMEOUT);
            }
            else
                this._handle.WaitOne();
        }
    }
}