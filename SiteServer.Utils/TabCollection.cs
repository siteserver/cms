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
    }
}
