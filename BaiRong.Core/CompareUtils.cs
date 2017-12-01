using System.Collections;

namespace BaiRong.Core
{
	public class CompareUtils
	{
        public static bool Contains(string strCollection, string item)
        {
            var contains = false;
            if (!string.IsNullOrEmpty(strCollection) && !string.IsNullOrEmpty(item))
            {
                var list = TranslateUtils.StringCollectionToStringList(strCollection);
                contains = list.Contains(item.Trim());
            }
            return contains;
        }

        public static bool ContainsInt(ICollection collection, int item)
        {
            var contains = false;
            if (collection != null && collection.Count != 0)
            {
                foreach (int i in collection)
                {
                    if (i == item)
                    {
                        contains = true;
                        break;
                    }
                }
            }
            return contains;
        }
	}
}
