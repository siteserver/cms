using System.Collections.Specialized;

namespace BaiRong.Core.Model
{
	public class LowerNameValueCollection : NameValueCollection
	{
        public override void Add(string name, string value)
        {
            if (!string.IsNullOrEmpty(name))
            {
                base.Add(name.ToLower(), value);
            }
        }

        public override void Set(string name, string value)
        {
            if (!string.IsNullOrEmpty(name))
            {
                base.Set(name.ToLower(), value);
            }
        }

        public override string Get(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                return base.Get(name.ToLower());
            }
            return null;
        }

        public override void Remove(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                base.Remove(name.ToLower());
            }
        }

        public override string[] GetValues(string name)
        {
            return !string.IsNullOrEmpty(name) ? base.GetValues(name.ToLower()) : null;
        }
	}
}
