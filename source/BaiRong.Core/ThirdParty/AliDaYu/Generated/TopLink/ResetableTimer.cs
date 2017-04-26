using System;
using System.Threading;

namespace Taobao.Top.Link
{
    /// <summary>easy timer impl
    /// </summary>
    public class ResetableTimer
    {
        //min=50ms by .net impl
        private Timer _timer;
        private int _periodMillisecond;
        public event EventHandler Elapsed;

        public ResetableTimer(int periodMillisecond)
        {
            this._periodMillisecond = periodMillisecond;
            this._timer = new Timer(o =>
            {
                if (Elapsed != null)
                {
                    this.Elapsed(null, null);
                }
            }, null
            , this._periodMillisecond
            , this._periodMillisecond);
        }
        /// <summary>cancel timer
        /// </summary>
        public void Cancel()
        {
            if (this._timer == null)
                return;
            this._timer.Dispose();
            this._timer = null;
        }
        /// <summary>delay in period
        /// </summary>
        public void Delay()
        {
            this._timer.Change(this._periodMillisecond, this._periodMillisecond);
        }
    }
}