namespace SiteServer.Utils
{
    public class Pair
    {
        public Pair(string key, object value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; }

        public object Value { get; }
    }
}
