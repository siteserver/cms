using System;
using System.Xml.Serialization;


namespace SS.CMS.Abstractions
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
