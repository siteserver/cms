using System;
using System.Web;
using System.Xml.Serialization;

namespace SiteServer.Utils
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
		    CacheUtils.Insert(filePath, tc, filePath);
		    return tc;
		}
    }
}
