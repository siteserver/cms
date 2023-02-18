namespace SSCMS.Dto
{
    public class SyncTask<T> where T : class, new()
    {
        public SyncTask(int siteId, string key, string filePath, string md5, T options)
        {
            SiteId = siteId;
            Key = key;
            FilePath = filePath;
            Md5 = md5;
            Executing = false;
            Options = options;
        }

        public int SiteId { get; set; }
        public string Key { get; set; }
        public string FilePath { get; set; }
        public string Md5 { get; set; }
        public T Options { get; set; }
        public bool Executing { get; set; }
    }
}
