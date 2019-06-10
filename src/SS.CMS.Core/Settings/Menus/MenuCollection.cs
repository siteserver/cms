using System;
using System.Xml.Serialization;

namespace SS.CMS.Core.Settings.Menus
{
	/// <summary>
	/// TabCollection is a container for all of the tabs.
	/// </summary>
	[Serializable]
	public class MenuCollection
	{
		private Menu[] _menus;
		/// <summary>
		/// Property Tabs (Tab[])
		/// </summary>
		[XmlArray("Menus")]
		public Menu[] Menus
		{
			get {  return _menus; }
			set {  _menus = value; }
		}

		public MenuCollection()
		{

		}

		public MenuCollection(Menu[] menus)
		{
			_menus = menus;
		}
    }
}
