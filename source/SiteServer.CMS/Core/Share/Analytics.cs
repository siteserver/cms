namespace SiteServer.CMS.Core.Share
{
   public class Analytics
    {
        private int _count;
        public int Count
        {
            get
            {
                return _count;
            }
            set
            {
                _count = value;
            }
        }
        private string _metric;
        public string Metric
        {
            get
            {
                return _metric;
            }
            set
            {
                _metric = value;
            }
        }
    }
}
