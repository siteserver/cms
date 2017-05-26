using System;
using System.Web;
using System.Web.Caching;
using System.Xml.Serialization;

namespace BaiRong.Core.Tabs
{
	/// <summary>
	/// TabCollection is a container for all of the tabs.
	/// </summary>
	[Serializable]
	public class TabCollection
	{
		private Tab[] _tabs;
		/// <summary>
		/// Property Tabs (Tab[])
		/// </summary>
		[XmlArray("Tabs")]
		public Tab[] Tabs
		{
			get {  return _tabs; }
			set {  _tabs = value; }
		}

		public TabCollection()
		{

		}

		public TabCollection(Tab[] tabs)
		{
			_tabs = tabs;
		}

		/// <summary>
		/// Returns the current instance of the TabCollection
		/// </summary>
		/// <returns></returns>
		public static TabCollection GetTabs(string filePath)
		{
		    if (filePath.StartsWith("/") || filePath.StartsWith("~"))
		    {
                filePath = HttpContext.Current.Server.MapPath(filePath);
            }

			var tc = CacheUtils.Get(filePath) as TabCollection;
		    if (tc != null) return tc;

		    tc = (TabCollection)Serializer.ConvertFileToObject(filePath,typeof(TabCollection));
		    CacheUtils.Max(filePath,tc,new CacheDependency(filePath));
		    return tc;
		}

        public static TabCollection GetTabsByPluginDirectory(string filePath, string directoryName)
        {
            if (filePath.StartsWith("/") || filePath.StartsWith("~"))
            {
                filePath = HttpContext.Current.Server.MapPath(filePath);
            }

            var tc = CacheUtils.Get(filePath) as TabCollection;
            if (tc != null) return tc;

            tc = (TabCollection)Serializer.ConvertFileToObject(filePath, typeof(TabCollection));

            var i = 0;
            foreach (var tab in tc.Tabs)
            {
                if (!string.IsNullOrEmpty(tab.Href) && !PageUtils.IsProtocolUrl(tab.Href))
                {
                    tab.Href = $"../sitefiles/plugins/{directoryName}/" + tab.Href;
                }
                if (!string.IsNullOrEmpty(tab.IconUrl) && !PageUtils.IsProtocolUrl(tab.IconUrl))
                {
                    tab.IconUrl = $"../sitefiles/plugins/{directoryName}/" + tab.IconUrl;
                }
                if (string.IsNullOrEmpty(tab.Id))
                {
                    tab.Id = directoryName + ++i;
                }
            }

            CacheUtils.Max(filePath, tc, new CacheDependency(filePath));
            return tc;
        }
    }
}
